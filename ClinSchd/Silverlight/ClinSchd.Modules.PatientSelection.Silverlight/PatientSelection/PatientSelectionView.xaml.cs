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
using System.Windows;
using System.Windows.Controls;

namespace ADT.Modules.PatientSelection.PatientSelection
{
    public partial class PatientSelectionView : UserControl, IPatientSelectionView
    {
		public PatientSelectionView()
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> ShowPatientList;

		public PatientSelectionPresentationModel Model
        {
            get
            {
				return this.DataContext as PatientSelectionPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		#region IPatientSelectionView Members

		public bool? DialogResult
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public event EventHandler Closed;

		public bool? ShowDialog()
		{
			throw new NotImplementedException();
		}

		public void Close()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
