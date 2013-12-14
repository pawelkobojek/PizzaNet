﻿using PizzaNetControls.Validation;
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
        }

        bool initialized = false;

        void ClientSettingsViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                this.ClientSettingsView = new ClientSettingsView(Worker);
                initialized = true;
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
        private string _cpass="password";
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
            set { _passn = value; NotifyPropertyChanged("NewPassword"); }
        }

        private string _passr;
        public string PasswordRepeated
        {
            get { return _passr; }
            set { _passr = value; NotifyPropertyChanged("PasswordRepeated"); }
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
            ClientSettingsView.User = ClientConfig.CurrentUser;
        }

        public bool LostFocusAction()
        {
            return true;
        }
    }
}
