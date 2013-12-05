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
using PizzaNetDataModel.Repository;
using PizzaService.Assemblers;

namespace PizzaService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class PizzaService : ServiceBase, IPizzaService
    {
        private IngredientAssembler ingAssembler = new IngredientAssembler();
        private RecipeAssembler recipeAssembler = new RecipeAssembler();
        private SizeAssembler sizeAssembler = new SizeAssembler();

        private PizzaUnitOfWork db = new PizzaUnitOfWork();

        public ListResponse<PizzaNetCommon.DTOs.IngredientDTO> GetIngredients()
        {
            try
            {
                return db.inTransaction(uof =>
                    {
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

        TrioResponse<List<RecipeDTO>, List<SizeDTO>, List<IngredientDTO>> IPizzaService.GetRecipeTabData()
        {
            try
            {
                return db.inTransaction(uof =>
                {
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
    }
}
