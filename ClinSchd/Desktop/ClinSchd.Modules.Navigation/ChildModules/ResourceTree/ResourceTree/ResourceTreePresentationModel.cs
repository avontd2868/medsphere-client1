//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Navigation.ResourceTree.Controllers;
using ClinSchd.Modules.Navigation.ResourceTree.Services;

namespace ClinSchd.Modules.Navigation.ResourceTree
{
    public class ResourceTreePresentationModel : IResourceTreePresentationModel, INotifyPropertyChanged
    {
		private readonly INavigationResourceTreeService navigationResourceTreeService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private Patient selectedPatient = new Patient();

		public ResourceTreePresentationModel(
			IResourceTreeView view,
			INavigationResourceTreeService navigationResourceTreeService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.navigationResourceTreeService = navigationResourceTreeService;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;

			AdmitPatientCommand = new DelegateCommand<string> (ExecuteAdmitPatientCommand, CanExecuteAdmitPatientCommand);

			AdmitPatientCommand = new DelegateCommand<string> (ExecuteAdmitPatientCommand, CanExecuteAdmitPatientCommand);
			DischargePatientCommand = new DelegateCommand<string>(ExecuteDischargePatientCommand, CanExecuteDischargePatientCommand);
			TransferPatientCommand = new DelegateCommand<string>(ExecuteTransferPatientCommand, CanExecuteTransferPatientCommand);
			DaySurgeryCommand = new DelegateCommand<string>(ExecuteDaySurgeryCommand, CanExecuteDaySurgeryCommand);
			IncompleteChartCommand = new DelegateCommand<string>(ExecuteIncompleteChartCommand, CanExecuteIncompleteChartCommand);
			ScheduledVisitCommand = new DelegateCommand<string>(ExecuteScheduledVisitCommand, CanExecuteScheduledVisitCommand);

			this.eventAggregator.GetEvent<PatientSelectedEvent>().Subscribe(PatientSelected, ThreadOption.UIThread, false);
		}

		public DelegateCommand<string> AdmitPatientCommand { get; private set; }
		public DelegateCommand<string> DischargePatientCommand { get; private set; }
		public DelegateCommand<string> TransferPatientCommand { get; private set; }
		public DelegateCommand<string> DaySurgeryCommand { get; private set; }
		public DelegateCommand<string> IncompleteChartCommand { get; private set; }
		public DelegateCommand<string> ScheduledVisitCommand { get; private set; }

		public void ExecuteAdmitPatientCommand(string title)
		{
			this.eventAggregator.GetEvent<SchedulerDisplayEvent> ().Publish (title);
		}

		public bool CanExecuteAdmitPatientCommand(string title)
		{
			return true;
		}

		public void ExecuteDischargePatientCommand(string title)
		{
			this.eventAggregator.GetEvent<TaskDischargePatientEvent>().Publish(SelectedPatient);
		}

		public bool CanExecuteDischargePatientCommand(string title)
		{
			return true;
		}

		public void ExecuteTransferPatientCommand(string title)
		{
			this.eventAggregator.GetEvent<TaskTransferPatientEvent>().Publish(SelectedPatient);
		}

		public bool CanExecuteTransferPatientCommand(string title)
		{
			return true;
		}

		public void ExecuteDaySurgeryCommand(string title)
		{
			this.eventAggregator.GetEvent<TaskDaySurgeryEvent>().Publish(SelectedPatient);
		}

		public bool CanExecuteDaySurgeryCommand(string title)
		{
			return true;
		}

		public void ExecuteIncompleteChartCommand(string title)
		{
			this.eventAggregator.GetEvent<TaskIncompleteChartEvent>().Publish(SelectedPatient);
		}

		public bool CanExecuteIncompleteChartCommand(string title)
		{
			return true;
		}

		public void ExecuteScheduledVisitCommand(string title)
		{
			this.eventAggregator.GetEvent<TaskScheduledVisitEvent>().Publish(SelectedPatient);
		}

		public bool CanExecuteScheduledVisitCommand(string title)
		{
			return true;
		}

		public void PatientSelected(Patient selectedPatient)
		{
			this.dataAccessService.AddPatientToWorkspace(selectedPatient);
			this.View.Refresh();
			//this.OnPropertyChanged("WorkspacePatients");
		}

		#region IResourceTreePresentationModel Members

		public IResourceTreeView View { get; private set; }

		public INavigationResourceTreeController Controller { get; set; }

		public IList<Patient> WorkspacePatients { get { return this.dataAccessService.GetWorkspacePatients(); } }

		public Patient SelectedPatient
		{
			get
			{
				return selectedPatient;
			}
			set
			{
				if (this.selectedPatient != value)
				{
					this.selectedPatient = value;
					this.OnPropertyChanged("SelectedPatient");

					// Notify anyone who cares that a patient has been selected
					this.eventAggregator.GetEvent<WorkspacePatientSelectedEvent>().Publish(this.selectedPatient);
				}
			}
		}

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

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
