using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Requests;
using PizzaNetCommon.Services;

namespace PizzaNetWorkClient.WCFClientInfrastructure
{
    public class WorkChannel : ServiceProxyBase<IPizzaService>
    {
        public WorkChannel(string address)
            : base(address)
        {
        }

        public ListResponse<IngredientDTO> GetIngredients()
        {
            return Channel.GetIngredients();
        }

        public TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<IngredientDTO>> GetRecipesTabData()
        {
            return Channel.GetRecipeTabData();
        }
    }
}
