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

namespace PizzaNetControls.Views
{
    public class WorkOrdersView : BaseView
    {
        public WorkOrdersView(IWorker worker) : base(worker)
        {
            this.PizzasCollection = new ObservableCollection<PizzaRow>();
            this.IngredientsCollection = new ObservableCollection<OrderIngredientDTO>();
            this.OrdersCollection = new ObservableCollection<OrdersRow>();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(OrdersCollection);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.State.StateValue", System.ComponentModel.ListSortDirection.Descending));

            OrdersRefresher = new BackgroundWorker();
            OrdersRefresher.DoWork += OrdersRefresher_DoWork;
            OrdersRefresher.RunWorkerCompleted += OrdersRefresher_RunWorkerCompleted;
            OrdersRefresher.RunWorkerAsync();
        }

        public ObservableCollection<PizzaRow> PizzasCollection { get; set; }
        public ObservableCollection<PizzaNetControls.OrdersRow> OrdersCollection { get; set; }
        public ObservableCollection<OrderIngredientDTO> IngredientsCollection { get; set; }
        private const int TIMER_INTERVAL = 60000;
        public BackgroundWorker OrdersRefresher {get; private set;}
        private const string ORDER_IMPOSSIBLE = "Action imposible! Not enough ingredient in stock!";
        private const string REFRESH_FAILED = "Refreshing orders failed!";

        void OrdersRefresher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshCurrentOrders();
            OrdersRefresher.RunWorkerAsync();
        }

        void OrdersRefresher_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(TIMER_INTERVAL);
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

            OrdersRow or = OrdersCollection[index];
            foreach (var od in or.OrderDTO.OrderDetailsDTO)
            {
                PizzasCollection.Add(new PizzaRow(od));
            }
        }

        internal void ChangePizzaSelection(int index)
        {
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
            PizzaRow pr = PizzasCollection[index];
            foreach (var ingr in pr.OrderDetailDTO.Ingredients)
            {
                IngredientsCollection.Add(ingr);
            }
        }

        internal void SetOrderInRealization(OrdersRow o)
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                using (var db = new PizzaUnitOfWork())
                {

                    return db.inTransaction(uof =>
                    {
                        try
                        {
                            List<OrderIngredient> orderIngredients = new List<OrderIngredient>();
                            foreach (var od in o.Order.OrderDetails)
                            {
                                foreach (var odIng in od.Ingredients)
                                {
                                    orderIngredients.Add(odIng);
                                }
                            }
                            foreach (var odIng in orderIngredients)
                            {
                                Ingredient i = uof.Db.Ingredients.Get(odIng.Ingredient.IngredientID);

                                if (i.StockQuantity - odIng.Quantity < 0)
                                {
                                    MessageBox.Show(ORDER_IMPOSSIBLE);
                                    return false;
                                }

                                i.StockQuantity -= odIng.Quantity;
                            }
                            uof.Db.Commit();
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc);
                            return false;
                        }
                        return true;
                    });
                }
            }, (a, s) =>
            {
                if ((bool)s.Result)
                    SetOrderStateInBackground(o, new State { StateValue = State.IN_REALISATION });
            }));
        }

        private void SetOrderStateInBackground(OrdersRow or, State state)
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                using (var db = new PizzaUnitOfWork())
                {
                    return db.inTransaction(uof =>
                    {
                        Console.WriteLine("Set in realisation");

                        Order o = uof.Db.Orders.Get(or.Order.OrderID);
                        o.State.StateValue = state.StateValue;
                        uof.Db.Commit();

                        Console.WriteLine("Order " + or.Order.OrderID + " state set to IN REALISATION");
                        return or;
                    });
                }
            },
            (s, a) =>
            {
                RefreshCurrentOrders();
                or.Order.State = state;
                or.Update();
                // TODO zwalone sortowanie po zmianie stanu, nie uaktualnia się
            }));
        }

        public void RefreshCurrentOrders()
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    using (var db = new PizzaUnitOfWork())
                    {
                        return db.inTransaction(uof =>
                        {
                            Console.WriteLine("Load Orders Start");
                            var result = uof.Db.Orders.FindAllEagerlyWhere((o) => o.State.StateValue == State.IN_REALISATION || o.State.StateValue == State.NEW);
                            Console.WriteLine("After query");
                            return result;
                        });
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                    return null;
                }
            }, (s, a) =>
            {
                IEnumerable<Order> orders = a.Result as IEnumerable<Order>;
                if (orders == null)
                {
                    MessageBox.Show(REFRESH_FAILED);
                    return;
                }
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
            SetOrderStateInBackground(o, new State { StateValue = State.DONE });
        }

        internal void RemoveOrder(int orderIndex)
        {
            OrdersRow or = OrdersCollection[orderIndex];
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                using (var db = new PizzaUnitOfWork())
                {
                    return db.inTransaction(uof =>
                    {

                        Console.WriteLine("Remove order");

                        Order o = db.Orders.Get(or.Order.OrderID);
                        db.Orders.Delete(o);
                        db.Commit();
                        Console.WriteLine("Order " + or.Order.OrderID + " removed.");
                        return or;
                    });
                }
            },
                (s, a) =>
                {
                    RefreshOrders();
                }));
        }

        public void RefreshOrders()
        {
            OrdersCollection.Clear();
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                try
                {
                    using (var db = new PizzaUnitOfWork())
                    {
                        return db.inTransaction(uof =>
                        {
                            Console.WriteLine("Load Orders Start");
                            var result = uof.Db.Orders.FindAllEagerlyWhere((o) => o.State.StateValue == State.IN_REALISATION || o.State.StateValue == State.NEW);
                            Console.WriteLine("After query");
                            return result;
                        });
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                    return null;
                }
            }, (s, a) =>
            {
                IEnumerable<Order> orders = a.Result as IEnumerable<Order>;
                if (orders == null)
                {
                    MessageBox.Show(REFRESH_FAILED);
                    return;
                }
                foreach (var order in orders)
                {
                    OrdersCollection.Add(new OrdersRow(order));
                }
            }));
        }

        public void Load()
        {
            RefreshOrders();
        }
    }
}
