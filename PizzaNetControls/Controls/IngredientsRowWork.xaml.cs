﻿using PizzaNetDataModel.Model;
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
using PizzaNetCommon.DTOs;

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [ContentProperty("Children")]
    public partial class IngredientsRowWork : UserControl, INotifyPropertyChanged
    {
        public IngredientsRowWork()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public IngredientsRowWork(OrderIngredientDTO ingredient)
            : this()
        {
            Ingredient = ingredient;
            Included = false;
        }

        public IngredientsRowWork(OrderIngredientDTO ingredient, bool included)
            : this(ingredient)
        {
            Included = included;
        }

        public IngredientsRowWork(StockIngredientDTO ingredient, bool included)
            : this(ingredient)
        {
            Included = included;
        }

        public IngredientsRowWork(StockIngredientDTO ingredient)
        {
            Ingredient = new OrderIngredientDTO
            {
                Quantity = ingredient.NormalWeight,
                Price = ingredient.PricePerUnit,
                NormalWeight = ingredient.NormalWeight,
                Name = ingredient.Name,
                IngredientID = ingredient.IngredientID,
                ExtraWeight = ingredient.ExtraWeight
            };
            Included = false;
        }

        private OrderIngredientDTO _ingredient;
        public OrderIngredientDTO Ingredient
        {
            get { return _ingredient; }
            set
            {
                _ingredient = value;
                NotifyPropertyChanged("Ingredient");
            }
        }

        private bool _included;
        public bool Included
        {
            get { return _included; }
            set { _included = value; NotifyPropertyChanged("Included"); }
        }

        public event EventHandler ButtonIncludeChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void Button_LT_Click(object sender, RoutedEventArgs e)
        {
            Included = false;
            if (ButtonIncludeChanged != null)
                ButtonIncludeChanged(this, new EventArgs());
        }

        private void Button_GT_Click(object sender, RoutedEventArgs e)
        {
            Included = true;
            if (ButtonIncludeChanged != null)
                ButtonIncludeChanged(this, new EventArgs());
        }
    }
}
