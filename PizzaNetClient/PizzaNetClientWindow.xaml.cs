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
            this.Closed += PizzaNetClientWindow_Closed;
            this.Closing += PizzaNetClientWindow_Closing;
            this.worker.RefreshButtonClicked += worker_RefreshButtonClicked;
            LastSelected = MainTab;
        }

        void PizzaNetClientWindow_Closing(object sender, CancelEventArgs e)
        {
            if (LastSelected == MainTab && !clientMainViewModel.LostFocusAction())
                e.Cancel = true;
            else if (LastSelected == OrdersTab && !myOrdersViewModel.LostFocusAction())
                e.Cancel = true;
            else if (LastSelected == SettingsTab && !settingsViewModel.LostFocusAction())
                e.Cancel = true;
        }

        void PizzaNetClientWindow_Closed(object sender, EventArgs e)
        {
            ClientConfig.Save();
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

            TabItem SelectedTab = null;
            if (OrdersTab.IsSelected)
                SelectedTab = OrdersTab;
            else if (SettingsTab.IsSelected)
                SelectedTab = SettingsTab;
            else if (MainTab.IsSelected)
                SelectedTab = MainTab;

            if (LastSelected == MainTab)
            {
                IsSelectionChanging = true;
                if (!clientMainViewModel.LostFocusAction())
                {
                    tabControl.SelectedIndex = 0;
                    IsSelectionChanging = false;
                    return;
                }
                IsSelectionChanging = false;

            }

            if (LastSelected == OrdersTab)
            {
                IsSelectionChanging = true;
                if (!myOrdersViewModel.LostFocusAction())
                {
                    tabControl.SelectedIndex = 1;
                    IsSelectionChanging = false;
                    return;
                }
                IsSelectionChanging = false;
            }

            if (LastSelected == SettingsTab)
            {
                IsSelectionChanging = true;
                if (!settingsViewModel.LostFocusAction())
                {
                    tabControl.SelectedIndex = 2;
                    IsSelectionChanging = false;
                    return;
                }
                IsSelectionChanging = false;
            }

            if (MainTab==SelectedTab)
            {
                IsSelectionChanging = true;
                tabControl.SelectedIndex = 0;
                IsSelectionChanging = false;
                clientMainViewModel.GotFocusAction();
                LastSelected = MainTab;
            }

            if (OrdersTab==SelectedTab)
            {
                IsSelectionChanging = true;
                tabControl.SelectedIndex = 1;
                IsSelectionChanging = false;
                myOrdersViewModel.GotFocusAction();
                LastSelected = OrdersTab;
            }
            if (SettingsTab==SelectedTab)
            {
                IsSelectionChanging = true;
                tabControl.SelectedIndex = 2;
                IsSelectionChanging = false;
                settingsViewModel.GotFocusAction();
                LastSelected = SettingsTab;
            }
        }
    }
}
