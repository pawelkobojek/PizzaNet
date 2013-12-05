using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PizzaNetCommon.DTOs;
using PizzaNetDataModel.Model;

namespace PizzaService.Assemblers
{
    public class IngredientAssembler
    {
        public IngredientDTO ToSimpleDto(Ingredient ing)
        {
            IngredientDTO ingDto = new IngredientDTO
            {
                ExtraWeight = ing.ExtraWeight,
                IngredientID = ing.IngredientID,
                Name = ing.Name,
                NormalWeight = ing.NormalWeight,
                PricePerUnit = ing.PricePerUnit,
                StockQuantity = ing.StockQuantity,
                IsPartOfRecipe = ing.Recipies!=null
            };

            return ingDto;
        }

    }
}