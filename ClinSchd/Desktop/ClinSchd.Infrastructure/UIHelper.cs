using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure
{
	public static class UIHelper
	{
		public static void FindChildren<T>(DependencyObject depObj, string childName, List<T> controlList)
		   where T : DependencyObject
		{
			// Confirm obj is valid.  
			if (depObj == null) return;

			// success case 
			if (depObj is T && (childName == null || ((FrameworkElement)depObj).Name == childName))
			{
				controlList.Add(depObj as T);
			}

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
				FindChildren<T>(child, childName, controlList);
			}
		}
	}
}
