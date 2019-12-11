using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

//Navisworks References
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api.ApplicationParts;


//External References
using IronPdf;

namespace MonkeyJobsNavisPlugIn
{
    [PluginAttribute("MonkeyJobs.Generate PDF Report",                   //Plugin name
                    "MNKJ",                                       //4 character Developer ID or GUID
                    ToolTip = "It converts Clash tests into a PDF Report",//The tooltip for the item in the ribbon
                    DisplayName = "GeneratePDF_Report")]          //Display name for the Plugin in the Ribbon
    [AddInPlugin(AddInLocation.AddIn)]
    //[AddInPlugin(AddInLocation.CurrentSelectionContextMenu)]

    public class GeneratePDFReport : AddInPlugin
    {
      
        public override int Execute(params string[] parameters)
        {
         Autodesk.Navisworks.Api.Document curDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;

         try
         {
            curDoc.OpenFile("C:\\Users\\jpdrc\\Desktop\\MSF_3D.nwd");
            // Test file address and set up: "C:\Users\jpdrc\Desktop\MSF_3D.nwd"

            
         }
         catch (Autodesk.Navisworks.Api.DocumentFileException e)
         {
            e.Message.ToString();

            MessageBox.Show(e.Message.ToString());
         }
            Autodesk.Navisworks.Api.Clash.DocumentClash clashDoc = curDoc.Clash as Autodesk.Navisworks.Api.Clash.DocumentClash;
            Autodesk.Navisworks.Api.Clash.DocumentClashTests documentClashTests = clashDoc.TestsData;
            //Autodesk.Navisworks.Api.SavedItem[] clashTestsInfo = documentClashTests.Tests.ToArray();
            string infoOut = string.Empty;
         if (documentClashTests.Tests.Count() > 0)
         {

            
            var listOfDirs = new List<String>();
            var listOfPaths = new List<String>();
            foreach (Autodesk.Navisworks.Api.Clash.ClashTest clashTest in documentClashTests.Tests)
            {
               string clashTestName = clashTest.DisplayName;
               string resultDir = "C:\\Temp_Monkey\\" + clashTestName + "\\";
               listOfDirs.Add(resultDir);
               System.IO.Directory.CreateDirectory(resultDir);
               infoOut = infoOut + "\r\n" + clashTestName;
               foreach (Autodesk.Navisworks.Api.SavedItem issue in clashTest.Children)
               {
                  string tempName = resultDir + issue.DisplayName + ".html";
                  listOfPaths.Add(tempName);
                  infoOut = infoOut + "_Issues:" + issue.DisplayName + ";";
                  //Fix temporary address
                  //MessageBox.Show();
                  //Navisworks is having issue with duplicate operation
                  duplicateTemplate(System.IO.Directory.GetCurrentDirectory() + "\\Templates\\NavisworksTemplate.html", tempName);
                  //Open Template
                  //var openedFile = System.IO.File.Open(System.IO.Directory.GetCurrentDirectory() + "\\Templates\\NavisworksTemplate.html");
                  //Add data
                  //Save as diferent file in temp file
                  //Convert html into pdf
                  //Append path info

               }
               //Combine pdfs into one file
               //move to prject location

            }
            deleteFiles(listOfPaths.ToArray());
            deleteDir(listOfDirs.ToArray());

            MessageBox.Show(infoOut);
         }
         else
         {
            MessageBox.Show(infoOut);
         }
            

            return 0;
        }

      public void duplicateTemplate(string templatePath, string tempPath) {
         System.IO.File.Move(templatePath, templatePath);
      }
      public void deleteFiles(string[] filetPaths)
      {
         foreach(string file in filetPaths)
         {
            System.IO.File.Delete(file);
         }
         
      }
      public void deleteDir(string[] directories) {
         foreach (string dir in directories) {
            System.IO.Directory.Delete(dir);
         }
      }

    }
   

}
