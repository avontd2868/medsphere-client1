using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace InstallerCustomActions
{
	[RunInstaller(true)]
	public partial class Actions : Installer
	{
		public Actions () {
		}

		protected override void OnAfterInstall (System.Collections.IDictionary savedState) {
			base.OnAfterInstall (savedState);
		}

		public override void Commit (System.Collections.IDictionary savedState) {
			base.Commit (savedState);
		}

		public override void Install (System.Collections.IDictionary stateSaver) {

			base.Install (stateSaver);

			string path = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			string text = string.Empty;
//		MessageBox.Show ("1");
			string paramInstallToWorkstation = Context.Parameters["INSTALL_TO_WORKSTATION"].ToString ().Replace (" ", "");
			string paramInstallToServer = Context.Parameters["INSTALL_TO_SERVER"].ToString ().Replace (" ", "");
			string paramNamespace = Context.Parameters["NAMESPACE"].ToString ().Replace (" ", "");
			string paramServer = Context.Parameters["SERVER"].ToString ().Replace (" ", "");
			string paramPort = Context.Parameters["PORT"].ToString ().Replace (" ", "");
			string paramAccessCode = "";
			string paramVerifyCode = "";

			if (paramInstallToWorkstation == "1") {
				try {
					text = File.ReadAllText (path + @"\ClinSchd.exe.config.mask");
					text = text.Replace ("{SERVER}", paramServer).
						Replace ("{NAMESPACE}", paramNamespace).
						Replace ("{SERVER}", paramServer).
						Replace ("{PORT}", paramPort).
						Replace ("{ACCESS_CODE}", paramAccessCode).
						Replace ("{VERIFY_CODE}", paramVerifyCode).
						Replace ("{SERVICE_NAME}", "WS_PIMSOVID_" + paramNamespace);
					File.WriteAllText (path + @"\ClinSchd.exe.config", text);
				} catch (Exception) {
				}
			}

			if (paramInstallToServer == "1") {
				try {
					text = File.ReadAllText (path + @"\CreateWebServices.exe.config.mask");
					text = text.Replace ("{SERVER}", paramServer).
						Replace ("{NAMESPACE}", paramNamespace).
						Replace ("{SERVER}", paramServer).
						Replace ("{PORT}", paramPort).
						Replace ("{ACCESS_CODE}", paramAccessCode).
						Replace ("{VERIFY_CODE}", paramVerifyCode).
						Replace ("{SERVICE_NAME}", "WS_PIMSOVID_" + paramNamespace);
					File.WriteAllText (path + @"\CreateWebServices.exe.config", text);
				} catch (Exception) {
				}

				try {
					text = File.ReadAllText (path + @"\RecreateWebServices.bat.mask");
					text = text.Replace ("{SERVER}", paramServer).
						Replace ("{NAMESPACE}", paramNamespace).
						Replace ("{SERVER}", paramServer).
						Replace ("{PORT}", paramPort).
						Replace ("{ACCESS_CODE}", paramAccessCode).
						Replace ("{VERIFY_CODE}", paramVerifyCode).
						Replace ("{SERVICE_NAME}", "WS_PIMSOVID_" + paramNamespace);
					File.WriteAllText (path + @"\RecreateWebServices.bat", text);
				} catch (Exception) {
				}

				try {
					string parms =
						"\"" + path + "\\PIMSOVID.war\" " +
						"PIMSOVID " +
						"\"" + path + "\\context.xml.mask\" " +
						"\"" + paramNamespace + "\" " +
						"\"" + paramServer + "\" " +
						"\"" + paramPort + "\" " +
						"\"" + paramAccessCode + "\" " +
						"\"" + paramVerifyCode + "\" " +
						"1";
					File.WriteAllText (path + @"\WebServiceInstall.bat", "\"" + path + "\\CreateWebServices.exe\" " + parms);
					Process process = new Process ();
					process.StartInfo = new ProcessStartInfo (path + "\\WebServiceInstall.bat", parms);
					process.Start ();
					process.WaitForExit ();
				} catch (Exception) {
				}
			}

			try {
				text = File.ReadAllText (path + @"\TestPIMSLoginDlg.exe.config.mask");
				text = text.Replace ("{SERVER}", paramServer).
					Replace ("{NAMESPACE}", paramNamespace).
					Replace ("{SERVER}", paramServer).
					Replace ("{PORT}", paramPort).
					Replace ("{ACCESS_CODE}", paramAccessCode).
					Replace ("{VERIFY_CODE}", paramVerifyCode).
					Replace ("{SERVICE_NAME}", "WS_PIMSOVID_" + paramNamespace);
				File.WriteAllText (path + @"\TestPIMSLoginDlg.exe.config", text);
			} catch (Exception) {
			}

		}
	}
}
