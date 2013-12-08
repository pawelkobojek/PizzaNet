using PizzaNetControls.Common;
using PizzaNetControls.Controls;
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
using PizzaNetCommon.Requests;
using PizzaNetCommon.DTOs;

namespace PizzaNetControls.Views
{
    public class StockView : BaseView
    {
        public ObservableCollection<StockItem> StockItemsCollection { get; set; }
        private const string ING_REMOVE_IMPOSSIBLE = "Can't remove this ingredient because there are recipies containing it";
        private const string TITLE = "PizzaNetWorkClient";
        private const string ADDRESS = "http://localhost:60499/PizzaService.svc";

        public StockView(IWorker worker) : base(worker)
        {
            this.StockItemsCollection = new ObservableCollection<StockItem>();
        }

        private void showError(string message)
        {
            Utils.showError(TITLE, message);
        }

        internal void AddIngredient()
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                using (var db = new PizzaUnitOfWork())
                {
                    return db.inTransaction(uof =>
                    {
                        Ingredient ing = new Ingredient { Name = "New Ingredient", StockQuantity = 0, PricePerUnit = 1, NormalWeight = 1, ExtraWeight = 2, Recipies = new List<Recipe>() };
                        uof.Db.Ingredients.Insert(ing);
                        uof.Db.Commit();
                        Console.WriteLine("Commited " + ing.Name);
                        return ing;
                    });
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
            }));
        }

        internal void RemoveIngredient(int index)
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                using (var db = new PizzaUnitOfWork())
                {
                    return db.inTransaction(uof =>
                    {
                        StockItem toRemove = args[0] as StockItem;
                        if (toRemove == null) return null;
                        if (toRemove.Ingredient.Recipies.Count != 0)
                        {
                            return null;
                        }
                        uof.Db.Ingredients.Delete(toRemove.Ingredient);

                        uof.Db.Commit();
                        return toRemove;
                    });
                }
            }, (s, args) =>
            {
                StockItem toRemove = args.Result as StockItem;
                if (toRemove == null)
                {
                    //Console.WriteLine("WARNING: Trying to remove null stock item!");
                    MessageBox.Show(ING_REMOVE_IMPOSSIBLE);
                    return;
                }
                StockItemsCollection.Remove(toRemove);
                Console.WriteLine("Removed " + toRemove.Ingredient.Name);
            }, StockItemsCollection[index]));
        }

        public void RefreshStockItems()
        {
            StockItemsCollection.Clear();

            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    using (var proxy = new WorkChannel(ADDRESS))
                    {
                        return proxy.GetIngredients(new EmptyRequest { Login = "Admin", Password = "123" });
                    }
                    //using (var db = new PizzaUnitOfWork())
                    //{
                    //    return db.inTransaction(uof =>
                    //    {
                    //        Console.WriteLine("LoadDataStart");

                    //        var result = uof.Db.Ingredients.FindAllIncludeRecipies();
                    //        Console.WriteLine("after query");

                    //        Console.WriteLine("Result is null: {0}", result == null);

                    //        return result;
                    //    });
                    //}
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    return null;
                }
            }, PostData, null));
        }

        private void PostData(object sender, PizzaNetControls.Workers.WorkFinishedEventArgs e)
        {
            if (e.Result == null)
            {
                Console.WriteLine("Result is null");
                return;
            }
            IList<StockIngredientDTO> ings = ((ListResponse<StockIngredientDTO>)e.Result).Data;
            foreach (var item in ings)
            {
                Console.WriteLine(item.Name);
                StockItemsCollection.Add(new StockItem(item));
            }
        }

        internal void UpdateStockItem(int index)
        {
            StockItem ingr = StockItemsCollection[index];
            Worker.EnqueueTask(new WorkerTask(args =>
            {
                var i = args[0] as Ingredient;
                Ingredient result = null;
                if (i == null) return false;
                try
                {
                    using (var ctx = new PizzaUnitOfWork())
                    {
                        bool b = ctx.inTransaction(uof =>
                        {
                            var res = ctx.Ingredients.Find(i.IngredientID);
                            if (res == null || res.Count() != 1) return false;
                            result = res.First();
                            result.IngredientID = i.IngredientID;
                            result.Name = i.Name;
                            result.NormalWeight = i.NormalWeight;
                            result.ExtraWeight = i.ExtraWeight;
                            result.PricePerUnit = i.PricePerUnit;
                            ctx.Commit();
                            return true;
                        });
                        if (!b) return b;
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
                    showError("Can't change ingredient detail!");
                }
                else
                {
                    var bl = args.Result as bool?;
                    if (bl != null) showError("Unknown error occured!");
                    else
                    {
                        var rec = args.Result as Ingredient;
                        if (rec == null) return;
                        ingr.Ingredient = rec;
                    }
                }
            }, ingr.Ingredient));
        }
    }
}
