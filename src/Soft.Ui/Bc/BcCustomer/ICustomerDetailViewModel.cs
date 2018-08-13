using Soft.Ui.ViewModel;

namespace Soft.Ui.Bc.BcCustomer
{
    /// <summary>
    /// DetailViewModel Interface for a Customer Entity
    /// </summary>
    /// <remarks>Extends IDetailViewModel by an Entity specific Wrapper property</remarks>
    public interface ICustomerDetailViewModel : IDetailViewModel
    {
        CustomerWrapper Customer { get;  }
    }
}