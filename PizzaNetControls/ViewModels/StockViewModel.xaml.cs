using PizzaNetCommon.DTOs;
using PizzaNetControls.Controls;
using PizzaNetControls.Dialogs;
using PizzaNetControls.Views;
using PizzaNetControls.Workers;
using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

        private bool IsSelectionChanging { get; set; }

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
                //TODO fix ings.Add(item.Ingredient);
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

        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
			_vo.SaveChanges();
        }

        private void TextBoxStockDetails_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (listStock.SelectedIndex < 0) return;
            //MODIFIED _vo.UpdateStockItem(listStock.SelectedIndex);
            StockView.Modified = true;
        }

        private void TextBoxStockDetails_KeyDown(object sender, KeyEventArgs e)
        {
            StockView.Modified = true;
            var txtb = sender as TextBox;
            if (txtb == null) return;
            if (listStock.SelectedIndex < 0) return;
            StockIngredientDTO rc = StockView.StockItemsCollection[listStock.SelectedIndex];
            if (e.Key == Key.Return)
            {
                BindingExpression exp = txtb.GetBindingExpression(TextBox.TextProperty);
                StockIngredientDTO ingr = exp.ResolvedSource as StockIngredientDTO;
                if (ingr == null) return;
                PropertyInfo pi = ingr.GetType().GetProperty(exp.ResolvedSourcePropertyName);
                string target = pi.GetValue(ingr).ToString();
                if (target != txtb.Text)
                    exp.UpdateSource();
                else exp.ValidateWithoutUpdate();
            }
        }

        private void CheckLastBinding()
        {
            if (listStock.SelectedIndex < 0) return;
            StockIngredientDTO rc = StockView.StockItemsCollection[listStock.SelectedIndex];
            NumberFormatInfo nfi = new CultureInfo("en-US", true).NumberFormat;
            StockView.Modified  |=
                tbId.Text       != rc.IngredientID.ToString()  ||
                tbName.Text     != rc.Name.ToString()          ||
                tbQuantity.Text != rc.StockQuantity.ToString() ||
                tbNW.Text       != rc.NormalWeight.ToString()  ||
                tbEW.Text       != rc.ExtraWeight.ToString()   ||
                tbPU.Text       != rc.PricePerUnit.ToString("0.########",nfi);
        }
    
        public void GotFocusAction()
        {
            StockView.RefreshStockItems();
        }
        public bool LostFocusAction()
        {
            CheckLastBinding();
            if (StockView.Modified)
            {
                return MessageBox.Show
                    (
                        "You have unsaved changes. Do you want to discard them?",
                        "PizzaNetWork",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Exclamation
                    ) != MessageBoxResult.No;
            }
            else return true;
        }

        private void TextBoxStockDetails_KeyUp(object sender, KeyEventArgs e)
        {
            StockView.Modified = true;
        }
    }
}
