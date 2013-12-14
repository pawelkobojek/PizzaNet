using PizzaNetControls;
using PizzaNetControls.Common;
using PizzaNetControls.Configuration;
using PizzaNetControls.Dialogs;
using PizzaNetControls.Workers;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PizzaNetClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PizzaNetClientWindow : Window, INotifyPropertyChanged
    {
        public PizzaNetClientWindow()
        {
            if (!((Properties.Settings.Default["UsesValidCertificate"] as bool?) ?? true))
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };
            InitializeComponent();
            this.DataContext = this;

            this.worker.Lock = this.tabControl;
            this.worker.RefreshButtonClicked += worker_RefreshButtonClicked;
            LastSelected = MainTab;
        }

        void worker_RefreshButtonClicked(object sender, EventArgs e)
        {
            if (LastSelected == MainTab && clientMainViewModel.LostFocusAction())
                clientMainViewModel.GotFocusAction();
            else if (LastSelected == OrdersTab && myOrdersViewModel.LostFocusAction())
                myOrdersViewModel.GotFocusAction();
            else if (LastSelected == SettingsTab && settingsViewModel.LostFocusAction())
                settingsViewModel.GotFocusAction();
        }

        public TabItem LastSelected { get; set; }
        public bool IsSelectionChanging { get; set; }

        private void PizzaNetWindowClass_Loaded(object sender, RoutedEventArgs e)
        {
            loginDialog.ModalDialogHidden += loginDialog_ModalDialogHidden;
            loginDialog.Show();
        }

        void loginDialog_ModalDialogHidden(object sender, ModalDialog.ModalDialogEventArgs e)
        {
            if (!loginDialog.DialogResult)
                this.Close();
            clientMainViewModel.GotFocusAction();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl) || !this.IsLoaded)
                return;

            if (IsSelectionChanging)
            {
                IsSelectionChanging = false;
                return;
            }

            if (LastSelected == MainTab)
            {
                if (!clientMainViewModel.LostFocusAction())
                {
                    IsSelectionChanging = true;
                    tabControl.SelectedIndex = 0;
                    e.Handled = true;
                    return;
                }

            }

            if (LastSelected == OrdersTab)
            {
                if (!myOrdersViewModel.LostFocusAction())
                {
                    IsSelectionChanging = true;
                    tabControl.SelectedIndex = 1;
                    e.Handled = true;
                    return;
                }
            }

            if (LastSelected == SettingsTab)
            {
                if (!settingsViewModel.LostFocusAction())
                {
                    IsSelectionChanging = true;
                    tabControl.SelectedIndex = 2;
                    e.Handled = true;
                    return;
                }
            }

            if (MainTab.IsSelected)
            {
                clientMainViewModel.GotFocusAction();
                LastSelected = MainTab;
            }

            if (OrdersTab.IsSelected)
            {
                myOrdersViewModel.GotFocusAction();
                LastSelected = OrdersTab;
            }
            if (SettingsTab.IsSelected)
            {
                settingsViewModel.GotFocusAction();
                LastSelected = SettingsTab;
            }
        }
    }
}
