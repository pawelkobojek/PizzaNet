using PizzaNetControls.Common;
using PizzaNetControls.Workers;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PizzaNetControls.Views
{
    public class RecipiesView : BaseView
    {
        public RecipiesView(IWorker worker) : base(worker)
        {
            this.IngredientsRowsCollection = new ObservableCollection<IngredientsRowWork>();
            this.RecipesCollection = new ObservableCollection<RecipeControl>();
            SelectedRecipe = -1;
        }

        public ObservableCollection<IngredientsRowWork> IngredientsRowsCollection { get; set; }
        public ObservableCollection<RecipeControl> RecipesCollection { get; set; }
        private const string TITLE = "PizzaNetWorkClient";
        private int SelectedRecipe { get; set; }

        private void showError(string message)
        {
            Utils.showError(TITLE, message);
        }

        internal void UpdateRecipe(int index)
        {
            RecipeControl rc = RecipesCollection[index];
            Worker.EnqueueTask(new WorkerTask(args =>
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

        internal void ChangeRecipeSelection(int index)
        {
            SelectedRecipe = index;
            for (int i = 0; i < IngredientsRowsCollection.Count; i++)
            {
                var item = IngredientsRowsCollection[i];
                bool res = null != RecipesCollection[index].Recipe.Ingredients.FirstOrDefault(
                ingr => ingr.IngredientID == item.Ingredient.IngredientID);
                IngredientsRowsCollection[i].Included = res;
            }
        }

        internal void RemoveRecipe(int index)
        {
            //MODIFIED
            //Recipe r = ((RecipeControl)RecipesContainer.SelectedItem).Recipe;
            //worker.EnqueueTask(new WorkerTask((args) =>
            //{
            //    using (var db = new PizzaUnitOfWork())
            //    {
            //        db.inTransaction(uof =>
            //        {
            //            Recipe rec = db.Recipies.Get(r.RecipeID);
            //            db.Recipies.Delete(rec);
            //            db.Commit();
            //            Console.WriteLine("Recipe " + rec.Name + " removed.");
            //        });
            //        return null;
            //    }
            //}, (s, args) =>
            //{
            //    RefreshRecipies();
            //}));
            Recipe r = RecipesCollection[index].Recipe;
            Worker.EnqueueTask(new WorkerTask((args) =>
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

        public void RefreshRecipies()
        {
            RecipesCollection.Clear();
            IngredientsRowsCollection.Clear();
            Worker.EnqueueTask(new WorkerTask((args) =>
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
            if (SelectedRecipe < 0) return;
            var rc = RecipesCollection[SelectedRecipe];
            var row = sender as IngredientsRowWork;
            if (row == null) return;
            Worker.EnqueueTask(new WorkerTask(args =>
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
                        showError("Unknown error");
                    res.First.Update(res.Second);
                    return;
                }
                else showError(exc.Message);
            },
            row, rc));
        }

        internal void AddRecipe()
        {
            Worker.EnqueueTask(new WorkerTask(args =>
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
    }
}
