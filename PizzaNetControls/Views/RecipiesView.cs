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
using PizzaNetWorkClient.WCFClientInfrastructure;
using PizzaNetControls.Configuration;
using PizzaNetCommon.Requests;
using PizzaNetCommon.DTOs;

namespace PizzaNetControls.Views
{
    public class RecipiesView : BaseView
    {
        public RecipiesView(IWorker worker)
            : base(worker)
        {
            this.IngredientsRowsCollection = new ObservableCollection<IngredientsRowWork>();
            this.RecipesCollection = new ObservableCollection<RecipeControl>();
            this.RemovedRecipes = new List<RecipeDTO>(5);
            SelectedRecipe = -1;
            Modified = false;
        }

        public ObservableCollection<IngredientsRowWork> IngredientsRowsCollection { get; set; }
        public ObservableCollection<RecipeControl> RecipesCollection { get; set; }
        public List<RecipeDTO> RemovedRecipes { get; set; }
        private const string TITLE = "PizzaNetWorkClient";
        private int SelectedRecipe { get; set; }
        public bool Modified { get; set; }

        private void showError(string message)
        {
            Utils.showError(TITLE, message);
        }

        internal void UpdateRecipe(int index)
        {
            Modified = true;
            RecipesCollection[index].Update(Sizes);
            //TODO
            //RecipeControl rc = RecipesCollection[index];
            //Worker.EnqueueTask(new WorkerTask(args =>
            //{
            //    var recipe = args[0] as Recipe;
            //    Recipe result = null;
            //    if (recipe == null) return false;
            //    string newName = recipe.Name;
            //    try
            //    {
            //        using (var ctx = new PizzaUnitOfWork())
            //        {
            //            var res = ctx.Recipies.FindEagerly(recipe.RecipeID);
            //            if (res == null || res.Count() != 1) return false;
            //            result = res.First();
            //            result.Name = newName;
            //            ctx.Commit();
            //        }
            //        return result;
            //    }
            //    catch (Exception exc)
            //    {
            //        return exc;
            //    }
            //}, (s, args) =>
            //{
            //    var exc = args.Result as Exception;
            //    if (exc != null)
            //    {
            //        showError("Can't change recipe name!");
            //    }
            //    else
            //    {
            //        var bl = args.Result as bool?;
            //        if (bl != null) showError("Unknown error occured!");
            //        else
            //        {
            //            var rec = args.Result as Recipe;
            //            if (rec == null) return;
            //            rc.Recipe = rec;
            //        }
            //    }
            //}, rc.Recipe));
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
            Modified = true;
            RemovedRecipes.Add(RecipesCollection[index].Recipe);
            RecipesCollection.RemoveAt(index);
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
            //Recipe r = RecipesCollection[index].Recipe;
            //Worker.EnqueueTask(new WorkerTask((args) =>
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
        }

        public void RefreshRecipies()
        {
            //TODO
            RecipesCollection.Clear();
            IngredientsRowsCollection.Clear();
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    using (var proxy = new WorkChannel(ClientConfig.getConfig().ServerAddress))
                    {
                        return proxy.GetRecipeTabData(new EmptyRequest
                        {
                            Login = ClientConfig.getConfig().User.Email,
                            Password = ClientConfig.getConfig().User.Password
                        });
                    }
                    //        using (var db = new PizzaUnitOfWork())
                    //        {
                    //            return db.inTransaction(uof =>
                    //            {
                    //                Console.WriteLine("LoadDataStart");
                    //                var result = new Trio<IEnumerable<Recipe>, PizzaNetDataModel.Model.Size[], IEnumerable<Ingredient>>
                    //                {
                    //                    First = db.Recipies.FindAllEagerly(),
                    //                    Second = db.Sizes.FindAll().ToArray(),
                    //                    Third = db.Ingredients.FindAll()
                    //                };

                    //                Console.WriteLine("after query");

                    //                Console.WriteLine("Result is null: {0}", result == null);

                    //                return result;
                    //            });
                    //        }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    return null;
                }
            }, (s, args) =>
            {
                var result = args.Result as TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>>;
                //    var result = args.Result as Trio<IEnumerable<Recipe>, PizzaNetDataModel.Size[], IEnumerable<Ingredient>>;
                if (result == null)
                {
                    Console.WriteLine("Result is null");
                    return;
                }
                foreach (var item in result.First)
                {
                    var rc = new RecipeControl();
                    rc.Recipe = item;
                    rc.Update(result.Second.ToArray());
                    RecipesCollection.Add(rc);
                }
                foreach (var item in result.Third)
                {
                    var row = new IngredientsRowWork(item, false);
                    row.ButtonIncludeChanged += row_PropertyChanged;
                    IngredientsRowsCollection.Add(row);
                }

                Sizes = result.Second.ToArray();
                //    foreach (var item in result.First)
                //    {
                //        var rc = new RecipeControl();
                //        rc.Recipe = item;
                //        rc.RecalculatePrices(result.Second.ToArray());
                //        RecipesCollection.Add(rc);
                //        Console.WriteLine(item.Name);
                //    }
                //    foreach (var item in result.Third)
                //    {
                //        var row = new IngredientsRowWork(item, false);
                //        row.ButtonIncludeChanged += row_PropertyChanged;
                //        IngredientsRowsCollection.Add(row);
                //    }
            }));
        }

        void row_PropertyChanged(object sender, EventArgs e)
        {
            //TODO ZROBIC
            if (SelectedRecipe < 0) return;
            var rc = RecipesCollection[SelectedRecipe];
            var row = sender as IngredientsRowWork;
            if (row == null) return;

            //int index = rc.Recipe.Ingredients.IndexOf(row.Ingredient);
            int index = -1;
            for (int i = 0; i < rc.Recipe.Ingredients.Count; i++)
            {
                if (rc.Recipe.Ingredients[i].IngredientID == row.Ingredient.IngredientID)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                rc.Recipe.Ingredients.RemoveAt(index);
            }
            else
            {
                rc.Recipe.Ingredients.Add(row.Ingredient);
            }

            UpdateRecipe(SelectedRecipe);
            //Worker.EnqueueTask(new WorkerTask(args =>
            //{
            //    var rw = args[0] as IngredientsRowWork;
            //    if (rw == null) return null;
            //    var rec = args[1] as RecipeControl;
            //    if (rec == null) return null;
            //    try
            //    {
            //        PizzaNetDataModel.Model.Size[] sizes = null;
            //        using (var ctx = new PizzaUnitOfWork())
            //        {
            //            return ctx.inTransaction(uof =>
            //            {
            //                var recipe = uof.Db.Recipies.FindEagerly(rec.Recipe.RecipeID);
            //                if (recipe.Count() != 1) throw new Exception("Incosistent data");
            //                rec.Recipe = recipe.First();

            //                var ingredient = uof.Db.Ingredients.Find(rw.Ingredient.IngredientID);
            //                if (ingredient.Count() != 1) throw new Exception("Incosistent data");
            //                rw.Ingredient = ingredient.First();

            //                if (rec.Recipe.Ingredients.Contains(rw.Ingredient))
            //                {
            //                    rec.Recipe.Ingredients.Remove(rw.Ingredient);
            //                }
            //                else
            //                {
            //                    rec.Recipe.Ingredients.Add(rw.Ingredient);
            //                }
            //                sizes = uof.Db.Sizes.FindAll().ToArray();
            //                uof.Db.Commit();

            //                return new Pair<RecipeControl, PizzaNetDataModel.Model.Size[]> { First = rec, Second = sizes };
            //            });
            //        }
            //    }
            //    catch (Exception exc)
            //    {
            //        return exc;
            //    }
            //},
            //(s, args) =>
            //{
            //    var exc = args.Result as Exception;
            //    if (exc == null)
            //    {
            //        var res = args.Result as Pair<RecipeControl, PizzaNetDataModel.Model.Size[]>;
            //        if (res == null)
            //            showError("Unknown error");
            //        res.First.Update(res.Second);
            //        return;
            //    }
            //    else showError(exc.Message);
            //},
            //row, rc));
        }

        internal void AddRecipe()
        {
            Modified = true;
            RecipesCollection.Add(new RecipeControl
            {
                Recipe = new RecipeDTO
                {
                    RecipeID = -1,
                    Name = "New Recipe",
                    Ingredients = new List<OrderIngredientDTO>()
                }
            });
            RecipesCollection[RecipesCollection.Count-1].Update(Sizes);
            //TODO
            //Worker.EnqueueTask(new WorkerTask(args =>
            //{
            //    try
            //    {
            //        Recipe r = null;
            //        using (var ctx = new PizzaUnitOfWork())
            //        {
            //            ctx.inTransaction(uof =>
            //            {
            //                r = new Recipe { Name = "New recipe", Ingredients = new List<Ingredient>() };
            //                uof.Db.Recipies.Insert(r);
            //                uof.Db.Commit();
            //            });
            //        }
            //        return r;
            //    }
            //    catch (Exception exc)
            //    {
            //        return exc;
            //    }
            //},
            //    (s, a) =>
            //    {
            //        Exception exc = a.Result as Exception;
            //        if (exc != null)
            //        {
            //            showError("Can't add recipe!");
            //            return;
            //        }
            //        else
            //        {
            //            Recipe r = a.Result as RecipeDTO;
            //            if (r == null)
            //            {
            //                showError("Unknown error occured!");
            //                return;
            //            }
            //            else RecipesCollection.Add(new RecipeControl() { Recipe = r });
            //        }
            //    }));
        }

        internal void SaveChanges()
        {
            Worker.EnqueueTask(new WorkerTask(args =>
                {
                    try
                    {
                        using (var proxy = new WorkChannel(ClientConfig.getConfig().ServerAddress))
                        {
                            List<RecipeDTO> toUpdate = new List<RecipeDTO>();
                            foreach (var recControl in RecipesCollection)
                            {
                                toUpdate.Add(recControl.Recipe);
                            }
                            return proxy.UpdateOrRmoveRecipe(new UpdateOrRemoveRequest<List<RecipeDTO>>
                            {
                                Data = toUpdate,
                                DataToRemove = RemovedRecipes,
                                Login = ClientConfig.getConfig().User.Email,
                                Password = ClientConfig.getConfig().User.Password
                            });
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                        Console.WriteLine("Failed");
                        return null;
                    }
                }, (s, e) =>
                    {
                        var result = e.Result as TrioResponse<List<RecipeDTO>, List<OrderIngredientDTO>, int>;
                        if (result == null)
                        {
                            Utils.showError("blablabla", "asd");
                            return;
                        }

                        RecipesCollection.Clear();
                        IngredientsRowsCollection.Clear();
                        RemovedRecipes.Clear();
                        foreach (var item in result.First)
                        {
                            var rc = new RecipeControl();
                            rc.Recipe = item;
                            //rc.RecalculatePrices(result.Second.ToArray());
                            rc.Update(Sizes);
                            RecipesCollection.Add(rc);
                        }
                        foreach (var item in result.Second)
                        {
                            var row = new IngredientsRowWork(item, false);
                            row.ButtonIncludeChanged += row_PropertyChanged;
                            IngredientsRowsCollection.Add(row);
                        }
                    }, null));
        }

        public SizeDTO[] Sizes { get; set; }
    }
}
