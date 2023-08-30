using Gtk;
using Gdk;
using Cairo;

namespace VTTGTK.Widgets;

class Battlemap : Bin {
	int mouseX, mouseY;
	bool mouseDown;
	int gridX, gridY;

	int dragStartX, dragStartY;

	Box vboxMainLayout;
	Box hboxSubLayout;

	EventBox eventBattlemap;
	ScrolledWindow scrollBattlemap;
	Fixed positionerBattlemap;
	DrawingArea canvasBattlemap;

	Box hboxTopBar;
	Label labelMousePosition;
	Label labelGridPosition;

	Box vboxRightSideButtons;

	public int GridWidth = 10, GridHeight = 10;
	public int GridSize = 70;

	public List<Token> Tokens = new();

	private void ScrollValueChanged(object? sender, EventArgs e) {
		RedrawBackground();
	}

	void MouseMove(object o, MotionNotifyEventArgs args) {
		mouseX = (int)args.Event.X + (int)scrollBattlemap.Hadjustment.Value;
		mouseY = (int)args.Event.Y + (int)scrollBattlemap.Vadjustment.Value;
		gridX = mouseX / GridSize;
		gridY = mouseY / GridSize;

		labelMousePosition.Text = $"Mouse: {mouseX}, {mouseY}";
		labelGridPosition.Text = $"Grid: {mouseX / GridSize}, {mouseY / GridSize}";

		if (mouseDown) {
			MouseDrag();
		}

		RedrawBackground();
	}
	void MouseDown(object o, ButtonPressEventArgs args) {
		mouseDown = true;
		dragStartX = mouseX;
		dragStartY = mouseY;
		RedrawBackground();
	}
	void MouseDrag() {
	}
	private void MouseUp(object o, ButtonReleaseEventArgs args) {
		mouseDown = false;
	}

	void RedrawBackground() {
		// hack because Widget.QueueDraw doesn't work
		canvasBattlemap.SetSizeRequest(GridWidth * GridSize, GridHeight * GridSize);
		canvasBattlemap.Hide();
		canvasBattlemap.Show();
	}
	void OnDraw(object o, DrawnArgs args) {
		var c = args.Cr;

		// draw grid
		c.SetSourceRGBA(0.9, 0.9, 0.9, 1);
		for (int x = -1; x <= GridWidth; x++) {
			c.LineTo(x * GridSize, 0);
			c.LineTo(x * GridSize, GridHeight * GridSize);
			c.Stroke();
		}
		for (int y = -1; y <= GridHeight; y++) {
			c.LineTo(0, y * GridSize);
			c.LineTo(GridWidth * GridSize, y * GridSize);
			c.Stroke();
		}

		c.SetSourceRGB(0, 0, 0);
		c.Translate(gridX * GridSize, gridY * GridSize);
		c.Rectangle(0, 0, GridSize, GridSize);
		c.Stroke();
		c.Translate(-gridX * GridSize, -gridY * GridSize);

		if (mouseDown) {
			c.MoveTo(dragStartX, dragStartY);
			c.LineTo(mouseX, mouseY);
			c.Stroke();
		}

		DrawTokens(c);
	}

	void DrawTokens(Context c) {
		c.SetSourceRGB(0, 1, 0);
		foreach (Token token in Tokens) {
			double x = token.X * GridSize;
			double y = token.Y * GridSize;
			double size = (double)token.Size * GridSize;

			c.Translate(x, y);
			c.Arc(size / 2, size / 2, size / 2, 0, Math.PI * 2);
			c.Fill();
			c.Translate(-x, -y);
		}
	}

	public Battlemap() {
		vboxMainLayout = new(Orientation.Vertical, DefaultItemSpacing);
		Add(vboxMainLayout);

		// top bar
		Entry entryWidth = new Entry() { PlaceholderText = "Width", };
		Entry entryHeight = new Entry() { PlaceholderText = "Height", };
		Button buttonSetDimensions = new Button("Apply");

		buttonSetDimensions.Clicked += delegate {
			if (int.TryParse(entryWidth.Text, out int newWidth)
			&& int.TryParse(entryHeight.Text, out int newHeight)) {
				GridWidth = newWidth;
				GridHeight = newHeight;

				RedrawBackground();
			}
		};

		labelMousePosition = new("mouse position");
		labelGridPosition = new("grid position");
		hboxTopBar = new Box(Orientation.Horizontal, DefaultItemSpacing) {
			new Label("Battlemap Dimensions: "), entryWidth, entryHeight, buttonSetDimensions, new Separator(Orientation.Vertical),

			labelMousePosition,
			labelGridPosition,
		};
		vboxMainLayout.Add(hboxTopBar);
		vboxMainLayout.Add(new Separator(Orientation.Horizontal));

		hboxSubLayout = new(Orientation.Horizontal, DefaultItemSpacing);
		vboxMainLayout.Add(hboxSubLayout);

		// positionable area
		eventBattlemap = new() {
			Expand = true,
		};
		hboxSubLayout.Add(eventBattlemap);
		scrollBattlemap = new() {
			Expand = true,
		};
		eventBattlemap.Add(scrollBattlemap);

		positionerBattlemap = new Fixed() {
			Expand = true,
		};
		scrollBattlemap.Add(positionerBattlemap);

		vboxRightSideButtons = new(Orientation.Vertical, DefaultItemSpacing);
		hboxSubLayout.Add(vboxRightSideButtons);
		Button testButton = new("test");
		testButton.SetSizeRequest(50, 50);
		vboxRightSideButtons.Add(testButton);

		canvasBattlemap = new();
		canvasBattlemap.Drawn += OnDraw;
		positionerBattlemap.Put(canvasBattlemap, 0, 0);
		canvasBattlemap.SetSizeRequest(GridWidth * GridSize, GridWidth * GridSize);

		eventBattlemap.Events |= EventMask.PointerMotionMask | EventMask.ScrollMask | EventMask.ButtonPressMask | EventMask.ButtonMotionMask | EventMask.AllEventsMask;
		eventBattlemap.ButtonPressEvent += MouseDown;
		eventBattlemap.MotionNotifyEvent += MouseMove;
		eventBattlemap.ButtonReleaseEvent += MouseUp;

		scrollBattlemap.Hadjustment.ValueChanged += ScrollValueChanged;
		scrollBattlemap.Vadjustment.ValueChanged += ScrollValueChanged;

		Tokens.Add(new Token() {
			X = 10,
			Y = 5,
		});
	}
}

class Token {
	public int X, Y;
	public int Size = 1;
}