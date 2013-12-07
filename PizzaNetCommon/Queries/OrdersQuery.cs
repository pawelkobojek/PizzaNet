using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.DTOs;

namespace PizzaNetCommon.Queries
{
    [DataContract]
    public class OrdersQuery : QueryBase
    {
        [DataMember]
        public int StateValue { get; set; }

        [DataMember]
        public Func<OrderDTO, bool> Predicate { get; set; }
    }
}
