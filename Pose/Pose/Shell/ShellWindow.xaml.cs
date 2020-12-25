using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Pose.Controls;
using Pose.Framework.Messaging;
using Pose.Framework.ViewModels;

namespace Pose.Shell
{
    public partial class ShellWindow : ModernWindow
    {
        public ShellWindow()
        {
            InitializeComponent();
        }

        private void ShellWindow_OnContentRendered(object sender, EventArgs e)
        {
            // better than OnLoaded because here, all subcontrols are fully loaded too, and their OnLoaded has already triggered, allowing their VM to initialize.
            MessageBus.Default.Publish(new UserInterfaceReady());
        }

        private void ShellWindow_OnDeactivated(object sender, EventArgs e)
        {
            MessageBus.Default.Publish(new ViewDeactivatedEvent());
        }

        private void ShellWindow_OnActivated(object sender, EventArgs e)
        {
            MessageBus.Default.Publish(new ViewActivatedEvent());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // our workflow passes this method for 2 reasons: viewmodel decides to exit, or user presses Close button in window chrome.
            if (ViewModel.IsExiting)
            {
                // viewmodel decided to exit app, do that.
                e.Cancel = false;
                return;
            }
            // user pressed the Close button on the window: initiate close workflow
            e.Cancel = true; // cancel the normal workflow, we go through the ViewModel.
            ViewModel.ExitApplication();
        }

        public ShellViewModel ViewModel => DataContext as ShellViewModel;

        #region RoutedCommands

        public static readonly RoutedCommand Exit = new RoutedCommand();
        public static readonly RoutedCommand Cancel = new RoutedCommand();
        public static readonly RoutedCommand ExportSpritesheet = new RoutedCommand();

        private void SaveExecute(object sender, RoutedEventArgs e)
        {
            ViewModel.DoSaveWorkflow();
        }
        
        private void UndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanUndo();
        }

        private void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Undo();
        }

        private void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanRedo();
        }

        private void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Redo();
        }

        private void NewExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.DoNewDocumentWorkflow();
        }

        private void OpenExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.DoOpenDocumentWorkflow();
        }

        private void SaveAsExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.DoSaveAsWorkflow();
        }

        private void ExitExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExitApplication();
        }

        private void CancelExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExitApplication();
        }

        private void ExportSpritesheetExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExportSpritesheet();
        }

        #endregion

    }
}
