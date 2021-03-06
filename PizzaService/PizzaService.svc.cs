﻿using System;
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
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using PizzaNetCommon.Queries;

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
        private ComplaintAssembler complaintAssembler = new ComplaintAssembler();

        private PizzaUnitOfWork db = new PizzaUnitOfWork();

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
                                throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

                            return ListResponse.Create(uof.Db.Ingredients.FindAll().ToList()
                                .Select(ingAssembler.ToSimpleDto)
                                .ToList());
                        });
                }
            }
            catch (FaultException<PizzaServiceFault> e)
            {
                throw e;
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
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

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
            catch (FaultException<PizzaServiceFault> e)
            {
                throw e;
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
                        if (!HasRights(GetUser(req).Data, 2))
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

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
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

                        return ListResponse.Create(uof.Db.Users.FindAll().ToList()
                            .Select(userAssembler.ToSimpleDto).ToList());
                    });
            }
        }

        public SingleItemResponse<OrderDTO> SetOrderState(UpdateRequest<OrderDTO> request)
        {
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uof =>
                    {
                        if (!HasRights(GetUser(request).Data, 2))
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);
                        Order o = uof.Db.Orders.Get(request.Data.OrderID);
                        State st = uof.Db.States.Find(request.Data.State.StateValue);
                        if (request.Data.State.StateValue == State.IN_REALISATION)
                        {
                            foreach (var orderDet in request.Data.OrderDetailsDTO)
                            {
                                foreach (var ingr in orderDet.Ingredients)
                                {
                                    Ingredient ing = uof.Db.Ingredients.Get(ingr.IngredientID);
                                    if (ing == null) //this should never happen (when removing ingredient it is removed from all orders)
                                        throw PizzaServiceFault.Create(Messages.SERVER_INTERNAL_ERROR);
                                    if (ing.StockQuantity - ingr.Quantity < 0)
                                    {
                                        throw PizzaServiceFault.Create(Messages.NOT_ENOUGH_INGS_MSG);
                                    }
                                    ing.StockQuantity -= ingr.Quantity;
                                }
                            }
                        }
                        o.State = st;
                        uof.Db.Commit();
                        return SingleItemResponse.Create(orderAssembler.ToSimpleDto(uof.Db.Orders.Get(o.OrderID)));
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
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

                        return ListResponse.Create(uof.Db.Orders.FindAll().ToList()
                            .Select(orderAssembler.ToSimpleDto).ToList());
                    });
            }
        }

        public ListResponse<StockIngredientDTO> UpdateIngredient(UpdateRequest<List<StockIngredientDTO>> request)
        {
            if (request.Data == null)
                throw PizzaServiceFault.Create(Messages.NO_DATA);

            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uof =>
                    {
                        if (!HasRights(GetUser(request).Data, 2))
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

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
                throw PizzaServiceFault.Create(Messages.NO_DATA);
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uof =>
                {
                    if (!HasRights(GetUser(request).Data, 2))
                        throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

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
                        if (user == null)
                            throw PizzaServiceFault.Create(Messages.INVALID_USER_OR_PASSWORD);
                        if (!PerformValidation(user, req))
                            throw PizzaServiceFault.Create(Messages.INVALID_USER_OR_PASSWORD);

                        var res = SingleItemResponse.Create(userAssembler.ToSimpleDto(user));
                        res.Data.Email = req.Login;
                        res.Data.Password = req.Password;
                        return res;
                    });
            }
        }

        public SingleItemResponse<UserDTO> GetUser(EmptyRequest req)
        {
            return GetUser((RequestBase)req);
        }

        private static bool PerformValidation(User user, RequestBase req)
        {
            return (user.Email == req.Login && user.Password == UserAssembler.GetHashedPassword(req.Password));
        }

        public ListResponse<OrderDTO> GetOrdersForUser(EmptyRequest req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uow =>
                    {
                        if (!HasRights(GetUser(req).Data, 1))
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

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
                        throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

                    foreach (var os in request.Data)
                    {
                        Ingredient ing = uow.Db.Ingredients.Get(os.IngredientID);
                        if (ing == null)
                            throw PizzaServiceFault.Create(Messages.INGS_LIST_OUT_OF_DATE);
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
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

                        Order o = db.Orders.Get(request.DataToRemove.OrderID);
                        if (o != null)
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
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

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
                    if (!HasRights(GetUser(req).Data, 2))
                        throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

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

        public TrioResponse<List<RecipeDTO>, List<OrderIngredientDTO>, int> UpdateOrRemoveRecipe(UpdateOrRemoveRequest<List<RecipeDTO>> request)
        {
            if (request.Data == null && request.DataToRemove == null)
                throw PizzaServiceFault.Create(Messages.NO_DATA);

            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uof =>
                {
                    if (!HasRights(GetUser(request).Data, 2))
                        throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

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
                                        throw PizzaServiceFault.Create(Messages.INGS_LIST_OUT_OF_DATE);
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
                                        throw PizzaServiceFault.Create(Messages.INGS_LIST_OUT_OF_DATE);
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
                throw PizzaServiceFault.Create(Messages.NO_DATA);

            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uow =>
                    {
                        if (!HasRights(GetUser(request).Data, 3))
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

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
                                    user = userAssembler.ToUser(userDto);
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
                        uow.Db.Commit();
                        return ListResponse.Create(uow.Db.Users.FindAll().ToList().Select(userAssembler.ToSimpleDto)
                            .ToList());
                    });
            }
        }

        public SingleItemResponse<UserDTO> RegisterUser(UpdateRequest<UserDTO> req)
        {
            if (req == null || req.Data == null)
                throw PizzaServiceFault.Create(Messages.NO_DATA);

            var user = req.Data;

            if (user.Password.Length == 0)
            {
                throw PizzaServiceFault.Create(PizzaServiceFault.PASSWORD_EMPTY);
            }
            if (!IsEmailValid(user.Email))
            {
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
                        var res = SingleItemResponse.Create(userAssembler.ToSimpleDto(uow.Db.Users.Find(ins.Email)));
                        res.Data.Password = req.Password;
                        return res;
                    });
            }
        }

        public static bool IsEmailValid(string email)
        {
            const string theEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            return Regex.IsMatch(email, theEmailPattern);
        }

        public SingleItemResponse<UserDTO> UpdateUser(UpdateRequest<UserDTO> request)
        {
            if (request.Data == null)
                throw PizzaServiceFault.Create(Messages.NO_DATA);

            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uow =>
                    {
                        User user = uow.Db.Users.Find(request.Login);
                        if (user == null)
                            throw PizzaServiceFault.Create(String.Format(Messages.USER_NOT_EXISTS_FORMAT, request.Login));
                        if (user.UserID != request.Data.UserID || !PerformValidation(user, request))
                            throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

                        userAssembler.UpdateEntityUserLevel(user, request.Data);
                        uow.Db.Commit();
                        var res = SingleItemResponse.Create(userAssembler.ToSimpleDto(uow.Db.Users.Get(request.Data.UserID)));
                        res.Data.Password = request.Data.Password;
                        return res;
                    });
            }
        }
        public static class Messages
        {
            public const string NO_DATA = "No data to proceed has been sent!";
            public const string NO_PERMISSIONS = "Operation not permitted!";
            public const string USER_NOT_EXISTS_FORMAT = "User {0} not exists!";
            public const string NOT_ENOUGH_INGS_MSG = "Not enough Ingredients in stock!";
            public const string INGS_LIST_OUT_OF_DATE = "Ingredients list is out of date!";
            public const string INVALID_USER_OR_PASSWORD = "Invalid user or password!";
            public const string SERVER_INTERNAL_ERROR = "Server internal error!";
        }

        private static string GetHashedPassword(string password)
        {
            SHA256 hash = SHA256Managed.Create();
            byte[] pswd = Encoding.UTF8.GetBytes(password);
            byte[] hashed = hash.ComputeHash(pswd);
            string hashedPassword = Encoding.UTF8.GetString(hashed);
            return hashedPassword;
        }


        public ListResponse<OrderIngredientDTO> QueryIngredients(QueryRequest<IngredientsQuery> request)
        {
            if (request.Query.IngredientIds == null)
                throw PizzaServiceFault.Create(Messages.NO_DATA);

            return db.inTransaction(uow =>
                {
                    return ListResponse.Create(uow.Db.Ingredients.Find(request.Query.IngredientIds).ToList()
                        .Select(ingAssembler.ToOrderIngredientDto).ToList());
                });
        }


        public void MakeOrderFromWeb(UpdateOrRemoveRequest<List<OrderInfoDTO>> req)
        {
            if (req.Data == null)
                throw PizzaServiceFault.Create(Messages.NO_DATA);

            db.inTransaction(uow =>
                {
                    UserDTO userDto = GetUser(req).Data;
                    if (!HasRights(userDto, 1))
                        throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);


                    User user = uow.Db.Users.Get(userDto.UserID);

                    List<OrderInfoDTO> data = req.Data;
                    List<OrderDetail> od = new List<OrderDetail>();

                    foreach (var item in data)
                    {
                        var sizeVal = Size.SMALL;
                        switch (item.Size)
                        {
                            case "normal":
                                sizeVal = Size.MEDIUM;
                                break;
                            case "big":
                                sizeVal = Size.GREAT;
                                break;
                            default:
                                break;
                        }
                        var size = uow.Db.Sizes.Find(sizeVal);
                        var ings = (List<Ingredient>)uow.Db.Ingredients.Find(item.Ingredients);
                        List<OrderIngredient> oings = new List<OrderIngredient>();

                        for (int i = 0; i < item.Quantities.Length; i++)
                        {
                            int quant = (item.Quantities[i] == "normal") ? ings[i].NormalWeight : ings[i].ExtraWeight;
                            oings.Add(new OrderIngredient
                            {
                                Ingredient = ings[i],
                                Quantity = quant
                            });
                        }

                        od.Add(new OrderDetail
                        {
                            Ingredients = oings,
                            Size = size
                        });
                    }

                    Order o = new Order
                    {
                        Address = userDto.Address,
                        CustomerPhone = userDto.Phone,
                        Date = DateTime.Now,
                        OrderDetails = od,
                        State = uow.Db.States.Find(State.NEW),
                        User = user
                    };

                    uow.Db.Orders.Insert(o);
                    uow.Db.Commit();

                    /*State st = uow.Db.States.Find(State.NEW);

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
                        User = user,
                        State = st,
                        UserID = user.UserID,
                        OrderDetails = od
                    };

                    uow.Db.Orders.Insert(order);
                    uow.Db.Commit();*/
                });
        }


        public ListResponse<OrderDTO> GetOrderInfo(OrdersQuery req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                if (!HasRights(GetUser(req).Data, 1))
                    throw PizzaServiceFault.Create(Messages.NO_PERMISSIONS);

                return db.inTransaction(uow =>
                    {
                        return ListResponse.Create(uow.Db.Orders.FindAllEagerlyWhere(o => req.Ids.Contains(o.OrderID))
                            .ToList().Select(orderAssembler.ToSimpleDto)
                            .ToList());
                    });
            }
        }


        public void CreateComplaint(UpdateRequest<ComplaintDTO> req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                db.inTransaction(uow =>
                    {
                        db.Complaints.Insert(complaintAssembler.ToComplaint(req.Data));
                    });
            }
        }

        public ListResponse<ComplaintDTO> GetComplaints(EmptyRequest req)
        {
            using (var db = new PizzaUnitOfWork())
            {
                return db.inTransaction(uow =>
                    {
                        return ListResponse.Create(uow.Db.Complaints.FindAll()
                            .Select(complaintAssembler.ToSimpleDto)
                            .ToList());
                    });
            }
        }
    }
}
