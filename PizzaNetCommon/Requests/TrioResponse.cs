using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.Requests
{
    [DataContract]
    public class TrioResponse<F, S, T>
    {
        [DataMember]
        public F First { get; set; }

        [DataMember]
        public S Second { get; set; }

        [DataMember]
        public T Third { get; set; }

        public TrioResponse(F first, S second, T third)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
        }
    }

    public static class TrioResponse
    {
        public static TrioResponse<F, S, T> Create<F, S, T>(F first, S second, T third)
        {
            return new TrioResponse<F, S, T>(first, second, third);
        }
    }
}
