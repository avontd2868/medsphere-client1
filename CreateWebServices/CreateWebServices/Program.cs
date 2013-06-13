using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;

using mscOVID.Domain;
using mscOVID.Domain.Mock;

using Microsoft.Win32;
using System.Windows.Forms;

namespace CreateWebServices
{
	class Program
	{
		static void Main (string[] args) {
			if (args.Length != 9) {
				Console.WriteLine ("CreateWebServices <source war> <dest war> <mask file> <namespace> <server> <port> <access> <verify> <delete previous 1=yes 0=no>");
				Environment.Exit (-1);
			}
			Helper.CreateWebServices (
				args[0].Replace ("\"", ""),
				args[1].Replace ("\"", ""),
				args[2].Replace ("\"", ""),
				args[3].Replace ("\"", ""),
				args[4].Replace ("\"", ""),
				args[5].Replace ("\"", ""),
				args[6].Replace ("\"", ""),
				args[7].Replace ("\"", ""),
				args[8].Replace ("\"", ""));
		}
	}

	public static class Helper
	{
		public static void CreateWebServices (
			string sourceWar,
			string destWar,
			string maskFile,
			string nameSpace,
			string server,
			string port,
			string access,
			string verify,
			string deletePrevious) {
			//MessageBox.Show("Attach debugger");
			string extension = Path.GetExtension (sourceWar);
			if (extension == string.Empty) {
				Console.WriteLine ("CreateWebServices <source war> <dest war> <mask file> <namespace> <server> <port> <access> <verify> <delete previous 1=yes 0=no>");
				Console.WriteLine ("                  <source> must have a file extension");
				Environment.Exit (-1);
			}

			string destWarPrefix = "WS_" + destWar;
			string tomcatPath = Environment.GetEnvironmentVariable ("CATALINA_HOME");
			if (tomcatPath == null || tomcatPath != string.Empty) {
				RegistryKey key = Registry.LocalMachine.OpenSubKey (@"SOFTWARE\Apache Software Foundation\Tomcat\6.0");
				if (key != null)
					tomcatPath = (string)key.GetValue ("InstallPath");
			}
			if (tomcatPath == null) {
				Console.WriteLine ("CreateWebServices <source war> <dest war> <mask file> <namespace> <server> <port> <access> <verify> <delete previous 1=yes 0=no>");
				Console.WriteLine ();
				Console.WriteLine (@"CATALINA_HOME environment variable or HKLM\SOFTWARE\Apache Software Foundation\Tomcat\6.0 registry key must be defined");
				Environment.Exit (-1);
			}

			if (deletePrevious == "1") {
				foreach (string file in Directory.GetFiles (tomcatPath + @"\webapps\", destWarPrefix + "*", SearchOption.TopDirectoryOnly)) {
					File.Delete (file);
				}
			}
			System.Threading.Thread.Sleep (8000);

			// Create a temporary web service to invoke the "SYSTEM STATUS" RPC on
			System.IO.File.Copy (sourceWar, tomcatPath + @"\webapps\" + destWarPrefix + "_" + nameSpace + extension, true);
			CreateService (maskFile, access, verify, tomcatPath, destWarPrefix, nameSpace, server, "|TCP|" + port);

			// Get the list of services
			OVIDDataTableRepository adt = new OVIDDataTableRepository ();
			List<string> parms = new List<string> ();
			DataTable table = null;
			for (int i = 0; i < 10; i++) {
				table = adt.callOVIDRPC ("BDGGSM SYSTEM STATUS", parms);
				if (table.Rows.Count > 0) {
					//adt.Logout();
					break;
				}
				System.Threading.Thread.Sleep (1000);
				table = null;
			}

			if (table != null) {
				// Copy over the war files
				foreach (DataRow row in table.Rows) {
					string cacheNamespace = GetCellString (row, "NAMESPACE");
					string routine = GetCellString (row, "ROUTINE");

					if (routine == "CIANBLIS") {
						// Copy web service war
						System.IO.File.Copy (sourceWar, tomcatPath + @"\webapps\" + destWarPrefix + "_" + cacheNamespace + extension, true);
					}
				}

				// Copy over the context.xml files
				foreach (DataRow row in table.Rows) {
					string protocolPort = GetCellString (row, "DEVICE");
					string cacheNamespace = GetCellString (row, "NAMESPACE");
					string routine = GetCellString (row, "ROUTINE");

					if (routine == "CIANBLIS") {
						CreateService (maskFile, access, verify, tomcatPath, destWarPrefix, cacheNamespace, server, protocolPort);
					}
				}
			}
		}

		private static void CreateService (
			string maskFile,
			string accessCode,
			string verifyCode,
			string tomcatPath,
			string destWarPrefix,
			string cacheNamespace,
			string server,
			string protocolPort) {
			// Wait for the web service directory to get built
			string configFile = tomcatPath + @"\conf\Catalina\localhost\" + destWarPrefix + "_" + cacheNamespace + @".xml";
			bool found = false;
			for (int i = 0; i < 30; i++) {
				// Check to see if the web service config file exists yet
				if (File.Exists (configFile)) {
					// Parse protocolPort
					string[] parts = protocolPort.Split ("|".ToCharArray ());
					if (parts.Length >= 3) {
						// Load in the mask file
						string text = File.ReadAllText (maskFile);
						text = text.Replace ("{SERVER}", server).
							Replace ("{PORT}", parts[2]).
							Replace ("{ACCESS_CODE}", accessCode).
							Replace ("{VERIFY_CODE}", verifyCode).
							Replace ("{NAMESPACE}", cacheNamespace);
						File.WriteAllText (configFile, text);
					}
					found = true;
					break;
				} else {
					System.Threading.Thread.Sleep (1000);
				}
			}
			if (!found) {
				Console.WriteLine ("Error: Could not find " + configFile);
				Environment.Exit (-1);
			}

			configFile = tomcatPath + @"\webapps\WS_" + destWarPrefix + "_" + cacheNamespace +
				@"\WEB-INF\classes\resources\log4j.properties";
			found = false;
			for (int i = 0; i < 30; i++) {
				// Check to see if the web service config file exists yet
				if (File.Exists (configFile)) {
					// Parse protocolPort
					string[] parts = protocolPort.Split ("|".ToCharArray ());
					if (parts.Length >= 3) {
						// Load in the mask file
						string text = File.ReadAllText (maskFile);
						text = text.Replace ("pims-ws-tilde-value-array", destWarPrefix);
						File.WriteAllText (configFile, text);
					}
					found = true;
					break;
				} else {
					System.Threading.Thread.Sleep (1000);
				}
			}
			if (!found) {
				Console.WriteLine ("Error: Could not find " + configFile);
				Environment.Exit (-1);
			}
		}

		// Safely extract a string from a DataRow
		private static string GetCellString (DataRow row, string name) {
			if (row.Table.Columns.Contains (name))
				return row[name].ToString ();
			else
				return string.Empty;
		}
	}
}