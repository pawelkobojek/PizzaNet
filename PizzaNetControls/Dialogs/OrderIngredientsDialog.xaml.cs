using PizzaNetCommon.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for OrderIngredientsDialog.xaml
    /// </summary>
    public partial class OrderIngredientsDialog : Window
    {
        public OrderIngredientsDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += OrderIngredientsDialog_Loaded;
            Data = new ObservableCollection<OrderSuppliesDTO>();
            Data.Clear();
        }

        void OrderIngredientsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            FillData(dataToSet);
        }

        private IEnumerable<StockIngredientDTO> dataToSet;

        public void SetData(IEnumerable<StockIngredientDTO> list)
        {
            dataToSet = list;
        }

        private void FillData(IEnumerable<StockIngredientDTO> list)
        {
            Data.Clear();
            foreach (var v in list)
                Data.Add(new OrderSuppliesDTO()
                {
                    IngredientID = v.IngredientID,
                    Name = v.Name,
                    OrderValue = 0
                });
        }

        public ObservableCollection<OrderSuppliesDTO> Data { get; set; }
    }
}
