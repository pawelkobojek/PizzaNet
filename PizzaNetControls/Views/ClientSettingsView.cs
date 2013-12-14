using PizzaNetControls.Configuration;
using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.DTOs;
using PizzaNetWorkClient.WCFClientInfrastructure;
using PizzaNetCommon.Requests;

namespace PizzaNetControls.Views
{
    public class ClientSettingsView : BaseView
    {
        public ClientSettingsView(IWorker worker)
            : base(worker)
        {

        }

        private User user;
        public User User
        {
            get { return user; }
            set { user = value; NotifyPropertyChanged("User"); }
        }

        public void Load()
        {
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                //MODIFIED return ClientConfig.getConfig();
                return null;
            }, (s, args) =>
            {
                //MODIFIED Config = args.Result as ClientConfig;
            }));
        }

        internal void SaveUserInfo()
        {
            Worker.EnqueueTask(new WorkerTask(args =>
                {
                    using (var proxy = new WorkChannel())
                    {
                        List<UserDTO> userData = new List<UserDTO>()
                        {
                            new UserDTO
                            {
                                UserID=User.UserID,
                                Address=User.Address,
                                Email=User.Email, 
                                Name=User.Name,
                                Password=User.Password,
                                Phone=User.Phone,
                                Rights=User.Rights
                            }
                        };

                        proxy.UpdateOrRemoveUser(new UpdateOrRemoveRequest<List<UserDTO>>
                        {
                            Data = userData,
                            DataToRemove = null,
                            Login = User.Email,
                            Password = User.Password
                        });
                        return null;
                        //return new User
                        //{
                        //    UserID = userData[0].UserID,
                        //    Address = userData[0].Address,
                        //    Email = userData[0].Email,
                        //    Name = userData[0].Name,
                        //    Password = userData[0].Password,
                        //    Phone = userData[0].Phone,
                        //    Rights = userData[0].Rights
                        //};
                    }
                }, (s, e) =>
                    {

                    }, null));
        }
    }
}
