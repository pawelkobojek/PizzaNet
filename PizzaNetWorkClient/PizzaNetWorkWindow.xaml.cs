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
            this.IngredientsRowsCollection = new ObservableCollection<IngredientsRow>();
            this.RecipesCollection = new ObservableCollection<RecipeControl>();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(OrdersCollection);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.State.StateValue",System.ComponentModel.ListSortDirection.Descending));

            #region example data
            #region recipes
            var c = new PizzaNetControls.IngredientsRow(new Ingredient() 
            { 
                Name = "Ingredient1",
                NormalWeight = 200,
                ExtraWeight = 300
            });
            this.IngredientsRowsCollection.Add(c);
            for (int i = 0; i < 10; i++)
            {
                c = new PizzaNetControls.IngredientsRow(new Ingredient() 
                { 
                    Name = "Mozzarella Cheese",
                    NormalWeight = 100,
                    ExtraWeight = 200
                });
                c.CurrentQuantity = c.Ingredient.NormalWeight;
                this.IngredientsRowsCollection.Add(c);
            }

            PizzaNetControls.RecipeControl d;
            for (int i = 0; i < 10; i++)
            {
                d = new PizzaNetControls.RecipeControl();
                d.RecipeName = "MyRecipeName";
                d.Prices = new PizzaNetControls.PriceData() { PriceLow = 10, PriceMed = 20, PriceHigh = 30 };
                d.Ingredients = new List<string>() { "Mozarella Cheese", "Mushrooms", "Ingredient3" };
                d.Width = 300;
                this.RecipesCollection.Add(d);
            }
            #endregion

            #region stock items
            PizzaNetControls.StockItem st;
            for (int i = 0; i < 20; i++)
            {
                st = new PizzaNetControls.StockItem(new Ingredient()
                    {
                        IngredientID = 13,
                        Name = "ItemName",
                        StockQuantity = 100,
                        NormalWeight = 10,
                        ExtraWeight = 20,
                        PricePerUnit = 1.2M
                   });
                StockItemsCollection.Add(st);
            }
            #endregion

            #region orders
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
            #endregion
        }

        public ObservableCollection<PizzaNetControls.StockItem> StockItemsCollection { get; set; }

        public ObservableCollection<PizzaNetControls.OrdersRow> OrdersCollection { get; set; }

        public ObservableCollection<PizzaNetControls.PizzaRow> PizzasCollection { get; set; }

        public ObservableCollection<OrderIngredient> IngredientsCollection { get; set; }

        public ObservableCollection<IngredientsRow> IngredientsRowsCollection { get; set; }

        public ObservableCollection<RecipeControl> RecipesCollection { get; set; }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl))
                return;
            if (StockTab.IsSelected)
            {
                StockItemsCollection.Clear();

                var worker = new PizzaNetControls.Worker.WorkerWindow(this, (args) =>
                    {
                        try
                        {
                            using (var db = new PizzaUnitOfWork())
                            {
                                Console.WriteLine("LoadDataStart");

                                var result = db.Ingredients.FindAll();
                                Console.WriteLine("after query");

                                Console.WriteLine("Result is null: {0}", result == null);

                                return result;
                            }
                        }
                        catch(Exception exc)
                        {
                            Console.WriteLine(exc.Message);
                            return null;
                        }
                    });
                worker.WorkFinished += PostData;
                worker.ShowDialog();

                //Task.Factory.StartNew(LoadData);
            }
        }

        private void PostData(object sender, PizzaNetControls.Worker.WorkFinishedEventArgs e)
        {
            if (e.Result == null)
            {
                Console.WriteLine("Result is null");
                return;
            }
            foreach (var item in (IEnumerable<Ingredient>)e.Result)
            {
                Console.WriteLine(item.Name);
                StockItemsCollection.Add(new StockItem(item));
            }
        }

        private delegate void OneArgDelegate(IEnumerable<Ingredient> e);

        private void LoadData()
        {
            using (var db = new PizzaUnitOfWork())
            {
                Console.WriteLine("LoadDataStart");

                var result = db.Ingredients.FindAll();
                Console.WriteLine("after query");

                DispatcherOperation dop = Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new OneArgDelegate(PostData),
                        result);

                dop.Wait();
            }
        }

        private void PostData(IEnumerable<Ingredient> e)
        {
            Console.WriteLine("PostData start");
            foreach (var ingredient in e)
            {
                StockItemsCollection.Add(new StockItem(ingredient));
            }
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

        IngredientMonitor im = new IngredientMonitor();
        private void listStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is ListView) || listStock.SelectedIndex < 0)
                return;

            Console.WriteLine("Zaznaczono " + listStock.SelectedIndex);
            Console.WriteLine("CollectionCount: " + StockItemsCollection.Count);
            im.StartMonitor(StockItemsCollection[listStock.SelectedIndex].Ingredient);
        }

        private void ButtonAddIngredient_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("AddIngredient click");
            using (var db = new PizzaUnitOfWork())
            {
                Ingredient ing = new Ingredient { Name = "Test", StockQuantity = 1, PricePerUnit = 1, NormalWeight = 1, ExtraWeight = 1 };
                db.Ingredients.Insert(ing);
                db.Commit();

                StockItemsCollection.Add(new StockItem(ing));

                Console.WriteLine("Added " + ing.Name);
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Remove clicked");
            using (var db = new PizzaUnitOfWork())
            {
                StockItem toRemove = StockItemsCollection[listStock.SelectedIndex];
                db.Ingredients.Delete(toRemove.Ingredient);
                db.Commit();
                StockItemsCollection.Remove(toRemove);

                Console.WriteLine("Removed " + toRemove.Ingredient.Name);
            }
        }

        private void listStock_LostFocus(object sender, RoutedEventArgs e)
        {
            // TODO: chyba trzeba zmienić Monitor żeby monitorował obiekty StockItem. Wtedy będzie można łatwiej
            //  wyciągać modyfikowany row.
            Console.WriteLine("Focus lost");
            im.Update(StockItemsCollection[listStock.SelectedIndex].Ingredient);
            Console.WriteLine("Updated " + StockItemsCollection[listStock.SelectedIndex].Ingredient.Name);
        }


        
    }
}
