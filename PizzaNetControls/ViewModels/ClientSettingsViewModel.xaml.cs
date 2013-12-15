using PizzaNetControls.Validation;
using PizzaNetControls.Views;
using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PizzaNetCommon.DTOs;
using PizzaNetControls.Configuration;
using PizzaNetControls.Common;

namespace PizzaNetControls.ViewModels
{
    /// <summary>
    /// Interaction logic for ClientSettingsView.xaml
    /// </summary>
    public partial class ClientSettingsViewModel : UserControl, INotifyPropertyChanged
    {
        public ClientSettingsViewModel()
        {
            this.DataContext = this;
            InitializeComponent();
            this.Loaded += ClientSettingsViewModel_Loaded;
        }

        bool initialized = false;

        void ClientSettingsViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                this.ClientSettingsView = new ClientSettingsView(Worker);
                this.ClientSettingsView.PropertyChanged += ClientSettingsView_PropertyChanged;
                this.repeatedPasswordConfig.SetTarget(typeof(ClientSettingsView), ClientSettingsView, "NewPassword");
                this.currentPasswordConfig.SetTarget(typeof(ClientSettingsView), ClientSettingsView, "Password");
                initialized = true;
            }
        }

        void ClientSettingsView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NewPassword")
                newPasswordRepeatInput.Password = ClientSettingsView.PasswordRepeated;
        }

        private ClientSettingsView _vo;
        public ClientSettingsView ClientSettingsView
        {
            get { return _vo; }
            set { _vo = value; NotifyPropertyChanged("ClientSettingsView"); }
        }

        public IWorker Worker
        {
            get { return (IWorker)GetValue(WorkerProperty); }
            set { SetValue(WorkerProperty, value); }
        }

        private void newPasswordInput_Validation(object s, ValidationEventArgs e)
        {
            ClientSettingsView.ModifiedPasssword = true;
            ClientSettingsView.HasCurrentValidationError = !e.Result;
        }

        private void passwordRepeatInput_Validation(object s, ValidationEventArgs e)
        {
            ClientSettingsView.ModifiedPasssword = true;
            ClientSettingsView.HasValidationError = !e.Result;
        }

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(ClientSettingsViewModel), new UIPropertyMetadata());

        private void SettingsButtonApply_Click(object sender, RoutedEventArgs e)
        {
            _vo.SaveUserInfo();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Utils.IsNumber(e.Text);
        }

        private void SettingsButtonSavePassword_Click(object sender, RoutedEventArgs e)
        {
            _vo.SavePassword();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void GotFocusAction()
        {
            ClientSettingsView.Load();
        }

        public bool LostFocusAction()
        {
            CheckLastBinding();
            if (ClientSettingsView.Modified)
                return Utils.showChangesDialog();
            else return true;
        }

        private void inputGotFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            if (s != null) s.SelectAll();
            var p = sender as PasswordBox;
            if (p != null) p.SelectAll();
        }

        private void CheckLastBinding()
        {
            ClientSettingsView.ModifiedUserData |=
                tbId.Text != ClientSettingsView.User.UserID.ToString() ||
                tbName.Text != ClientSettingsView.User.Name ||
                tbAddress.Text != ClientSettingsView.User.Address ||
                tbEmail.Text != ClientSettingsView.User.Email ||
                tbPhone.Text != ClientSettingsView.User.Phone.ToString() ||
                tbRights.Text != ClientSettingsView.User.Rights.ToString() ||
                tbRefreshTime.Text != ClientSettingsView.User.RefreshRate.ToString();
        }

        private void refreshTimeValidation(object sender, ValidationEventArgs e)
        {
            ClientSettingsView.ModifiedUserData = true;
        }

        private void binding_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (sender is TextBox)
                ClientSettingsView.ModifiedUserData = true;
            if (sender is PasswordBox)
                ClientSettingsView.ModifiedPasssword = true;
        }
    }
}
