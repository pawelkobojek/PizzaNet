using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Repository;

namespace PizzaNetDataModel.Monitors
{
    public class IngredientMonitor : IMonitor<Ingredient>
    {
        private Ingredient _prevState;
        private static Ingredient clone(Ingredient ingr)
        {
            return new Ingredient()
            {
                IngredientID = ingr.IngredientID,
                Name = ingr.Name,
                NormalWeight = ingr.NormalWeight,
                ExtraWeight = ingr.ExtraWeight,
                PricePerUnit = ingr.PricePerUnit,
                StockQuantity = ingr.StockQuantity
            };
        }
        private static bool isEqual(Ingredient first, Ingredient second)
        {
            return
                first.IngredientID == second.IngredientID &&
                first.Name == second.Name &&
                first.NormalWeight == second.NormalWeight &&
                first.ExtraWeight == second.ExtraWeight &&
                first.PricePerUnit == second.PricePerUnit &&
                first.StockQuantity == second.StockQuantity;
        }

        public void StartMonitor(Ingredient ingredient)
        {
            _prevState = clone(ingredient);
        }
        public bool Update(Ingredient ingredient)
        {
            bool res = HasStateChanged(ingredient);
            if (res)
            {
                using (var db = new PizzaUnitOfWork())
                {
                    db.Ingredients.Update(_prevState, ingredient);
                    db.Commit();
                }
                StartMonitor(ingredient);
            }
            return res;
        }
        public bool HasStateChanged(Ingredient ingredient)
        {
            return !isEqual(_prevState, ingredient);
        }


        public bool IsMonitoring()
        {
            return _prevState != null;
        }
    }
}
