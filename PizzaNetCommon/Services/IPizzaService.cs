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
        [FaultContract(typeof(PizzaServiceFault))]
        ListResponse<StockIngredientDTO> GetIngredients(EmptyRequest req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        SingleItemResponse<UserDTO> RegisterUser(UpdateRequest<UserDTO> req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>> GetRecipeTabData(EmptyRequest req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        ListResponse<OrderDTO> GetOrders(EmptyRequest req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        ListResponse<OrderDTO> GetUndoneOrders(EmptyRequest req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        ListResponse<UserDTO> GetUsers(EmptyRequest req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        SingleItemResponse<OrderDTO> SetOrderState(UpdateRequest<OrderDTO> request);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        ListResponse<OrderSuppliesDTO> OrderSupplies(UpdateRequest<List<OrderSuppliesDTO>> request);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        ListResponse<StockIngredientDTO> UpdateIngredient(UpdateRequest<List<StockIngredientDTO>> request);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        ListResponse<StockIngredientDTO> UpdateOrRemoveIngredient(UpdateOrRemoveRequest<List<StockIngredientDTO>> request);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        SingleItemResponse<UserDTO> GetUser(EmptyRequest req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        ListResponse<OrderDTO> GetOrdersForUser(EmptyRequest req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        void RemoveOrder(UpdateOrRemoveRequest<OrderDTO> request);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        void MakeOrder(UpdateRequest<OrderDTO> req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        void InsertRecipe(UpdateRequest<RecipeDTO> req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        TrioResponse<List<RecipeDTO>, List<OrderIngredientDTO>, int> UpdateOrRemoveRecipe(UpdateOrRemoveRequest<List<RecipeDTO>> req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        ListResponse<UserDTO> UpdateOrRemoveUser(UpdateOrRemoveRequest<List<UserDTO>> req);

        [OperationContract]
        [FaultContract(typeof(PizzaServiceFault))]
        SingleItemResponse<UserDTO> UpdateUser(UpdateRequest<UserDTO> req);

        [OperationContract]
        ListResponse<OrderIngredientDTO> QueryIngredients(QueryRequest<IngredientsQuery> req);

        [OperationContract]
        void MakeOrderFromWeb(UpdateOrRemoveRequest<List<OrderInfoDTO>> req);

        [OperationContract]
        ListResponse<OrderDTO> GetOrderInfo(OrdersQuery req);

        [OperationContract]
        void CreateComplaint(UpdateRequest<ComplaintDTO> req);

        [OperationContract]
        ListResponse<ComplaintDTO> GetComplaints(EmptyRequest req);
    }
}
