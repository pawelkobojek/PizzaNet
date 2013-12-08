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

        public ListResponse<StockIngredientDTO> GetIngredients(EmptyRequest req)
        {
            return Channel.GetIngredients(req);
        }

        public TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<StockIngredientDTO>> GetRecipeTabData(EmptyRequest req)
        {
            return Channel.GetRecipeTabData(req);
        }

        public ListResponse<OrderDTO> GetUndoneOrders(EmptyRequest req)
        {
            return Channel.GetUndoneOrders(req);
        }

        public void SetOrderState(UpdateRequest<OrderDTO> request)
        {
            Channel.SetOrderState(request);
        }

        public ListResponse<OrderDTO> GetOrders(EmptyRequest req)
        {
            return Channel.GetOrders(req);
        }

        public ListResponse<UserDTO> GetUsers(EmptyRequest req)
        {
            return Channel.GetUsers(req);
        }

        public void UpdateIngredient(UpdateRequest<IList<StockIngredientDTO>> request)
        {
            Channel.UpdateIngredient(request);
        }

        public SingleItemResponse<UserDTO> GetUser(RequestBase req)
        {
            return Channel.GetUser(req);
        }
    }
}
