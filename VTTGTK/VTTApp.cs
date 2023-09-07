using Gtk;
using VTTGTK.Pages;
using VTTGTK.Widgets;

namespace VTTGTK;

class VTTApp : Window {
	public Battlemap Battlemap {
		get {
			if (Child is VTTPage vp && vp is Tabletop table) {
				return table.Battlemap;
			}
			return null;
		}
	}

	public void ChangePageTo<T>(T widget)
		where T : VTTPage {

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
