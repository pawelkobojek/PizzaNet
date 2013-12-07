using PizzaNetControls.Controls;
using PizzaNetControls.Dialogs;
using PizzaNetControls.Views;
using PizzaNetControls.Workers;
using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for StockViewModel.xaml
    /// </summary>
    public partial class StockViewModel : UserControl, INotifyPropertyChanged
    {
        public StockViewModel()
        {
            this.DataContext = this;
            InitializeComponent();
            this.Loaded += StockViewModel_Loaded;
        }

        bool initialized = false;

        void StockViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                this.StockView = new StockView(Worker);
                initialized = true;
            }
        }

        private StockView _vo;
        public StockView StockView
        {
            get { return _vo; }
            set { _vo = value; NotifyPropertyChanged("StockView"); }
        }

        public IWorker Worker
        {
            get { return (IWorker)GetValue(WorkerProperty); }
            set { SetValue(WorkerProperty, value); }
        }

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(StockViewModel), new UIPropertyMetadata());

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ButtonAddIngredient_Click(object sender, RoutedEventArgs e)
        {
            _vo.AddIngredient();
        }

        private void ButtonRemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (listStock.SelectedIndex < 0) return;
            _vo.RemoveIngredient(listStock.SelectedIndex);
        }

        private void ButtonOrderSupplies_Click(object sender, RoutedEventArgs e)
        {
            // TODO modify OrderIngredientForm
            ObservableCollection<Ingredient> ings = new ObservableCollection<Ingredient>();
            foreach (var item in StockView.StockItemsCollection)
            {
                ings.Add(item.Ingredient);
            }
            var parent = VisualTreeHelper.GetParent(this);
            while (!(parent is Window))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            OrderIngredientForm form = new OrderIngredientForm((Window)parent, ings);
            form.ShowDialog();
            StockView.RefreshStockItems();
        }

        private void TextBoxStockDetails_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (listStock.SelectedIndex < 0) return;
            _vo.UpdateStockItem(listStock.SelectedIndex);
        }

        private void TextBoxStockDetails_KeyDown(object sender, KeyEventArgs e)
        {
            var txtb = sender as TextBox;
            if (txtb == null) return;
            if (listStock.SelectedIndex < 0) return;
            StockItem rc = StockView.StockItemsCollection[listStock.SelectedIndex];
            if (e.Key == Key.Return)
            {
                BindingExpression exp = txtb.GetBindingExpression(TextBox.TextProperty);
                Ingredient ingr = exp.ResolvedSource as Ingredient;
                if (ingr == null) return;
                PropertyInfo pi = ingr.GetType().GetProperty(exp.ResolvedSourcePropertyName);
                string target = pi.GetValue(ingr).ToString();
                if (target != txtb.Text)
                    exp.UpdateSource();
                else exp.ValidateWithoutUpdate();
            }
        }
    }
}
