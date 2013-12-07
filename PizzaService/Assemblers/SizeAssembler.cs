using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Services;
using PizzaNetDataModel.Model;

namespace PizzaService.Assemblers
{
    public class SizeAssembler
    {
        public SizeDTO ToSimpleDto(Size s)
        {
            return new SizeDTO
            {
                SizeID = s.SizeID,
                SizeValue = s.SizeValue
            };
        }
    }
}