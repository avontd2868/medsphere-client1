using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Task.Scheduler;
using ClinSchd.Modules.Task.Scheduler.Controllers;
using ClinSchd.Modules.Task.Scheduler.Services;

namespace ClinSchd.Modules.Task.Scheduler
{
    public class TaskSchedulerModule : IModule
    {
        private readonly IUnityContainer container;

		public TaskSchedulerModule(IUnityContainer container)
        {
            this.container = container;
		}

        public void Initialize()
        {
			this.RegisterViewsAndServices();

			ITaskSchedulerController controller = this.container.Resolve<ITaskSchedulerController>();
			controller.Run();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<ITaskSchedulerController, TaskSchedulerController>();
        }
    }
}
