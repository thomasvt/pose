using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Pose.Domain.Editor;
using Pose.Shell;
using Pose.Startup;

namespace Pose
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceProvider = new Bootstrapper().Start();
            Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            StartMainWindow();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is UserActionException userActionException)
            {
                MessageBox.Show(userActionException.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"An unexpected error occurred. We'll try to keep things going, but restarting Pose may be a good idea: \n[{e.Exception.GetType().Name}] {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            e.Handled = true;
        }

        private void StartMainWindow()
        {
            var mainWindow = ServiceProvider.GetRequiredService<ShellWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<ShellViewModel>();
            mainWindow.Show();
        }

        public IServiceProvider ServiceProvider { get; private set; }
    }
}
