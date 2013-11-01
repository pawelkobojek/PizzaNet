﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel.Repository
{
    public interface IIngredientRepository : IRepository<Ingredient, int>
    {
        IEnumerable<Ingredient> FindAll();
        IEnumerable<Ingredient> Find(int id);
        Ingredient Get(string name);
    }

    public class IngredientRepository : IIngredientRepository
    {
        private readonly PizzaContext db;

        public IngredientRepository(PizzaContext ctx)
        {
            this.db = ctx;
        }

        public IEnumerable<Model.Ingredient> FindAll()
        {
            return db.Ingredients.ToList();
        }

        public IEnumerable<Model.Ingredient> Find(int id)
        {
            return db.Ingredients.Where(ing => ing.IngredientID == id);
        }

        public Model.Ingredient Get(int id)
        {
            return db.Ingredients.Find(id);
        }

        public void Update(Model.Ingredient entity, Ingredient newEntity)
        {
            var ing = db.Ingredients.Find(entity.IngredientID);
            ing.Name = newEntity.Name;
            ing.NormalWeight = newEntity.NormalWeight;
            ing.ExtraWeight = newEntity.ExtraWeight;
            ing.PricePerUnit = newEntity.PricePerUnit;
            //ing.Recipies = newEntity.Recipies;
            ing.StockQuantity = newEntity.StockQuantity;
            //db.Ingredients.Attach(ing);
        }

        public void Delete(Model.Ingredient entity)
        {
            db.Ingredients.Attach(entity);
            db.Ingredients.Remove(entity);
        }

        public void Insert(Ingredient entity)
        {
            db.Ingredients.Add(entity);
        }


        public Ingredient Get(string name)
        {
            return db.Ingredients.Where(ing => ing.Name == name).First();
        }
    }
}
