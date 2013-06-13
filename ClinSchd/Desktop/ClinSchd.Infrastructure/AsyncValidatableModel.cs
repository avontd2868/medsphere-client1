using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Controls;
using ClinSchd.Infrastructure.Interfaces;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;

namespace ClinSchd.Infrastructure
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class AsyncValidatableModel: DependencyObject, INotifyPropertyChanged, IDataErrorInfo
	{	
		/// <summary>
		/// 
		/// </summary>
		public AsyncValidatableModel ()
		{
			IsAsync = true;
		}

		#region Async

		public AsyncWrapper AsyncWrapper
		{
			get
			{
				try
				{
					return (AsyncWrapper)GetValue(AsyncWrapperProperty);
				}
				catch
				{
					return null;
				}
			}
			set
			{
				try
				{
					SetValue(AsyncWrapperProperty, value);
				}
				catch
				{
				}
			}
		}

		public static readonly DependencyProperty AsyncWrapperProperty =
			DependencyProperty.Register ("AsyncWrapper", typeof (ClinSchd.Infrastructure.Controls.AsyncWrapper), typeof (AsyncValidatableModel));

		WorkCompletedMethod externalCompletedMethod;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="args"></param>
		protected delegate void WorkMethod (object s, DoWorkEventArgs args);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="args"></param>
		public delegate void WorkCompletedMethod (object s, RunWorkerCompletedEventArgs args);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="workMethod"></param>
		/// <param name="workCompletedMethod"></param>
		protected void DoWorkAsync (WorkMethod workMethod, WorkCompletedMethod workCompletedMethod)
		{
			externalCompletedMethod = workCompletedMethod;
			if (IsAsync)
			{
				MakeBusy ();
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += new DoWorkEventHandler(workMethod);
				worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnWorkerCompleted);
				worker.RunWorkerAsync();
			}
			else
			{
				DoWorkEventArgs args = new DoWorkEventArgs (null);
				workMethod.Invoke(null, args);
				RunWorkerCompletedEventArgs completedArgs = new RunWorkerCompletedEventArgs (args.Result, null, args.Cancel);
				externalCompletedMethod.Invoke(null, completedArgs);
			}
		}

		private void MakeBusy ()
		{
			if (AsyncWrapper != null) {
				AsyncWrapper.Dispatcher.BeginInvoke (new Action (() => {
					AsyncWrapper.DisableScreen ();
				}));
			}
		}

		protected void OnWorkerCompleted (object s, RunWorkerCompletedEventArgs args)
		{
			if (externalCompletedMethod != null)
				externalCompletedMethod (s, args);
			MakeNotBusy (s, args);
		}

		private void MakeNotBusy (object s, RunWorkerCompletedEventArgs args)
		{
			if (AsyncWrapper != null) {
				AsyncWrapper.Dispatcher.BeginInvoke (new Action (() => {
					AsyncWrapper.EnableScreen ();
				}));
			}
		}
		#endregion

		#region Validation
		Dictionary<string, string> _errors = new Dictionary<string, string> ();
		public Dictionary<string, string> Errors
		{
			get { return _errors; }
			set
			{
				_errors = value;
				BuildErrorSummary ();
			}
		}

		//string _error_summary_header_template = "Please correct the following errors:\n";
		string _error_summary_header_template = string.Empty;
		public string ErrorSummaryHeaderTemplate
		{
			get { return _error_summary_header_template; }
			set { _error_summary_header_template = value; }
		}

		string _error_summary_item_template = "{0}: {1}\n";
		public string ErrorSummaryItemTemplate
		{
			get { return _error_summary_item_template; }
			set { _error_summary_item_template = value; }
		}


		/// <summary>
		/// Can be used for Save command binding to prohibit Save
		/// </summary>
		public bool HasErrors
		{
			get { return _errors.Count > 0; }
		}


		protected void AddError (string field, string error)
		{
			if (_errors.ContainsKey (field))
				_errors[field] = error;
			else
				_errors.Add (field, error);
			BuildErrorSummary ();
		}

		protected void ClearError (string field)
		{
			if (_errors.ContainsKey (field))
				_errors.Remove (field);
			BuildErrorSummary ();
		}

		public void ClearErrors ()
		{
			Error = string.Empty;
			Errors.Clear ();
		}

		/// <summary>
		/// Error summary
		/// </summary>
		public string Error { get; set; }
		
		/// <summary>
		/// Should async calls be made or not.
		/// </summary>
		public bool IsAsync { get; set; }

		/// <summary>
		/// IDataErrorInfo method for access to errors by the view.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string this[string name]
		{
			get
			{
				if (_errors.ContainsKey (name))
					return _errors[name];
				else
					return null;
			}
		}

		void BuildErrorSummary ()
		{
			if (_errors.Count () > 0) {
				Error = _error_summary_header_template;

				foreach (KeyValuePair<string, string> err in _errors)
					Error += string.Format (_error_summary_item_template, err.Key, err.Value);
				Error = Error.TrimEnd ("\n".ToCharArray ());
			} else
				Error = string.Empty;

			NotifyPropertyChanged ("Error");	// cause view to refresh error summary
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void NotifyPropertyChanged (string property_name)
		{
			if (PropertyChanged != null)
				PropertyChanged (this, new PropertyChangedEventArgs (property_name));
		}
		#endregion

		#endregion
	}
}
