using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Queries;
using PizzaNetCommon.Requests;

namespace PizzaNetCommon.Services
{
    [ServiceContract]
    public interface IPizzaService
    {
        [OperationContract]
        ListResponse<StockIngredientDTO> GetIngredients(EmptyRequest req);

        [OperationContract]
        TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>> GetRecipeTabData(EmptyRequest req);

        [OperationContract]
        ListResponse<OrderDTO> GetOrders(EmptyRequest req);

        [OperationContract]
        ListResponse<OrderDTO> GetUndoneOrders(EmptyRequest req);

        [OperationContract]
        ListResponse<UserDTO> GetUsers(EmptyRequest req);

        [OperationContract]
        void SetOrderState(UpdateRequest<OrderDTO> request);

        [OperationContract]
        ListResponse<OrderSuppliesDTO> OrderSupplies(UpdateRequest<List<OrderSuppliesDTO>> request);

        [OperationContract]
        ListResponse<StockIngredientDTO> UpdateIngredient(UpdateRequest<List<StockIngredientDTO>> request);

        [OperationContract]
        ListResponse<StockIngredientDTO> UpdateOrRemoveIngredient(UpdateOrRemoveRequest<List<StockIngredientDTO>> request);

        [OperationContract]
        SingleItemResponse<UserDTO> GetUser(EmptyRequest req);

        [OperationContract]
        ListResponse<OrderDTO> GetOrdersForUser(EmptyRequest req);

        [OperationContract]
        void RemoveOrder(UpdateOrRemoveRequest<OrderDTO> request);

        [OperationContract]
        void MakeOrder(UpdateRequest<OrderDTO> req);

        [OperationContract]
        void InsertRecipe(UpdateRequest<RecipeDTO> req);

        [OperationContract]
        TrioResponse<List<RecipeDTO>, List<OrderIngredientDTO>, int> UpdateOrRmoveRecipe(UpdateOrRemoveRequest<List<RecipeDTO>> req);

        [OperationContract]
        ListResponse<UserDTO> UpdateOrRemoveUser(UpdateOrRemoveRequest<List<UserDTO>> req);
    }
}
