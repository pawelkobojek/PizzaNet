using PizzaNetControls.Workers;
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

namespace PizzaNetControls.Views
{
    public class MyOrdersView : BaseView
    {
        public ObservableCollection<OrdersRow> OrdersCollection { get; set; }
        public ObservableCollection<OrderIngredient> IngredientsCollection { get; set; }
        public ObservableCollection<PizzaRow> PizzasCollection { get; set; }
        public BackgroundWorker OrdersRefresher { get; private set; }

        private const int TIMER_INTERVAL = 60000;
        private const string REFRESH_FAILED = "Refreshing orders failed!";

        public MyOrdersView(IWorker worker)
            : base(worker)
        {
            this.OrdersCollection = new ObservableCollection<OrdersRow>();
            this.IngredientsCollection = new ObservableCollection<OrderIngredient>();
            this.PizzasCollection = new ObservableCollection<PizzaRow>();

            //OrdersRefresher = new BackgroundWorker();
            //OrdersRefresher.DoWork += OrdersRefresher_DoWork;
            //OrdersRefresher.RunWorkerCompleted += OrdersRefresher_RunWorkerCompleted;
            //OrdersRefresher.RunWorkerAsync();
        }

        private void OrdersRefresher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshCurrentOrders();
            OrdersRefresher.RunWorkerAsync();
        }

        private void OrdersRefresher_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(TIMER_INTERVAL);
        }

        private WorkChannel proxy = new WorkChannel(ClientConfig.getConfig().ServerAddress);
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
                    var cfg = ClientConfig.getConfig();
                    return proxy.GetOrdersForUser(new EmptyRequest { Login = cfg.User.Email, Password = cfg.User.Password });

                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                    return null;
                }
            }, (s, a) =>
            {
                IList<OrderDTO> orders = ((ListResponse<OrderDTO>)a.Result).Data;
                if (orders == null)
                {
                    MessageBox.Show(REFRESH_FAILED);
                    return;
                }
                foreach (var item in orders)
                {
                    OrdersCollection.Add(new OrdersRow(item));
                }

                //TODO odkomentować poniższe, zakomentować powyższe
                //bool[] current = new bool[orders.Count()];
                //foreach (var order in orders)
                //{
                //    Console.WriteLine("Order#" + order.OrderID);
                //    Console.WriteLine("Order state value: " + order.State.StateValue);
                //    OrdersRow row = OrdersCollection.FirstOrDefault(r => { return r.Order.OrderID == order.OrderID; });
                //    if (row != null) row.OrderDTO = order;
                //    else OrdersCollection.Add(new OrdersRow(order));
                //}
            }));
        }

    }
}
