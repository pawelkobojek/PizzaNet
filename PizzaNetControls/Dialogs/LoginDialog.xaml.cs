﻿using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PizzaNetControls.Dialogs
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : ModalDialog
    {
        public LoginDialog()
        {
            InitializeComponent();
            this.DataContext = this;

            SignUpCommand = new SignUpCommand();
        }

        public SignUpCommand SignUpCommand { get; private set; }

        public bool ShowSignUp
        {
            get { return (bool)GetValue(ShowSignUpProperty); }
            set { SetValue(ShowSignUpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSignUp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSignUpProperty =
            DependencyProperty.Register("ShowSignUp", typeof(bool), typeof(LoginDialog), new UIPropertyMetadata(true));
        
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            ValidateLogin((s,o) =>
            {
                bool res = false;
                if (o.Result as bool? != null)
                    res = o.Result as bool? ?? false;
                if (res)
                {
                    DialogResult = true;
                    this.Hide();
                }
            },"", "");
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Hide();
        }

        private void ValidateLogin(WorkerTask.WorkFinishedHandler handler, string login, string password)
        {
            worker.EnqueueTask(new WorkerTask(
                (args) =>
                {
                    System.Threading.Thread.Sleep(2000);
                    return true;
                },handler, null));
        }
    }

    public class SignUpCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            MessageBox.Show("sign up");
        }
    }
}
