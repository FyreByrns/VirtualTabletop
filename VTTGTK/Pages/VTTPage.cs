using Gtk;
using VTTGT;

namespace VTTGTK.Pages;

/// <summary>
/// Bin widget which contains a reference to the main VTTApp window.
/// </summary>
class VTTPage : Bin
{
    public VTTApp Parent;

    public VTTPage(VTTApp parent)
    {
        Parent = parent;
    }
}
