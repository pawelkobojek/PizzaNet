using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.DTOs;
using System.Collections.ObjectModel;
using PizzaNetWorkClient.WCFClientInfrastructure;
using PizzaNetControls.Configuration;
using PizzaNetCommon.Requests;
using PizzaNetControls.Common;
using PizzaNetControls.Dialogs;

namespace PizzaNetControls.Views
{
    public class UsersView : BaseView
    {
        public ObservableCollection<UserDTO> UsersCollection { get; set; }
        public ObservableCollection<string> Rights { get; set; }
        public ObservableCollection<UserDTO> RemovedUsers { get; set; }
        public bool Modified { get; set; }

        public UsersView(IWorker worker)
            : base(worker)
        {
            UsersCollection = new ObservableCollection<UserDTO>();
            RemovedUsers = new ObservableCollection<UserDTO>();
            Rights = new ObservableCollection<string>()
                {
                    "Customer",
                    "Employee",
                    "Admin"
                };
            Modified = false;
        }

        internal void RefreshUsers()
        {
            Worker.EnqueueTask(new WorkerTask(args =>
                {
                    try
                    {
                        using (var proxy = new WorkChannel())
                        {
                            return proxy.GetUsers(new EmptyRequest
                            {
                                Login = ClientConfig.CurrentUser.Email,
                                Password = ClientConfig.CurrentUser.Password
                            });
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                        return exc;
                    }
                }, (s, e) =>
                    {
                        if( e.Result is Exception)
                        {
                            Utils.HandleException(e.Result as Exception);
                            return;
                        }
                        var result = e.Result as ListResponse<UserDTO>;
                        if (result == null)
                        {
                            Utils.showError(Utils.Messages.UNKNOWN_ERROR);
                            return;
                        }

                        UsersCollection.Clear();
                        foreach (var user in result.Data)
                        {
                            UsersCollection.Add(user);
                        }
                        Modified = false;
                    }, null));
        }

        private int counter = 1;
        internal void AddUser()
        {
            var pass = InputPassword.Show("Insert password for new user:", "Insert password");
            if (string.IsNullOrEmpty(pass)) return;
            UsersCollection.Add(new UserDTO { Address = "Address", Email = "Email " + counter.ToString(), Name = "Name", UserID = -counter, Phone = 0, Rights = 1, Password=pass });
            counter++;
            Modified = true;
        }

        internal void RemoveUser(int index)
        {
            RemovedUsers.Add(UsersCollection[index]);
            UsersCollection.RemoveAt(index);
            Modified = true;
        }

        internal void SaveChanges()
        {
            Worker.EnqueueTask(new WorkerTask(args =>
                {
                    try
                    {
                        using (var proxy = new WorkChannel())
                        {
                            List<UserDTO> toUpdate = new List<UserDTO>();
                            List<UserDTO> toRemove = new List<UserDTO>();
                            foreach (var user in UsersCollection)
                            {
                                toUpdate.Add(user);
                            }

                            foreach (var user in RemovedUsers)
                            {
                                toRemove.Add(user);
                            }

                            return proxy.UpdateOrRemoveUser(new UpdateOrRemoveRequest<List<UserDTO>>
                            {
                                Data = toUpdate,
                                DataToRemove = toRemove,
                                Login = ClientConfig.CurrentUser.Email,
                                Password = ClientConfig.CurrentUser.Password
                            });
                        }
                    }
                    catch (Exception exc)
                    {
                        return exc;
                    }
                }, (s, e) =>
                    {
                        if (e.Result is Exception)
                        {
                            Utils.HandleException(e.Result as Exception);
                            return;
                        }
                        var result = e.Result as ListResponse<UserDTO>;
                        if (result == null)
                        {
                            Utils.showError(Utils.Messages.UNKNOWN_ERROR);
                            return;
                        }
                        UsersCollection.Clear();
                        RemovedUsers.Clear();
                        foreach (var item in result.Data)
                        {
                            UsersCollection.Add(item);
                        }
                        Modified = false;
                    }, null));
        }
    }
}
