using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.DataAccess.Controllers;

namespace ClinSchd.Modules.DataAccess.DataAccess
{

    public class DataAccessPresentationModel : IDataAccessPresentationModel, INotifyPropertyChanged
    {
        private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;

		public DataAccessPresentationModel(IDataAccessView view, IDataAccessService dataAccessService, IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
            this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;
			AdmitDataAccessCommand = new DelegateCommand<string>(ExecuteAdmitDataAccessCommand, CanExecuteAdmitDataAccessCommand);
			DischargeDataAccessCommand = new DelegateCommand<string> (ExecuteDischargeDataAccessCommand, CanExecuteDischargeDataAccessCommand);
			TransferDataAccessCommand = new DelegateCommand<string> (ExecuteTransferDataAccessCommand, CanExecuteTransferDataAccessCommand);
			//this.eventAggregator.GetEvent<DataAccessSelectionSelectedEvent>().Subscribe(DataAccessSelected, ThreadOption.UIThread);
		}

        public IDataAccessView View { get; private set; }

        public IDataAccessController Controller { get; set; }

		public DelegateCommand<string> AdmitDataAccessCommand { get; private set; }
		public DelegateCommand<string> DischargeDataAccessCommand { get; private set; }
		public DelegateCommand<string> TransferDataAccessCommand { get; private set; }

		public void ExecuteAdmitDataAccessCommand(string title)
		{
		}

		public void ExecuteDischargeDataAccessCommand (string title)
		{
		}

		public void ExecuteTransferDataAccessCommand (string title)
		{
		}

		public bool CanExecuteAdmitDataAccessCommand(string title)
		{
			return true;
		}

		public bool CanExecuteDischargeDataAccessCommand (string title)
		{
			return true;
		}

		public bool CanExecuteTransferDataAccessCommand (string title)
		{
			return true;
		}

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
