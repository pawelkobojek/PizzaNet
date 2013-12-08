using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.Requests
{
    [DataContract]
    public class UpdateOrRemoveRequest<TData> : RequestBase
    {
        [DataMember]
        public TData Data { get; set; }

        [DataMember]
        public TData DataToRemove { get; set; }
    }
}
