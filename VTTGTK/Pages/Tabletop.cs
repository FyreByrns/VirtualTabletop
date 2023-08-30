using Gtk;
using VTTGT;
using VTTGT.Messages;
using VTTGTK.Widgets;

namespace VTTGTK.Pages;

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

class Tabletop : VTTPage {
	int ClientID;
	string Name;

	Box mainContainer;
	Box statusBarContainer;

	StatusWidget status;

	public Tabletop(VTTApp parent) : base(parent) {
		mainContainer = new(Orientation.Vertical, DefaultItemSpacing);
		statusBarContainer = new(Orientation.Horizontal, DefaultItemSpacing) { };

		Add(mainContainer);
		mainContainer.Add(statusBarContainer);

		Battlemap backGrid = new() {
			Expand = true,
		};
		mainContainer.Add(backGrid);

		//backGrid.Put(new Label("hello"), 4, 4);
		//backGrid.Put(new Label("there"), 2, 4);

		status = new();
		statusBarContainer.Add(status);

		ClientManager.OnClientMessageReceived += OnClientData;
	}

	void OnClientData(byte[] data) {
		var message = Message.Parse(data, data.Length);
		Console.WriteLine($"c rcv {data.Length}:{message.Type}");

		switch (message.Type) {
			case MessageType.IDMessage: {
				if (message is IDMessage idm) {
					ClientID = idm.ID;
					status.ID = ClientID.ToString();
				}
				break;
			}

			case MessageType.InfoRequest: {
				if (message is InfoRequestMessage imreq) {
					switch (imreq.RequestType) {
						case InfoType.Name: {
							string nameAttempt = Parent.Ask("What is your name?");

							ClientManager.Send(
								new InfoMessage(
									InfoType.Name,
									nameAttempt)
								);

							break;
						}
					}
				}
				break;
			}

			case MessageType.InfoResponse: {
				if (message is InfoMessage imres) {
					switch (imres.RequestType) {
						case InfoType.Name: {
							Name = imres.Contents;
							Console.WriteLine($"accepted name: {Name}");
							status.Name = Name;
							break;
						}
					}
				}
				break;
			}
		}
	}
}
