global using static VTTGTK.Constants;
global using MessageType = VTTGTK.Messages.MessageType;

using Gtk;

namespace VTTGTK;

internal class Program {
	static void Main(string[] args) {
		Application.Init();
		new VTTApp();
		Application.Run();
	}
}
