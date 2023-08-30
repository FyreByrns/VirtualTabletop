using Gtk;
using VTTGTK.Pages;

namespace VTTGT;

class VTTApp : Window {
	public void ChangePageTo(Widget widget) {
		Remove(Child);
		Add(widget);
		ShowAll();
	}

	public string Ask(string question) {
		TextInputDialogue dialogue = new(question);
		dialogue.Run();
		dialogue.Show();
		return dialogue.Answer;
	}

	public VTTApp() : base("VTT") {
		// load CSS
		CssProvider css = new CssProvider();
		css.LoadFromPath("style.css");
		StyleContext.AddProviderForScreen(Screen, css, 800);

		SetSizeRequest(400, 400);
		SetPosition(WindowPosition.Center);
		DeleteEvent += delegate { Application.Quit(); };

		Child = new MainMenu(this);

		ShowAll();
	}
}
