using PizzaNetControls.Views;
using PizzaNetControls.Workers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PizzaNetControls.ViewModels
{
    /// <summary>
    /// Interaction logic for UsersViewModel.xaml
    /// </summary>
    public partial class UsersViewModel : UserControl, INotifyPropertyChanged
    {
        #region view model init and so on

        public UsersViewModel()
        {
            this.DataContext = this;
            InitializeComponent();
            this.Loaded += UsersViewModel_Loaded;
        }

        bool initialized = false;

        void UsersViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                this.UsersView = new UsersView(Worker);
                initialized = true;
            }
        }

        private UsersView _vo;
        public UsersView UsersView
        {
            get { return _vo; }
            set { _vo = value; NotifyPropertyChanged("UsersView"); }
        }

        public IWorker Worker
        {
            get { return (IWorker)GetValue(WorkerProperty); }
            set { SetValue(WorkerProperty, value); }
        }

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(UsersViewModel), new UIPropertyMetadata());

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void ButtonRemoveUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
