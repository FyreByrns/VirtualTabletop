using System.Net;
using System.Net.Sockets;
using VTTGTK.Messages;
using VTTGTK.Messages;

namespace VTTGTK;

static class ServerManager {
	static Random RNG = new();
	static Socket Server;

	static List<Connection> Clients = new();
	static Connection[] SNAPSHOT_Clients => Clients.ToArray();
	static HashSet<int> UsedIDs = new();
	static Dictionary<int, Connection> Connections = new();

	static LobbyState LobbyState = new();

	static async void AddConnection(Socket client) {
		int id = RNG.Next(int.MaxValue);
		while (UsedIDs.Contains(id)) {
			id = RNG.Next(int.MaxValue);
		}

		byte[] buf = new byte[MessageBufferSize];
		int initialRcvLength = await client.ReceiveAsync(buf);

		Connection connection = new(client, id);
		Task _ = connection.Listen();

		Clients.Add(connection);
		Connections[connection.ID] = connection;

		SendTo(new IDMessage(connection.ID), connection.ID);

		Message initialMessage = Message.Parse(buf, initialRcvLength);

		if (initialMessage is InfoResponseMessage irm && irm.RequestType == InfoType.Name) {
			if (NameAllowed(irm.Contents)) {
				SetName(connection, irm.Contents);
			}
		}
	}
	static void RemoveConnection(Connection connection) {
		Clients.Remove(connection);
		Connections.Remove(connection.ID);
		UsedIDs.Remove(connection.ID);
	}

	static bool NameAllowed(string attempt) {
		// test against all other names
		foreach (var connection in SNAPSHOT_Clients) {
			if (connection.Name == attempt) {
				return false;
			}
		}

		Console.WriteLine($"{attempt} accepted");
		return true;
	}
	static void SetName(Connection client, string name) {
		client.Name = name;
		client.State = ConnectionState.AwaitingLobbyInfo;
		SendTo(new InfoResponseMessage(InfoType.Name, name), client.ID);
	}

	public static void Update() {
		foreach (var connection in SNAPSHOT_Clients) {
			// update client

			switch (connection.State) {
				case ConnectionState.AwaitingLobbyInfo: {
					SendTo(new LobbyStateMessage(LobbyState), connection.ID);
					connection.State = ConnectionState.Normal;
					break;
				}
				case ConnectionState.Normal: {
					break;
				}

				default: { break; }
			}
		}
	}

	static async Task ListenerTask() {
		while (true) {
			var client = await Server.AcceptAsync();
			Console.WriteLine($"socket connected: {client.RemoteEndPoint}");
			AddConnection(client);

			Update();

			await Task.Yield();
		}
	}

	public static void StartServer(string port) {
		IPEndPoint ep = new(IPAddress.Any, int.Parse(port));
		Server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		Server.Bind(ep);
		Server.Listen();

		Connection.OnDataSent += DataReceived;

		Task _ = ListenerTask();
		Console.WriteLine("listener started");
	}

	private static void DataReceived(Connection client, byte[] buffer, int received) {
		int id = client.ID;
		var message = Message.Parse(buffer, received);

		Console.WriteLine($"s rcv {received} bytes from [{client.Name}:{id}]({client.State})");

		switch (client.State) {
			case ConnectionState.Normal:
			case ConnectionState.AwaitingLobbyInfo: {
				switch (message.Type) {
					case MessageType.TokenMove: {
						if (message is TokenMovedMessage tmm) {
							SendToAll(tmm);
						}

						break;
					}
				}

				break;
			}

			default: { break; }
		}
	}

	public static void StopServer() {
		SendToAll(new BlankMessage(MessageType.Disconnecting));

		Server?.Close();
		Server?.Dispose();
	}

	public static void SendTo(Message msg, int id) {
		try {
			var client = Connections[id];
			if (client.PollFailed()) {
				Console.WriteLine($"s poll failed for {id}");
			}
			else {
				client.Remote.SendAsync(msg.GetBytes());
			}
		}
		catch (Exception e) {
			Console.WriteLine($"error sending data to {id}: {e}");
		}
	}
	public static void SendTo(Message msg, params int[] ids) {
		foreach (int id in ids) {
			SendTo(msg, id);
		}
	}
	public static void SendToAll(Message msg) {
		List<Connection> disconnectedClients = new();

		foreach (var client in SNAPSHOT_Clients) {
			try {
				// this may fail if polling and a connection is pending, warning
				if (client.PollFailed()) {
					Console.WriteLine($"error sending data to {client.Remote.RemoteEndPoint}: poll failed, disconnecting.");
					disconnectedClients.Add(client);
				}

				client.Remote.SendAsync(msg.GetBytes());
			}
			catch (Exception ex) {
				Console.WriteLine($"error sending data to {client.Remote.RemoteEndPoint}: {ex}, disconnecting.");
				disconnectedClients.Add(client);
			}
		}

		foreach (var client in disconnectedClients) {
			RemoveConnection(client);
		}
	}
}
