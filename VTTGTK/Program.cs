global using static VTTGT.Constants;
global using MessageType = VTTGT.Messages.MessageType;

using Gtk;

namespace VTTGT;

internal class Program {
	static void Main(string[] args) {
		Application.Init();
		new VTTApp();
		Application.Run();
	}
}
