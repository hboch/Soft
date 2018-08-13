using Moq;
using Soft.Ui.Bc.Shared;
using Soft.Ui.ViewModel;
using Xunit;

namespace Soft.Ui.Tests.ViewModel
{
    public class ViewModelFactoryTests
    {
        //TODO: Hier sollte getestet werden ob Autofac.IIndexer das richtige ViewModel selektiert
        //Statt Autofac.Features.Indexed.IIndex<string, IViewModel>
        //liesse sich im Konstruktor von ViewModelFactory ein IIndex Interface verwenden.
        //Allerdings ist es in Bootstrapper.cs nicht gelungen dann 
        //Autofac.Features.Indexed.IIndex<string, IViewModel>
        //für das Interface IIndex<string, IViewModel> zu mit
        //builder.RegisterType<Autofac.Features.Indexed.IIndex<string,IViewModel>>().As<Soft.Ui.ViewModel.IIndex<string,IViewModel>>();
        //registrieren bzw. startet das Programm mit einer Fehlermeldung.

        //[Fact]
        //public void Create_When_Called_With_AboutViewModel_Returns_Type_AboutViewModel()
        //{
        //    // Arrange
        //    var iViewModelMock = new Mock<IViewModel>();
        //    var iViewModelMockObject = iViewModelMock.Object;
        
        //    var iIndexMock = new Mock<IIndex<string, IViewModel>>();
        
        //    iIndexMock.Setup(x => x[(nameof (AboutViewModel))]).Returns(iViewModelMock.Object);

        //    var something = new ViewModelFactory(iIndexMock.Object);

        //    //// Act
        //    var viewModel = something.Create(nameof(AboutViewModel));

        //    //Assert
        //    Assert.Equal(iViewModelMock.Object, viewModel);
        //}
    }    
}
