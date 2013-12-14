using PizzaNetCommon.DTOs;
using PizzaNetCommon.Requests;
using PizzaNetControls.Common;
using PizzaNetControls.Configuration;
using PizzaNetWorkClient.WCFClientInfrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PizzaNetControls.Dialogs
{
    /// <summary>
    /// Interaction logic for OrderIngredientsDialog.xaml
    /// </summary>
    public partial class OrderIngredientsDialog : Window
    {
        public OrderIngredientsDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            Data = new ObservableCollection<OrderSuppliesDTO>();
        }

        public void SetData(IEnumerable<StockIngredientDTO> list)
        {
            foreach (var v in list)
                Data.Add(new OrderSuppliesDTO()
                {
                    IngredientID = v.IngredientID,
                    StockQuantity = v.StockQuantity,
                    Name = v.Name,
                    OrderValue = 0
                });
        }

        public ObservableCollection<OrderSuppliesDTO> Data { get; set; }
        private const string ORDER_FAILURE = "Ordering supplies failed";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            worker.EnqueueTask(new Workers.WorkerTask((args) =>
                {
                    var list = args[0] as IList<OrderSuppliesDTO>;
                    try
                    {
                        using (var proxy = new WorkChannel())
                        {
                            return proxy.OrderSupplies(new UpdateRequest<IList<OrderSuppliesDTO>>()
                            {
                                Data = list,
                                Login = ClientConfig.CurrentUser.Email,
                                Password = ClientConfig.CurrentUser.Password
                            });
                        }
                    }
                    catch(Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                        return null;
                    }
                },
                (s,ex) =>
                {
                    var res = ex.Result as ListResponse<OrderSuppliesDTO>;
                    if (res==null)
                    {
                        Utils.showError(ORDER_FAILURE);
                        return;
                    }
                    Data.Clear();
                    foreach(var item in res.Data)
                        Data.Add(item);
                }, Data.ToList()));
        }
    }
}
