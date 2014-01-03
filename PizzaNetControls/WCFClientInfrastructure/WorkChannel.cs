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
        public WorkChannel()
            : base()
        {
        }

        public ListResponse<StockIngredientDTO> GetIngredients(EmptyRequest req)
        {
            return Channel.GetIngredients(req);
        }

        public TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>> GetRecipeTabData(EmptyRequest req)
        {
            return Channel.GetRecipeTabData(req);
        }

        public ListResponse<OrderDTO> GetUndoneOrders(EmptyRequest req)
        {
            return Channel.GetUndoneOrders(req);
        }

        public SingleItemResponse<OrderDTO> SetOrderState(UpdateRequest<OrderDTO> request)
        {
            return Channel.SetOrderState(request);
        }

        public ListResponse<OrderDTO> GetOrders(EmptyRequest req)
        {
            return Channel.GetOrders(req);
        }

        public ListResponse<UserDTO> GetUsers(EmptyRequest req)
        {
            return Channel.GetUsers(req);
        }

        public SingleItemResponse<UserDTO> GetUser(EmptyRequest req)
        {
            return Channel.GetUser(req);
        }

        public ListResponse<StockIngredientDTO> UpdateIngredient(UpdateRequest<List<StockIngredientDTO>> request)
        {
            return Channel.UpdateIngredient(request);
        }

        public ListResponse<StockIngredientDTO> UpdateOrRemoveIngredient(UpdateOrRemoveRequest<List<StockIngredientDTO>> request)
        {
            return Channel.UpdateOrRemoveIngredient(request);
        }


        public ListResponse<OrderDTO> GetOrdersForUser(EmptyRequest req)
        {
            return Channel.GetOrdersForUser(req);
        }


        public ListResponse<OrderSuppliesDTO> OrderSupplies(UpdateRequest<List<OrderSuppliesDTO>> request)
        {
            return Channel.OrderSupplies(request);
        }


        public void RemoveOrder(UpdateOrRemoveRequest<OrderDTO> request)
        {
            Channel.RemoveOrder(request);
        }


        public void MakeOrder(UpdateRequest<OrderDTO> req)
        {
            Channel.MakeOrder(req);
        }

        public void InsertRecipe(UpdateRequest<RecipeDTO> req)
        {
            Channel.InsertRecipe(req);
        }


        public TrioResponse<List<RecipeDTO>, List<OrderIngredientDTO>, int> UpdateOrRemoveRecipe(UpdateOrRemoveRequest<List<RecipeDTO>> req)
        {
            return Channel.UpdateOrRemoveRecipe(req);
        }


        public ListResponse<UserDTO> UpdateOrRemoveUser(UpdateOrRemoveRequest<List<UserDTO>> req)
        {
            return Channel.UpdateOrRemoveUser(req);
        }

        public SingleItemResponse<UserDTO> RegisterUser(UpdateRequest<UserDTO> req)
        {
            return Channel.RegisterUser(req);
        }


        public SingleItemResponse<UserDTO> UpdateUser(UpdateRequest<UserDTO> req)
        {
            return Channel.UpdateUser(req);
        }


        public ListResponse<OrderIngredientDTO> QueryIngredients(QueryRequest<IngredientsQuery> req)
        {
            return Channel.QueryIngredients(req);
        }

        public void MakeOrderFromWeb(UpdateOrRemoveRequest<List<OrderInfoDTO>> req)
        {
            Channel.MakeOrderFromWeb(req);
        }
    }
}
