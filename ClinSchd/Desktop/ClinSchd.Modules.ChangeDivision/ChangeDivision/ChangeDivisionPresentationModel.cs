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
using ClinSchd.Modules.ChangeDivision.Controllers;
using ClinSchd.Modules.ChangeDivision.Services;

namespace ClinSchd.Modules.ChangeDivision.ChangeDivision
{
	public class ChangeDivisionPresentationModel : IChangeDivisionPresentationModel, INotifyPropertyChanged
    {
        private readonly IChangeDivisionService changeDivisionService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private ValidationMessage validationMessage;
		private IList<Divisions> _divisions;

		public ChangeDivisionPresentationModel (
			IChangeDivisionView view,
			IChangeDivisionService changeDivisionService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.eventAggregator = eventAggregator;
			this.changeDivisionService = changeDivisionService;
			this.dataAccessService = dataAccessService;
			this.ValidationMessage = new ValidationMessage ();
			InitialErrorMessage ();

			ChangeDivisionCommand = new DelegateCommand<string> (ExecuteChangeDivisionCommand, CanExecuteChangeDivisionCommand);
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public void GetDivisions ()
		{
			DivisionList = this.dataAccessService.GetDivisions ();
			OnPropertyChanged ("DivisionList");

			foreach (Divisions d in DivisionList) {
				if (d.IsDefault) {
					this.DivisionIEN = d.IEN;
					break;
				}
			}
			OnPropertyChanged ("DivisionIEN");
		}

		public void ExecuteChangeDivisionCommand (string command)
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
			bool returnValue = false;

			returnValue = this.dataAccessService.SetDivision (this.DivisionIEN);

			if (returnValue) {
				this.dataAccessService.LoadGlobalRecordsets ();
				this.dataAccessService.CurrentDivision = this.dataAccessService.GetCurrentDivision ();
				this.eventAggregator.GetEvent<DisplayDefaultDivisionEvent> ().Publish (this.dataAccessService.CurrentDivision);
			} else {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Change Division";
				this.validationMessage.Message = "The current division cannot be changed";
				return;
			}
		}

		public bool CanExecuteChangeDivisionCommand (string command)
		{
			return true;
		}


		public void OnClose()
		{
			View.Close();
		}

		#region IChangeDivisionPresentationModel Members

		public DelegateCommand<string> ChangeDivisionCommand { get; private set; }

		public IChangeDivisionView View { get; private set; }
		public string DivisionIEN { get; set; }
		public IList<Divisions> DivisionList
		{
			get
			{
				return _divisions;
			}
			set
			{
				_divisions = value;
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

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler Handler = PropertyChanged;
			if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}
