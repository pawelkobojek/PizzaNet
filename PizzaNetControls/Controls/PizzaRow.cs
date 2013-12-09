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

        public PizzaRow(OrderDetail orderDetail)
            : this()
        {
            this._orderDetail = orderDetail;
        }

        public PizzaRow(PizzaNetCommon.DTOs.OrderDetailDTO od)
        {
            this.od = od;
        }

        private OrderDetail _orderDetail = new OrderDetail();
        public OrderDetail OrderDetail
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
        private OrderDetailDTO od;
        public OrderDetailDTO OrderDetailDTO
        {
            get { return od; }
            set
            {
                od = value; NotifyPropertyChanged("OrderDetailDTO");
            }
        }
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
