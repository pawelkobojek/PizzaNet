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
        public StockIngredientDTO ToSimpleDto(Ingredient ing)
        {
            StockIngredientDTO ingDto = new StockIngredientDTO
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

        public Ingredient ToEntityWithEmptyRecipies(StockIngredientDTO ing)
        {
            Ingredient ingDto = new Ingredient
            {
                ExtraWeight = ing.ExtraWeight,
                IngredientID = ing.IngredientID,
                Name = ing.Name,
                NormalWeight = ing.NormalWeight,
                PricePerUnit = ing.PricePerUnit,
                StockQuantity = ing.StockQuantity,
                Recipies = new List<Recipe>()
            };

            return ingDto;
        }

        public void UpdateIngredient(Ingredient ing, StockIngredientDTO dto)
        {
            if(ing.IngredientID!=dto.IngredientID)
                return;

            ing.ExtraWeight = dto.ExtraWeight;
            ing.Name = dto.Name;
            ing.NormalWeight = dto.NormalWeight;
            ing.PricePerUnit = dto.PricePerUnit;
        }

    }
}