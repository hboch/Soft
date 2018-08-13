using Autofac;
using Soft.Ui.Startup;
using System;
using System.Windows;

namespace Soft.Ui
{
    /// <summary>
    /// Interaction logic for "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Method is called when application starts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //TODO Wie testen ?
            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Bootstrap();

            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unexpected Error. Please contact the hotline."
                + Environment.NewLine + e.Exception.Message, "Unexpected Error");
            e.Handled = true;
        }
    }
}
