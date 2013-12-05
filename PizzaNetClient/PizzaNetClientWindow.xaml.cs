using PizzaNetControls;
using PizzaNetControls.Common;
using PizzaNetControls.Configuration;
using PizzaNetControls.Dialogs;
using PizzaNetControls.Workers;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PizzaNetClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PizzaNetClientWindow : Window, INotifyPropertyChanged
    {
        public PizzaNetClientWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            this.worker.Lock = this.contentControl;
        }

        private ClientConfig _config;
        public ClientConfig Config 
        {
            get { return _config; }
            set { _config = value; NotifyPropertyChanged("Config"); }
        }
        
        private void PizzaNetWindowClass_Loaded(object sender, RoutedEventArgs e)
        {
            loginDialog.ModalDialogHidden += loginDialog_ModalDialogHidden;
            loginDialog.Show();
        }

        void loginDialog_ModalDialogHidden(object sender, ModalDialog.ModalDialogEventArgs e)
        {
            if (!loginDialog.DialogResult) 
                this.Close();
            clientMainViewModel.ClientMainView.Load();
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
                        
        /// <summary>
        /// Method needed to merge Ingredients and avoid to duplicate them in Ingredients table
        /// </summary>
        /// <param name="det"></param>
        /// <param name="ing"></param>
        /// <returns></returns>
                
        private void SettingsButtonApply_Click(object sender, RoutedEventArgs e)
        {
            worker.EnqueueTask(new WorkerTask((args) =>
            {
                Config.Save(ClientConfig.CONFIGNAME, typeof(ClientConfig));
                return null;
            }, null));
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumber(e.Text);
        }

        private static bool IsNumber(string text)
        {
            Regex regex = new Regex("^[0-9]+$");
            return regex.IsMatch(text);
        }

        private void contentControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contentControl.SelectedIndex==2)
            {
                worker.EnqueueTask(new WorkerTask((args) =>
                    {
                        return ClientConfig.getConfig();
                    }, (s, args) =>
                    {
                        Config = args.Result as ClientConfig;
                    }));
            }
        }
    }
}
