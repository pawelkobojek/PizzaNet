using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetDataModel.Model;

namespace PizzaNetDataModel
{
    public class PizzaContext : DbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderIngredient> OrderIngredients { get; set; }
        public DbSet<Recipe> Recipies { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<User> Users { get; set; }

        public PizzaContext()
        {
            Database.SetInitializer<PizzaContext>(new PizzaContextInitializer());
            Database.Initialize(false);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>().Property(o => o.PricePerUnit).HasPrecision(12, 10);
            base.OnModelCreating(modelBuilder);
        }

        public PizzaContext(IDatabaseInitializer<PizzaContext> db)
        {
            Database.SetInitializer(db);
            Database.Initialize(false);
        }
    }
}
