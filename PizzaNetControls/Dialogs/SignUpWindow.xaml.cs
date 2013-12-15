using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PizzaNetControls.Validation;
using PizzaNetControls.Workers;
using PizzaNetWorkClient.WCFClientInfrastructure;
using PizzaNetControls.Configuration;
using PizzaNetCommon.DTOs;
using PizzaNetControls.Common;
using PizzaNetCommon.Requests;

namespace PizzaNetControls.Dialogs
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window, INotifyPropertyChanged
    {
        public SignUpWindow()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void ButtonSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Length == 0 || !RepeatedPassword.Equals(Password))
            {
                Utils.showExclamation(INVALID_PASSWORD);
                return;
            }
            if (!Utils.IsEmailValid(emailInput.Text))
            {
                Utils.showExclamation(INVALID_EMAIL);
                return;
            }
            if (addressInput.Text.Length == 0)
            {
                Utils.showExclamation(INVALID_ADDRESS);
                return;
            }
            int phone = 0;
            int.TryParse(phoneInput.Text, out phone);
            UserDTO user = new UserDTO()
            {
                Address = addressInput.Text,
                Email = emailInput.Text,
                Password = Password,
                Name = nameInput.Text,
                Phone = phone
            };
            worker.EnqueueTask(new WorkerTask((args) =>
                {
                    var us = args[0] as UserDTO;
                    if (us == null)
                        return null;
                    try
                    {
                        using (var proxy = new WorkChannel())
                        {
                            return proxy.RegisterUser(new UpdateRequest<UserDTO>()
                            {
                                Data = user
                            });
                        }
                    }
                    catch (Exception exc)
                    {
                        return exc;
                    }
                },
                (s, x) =>
                {
                    var exc = x.Result as Exception;
                    if (exc != null)
                    {
                        Utils.HandleException(exc);
                        return;
                    }
                    var res = x.Result as SingleItemResponse<UserDTO>;
                    if (res == null)
                    {
                        Utils.showExclamation(Utils.Messages.REGISTRATION_FAILED);
                        return;
                    }
                    else
                    {
                        Utils.showInformation(Utils.Messages.REGISTRATION_COMPLETED);
                        this.Close();
                        return;
                    }
                }, user));
        }

        private string _pass;
        public string Password
        {
            get { return _pass; }
            set
            {
                _pass = value;
                NotifyPropertyChanged("Password");
                passwordRepeatInput.Password = RepeatedPassword;
            }
        }

        private string _reppass;
        public string RepeatedPassword
        {
            get { return _reppass; }
            set
            {
                _reppass = value;
                NotifyPropertyChanged("RepeatedPassword");
            }
        }

        private bool _haserr;
        public bool HasValidationError
        {
            get { return _haserr; }
            set { _haserr = value; NotifyPropertyChanged("HasValidationError"); }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, emailInput);
            Keyboard.Focus(emailInput);
            repeatedPasswordConfig.SetTarget(typeof(SignUpWindow), this, "Password");
            Password = "";
            RepeatedPassword = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private const string INVALID_PASSWORD = "Password is invalid";
        private const string INVALID_EMAIL = "Email address seems to be invalid";
        private const string INVALID_ADDRESS = "Address must not be empty";

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void passwordRepeatInput_Validation(object s, ValidationEventArgs e)
        {
            HasValidationError = !e.Result;
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
