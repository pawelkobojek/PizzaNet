using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PizzaNetCommon.DTOs;

namespace PizzaService.Assemblers
{
    public class StateAssembler
    {
        public StateDTO ToSimpleDto(PizzaNetDataModel.Model.State state)
        {
            return new StateDTO { StateID = state.StateID, StateValue = state.StateValue };
        }
    }
}
