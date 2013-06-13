using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Events;
using ClinSchd.Modules.UserLogin.Controllers;
using ClinSchd.Modules.UserLogin.Services;

namespace ClinSchd.Modules.UserLogin.UserLogin
{
	public class UserLoginPresentationModel : IUserLoginPresentationModel, INotifyPropertyChanged
    {
        private readonly IUserLoginService UserLoginService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private ValidationMessage validationMessage;
		private IList<Divisions> _divisions;

		public UserLoginPresentationModel (
			IUserLoginView view,
			IUserLoginService UserLoginService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.eventAggregator = eventAggregator;
			this.UserLoginService = UserLoginService;
			this.dataAccessService = dataAccessService;
			this.ValidationMessage = new ValidationMessage ();
			InitialErrorMessage ();

			UserLoginCommand = new DelegateCommand<string> (ExecuteUserLoginCommand, CanExecuteUserLoginCommand);
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		

		public void ExecuteUserLoginCommand (string command)
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
			bool returnValue = false;

			returnValue = this.dataAccessService.LoginUser(AccessCode, VerifyCode);

			if (returnValue) {
				this.dataAccessService.LoadGlobalRecordsets ();
				
			} else {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "User Login";
				this.validationMessage.Message = "Login failed";
				return;
			}
		}

		public bool CanExecuteUserLoginCommand (string command)
		{
			return true;
		}


		public void OnClose()
		{
			View.Close();
		}

		#region IUserLoginPresentationModel Members

		public DelegateCommand<string> UserLoginCommand { get; private set; }

		public IUserLoginView View { get; private set; }

		public string AccessCode { get; set; }
		public string VerifyCode { get; set; }
		
		public ValidationMessage ValidationMessage
		{
			get
			{
				return this.validationMessage;
			}
			set
			{
				this.validationMessage = value;
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
