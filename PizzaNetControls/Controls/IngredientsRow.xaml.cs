using PizzaNetControls.Common;
using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PizzaNetControls.Controls;
using PizzaNetCommon.DTOs;

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [ContentProperty("Children")]
    public partial class IngredientsRow : UserControl, INotifyPropertyChanged
    {
        public IngredientsRow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.PropertyChanged += IngredientsRow_PropertyChanged;
        }

        void IngredientsRow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Ingredient" || e.PropertyName == "CurrentQuantity")
                updateButtons();
        }

        public IngredientsRow(OrderIngredientDTO ingredient)
            : this()
        {
            Ingredient = ingredient;
            CurrentQuantity = 0;
        }

        public IngredientsRow(OrderIngredientDTO ingredient, int currentQuantity)
            : this(ingredient)
        {
            CurrentQuantity = currentQuantity;
        }

        public IngredientsRow(OrderIngredientDTO ingredient, int currentQuantity, SizeDTO size)
            : this(ingredient, currentQuantity)
        {
            this.CurrentSize = size;
        }

        private OrderIngredientDTO _ingredient;
        public OrderIngredientDTO Ingredient
        {
            get { return _ingredient; }
            set
            {
                _ingredient = value;
                BackgroundParameter = new Pair<int, int>() { First = _currentQuantity, Second = Ingredient.ExtraWeight };
                NotifyPropertyChanged("Ingredient");
            }
        }

        private int _currentQuantity;
        public int CurrentQuantity
        {
            get { return _currentQuantity; }
            set
            {
                _currentQuantity = value;
                BackgroundParameter = new Pair<int, int>() { First = _currentQuantity, Second = Ingredient.ExtraWeight };
                NotifyPropertyChanged("CurrentQuantity");
                NotifyPropertyChanged("CurrentWeight");
            }
        }

        public int CurrentWeight
        {
            get
            {
                return (int)(CurrentQuantity * ((CurrentSize != null) ? CurrentSize.SizeValue : 0));
            }
        }

        private SizeDTO _currentSize;
        public SizeDTO CurrentSize
        {
            get { return _currentSize; }
            set
            {
                _currentSize = value;
                BackgroundParameter = new Pair<int, int>() { First = _currentQuantity, Second = Ingredient.ExtraWeight };
                NotifyPropertyChanged("CurrentSize");
                NotifyPropertyChanged("CurrentWeight");
            }
        }

        private Pair<int, int> _backgroundParameter = new Pair<int, int>();
        public Pair<int, int> BackgroundParameter
        {
            get { return _backgroundParameter; }
            set { _backgroundParameter = value; NotifyPropertyChanged("BackgroundParameter"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;/*
        private Ingredient item;
        private int p;
        private PizzaNetDataModel.Model.Size size;*/
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void updateButtons()
        {
            buttonLT.IsEnabled = (CurrentQuantity != 0);
            buttonGT.IsEnabled = (CurrentQuantity != Ingredient.ExtraWeight);
        }

        private void decQuantity()
        {
            if (CurrentQuantity == Ingredient.NormalWeight) CurrentQuantity = 0;
            else if (CurrentQuantity == Ingredient.ExtraWeight) CurrentQuantity = Ingredient.NormalWeight;
            else if (CurrentQuantity == 0) return;
            else throw new NotSupportedException(String.Format("Not supported decrase of value {0}", CurrentQuantity));
        }

        private void incQuantity()
        {
            if (CurrentQuantity == Ingredient.NormalWeight) CurrentQuantity = Ingredient.ExtraWeight;
            else if (CurrentQuantity == Ingredient.ExtraWeight) return;
            else if (CurrentQuantity == 0) CurrentQuantity = Ingredient.NormalWeight;
            else throw new NotSupportedException(String.Format("Not supported decrase of value {0}", CurrentQuantity));
        }

        private void Button_LT_Click(object sender, RoutedEventArgs e)
        {
            decQuantity();
            updateButtons();
        }

        private void Button_GT_Click(object sender, RoutedEventArgs e)
        {
            incQuantity();
            updateButtons();
        }
    }
}
