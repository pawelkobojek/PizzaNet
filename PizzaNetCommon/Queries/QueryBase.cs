using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.Requests;

namespace PizzaNetCommon.Queries
{
    [DataContract]
    public abstract class QueryBase : RequestBase
    {
    }
}
