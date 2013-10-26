using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            Database.SetInitializer<PizzaContext>(new PizzaContextInitializer());
            using (var db = new PizzaContext())
            {
                var query = from r in db.Recipies
                            select r;
                //var q = from i in db.Ingredients
                //        select i;



                foreach (var item in query)
                {
                    Console.WriteLine(item.Name);
                    foreach (var itanz in item.Ingredients)
                    {
                        Console.WriteLine("\t{0}", itanz.Name);
                    }
                }

                //foreach (var item in q)
                //{
                //    Console.WriteLine(item.Name);
                //}
            }
        }
    }
}
