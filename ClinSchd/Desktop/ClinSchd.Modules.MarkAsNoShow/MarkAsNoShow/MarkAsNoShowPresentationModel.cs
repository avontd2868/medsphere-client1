using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.MarkAsNoShow.Controllers;
using ClinSchd.Modules.MarkAsNoShow.Services;

namespace ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow
{
	public class MarkAsNoShowPresentationModel : IMarkAsNoShowPresentationModel, INotifyPropertyChanged
	{
		private readonly IMarkAsNoShowService MarkAsNoShowService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private ValidationMessage validationMessage;
		private AutoRebookData _autoRebook;

		public MarkAsNoShowPresentationModel (
			IMarkAsNoShowView view,
			IMarkAsNoShowService markAsNoShowService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
		{
			View = view;
			View.Model = this;
			this.eventAggregator = eventAggregator;
			this.MarkAsNoShowService = markAsNoShowService;
			this.dataAccessService = dataAccessService;
			this.ValidationMessage = new ValidationMessage ();
			InitialErrorMessage ();
			NoShowAppointmentCommand = new DelegateCommand<string> (ExecuteNoShowAppointmentCommand, CanExecuteNoShowAppointmentCommand);
			PrintLetterCommand = new DelegateCommand<string> (ExecutePrintLetterCommand, CanExecutePrintLetterCommand);
		}

		public void NoShowAppointment (SchdAppointment newAppointment)
		{
			AutoRebookData autoRebook = new AutoRebookData ();
			if (newAppointment == null || newAppointment.APPOINTMENTID == null) {
				return;
			} else {
				this.SelectedAppointment = newAppointment;
				autoRebook.RebookAppointment = newAppointment;
				autoRebook.IsAccessTypeChecked = true;
				autoRebook.MinimumDays = 7;
				autoRebook.MaximumDays = 30;
				this.IsAutoRebookChecked = false;
				OnPropertyChanged ("IsAutoRebookChecked");
			}

			this._autoRebook = autoRebook;
			OnPropertyChanged ("AutoRebook");
		}

		public void ExecuteNoShowAppointmentCommand (string command)
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			string errorMessage = this.dataAccessService.AppointmentNoShow (this.SelectedAppointment.APPOINTMENTID, true);

			if (errorMessage != string.Empty) {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Mark As NoShow Appointment";
				this.validationMessage.Message = errorMessage;
				return;
			} else {
				this.eventAggregator.GetEvent<RefreshScheduler> ().Publish (command);
			}

			if (this.IsAutoRebookChecked) {
				if (this.AutoRebook.IsAccessTypeChecked) {
					this.AutoRebook.AccessTypeID = int.Parse(this.SelectedAppointment.ACCESSTYPEID);
				} else {
					this.AutoRebook.AccessTypeID = 0;
				}
				errorMessage = this.dataAccessService.AutoRebook (this.AutoRebook);
				if (errorMessage != string.Empty) {
					this.validationMessage.IsValid = false;
					this.validationMessage.Title = "AutoRebook Appointment";
					this.validationMessage.Message = errorMessage;
					return;
				}
			}
		}

		private void ExecutePrintLetterCommand (string command)
		{
			object[] args = new object[3];
			args[0] = "Patient No Show Letter";
			args[1] = this.SelectedAppointment.APPOINTMENTID;
			args[2] = "N";
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		public bool CanExecuteNoShowAppointmentCommand (string command)
		{
			return true;
		}

		private bool CanExecutePrintLetterCommand (string command)
		{
			return true;
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public void OnClose ()
		{
			View.Close ();
		}

		public DelegateCommand<string> PrintLetterCommand { get; private set; }

		#region IMarkAsNoShowPresentationModel Members

		public DelegateCommand<string> NoShowAppointmentCommand { get; private set; }
		public IMarkAsNoShowView View { get; private set; }
		public SchdAppointment SelectedAppointment { get; set; }
		public bool IsAutoRebookChecked { get; set; }
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

		public AutoRebookData AutoRebook
		{
			get
			{
				return _autoRebook;
			}
			set
			{
				_autoRebook = value;
			}
		}

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler Handler = PropertyChanged;
			if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
