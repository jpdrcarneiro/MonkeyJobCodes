using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Navisworks.Api.Plugins;

namespace MonkeyJobsNavisPlugIn
{
    [Plugin("MonkeyJobsNavis", "jpdrcarneiro", DisplayName = "Monkey Jobs")]
    public class MonkeyJobsPlugin:CommandHandlerPlugin
    {

        public override int ExecuteCommand(string name, params string[] parameters)
        {
            return 0;
        }
    }
}
