﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PizzaNetDataModel;
using PizzaNetDataModel.Model;

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
            this.StockItemsCollection = new ObservableCollection<PizzaNetControls.StockItem>();
            this.OrdersCollection = new ObservableCollection<PizzaNetControls.OrdersRow>();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(OrdersCollection);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.State.StateValue",System.ComponentModel.ListSortDirection.Descending));

            #region example data
            PizzaNetControls.StockItem st;
            for (int i = 0; i < 20; i++)
            {
                st = new PizzaNetControls.StockItem();
                st.StockItemName = "ItemName";
                st.StockQuantity = 100;
                st.NormalWeight = 10;
                st.ExtraWeight = 20;
                st.PricePerUnit = 1.2M;
                StockItemsCollection.Add(st);
            }

            PizzaNetControls.OrdersRow o;
            for (int i = 0; i < 10; i++)
            {
                o = new PizzaNetControls.OrdersRow(new Order() { OrderID = 12*i, State = new State() {StateValue = i%3}});
                OrdersCollection.Add(o);
            }
            #endregion
        }

        public ObservableCollection<PizzaNetControls.StockItem> StockItemsCollection { get; set; }

        public ObservableCollection<PizzaNetControls.OrdersRow> OrdersCollection { get; set; }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*StockItemsCollection = new ObservableCollection<PizzaNetControls.StockItem>();
            using (PizzaContext db = new PizzaContext())
            {
                var query = from p in db.Ingredients
                            select p;

                foreach (var item in query)
                {
                    StockItemsCollection.Add(new PizzaNetControls.StockItem(item));
                }
            };*/
        }
    }
}
