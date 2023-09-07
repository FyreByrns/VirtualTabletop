using Gtk;
using Gdk;
using Cairo;
using System.Drawing;

namespace VTTGTK.Widgets;

class Battlemap : Bin {
	public delegate void TokenMovedEventHandler(int fromX, int fromY, int toX, int toY);
	public event TokenMovedEventHandler TokenMoved;

	int mouseX, mouseY;
	bool mouseDown;
	int gridX, gridY;

	int gridDragStartX, gridDragStartY;
	int dragStartX, dragStartY;
	Token draggedToken;

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

	public Token? GetTokenAt(int x, int y) {
		foreach (Token token in Tokens) {
			if (token.X == x && token.Y == y) {
				return token;
			}
		}

		return null;
	}

	// todo: check if the token may be moved
	public void MoveToken(int fromX, int fromY, int toX, int toY, bool notify) {
		Token token = GetTokenAt(fromX, fromY);
		if (token is not null) {
			token.X = toX;
			token.Y = toY;

			if (notify) {
				TokenMoved?.Invoke(fromX, fromY, toX, toY);
			}

			RedrawBackground();
		}
	}

	void ScrollValueChanged(object? sender, EventArgs e) {
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
		StartDrag();
		RedrawBackground();
	}
	void MouseDrag() {
		ContinueDrag();
	}
	private void MouseUp(object o, ButtonReleaseEventArgs args) {
		StopDrag();
	}

	void StartDrag() {
		mouseDown = true;
		dragStartX = mouseX;
		dragStartY = mouseY;
		gridDragStartX = gridX;
		gridDragStartY = gridY;

		// test for tokens grabbed
		foreach (Token token in Tokens) {
			if (gridX >= token.X && gridX < token.X + token.Size
			 && gridY >= token.Y && gridY < token.Y + token.Size) {
				draggedToken = token;
				break;
			}
		}
	}
	void ContinueDrag() {

	}
	void StopDrag() {
		mouseDown = false;

		if (draggedToken != null) {
			int newX = gridX - (int)draggedToken.Size / 2;
			int newY = gridY - (int)draggedToken.Size / 2;

			MoveToken(draggedToken.X, draggedToken.Y, newX, newY, true);
		}
		draggedToken = null;
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
		void drawToken(Token token) {
			double x = token.X * GridSize;
			double y = token.Y * GridSize;
			double size = (double)token.Size * GridSize;

			c.Translate(x, y);
			c.Arc(size / 2, size / 2, size / 2, 0, Math.PI * 2);
			c.Fill();
			c.Translate(-x, -y);
		}

		c.SetSourceRGB(0, 1, 0);
		foreach (Token token in Tokens) {
			drawToken(token);
		}

		// draw dragged token
		if (draggedToken != null) {
			c.SetSourceRGB(0, 0, 1);
			c.Translate(mouseX, mouseY);
			c.Arc(0, 0, draggedToken.Size / 2 * GridSize, 0, Math.PI * 2);
			c.Fill();
			c.Translate(-mouseX, -mouseY);

			// highlight drop area
			c.Rectangle((gridX - draggedToken.Size / 2) * GridSize, (gridY - draggedToken.Size / 2) * GridSize, draggedToken.Size * GridSize, draggedToken.Size * GridSize);
			c.Stroke();
		}
	}

	public Battlemap() {
		vboxMainLayout = new(Orientation.Vertical, DefaultItemSpacing);
		Add(vboxMainLayout);

		// top bar
		Entry entryWidth = new Entry() { PlaceholderText = "Width", };
		Entry entryHeight = new Entry() { PlaceholderText = "Height", };
		Entry entrySize = new Entry() { PlaceholderText = "Size", };
		Button buttonSetDimensions = new Button("Apply");

		buttonSetDimensions.Clicked += delegate {
			if (int.TryParse(entryWidth.Text, out int newWidth)
			&& int.TryParse(entryHeight.Text, out int newHeight)
			&& int.TryParse(entrySize.Text, out int newSize)) {
				GridWidth = newWidth;
				GridHeight = newHeight;
				GridSize = newSize;

				RedrawBackground();
			}
		};

		labelMousePosition = new("mouse position");
		labelGridPosition = new("grid position");
		hboxTopBar = new Box(Orientation.Horizontal, DefaultItemSpacing) {
			new Label("Battlemap Dimensions: "), entryWidth, entryHeight, entrySize, buttonSetDimensions, new Separator(Orientation.Vertical),

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
			X = 2,
			Y = 5,
			Size = 1,
		});
	}
}

class Token {
	public int X, Y;
	public int Size = 1;
}