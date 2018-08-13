 using Soft.Model.Shared;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Soft.Model
{
    /// <summary>
    /// Broker
    /// A Broker must have a CustomerNo
    /// </summary>
    public class Broker : EntityBase
    {
        #region Properties
        [Required]
        [StringLength(20)]
        public string CustomerNo { get; set; }

        /// <summary>
        /// Broker full name
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// A Broker can be related to 0:n Customers
        /// </summary>
        public ICollection<Customer> Customers { get; set; }
        #endregion

        #region Constructor
        public Broker()
        {
            Customers = new Collection<Customer>();
        }
        #endregion
    }
}
