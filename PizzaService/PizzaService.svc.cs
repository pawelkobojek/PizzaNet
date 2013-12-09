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

        private bool HasRights(UserDTO userDTO, int p)
        {
            return userDTO != null && userDTO.Rights >= p;
        }

        public ListResponse<PizzaNetCommon.DTOs.StockIngredientDTO> GetIngredients(EmptyRequest req)
        {
            try
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
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<StockIngredientDTO>> IPizzaService.GetRecipeTabData(EmptyRequest req)
        {
            try
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
                        .Select(ingAssembler.ToSimpleDto)
                        .ToList());
                });
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
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }


        public ListResponse<UserDTO> GetUsers(EmptyRequest req)
        {
            return db.inTransaction(uof =>
                {
                    if (!HasRights(GetUser(req).Data, 3))
                        return null;

                    return ListResponse.Create(uof.Db.Users.FindAll().ToList()
                        .Select(userAssembler.ToSimpleDto).ToList());
                });
        }

        public void SetOrderState(UpdateRequest<OrderDTO> request)
        {
            db.inTransaction(uof =>
                {
                    if (!HasRights(GetUser(request).Data, 2))
                        return;
                    Order o = uof.Db.Orders.Get(request.Data.OrderID);
                    State st = uof.Db.States.Find(request.Data.State.StateValue);
                    o.State = st;
                    uof.Db.Commit();
                });
        }

        public ListResponse<OrderDTO> GetOrders(EmptyRequest req)
        {
            return db.inTransaction(uof =>
                {
                    if (!HasRights(GetUser(req).Data, 2))
                        return null;

                    return ListResponse.Create(uof.Db.Orders.FindAll().ToList()
                        .Select(orderAssembler.ToSimpleDto).ToList());
                });
        }


        public ListResponse<StockIngredientDTO> UpdateIngredient(UpdateRequest<IList<StockIngredientDTO>> request)
        {
            if (request.Data == null)
                return null;

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

        public ListResponse<StockIngredientDTO> UpdateOrRemoveIngredient(UpdateOrRemoveRequest<IList<StockIngredientDTO>> request)
        {
            if (request.Data == null && request.DataToRemove == null)
                return null;

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

        private SingleItemResponse<UserDTO> GetUser(RequestBase req)
        {
            User user = db.Users.Find(req.Login);
            if (user == null || !PerformValidation(user, req))
                return null;

            return SingleItemResponse.Create(userAssembler.ToSimpleDto(user));
        }

        public SingleItemResponse<UserDTO> GetUser(EmptyRequest req)
        {
            return GetUser((RequestBase)req);
        }

        private static bool PerformValidation(User user, RequestBase req)
        {
            return (user.Email == req.Login && user.Password == req.Password);
        }

    }
}
