using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ResourceSelection.Controllers;
using ClinSchd.Modules.ResourceSelection.Services;
using Microsoft.Practices.Unity;

namespace ClinSchd.Modules.ResourceSelection.ResourceSelection
{
    public class ResourceSelectionPresentationModel : IResourceSelectionPresentationModel, INotifyPropertyChanged
    {
        private readonly IResourceSelectionService resourceSelectionService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private string searchString;
		private IList<SchdResource> resourceList;
		private SchdResource selectedResource;
		private RPMSClinic selectedClinic;
		private string _paneTitle = "";

		public ResourceSelectionPresentationModel (
			IResourceSelectionView view,
			IResourceSelectionService resourceSelectionService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.eventAggregator = eventAggregator;
			this.resourceSelectionService = resourceSelectionService;
			this.dataAccessService = dataAccessService;
		}

		public void SearchStringChanged(string newSearchString)
		{
			this.SearchString = newSearchString;

			IList<SchdResource> newResourcesList = this.dataAccessService.GetResources (newSearchString == string.Empty ? null : newSearchString, false);
			this.ResourceList = newResourcesList;
		}
		
		public void OnClose()
		{
			View.Close();
			if (View.DialogResult != null && View.DialogResult.Value && selectedResource != null)
			{
				// Forward the sender's event
				NewForwardingEvent.Publish (SelectedResource);
				
				// Notify anyone who cares that a patient has been selected
				this.eventAggregator.GetEvent<ResourceSelectedEvent> ().Publish (SelectedResource);
			}
		}

		[Dependency]
		public Factory<SchdResource> ResourceFactory { get; set; }

		public void ResetResource ()
		{
			SelectedResource = ResourceFactory.Create ();
		}

		#region IResourceSelectionPresentationModel Members

		public IResourceSelectionView View { get; private set; }
		public CompositePresentationEvent<SchdResource> NewForwardingEvent { get; set; }

		public string PaneTitle
		{
			get
			{
				return _paneTitle;
			}
			set
			{
				_paneTitle = value;
			}
		}

		public string SearchString
		{
			get
			{
				return this.searchString;
			}
			set
			{
				if (this.searchString != value)
				{
					this.searchString = value;
					this.OnPropertyChanged("SearchString");
				}
			}
		}

		public IList<SchdResource> ResourceList
		{
			get
			{
				return resourceList;
			}
			private set
			{
				if (this.resourceList != value) {
					this.resourceList = value;
					this.OnPropertyChanged ("ResourceList");
				}
			}
		}

		public SchdResource SelectedResource
		{
			get
			{
				return selectedResource;
			}
			set
			{
				if (this.selectedResource != value && value != null) {
					this.selectedResource = value;
					this.OnPropertyChanged ("SelectedResource");
				}
			}
		}

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler Handler = PropertyChanged;
			if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}
