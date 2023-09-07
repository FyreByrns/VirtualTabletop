using Gtk;
using System.Net;
using VTTGTK;

namespace VTTGTK.Pages;

class MainMenu : VTTPage {
	Label connectionInfoValidation;
	Entry ipField, portField;

	public MainMenu(VTTApp parent) : base(parent) {
		Box vbox = new(Orientation.Vertical, 3);
		Add(vbox);

		Button hostButton = new("Host Session");
		vbox.Add(hostButton);
		Button connectButton = new("Connect to Session");
		vbox.Add(connectButton);

		Box connectionInfoContainer = new(Orientation.Horizontal, 3);
		vbox.Add(connectionInfoContainer);

		ipField = new(DefaultIPString);
		connectionInfoContainer.Add(new Label("IP: "));
		connectionInfoContainer.Add(ipField);

		portField = new(DefaultPortString);
		connectionInfoContainer.Add(new Label("Port: "));
		connectionInfoContainer.Add(portField);

		// show whether ip and / or port are valid
		connectionInfoValidation = new Label("");
		vbox.Add(connectionInfoValidation);
		ipField.Changed += ConnectionTextChanged;
		portField.Changed += ConnectionTextChanged;
		ConnectionTextChanged(null, null);

		Box nameContainer = new(Orientation.Horizontal, 3);
		vbox.Add(nameContainer);
		Label nameLabel = new() { Text = "Name: " };
		Entry nameField = new() { PlaceholderText = "Enter your name." };
		nameContainer.Add(nameLabel);
		nameContainer.Add(nameField);

		hostButton.Pressed += delegate {
			Manager.StartServer(ipField.Text, portField.Text);
			Manager.StartClient(Parent, ipField.Text, portField.Text, nameField.Text);
			Parent.ChangePageTo(new Tabletop(Parent));
		};
		connectButton.Pressed += delegate {
			Manager.StartClient(Parent, ipField.Text, portField.Text, nameField.Text);
			Parent.ChangePageTo(new Tabletop(Parent));
		};
	}

	void ConnectionTextChanged(object source, object args) {
		// clear feedback label
		connectionInfoValidation.Text = "";

		// check IP address
		if (!IPAddress.TryParse(ipField.Text, out _)) {
			connectionInfoValidation.Text += "Invalid IP. ";
		}
		else {
			connectionInfoValidation.Text += "Valid IP. ";
		}

		// check port
		if (!int.TryParse(portField.Text, out _)) {
			connectionInfoValidation.Text += "Invalid port.";
		}
		else {
			connectionInfoValidation.Text += "Valid port.";
		}
	}
}
