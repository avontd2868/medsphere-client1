using System;
using System.IO;
using System.Xml;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

using Microsoft.Practices.Unity;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;


#if !SILVERLIGHT
using System.Data;
using System.Collections.ObjectModel;
#endif

namespace ClinSchd.Modules.Reports.Helper_Classes
{
	/// <summary>
	/// 
	/// </summary>
	public class ReportsModel : AsyncValidatableModel, IReportsModel
	{
		#region Properties
		
		public List<ReportParameter> ParameterDictionary { get; set; }
		private IDataAccessService DataAccessService;

		public ReportParameterCreator reportParameterCreator { get; set; }
		public Dictionary<string,ReportParameter> Params { get; set; }
		public List<ReportParameter> HardCodedParams { get; set; }
		//public IList<NameValue> WardLocations { get { return DataAccessService.GetWardLocations(); } }
		//public IList<NameValue> TreatingSpecialities { get { return DataAccessService.GetTreatingSpecialities(); } }
		//public IList<NameValue> TreatingSpecialitiesAll { get { return DataAccessService.GetTreatingSpecialitiesAll(); } }
		//public IList<NameValue> Providers { get { return DataAccessService.GetProviders(); } }
		//public IList<NameValue> ProvidersAll { get { return DataAccessService.GetProvidersAll(); } }
		//public IList<NameValue> Religions { get { return DataAccessService.GetReligions(); } }
		//public IList<NameValue> HospitalLocations { get { return DataAccessService.GetClinicList(); } }
		public IList<NameValue> ServerPrinterList { get { return DataAccessService.GetServerPrinters(); } }
		//public ObservableCollection<Movement> PatientAdmissionsList { get; private set; }
		//public ObservableCollection<IncompleteChartModel.IncompleteChart> PatientIncompleteChartList { get; private set; }

		//private void LoadPatientAdmissions()
		//{
		//    PatientAdmissionsList.Clear();
		//    foreach (Movement item in DataAccessService.GetMovementList(PatientIEN, Enums.MovementTypes.Admission))
		//    {
		//        PatientAdmissionsList.Add(item);
		//    }
		//}

		//private void LoadIncompleteCharts()
		//{
		//    PatientIncompleteChartList.Clear();
		//    foreach (IncompleteChartModel.IncompleteChart item in DataAccessService.
		//        GetIncompleteChartList(PatientIEN))
		//    {
		//        PatientIncompleteChartList.Add(item);
		//    }
		//}

		private string reportName;
		public string ReportDisplayName
		{
			get
			{
				return reportName;
			}
			set
			{
				reportName = value;
				InitializeReport();
			}
		}

		private string patientIEN;
		public string PatientIEN
		{
			get
			{
				return patientIEN;
			}
			set
			{
				patientIEN = value;
				//LoadPatientAdmissions ();
			}
		}

		public string ReportType { get; set; }

		/// <summary>
		/// Gets the name of the RPC that retrieves the selected report.
		/// </summary>
		public string ReportRPC { get; set; }

		/// <summary>
		/// Gets the list of required parameters for the selected report.
		/// </summary>
		public List<ReportParameter> RequiredParams { get; private set; }

		private bool reportTextIsStale = true;
		private string reportText;
		/// <summary>
		/// Gets the full text of the selected report.
		/// </summary>
		public string ReportText
		{
			get
			{
				if (reportTextIsStale) {
					if (reportText == null && this.ReportType != null) {
						DoWorkAsync ((s, args) => {
							reportText = DataAccessService.GetReportText(this.ReportRPC, this.GetRPCParams());
						}, (s, args) => {
							ReportText = reportText;
							reportTextIsStale = false;
							NotifyPropertyChanged ("ReportText");
						});
						return string.Empty;
					} else {
						return null;
					}
				} else {
					reportTextIsStale = true;
					return reportText;
				}
			}
			private set
			{
				reportText = value;
			}
		}

		private List<List<string>> GetRPCParams ()
		{
			List<List<string>> parms = new List<List<string>> ();
			foreach (ReportParameter item in this.RequiredParams) {
				if (!this.Params.ContainsKey (item.Description)) {
					throw new ArgumentException ("Required parameter " + item.ToString () + " was not passed.");
				}
				if (!(this.Params[item.Description].Value is List<string>)) {
					List<string> newList = new List<string> ();
					newList.Add (Params[item.Description].Value as string);
					parms.Add (newList);
				} else {
					parms.Add (Params[item.Description].Value as List<string>);
				}
			}
			return parms;
		}

		#endregion

		#region Methods

		public ReportsModel(
			IDataAccessService _DataAccessService
		)
		{
			DataAccessService = _DataAccessService;
			reportParameterCreator = new ReportParameterCreator(this);
			//PatientAdmissionsList = new ObservableCollection<Movement>();
			//PatientIncompleteChartList = new ObservableCollection<IncompleteChartModel.IncompleteChart>();
		}

		private void InitializeReport ()
		{
			ReportLoader.LoadReport (this);
		}

		public void InjectManualMappings (string propertyChanged)
		{
			switch (propertyChanged) {
			case "VisitType":
				Params["SortType"].PossibleValues = new ObservableDictionary<string, string> ();
				Params["SortType"].PossibleValues.Add ("Authorizing Provider", "1");
				Params["SortType"].PossibleValues.Add ("Case Manager", "2");
				Params["SortType"].PossibleValues.Add ("Community", "4");
				Params["SortType"].PossibleValues.Add ("Date Expected", "5");
				Params["SortType"].PossibleValues.Add ("Patient Name", "6");
				Params["SortType"].PossibleValues.Add ("Visit Disposition", "8");				
				switch (Params["VisitType"].Value as string) {
				case "A":
					Params["SortType"].PossibleValues.Add ("Service", "7");
					Params["SortType"].PossibleValues.Add ("Ward", "9");
					break;
				case "D":
					Params["SortType"].PossibleValues.Add ("Service", "7");
					break;
				case "O":
					Params["SortType"].PossibleValues.Add ("Clinic", "3");
					break;
				default:
					break;
				}
				Params["SubSortType"].PossibleValues = Params["SortType"].PossibleValues;
				NotifyPropertyChanged ("Params");
				break;				
			case "ReportToDisplay":
				if (Params.ContainsKey ("ReportTypeSort")) {
					Params["ReportTypeSort"].PossibleValues = new ObservableDictionary<string, string> ();
					switch (Params["ReportToDisplay"].Value as string) {
					case "1":
					case "2":
					case "3":
					case "5":
					case "7":
					default:
						Params["ReportTypeSort"].PossibleValues.Add ("By DATE Only", "1");
						Params["ReportTypeSort"].PossibleValues.Add ("By WARD", "2");
						Params["ReportTypeSort"].PossibleValues.Add ("By SERVICE", "3");
						Params["ReportTypeSort"].PossibleValues.Add ("By ADMITTING Provider", "4");
						Params["ReportTypeSort"].PossibleValues.Add ("By Provider's SERVICE", "5");
						Params["ReportTypeSort"].PossibleValues.Add ("By Community", "6");
						Params["ReportTypeSort"].PossibleValues.Add ("By Service Unit", "7");
						Params["ReportTypeSort"].PossibleValues.Add ("By Patient Name", "8");
						break;
					case "4":
						Params["ReportTypeSort"].PossibleValues.Add ("TRANSFERS to ICU", "1");
						Params["ReportTypeSort"].PossibleValues.Add ("RETURNS to ICU", "2");
						break;
					case "6":
						Params["ReportTypeSort"].PossibleValues.Add ("LISTING Only", "1");
						Params["ReportTypeSort"].PossibleValues.Add ("STATISTICS Only", "2");
						Params["ReportTypeSort"].PossibleValues.Add ("BOTH Listings and Stats", "3");
						break;
					}
					NotifyPropertyChanged ("Params");
				}
				break;
			default:
				break;
			}		
		}	

		#endregion

		public static class ReportLoader
		{
			private static XmlDocument ReadInConfigFile ()
			{
				XmlDocument ConfigXMLDoc = new XmlDocument ();
				string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames ();
				System.IO.Stream xmlStream = Assembly.GetExecutingAssembly().
					GetManifestResourceStream("ClinSchd.Modules.Reports.ReportsConfiguration.xml");
				XmlTextReader Input = new XmlTextReader(xmlStream);
				Input.WhitespaceHandling = WhitespaceHandling.None;
				ConfigXMLDoc.Load (Input);
				return ConfigXMLDoc;
			}

			private static List<ReportParameter> GetParameterDictionary (XmlDocument ConfigXMLDoc, ReportParameterCreator ReportParameterCreator)
			{
				List<ReportParameter> ParameterDictionary = new List<ReportParameter> ();
				foreach (XmlNode item in ConfigXMLDoc["ReportsConfiguration"].ChildNodes) {
					switch (item.Name) {
					case "Parameters":
						foreach (XmlNode parameterNode in item.ChildNodes) {
							string parameterName = parameterNode.Attributes["Name"].Value;
							ReportParameter newParam = ReportParameterCreator.GetNewWrapper (null, parameterName);
							newParam.PossibleValues = new ObservableDictionary<string, string> ();
							if (parameterNode.Attributes["ValueType"] != null) {
								newParam.ValueType = parameterNode.Attributes["ValueType"].Value;
							}
							foreach (XmlNode parameterValuesNode in parameterNode.ChildNodes) {
								if (parameterValuesNode.FirstChild != null) {
									newParam.PossibleValues.Add (parameterValuesNode.Attributes["ID"].Value, parameterValuesNode.FirstChild.Value);
								}
							}
							ParameterDictionary.Add (newParam);						
						}
						break;
					default:
						break;
					}
				}
				return ParameterDictionary;
			}

			public static void LoadReport (ReportsModel model)
			{
				XmlDocument ConfigXMLDoc = ReadInConfigFile ();
				model.ParameterDictionary = GetParameterDictionary (ConfigXMLDoc, model.reportParameterCreator);
				model.HardCodedParams = new List<ReportParameter> ();
				model.RequiredParams = new List<ReportParameter> ();

				foreach (XmlNode item in ConfigXMLDoc["ReportsConfiguration"].ChildNodes) {
					switch (item.Name) {
					case "Reports":
						foreach (XmlNode reportNode in item.ChildNodes) {
							if (reportNode.Attributes["DisplayName"].Value == model.ReportDisplayName) {
								model.ReportType = reportNode.Attributes["ID"].Value;
								model.ReportRPC = reportNode.Attributes["RPC"].Value;
								foreach (XmlNode reportParametersNode in reportNode.ChildNodes) {
									switch (reportParametersNode.Name) {
									case "RequiredParams":
										foreach (XmlNode requiredParamNode in reportParametersNode.ChildNodes) {
											foreach (ReportParameter requiredParam in model.ParameterDictionary) {
												if (requiredParam.Description == requiredParamNode.FirstChild.Value) {
													model.RequiredParams.Add (requiredParam);
												}
											}
										}
										break;
									case "HardCodedParams":
										foreach (XmlNode hardCodedParamNode in reportParametersNode.ChildNodes) {
											model.HardCodedParams.Add (model.reportParameterCreator.GetNewWrapper (hardCodedParamNode.FirstChild.Value, hardCodedParamNode.Attributes["ID"].Value));
										}
										break;
									default:
										break;
									}
								}
							}
						}
						break;
					default:
						break;
					}
				}

				InitializeParams (model);
			}

			private static void InitializeParams (ReportsModel model)
			{
				model.Params = new Dictionary<string, ReportParameter> ();
				foreach (ReportParameter item in model.RequiredParams) {
					model.Params.Add (item.Description, item);
				}
				foreach (ReportParameter item in model.HardCodedParams) {
					model.Params[item.Description].Value = item.Value;
				}
			}
		}
	}
}