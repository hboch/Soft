using System.ComponentModel.DataAnnotations;
using Soft.Model.Shared;

namespace Soft.Model
{
    /// <summary>
    /// Account Manager of Customer
    /// </summary>
    /// <remarks>A Customer can have 0:1 Account Managers</remarks>
    public class AccountManager : EntityBase
    {
        /// <summary>
        /// Fullname of Account Manager
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}