using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PizzaNetCommon.DTOs;
using PizzaNetDataModel.Model;

namespace PizzaService.Assemblers
{
    public class RecipeAssembler
    {
        public RecipeDTO ToSimpleDto(Recipe r)
        {
            RecipeDTO rDto = new RecipeDTO { Name = r.Name, RecipeID = r.RecipeID };

            if (r.Ingredients != null)
            {
                List<IngredientDTO> ingDtos = new List<IngredientDTO>();

                foreach (var ing in r.Ingredients)
                {
                    ingDtos.Add(new IngredientAssembler().ToSimpleDto(ing));
                }
                rDto.Ingredients = ingDtos;
            }

            return rDto;
        }
    }
}