using PizzaNetCommon.DTOs;
using PizzaNetCommon.Requests;
using PizzaNetControls.Common;
using PizzaNetControls.Configuration;
using PizzaNetControls.Workers;
using PizzaNetWorkClient.WCFClientInfrastructure;
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
            this.worker.Lock = content;
            this.Loaded += LoginDialog_Loaded;

            SignUpCommand = new SignUpCommand();
        }

        void LoginDialog_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, emailInput);
            Keyboard.Focus(emailInput);
        }

        public SignUpCommand SignUpCommand { get; private set; }
        public int MinRightsLevel { get; set; }

        public bool ShowSignUp
        {
            get { return (bool)GetValue(ShowSignUpProperty); }
            set { SetValue(ShowSignUpProperty, value); }
        }

        public Window ParentWindow
        {
            get { return (Window)GetValue(ParentWindowProperty); }
            set { SetValue(ParentWindowProperty, value); }
        }

        public object Me { get { return this; } }

        // Using a DependencyProperty as the backing store for ParentWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentWindowProperty =
            DependencyProperty.Register("ParentWindow", typeof(Window), typeof(LoginDialog), new FrameworkPropertyMetadata());

        

        // Using a DependencyProperty as the backing store for ShowSignUp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSignUpProperty =
            DependencyProperty.Register("ShowSignUp", typeof(bool), typeof(LoginDialog), new UIPropertyMetadata(true));

        public event EventHandler<EventArgs> OnLogin;

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            ValidateLogin((s,o) =>
            {
                if (o.Result is Exception)
                {
                    Utils.HandleException(o.Result as Exception);
                    return;
                }
                UserDTO res = o.Result as UserDTO;
                if (res!=null)
                {
                    ClientConfig.CurrentUser = ClientConfig.GetUser(res.Email);
                    ClientConfig.CurrentUser.UpdateWithUserDTO(res);
                    ClientConfig.CurrentUser.Password = passwordInput.Password;
                    if (ClientConfig.CurrentUser.Rights < MinRightsLevel)
                    {
                        Utils.showError(Utils.Messages.RIGHTS_LEVEL_FAILURE);
                        return;
                    }
                    if (OnLogin != null)
                        OnLogin(this, new EventArgs());
                    DialogResult = true;
                    this.Hide();
                }
                else
                {
                    Utils.showError(Utils.Messages.UNKNOWN_ERROR_FORMAT);
                    return;
                }
            },emailInput.Text, passwordInput.Password);
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
                    var log = args[0] as string;
                    var pass = args[1] as string;
                    try
                    {
                        using (var proxy = new WorkChannel())
                        {
                            var result = proxy.GetUser(new EmptyRequest() { Login = log, Password = pass });
                            if (result == null) return null;
                            return result.Data;
                        }
                    }
                    catch(Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                        return exc;
                    }
                },handler, login, password));
        }

        private void inputGotFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            if (s != null) s.SelectAll();
            var p = sender as PasswordBox;
            if (p != null) p.SelectAll();
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
            var w = parameter as LoginDialog;
            new SignUpWindow(){ Owner = w.ParentWindow }.ShowDialog();
            FocusManager.SetFocusedElement(w, w.emailInput);
            Keyboard.Focus(w.emailInput);
        }
    }
}
