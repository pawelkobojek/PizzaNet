using PizzaNetControls;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PizzaNetClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PizzaNetClientWindow : Window, INotifyPropertyChanged
    {
        public PizzaNetClientWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.IngredientsCollection = new ObservableCollection<PizzaNetControls.IngredientsRow>();
            this.RecipesCollection = new ObservableCollection<PizzaNetControls.RecipeControl>();

            #region ExampleData
           /* var c = new PizzaNetControls.IngredientsRow(new Ingredient()
            {
                Name = "Ingredient1",
                NormalWeight = 200,
                ExtraWeight = 300
            });
            this.IngredientsCollection.Add(c);
            for (int i = 0; i < 10; i++)
            {
                c = new PizzaNetControls.IngredientsRow(new Ingredient()
                {
                    Name = "Mozzarella Cheese",
                    NormalWeight = 100,
                    ExtraWeight = 200
                });
                c.CurrentQuantity = c.Ingredient.NormalWeight;
                this.IngredientsCollection.Add(c);
            }*/
            
           /* PizzaNetControls.RecipeControl d;
            for (int i = 0; i < 10; i++)
            {
                d = new PizzaNetControls.RecipeControl();
                d.Recipe = new Recipe()
                {
                    Ingredients = new List<Ingredient>() { new Ingredient() { Name="Mozarella Cheese", NormalWeight=100, PricePerUnit=0.2M},
                                                                new Ingredient() { Name="Mushrooms", NormalWeight=50, PricePerUnit=0.2M},
                                                                new Ingredient() { Name="Ingredient3", NormalWeight=200, PricePerUnit=0.2M}},
                    Name = "MyRecipe"
                };
                d.RecalculatePrices(new PizzaNetDataModel.Model.Size[] { new PizzaNetDataModel.Model.Size() { SizeValue=1},
                                                        new PizzaNetDataModel.Model.Size() { SizeValue=1.5},
                                                        new PizzaNetDataModel.Model.Size() { SizeValue=2}});
                this.RecipesCollection.Add(d);
            }*/
            #endregion
        }

        public ObservableCollection<PizzaNetControls.IngredientsRow> IngredientsCollection { get; set; }
        public ObservableCollection<PizzaNetControls.RecipeControl> RecipesCollection { get; set; }

        private string _sizeSelectedText = "Small";
        public string SizeSelectedText
        {
            get { return _sizeSelectedText; }
            set { _sizeSelectedText = value; NotifyPropertyChanged("SizeSelectedText"); }
        }

        private double _pizzaInfoPrice = 0;
        public double PizzaInfoPrice
        {
            get { return _pizzaInfoPrice; }
            set { _pizzaInfoPrice = value; NotifyPropertyChanged("PizzaInfoPrice"); }
        }

        private void PizzaNetWindowClass_Loaded(object sender, RoutedEventArgs e)
        {
            var worker = new PizzaNetControls.Worker.WorkerWindow(this, (args) =>
            {
                try
                {
                    using (var db = new PizzaUnitOfWork())
                    {
                        Console.WriteLine("LoadDataStart");
                        var result = new Pair<IEnumerable<Recipe>, PizzaNetDataModel.Model.Size[]>
                        {
                            First = db.Recipies.FindAllEagerly(),
                            Second = db.Sizes.FindAll().ToArray()
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
            }, (s,args) =>
                {
                    var result = args.Result as Pair<IEnumerable<Recipe>, PizzaNetDataModel.Model.Size[]>;
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
                }, null);
            worker.ShowDialog();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null) return;
            SizeSelectedText = rb.Content.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RecipesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource!=RecipesContainer) return;
            if (RecipesContainer.SelectedIndex < 0) return;
            IngredientsCollection.Clear();
            foreach (var i in RecipesCollection[RecipesContainer.SelectedIndex].Recipe.Ingredients)
            {
                var row = new IngredientsRow(i);
                row.CurrentQuantity = i.NormalWeight;
                IngredientsCollection.Add(row);
            }    
        }
    }
}
