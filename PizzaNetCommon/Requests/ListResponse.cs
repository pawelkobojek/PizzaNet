using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.Requests
{
    [DataContract]
    public class ListResponse<TData>
    {
        [DataMember]
        public List<TData> Data { get; set; }

        public ListResponse(List<TData> data)
        {
            this.Data = data;
        }
    }

    public static class ListResponse
    {
        public static ListResponse<TData> Create<TData>(List<TData> data)
        {
            return new ListResponse<TData>(data);
        }
    }
}
