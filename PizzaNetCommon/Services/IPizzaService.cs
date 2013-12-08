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
        TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<StockIngredientDTO>> GetRecipeTabData(EmptyRequest req);

        [OperationContract]
        ListResponse<OrderDTO> GetOrders(EmptyRequest req);

        [OperationContract]
        ListResponse<OrderDTO> GetUndoneOrders(EmptyRequest req);

        [OperationContract]
        ListResponse<UserDTO> GetUsers(EmptyRequest req);

        [OperationContract]
        void SetOrderState(UpdateRequest<OrderDTO> request);

        [OperationContract]
        ListResponse<StockIngredientDTO> UpdateIngredient(UpdateRequest<IList<StockIngredientDTO>> request);

        [OperationContract]
        ListResponse<StockIngredientDTO> UpdateOrRemoveIngredient(UpdateOrRemoveRequest<IList<StockIngredientDTO>> request);

        [OperationContract]
        SingleItemResponse<UserDTO> GetUser(EmptyRequest req);
    }
}
