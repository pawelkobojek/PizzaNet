using PizzaNetControls.Configuration;
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

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public SettingsWindow(Window owner) : this()
        {
            this.Owner = owner;
        }

        private ClientConfig _config;
        public ClientConfig Config
        {
            get { return _config; }
            set { _config = value; NotifyPropertyChanged("Config"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Update()
        {
            NotifyPropertyChanged("Config");
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            Config.Save(ClientConfig.CONFIGNAME, typeof(ClientConfig));
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
