using VTTGTK;

namespace VTTGTK;

static class Manager {
	public static void StartServer(string ip, string port) {
		ServerManager.StartServer(port);
	}
	public static void StartClient(VTTApp instance, string ip, string port, string name) {
		ClientManager.Connect(instance, ip, port, name);
	}
}