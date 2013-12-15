using PizzaNetControls.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net;
using System.Configuration;
using PizzaNetControls.Configuration;
using System.ComponentModel;
using System;

namespace PizzaNetWorkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PizzaNetWorkWindow : Window, INotifyPropertyChanged
    {
        public PizzaNetWorkWindow()
        {
            if (!((Properties.Settings.Default["UsesValidCertificate"] as bool?) ?? true))
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };
            InitializeComponent();
            this.DataContext = this;
            this.worker.Lock = this.tabControl;
            this.Closed += PizzaNetWorkWindow_Closed;
            this.worker.RefreshButtonClicked += worker_RefreshButtonClicked;
            LastSelected = OrdersTab;
        }

        void PizzaNetWorkWindow_Closed(object sender, System.EventArgs e)
        {
            ClientConfig.Save();
        }

        void worker_RefreshButtonClicked(object sender, EventArgs e)
        {
            if (LastSelected == OrdersTab && ordersViewModel.LostFocusAction())
                ordersViewModel.GotFocusAction();
            else if (LastSelected == StockTab && stockViewModel.LostFocusAction())
                stockViewModel.GotFocusAction();
            else if (LastSelected == RecipiesTab && recipiesViewModel.LostFocusAction())
                recipiesViewModel.GotFocusAction();
            else if (LastSelected == UsersTab && usersViewModel.LostFocusAction())
                usersViewModel.GotFocusAction();
        }

        public TabItem LastSelected { get; set; }
        public bool IsSelectionChanging { get; set; }
        public bool AdminRightsLevel
        {
            get
            {
                if (ClientConfig.CurrentUser == null)
                    return false;
                else return ClientConfig.CurrentUser.Rights==3;
            }
        }

        private void PizzaNetWindowClass_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, loginDialog);
            loginDialog.ModalDialogHidden += loginDialog_ModalDialogHidden;
            loginDialog.Show();
        }

        void loginDialog_ModalDialogHidden(object sender, ModalDialog.ModalDialogEventArgs e)
        {
            if (!loginDialog.DialogResult)
                this.Close();
            NotifyRightsLevelChanged();
            ordersViewModel.GotFocusAction();
        }

        /// <summary>
        /// Workaround selection changing since there is no SelectionChanging event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl) || !this.IsLoaded)
                return;

            if (IsSelectionChanging)
            {
                IsSelectionChanging = false;
                return;
            }

            TabItem SelectedTab=null;
            if (OrdersTab.IsSelected)
                SelectedTab = OrdersTab;
            else if (StockTab.IsSelected)
                SelectedTab = StockTab;
            else if (RecipiesTab.IsSelected)
                SelectedTab = RecipiesTab;
            else if (UsersTab.IsSelected)
                SelectedTab = UsersTab;
            
            if (LastSelected == OrdersTab)
            {
                IsSelectionChanging = true;
                if (!ordersViewModel.LostFocusAction())
                {
                    tabControl.SelectedIndex = 0;
                    IsSelectionChanging = false;
                    return;
                }
                IsSelectionChanging = false;
            }

            if (LastSelected == StockTab)
            {
                IsSelectionChanging = true;
                if (!stockViewModel.LostFocusAction())
                {
                    tabControl.SelectedIndex = 1;
                    IsSelectionChanging = false;
                    return;
                }
                IsSelectionChanging = false;
            }

            if (LastSelected == RecipiesTab)
            {
                IsSelectionChanging = true;
                if (!recipiesViewModel.LostFocusAction())
                {
                    tabControl.SelectedIndex = 2;
                    IsSelectionChanging = false;
                    return;
                }
                IsSelectionChanging = false;
            }

            if (LastSelected == UsersTab)
            {
                IsSelectionChanging = true;
                if (!usersViewModel.LostFocusAction())
                {
                    tabControl.SelectedIndex = 3;
                    IsSelectionChanging = false;
                    return;
                }
                IsSelectionChanging = false;
            }

            if (StockTab == SelectedTab)
            {
                IsSelectionChanging = true;
                tabControl.SelectedIndex = 1;
                IsSelectionChanging = false;
                stockViewModel.GotFocusAction();
                LastSelected = StockTab;
            }

            if (OrdersTab == SelectedTab)
            {
                IsSelectionChanging = true;
                tabControl.SelectedIndex = 0;
                IsSelectionChanging = false;
                ordersViewModel.GotFocusAction();
                LastSelected = OrdersTab;
            }

            if (RecipiesTab == SelectedTab)
            {
                IsSelectionChanging = true;
                tabControl.SelectedIndex = 2;
                IsSelectionChanging = false;
                recipiesViewModel.GotFocusAction();
                LastSelected = RecipiesTab;
            }

            if (UsersTab == SelectedTab)
            {
                IsSelectionChanging = true;
                tabControl.SelectedIndex = 3;
                IsSelectionChanging = false;
                usersViewModel.GotFocusAction();
                LastSelected = UsersTab;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyRightsLevelChanged()
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs("AdminRightsLevel"));
            }
        }
    }
}

