using PizzaNetControls.Validation;
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

namespace PizzaNetControls.Dialogs
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputPassword : Window, INotifyPropertyChanged
    {
        public InputPassword()
        {
            this.DataContext = this;
            InitializeComponent();
            this.repeatedPasswordConfig.SetTarget(typeof(InputPassword), this, "NewPassword");
            NewPassword = PasswordRepeated = "";
            HasValidationError = false;
        }

        public event EventHandler<OkEventArgs> OnOKClicked;

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

        private bool _haserr;
        public bool HasValidationError
        {
            get { return _haserr; }
            set { _haserr = value; NotifyPropertyChanged("HasValidationError"); }
        }

        private void passwordRepeatInput_Validation(object s, ValidationEventArgs e)
        {
            HasValidationError = !e.Result;
        }

        public static string Show(string prompt, string title, EventHandler<OkEventArgs> handler)
        {
            var dlg = new InputPassword();
            dlg.prompt.Text = prompt;
            dlg.Title = title;
            if (handler != null)
                dlg.OnOKClicked += handler;
            dlg.ShowDialog();
            return dlg.input.Password;
        }
        public static string Show(string prompt, string title)
        {
            return Show(prompt, title, null);
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            var args = new OkEventArgs() { Result = input.Password, IsValid = true};
            if (OnOKClicked != null)
                OnOKClicked(this, args);
            if (args.IsValid)
                Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
    }
}
