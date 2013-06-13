using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Task.Group;
using ClinSchd.Modules.Task.Controllers;
using ClinSchd.Modules.Task.Services;

namespace ClinSchd.Modules.Task
{
    public class TaskModule : IModule
    {
        private readonly IUnityContainer container;

        public TaskModule(IUnityContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

            ITaskController controller = this.container.Resolve<ITaskController>();
            controller.Run();
        }

        protected void RegisterViewsAndServices()
        {
            this.container.RegisterType<ITaskController, TaskController>(new ContainerControlledLifetimeManager());
			this.container.RegisterType<IGroupView, GroupView>();
			this.container.RegisterType<IGroupPresentationModel, GroupPresentationModel>();
            this.container.RegisterType<ITaskService, TaskService>(new ContainerControlledLifetimeManager());
        }
    }
}
