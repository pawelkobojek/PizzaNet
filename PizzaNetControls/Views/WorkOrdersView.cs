using PizzaNetControls.Controls;
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
using System.Windows.Data;
using PizzaNetCommon.DTOs;
using PizzaNetWorkClient.WCFClientInfrastructure;
using PizzaNetControls.Configuration;
using PizzaNetCommon.Requests;
using PizzaNetControls.Common;

namespace PizzaNetControls.Views
{
    public class WorkOrdersView : BaseView
    {
        public WorkOrdersView(IWorker worker)
            : base(worker)
        {
            this.PizzasCollection = new ObservableCollection<PizzaRow>();
            this.IngredientsCollection = new ObservableCollection<OrderIngredientDTO>();
            this.OrdersCollection = new NotifiedObservableCollection<OrdersRow>();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(OrdersCollection);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.State.StateValue", System.ComponentModel.ListSortDirection.Descending));

            OrdersRefresher = new BackgroundWorker();
            OrdersRefresher.DoWork += OrdersRefresher_DoWork;
            OrdersRefresher.RunWorkerCompleted += OrdersRefresher_RunWorkerCompleted;
            //OrdersRefresher.RunWorkerAsync();
        }

        public ObservableCollection<PizzaRow> PizzasCollection { get; set; }
        public NotifiedObservableCollection<PizzaNetControls.OrdersRow> OrdersCollection { get; set; }
        public ObservableCollection<OrderIngredientDTO> IngredientsCollection { get; set; }
        private const int SECOND = 1000;
        public BackgroundWorker OrdersRefresher { get; private set; }
        private const string ORDER_IMPOSSIBLE = "Action imposible! Not enough ingredient in stock!";

        void OrdersRefresher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (AutoRefreshingEnabled)
            {
                RefreshCurrentOrders();
                if (!OrdersRefresher.IsBusy)
                    OrdersRefresher.RunWorkerAsync();
            }
        }

        void OrdersRefresher_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(ClientConfig.CurrentUser.RefreshRate * SECOND);
        }

        internal void ChangeOrderSelection(int index)
        {
            // MODIFIED
            //OrdersRow ord = ordersListView.SelectedItem as OrdersRow;
            //if (ord != null)
            //{
            //    Console.WriteLine("Order.Address: " + ord.Order.Address);
            //    Console.WriteLine("Order.CustomerPhone: " + ord.Order.CustomerPhone.ToString());
            //    Console.WriteLine("Order.Date: " + ord.Order.Date.ToString());
            //    Console.WriteLine("Order.State: " + ord.Order.State.ToString());
            //}
            IngredientsCollection.Clear();
            PizzasCollection.Clear();
            foreach (var item in OrdersCollection[index].Order.OrderDetailsDTO)
            {
                PizzasCollection.Add(new PizzaRow(item));
            }
            //PizzasCollection.Clear();
            //IList selected = e.AddedItems;
            //if (selected.Count > 0)
            //{
            //    OrdersRow or = selected[0] as OrdersRow;
            //    if (or != null)
            //    {
            //        foreach (var od in or.Order.OrderDetails)
            //        {
            //            PizzasCollection.Add(new PizzaRow(od));
            //        }
            //    }
            //}

            //OrdersRow or = OrdersCollection[index];
            //foreach (var od in or.OrderDTO.OrderDetailsDTO)
            //{
            //    PizzasCollection.Add(new PizzaRow(od));
            //}
        }

        internal void ChangePizzaSelection(int index)
        {
            IngredientsCollection.Clear();
            foreach (var item in PizzasCollection[index].OrderDetail.Ingredients)
            {
                IngredientsCollection.Add(item);
            }
            // MODIFIED
            //IngredientsCollection.Clear();
            //IList selected = e.AddedItems;
            //if (selected.Count > 0)
            //{
            //    PizzaRow pr = selected[0] as PizzaRow;
            //    if (pr != null)
            //    {
            //        foreach (var ingr in pr.OrderDetail.Ingredients)
            //        {
            //            IngredientsCollection.Add(ingr);
            //        }
            //    }
            //}
            //PizzaRow pr = PizzasCollection[index];
            //foreach (var ingr in pr.OrderDetailDTO.Ingredients)
            //{
            //    IngredientsCollection.Add(ingr);
            //}
        }

        internal void SetOrderInRealization(OrdersRow o)
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    using (var proxy = new WorkChannel())
                    {
                        OrderDTO order = o.Order.Clone();
                        order.State.StateValue = State.IN_REALISATION;
                        return proxy.SetOrderState(new UpdateRequest<OrderDTO>
                        {
                            Data = order,
                            Login = ClientConfig.CurrentUser.Email,
                            Password = ClientConfig.CurrentUser.Password
                        });
                    }
                }
                catch (Exception e)
                {
                    return e;
                }
            }, (a, s) =>
            {
                if (s.Result is Exception)
                {
                    Utils.HandleException(s.Result as Exception);
                    return;
                }
                var res = s.Result as SingleItemResponse<OrderDTO>;
                if (res == null)
                {
                    Utils.showError(Utils.Messages.UNKNOWN_ERROR_FORMAT);
                    return;
                }
                o.Order.State = res.Data.State;
                o.Update();
            }));
        }

        //private void SetOrderStateInBackground(OrdersRow or, State state)
        //{
        //    Worker.EnqueueTask(new WorkerTask((args) =>
        //    {
        //        using (var db = new PizzaUnitOfWork())
        //        {
        //            return db.inTransaction(uof =>
        //            {
        //                Console.WriteLine("Set in realisation");

        //                Order o = uof.Db.Orders.Get(or.Order.OrderID);
        //                o.State.StateValue = state.StateValue;
        //                uof.Db.Commit();

        //                Console.WriteLine("Order " + or.Order.OrderID + " state set to IN REALISATION");
        //                return or;
        //            });
        //        }
        //    },
        //    (s, a) =>
        //    {
        //        RefreshCurrentOrders();
        //        //or.Order.State = state;
        //        or.Update();
        //        // TODO zwalone sortowanie po zmianie stanu, nie uaktualnia się
        //    }));
        //}

        public void RefreshCurrentOrders()
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    using (var proxy = new WorkChannel())
                    {
                        return proxy.GetUndoneOrders(new PizzaNetCommon.Requests.EmptyRequest
                        {
                            Login = ClientConfig.CurrentUser.Email,
                            Password = ClientConfig.CurrentUser.Password
                        });
                    }
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
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
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
                    OrdersRow row = OrdersCollection.FirstOrDefault(r => { return r.Order.OrderID == order.OrderID; });
                    if (row != null) row.Order = order;
                    else OrdersCollection.Add(new OrdersRow(order));
                }
            }));
        }

        internal void SetOrderDone(OrdersRow o)
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                var order = o.Order.Clone();
                try
                {
                    order.State.StateValue = State.DONE;
                    using (var proxy = new WorkChannel())
                    {
                        return proxy.SetOrderState(new UpdateRequest<OrderDTO>
                        {
                            Data = order,
                            Login = ClientConfig.CurrentUser.Email,
                            Password = ClientConfig.CurrentUser.Password
                        });
                    }
                }
                catch (Exception e)
                {
                    return e;
                }

            }, (a, s) =>
            {
                if (s.Result is Exception)
                {
                    Utils.HandleException(s.Result as Exception);
                    return;
                }
                var res = s.Result as SingleItemResponse<OrderDTO>;
                if (res == null)
                {
                    Utils.showError(Utils.Messages.UNKNOWN_ERROR_FORMAT);
                    return;
                }
                o.Order.State = res.Data.State;
                o.Update();
            }));
        }

        internal void RemoveOrder(int orderIndex)
        {
            OrdersRow or = OrdersCollection[orderIndex];
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    using (var proxy = new WorkChannel())
                    {
                        proxy.RemoveOrder(new UpdateOrRemoveRequest<OrderDTO>
                        {
                            Data = null,
                            DataToRemove = or.Order,
                            Login = ClientConfig.CurrentUser.Email,
                            Password = ClientConfig.CurrentUser.Password
                        });
                    }
                    return true;
                }
                catch(Exception e)
                {
                    return e;
                }
                //using (var db = new PizzaUnitOfWork())
                //{
                //    return db.inTransaction(uof =>
                //    {

                //        Console.WriteLine("Remove order");

                //        Order o = db.Orders.Get(or.Order.OrderID);
                //        db.Orders.Delete(o);
                //        db.Commit();
                //        Console.WriteLine("Order " + or.Order.OrderID + " removed.");
                //        return or;
                //    });
                //}
            },
            (s, a) =>
            {
                if (a.Result is Exception)
                {
                    Utils.HandleException(a.Result as Exception);
                    return;
                }
                if ((a.Result as bool?)??false)
                {
                    Utils.showError(Utils.Messages.UNKNOWN_ERROR_FORMAT);
                    return;
                }
                RefreshCurrentOrders();
            }));
        }

        //public void RefreshOrders()
        //{
        //    OrdersCollection.Clear();
        //    Worker.EnqueueTask(new WorkerTask((args) =>
        //    {
        //        try
        //        {
        //            using (var db = new PizzaUnitOfWork())
        //            {
        //                return db.inTransaction(uof =>
        //                {
        //                    Console.WriteLine("Load Orders Start");
        //                    var result = uof.Db.Orders.FindAllEagerlyWhere((o) => o.State.StateValue == State.IN_REALISATION || o.State.StateValue == State.NEW);
        //                    Console.WriteLine("After query");
        //                    return result;
        //                });
        //            }
        //        }
        //        catch (Exception exc)
        //        {
        //            Console.WriteLine(exc);
        //            return null;
        //        }
        //    }, (s, a) =>
        //    {
        //        IEnumerable<Order> orders = a.Result as IEnumerable<Order>;
        //        if (orders == null)
        //        {
        //            MessageBox.Show(REFRESH_FAILED);
        //            return;
        //        }
        //        //foreach (var order in orders)
        //        //{
        //        //    OrdersCollection.Add(new OrdersRow(order));
        //        //}
        //    }));
        //}

        public void Load()
        {
            RefreshCurrentOrders();
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
    }
}
