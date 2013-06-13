using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeUser.Controllers;
using ClinSchd.Modules.ChangeUser.Services;

namespace ClinSchd.Modules.ChangeUser.ChangeUser
{
    public class ChangeUserPresentationModel : IChangeUserPresentationModel, INotifyPropertyChanged
    {
        private readonly IChangeUserService changeUserService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private string searchString;
		private IList<User> userList;
		private User selectedUser;

		public ChangeUserPresentationModel (
			IChangeUserView view,
			IChangeUserService changeUserService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.eventAggregator = eventAggregator;
			this.changeUserService = changeUserService;
			this.dataAccessService = dataAccessService;
		}

		public void SearchStringChanged(string newSearchString)
		{
//			IList<User> newUserList = this.dataAccessService.
//				GetPatients(newSearchString == string.Empty ? null : newSearchString, 20);
			this.SearchString = newSearchString;
//			this.ServerList = newServerList;
		}
		
		public void OnClose()
		{
			View.Close();
			if (View.DialogResult != null && View.DialogResult.Value && SelectedUser != null)
			{
				// Forward the sender's event
				ForwardingEvent.Publish(SelectedUser);
				
				// Notify anyone who cares that a patient has been selected
				this.eventAggregator.GetEvent<UserSelectedEvent>().Publish(SelectedUser);
			}
		}

		#region IChangeUserPresentationModel Members

		public IChangeUserView View { get; private set; }
		public CompositePresentationEvent<User> ForwardingEvent { get; set; }

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

		public IList<User> UserList
		{
			get
			{
				return userList;
			}
			private set
			{
				if (this.userList != value)
				{
					this.userList = value;
					this.OnPropertyChanged("UserList");
				}
			}
		}

		public User SelectedUser
		{
			get
			{
				return selectedUser;
			}
			set
			{
				if (this.selectedUser != value && value != null && value.IEN != null)
				{
					this.selectedUser = value;
					this.OnPropertyChanged("SelectedUser");
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
