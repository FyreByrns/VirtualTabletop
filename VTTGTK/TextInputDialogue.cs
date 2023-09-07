using Gtk;

namespace VTTGTK;

class TextInputDialogue : Dialog {
	public string Answer;

	public TextInputDialogue(string question) {
		Title = question;
		Entry answerField = new();
		AddActionWidget(answerField, ResponseType.Accept);

		answerField.Changed += delegate {
			Answer = answerField.Text;
		};
	}
}
