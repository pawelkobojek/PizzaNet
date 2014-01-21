using PizzaNetCommon.DTOs;
using PizzaNetCommon.Queries;
using PizzaNetCommon.Requests;
using PizzaNetWorkClient.WCFClientInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetTests.Mocks
{
    public class WorkChannelMock : IWorkChannel
    {
        public List<OrderInfoDTO> OrdersMade { get; set; }
        public List<OrderInfoDTO> OrdersRemoved { get; set; }

        public List<UserDTO> UsersUpdated { get; set; }

        public List<ComplaintDTO> ComplaintsMade { get; set; }

        public List<UserDTO> UsersRegistered { get; set; }

        public ListResponse<StockIngredientDTO> GetIngredients(EmptyRequest req)
        {
            throw new NotImplementedException();
        }

        public SingleItemResponse<UserDTO> RegisterUser(UpdateRequest<UserDTO> req)
        {
            UsersRegistered = new List<UserDTO>();
            UsersRegistered.Add(req.Data);
            return new SingleItemResponse<UserDTO>(req.Data);
        }

        public TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>> GetRecipeTabData(EmptyRequest req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
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
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public ListResponse<OrderDTO> GetUndoneOrders(EmptyRequest req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public ListResponse<UserDTO> GetUsers(EmptyRequest req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public SingleItemResponse<OrderDTO> SetOrderState(UpdateRequest<OrderDTO> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public ListResponse<OrderSuppliesDTO> OrderSupplies(UpdateRequest<List<OrderSuppliesDTO>> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public ListResponse<StockIngredientDTO> UpdateIngredient(UpdateRequest<List<StockIngredientDTO>> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public ListResponse<StockIngredientDTO> UpdateOrRemoveIngredient(UpdateOrRemoveRequest<List<StockIngredientDTO>> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public SingleItemResponse<UserDTO> GetUser(EmptyRequest req)
        {
            if (req.Login != "Admin" || req.Password != "123") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            return new SingleItemResponse<UserDTO>(new UserDTO() { Email = "Admin", Password = "123" });
        }

        public ListResponse<OrderDTO> GetOrdersForUser(EmptyRequest req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            return new ListResponse<OrderDTO>(new List<OrderDTO>()
                {
                    new OrderDTO()
                    {
                    }
                });
        }

        public void RemoveOrder(UpdateOrRemoveRequest<OrderDTO> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public void MakeOrder(UpdateRequest<OrderDTO> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public void InsertRecipe(UpdateRequest<RecipeDTO> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public TrioResponse<List<RecipeDTO>, List<OrderIngredientDTO>, int> UpdateOrRemoveRecipe(UpdateOrRemoveRequest<List<RecipeDTO>> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public ListResponse<UserDTO> UpdateOrRemoveUser(UpdateOrRemoveRequest<List<UserDTO>> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }

        public SingleItemResponse<UserDTO> UpdateUser(UpdateRequest<UserDTO> req)
        {
            if (req.Login != "Admin" || req.Password!="123")
                throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user or password"));
            UsersUpdated = new List<UserDTO>();
            UsersUpdated.Add(req.Data);
            return new SingleItemResponse<UserDTO>(req.Data);
        }

        public ListResponse<OrderIngredientDTO> QueryIngredients(QueryRequest<IngredientsQuery> req)
        {
            if (req.Query.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
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
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            OrdersMade = req.Data;
            OrdersRemoved = req.DataToRemove;
        }

        public ListResponse<OrderDTO> GetOrderInfo(PizzaNetCommon.Queries.OrdersQuery req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            List<OrderDTO> list = new List<OrderDTO>();
            foreach (int i in req.Ids)
                list.Add(new OrderDTO() { OrderID = i });
            return new ListResponse<OrderDTO>(list);
        }

        public void Dispose()
        {
        }


        public void CreateComplaint(UpdateRequest<ComplaintDTO> req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            ComplaintsMade = new List<ComplaintDTO>();
            ComplaintsMade.Add(req.Data);
        }

        public ListResponse<ComplaintDTO> GetComplaints(EmptyRequest req)
        {
            if (req.Login != "Admin") throw new FaultException<PizzaServiceFault>(new PizzaServiceFault("wrong user"));
            throw new NotImplementedException();
        }
    }
}
