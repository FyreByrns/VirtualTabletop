using Gtk;

namespace VTTGTK.Widgets;

class StatusWidget : Button {
	Widget showHideToggle;
	Box popupContainer;
	Label nameLabel;
	Label idLabel;

	public string Name {
		get {
			return nameLabel?.Text ?? "%name%";
		}
		set {
			if (nameLabel is not null) {
				nameLabel.Text = value;
			}
		}
	}
	public string ID {
		get {
			return idLabel?.Text ?? "%id%";
		}
		set {
			if (idLabel is not null) {
				idLabel.Text = value;
			}
		}
	}

	public StatusWidget() {
		Label = "Status";
		Clicked += Click;

		popupContainer = new Box(Orientation.Horizontal, DefaultItemSpacing);

		nameLabel = new();
		idLabel = new();

		Box layout = new(Orientation.Horizontal, DefaultItemSpacing) {
			new Box(Orientation.Horizontal, DefaultItemSpacing) {
				new Label("Name: "),
				nameLabel,
			},
			new Box(Orientation.Horizontal, DefaultItemSpacing) {
				new Label("ID: "),
				idLabel,
			},
		};
		popupContainer.Add(layout);
		showHideToggle = popupContainer;
	}

	private void Click(object? sender, EventArgs e) {
		var temp = Child;
		Remove(Child);
		Add(showHideToggle);
		showHideToggle = temp;
		ShowAll();
	}
}
