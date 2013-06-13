using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Reports.Controllers;
using ClinSchd.Modules.Reports.Helper_Classes;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Regions;
using System.Windows.Data;
using ClinSchd.Infrastructure.Controls;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Xml;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Reports.Reports
{
    public class ReportParamsPresentationModel : IReportParamsPresentationModel, INotifyPropertyChanged
    {
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;

		public ReportParamsPresentationModel(
			IReportParamsView view,
			ReportsModel model,
			IUnityContainer container,
			IRegionManager regionManager,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;
			Model = model;
			BuildRequiredControls ();
			this.container = container;
			this.regionManager = regionManager;
			this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;

			OKCommand = new DelegateCommand<string> (ExecuteOKCommand, CanExecuteOKCommand);
			CancelCommand = new DelegateCommand<string> (ExecuteCancelCommand, CanExecuteCancelCommand);
			SelectPatientCommand = new DelegateCommand<string> (ExecuteSelectPatientCommand, CanExecuteSelectPatientCommand);
			SelectPatientForListCommand = new DelegateCommand<string> (ExecuteSelectPatientForListCommand, CanExecuteSelectPatientForListCommand);

			regionManager.Regions[RegionNames.TaskGroupRegion].Add (View);
		}

		private RadButton selectPatientButton;
		private Label selectPatientLabel;
		private RadGridView patientListGridView;
		public void BuildRequiredControls ()
		{
			if (Model != null && Model.Params != null) {
				foreach (KeyValuePair<string, ReportParameter> item in Model.Params) {
					bool paramIsHardCoded = false;
					foreach (ReportParameter hardCodedParam in Model.HardCodedParams) {
						if (hardCodedParam.Description == item.Key) {
							paramIsHardCoded = true;
							break;
						}
					}
					if (!paramIsHardCoded) {
						Control valueControl = new Control ();
						Label keyControl = new Label ();
						View.ParamControlsGrid.RowDefinitions.Add (new RowDefinition ());

						if (item.Value.PossibleValues.Count == 0 && item.Value.ValueType == null) {
							valueControl = new TextBox ();
							valueControl.Width = 100;
							Binding valueBinding = new Binding ("Params[" + item.Key.ToString () + "].Value");
							valueBinding.Source = Model;
							valueBinding.Mode = BindingMode.TwoWay;
							valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
							valueControl.SetBinding (TextBox.TextProperty, valueBinding);
						} else {
							if (item.Value.ValueType != null) {
								switch (item.Value.ValueType) {
								case "DateTime":
									valueControl = new DateTimePicker();
									((DateTimePicker)valueControl).HideTimePicker = true;
									valueControl.Width = 100;
									Binding valueBinding = new Binding("Params[" + item.Key.ToString() + "].Value");
									valueBinding.Source = Model;
									valueBinding.Mode = BindingMode.OneWayToSource;
									valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
									valueControl.SetBinding(DateTimePicker.SelectedDateTimeFormattedInternalProperty, valueBinding);
									((DateTimePicker)valueControl).SelectedDate = DateTime.Today;
									break;
								//case "DateTimeExternal":
								//    valueControl = new DateTimePicker();
								//    ((DateTimePicker)valueControl).HideTimePicker = true;
								//    ((DateTimePicker)valueControl).TruncateTime = true;
								//    valueControl.Width = 100;
								//    valueBinding = new Binding("Params[" + item.Key.ToString() + "].Value");
								//    valueBinding.Source = Model;
								//    valueBinding.Mode = BindingMode.OneWayToSource;
								//    valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								//    valueControl.SetBinding(DateTimePicker.SelectedDateTimeFormattedProperty, valueBinding);
								//    ((DateTimePicker)valueControl).SelectedDate = DateTime.Today;
								//    break;
								case "Patient":
									valueControl = new RadButton();
									((RadButton)valueControl).Content = "Select...";
									((RadButton)valueControl).Command = SelectPatientCommand;
									selectPatientButton = valueControl as RadButton;
									valueControl.Padding = new Thickness(10, 5, 10, 5);
									Label PatientIENLabel = new Label();
									PatientIENLabel.VerticalAlignment = VerticalAlignment.Center;
									PatientIENLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
									PatientIENLabel.Margin = new Thickness(15, 5, 0, 0);
									selectPatientLabel = PatientIENLabel;
									View.ParamControlsGrid.Children.Add(PatientIENLabel);
									Grid.SetRow(PatientIENLabel, View.ParamControlsGrid.RowDefinitions.Count - 1);
									Grid.SetColumn(PatientIENLabel, 1);
									break;
								case "PatientAdmissionIEN":
									valueControl = new RadGridView();
									((RadGridView)valueControl).SelectionMode = SelectionMode.Single;
									valueControl.Width = 400;
									valueControl.Height = 100;
									valueBinding = new Binding("Params[" + item.Key.ToString() + "].Value");
									valueBinding.Source = Model;
									valueBinding.Mode = BindingMode.TwoWay;
									valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
									valueControl.SetBinding(RadGridView.SelectedItemProperty, valueBinding);
									Binding itemsBinding = new Binding("PatientAdmissionsList");
									itemsBinding.Source = Model;
									itemsBinding.Mode = BindingMode.OneWay;
									itemsBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
									valueControl.SetBinding(RadGridView.ItemsSourceProperty, itemsBinding);
									break;
								case "PatientIncompleteChartIEN":
									valueControl = new RadGridView();
									((RadGridView)valueControl).SelectionMode = SelectionMode.Single;
									valueControl.Width = 400;
									valueControl.Height = 100;
									valueBinding = new Binding("Params[" + item.Key.ToString() + "].Value");
									valueBinding.Source = Model;
									valueBinding.Mode = BindingMode.TwoWay;
									valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
									valueControl.SetBinding(RadGridView.SelectedItemProperty, valueBinding);
									itemsBinding = new Binding("PatientIncompleteChartList");
									itemsBinding.Source = Model;
									itemsBinding.Mode = BindingMode.OneWay;
									itemsBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
									valueControl.SetBinding(RadGridView.ItemsSourceProperty, itemsBinding);
									break;
								//case "Ward":
								//    valueControl = new ComboBox ();
								//    valueControl.Width = 100;
								//    valueBinding = new Binding ("Params[" + item.Key.ToString () + "].Value");
								//    valueBinding.Source = Model;
								//    valueBinding.Mode = BindingMode.OneWayToSource;
								//    valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								//    valueControl.SetBinding (ComboBox.SelectedValueProperty, valueBinding);
								//    ((ComboBox)valueControl).ItemsSource = Model.WardLocations;
								//    ((ComboBox)valueControl).DisplayMemberPath = "Name";
								//    ((ComboBox)valueControl).SelectedValuePath = "Value";
								//    if (((ComboBox)valueControl).Items.Count > 0) {
								//        ((ComboBox)valueControl).SelectedIndex = 0;
								//    }
								//    break;
								//case "Provider":
								//    valueControl = new ComboBox();
								//    valueControl.Width = 100;
								//    valueBinding = new Binding("Params[" + item.Key.ToString() + "].Value");
								//    valueBinding.Source = Model;
								//    valueBinding.Mode = BindingMode.OneWayToSource;
								//    valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								//    valueControl.SetBinding(ComboBox.SelectedValueProperty, valueBinding);
								//    ((ComboBox)valueControl).ItemsSource = Model.Providers;
								//    ((ComboBox)valueControl).DisplayMemberPath = "Name";
								//    ((ComboBox)valueControl).SelectedValuePath = "Value";
								//    if (((ComboBox)valueControl).Items.Count > 0)
								//    {
								//        ((ComboBox)valueControl).SelectedIndex = 0;
								//    }
								//    break;
								//case "ProviderAll":
								//    valueControl = new ComboBox();
								//    valueControl.Width = 100;
								//    valueBinding = new Binding("Params[" + item.Key.ToString() + "].Value");
								//    valueBinding.Source = Model;
								//    valueBinding.Mode = BindingMode.OneWayToSource;
								//    valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								//    valueControl.SetBinding(ComboBox.SelectedValueProperty, valueBinding);
								//    ((ComboBox)valueControl).ItemsSource = Model.ProvidersAll;
								//    ((ComboBox)valueControl).DisplayMemberPath = "Name";
								//    ((ComboBox)valueControl).SelectedValuePath = "Value";
								//    if (((ComboBox)valueControl).Items.Count > 0)
								//    {
								//        ((ComboBox)valueControl).SelectedIndex = 0;
								//    }
								//    break;
								//case "Religion":
								//    valueControl = new ComboBox ();
								//    valueControl.Width = 100;
								//    valueBinding = new Binding ("Params[" + item.Key.ToString () + "].Value");
								//    valueBinding.Source = Model;
								//    valueBinding.Mode = BindingMode.OneWayToSource;
								//    valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								//    valueControl.SetBinding (ComboBox.SelectedValueProperty, valueBinding);
								//    ((ComboBox)valueControl).ItemsSource = Model.Religions;
								//    ((ComboBox)valueControl).DisplayMemberPath = "Name";
								//    ((ComboBox)valueControl).SelectedValuePath = "Value";
								//    if (((ComboBox)valueControl).Items.Count > 0) {
								//        ((ComboBox)valueControl).SelectedIndex = 0;
								//    }
								//    break;
								//case "FacilityTreatingSpecialty":
								//    valueControl = new ComboBox();
								//    valueControl.Width = 100;
								//    valueBinding = new Binding("Params[" + item.Key.ToString() + "].Value");
								//    valueBinding.Source = Model;
								//    valueBinding.Mode = BindingMode.OneWayToSource;
								//    valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								//    valueControl.SetBinding(ComboBox.SelectedValueProperty, valueBinding);
								//    ((ComboBox)valueControl).ItemsSource = Model.TreatingSpecialities;
								//    ((ComboBox)valueControl).DisplayMemberPath = "Name";
								//    ((ComboBox)valueControl).SelectedValuePath = "Value";
								//    if (((ComboBox)valueControl).Items.Count > 0)
								//    {
								//        ((ComboBox)valueControl).SelectedIndex = 0;
								//    }
								//    break;
								//case "FacilityTreatingSpecialtyAll":
								//    valueControl = new ComboBox();
								//    valueControl.Width = 100;
								//    valueBinding = new Binding("Params[" + item.Key.ToString() + "].Value");
								//    valueBinding.Source = Model;
								//    valueBinding.Mode = BindingMode.OneWayToSource;
								//    valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								//    valueControl.SetBinding(ComboBox.SelectedValueProperty, valueBinding);
								//    ((ComboBox)valueControl).ItemsSource = Model.TreatingSpecialitiesAll;
								//    ((ComboBox)valueControl).DisplayMemberPath = "Name";
								//    ((ComboBox)valueControl).SelectedValuePath = "Value";
								//    if (((ComboBox)valueControl).Items.Count > 0)
								//    {
								//        ((ComboBox)valueControl).SelectedIndex = 0;
								//    }
								//    break;
								//case "Clinic":
								//    valueControl = new ComboBox ();
								//    valueControl.Width = 100;
								//    valueBinding = new Binding ("Params[" + item.Key.ToString () + "].Value");
								//    valueBinding.Source = Model;
								//    valueBinding.Mode = BindingMode.OneWayToSource;
								//    valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								//    valueControl.SetBinding (ComboBox.SelectedValueProperty, valueBinding);
								//    ((ComboBox)valueControl).ItemsSource = Model.HospitalLocations;
								//    ((ComboBox)valueControl).DisplayMemberPath = "Name";
								//    ((ComboBox)valueControl).SelectedValuePath = "Value";
								//    if (((ComboBox)valueControl).Items.Count > 0) {
								//        ((ComboBox)valueControl).SelectedIndex = 0;
								//    }
								//    break;
								case "PatientIENList":
									valueControl = new RadButton ();
									((RadButton)valueControl).Content = "Select...";
									((RadButton)valueControl).Command = SelectPatientForListCommand;
									selectPatientButton = valueControl as RadButton;
									valueControl.Padding = new Thickness (10, 5, 10, 5);
									RadGridView PatientListGrid = new RadGridView ();
									PatientListGrid.MaxWidth = 300;
									ScrollViewer.SetHorizontalScrollBarVisibility (PatientListGrid, ScrollBarVisibility.Disabled);
									PatientListGrid.AutoGenerateColumns = false;
									PatientListGrid.ShowGroupPanel = false;
									PatientListGrid.IsFilteringAllowed = false;
									PatientListGrid.CanUserDeleteRows = true;
									PatientListGrid.CanUserSelect = true;
									PatientListGrid.Deleted += new EventHandler<GridViewDeletedEventArgs> (PatientListGrid_Deleted);
									Binding patientNameBinding = new Binding ("Name");
									patientNameBinding.Mode = BindingMode.OneWay;
									patientNameBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
									PatientListGrid.Columns.Add (new Telerik.Windows.Controls.GridViewDataColumn () { Header = "Patient List", IsReadOnly = true, DataMemberBinding = patientNameBinding });
									PatientListGrid.Columns[0].Width = new GridViewLength (1, GridViewLengthUnitType.Star);
									PatientListGrid.VerticalAlignment = VerticalAlignment.Center;
									PatientListGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
									PatientListGrid.Margin = new Thickness (0, 5, 0, 0);
									patientListGridView = PatientListGrid;
									View.ParamControlsGrid.Children.Add (PatientListGrid);
									Grid.SetRow (PatientListGrid, View.ParamControlsGrid.RowDefinitions.Count - 1);
									Grid.SetColumn (PatientListGrid, 2);
									break;
								default:
									break;
								}
							} else {
								valueControl = new ComboBox ();
								valueControl.Width = 100;								
								Binding valueBinding = new Binding ("Params[" + item.Key.ToString () + "].Value");
								valueBinding.Source = Model;
								valueBinding.Mode = BindingMode.OneWayToSource;
								valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								valueControl.SetBinding (ComboBox.SelectedValueProperty, valueBinding);
								Binding itemsBinding = new Binding ("Params[" + item.Key.ToString () + "].PossibleValues");
								itemsBinding.Source = Model;
								itemsBinding.Mode = BindingMode.OneWay;
								itemsBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
								((ComboBox)valueControl).SetBinding(ComboBox.ItemsSourceProperty, itemsBinding);
								((ComboBox)valueControl).DisplayMemberPath = "Key";
								((ComboBox)valueControl).SelectedValuePath = "Value";
								if (((ComboBox)valueControl).Items.Count > 0) {
									((ComboBox)valueControl).SelectedIndex = 0;
								}
							}
						}

						keyControl.Content = item.Key.ToString ().InsertSpaces ();
						keyControl.Content += ":";
						Thickness margin = new Thickness (15, 5, 0, 0);
						valueControl.Margin = margin;
						keyControl.Margin = margin;
						valueControl.VerticalAlignment = VerticalAlignment.Center;
						keyControl.VerticalAlignment = VerticalAlignment.Center;
						valueControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
						keyControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

						View.ParamControlsGrid.Children.Add (keyControl);
						View.ParamControlsGrid.Children.Add (valueControl);
						Grid.SetRow (keyControl, View.ParamControlsGrid.RowDefinitions.Count - 1);
						Grid.SetRow (valueControl, View.ParamControlsGrid.RowDefinitions.Count - 1);
						Grid.SetColumn (keyControl, 0);
						Grid.SetColumn (valueControl, 1);
					}
				}
			}
		}

		void PatientListGrid_Deleted (object sender, GridViewDeletedEventArgs e)
		{
			foreach (string item in e.Items) {
				((List<string>)Model.Params["PatientIENList"].Value).Remove (item);
			}
		}	
		
		public void OnClose()
		{
			DisplayModel = container.Resolve<ReportsPresentationModel> ();
			DisplayModel.Model = Model;
			regionManager.Regions[RegionNames.TaskGroupRegion].Remove (View);
		}

		public DelegateCommand<string> OKCommand { get; private set; }
		public DelegateCommand<string> CancelCommand { get; private set; }
		public DelegateCommand<string> SelectPatientCommand { get; private set; }
		public DelegateCommand<string> SelectPatientForListCommand { get; private set; }

		public void ExecuteOKCommand (string title)
		{
			OnClose ();
		}

		public void ExecuteCancelCommand (string title)
		{
			regionManager.Regions[RegionNames.TaskGroupRegion].Remove (View);
		}

		public void ExecuteSelectPatientCommand (string name)
		{
			this.eventAggregator.GetEvent<TaskSelectReportPatientEvent> ().Subscribe (ReportPatientSelected, ThreadOption.UIThread, true);
			CompositePresentationEvent<ClinSchd.Infrastructure.Models.Patient> forwardingEvent = this.eventAggregator.GetEvent<TaskSelectReportPatientEvent> ();
			this.eventAggregator.GetEvent<LaunchPatientSelectionDialogNoHistoryEvent> ().Publish (forwardingEvent);
		}

		public void ExecuteSelectPatientForListCommand (string name)
		{
			this.eventAggregator.GetEvent<TaskSelectReportPatientEvent> ().Subscribe (ReportPatientSelectedForList, ThreadOption.UIThread, true);
			CompositePresentationEvent<ClinSchd.Infrastructure.Models.Patient> forwardingEvent = this.eventAggregator.GetEvent<TaskSelectReportPatientEvent> ();
			this.eventAggregator.GetEvent<LaunchPatientSelectionDialogNoHistoryEvent> ().Publish (forwardingEvent);
		}

		public bool CanExecuteOKCommand (string title)
		{
			return true;
		}

		public bool CanExecuteCancelCommand (string title)
		{
			return true;
		}

		public bool CanExecuteSelectPatientCommand (string name)
		{
			return true;
		}

		public bool CanExecuteSelectPatientForListCommand (string name)
		{
			return true;
		}

		public void ReportPatientSelected (Patient patient)
		{
			Model.Params["PatientIEN"].Value = patient.IEN;
			selectPatientButton.Visibility = Visibility.Collapsed;
			selectPatientLabel.Content = patient.Name;
			Model.PatientIEN = patient.IEN;
		}

		public void ReportPatientSelectedForList (Patient patient)
		{
			if (Model.Params["PatientIENList"].Value == null) {
				Model.Params["PatientIENList"].Value = new List<string> ();
			}
			bool patientAlreadySelected = false;
			foreach (string item in ((List<string>)Model.Params["PatientIENList"].Value)) {
				if (item.Split ("~".ToCharArray())[1] == patient.IEN) {
					patientAlreadySelected = true;
				}
			}
			if (!patientAlreadySelected) {
				((List<string>)Model.Params["PatientIENList"].Value).Add ("~" + patient.IEN);
				patientListGridView.Items.Add (patient);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler Handler = PropertyChanged;
			if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#region IReportParamsPresentationModel Members

		public IReportParamsView View { get; private set; }
		public ReportsModel Model { get; set; }
		public ReportsPresentationModel DisplayModel { get; set; }

		#endregion
	}
}