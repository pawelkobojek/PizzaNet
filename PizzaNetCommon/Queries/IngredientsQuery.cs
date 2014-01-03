using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.Queries
{
    [DataContract]
    public class IngredientsQuery : QueryBase
    {
        [DataMember]
        public int[] IngredientIds { get; set; }
    }
}
