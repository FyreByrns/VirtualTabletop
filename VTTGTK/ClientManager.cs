using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VTTGTK.Messages;

namespace VTTGTK;

static class ClientManager {
	public delegate void ClientMessageReceived(byte[] data);
	public static event ClientMessageReceived OnClientMessageReceived;

	#region local state

	public static VTTApp Instance { get; private set; }

	#endregion local state

	#region messaging

	public static void Send(Message message) {
		Client.SendAsync(message.GetBytes());
	}

	#endregion messaging

	#region connection
	public static Socket Client;
	static async Task ListenerTask() {
		while (true) {
			byte[] buffer = new byte[1024];
			int received = await Client.ReceiveAsync(buffer, SocketFlags.None);
			string message = Encoding.ASCII.GetString(buffer, 0, received);

			OnClientMessageReceived?.Invoke(buffer[..received]);

			await Task.Yield();
		}
	}
	public static async void Connect(VTTApp instance, string ipString, string portString, string name) {
		Instance = instance;

		Client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		await Client.ConnectAsync(new IPEndPoint(IPAddress.Parse(ipString), int.Parse(portString)));

		Task _ = ListenerTask();

		Send(new InfoResponseMessage(InfoType.Name, name));
	}
	public static void Disconnect() {
		try {
			Client.Send(Encoding.ASCII.GetBytes("disconnecting."));
			Client.Close();
		}
		catch (NullReferenceException e) { }
	}
	#endregion connection
}
