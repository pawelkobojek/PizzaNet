using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.Requests
{
    [DataContract]
    public class SingleItemResponse<TData>
    {
        [DataMember]
        public TData Data { get; set; }

        public SingleItemResponse(TData data)
        {
            Data = data;
        }
    }

    public static class SingleItemResponse
    {
        public static SingleItemResponse<TData> Create<TData>(TData data)
        {
            return new SingleItemResponse<TData>(data);
        }
    }
}
