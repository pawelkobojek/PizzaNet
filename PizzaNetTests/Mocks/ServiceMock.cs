using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Requests;
using PizzaNetCommon.Services;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using PizzaService.Assemblers;

namespace PizzaNetTests.Mocks
{
    public class ServiceMock : IPizzaService
    {
        private readonly PizzaUnitOfWork db = new PizzaUnitOfWork();

        public ListResponse<StockIngredientDTO> GetIngredients()
        {
            return ListResponse.Create(new List<StockIngredientDTO>
                {
                    new StockIngredientDTO{ StockQuantity=20, PricePerUnit=1M, NormalWeight=2, Name="A", IsPartOfRecipe=true, ExtraWeight=3},
                    new StockIngredientDTO{ StockQuantity=25, PricePerUnit=0.5M, NormalWeight=3, Name="B", IsPartOfRecipe=false, ExtraWeight=5}
                });
        }

        public TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<StockIngredientDTO>> GetRecipeTabData()
        {
            return TrioResponse.Create(
                new List<RecipeDTO>
            {
                new RecipeDTO{
                     Ingredients=new List<StockIngredientDTO> {
                            new StockIngredientDTO{ StockQuantity=20, PricePerUnit=1M, NormalWeight=2, Name="C", IsPartOfRecipe=true, ExtraWeight=3},
                            new StockIngredientDTO{ StockQuantity=25, PricePerUnit=0.5M, NormalWeight=3, Name="D", IsPartOfRecipe=true, ExtraWeight=5},
                            new StockIngredientDTO{ StockQuantity=20, PricePerUnit=1M, NormalWeight=2, Name="E", IsPartOfRecipe=true, ExtraWeight=3},
                            new StockIngredientDTO{ StockQuantity=25, PricePerUnit=0.5M, NormalWeight=3, Name="F", IsPartOfRecipe=true, ExtraWeight=5}
                     }, Name="RecipeA"
                }
            },
            new List<SizeDTO>
            {
                new SizeDTO{ SizeValue=Size.MEDIUM}
            },
            new List<StockIngredientDTO>
            {
                new StockIngredientDTO{ StockQuantity=20, PricePerUnit=1M, NormalWeight=2, Name="C", IsPartOfRecipe=true, ExtraWeight=3},
                            new StockIngredientDTO{ StockQuantity=25, PricePerUnit=0.5M, NormalWeight=3, Name="D", IsPartOfRecipe=true, ExtraWeight=5},
                            new StockIngredientDTO{ StockQuantity=20, PricePerUnit=1M, NormalWeight=2, Name="E", IsPartOfRecipe=true, ExtraWeight=3},
                            new StockIngredientDTO{ StockQuantity=25, PricePerUnit=0.5M, NormalWeight=3, Name="F", IsPartOfRecipe=true, ExtraWeight=5}
            });
        }

        public ListResponse<OrderDTO> GetOrders()
        {
            return ListResponse.Create(new List<OrderDTO>
            {
                new OrderDTO
                { 
                    Address="AddressA",
                    CustomerPhone=1,
                    Date=DateTime.Now,
                    State = new StateDTO { StateValue=State.IN_REALISATION},
                    OrderDetailsDTO=new List<OrderDetailDTO> {
                        new OrderDetailDTO {
                            Ingredients=new List<OrderIngredientDTO>{ 
                                new OrderIngredientDTO{ Name="IngredientA", Price=0.1M, Quantity=1},
                                new OrderIngredientDTO{ Name="IngredientC", Price=0.1M, Quantity=3}
                            }
                        },
                        new OrderDetailDTO {
                            Ingredients= new List<OrderIngredientDTO> {
                                new OrderIngredientDTO{ Name="IngredientB", Price=0.2M, Quantity=2}
                            }
                        }
                    }
                },
                new OrderDTO
                {
                    Address="AddressB",
                    CustomerPhone=2,
                    Date=DateTime.Now,
                    State = new StateDTO{ StateValue=State.NEW},
                    OrderDetailsDTO=new List<OrderDetailDTO> {
                        new OrderDetailDTO {
                            Ingredients = new List<OrderIngredientDTO> {
                                new OrderIngredientDTO{ Name="IngredientD", Price =0.15M, Quantity=5}
                            }
                        },
                        new OrderDetailDTO {
                            Ingredients = new List<OrderIngredientDTO> {
                                new OrderIngredientDTO { Name="IngredientE", Price=0.5M, Quantity=20},
                                new OrderIngredientDTO{ Name="IngredientF", Price=0.02M, Quantity=30}
                            }
                        }
                    }
                }
            });
        }

        public ListResponse<OrderDTO> GetUndoneOrders()
        {
            return ListResponse.Create(GetOrders().Data.Where(o => o.State.StateValue == State.NEW ||
                o.State.StateValue == State.IN_REALISATION)
                .ToList());
        }

        public ListResponse<UserDTO> GetUsers()
        {
            return ListResponse.Create(new List<UserDTO>
            {
                new UserDTO{ Email="EmailA", Name="NameA", Phone=1, Rights=1},
                new UserDTO{ Email="EmailB", Name="NameB", Phone=2, Rights=1}
            });
        }

        public void SetOrderState(UpdateRequest<OrderDTO> request)
        {
            db.inTransaction(uof =>
            {
                uof.RequestRollback = true;
                Order o = uof.Db.Orders.Get(request.Data.OrderID);
                State st = uof.Db.States.Find(request.Data.State.StateValue);
                o.State = st;
            });
        }

        private IngredientAssembler ingredientAssembler = new IngredientAssembler();

        public void UpdateIngredient(UpdateRequest<IList<StockIngredientDTO>> request)
        {
            if (request.Data == null)
                return;

            db.inTransaction(uof =>
            {
                uof.RequestRollback = true;
                foreach (var stockItem in request.Data)
                {
                    Ingredient ing = uof.Db.Ingredients.Get(stockItem.IngredientID);
                    ingredientAssembler.UpdateIngredient(ing, stockItem);
                }
            });
        }
    }
}
