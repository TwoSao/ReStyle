using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace ReStyle;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App(AppShell shell)
    {
        InitializeComponent();
        MainPage = shell;
    }
}
