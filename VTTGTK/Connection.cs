using System.Net.Sockets;
using System.Text;

namespace VTTGT;

enum ConnectionState {
    None = 0,

    AwaitingLobbyInfo,

    Normal,
}

class Connection {
    public delegate void ClientDataSent(Connection client, byte[] buffer, int received);
    public static event ClientDataSent OnDataSent;

    public ConnectionState State;
    public Socket Remote;
    public int ID;
    public string Name = "%unnamed%";

    public bool PollFailed(int microSeconds = 10) {
        return Remote.Poll(microSeconds, SelectMode.SelectRead);
    }

    public async Task Listen() {
        while (true) {
            byte[] buffer = new byte[1024];
            int received = await Remote.ReceiveAsync(buffer, SocketFlags.None);
            string message = Encoding.ASCII.GetString(buffer, 0, received);
            await Task.Run(() => OnDataSent?.Invoke(this, buffer, received));

            await Task.Yield();
        }
    }

    public Connection(Socket socket, int iD) {
        Remote = socket;
        ID = iD;
    }
}
