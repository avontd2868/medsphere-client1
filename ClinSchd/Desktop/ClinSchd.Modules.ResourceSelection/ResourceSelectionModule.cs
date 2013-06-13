using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.ResourceSelection.ResourceSelection;
using ClinSchd.Modules.ResourceSelection.Controllers;
using ClinSchd.Modules.ResourceSelection.Services;

namespace ClinSchd.Modules.ResourceSelection
{
    public class ResourceSelectionModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;

		public ResourceSelectionModule(IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

        public void Initialize()
        {
            this.RegisterViewsAndServices();
			this.eventAggregator.GetEvent<LaunchResourceSelectionDialogEvent> ()
				.Subscribe (LaunchResourceSelectionDialog, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<LaunchResourceAvailibilityDialogEvent> ()
				.Subscribe (LaunchResourceAvailibilityDialog, ThreadOption.UIThread, true);
        }

		public void LaunchResourceSelectionDialog (CompositePresentationEvent<SchdResource> newforwardingEvent)
		{
			IResourceSelectionController controller = this.container.Resolve<IResourceSelectionController> ();
			controller.Model.NewForwardingEvent = newforwardingEvent;
			controller.Run ("Resource Selection");
		}

		public void LaunchResourceAvailibilityDialog (CompositePresentationEvent<SchdResource> newforwardingEvent)
		{
			IResourceSelectionController controller = this.container.Resolve<IResourceSelectionController> ();
			controller.Model.NewForwardingEvent = newforwardingEvent;
			controller.Run ("Availibility Selection");
		}

        protected void RegisterViewsAndServices()
        {
            this.container.RegisterType<IResourceSelectionController, ResourceSelectionController>();
			this.container.RegisterType<IResourceSelectionView, ResourceSelectionView> ();
			this.container.RegisterType<IResourceSelectionPresentationModel, ResourceSelectionPresentationModel> ();
            this.container.RegisterType<IResourceSelectionService, ResourceSelectionService>();
        }
    }
}
