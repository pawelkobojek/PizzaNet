using PizzaNetControls.Views;
using PizzaNetControls.Workers;
using PizzaNetDataModel.Model;
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
    /// Interaction logic for ClienMainViewModel.xaml
    /// </summary>
    public partial class ClientMainViewModel : UserControl, INotifyPropertyChanged
    {
        public ClientMainViewModel()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += ClientMainViewModel_Loaded;
        }

        bool initialized = false;

        void ClientMainViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                this.ClientMainView = new ClientMainView(Worker);
                initialized = true;
            }
        }

        private ClientMainView _vo;
        public ClientMainView ClientMainView
        {
            get { return _vo; }
            set { _vo = value; NotifyPropertyChanged("ClientMainView"); }
        }

        public IWorker Worker
        {
            get { return (IWorker)GetValue(WorkerProperty); }
            set { SetValue(WorkerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Worker.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(ClientMainViewModel), new UIPropertyMetadata());

        

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            _vo.AddOrder();
        }

        private void ButtonRemoveFromOrder_Click(object sender, RoutedEventArgs e)
        {
            _vo.RemoveFromOrder(orderedPizzasContainer.SelectedIndex);
        }

        private void ButtonClearOrder_Click(object sender, RoutedEventArgs e)
        {
            _vo.ClearOrder();
        }

        private void ButtonOrder_Click(object sender, RoutedEventArgs e)
        {
            _vo.Order();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null) return;
            var value = rb.Tag as PizzaNetDataModel.Model.Size;
            if (value != null)
                _vo.ChangeCurrentSize(value);
        }

        private void RecipesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != RecipesContainer) return;
            if (RecipesContainer.SelectedIndex < 0) return;
            _vo.ChangeSelectedRecipe(RecipesContainer.SelectedIndex);
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
