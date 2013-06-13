using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.DataAccess.DataAccess;
using ClinSchd.Modules.DataAccess.Controllers;
using ClinSchd.Modules.DataAccess.Services;

namespace ClinSchd.Modules.DataAccess
{
    public class DataAccessModule : IModule
    {
        private readonly IUnityContainer container;

        public DataAccessModule(IUnityContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

            IDataAccessController controller = this.container.Resolve<IDataAccessController>();
            controller.Run();
        }

        protected void RegisterViewsAndServices()
        {
            this.container.RegisterType<IDataAccessController, DataAccessController>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<IDataAccessView, DataAccessView>();
            this.container.RegisterType<IDataAccessPresentationModel, DataAccessPresentationModel>();
			//this.container.RegisterInstance<IDataAccessService> (new DataAccessService (this.container));
            //this.container.RegisterType<IDataAccessService, DataAccessService>(new ContainerControlledLifetimeManager());
        }
    }
}
