using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Requests;

namespace PizzaNetCommon.Services
{
    [ServiceContract]
    public interface IPizzaService
    {
        [OperationContract]
        ListResponse<IngredientDTO> GetIngredients();

        [OperationContract]
        TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<IngredientDTO>> GetRecipeTabData();
    }
}
