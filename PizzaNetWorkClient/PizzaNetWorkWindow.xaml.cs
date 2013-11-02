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
using PizzaNetDataModel.Monitors;
using System.Threading;

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
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.State.StateValue", System.ComponentModel.ListSortDirection.Descending));

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

        #region fields and properties
        public ObservableCollection<PizzaNetControls.StockItem> StockItemsCollection { get; set; }
        public ObservableCollection<PizzaNetControls.OrdersRow> OrdersCollection { get; set; }
        public ObservableCollection<PizzaNetControls.PizzaRow> PizzasCollection { get; set; }
        public ObservableCollection<OrderIngredient> IngredientsCollection { get; set; }
        public ObservableCollection<IngredientsRow> IngredientsRowsCollection { get; set; }
        public ObservableCollection<RecipeControl> RecipesCollection { get; set; }

        IngredientMonitor im = new IngredientMonitor();
        Ingredient lastSelectedIngredient = null;
        #endregion

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl) || !this.IsLoaded)
                return;

            if (StockTab.IsSelected)
            {
                RefreshStockItems();
            }
            else
            {
                if (lastSelectedIngredient != null)
                    Updater<IMonitor<Ingredient>, Ingredient>.Update(this, im, lastSelectedIngredient);
                lastSelectedIngredient = null;
            }

            if (OrdersTab.IsSelected)
            {
                RefreshOrders();
            }
        }

        private void RefreshOrders()
        {
            OrdersCollection.Clear();
            var worker = new PizzaNetControls.Worker.WorkerWindow(this, (args) =>
            {
                try
                {
                    using (var db = new PizzaUnitOfWork())
                    {
                        Console.WriteLine("Load Orders Start");
                        var result = db.Orders.FindAll();
                        Console.WriteLine("After query");
                        return result;
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                    return null;
                }
            }, (s, a) =>
            {
                IEnumerable<Order> orders = a.Result as IEnumerable<Order>;

                foreach (var order in orders)
                {
                    OrdersCollection.Add(new OrdersRow(order));
                }
            });

            worker.ShowDialog();
        }

        private void RefreshStockItems()
        {
            lastSelectedIngredient = null;
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
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    return null;
                }
            }, PostData, null);
            worker.ShowDialog();
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
            if (selected.Count > 0)
            {
                OrdersRow or = selected[0] as OrdersRow;
                if (or != null)
                {
                    foreach (var od in or.Order.OrderDetails)
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
            var worker = new PizzaNetControls.Worker.WorkerWindow(this, (args) =>
                {
                    using (var db = new PizzaUnitOfWork())
                    {
                        Ingredient ing = new Ingredient { Name = "New Ingredient", StockQuantity = 0, PricePerUnit = 1, NormalWeight = 1, ExtraWeight = 2 };
                        db.Ingredients.Insert(ing);
                        db.Commit();
                        Console.WriteLine("Commited " + ing.Name);
                        return ing;
                    }
                }, (s, a) =>
                {
                    Ingredient ing = a.Result as Ingredient;
                    if (ing == null)
                    {
                        Console.WriteLine("WARNING: Trying to add null ingredient!");
                        return;
                    }
                    StockItemsCollection.Add(new StockItem(ing));
                    Console.WriteLine("Added " + ing.Name);
                }, null);
            worker.ShowDialog();
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Remove clicked");
            if (listStock.SelectedIndex < 0) return;
            var worker = new PizzaNetControls.Worker.WorkerWindow(this, (args) =>
                {
                    using (var db = new PizzaUnitOfWork())
                    {
                        StockItem toRemove = args[0] as StockItem;
                        if (toRemove == null) return null;
                        db.Ingredients.Delete(toRemove.Ingredient);
                        db.Commit();
                        return toRemove;
                    }
                }, (s, args) =>
                {
                    StockItem toRemove = args.Result as StockItem;
                    if (toRemove == null)
                    {
                        Console.WriteLine("WARNING: Trying to remove null stock item!");
                        return;
                    }
                    StockItemsCollection.Remove(toRemove);
                    Console.WriteLine("Removed " + toRemove.Ingredient.Name);
                }, StockItemsCollection[listStock.SelectedIndex]);
            worker.ShowDialog();
        }

        private void listStock_LostFocus(object sender, RoutedEventArgs e)
        {
            // TODO: chyba trzeba zmienić Monitor żeby monitorował obiekty StockItem. Wtedy będzie można łatwiej
            //  wyciągać modyfikowany row.
            Console.WriteLine("Focus lost");
            if (listStock.SelectedIndex < 0) return;
            lastSelectedIngredient = StockItemsCollection[listStock.SelectedIndex].Ingredient;
            Updater<IMonitor<Ingredient>, Ingredient>.Update(this, im, lastSelectedIngredient);
            Console.WriteLine("Updated " + StockItemsCollection[listStock.SelectedIndex].Ingredient.Name);
        }

        private void TextBox_StockItemDetails_FocusChanged(object sender, RoutedEventArgs e)
        {
            if (lastSelectedIngredient != null)
                Updater<IMonitor<Ingredient>, Ingredient>.Update(this, im, lastSelectedIngredient);
        }

        private void ButtonOrderSupplies_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Ingredient> ings = new ObservableCollection<Ingredient>();
            foreach (var item in StockItemsCollection)
            {
                ings.Add(item.Ingredient);
            }
            OrderIngredientForm form = new OrderIngredientForm(ings);
            form.ShowDialog();
            RefreshStockItems();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshOrders();
        }
    }
}
