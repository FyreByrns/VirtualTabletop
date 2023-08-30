using VTTGT;

namespace VTTGT;

static class Manager {
	public static void StartServer(string ip, string port) {
		ServerManager.StartServer(port);
	}
	public static void StartClient(string ip, string port, string name) {
		ClientManager.Connect(ip, port, name);
	}
}