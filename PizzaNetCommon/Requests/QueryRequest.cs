using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.Queries;

namespace PizzaNetCommon.Requests
{
    [DataContract]
    public class QueryRequest<TQuery> where TQuery : QueryBase
    {
        public QueryRequest()
        {
        }

        public QueryRequest(TQuery query)
        {
            Query = query;
        }

        [DataMember]
        public TQuery Query { get; set; }
    }

    public static class QueryRequest
    {
        public static QueryRequest<T> New<T>(T query)
            where T : QueryBase
        {
            return new QueryRequest<T>(query);
        }
    }
}
