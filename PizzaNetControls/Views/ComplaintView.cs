using PizzaNetCommon.DTOs;
using PizzaNetCommon.Requests;
using PizzaNetControls.Common;
using PizzaNetControls.Configuration;
using PizzaNetControls.Workers;
using PizzaNetWorkClient.WCFClientInfrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetControls.Views
{
    public class ComplaintView : BaseView
    {
        public ObservableCollection<ComplaintDTO> ComplaintsCollection { get; set; }

        public ComplaintView(IWorker worker)
            : base(worker)
        {
            ComplaintsCollection = new ObservableCollection<ComplaintDTO>();
        }

        public void RefreshComplaints()
        {
            ComplaintsCollection.Clear();
            Worker.EnqueueTask(new WorkerTask((a) =>
                {
                    try
                    {
                        using (var proxy = new WorkChannel())
                        {
                            var res = proxy.GetComplaints(new EmptyRequest
                                {
                                    Login = ClientConfig.CurrentUser.Email,
                                    Password = ClientConfig.CurrentUser.Password
                                });
                            return res.Data;
                        }
                    }
                    catch (Exception e)
                    {
                        return e;
                    }
                }, (s, e) =>
                {
                    if (e.Result is Exception)
                    {
                        Utils.HandleException(e.Result as Exception);
                        return;
                    }
                    var list = e.Result as List<ComplaintDTO>;
                    foreach (var i in list)
                        ComplaintsCollection.Add(i);
                }, null));
        }
    }
}
