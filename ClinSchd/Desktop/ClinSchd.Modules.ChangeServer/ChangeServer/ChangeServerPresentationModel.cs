using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeServer.Controllers;
using ClinSchd.Modules.ChangeServer.Services;

namespace ClinSchd.Modules.ChangeServer.ChangeServer
{
    public class ChangeServerPresentationModel : IChangeServerPresentationModel, INotifyPropertyChanged
    {
        private readonly IChangeServerService changeServerService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private string searchString;
		private IList<Server> serverList;
		private Server selectedServer;

		public ChangeServerPresentationModel (
			IChangeServerView view,
			IChangeServerService changeServerService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.eventAggregator = eventAggregator;
			this.changeServerService = changeServerService;
			this.dataAccessService = dataAccessService;
		}

		public void SearchStringChanged(string newSearchString)
		{
//			IList<Server> newServerList = this.dataAccessService.
//				GetPatients(newSearchString == string.Empty ? null : newSearchString, 20);
			this.SearchString = newSearchString;
//			this.ServerList = newServerList;
		}
		
		public void OnClose()
		{
			View.Close();
			if (View.DialogResult != null && View.DialogResult.Value && SelectedServer != null)
			{
				// Forward the sender's event
				ForwardingEvent.Publish(SelectedServer);
				
				// Notify anyone who cares that a patient has been selected
				this.eventAggregator.GetEvent<ServerSelectedEvent>().Publish(SelectedServer);
			}
		}

		#region IChangeServerPresentationModel Members

		public IChangeServerView View { get; private set; }
		public CompositePresentationEvent<Server> ForwardingEvent { get; set; }

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

		public IList<Server> ServerList
		{
			get
			{
				return serverList;
			}
			private set
			{
				if (this.serverList != value)
				{
					this.serverList = value;
					this.OnPropertyChanged("ServerList");
				}
			}
		}

		public Server SelectedServer
		{
			get
			{
				return selectedServer;
			}
			set
			{
				if (this.selectedServer != value && value != null && value.IEN != null)
				{
					this.selectedServer = value;
					this.OnPropertyChanged("SelectedServer");
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
