using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Navigation.Controllers;
using ClinSchd.Modules.Navigation.Services;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using System.IO;
using System.Windows.Markup;

namespace ClinSchd.Modules.Navigation.Group
{
    public class GroupPresentationModel : IGroupPresentationModel, INotifyPropertyChanged
    {
        private readonly INavigationService navigationService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private readonly IDataAccessService dataAccessService;
		private bool patientSelectedByMe;
		private bool resourceListStale;
		private bool resourceGroupListStale;
		private bool providerListStale;
		private readonly Dispatcher Dispatcher;
		private readonly Factory<SchdResource> SchdResourceFactory;

		public GroupPresentationModel(IGroupView view,
			INavigationService navigationService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator,
			Factory<SchdResource> schdResourceFactory,
			Factory<SchdResourceGroup> schdResourceGroupFactory,
			Factory<Provider> providerFactory)
        {
			Dispatcher = Dispatcher.CurrentDispatcher;
            View = view;
            View.Model = this;
            this.navigationService = navigationService;
            this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;
			this.AdmittedPatients = new ObservableCollection<Patient> ();
			this.Resource = schdResourceFactory.Create ();
			this.ResourceGroup = schdResourceGroupFactory.Create ();
			this.Provider = providerFactory.Create ();
			this.SchdResourceFactory = schdResourceFactory;

			SchedulerDisplayGroupCommand = new DelegateCommand<SchdResourceGroup> (ExecuteSchedulerDisplayGroupCommand, CanExecuteSchedulerDisplayGroupCommand);
			SchedulerDisplayCommand = new DelegateCommand<SchdResource> (ExecuteSchedulerDisplayCommand, CanExecuteSchedulerDisplayCommand);
			SchedulerDisplayProviderCommand = new DelegateCommand<Provider> (ExecuteSchedulerDisplayProviderCommand, CanExecuteSchedulerDisplayProviderCommand);

			this.eventAggregator.GetEvent<PatientSelectedEvent> ().Subscribe (PatientSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<ResourceSelectedEvent> ().Subscribe (ResourceSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<ProviderSelectedEvent> ().Subscribe (ProviderSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<ResourceGroupSelectedEvent> ().Subscribe (ResourceGroupSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshDataSources, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<LoadGlobalRecordsetsEvent> ().Subscribe (LoadGlobalRecordsets, ThreadOption.UIThread, false);
        }

		private void LoadGlobalRecordsets (string T)
		{
			this.dataAccessService.LoadGlobalRecordsets ();
			this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish (null);
		}

		public void PatientSelected (Patient selectedPatient)
		{
			if (selectedPatient != null) {
				if (!patientSelectedByMe) {
					bool patientExistsInWorkspace = false;
					foreach (Patient item in AdmittedPatients) {
						if (item.IEN == selectedPatient.IEN) {
							if (!String.IsNullOrEmpty (selectedPatient.IEN)) {
								patientExistsInWorkspace = true;
								selectedPatient = item;
								break;
							} else {
								if (item.Name == selectedPatient.Name) {
									patientExistsInWorkspace = true;
									selectedPatient = item;
									break;
								}
							}
						}
					}
					if (!patientExistsInWorkspace) {
						AdmittedPatients.Add (selectedPatient);
					}
					SelectedPatient = selectedPatient;
					OnPropertyChanged ("SelectedPatient");
				} else {
					patientSelectedByMe = false;
				}
			}
		}

		public bool AreDropDownsOpen { get; set; }
		private void CollapseComboBoxes ()
		{
			AreDropDownsOpen = false;
			OnPropertyChanged ("AreDropDownsOpen");
		}

		public void SelectPatient (Patient selectedPatient)
		{
			patientSelectedByMe = true;
			this.eventAggregator.GetEvent<PatientSelectedEvent> ().Publish (selectedPatient);
		}

		public void ResourceSelected (SchdResource selectedResource)
		{
			Resource = selectedResource;
			CollapseComboBoxes ();
			OnPropertyChanged ("Resource");
		}

		private void ProviderSelected (Provider provider)
		{
			Provider = provider;
			CollapseComboBoxes ();
			OnPropertyChanged ("Provider");
		}

		public void ResourceGroupSelected (SchdResourceGroup selectedResourceGroup)
		{
			ResourceGroup = selectedResourceGroup;
			CollapseComboBoxes ();
			OnPropertyChanged ("ResourceGroup");
		}

		public void RemovePatientFromWorkspace (Patient selectedPatient)
		{
			AdmittedPatients.Remove (selectedPatient);
			SelectedPatient = null;
		}

		public void ViewDayRange (DateTime dayToView)
		{
			this.eventAggregator.GetEvent<SchedulerViewDateEvent> ().Publish (dayToView);
		}

		private void RefreshDataSources (string arg)
		{
			SchdResource resource = Resource;
			SchdResourceGroup resourceGroup = ResourceGroup;
			Provider provider = Provider;
			resourceListStale = true;
			OnPropertyChanged ("ResourceList");
			resourceGroupListStale = true;
			OnPropertyChanged ("ResourceGroupList");
			providerListStale = true;
			OnPropertyChanged ("ProviderList");
			foreach (SchdResource item in ResourceList) {
				if (resource != null) {
					if (item.RESOURCEID == resource.RESOURCEID) {
						Resource = item;
						OnPropertyChanged ("Resource");
						break;
					}
				}
			}
			foreach (SchdResourceGroup item in ResourceGroupList) {
				if (resourceGroup != null) {
					if (item.IEN == resourceGroup.IEN) {
						ResourceGroup = item;
						OnPropertyChanged ("ResourceGroup");
						break;
					}
				}
			}
			foreach (Provider item in ProviderList) {
				if (provider != null) {
					if (item.IEN == provider.IEN) {
						Provider = item;
						OnPropertyChanged ("Provider");
						break;
					}
				}
			}
		}

		public ObservableCollection<Patient> AdmittedPatients { get; set; }
		public Patient SelectedPatient { get; set; }

		public IGroupView View { get; private set; }
		public SchdResource Resource { get; set; }
		private IList<SchdResource> resourceList;
		public IList<SchdResource> ResourceList
		{
			get
			{
				if (resourceList == null || resourceListStale) {
					if (Resource == null) {
						Resource = new SchdResource (dataAccessService);
					}
					Resource.IsAsync = false;
					Resource.GetAllResources ((s, args) => {
						resourceList = args.Result as IList<SchdResource>;
					    resourceListStale = false;
					    OnPropertyChanged ("ResourceList");
					});
				}
				return resourceList;
			}
		}
		public SchdResourceGroup ResourceGroup { get; set; }
		private IList<SchdResourceGroup> resourceGroupList;
		public IList<SchdResourceGroup> ResourceGroupList
		{
			get
			{
				if (resourceGroupList == null || resourceGroupListStale) {
					if (ResourceGroup == null) {
						ResourceGroup = new SchdResourceGroup (dataAccessService);
					}
					ResourceGroup.IsAsync = false;
					ResourceGroup.GetAllResourceGroups ((s, args) => {
						lock (args) {
							resourceGroupList = args.Result as IList<SchdResourceGroup>;
						}
						resourceGroupListStale = false;
						OnPropertyChanged ("ResourceGroupList");
					});
				}
				return resourceGroupList;
			}
		}
		public Provider Provider { get; set; }
		private IList<Provider> providerList;
		public IList<Provider> ProviderList
		{
			get
			{
				if (providerList == null || providerListStale) {
					Provider.IsAsync = false;
					Provider.GetAllProviders ((s, args) => {
						providerList = args.Result as IList<Provider>;
						providerListStale = false;
						OnPropertyChanged ("ProviderList");
					});
				}
				return providerList;
			}
		}

        public INavigationController Controller { get; set; }

		public DelegateCommand<Provider> SchedulerDisplayProviderCommand { get; private set; }

		public void ExecuteSchedulerDisplayProviderCommand (Provider param)
		{
			SchdResourceGroup providerGroup = new SchdResourceGroup (this.dataAccessService);
			providerGroup.Name = param.Name;
			foreach (SchdResource item in param.Resources) {
				providerGroup.Resources.Add (item);
			}
			this.eventAggregator.GetEvent<ProviderSelectedEvent> ().Publish (param);
			if (providerGroup.Resources.Count > 0) {
				this.eventAggregator.GetEvent<SchedulerDisplayGroupEvent> ().Publish (providerGroup);
			} else {
				this.eventAggregator.GetEvent<SchedulerDisplayEmptyPaneEvent> ().Publish ("There are no Clinics associated with " + param.Name + ".");
			}
		}

		public bool CanExecuteSchedulerDisplayProviderCommand (Provider param)
		{
			return true;
		}

		public DelegateCommand<SchdResource> SchedulerDisplayCommand { get; private set; }

		public void ExecuteSchedulerDisplayCommand (SchdResource param)
		{
			this.eventAggregator.GetEvent<SchedulerDisplayEvent> ().Publish (param);
			this.eventAggregator.GetEvent<ResourceSelectedEvent> ().Publish (param);
			this.eventAggregator.GetEvent<FocusSchedulerEvent> ().Publish (null);
		}

		public bool CanExecuteSchedulerDisplayCommand (SchdResource resourceID)
		{
			return true;
		}

		public DelegateCommand<SchdResourceGroup> SchedulerDisplayGroupCommand { get; private set; }

		public void ExecuteSchedulerDisplayGroupCommand (SchdResourceGroup param)
		{
			this.eventAggregator.GetEvent<ResourceGroupSelectedEvent> ().Publish (param);
			this.eventAggregator.GetEvent<SchedulerDisplayGroupEvent> ().Publish (param);
		}

		public bool CanExecuteSchedulerDisplayGroupCommand (SchdResourceGroup param)
		{
			return true;
		}	
		
		public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
	}
}