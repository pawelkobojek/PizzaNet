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
    /// Interaction logic for ComplaintsViewModel.xaml
    /// </summary>
    public partial class ComplaintsViewModel : UserControl, INotifyPropertyChanged
    {
        #region view model init and so on
        public ComplaintsViewModel()
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
                this.ComplaintView = new ComplaintView(Worker);
                initialized = true;
            }
        }

        private ComplaintView _vo;
        public ComplaintView ComplaintView
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
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(ComplaintsViewModel), new UIPropertyMetadata());

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

        public void GotFocusAction()
        {
            this.ComplaintView.RefreshComplaints();
        }
        public bool LostFocusAction()
        {
            return true;
        }
    }
}
