using System.Windows;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.UnityExtensions;
using Telerik.Windows.Controls;
using Microsoft.Practices.Composite.Presentation.Regions;

using ClinSchd.Modules.DataAccess;

using ClinSchd.Modules.Ribbon;

using ClinSchd.Modules.Navigation;

using ClinSchd.Modules.Task;
using ClinSchd.Modules.Task.Scheduler;
using ClinSchd.Modules.Task.Availability;
using ClinSchd.Modules.Task.MultiScheduler;
using ClinSchd.Modules.Task.EmptyScheduler;
using ClinSchd.Modules.Task.AddEditAvailability;

using ClinSchd.Modules.Management;
using ClinSchd.Modules.Management.Resources;
using ClinSchd.Modules.Management.AddResourceUser;
using ClinSchd.Modules.Management.AddResource;
using ClinSchd.Modules.Management.AddAccessType;
using ClinSchd.Modules.Management.AddAccessGroup;
using ClinSchd.Modules.Management.AddResourceGroup;
using ClinSchd.Modules.Management.AccessGroups;
using ClinSchd.Modules.Management.ResourceGroups;

using ClinSchd.Modules.PatientAppt;

using ClinSchd.Modules.ResourceSelection;
using ClinSchd.Modules.PatientSelection;

using ClinSchd.Modules.ChangeDivision;

using ClinSchd.Modules.MarkAsNoShow;
using ClinSchd.Modules.CheckIn;
using ClinSchd.Modules.CheckOut;
using ClinSchd.Modules.CancelAppt;
using ClinSchd.Modules.FindAppt;
using ClinSchd.Modules.Reports;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.DataAccess.Services;
using Microsoft.Practices.Unity;
using ClinSchd.Modules.UserLogin;


namespace ClinSchd
{
    public partial class ClinSchdBootstrapper : UnityBootstrapper
    {
        protected override IModuleCatalog GetModuleCatalog()
        {
            var catalog = new ModuleCatalog();
            catalog
				.AddModule(typeof(DataAccessModule))
				
				.AddModule(typeof(RibbonModule))

				.AddModule(typeof(NavigationModule))

				.AddModule(typeof(TaskModule))
				.AddModule(typeof(TaskSchedulerModule))
				.AddModule(typeof(TaskAvailabilityModule))
				.AddModule(typeof(TaskMultiSchedulerModule))
				.AddModule(typeof(TaskEmptySchedulerModule))
				.AddModule (typeof (TaskAddEditAvailabilityModule))

				.AddModule (typeof (ManagementModule))
				.AddModule (typeof (ManagementAccessGroupsModule))
				.AddModule (typeof (ManagementResourceGroupsModule))
				.AddModule (typeof (ManagementResourcesModule))
				.AddModule (typeof (ManagementAddResourceModule))
				.AddModule (typeof (ManagementAddResourceUserModule))
				.AddModule (typeof (ManagementAddAccessTypeModule))
				.AddModule (typeof (ManagementAddAccessGroupModule))
				.AddModule (typeof (ManagementAddResourceGroupModule))

				.AddModule (typeof (PatientApptModule))

				.AddModule(typeof(ResourceSelectionModule))
				.AddModule (typeof (PatientSelectionModule))
				.AddModule (typeof (ChangeDivisionModule))
				.AddModule (typeof (UserLoginModule))

				.AddModule (typeof (MarkAsNoShowModule))
				.AddModule (typeof (CheckInModule))
				.AddModule (typeof (CheckOutModule))
				.AddModule (typeof (CancelApptModule))
				.AddModule (typeof (FindApptModule))
				.AddModule (typeof (ReportsModule))
			;

            return catalog;
        }

        protected override void ConfigureContainer()
        {
			base.ConfigureContainer ();
            Container.RegisterType<IShellView, Shell>();
			Container.RegisterType<Patient> ();
			Container.RegisterType<SchdAppointment> ();
			Container.RegisterType<Provider> ();
			Container.RegisterType<SchdResource> ();
			Container.RegisterInstance<IDataAccessService> (new DataAccessService (Container));
			Container.RegisterInstance<Factory<Patient>> (new Factory<Patient> (Container));
			Container.RegisterInstance<Factory<SchdAppointment>> (new Factory<SchdAppointment> (Container));
			Container.RegisterInstance<Factory<Provider>> (new Factory<Provider> (Container));
			Container.RegisterInstance<Factory<SchdResource>> (new Factory<SchdResource> (Container));
			Container.RegisterInstance<Factory<SchdResourceGroup>> (new Factory<SchdResourceGroup> (Container));
        }

        protected override DependencyObject CreateShell()
        {
            ShellPresenter presenter = Container.Resolve<ShellPresenter>();
            IShellView view = presenter.View;

            view.ShowView();

            return view as DependencyObject;
        }

		protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
		{
			var mappings = base.ConfigureRegionAdapterMappings();

			mappings.RegisterMapping(typeof(RadPaneGroup), this.Container.Resolve<RadPaneGroupRegionAdapter>());

			return mappings;
		}
    }
}
