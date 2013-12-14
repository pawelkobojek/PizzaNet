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
            this.repeatedPasswordConfig.SetTarget(typeof(ClientSettingsViewModel), this, "NewPassword");
            this.currentPasswordConfig.SetTarget(typeof(ClientSettingsViewModel), this, "Password");
        }

        bool initialized = false;

        void ClientSettingsViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                this.ClientSettingsView = new ClientSettingsView(Worker);
                initialized = true;

                CurrentPassword = PasswordRepeated = NewPassword = "";
            }
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

        #region passwords
        //TODO przenieść obsługę haseł do ClientSettingsView
        private string _cpass;
        public string CurrentPassword
        {
            get { return _cpass; }
            set { _cpass = value; NotifyPropertyChanged("CurrentPassword"); }
        }

        private string _pass;
        public string Password 
        {
            get { return _pass; }
            set { _pass = value; NotifyPropertyChanged("Password"); }
        }

        private string _passn;
        public string NewPassword
        {
            get { return _passn; }
            set { _passn = value; NotifyPropertyChanged("NewPassword"); newPasswordRepeatInput.Password = PasswordRepeated; }
        }

        private string _passr;
        public string PasswordRepeated
        {
            get { return _passr; }
            set { _passr = value; NotifyPropertyChanged("PasswordRepeated"); }
        }

        private bool _haserr;
        public bool HasValidationError
        {
            get { return _haserr; }
            set { _haserr = value; NotifyPropertyChanged("HasValidationError"); }
        }

        private bool _hascurrerr;
        public bool HasCurrentValidationError
        {
            get { return _hascurrerr; }
            set { _hascurrerr = value; NotifyPropertyChanged("HasCurrentValidationError"); }
        }

        private void newPasswordInput_Validation(object s, PasswordEqualRule.ValidationEventArgs e)
        {
            HasCurrentValidationError = !e.Result;
        }

        private void passwordRepeatInput_Validation(object s, PasswordEqualRule.ValidationEventArgs e)
        {
            HasValidationError = !e.Result;
        }
        #endregion

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(ClientSettingsViewModel), new UIPropertyMetadata());

        private void SettingsButtonApply_Click(object sender, RoutedEventArgs e)
        {
            //_vo.SaveConfig();
            _vo.SaveUserInfo();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumber(e.Text);
        }

        private void SettingsButtonSavePassword_Click(object sender, RoutedEventArgs e)
        {
            //TODO implement save new password
        }

        private static bool IsNumber(string text)
        {
            Regex regex = new Regex("^[0-9]+$");
            return regex.IsMatch(text);
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
            Password = ClientConfig.CurrentUser.Password;
        }

        public bool LostFocusAction()
        {
            return true;
        }

        private void inputGotFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            if (s != null) s.SelectAll();
            var p = sender as PasswordBox;
            if (p != null) p.SelectAll();
        }
    }
}
