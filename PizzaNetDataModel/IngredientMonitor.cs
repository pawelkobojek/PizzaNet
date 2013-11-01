using PizzaNetDataModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel
{
    public class IngredientMonitor
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
                PricePerUnit = ingr.PricePerUnit
            };
        }
        private static bool isEqual(Ingredient first, Ingredient second)
        {
            return
                first.IngredientID == second.IngredientID &&
                first.Name == second.Name &&
                first.NormalWeight == second.NormalWeight &&
                first.ExtraWeight == second.ExtraWeight &&
                first.PricePerUnit == second.PricePerUnit;
        }

        public void StartMonitor(Ingredient ingredient)
        {
            _prevState = clone(ingredient);
        }
        public bool Update(Ingredient ingredient)
        {
            bool res = HasStateChanged(ingredient);

            return res;
        }
        public bool HasStateChanged(Ingredient ingredient)
        {
            return isEqual(_prevState, ingredient);
        }
    }
}
