using System;
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
                OrderID = o.OrderID,
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
                        Quantity = ing.Quantity,
                        ExtraWeight = ing.Ingredient.ExtraWeight,
                        NormalWeight = ing.Ingredient.NormalWeight
                    });
                }
                oDto.OrderDetailsDTO.Add(new OrderDetailDTO { OrderDetailID=od.OrderDetailID, Ingredients = oIngDtos, Size = new SizeDTO { SizeValue = od.Size.SizeValue } });
            }


            return oDto;
        }

        public void UpdateEntity(Order order, OrderDTO orderDTO)
        {
            order.StateID = orderDTO.State.StateID;
        }
    }
}