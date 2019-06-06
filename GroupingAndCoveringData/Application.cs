using System.Windows;
using Unity;

namespace GroupingAndCoveringData
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            UnityConfig unityConfig = new UnityConfig();
            var container = UnityConfig.GetConfiguredContainer();
            var mainWindow = container.Resolve<MainWindow>(); // Creating Main window
            mainWindow.Show();
        }
    }
}
