using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Queries;
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

        public ListResponse<StockIngredientDTO> GetIngredients()
        {
            return Channel.GetIngredients();
        }

        public TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<StockIngredientDTO>> GetRecipesTabData()
        {
            return Channel.GetRecipeTabData();
        }

        public ListResponse<OrderDTO> GetUndoneOrders()
        {
            return Channel.GetUndoneOrders();
        }
    }
}
