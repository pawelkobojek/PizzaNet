using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using PizzaNetCommon.DTOs;
using PizzaNetCommon.Requests;
using PizzaNetCommon.Services;
using PizzaNetDataModel.Model;
using PizzaNetDataModel.Repository;
using PizzaService.Assemblers;

namespace PizzaService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    public class PizzaService : ServiceBase, IPizzaService
    {
        private IngredientAssembler ingAssembler = new IngredientAssembler();
        private RecipeAssembler recipeAssembler = new RecipeAssembler();
        private SizeAssembler sizeAssembler = new SizeAssembler();
        private OrderAssembler orderAssembler = new OrderAssembler();
        private UserAssembler userAssembler = new UserAssembler();
        private StateAssembler stateAssembler = new StateAssembler();

        private PizzaUnitOfWork db = new PizzaUnitOfWork();
        private const string NOT_ENOUGH_INGS_MSG = "Not enough Ingredients in stock!";

        private bool HasRights(UserDTO userDTO, int p)
        {
            return userDTO != null && userDTO.Rights >= p;
        }

        public ListResponse<PizzaNetCommon.DTOs.StockIngredientDTO> GetIngredients(EmptyRequest req)
        {
            try
            {
                using (var db = new PizzaUnitOfWork())
                {

                    return db.inTransaction(uof =>
                        {
                            if (!HasRights(GetUser(req).Data, 2))
                                return null;

                            return ListResponse.Create(uof.Db.Ingredients.FindAll().ToList()
                                .Select(ingAssembler.ToSimpleDto)
                                .ToList());
                        });
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        public TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<OrderIngredientDTO>> GetRecipeTabData(EmptyRequest req)
        {
            try
            {
                using (var db = new PizzaUnitOfWork())
                {

                    return db.inTransaction(uof =>
                    {
                        if (!HasRights(GetUser(req).Data, 1))
                            return null;

                        return TrioResponse.Create(uof.Db.Recipies.FindAll().ToList()
                            .Select(recipeAssembler.ToSimpleDto)
                            .ToList(), uof.Db.Sizes.FindAll().ToList()
                            .Select(sizeAssembler.ToSimpleDto)
                            .ToList(), uof.Db.Ingredients.FindAll().ToList()
                            .Select(ingAssembler.ToOrderIngredientDto)
                            .ToList());
                    });
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        public ListResponse<OrderDTO> GetUndoneOrders(EmptyRequest req)
        {
            try
            {
                using (var db = new PizzaUnitOfWork())
                {
                    return db.inTransaction(uof =>
                    {
                        if (GetUser(req).Data == null)
                            return null;

                        return ListResponse.Create(db.Orders.FindAllEagerlyWhere(o => o.State.StateValue == State.NEW ||
                            o.State.StateValue == State.IN_REALISATION)
                            .ToList()
                            .Select(orderAssembler.ToSimpleDto)
                            .ToList());
                    });
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }


        public ListResponse<UserDTO> GetUsers(EmptyRequest req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uof =>
                    {
                        if (!HasRights(GetUser(req).Data, 3))
                            return null;

                        return ListResponse.Create(uof.Db.Users.FindAll().ToList()
                            .Select(userAssembler.ToSimpleDto).ToList());
                    });
            }
        }

        public void SetOrderState(UpdateRequest<OrderDTO> request)
        {
            using (var db = new PizzaUnitOfWork())
            {
                db.inTransaction(uof =>
                    {
                        if (!HasRights(GetUser(request).Data, 2))
                            return;
                        Order o = uof.Db.Orders.Get(request.Data.OrderID);
                        State st = uof.Db.States.Find(request.Data.State.StateValue);
                        if (request.Data.State.StateValue == State.IN_REALISATION)
                        {
                            foreach (var orderDet in request.Data.OrderDetailsDTO)
                            {
                                foreach (var ingr in orderDet.Ingredients)
                                {
                                    Ingredient ing = uof.Db.Ingredients.Get(ingr.IngredientID);
                                    if (ing == null) return;
                                    if (ing.StockQuantity - ingr.Quantity < 0)
                                    {
                                        throw new FaultException(NOT_ENOUGH_INGS_MSG);
                                    }
                                    ing.StockQuantity -= ingr.Quantity;
                                }
                            }
                        }
                        o.State = st;
                        uof.Db.Commit();
                    });
            }
        }

        public ListResponse<OrderDTO> GetOrders(EmptyRequest req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uof =>
                    {
                        if (!HasRights(GetUser(req).Data, 2))
                            return null;

                        return ListResponse.Create(uof.Db.Orders.FindAll().ToList()
                            .Select(orderAssembler.ToSimpleDto).ToList());
                    });
            }
        }

        public ListResponse<StockIngredientDTO> UpdateIngredient(UpdateRequest<List<StockIngredientDTO>> request)
        {
            if (request.Data == null)
                return null;

            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uof =>
                    {
                        if (!HasRights(GetUser(request).Data, 2))
                            return null;

                        foreach (var stockItem in request.Data)
                        {
                            Ingredient ing = uof.Db.Ingredients.Get(stockItem.IngredientID);
                            if (ing != null)
                                ingAssembler.UpdateIngredient(ing, stockItem);
                            else uof.Db.Ingredients.Insert(ingAssembler.ToEntityWithEmptyRecipies(stockItem));
                        }
                        uof.Db.Commit();
                        return ListResponse.Create(uof.Db.Ingredients.FindAll().ToList()
                            .Select(ingAssembler.ToSimpleDto)
                            .ToList());
                    });
            }
        }

        public ListResponse<StockIngredientDTO> UpdateOrRemoveIngredient(UpdateOrRemoveRequest<List<StockIngredientDTO>> request)
        {
            if (request.Data == null && request.DataToRemove == null)
                return null;
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uof =>
                {
                    if (!HasRights(GetUser(request).Data, 2))
                        return null;

                    if (request.Data != null)
                    {
                        foreach (var stockItem in request.Data)
                        {
                            Ingredient ing = uof.Db.Ingredients.Get(stockItem.IngredientID);
                            if (ing != null)
                                ingAssembler.UpdateIngredient(ing, stockItem);
                            else uof.Db.Ingredients.Insert(ingAssembler.ToEntityWithEmptyRecipies(stockItem));
                        }
                    }
                    if (request.DataToRemove != null)
                    {
                        foreach (var stockItem in request.DataToRemove)
                        {
                            Ingredient ing = uof.Db.Ingredients.Get(stockItem.IngredientID);
                            if (ing != null)
                                uof.Db.Ingredients.Delete(ing);
                        }
                    }
                    uof.Db.Commit();
                    return ListResponse.Create(uof.Db.Ingredients.FindAll().ToList()
                        .Select(ingAssembler.ToSimpleDto)
                        .ToList());
                });
            }
        }

        private SingleItemResponse<UserDTO> GetUser(RequestBase req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uow =>
                    {
                        User user = uow.Db.Users.Find(req.Login);
                        if (user == null || !PerformValidation(user, req))
                            return null;

                        return SingleItemResponse.Create(userAssembler.ToSimpleDto(user));
                    });
            }
        }

        public SingleItemResponse<UserDTO> GetUser(EmptyRequest req)
        {
            return GetUser((RequestBase)req);
        }

        private static bool PerformValidation(User user, RequestBase req)
        {
            return (user.Email == req.Login && user.Password == req.Password);
        }

        public ListResponse<OrderDTO> GetOrdersForUser(EmptyRequest req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uow =>
                    {
                        if (!HasRights(GetUser(req).Data, 1))
                            return null;

                        return ListResponse.Create(uow.Db.Orders.FindAllEagerlyWhere(o => o.User.Email == req.Login)
                            .ToList().Select(orderAssembler.ToSimpleDto).ToList());
                    });
            }
        }

        public ListResponse<OrderSuppliesDTO> OrderSupplies(UpdateRequest<List<OrderSuppliesDTO>> request)
        {
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uow =>
                {
                    if (!HasRights(GetUser(request).Data, 2))
                        return null;

                    foreach (var os in request.Data)
                    {
                        Ingredient ing = uow.Db.Ingredients.Get(os.IngredientID);
                        if (ing == null) return null;
                        ing.StockQuantity += os.OrderValue;
                    }

                    uow.Db.Commit();

                    return ListResponse.Create(uow.Db.Ingredients.FindAll().ToList().Select(ingAssembler.ToOrderSuppliesDTO).ToList());
                });
            }
        }


        public void RemoveOrder(UpdateOrRemoveRequest<OrderDTO> request)
        {
            using (var db = new PizzaUnitOfWork())
            {
                db.inTransaction(uow =>
                    {
                        if (!HasRights(GetUser(request).Data, 2))
                            return;

                        Order o = db.Orders.Get(request.DataToRemove.OrderID);
                        db.Orders.Delete(o);

                        uow.Db.Commit();
                    });
            }
        }


        public void MakeOrder(UpdateRequest<OrderDTO> req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                db.inTransaction(uow =>
                    {
                        UserDTO userDto = GetUser(req).Data;
                        if (!HasRights(userDto, 1))
                            return;

                        User user = uow.Db.Users.Get(userDto.UserID);
                        OrderDTO o = req.Data;
                        State st = uow.Db.States.Find(State.NEW);


                        List<OrderDetail> od = new List<OrderDetail>();
                        foreach (var ordDto in o.OrderDetailsDTO)
                        {
                            List<OrderIngredient> oIngs = new List<OrderIngredient>();
                            foreach (var oIng in ordDto.Ingredients)
                            {
                                oIngs.Add(new OrderIngredient
                                {
                                    Ingredient = uow.Db.Ingredients.Get(oIng.IngredientID),
                                    Quantity = oIng.Quantity
                                });
                            }
                            od.Add(new OrderDetail
                            {
                                Size = uow.Db.Sizes.Get(ordDto.Size.SizeID),
                                Ingredients = oIngs
                            });
                        }
                        Order order = new Order
                        {
                            Address = o.Address,
                            CustomerPhone = o.CustomerPhone,
                            Date = o.Date,
                            OrderID = o.OrderID,
                            User = user,
                            State = st,
                            UserID = user.UserID,
                            OrderDetails = od
                        };

                        uow.Db.Orders.Insert(order);
                        uow.Db.Commit();
                    });
            }
        }

        //private List<OrderDetail> mergeIngredients(List<OrderDetailDTO> det, IEnumerable<OrderIngredientDTO> ing)
        //{
        //    foreach (var d in det)
        //        foreach (var i in d.Ingredients)
        //        {
        //            Ingredient s = ing.FirstOrDefault((e) => { return e.IngredientID == i.Ingredient.IngredientID; });
        //            if (s == null) throw new Exception("Inconsistien data");
        //            i.Ingredient = s;
        //        }
        //    return det;
        //}
        //private List<OrderDetail> mergeSizes(List<OrderDetailDTO> det, IEnumerable<SizeDTO> sizes)
        //{
        //    foreach (var d in det)
        //    {
        //        d.Size = sizes.FirstOrDefault((e) => { return e.SizeValue == d.Size.SizeValue; });
        //        if (d.Size == null) throw new Exception("Inconsistien data");
        //    }
        //    return det;
        //}


        public void InsertRecipe(UpdateRequest<RecipeDTO> req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                db.inTransaction(uow =>
                    {
                        Recipe r = new Recipe { Name = req.Data.Name };
                        List<Ingredient> ings = new List<Ingredient>();

                        foreach (var ingDto in req.Data.Ingredients)
                        {
                            ings.Add(db.Ingredients.Get(ingDto.IngredientID));
                        }
                        r.Ingredients = ings;
                        db.Recipies.Insert(r);
                        uow.Db.Commit();
                    });
            }
        }


        public TrioResponse<List<RecipeDTO>, List<OrderIngredientDTO>, int> UpdateOrRmoveRecipe(UpdateOrRemoveRequest<List<RecipeDTO>> request)
        {
            if (request.Data == null && request.DataToRemove == null)
                return null;

            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uof =>
                {
                    if (!HasRights(GetUser(request).Data, 2))
                        return null;

                    if (request.Data != null)
                    {
                        foreach (var recipe in request.Data)
                        {
                            Recipe rec = uof.Db.Recipies.Get(recipe.RecipeID);
                            if (rec != null)
                            {

                                rec.Name = recipe.Name;
                                int j;
                                for (int i = 0; i < rec.Ingredients.Count; i++)
                                {
                                    Ingredient ingredient = rec.Ingredients.ElementAt(i);
                                    for (j = 0; j < recipe.Ingredients.Count; j++)
                                    {
                                        if (ingredient.IngredientID == recipe.Ingredients[j].IngredientID)
                                            break;
                                    }
                                    if (j == recipe.Ingredients.Count)
                                    {
                                        rec.Ingredients.Remove(ingredient);
                                    }
                                }

                                foreach (var ing in recipe.Ingredients)
                                {
                                    Ingredient ingredient = uof.Db.Ingredients.Get(ing.IngredientID);
                                    if (ingredient == null)
                                    {
                                        db.RequestRollback = true;
                                        return null;
                                    }

                                    rec.Ingredients.Add(ingredient);
                                }
                                uof.Db.Recipies.Update(rec);
                            }
                            else
                            {
                                rec = new Recipe { Name = recipe.Name };
                                rec.Ingredients = new List<Ingredient>();
                                foreach (var ing in recipe.Ingredients)
                                {
                                    Ingredient ingredient = uof.Db.Ingredients.Get(ing.IngredientID);
                                    if (ingredient == null)
                                    {
                                        db.RequestRollback = true;
                                        return null;
                                    }

                                    rec.Ingredients.Add(ingredient);
                                }

                                uof.Db.Recipies.Insert(rec);
                            }
                        }
                    }
                    if (request.DataToRemove != null)
                    {
                        foreach (var recipe in request.DataToRemove)
                        {
                            Recipe rec = uof.Db.Recipies.Get(recipe.RecipeID);
                            if (rec != null)
                                uof.Db.Recipies.Delete(rec);
                        }
                    }

                    uof.Db.Commit();

                    return TrioResponse.Create(uof.Db.Recipies.FindAll().ToList().Select(recipeAssembler.ToSimpleDto).ToList(),
                        uof.Db.Ingredients.FindAll().Select(ingAssembler.ToOrderIngredientDto).ToList(),
                        0);
                });
            }
        }


        public ListResponse<UserDTO> UpdateOrRemoveUser(UpdateOrRemoveRequest<List<UserDTO>> request)
        {
            if (request.Data == null && request.DataToRemove == null)
                return null;

            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uow =>
                    {
                        if (!HasRights(GetUser(request).Data, 3))
                            return null;


                        if (request.Data != null)
                        {
                            foreach (var userDto in request.Data)
                            {
                                User user = uow.Db.Users.Get(userDto.UserID);

                                if (user != null)
                                {
                                    userAssembler.UpdateEntity(user, userDto);
                                    uow.Db.Users.Update(user);
                                }
                                else
                                {
                                    user = new User
                                    {
                                        Address = userDto.Address,
                                        Email = userDto.Email,
                                        Name = userDto.Name,
                                        Password = userDto.Password,
                                        Phone = userDto.Phone,
                                        Rights = userDto.Rights
                                    };
                                    uow.Db.Users.Insert(user);
                                }
                            }
                        }

                        if (request.DataToRemove != null)
                        {
                            foreach (var userDto in request.DataToRemove)
                            {
                                User user = uow.Db.Users.Get(userDto.UserID);

                                if (user != null)
                                    uow.Db.Users.Delete(user);
                            }
                        }
                        db.RequestRollback = false;
                        uow.Db.Commit();
                        return ListResponse.Create(uow.Db.Users.FindAll().ToList().Select(userAssembler.ToSimpleDto)
                            .ToList());
                    });
            }
        }

        public SingleItemResponse<UserDTO> RegisterUser(UpdateRequest<RegisterUserDTO> req)
        {
            var user = req.Data;
            if (user == null)
                return null;

            if (user.Password.Length == 0)
            {
                throw PizzaServiceFault.Create(PizzaServiceFault.PASSWORD_EMPTY);
            }
            if (user.Email.Length == 0)
            {
                //TODO email format check
                throw PizzaServiceFault.Create(PizzaServiceFault.EMAIL_EMPTY);
            }
            if (user.Address.Length == 0)
            {
                throw PizzaServiceFault.Create(PizzaServiceFault.ADDRESS_EMPTY);
            }

            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uow =>
                    {
                        if (uow.Db.Users.Find(user.Email) != null)
                            throw PizzaServiceFault.Create(String.Format(PizzaServiceFault.EMAIL_ALREADY_REGISTERED_FORMAT, user.Email));
                        var ins = userAssembler.ToUser(user);
                        ins.UserID = -1;
                        uow.Db.Users.Insert(ins);
                        uow.Db.Commit();
                        return SingleItemResponse.Create(userAssembler.ToSimpleDto(uow.Db.Users.Find(ins.Email)));
                    });
            }
        }
    }
}
