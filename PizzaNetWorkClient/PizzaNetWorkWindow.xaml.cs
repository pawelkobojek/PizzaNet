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
using PizzaNetControls.Workers;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using PizzaNetControls.Dialogs;
using PizzaNetControls.Controls;
using PizzaNetControls.Common;

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
            this.IngredientsRowsCollection = new ObservableCollection<IngredientsRowWork>();
            this.RecipesCollection = new ObservableCollection<RecipeControl>();

            this.worker.Lock = this.tabControl;
        }

        #region fields and properties
        public ObservableCollection<IngredientsRowWork> IngredientsRowsCollection { get; set; }
        public ObservableCollection<RecipeControl> RecipesCollection { get; set; }

        #endregion

        private void showError(string message)
        {
            Utils.showError(this.Title, message);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.OriginalSource is TabControl) || !this.IsLoaded)
                return;

            if (StockTab.IsSelected)
            {
                stockViewModel.StockView.RefreshStockItems();
            }

            if (OrdersTab.IsSelected)
            {
                ordersViewModel.WorkOrdersView.RefreshCurrentOrders();
            }

            if (RecipiesTab.IsSelected)
            {
                RefreshRecipies();
            }
        }

        private void RefreshRecipies()
        {
            RecipesCollection.Clear();
            IngredientsRowsCollection.Clear();
            worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    using (var db = new PizzaUnitOfWork())
                    {
                        return db.inTransaction(uof =>
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
                        });
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
            }));
        }

        void row_PropertyChanged(object sender, EventArgs e)
        {
            if (RecipesContainer.SelectedIndex < 0) return;
            var rc = RecipesCollection[RecipesContainer.SelectedIndex];
            var row = sender as IngredientsRowWork;
            if (row == null) return;
            worker.EnqueueTask(new WorkerTask(args =>
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
                        return ctx.inTransaction(uof =>
                        {
                            var recipe = uof.Db.Recipies.FindEagerly(rec.Recipe.RecipeID);
                            if (recipe.Count() != 1) throw new Exception("Incosistent data");
                            rec.Recipe = recipe.First();

                            var ingredient = uof.Db.Ingredients.Find(rw.Ingredient.IngredientID);
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
                            sizes = uof.Db.Sizes.FindAll().ToArray();
                            uof.Db.Commit();

                            return new Pair<RecipeControl, PizzaNetDataModel.Model.Size[]> { First = rec, Second = sizes };
                        });
                    }
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
            row, rc));
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
            worker.EnqueueTask(new WorkerTask((args) =>
                {
                    using (var db = new PizzaUnitOfWork())
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
                }));
        }

        private void ButtonAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            worker.EnqueueTask(new WorkerTask(args =>
                {
                    try
                    {
                        Recipe r = null;
                        using (var ctx = new PizzaUnitOfWork())
                        {
                            ctx.inTransaction(uof =>
                            {
                                r = new Recipe { Name = "New recipe", Ingredients = new List<Ingredient>() };
                                uof.Db.Recipies.Insert(r);
                                uof.Db.Commit();
                            });
                        }
                        return r;
                    }
                    catch (Exception exc)
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
                }));
        }

        private void TextBoxRecipeName_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (RecipesContainer.SelectedIndex < 0) return;
            RecipeControl rc = RecipesCollection[RecipesContainer.SelectedIndex];
            worker.EnqueueTask(new WorkerTask(args =>
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
                    catch (Exception exc)
                    {
                        return exc;
                    }
                }, (s, args) =>
                {
                    var exc = args.Result as Exception;
                    if (exc != null)
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
                }, rc.Recipe));
        }
        
        private void TextBoxRecipeName_KeyDown(object sender, KeyEventArgs e)
        {
            var txtb = sender as TextBox;
            if (txtb == null) return;
            if (RecipesContainer.SelectedIndex < 0) return;
            RecipeControl rc = RecipesCollection[RecipesContainer.SelectedIndex];
            if (e.Key == Key.Return)
            {
                if (rc.Recipe.Name != txtb.Text)
                    txtb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
