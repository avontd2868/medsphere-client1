using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.Controllers;
using ClinSchd.Modules.Management.Services;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;


namespace ClinSchd.Modules.Management.Group
{

	public class GroupPresentationModel : IGroupPresentationModel, INotifyPropertyChanged
    {
		private readonly IManagementService managementService;
        public event PropertyChangedEventHandler PropertyChanged;
		private readonly IEventAggregator eventAggregator;
bool test = false;

		public GroupPresentationModel (IGroupView view, IManagementService managementService, IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.managementService = managementService;
			this.eventAggregator = eventAggregator;
        }

		public void OnClose ()
		{
			View.Close ();
		}

		public IGroupView View { get; private set; }

		public IManagementController Controller { get; set; }
		public CompositePresentationEvent<RPMSClinic> NewForwardingEvent { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
