using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Task.Availability;
using ClinSchd.Modules.Task.Availability.Controllers;
using ClinSchd.Modules.Task.Availability.Services;

namespace ClinSchd.Modules.Task.Availability
{
    public class TaskAvailabilityModule : IModule
    {
        private readonly IUnityContainer container;

		public TaskAvailabilityModule (IUnityContainer container)
        {
            this.container = container;
		}

        public void Initialize()
        {
			this.RegisterViewsAndServices();

			ITaskAvailabilityController controller = this.container.Resolve<ITaskAvailabilityController> ();
			controller.Run();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<ITaskAvailabilityController, TaskAvailabilityController> ();
        }
    }
}
