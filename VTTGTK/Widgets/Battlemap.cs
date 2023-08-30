using Gtk;
using Gdk;

namespace VTTGTK.Widgets;

class Battlemap : Bin {
	int mouseX, mouseY;
	int gridX, gridY;

	EventBox eventBox;
	ScrolledWindow scroller;
	Fixed positioner;
	DrawingArea back;

	Box topBar;
	Label labelMousePosition;
	Label labelGridPosition;

	public int GridWidth = 10, GridHeight = 10;
	public int GridSize = 70;

	private void ScrollValueChanged(object? sender, EventArgs e) {
		RedrawBackground();
	}

	void MouseMove(object o, MotionNotifyEventArgs args) {
		mouseX = (int)args.Event.X + (int)scroller.Hadjustment.Value;
		mouseY = (int)args.Event.Y + (int)scroller.Vadjustment.Value;
		gridX = mouseX / GridSize;
		gridY = mouseY / GridSize;

		labelMousePosition.Text = $"Mouse: {mouseX}, {mouseY}";
		labelGridPosition.Text = $"Grid: {mouseX / GridSize}, {mouseY / GridSize}";

		RedrawBackground();
	}

	private void OnClick(object o, ButtonPressEventArgs args) {
		RedrawBackground();
	}

	void RedrawBackground() {
		// hack because Widget.QueueDraw doesn't work
		back.SetSizeRequest(GridWidth * GridSize, GridHeight * GridSize);
		back.Hide();
		back.Show();
	}
	void OnDraw(object o, DrawnArgs args) {
		var c = args.Cr;

		c.SetSourceRGB(0, 0, 0);
		c.Translate(gridX * GridSize, gridY * GridSize);
		c.Rectangle(0, 0, GridSize, GridSize);
		c.Stroke();
		c.Translate(-gridX * GridSize, -gridY * GridSize);

		// draw grid
		c.SetSourceRGBA(0, 0, 0, 0.1);
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
	}

	public Battlemap() {
		Box container = new(Orientation.Vertical, DefaultItemSpacing);
		Add(container);

		// top bar
		Entry entryWidth = new Entry() { PlaceholderText = "Battlemap Width", };
		Entry entryHeight = new Entry() { PlaceholderText = "Battlemap Height", };
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
		topBar = new Box(Orientation.Horizontal, DefaultItemSpacing) {
			new Label("Battlemap Dimensions: "), entryWidth, entryHeight, buttonSetDimensions, new Separator(Orientation.Vertical),

			labelMousePosition,
			labelGridPosition,
		};
		container.Add(topBar);
		container.Add(new Separator(Orientation.Horizontal));

		// positionable area
		eventBox = new() {
			Expand = true,
		};
		container.Add(eventBox);
		scroller = new() {
			Expand = true,
		};
		eventBox.Add(scroller);

		positioner = new Fixed() {
			Expand = true,
		};
		scroller.Add(positioner);

		back = new();
		back.Drawn += OnDraw;
		positioner.Put(back, 0, 0);
		back.SetSizeRequest(GridWidth * GridSize, GridWidth * GridSize);

		eventBox.Events |= EventMask.PointerMotionMask | EventMask.ScrollMask;
		eventBox.ButtonPressEvent += OnClick; ;
		eventBox.MotionNotifyEvent += MouseMove;

		scroller.Hadjustment.ValueChanged += ScrollValueChanged;
		scroller.Vadjustment.ValueChanged += ScrollValueChanged;
	}
}