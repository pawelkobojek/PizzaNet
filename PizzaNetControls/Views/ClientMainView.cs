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

namespace PizzaNetControls.Views
{
    public class ClientMainView : BaseView
    {
        public ClientMainView(IWorker worker) : base(worker)
        {
            this.IngredientsCollection = new ObservableCollection<PizzaNetControls.IngredientsRow>();
            this.RecipesCollection = new ObservableCollection<PizzaNetControls.RecipeControl>();
            this.OrderedPizzasCollection = new ObservableCollection<IngredientsList>();
            this.Ingredients = new List<Ingredient>();
        }

        internal void AddOrder()
        {
            var ingr = new List<OrderIngredient>();
            foreach (var item in IngredientsCollection)
            {
                if (item.CurrentQuantity > 0)
                    ingr.Add(new OrderIngredient() { Ingredient = item.Ingredient, Quantity = item.CurrentQuantity });
            }
            var orderDetail = new OrderDetail()
            {
                Ingredients = ingr,
                Size = CurrentSizeValue
            };
            OrderedPizzasCollection.Add(new IngredientsList(orderDetail));
        }

        public ObservableCollection<PizzaNetControls.RecipeControl> RecipesCollection { get; set; }
        public ObservableCollection<PizzaNetControls.IngredientsList> OrderedPizzasCollection { get; set; }
        public ObservableCollection<PizzaNetControls.IngredientsRow> IngredientsCollection { get; set; }

        private PizzaNetDataModel.Model.Size _currentSizeValue;
        public PizzaNetDataModel.Model.Size CurrentSizeValue
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
            List<OrderDetail> details = new List<OrderDetail>(OrderedPizzasCollection.Count);
            foreach (var item in OrderedPizzasCollection)
                details.Add(item.OrderDetail);

            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                var cfg = args[0] as ClientConfig;
                var det = args[1] as List<OrderDetail>;
                if (cfg == null || det == null) return false;
                try
                {
                    using (var ctx = new PizzaUnitOfWork())
                    {
                        ctx.inTransaction(uof =>
                        {
                            det = mergeIngredients(det, uof.Db.Ingredients.FindAll());
                            det = mergeSizes(det, uof.Db.Sizes.FindAll());

                            foreach (var d in det)
                                foreach (var ingr in d.Ingredients)
                                    ingr.Quantity = (int)(ingr.Quantity * d.Size.SizeValue);

                            uof.Db.Orders.Insert(new Order()
                            {
                                Address = cfg.UserAddress,
                                CustomerPhone = cfg.Phone,
                                Date = DateTime.Now,
                                OrderDetails = det,
                                State = new State() { StateValue = State.NEW }
                            });
                            uof.Db.Commit();
                        });
                    }
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }, (s, args) =>
            {
                bool b = (args.Result as bool?) ?? false;
                MessageBox.Show((b) ? "Ordered successfully" : "Error while ordering", "PizzaNet");
                if (b) OrderedPizzasCollection.Clear();
            }, ClientConfig.getConfig(), details));
        }

        /// <summary>
        /// Method needed to merge Ingredients and avoid to duplicate them in Ingredients table
        /// </summary>
        /// <param name="det"></param>
        /// <param name="ing"></param>
        /// <returns></returns>
        private List<OrderDetail> mergeIngredients(List<OrderDetail> det, IEnumerable<Ingredient> ing)
        {
            foreach (var d in det)
                foreach (var i in d.Ingredients)
                {
                    Ingredient s = ing.FirstOrDefault((e) => { return e.IngredientID == i.Ingredient.IngredientID; });
                    if (s == null) throw new Exception("Inconsistien data");
                    i.Ingredient = s;
                }
            return det;
        }
        private List<OrderDetail> mergeSizes(List<OrderDetail> det, IEnumerable<PizzaNetDataModel.Model.Size> sizes)
        {
            foreach (var d in det)
            {
                d.Size = sizes.FirstOrDefault((e) => { return e.SizeValue == d.Size.SizeValue; });
                if (d.Size == null) throw new Exception("Inconsistien data");
            }
            return det;
        }

        internal void ChangeCurrentSize(PizzaNetDataModel.Model.Size size)
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

        public List<Ingredient> Ingredients { get; set; }

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
                if (ind > 0)
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

        public void Load()
        {
            this.IngredientsCollection.Clear();
            this.RecipesCollection.Clear();
            this.OrderedPizzasCollection.Clear();
            this.Ingredients.Clear();

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
                                First = uof.Db.Recipies.FindAllEagerly(),
                                Second = uof.Db.Sizes.FindAll().ToArray(),
                                Third = uof.Db.Ingredients.FindAll()
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
                if (result.Second.Length != 3) throw new Exception("Invalid number of sizes");
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

        private PizzaNetDataModel.Model.Size _smallSize;
        public PizzaNetDataModel.Model.Size SmallSize
        {
            get { return _smallSize; }
            set
            {
                _smallSize = value;
                NotifyPropertyChanged("SmallSize");
            }
        }
        private PizzaNetDataModel.Model.Size _medSize;
        public PizzaNetDataModel.Model.Size MedSize
        {
            get { return _medSize; }
            set
            {
                _medSize = value;
                NotifyPropertyChanged("MedSize");
            }
        }
        private PizzaNetDataModel.Model.Size _greatSize;
        public PizzaNetDataModel.Model.Size GreatSize
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
