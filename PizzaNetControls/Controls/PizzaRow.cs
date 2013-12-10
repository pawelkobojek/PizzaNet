using PizzaNetDataModel.Model;
using PizzaNetControls.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.DTOs;

namespace PizzaNetControls.Controls
{
    public class PizzaRow : INotifyPropertyChanged
    {
        public PizzaRow()
        {
        }

        public PizzaRow(OrderDetailDTO orderDetail)
            : this()
        {
            this.OrderDetail = orderDetail;
        }

        private OrderDetailDTO _orderDetail = new OrderDetailDTO();
        public OrderDetailDTO OrderDetail
        {
            get
            {
                return _orderDetail;
            }
            set
            {
                _orderDetail = value;
                NotifyPropertyChanged("OrderDetail");
            }
        }

        public void Update()
        {
            NotifyPropertyChanged("OrderDetail");
            NotifyPropertyChanged("PizzaCost");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
