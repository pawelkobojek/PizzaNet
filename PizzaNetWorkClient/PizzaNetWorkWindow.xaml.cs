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
            this.PizzasCollection = new ObservableCollection<PizzaNetControls.PizzaRow>();
            this.IngredientsCollection = new ObservableCollection<OrderIngredient>();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(OrdersCollection);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.State.StateValue",System.ComponentModel.ListSortDirection.Descending));

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

            PizzaNetControls.OrdersRow o;
            for (int i = 0; i < 10; i++)
            {
                o = new PizzaNetControls.OrdersRow(new Order() 
                { 
                    OrderID = 12 * i, 
                    State = new State() { StateValue = i % 3 },
                    OrderDetails = new List<OrderDetail>() { new OrderDetail() 
                                                                { 
                                                                    OrderDetailID = 2*i,
                                                                    Ingredients = new List<OrderIngredient>()
                                                                    {
                                                                        new OrderIngredient() { Ingredient = new Ingredient() {Name = String.Format("Ingredient #{0}",4*i)}, Quantity=20*(i%2+1)},
                                                                        new OrderIngredient() { Ingredient = new Ingredient() {Name = String.Format("Ingredient #{0}",4*i+1)}, Quantity=30*(i%2+1)}
                                                                    }
                                                                },
                                                             new OrderDetail() { OrderDetailID = 2*i+1,
                                                                    Ingredients = new List<OrderIngredient>()
                                                                    {
                                                                        new OrderIngredient() { Ingredient = new Ingredient() {Name = String.Format("Ingredient #{0}",4*i+2)}, Quantity=40*(i%2+1)},
                                                                        new OrderIngredient() { Ingredient = new Ingredient() {Name = String.Format("Ingredient #{0}",4*i+3)}, Quantity=50*(i%2+1)}
                                                                    }
                                                             }}
                });
                OrdersCollection.Add(o);
            }
            #endregion
        }

        public ObservableCollection<PizzaNetControls.StockItem> StockItemsCollection { get; set; }

        public ObservableCollection<PizzaNetControls.OrdersRow> OrdersCollection { get; set; }

        public ObservableCollection<PizzaNetControls.PizzaRow> PizzasCollection { get; set; }

        public ObservableCollection<OrderIngredient> IngredientsCollection { get; set; }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if (!(e.OriginalSource is TabControl))
                return;


            StockItemsCollection.Clear();

            Task.Factory.StartNew(LoadData);*/
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

        private void ordersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PizzasCollection.Clear();
            IList selected = e.AddedItems;
            if (selected.Count>0)
            {
                OrdersRow or = selected[0] as OrdersRow;
                if (or != null)
                {
                    foreach(var od in or.Order.OrderDetails)
                    {
                        PizzasCollection.Add(new PizzaRow(od));
                    }
                }
            }
        }

        private void pizzasListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IngredientsCollection.Clear();
            IList selected = e.AddedItems;
            if (selected.Count > 0)
            {
                PizzaRow pr = selected[0] as PizzaRow;
                if (pr != null)
                {
                    foreach (var ingr in pr.OrderDetail.Ingredients)
                    {
                        IngredientsCollection.Add(ingr);
                    }
                }
            }
        }
        
    }
}
