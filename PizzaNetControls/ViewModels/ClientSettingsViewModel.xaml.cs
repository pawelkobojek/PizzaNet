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

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(ClientSettingsViewModel), new UIPropertyMetadata());

        private void SettingsButtonApply_Click(object sender, RoutedEventArgs e)
        {
            _vo.SaveConfig();
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
    }
}
