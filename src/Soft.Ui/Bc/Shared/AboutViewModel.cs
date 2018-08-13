using Prism.Events;
using Soft.Ui.ViewModel;
using Soft.Ui.ViewModel.Services;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Soft.Ui.Bc.Shared
{
    /// <summary>
    /// ViewModel for About View of the application
    /// </summary>
    /// <remarks>AboutViewModel behaves like a DetailViewModel with reduced functionality. There is no Delete- and Save-functionality.
    /// It is an example of a ViewModel for a view which is only displaying information.</remarks>
    public class AboutViewModel : DetailViewModelBase
    {
        //ToDo Tests
        public string ApplicationInfo
        {
            get
            {
                string version = typeof(AboutViewModel).GetTypeInfo().Assembly.GetName().Version.ToString();
                return "Information about the Soft application version " + version;
            }
        }
        public string Copyrightinfo
        {
            get
            {
                string copyrightinfo = typeof(AboutViewModel).GetTypeInfo().Assembly.GetName().Version.ToString();

                AssemblyCopyrightAttribute m_Copyright;
                var m_AsmInfo = Assembly.GetExecutingAssembly();
                m_Copyright = m_AsmInfo.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute;
                return m_Copyright.Copyright;
            }
        }

        public override string TabDisplayName => "About";

        public AboutViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
            : base(eventAggregator, messageDialogService)
        {
        }
        /// <summary>
        /// Load data for the About View
        /// </summary>
        /// <param name="id">Dummy Id value, as the About Viewmodel is based on DetailViewModel but has no related Entity which has to be loaded from a repository</param>
        /// <returns></returns>
        public override Task<bool> LoadAsync(int id)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// About View has no Delete functionality
        /// </summary>
        protected override void OnCommandDeleteExecute()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// About View has no Save functionality
        /// </summary>
        /// <returns></returns>
        protected override bool OnCommandSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// About View has no Save functionality
        /// </summary>
        protected override void OnCommandSaveExecute()
        {
            throw new NotImplementedException();
        }
    }
}
