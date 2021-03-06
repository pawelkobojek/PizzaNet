﻿using PizzaNetControls.Common;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PizzaNetCommon.Services;
using PizzaNetCommon.DTOs;

namespace PizzaNetControls
{
    /// <summary>
    /// Interaction logic for RecipeControl.xaml
    /// </summary>
    public partial class RecipeControl : UserControl, INotifyPropertyChanged
    {
        public RecipeControl()
        {
            InitializeComponent();
            this.DataContext = this;
            Prices = new PriceData() { PriceLow = 0, PriceMed = 0, PriceHigh = 0 };
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

        private RecipeDTO _recipe;
        public RecipeDTO Recipe
        {
            get { return _recipe; }
            set { 
                    _recipe = value; 
                    NotifyPropertyChanged("Recipe"); 
                }
        }

        private PriceData _prices;
        public PriceData Prices
        {
            get { return _prices; }
            private set { _prices = value; NotifyPropertyChanged("Prices"); }
        }

        public void RecalculatePrices(SizeDTO[] sizes)
        {
            Prices = PriceCalculator.GetPrices(Recipe, sizes);
        }

        public void Update(SizeDTO[] sizes)
        {
            NotifyPropertyChanged("Recipe");
            RecalculatePrices(sizes);
        }
    }
}
