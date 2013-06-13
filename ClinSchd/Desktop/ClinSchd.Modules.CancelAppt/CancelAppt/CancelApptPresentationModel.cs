using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CancelAppt.Controllers;
using ClinSchd.Modules.CancelAppt.Services;

namespace ClinSchd.Modules.CancelAppt.CancelAppt
{
	public class CancelApptPresentationModel : ICancelApptPresentationModel, INotifyPropertyChanged
	{
		private readonly ICancelApptService CancelApptService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private ValidationMessage validationMessage;
		private CancelAppointment _cancelAppointment;
		private AutoRebookData _autoRebook;

		public CancelApptPresentationModel (
			ICancelApptView view,
			ICancelApptService CancelApptService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
		{
			View = view;
			View.Model = this;
			this.eventAggregator = eventAggregator;
			this.CancelApptService = CancelApptService;
			this.dataAccessService = dataAccessService;
			this.ValidationMessage = new ValidationMessage ();
			InitialErrorMessage ();

			CancelApptAppointmentCommand = new DelegateCommand<string> (ExecuteCancelApptAppointmentCommand, CanExecuteCancelApptAppointmentCommand);
			PrintLetterCommand = new DelegateCommand<string> (ExecutePrintLetterCommand, CanExecutePrintLetterCommand);
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public void CancelApptAppointment (SchdAppointment newAppointment)
		{
			CancelAppointment cancelAppointment = new CancelAppointment ();
			AutoRebookData autoRebook = new AutoRebookData ();
			if (newAppointment == null || newAppointment.APPOINTMENTID == null) {
				return;
			} else {
				cancelAppointment.ApptID = newAppointment.APPOINTMENTID;
				cancelAppointment.CancelledByClinic = true;
				autoRebook.RebookAppointment = newAppointment;
				autoRebook.IsAccessTypeChecked = true;
				autoRebook.MinimumDays = 7;
				autoRebook.MaximumDays = 30;
				this.IsAutoRebookChecked = false;
				OnPropertyChanged ("IsAutoRebookChecked");
			}

			//reason processing
			Reasons = this.dataAccessService.GetCancellationReasons ();
			OnPropertyChanged ("Reasons");

			this.ReasonIEN = Reasons[0].Value;
			cancelAppointment.ReasonIEN = this.ReasonIEN;
			OnPropertyChanged ("ReasonIEN");

			this._cancelAppointment = cancelAppointment;
			OnPropertyChanged ("CancelAppointment");

			this._autoRebook = autoRebook;
			OnPropertyChanged ("AutoRebook");
		}

		public void ExecuteCancelApptAppointmentCommand (string command)
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			string errorMessage = this.dataAccessService.CancelAppointment (this.CancelAppointment);

			if (errorMessage == string.Empty) {
				this.eventAggregator.GetEvent<RefreshScheduler> ().Publish (command);
			} else {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Cancel Appointment";
				this.validationMessage.Message = errorMessage;
				return;
			}

			if (this.IsAutoRebookChecked) {
				if (this.AutoRebook.IsAccessTypeChecked) {
					this.AutoRebook.AccessTypeID = int.Parse (this.AutoRebook.RebookAppointment.ACCESSTYPEID);
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

		public bool CanExecuteCancelApptAppointmentCommand (string command)
		{
			return true;
		}

		private void ExecutePrintLetterCommand (string command)
		{
			object[] args = new object[3];
			args[0] = "Patient Cancellation Letter";
			args[1] = this.CancelAppointment.ApptID;
			args[2] = this.CancelAppointment.CancelledByClinic ? "C" : "A";
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		private bool CanExecutePrintLetterCommand (string command)
		{
			return true;
		}

		public void OnClose()
		{
			View.Close();
		}

		#region ICancelApptPresentationModel Members

		public ICancelApptView View { get; private set; }
		public SchdAppointment SelectedAppointment { get; set; }
		public string ReasonIEN { get; set; }
		public string AccessType { get; set; }
		public bool IsAutoRebookChecked { get; set; }
		public IList<NameValue> Reasons { get; set; }
		public DelegateCommand<string> CancelApptAppointmentCommand { get; private set; }
		public DelegateCommand<string> PrintLetterCommand { get; private set; }

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

		public CancelAppointment CancelAppointment
		{
			get
			{
				return _cancelAppointment;
			}
			set
			{
				_cancelAppointment = value;
			}
		}

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

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler Handler = PropertyChanged;
			if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
