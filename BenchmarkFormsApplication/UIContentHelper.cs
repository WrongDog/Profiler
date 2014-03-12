using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace BenchmarkFormsApplication
{
    public class UIContentHelper
    {
        public static void Save(List<Type> controlTypes, Control parent)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (Control control in parent.Controls)
            {
                if (controlTypes.Contains(control.GetType()))
                {

                    config.AppSettings.Settings.Remove(control.Name);
                    config.AppSettings.Settings.Add(control.Name, control.Text);

                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                else
                {
                    Save(controlTypes, control);
                }
            }
        }
        public static void Load(List<Type> controlTypes, Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (controlTypes.Contains(control.GetType()))
                {
                   control.Text= ConfigurationManager.AppSettings[control.Name];

                }
                else
                {
                    Load(controlTypes, control);
                }
            }
        }
    }
}
