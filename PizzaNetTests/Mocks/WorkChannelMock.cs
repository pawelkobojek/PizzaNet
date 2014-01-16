using PizzaNetCommon.DTOs;
using PizzaNetCommon.Queries;
using PizzaNetCommon.Requests;
using PizzaNetWorkClient.WCFClientInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetTests.Mocks
{
    public class WorkChannelMock : IWorkChannel
    {
        public List<OrderInfoDTO> OrdersMade { get; set; }
        public List<OrderInfoDTO> OrdersRemoved { get; set; }

        public ListResponse<StockIngredientDTO> GetIngredients(EmptyRequest req)
        {
            throw new NotImplementedException();
        }

        public SingleItemResponse<UserDTO> RegisterUser(UpdateRequest<UserDTO> req)
        {
            throw new NotImplementedException();
        }

        public TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>> GetRecipeTabData(EmptyRequest req)
        {
            List<RecipeDTO> lr = new List<RecipeDTO>()
            {
                new RecipeDTO()
                {
                    Ingredients = new List<OrderIngredientDTO>(),
                    Name = "MyRecipe",
                    RecipeID = 1
                }
            };
            List<SizeDTO> ls = new List<SizeDTO>()
            {
                new SizeDTO()
                {
                    SizeID = 0
                },
                new SizeDTO()
                {
                    SizeID = 1
                },
                new SizeDTO()
                {
                    SizeID = 2
                }
            };
            List<OrderIngredientDTO> lo = new List<OrderIngredientDTO>()
            {
                new OrderIngredientDTO()
                {
                    Name = "Ingredient"
                }
            };
            return new TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>>(lr, ls, lo);
        }

        public ListResponse<OrderDTO> GetOrders(EmptyRequest req)
        {
            throw new NotImplementedException();
        }

        public ListResponse<OrderDTO> GetUndoneOrders(EmptyRequest req)
        {
            throw new NotImplementedException();
        }

        public ListResponse<UserDTO> GetUsers(EmptyRequest req)
        {
            throw new NotImplementedException();
        }

        public SingleItemResponse<OrderDTO> SetOrderState(UpdateRequest<OrderDTO> request)
        {
            throw new NotImplementedException();
        }

        public ListResponse<OrderSuppliesDTO> OrderSupplies(UpdateRequest<List<OrderSuppliesDTO>> request)
        {
            throw new NotImplementedException();
        }

        public ListResponse<StockIngredientDTO> UpdateIngredient(UpdateRequest<List<StockIngredientDTO>> request)
        {
            throw new NotImplementedException();
        }

        public ListResponse<StockIngredientDTO> UpdateOrRemoveIngredient(UpdateOrRemoveRequest<List<StockIngredientDTO>> request)
        {
            throw new NotImplementedException();
        }

        public SingleItemResponse<UserDTO> GetUser(EmptyRequest req)
        {
            throw new NotImplementedException();
        }

        public ListResponse<OrderDTO> GetOrdersForUser(EmptyRequest req)
        {
            return new ListResponse<OrderDTO>(new List<OrderDTO>()
                {
                    new OrderDTO()
                    {
                    }
                });
        }

        public void RemoveOrder(UpdateOrRemoveRequest<OrderDTO> request)
        {
            throw new NotImplementedException();
        }

        public void MakeOrder(UpdateRequest<OrderDTO> req)
        {
            throw new NotImplementedException();
        }

        public void InsertRecipe(UpdateRequest<RecipeDTO> req)
        {
            throw new NotImplementedException();
        }

        public TrioResponse<List<RecipeDTO>, List<OrderIngredientDTO>, int> UpdateOrRemoveRecipe(UpdateOrRemoveRequest<List<RecipeDTO>> req)
        {
            throw new NotImplementedException();
        }

        public ListResponse<UserDTO> UpdateOrRemoveUser(UpdateOrRemoveRequest<List<UserDTO>> req)
        {
            throw new NotImplementedException();
        }

        public SingleItemResponse<UserDTO> UpdateUser(UpdateRequest<UserDTO> req)
        {
            throw new NotImplementedException();
        }

        public ListResponse<OrderIngredientDTO> QueryIngredients(QueryRequest<IngredientsQuery> req)
        {
            var list = new List<OrderIngredientDTO>();
            foreach (var i in req.Query.IngredientIds)
                list.Add(new OrderIngredientDTO()
                    {
                        Name = String.Format("Ingredient {0}",i),
                        IngredientID = i
                    });
            return new ListResponse<OrderIngredientDTO>(list);
        }

        public void MakeOrderFromWeb(UpdateOrRemoveRequest<List<OrderInfoDTO>> req)
        {
            OrdersMade = req.Data;
            OrdersRemoved = req.DataToRemove;
        }

        public ListResponse<OrderDTO> GetOrderInfo(PizzaNetCommon.Queries.OrdersQuery req)
        {
            List<OrderDTO> list = new List<OrderDTO>();
            foreach (int i in req.Ids)
                list.Add(new OrderDTO() { OrderID = i });
            return new ListResponse<OrderDTO>(list);
        }

        public void Dispose()
        {
        }
    }
}
