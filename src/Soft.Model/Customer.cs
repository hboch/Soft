 using Soft.Model.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Soft.Model
{
    /// <summary>
    /// Customer. Must have a unique Customer Id, CustomerNo. and Name.
    /// </summary>
    public class Customer : EntityBase
    {
        #region Properties
        [Required]
        [StringLength(20)]
        public string CustomerNo { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        //[Timestamp]
        //public byte[] RowVersion { get; set; }

        //TODO DatePicker Konvertierungsfehler, wenn leer
        //TODO DatePicker MaxDate=Heute
        public DateTime CustomerSince { get; set; }

        /// <summary>
        /// Internetadresse des Customern
        /// </summary>
        [StringLength(50)]
        public string InternetAdress { get; set; }

        /// <summary>
        /// A Customer can be related to 0:1 Account Manager
        /// </summary>
        public int? AccountManagerId { get; set; }
        public AccountManager AccountManager { get; set; }

        /// <summary>
        /// A Customer can be related to 0:n BankAccounts
        /// </summary>
        public ICollection<BankAccount> BankAccounts { get; set; }

        /// <summary>
        /// A Customer can be related to 0:n CustomerBroker
        /// </summary>
        public ICollection<Broker> CustomerBrokers { get; set; }
        #endregion

        #region Constructor
        public Customer()
        {
            BankAccounts = new Collection<BankAccount>();
            CustomerBrokers = new Collection<Broker>();
        }
        #endregion
    }
}
