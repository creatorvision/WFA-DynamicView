using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revoWorkbench
{
    class TableauAutomation
    {
        private string _tableauExePath;
        public TableauAutomation(string tableauExePath)
        {
            _tableauExePath = tableauExePath;
        }

        public void AutomateTableau(string activationKey, Userview tableauRegistrationModelData)
        {
            if (ActivateTableauDesktop(activationKey))
            {
                UpdateTableauRegistryValues(tableauRegistrationModelData);
                Process.Start(_tableauExePath);
            }
        }

        private bool ActivateTableauDesktop(string activationKey)
        {
            string args = "-activate " + activationKey;
            var proc = Process.Start(_tableauExePath, args);
            if (proc != null)
            {
                proc.WaitForExit();
                int result = proc.ExitCode;
                if (result == 0)
                    return true;
            }
            return false;
        }

        private void UpdateTableauRegistryValues(Userview tableauRegistrationModelData)
        {
            RegistryKey tableauRegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Tableau\\Registration", true);
            if (tableauRegistryKey != null)
            {
                tableauRegistryKey = tableauRegistryKey.CreateSubKey("Data");
                tableauRegistryKey.SetValue("company", tableauRegistrationModelData.Company, RegistryValueKind.String);
                tableauRegistryKey.SetValue("email", tableauRegistrationModelData.Email, RegistryValueKind.String);
                tableauRegistryKey.SetValue("first_name", tableauRegistrationModelData.FirstName, RegistryValueKind.String);
                tableauRegistryKey.SetValue("last_name", tableauRegistrationModelData.LastName, RegistryValueKind.String);
                tableauRegistryKey.Close();
                string args = "-register ";
                var proc = Process.Start(_tableauExePath, args);
                if (proc != null)
                {
                    proc.WaitForExit();
                    int result = proc.ExitCode;
                }
            }
        }
    }
}
