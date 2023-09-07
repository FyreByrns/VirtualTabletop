using Gtk;
using VTTGTK;
using VTTGTK.Messages;
using VTTGTK.Widgets;

namespace VTTGTK.Pages;

class Tabletop : VTTPage {
	public Battlemap Battlemap;
	
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

		Battlemap = new() {
			Expand = true,
		};
		mainContainer.Add(Battlemap);

		//backGrid.Put(new Label("hello"), 4, 4);
		//backGrid.Put(new Label("there"), 2, 4);

		status = new();
		statusBarContainer.Add(status);

		ClientManager.OnClientMessageReceived += OnClientData;
		Battlemap.TokenMoved += TokenMoved;
	}

	private void TokenMoved(int fromX, int fromY, int toX, int toY) {
		ClientManager.Send(new TokenMovedMessage(fromX, fromY, toX, toY));
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

			case MessageType.TokenMove: {
				if (message is TokenMovedMessage tmm) {
					Parent.Battlemap.MoveToken(tmm.FromX, tmm.FromY, tmm.ToX, tmm.ToY, false);
				}
				break;
			}
		}
	}
}
