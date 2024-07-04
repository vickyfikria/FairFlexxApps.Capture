using Foundation;

namespace FairFlexxApps.Capture;

[Register(nameof(AppDelegate))]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram_.CreateMauiApp();
}
