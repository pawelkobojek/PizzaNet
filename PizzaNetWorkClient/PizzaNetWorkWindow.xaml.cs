using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PizzaNetControls;
using PizzaNetDataModel;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;

namespace PizzaNetWorkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PizzaNetWorkWindow : Window
    {
        public PizzaNetWorkWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.StockItemsCollection = new ObservableCollection<PizzaNetControls.StockItem>();
            this.OrdersCollection = new ObservableCollection<PizzaNetControls.OrdersRow>();

            #region example data
            //PizzaNetControls.StockItem st;
            //for (int i = 0; i < 20; i++)
            //{
            //    st = new PizzaNetControls.StockItem();
            //    st.StockItemName = "ItemName";
            //    st.StockQuantity = 100;
            //    st.NormalWeight = 10;
            //    st.ExtraWeight = 20;
            //    st.PricePerUnit = 1.2M;
            //    StockItemsCollection.Add(st);
            //}

            //PizzaNetControls.OrdersRow o;
            //for (int i = 0; i < 10; i++)
            //{
            //    o = new PizzaNetControls.OrdersRow(new Order() { OrderID = 12*i, StateID=i%3 });
            //    OrdersCollection.Add(o);
            //}
            #endregion
        }

        public ObservableCollection<PizzaNetControls.StockItem> StockItemsCollection { get; set; }

        public ObservableCollection<PizzaNetControls.OrdersRow> OrdersCollection { get; set; }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl))
                return;


            StockItemsCollection.Clear();

            Task.Factory.StartNew(LoadData);
        }

        private delegate void OneArgDelegate(IEnumerable<Ingredient> e);
        private bool isLoading = false;

        private void LoadData()
        {
            using (var db = new PizzaUnitOfWork())
            {
                Console.WriteLine("LoadDataStart");
                isLoading = true;

                var result = db.Ingredients.FindAll();
                Console.WriteLine("after query");


                Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new OneArgDelegate(PostData),
                        result);
                while (isLoading) ;
            }
        }

        private void PostData(IEnumerable<Ingredient> e)
        {
            Console.WriteLine("PostData start");
            foreach (var ingredient in e)
            {
                StockItemsCollection.Add(new StockItem(ingredient));
            }
            isLoading = false;
            Console.WriteLine("PostData end");
        }
        
    }
}
