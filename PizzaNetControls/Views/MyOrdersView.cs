﻿using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using PizzaNetCommon.DTOs;
using PizzaNetControls.Controls;
using PizzaNetDataModel.Model;
using System.Windows.Data;
using PizzaNetDataModel.Repository;
using System.Windows;
using PizzaNetWorkClient.WCFClientInfrastructure;
using PizzaNetControls.Configuration;
using PizzaNetCommon.Requests;
using PizzaNetControls.Common;

namespace PizzaNetControls.Views
{
    public class MyOrdersView : BaseView
    {
        public NotifiedObservableCollection<OrdersRow> OrdersCollection { get; set; }
        public ObservableCollection<OrderIngredientDTO> IngredientsCollection { get; set; }
        public ObservableCollection<PizzaRow> PizzasCollection { get; set; }
        public BackgroundWorker OrdersRefresher { get; private set; }

        private const int SECOND = 1000;
        
        public MyOrdersView(IWorker worker)
            : base(worker)
        {
            this.OrdersCollection = new NotifiedObservableCollection<OrdersRow>();
            this.IngredientsCollection = new ObservableCollection<OrderIngredientDTO>();
            this.PizzasCollection = new ObservableCollection<PizzaRow>();
            OrdersRefresher = new BackgroundWorker();
            OrdersRefresher.DoWork+=OrdersRefresher_DoWork;
            OrdersRefresher.RunWorkerCompleted += OrdersRefresher_RunWorkerCompleted;
        }

        private void OrdersRefresher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (AutoRefreshingEnabled)
            {
                RefreshCurrentOrders();
                if (!OrdersRefresher.IsBusy)
                    OrdersRefresher.RunWorkerAsync();
            }
        }

        private bool AutoRefreshingEnabled = false;

        public void SetAutoRefresh(bool enable)
        {
            if (enable && !AutoRefreshingEnabled)
            {
                if (!OrdersRefresher.IsBusy)
                    this.OrdersRefresher.RunWorkerAsync();
            }
            AutoRefreshingEnabled = enable;
        }

        private void OrdersRefresher_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(ClientConfig.CurrentUser.RefreshRate*SECOND);
        }

        public void RefreshCurrentOrders()
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    //using (var db = new PizzaUnitOfWork())
                    //{
                    //    return db.inTransaction(uof =>
                    //    {
                    //        Console.WriteLine("Load Orders Start");
                    //        var result = uof.Db.Orders.FindAllEagerlyWhere((o) => o.State.StateValue == State.IN_REALISATION || o.State.StateValue == State.NEW);
                    //        Console.WriteLine("After query");
                    //        return result;
                    //    });
                    //}
                    using (var proxy = new WorkChannel())
                    {
                        return proxy.GetOrdersForUser(new EmptyRequest { Login = ClientConfig.CurrentUser.Email, Password = ClientConfig.CurrentUser.Password });
                    }

                }
                catch (Exception exc)
                {
                    return exc;
                }
            }, (s, a) =>
            {
                if (a.Result is Exception)
                {
                    Utils.HandleException(a.Result as Exception);
                    return;
                }
                ListResponse<OrderDTO> res = a.Result as ListResponse<OrderDTO>;
                if (res == null)
                {
                    Utils.showExclamation(Utils.Messages.ORDERS_REFRESH_FAILED);
                    return;
                }
                List<OrderDTO> orders = res.Data;

                bool[] current = new bool[orders.Count()];
                foreach (var order in orders)
                {
                    Console.WriteLine("Order#" + order.OrderID);
                    Console.WriteLine("Order state value: " + order.State.StateValue);
                    OrdersRow row = OrdersCollection.FirstOrDefault(r => { return r.Order.OrderID == order.OrderID; });
                    if (row != null) row.Order = order;
                    else OrdersCollection.Add(new OrdersRow(order));
                }
                OrdersCollection.NotifyCollectionChanged();
            }));
        }


        internal void OrderSelectionChanged(int p)
        {
            PizzasCollection.Clear();

            foreach (var item in OrdersCollection[p].Order.OrderDetailsDTO)
            {
                PizzasCollection.Add(new PizzaRow(item));
            }
        }

        internal void PizzaSelectionChanged(int p)
        {
            IngredientsCollection.Clear();

            foreach (var item in PizzasCollection[p].OrderDetail.Ingredients)
            {
                IngredientsCollection.Add(item);
            }
        }
    }
}
