using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new PizzaContext())
            {
                var query = from r in db.Recipies
                            select r;

                foreach (var item in query)
                {
                    Console.WriteLine(item.Name);
                }
            }
        }
    }
}
