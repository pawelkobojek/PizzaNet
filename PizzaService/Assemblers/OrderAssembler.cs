﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PizzaNetCommon.DTOs;
using PizzaNetDataModel.Model;

namespace PizzaService.Assemblers
{
    public class OrderAssembler
    {
        public OrderDTO ToSimpleDto(Order o)
        {
            OrderDTO oDto = new OrderDTO
            {
                Address = o.Address,
                CustomerPhone = o.CustomerPhone,
                Date = o.Date,
                State = (new StateAssembler()).ToSimpleDto(o.State)
            };

            oDto.OrderDetailsDTO = new List<OrderDetailDTO>();

            foreach (var od in o.OrderDetails)
            {
                List<OrderIngredientDTO> oIngDtos = new List<OrderIngredientDTO>();
                foreach (var ing in od.Ingredients)
                {
                    oIngDtos.Add(new OrderIngredientDTO
                    {
                        IngredientID = ing.IngredientID,
                        Name = ing.Ingredient.Name,
                        Price = ing.Ingredient.PricePerUnit,
                        Quantity = ing.Quantity
                    });
                }
                oDto.OrderDetailsDTO.Add(new OrderDetailDTO { Ingredients = oIngDtos });
            }


            return oDto;
        }
    }
}