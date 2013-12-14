using PizzaNetControls.Common;
using PizzaNetControls.Configuration;
using PizzaNetControls.Workers;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PizzaNetWorkClient.WCFClientInfrastructure;
using PizzaNetCommon.Requests;
using PizzaNetCommon.DTOs;

namespace PizzaNetControls.Views
{
    public class ClientMainView : BaseView
    {
        public ClientMainView(IWorker worker)
            : base(worker)
        {
            this.IngredientsCollection = new ObservableCollection<PizzaNetControls.IngredientsRow>();
            this.RecipesCollection = new ObservableCollection<PizzaNetControls.RecipeControl>();
            this.OrderedPizzasCollection = new ObservableCollection<IngredientsList>();
            this.Ingredients = new List<OrderIngredientDTO>();
        }

        internal void AddOrder()
        {
            var ingr = new List<OrderIngredientDTO>();
            foreach (var item in IngredientsCollection)
            {
                if (item.CurrentQuantity > 0)
                    ingr.Add(new OrderIngredientDTO()
                    {
                        ExtraWeight = item.Ingredient.ExtraWeight,
                        NormalWeight = item.Ingredient.NormalWeight,
                        IngredientID = item.Ingredient.IngredientID,
                        Name = item.Ingredient.Name,
                        Quantity = item.CurrentQuantity
                    });
            }
            var orderDetail = new OrderDetailDTO()
            {
                Ingredients = ingr,
                Size = CurrentSizeValue
            };
            OrderedPizzasCollection.Add(new IngredientsList(orderDetail));
        }

        public ObservableCollection<PizzaNetControls.RecipeControl> RecipesCollection { get; set; }
        public ObservableCollection<PizzaNetControls.IngredientsList> OrderedPizzasCollection { get; set; }
        public ObservableCollection<PizzaNetControls.IngredientsRow> IngredientsCollection { get; set; }

        private SizeDTO _currentSizeValue;
        public SizeDTO CurrentSizeValue
        {
            get { return _currentSizeValue; }
            set { _currentSizeValue = value; NotifyPropertyChanged("CurrentSizeValue"); }
        }

        internal void RemoveFromOrder(int index)
        {
            // MODIFIED
            //if (orderedPizzasContainer.SelectedIndex < 0) return;
            //OrderedPizzasCollection.RemoveAt(orderedPizzasContainer.SelectedIndex);
            if (index < 0) return;
            OrderedPizzasCollection.RemoveAt(index);
        }

        internal void ClearOrder()
        {
            OrderedPizzasCollection.Clear();
        }

        internal void Order()
        {
            List<OrderDetailDTO> details = new List<OrderDetailDTO>(OrderedPizzasCollection.Count);
            foreach (var item in OrderedPizzasCollection)
                details.Add(item.OrderDetail);

            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                var det = args[0] as List<OrderDetailDTO>;
                try
                {
                    using (var proxy = new WorkChannel())
                    {
                        proxy.MakeOrder(new UpdateRequest<OrderDTO>
                        {
                            Data = new OrderDTO
                                {
                                    Address = ClientConfig.CurrentUser.Address,
                                    Date = DateTime.Now,
                                    CustomerPhone = ClientConfig.CurrentUser.Phone,
                                    OrderDetailsDTO = det
                                },
                            Login = ClientConfig.CurrentUser.Email,
                            Password = ClientConfig.CurrentUser.Password
                        });
                    }
                    return true;
                }
                catch (Exception e)
                {
                    return e;
                }
                //var cfg = args[0] as ClientConfig;
                //var det = args[1] as List<OrderDetail>;
                //if (cfg == null || det == null) return false;
                //try
                //{
                //    using (var ctx = new PizzaUnitOfWork())
                //    {
                //        ctx.inTransaction(uof =>
                //        {
                //            det = mergeIngredients(det, uof.Db.Ingredients.FindAll());
                //            det = mergeSizes(det, uof.Db.Sizes.FindAll());

                //            foreach (var d in det)
                //                foreach (var ingr in d.Ingredients)
                //                    ingr.Quantity = (int)(ingr.Quantity * d.Size.SizeValue);

                //            uof.Db.Orders.Insert(new Order()
                //            {
                //                Address = cfg.User.Address,
                //                CustomerPhone = cfg.User.Phone,
                //                Date = DateTime.Now,
                //                OrderDetails = det,
                //                State = new State() { StateValue = State.NEW }
                //            });
                //            uof.Db.Commit();
                //        });
                //    }
                //}
                //catch (Exception)
                //{
                //    return false;
                //}
                //return true;
            }, (s, args) =>
            {
                if (args.Result is Exception)
                {
                    Utils.HandleException(args.Result as Exception);
                    return;
                }
                bool b = (args.Result as bool?) ?? false;
                if (b)
                {
                    Utils.showInformation(Utils.Messages.ORDERED_SUCCESSFULLY);
                    OrderedPizzasCollection.Clear();
                }
                else
                    Utils.showExclamation(Utils.Messages.ORDERING_ERROR);
            }, details));
        }

        internal void ChangeCurrentSize(SizeDTO size)
        {
            // MODIFIED
            //RadioButton rb = sender as RadioButton;
            //if (rb == null) return;
            //var value = rb.Tag as PizzaNetDataModel.Model.Size;
            //if (value != null)
            //{
            //    CurrentSizeValue = value;
            //    foreach (var item in IngredientsCollection)
            //        item.CurrentSize = value;
            //}
            //RecalculatePrice();

            CurrentSizeValue = size;
            foreach (var item in IngredientsCollection)
                item.CurrentSize = size;
            RecalculatePrice();
        }

        public List<OrderIngredientDTO> Ingredients { get; set; }

        internal void ChangeSelectedRecipe(int index)
        {
            // MODIFIED
            //if (e.OriginalSource != RecipesContainer) return;
            //if (RecipesContainer.SelectedIndex < 0) return;
            //bool[] quantities = new bool[Ingredients.Count];
            //foreach (var i in RecipesCollection[RecipesContainer.SelectedIndex].Recipe.Ingredients)
            //{
            //    int ind = Ingredients.FindIndex((ing) => { return ing.IngredientID == i.IngredientID; });
            //    if (ind > 0)
            //        quantities[ind] = true;
            //}
            //SetCurrentQuantities(quantities);
            bool[] quantities = new bool[Ingredients.Count];
            foreach (var i in RecipesCollection[index].Recipe.Ingredients)
            {
                int ind = Ingredients.FindIndex((ing) => { return ing.IngredientID == i.IngredientID; });
                if (ind >= 0)
                    quantities[ind] = true;
            }
            SetCurrentQuantities(quantities);
        }

        private void RecalculatePrice()
        {
            PizzaInfoPrice = PriceCalculator.Calculate(IngredientsCollection, _currentSizeValue.SizeValue);
        }

        private double _pizzaInfoPrice = 0;
        public double PizzaInfoPrice
        {
            get { return _pizzaInfoPrice; }
            set { _pizzaInfoPrice = value; NotifyPropertyChanged("PizzaInfoPrice"); }
        }

        private void SetCurrentQuantities(bool[] quantities)
        {
            int i = 0;
            foreach (var item in IngredientsCollection)
                item.CurrentQuantity = (quantities[i++]) ? item.Ingredient.NormalWeight : 0;
        }

        public void RefreshRecipes()
        {
            this.IngredientsCollection.Clear();
            this.RecipesCollection.Clear();
            this.OrderedPizzasCollection.Clear();
            this.Ingredients.Clear();

            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    using (var proxy = new WorkChannel())
                    {
                        return proxy.GetRecipeTabData(new PizzaNetCommon.Requests.EmptyRequest
                        {
                            Login = ClientConfig.CurrentUser.Email,
                            Password = ClientConfig.CurrentUser.Password
                        });
                    }
                    //using (var db = new PizzaUnitOfWork())
                    //{
                    //    return db.inTransaction(uof =>
                    //    {
                    //        Console.WriteLine("LoadDataStart");
                    //        var result = new Trio<IEnumerable<Recipe>, PizzaNetDataModel.Model.Size[], IEnumerable<Ingredient>>
                    //        {
                    //            First = uof.Db.Recipies.FindAllEagerly(),
                    //            Second = uof.Db.Sizes.FindAll().ToArray(),
                    //            Third = uof.Db.Ingredients.FindAll()
                    //        };

                    //        Console.WriteLine("after query");

                    //        Console.WriteLine("Result is null: {0}", result == null);

                    //        return result;
                    //    });
                    //}
                }
                catch (Exception exc)
                {
                    return exc;
                }
            }, (s, args) =>
            {
                if (args.Result is Exception)
                {
                    Utils.HandleException(args.Result as Exception);
                    return;
                }
                var result = args.Result as TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>>;
                if (result == null)
                {
                    Utils.showExclamation(Utils.Messages.RECIPES_REFRESH_FAILED);
                    return;
                }
                if (result.Second.Count != 3) throw new Exception(INVALID_SIZES_COUNT);
                foreach (var item in result.First)
                {
                    var rc = new RecipeControl();
                    rc.Recipe = item;
                    rc.RecalculatePrices(result.Second.ToArray());
                    RecipesCollection.Add(rc);
                    Console.WriteLine(item.Name);
                }
                foreach (var item in result.Third)
                {
                    var row = new IngredientsRow(item, 0, result.Second[0]);
                    row.PropertyChanged += row_PropertyChanged;
                    IngredientsCollection.Add(row);
                    Ingredients.Add(item);
                }
                // MODIFIED Tag uses now dependency property
                SmallSize = result.Second[0];
                MedSize = result.Second[1];
                GreatSize = result.Second[2];
                CurrentSizeValue = result.Second[0];
            }));
        }

        private SizeDTO _smallSize;
        public SizeDTO SmallSize
        {
            get { return _smallSize; }
            set
            {
                _smallSize = value;
                NotifyPropertyChanged("SmallSize");
            }
        }
        private SizeDTO _medSize;
        public SizeDTO MedSize
        {
            get { return _medSize; }
            set
            {
                _medSize = value;
                NotifyPropertyChanged("MedSize");
            }
        }
        private SizeDTO _greatSize;
        private const string INVALID_SIZES_COUNT = "Fatal error: Invalid number of sizes";
        public SizeDTO GreatSize
        {
            get { return _greatSize; }
            set
            {
                _greatSize = value;
                NotifyPropertyChanged("GreatSize");
            }
        }

        void row_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Ingredient" || e.PropertyName == "CurrentQuantity")
                RecalculatePrice();
        }
    }
}
