using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using System.Globalization;

namespace ClinSchd.Infrastructure
{
	/// <summary>
	/// Extension Methods for ADT.
	/// </summary>
	public static class ExtensionMethods
	{

		#region DateConversion

		/// <summary>
		/// ConvertToDateTime
		/// </summary>
		/// <param name="value"></param>
		/// <param name="isInternal"></param>
		/// <param name="TruncateSeconds"></param>
		/// <param name="FixZeroTime"></param>
		/// <returns></returns>
		public static DateTime? ConvertToDateTime (this string value, bool isInternal, bool TruncateSeconds, bool FixZeroTime)
		{
			if (value == null)
				return null;

			if (value == string.Empty)
				return DateTime.MinValue;

			DateTime selectedDateTime = new DateTime ();
			if (isInternal) {
				if (value != string.Empty) {
					string[] parts = value.Split (".".ToCharArray ());
					string yearComponent = (int.Parse (parts[0].Substring (0, parts[0].Length - 4)) + 1700).ToString ();

					selectedDateTime = DateTime.Parse (parts[0].Substring (parts[0].Length - 4, 2) + "/" + parts[0].Substring (parts[0].Length - 2, 2) + "/" + yearComponent);
					if (parts.Length == 2) {
						string selectedTime;
						if (TruncateSeconds || parts[1].Length < 5) {
							parts[1] += "0000";
							selectedTime = parts[1].Substring (0, 2) + ":" + parts[1].Substring (2, 2);
							if (FixZeroTime)
								selectedTime = selectedTime.Replace ("00:00", "00:01");
						} else {
							parts[1] += "000000";
							selectedTime = parts[1].Substring (0, 2) + ":" + parts[1].Substring (2, 2) + ":" + parts[1].Substring (4, 2);
							if (FixZeroTime)
								selectedTime = selectedTime.Replace ("00:00:00", "00:00:01");
						}
						selectedDateTime = selectedDateTime.Add (TimeSpan.Parse (selectedTime));
					}
				}
			} else {
				value = value.Replace (":", "");
				string[] parts = value.Split ("@".ToCharArray ());
				if (parts.Length == 1) {
					selectedDateTime = Convert.ToDateTime (parts[0]);
				} else {
					string time = parts[1];
					if (FixZeroTime) {
						if (time == "0000")
							time = "0001";
						else if (time == "000000")
							time = "000001";
					}
					if (TruncateSeconds) {
						time = time.Substring (0, 2) + ":" + time.Substring (2, 2);
					} else {
						time = time.Substring (0, 2) + ":" + time.Substring (2, 2) + ":" + time.Substring (4, 2);
					}
					selectedDateTime = Convert.ToDateTime (parts[0] + " " + time);
				}
			}
			return selectedDateTime;
		}
	
		/// <summary>
		/// ConvertToExternalDateTimeFormat
		/// </summary>
		/// <param name="inputDate"></param>
		/// <param name="TruncateSeconds"></param>
		/// <param name="FixZeroTime"></param>
		/// <returns></returns>
		public static string ConvertToExternalDateTimeFormat (this DateTime inputDate, bool TruncateSeconds, bool FixZeroTime)
		{
			string formattedDate = string.Empty;
			string formattedTime;
			if (TruncateSeconds) {
				formattedTime = inputDate.ToString ("HH:mm", CultureInfo.InvariantCulture);
				if (FixZeroTime)
					formattedTime = formattedTime.Replace ("00:00", "00:01");
			} else {
				formattedTime = inputDate.ToString ("HH:mm:ss", CultureInfo.InvariantCulture);
				if (FixZeroTime)
					formattedTime = formattedTime.Replace ("00:00:00", "00:00:01");
			}
			formattedDate = inputDate.ToString ("MMM").ToUpper () + " " + inputDate.ToString ("dd") + ", " + inputDate.Year
				+ "@" + formattedTime;
			return formattedDate;
		}

		/// <summary>
		/// ConvertToInternalDateTimeFormat
		/// </summary>
		/// <param name="inputDate"></param>
		/// <param name="TruncateSeconds"></param>
		/// <param name="FixZeroTime"></param>
		/// <returns></returns>
		public static string ConvertToInternalDateTimeFormat (this DateTime inputDate, bool TruncateSeconds,bool FixZeroTime)
		{
			string formattedDate = string.Empty;
			string formattedTime;
			if (TruncateSeconds) {
				formattedTime = inputDate.ToString ("HHmm", CultureInfo.InvariantCulture);
				if (FixZeroTime)
					formattedTime = formattedTime.Replace ("0000", "0001");
			} else {
				formattedTime = inputDate.ToString ("HHmmss", CultureInfo.InvariantCulture);
				if (FixZeroTime)
					formattedTime = formattedTime.Replace ("000000", "000001");
			}
			formattedDate = (inputDate.Year - 1700).ToString () + inputDate.ToString ("MM") + inputDate.ToString ("dd") + "."
				+ formattedTime;
			return formattedDate;
		}

		#endregion

		///// <summary>
		///// Converts an IList of results into a validatable Dictionary(string, string). 
		///// </summary>
		///// <param name="obj"></param>
		///// <returns></returns>
		//public static Dictionary<string, string> MakeValidatable(this IList<MovementEventResult> obj)
		//{
		//    Dictionary<string, string> convertedList = new Dictionary<string, string>();
		//    foreach (IValidatableResult item in obj)
		//    {
		//        if (item.FieldName != null)
		//        {
		//            convertedList.Add(item.FieldName, item.ErrorText);
		//        }
		//        else if (item.ErrorText != null && item.ErrorText != string.Empty)
		//        {
		//            convertedList.Add("Error", item.ErrorText);
		//        }
		//    }
		//    return convertedList;
		//}

		///// <summary>
		///// Converts an IList of results into a validatable Dictionary(string, string). 
		///// </summary>
		///// <param name="obj"></param>
		///// <returns></returns>
		//public static Dictionary<string, string> MakeValidatable (this IList<EditAdmissionResult> obj)
		//{
		//    Dictionary<string, string> convertedList = new Dictionary<string, string> ();
		//    foreach (IValidatableResult item in obj)
		//    {
		//        if (item.FieldName != null)
		//        {
		//            convertedList.Add (item.FieldName, item.ErrorText);
		//        }
		//        else if (item.ErrorText != null && item.ErrorText != string.Empty)
		//        {
		//            convertedList.Add ("Error", item.ErrorText);
		//        }
		//    }
		//    return convertedList;
		//}

		///// <summary>
		///// Converts an IList of results into a validatable Dictionary(string, string). 
		///// </summary>
		///// <param name="obj"></param>
		///// <returns></returns>
		//public static Dictionary<string, string> MakeValidatable (this IList<EditDischargeResult> obj)
		//{
		//    Dictionary<string, string> convertedList = new Dictionary<string, string> ();
		//    foreach (IValidatableResult item in obj) {
		//        if (item.FieldName != null) {
		//            convertedList.Add (item.FieldName, item.ErrorText);
		//        } else if (item.ErrorText != null) {
		//            convertedList.Add ("Error", item.ErrorText);
		//        }

		//    }
		//    return convertedList;
		//}

		///// <summary>
		///// Converts an IList of results into a validatable Dictionary(string, string). 
		///// </summary>
		///// <param name="obj"></param>
		///// <returns></returns>
		//public static Dictionary<string, string> MakeValidatable (this IList<string> obj)
		//{
		//    Dictionary<string, string> convertedList = new Dictionary<string, string> ();
		//    foreach (string item in obj)
		//    {
		//        if (item != string.Empty)
		//            convertedList.Add("Error", item);
		//    }
		//    return convertedList;
		//}

		public static string InsertSpaces (this string obj)
		{
			string withSpaces = string.Empty;
			for (int i = 0; i < obj.Length - 1; i++) {
				withSpaces += obj[i];
				if (char.IsUpper (obj, i + 1) && obj[i].ToString () != " ") {
					withSpaces += " ";
				}
			}
			withSpaces += obj[obj.Length - 1];
			return withSpaces;
		}
	}
}
