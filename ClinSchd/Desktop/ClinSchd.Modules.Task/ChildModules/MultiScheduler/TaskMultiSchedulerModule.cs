using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Task.MultiScheduler;
using ClinSchd.Modules.Task.MultiScheduler.Controllers;
using ClinSchd.Modules.Task.MultiScheduler.Services;

namespace ClinSchd.Modules.Task.MultiScheduler
{
    public class TaskMultiSchedulerModule : IModule
    {
        private readonly IUnityContainer container;

		public TaskMultiSchedulerModule(IUnityContainer container)
        {
            this.container = container;
		}

        public void Initialize()
        {
			this.RegisterViewsAndServices();

			ITaskMultiSchedulerController controller = this.container.Resolve<ITaskMultiSchedulerController>();
			controller.Run();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<ITaskMultiSchedulerController, TaskMultiSchedulerController>();
			this.container.RegisterType<IMultiSchedulerView, MultiSchedulerView> ();
			this.container.RegisterType<IMultiSchedulerPresentationModel, MultiSchedulerPresentationModel> ();
        }
    }
}
