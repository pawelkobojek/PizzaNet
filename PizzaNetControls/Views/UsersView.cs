﻿using PizzaNetControls.Workers;
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

namespace PizzaNetControls.Views
{
    public class UsersView : BaseView
    {
        public ObservableCollection<UserDTO> UsersCollection { get; set; }
        public ObservableCollection<string> Rights { get; set; }
        public ObservableCollection<UserDTO RemovedUsers { get; set; }
        public bool Modified { get; set; }

        public UsersView(IWorker worker)
            : base(worker)
        {
            UsersCollection = new ObservableCollection<UserDTO>();
            Rights = new ObservableCollection<string>()
                {
                    "Customer",
                    "Employee",
                    "Admin"
                };
        }

        internal void RefreshUsers()
        {
            UsersCollection.Clear();
            Worker.EnqueueTask(new WorkerTask(args =>
                {
                    try
                    {
                        using (var proxy = new WorkChannel(ClientConfig.getConfig().ServerAddress))
                        {
                            return proxy.GetUsers(new EmptyRequest
                            {
                                Login = ClientConfig.getConfig().User.Email,
                                Password = ClientConfig.getConfig().User.Password
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
                        var result = e.Result as ListResponse<UserDTO>;
                        if (result == null)
                        {
                            Utils.showError("blabla", "blablablabla");
                            return;
                        }

                        foreach (var user in result.Data)
                        {
                            UsersCollection.Add(user);
                        }

                    }, null));
        }

        private int counter = 1;
        internal void AddUser()
        {
            UsersCollection.Add(new UserDTO { Address = "Address", Email = "Email " + counter.ToString(), Name = "Name", UserID=-counter, Phone = -1, Rights = 1 });
            counter++;
            Modified = true;
        }

        internal void RemoveUser(int index)
        {
            RemovedUsers.Add(UsersCollection[index]);
            UsersCollection.RemoveAt(index);
        }
    }
}
