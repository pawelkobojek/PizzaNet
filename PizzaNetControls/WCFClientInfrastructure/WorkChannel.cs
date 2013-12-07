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
    public interface IWorkChannel : IPizzaService
    {
    }

    public class WorkChannel : ServiceProxyBase<IPizzaService>, IWorkChannel
    {
        public WorkChannel(string address)
            : base(address)
        {
        }

        public ListResponse<StockIngredientDTO> GetIngredients()
        {
            return Channel.GetIngredients();
        }

        public TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<StockIngredientDTO>> GetRecipeTabData()
        {
            return Channel.GetRecipeTabData();
        }

        public ListResponse<OrderDTO> GetUndoneOrders()
        {
            return Channel.GetUndoneOrders();
        }

        public void SetOrderState(UpdateRequest<OrderDTO> request)
        {
            Channel.SetOrderState(request);
        }

        public ListResponse<OrderDTO> GetOrders()
        {
            return Channel.GetOrders();
        }

        public ListResponse<UserDTO> GetUsers()
        {
            return Channel.GetUsers();
        }

        public void UpdateIngredient(UpdateRequest<IList<StockIngredientDTO>> request)
        {
            Channel.UpdateIngredient(request);
        }
    }
}
