 using Soft.Model.Shared;
using System.ComponentModel.DataAnnotations;

namespace Soft.Model
{
    /// <summary>
    /// Bank Account of a Customer
    /// </summary>
    public class BankAccount : EntityBase
    {
        [Required]
        [StringLength(50)]
        public string AccountNo { get; set; }

        //Foreign Key to Customer
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        /// <summary>
        /// Is account a none Euro account
        /// </summary>
        public bool IsForeignAccount { get; set; }

        /// <summary>
        /// Account Notes
        /// </summary>
        public string Notes { get; set; }
    }
}