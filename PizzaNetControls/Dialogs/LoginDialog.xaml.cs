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
        public const string TITLE = "Login";
        public const string LOGIN_FAILED = "Login failed";
        public const string RIGHTS_LEVEL_FAILURE = "You don't have enough rights level";

        public bool ShowSignUp
        {
            get { return (bool)GetValue(ShowSignUpProperty); }
            set { SetValue(ShowSignUpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSignUp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSignUpProperty =
            DependencyProperty.Register("ShowSignUp", typeof(bool), typeof(LoginDialog), new UIPropertyMetadata(true));

        public event EventHandler<EventArgs> OnLogin;

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            ValidateLogin((s,o) =>
            {
                UserDTO res = o.Result as UserDTO;
                if (res!=null)
                {
                    ClientConfig cfg = ClientConfig.getConfig();
                    cfg.User = res;
                    cfg.Password = passwordInput.Password;
                    if (cfg.User.Rights<MinRightsLevel)
                    {
                        Utils.showError(TITLE, RIGHTS_LEVEL_FAILURE);
                        return;
                    }
                    if (OnLogin != null)
                        OnLogin(this, new EventArgs());
                    DialogResult = true;
                    this.Hide();
                }
                else
                {
                    Utils.showError(TITLE, LOGIN_FAILED);
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
                    var cfg = args[2] as ClientConfig;
                    try
                    {
                        using(var proxy = new WorkChannel(cfg.ServerAddress))
                        {
                            var result = proxy.GetUser(new EmptyRequest() { Login = log, Password = pass });
                            if (result == null) return null;
                            return result.Data;
                        }
                    }
                    catch(Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                        return null;
                    }
                },handler, login, password, ClientConfig.getConfig()));
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
            new SignUpWindow().ShowDialog();
        }
    }
}
