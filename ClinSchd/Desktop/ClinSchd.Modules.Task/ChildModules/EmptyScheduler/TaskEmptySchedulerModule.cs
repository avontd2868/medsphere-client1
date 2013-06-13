using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Task.EmptyScheduler;
using ClinSchd.Modules.Task.EmptyScheduler.Controllers;
using ClinSchd.Modules.Task.EmptyScheduler.Services;

namespace ClinSchd.Modules.Task.EmptyScheduler
{
    public class TaskEmptySchedulerModule : IModule
    {
        private readonly IUnityContainer container;

		public TaskEmptySchedulerModule(IUnityContainer container)
        {
            this.container = container;
		}

        public void Initialize()
        {
			this.RegisterViewsAndServices();

			ITaskEmptySchedulerController controller = this.container.Resolve<ITaskEmptySchedulerController>();
			controller.Run();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<ITaskEmptySchedulerController, TaskEmptySchedulerController>();
        }
    }
}
