using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetDataModel.Model
{
    /// <summary>
    /// Class representing Order.
    /// </summary>
    public class Order : Entity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int OrderID { get; set; }
        /// <summary>
        /// Date of order submition.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Optional phone number of ordering customer.
        /// </summary>
        public int CustomerPhone { get; set; }
        /// <summary>
        /// Address for which order is going to be provided.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Foreign key of related state.
        /// </summary>
        [ForeignKey("State")]
        public int StateID { get; set; }
        /// <summary>
        /// State of the order. It can be either "New", "In realisation" or "Done"
        /// </summary>
        public virtual State State { get; set; }

        /// <summary>
        /// List of ordered pizzas. Contains informations about its ingredients and size.
        /// </summary>
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
