using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Navisworks References
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;

//External References
using IronPdf;

namespace MonkeyJobsNavisPlugIn
{
    [PluginAttribute("MonkeyJobs.Generate PDF Report",                   //Plugin name
                    "MNKJ",                                       //4 character Developer ID or GUID
                    ToolTip = "It converts Clash tests into a PDF Report",//The tooltip for the item in the ribbon
                    DisplayName = "Generate PDF Report")]          //Display name for the Plugin in the Ribbon

    public class GeneratePDFReport : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            MessageBox.Show(Autodesk.Navisworks.Api.Application.Gui.MainWindow, "Generate PDF Success!!!");
            return 0;
        }
    }
}
