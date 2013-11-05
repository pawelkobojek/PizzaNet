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
using PizzaNetControls.Worker;
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
            this.IngredientsRowsCollection = new ObservableCollection<IngredientsRowWork>();
            this.RecipesCollection = new ObservableCollection<RecipeControl>();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(OrdersCollection);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.State.StateValue", System.ComponentModel.ListSortDirection.Descending));

            /*#region example data
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
            #endregion*/
        }

        #region fields and properties
        public ObservableCollection<PizzaNetControls.StockItem> StockItemsCollection { get; set; }
        public ObservableCollection<PizzaNetControls.OrdersRow> OrdersCollection { get; set; }
        public ObservableCollection<PizzaNetControls.PizzaRow> PizzasCollection { get; set; }
        public ObservableCollection<OrderIngredient> IngredientsCollection { get; set; }
        public ObservableCollection<IngredientsRowWork> IngredientsRowsCollection { get; set; }
        public ObservableCollection<RecipeControl> RecipesCollection { get; set; }

        IngredientMonitor im = new IngredientMonitor();
        Ingredient lastSelectedIngredient = null;
        private const string ORDER_IMPOSSIBLE = "Action imposible! Not enough ingredient in stock!";
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

            if (RecipiesTab.IsSelected)
            {
                RefreshRecipies();
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
                        var result = db.Orders.FindAllEagerly();
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

        private void ordersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OrdersRow ord = ordersListView.SelectedItem as OrdersRow;
            if (ord != null)
            {
                Console.WriteLine("Order.Address: " + ord.Order.Address);
                Console.WriteLine("Order.CustomerPhone: " + ord.Order.CustomerPhone.ToString());
                Console.WriteLine("Order.Date: " + ord.Order.Date.ToString());
                Console.WriteLine("Order.State: " + ord.Order.State.ToString());
            }

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
            OrderIngredientForm form = new OrderIngredientForm(this, ings);
            form.ShowDialog();
            RefreshStockItems();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshOrders();
        }

        private void ButtonSetInRealisation_Click(object sender, RoutedEventArgs e)
        {
            Order o = ((OrdersRow)ordersListView.SelectedItem).Order;
            WorkerWindow worker = new WorkerWindow(this, (args) =>
                {
                    try
                    {
                        List<OrderIngredient> orderIngredients = new List<OrderIngredient>();
                        foreach (var od in o.OrderDetails)
                        {
                            foreach (var odIng in od.Ingredients)
                            {
                                orderIngredients.Add(odIng);
                            }
                        }
                        using (var db = new PizzaUnitOfWork())
                        {
                            foreach (var odIng in orderIngredients)
                            {
                                Ingredient i = db.Ingredients.Get(odIng.Ingredient.IngredientID);

                                if (i.StockQuantity - odIng.Quantity < 0)
                                {
                                    MessageBox.Show(ORDER_IMPOSSIBLE);
                                    return false;
                                }

                                i.StockQuantity -= odIng.Quantity;
                            }
                            db.Commit();
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                        return false;
                    }
                    return true;
                }, (a, s) =>
            {
                if ((bool)s.Result)
                    SetOrderStateInBackground(new State { StateValue = State.IN_REALISATION });
            }, null);
            worker.ShowDialog();
        }

        private void SetOrderStateInBackground(State state)
        {
            OrdersRow or = ordersListView.SelectedItem as OrdersRow;
            WorkerWindow worker = new WorkerWindow(this, (args) =>
            {
                using (var db = new PizzaUnitOfWork())
                {
                    Console.WriteLine("Set in realisation");

                    Order o = db.Orders.Get(or.Order.OrderID);
                    o.State.StateValue = state.StateValue;
                    db.Commit();

                    Console.WriteLine("Order " + or.Order.OrderID + " state set to IN REALISATION");
                    return null;
                }
            },
                (s, a) =>
                {
                    RefreshOrders();
                });
            worker.ShowDialog();
        }

        private void ButtonSetDone_Click(object sender, RoutedEventArgs e)
        {
            SetOrderStateInBackground(new State { StateValue = State.DONE });
        }

        private void ButtonRemoveOrder_Click(object sender, RoutedEventArgs e)
        {
            OrdersRow or = ordersListView.SelectedItem as OrdersRow;
            WorkerWindow worker = new WorkerWindow(this, (args) =>
            {
                using (var db = new PizzaUnitOfWork())
                {
                    Console.WriteLine("Remove order");

                    Order o = db.Orders.Get(or.Order.OrderID);
                    db.Orders.Delete(o);
                    db.Commit();
                    Console.WriteLine("Order " + or.Order.OrderID + " removed.");
                    return null;
                }
            },
                (s, a) =>
                {
                    RefreshOrders();
                });
            worker.ShowDialog();
        }

        private void RefreshRecipies()
        {
            RecipesCollection.Clear();
            IngredientsRowsCollection.Clear();
            var worker = new PizzaNetControls.Worker.WorkerWindow(this, (args) =>
            {
                try
                {
                    using (var db = new PizzaUnitOfWork())
                    {
                        Console.WriteLine("LoadDataStart");
                        var result = new Trio<IEnumerable<Recipe>, PizzaNetDataModel.Model.Size[], IEnumerable<Ingredient>>
                        {
                            First = db.Recipies.FindAllEagerly(),
                            Second = db.Sizes.FindAll().ToArray(),
                            Third = db.Ingredients.FindAll()
                        };

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
            }, (s, args) =>
            {
                var result = args.Result as Trio<IEnumerable<Recipe>, PizzaNetDataModel.Model.Size[], IEnumerable<Ingredient>>;
                if (result == null)
                {
                    Console.WriteLine("Result is null");
                    return;
                }
                foreach (var item in result.First)
                {
                    var rc = new RecipeControl();
                    rc.Recipe = item;
                    rc.RecalculatePrices(result.Second);
                    RecipesCollection.Add(rc);
                    Console.WriteLine(item.Name);
                }
                foreach (var item in result.Third)
                {
                    var row = new IngredientsRowWork(item, false);
                    row.ButtonIncludeChanged += row_PropertyChanged;
                    IngredientsRowsCollection.Add(row);
                }
            }, null);
            worker.ShowDialog();
        }

        void row_PropertyChanged(object sender, EventArgs e)
        {
            if (RecipesContainer.SelectedIndex < 0) return;
            var rc = RecipesCollection[RecipesContainer.SelectedIndex];
            var row = sender as IngredientsRowWork;
            if (row == null) return;
            new WorkerWindow(this, args =>
            {
                var rw = args[0] as IngredientsRowWork;
                if (rw == null) return null;
                var rec = args[1] as RecipeControl;
                if (rec == null) return null;
                try
                {
                    PizzaNetDataModel.Model.Size[] sizes = null;
                    using (var ctx = new PizzaUnitOfWork())
                    {
                        var recipe = ctx.Recipies.FindEagerly(rec.Recipe.RecipeID);
                        if (recipe.Count() != 1) throw new Exception("Incosistent data");
                        rec.Recipe = recipe.First();

                        var ingredient = ctx.Ingredients.Find(rw.Ingredient.IngredientID);
                        if (ingredient.Count() != 1) throw new Exception("Incosistent data");
                        rw.Ingredient = ingredient.First();

                        if (rec.Recipe.Ingredients.Contains(rw.Ingredient))
                        {
                            rec.Recipe.Ingredients.Remove(rw.Ingredient);
                        }
                        else
                        {
                            rec.Recipe.Ingredients.Add(rw.Ingredient);
                        }
                        sizes = ctx.Sizes.FindAll().ToArray();
                        ctx.Commit();
                    }
                    return new Pair<RecipeControl, PizzaNetDataModel.Model.Size[]> { First = rec, Second = sizes };
                }
                catch (Exception exc)
                {
                    return exc;
                }
            },
            (s, args) =>
            {
                var exc = args.Result as Exception;
                if (exc == null)
                {
                    var res = args.Result as Pair<RecipeControl, PizzaNetDataModel.Model.Size[]>;
                    if (res == null)
                        MessageBox.Show(exc.Message, "Unknown error", MessageBoxButton.OK, MessageBoxImage.Error);
                    res.First.Update(res.Second);
                    return;
                }
                else MessageBox.Show(exc.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            },
            row, rc).ShowDialog();
        }

        private void RecipesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != RecipesContainer) return;
            if (RecipesContainer.SelectedIndex < 0) return;
            for (int i = 0; i < IngredientsRowsCollection.Count; i++)
            {
                var item = IngredientsRowsCollection[i];
                bool res = null != RecipesCollection[RecipesContainer.SelectedIndex].Recipe.Ingredients.FirstOrDefault(
                ingr => ingr.IngredientID == item.Ingredient.IngredientID);
                IngredientsRowsCollection[i].Included = res;
            }
        }

        private void ButtonRemoveRecipe_Click(object sender, RoutedEventArgs e)
        {
            
            Recipe r = ((RecipeControl)RecipesContainer.SelectedItem).Recipe;
            WorkerWindow worker = new WorkerWindow(this, (args) =>
                {
                    using(var db = new PizzaUnitOfWork())
	                {
                        db.inTransaction(uof =>
                            {
                                Recipe rec = db.Recipies.Get(r.RecipeID);
                                db.Recipies.Delete(rec);
                                db.Commit();
                                Console.WriteLine("Recipe " + rec.Name + " removed.");
                            });
                        return null;
	                }
                }, (s, args) =>
                {
                    RefreshRecipies();
                });
            worker.ShowDialog();
        }

        private void ButtonAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            new WorkerWindow(this, args =>
                {
                    try
                    {
                        Recipe r = null;
                        using(var ctx = new PizzaUnitOfWork())
                        {
                            r = new Recipe { Name = "New recipe", Ingredients=new List<Ingredient>() };
                            ctx.Recipies.Insert(r);
                            ctx.Commit();
                        }
                        return r;
                    }
                    catch(Exception exc)
                    {
                        return exc;
                    }
                },
                (s, a) =>
                {
                    Exception exc = a.Result as Exception;
                    if (exc != null)
                    {
                        showError("Can't add recipe!");
                        return;
                    }
                    else
                    {
                        Recipe r = a.Result as Recipe;
                        if (r == null)
                        {
                            showError("Unknown error occured!");
                            return;
                        }
                        else RecipesCollection.Add(new RecipeControl() { Recipe = r });
                    }
                },
                null).ShowDialog();
        }

        private void TextBoxRecipeName_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (RecipesContainer.SelectedIndex < 0) return;
            RecipeControl rc = RecipesCollection[RecipesContainer.SelectedIndex];
            new WorkerWindow(this, args =>
                {
                    var recipe = args[0] as Recipe;
                    Recipe result = null;
                    if (recipe == null) return false;
                    string newName = recipe.Name;
                    try
                    {
                        using (var ctx = new PizzaUnitOfWork())
                        {
                            var res = ctx.Recipies.FindEagerly(recipe.RecipeID);
                            if (res == null || res.Count() != 1) return false;
                            result = res.First();
                            result.Name = newName;
                            ctx.Commit();
                        }
                        return result;
                    }
                    catch(Exception exc)
                    {
                        return exc;
                    }
                }, (s, args) =>
                {
                    var exc = args.Result as Exception;
                    if (exc!=null)
                    {
                        showError("Can't change recipe name!");
                    }
                    else
                    {
                        var bl = args.Result as bool?;
                        if (bl != null) showError("Unknown error occured!");
                        else
                        {
                            var rec = args.Result as Recipe;
                            if (rec == null) return;
                            rc.Recipe = rec;
                        }
                    }
                }, rc.Recipe).ShowDialog();
        }

        private void showError(string message)
        {
            MessageBox.Show(message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
