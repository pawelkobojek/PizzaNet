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
        ListResponse<StockIngredientDTO> GetIngredients();

        [OperationContract]
        TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<StockIngredientDTO>> GetRecipeTabData();

        [OperationContract]
        ListResponse<OrderDTO> GetOrders();

        [OperationContract]
        ListResponse<OrderDTO> GetUndoneOrders();

        [OperationContract]
        ListResponse<UserDTO> GetUsers();

        [OperationContract]
        void SetOrderState(UpdateRequest<OrderDTO> request);

        [OperationContract]
        void UpdateIngredient(UpdateRequest<IList<StockIngredientDTO>> request);
    }
}
