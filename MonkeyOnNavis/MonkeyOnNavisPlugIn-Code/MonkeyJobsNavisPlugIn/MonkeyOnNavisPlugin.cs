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
using HtmlAgilityPack;

namespace MonkeyOnNavis
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
               IronPdf.HtmlToPdf renderer = new HtmlToPdf();
               if (clashTest.Children.Count() <= 0) {
                  continue;
               }
               string clashTestName = clashTest.DisplayName;

               //Create new directory
               string resultDir = "D:\\Temp_Monkey\\" + clashTestName + ".pdf"; //ask user for input;
               //listOfDirs.Add(resultDir);
               System.IO.Directory.CreateDirectory("D:\\Temp_Monkey\\");
               //infoOut = infoOut + "\r\n" + clashTestName;

               List<HtmlAgilityPack.HtmlDocument> listHtmls =new List<HtmlAgilityPack.HtmlDocument>();//create list to add htmls
               List<IronPdf.PdfDocument> listPDFs = new List<IronPdf.PdfDocument>();

               foreach (Autodesk.Navisworks.Api.SavedItem issue in clashTest.Children)
               {

                  //The template copy method changed
                  //string tempName = resultDir + issue.DisplayName + ".html";
                  //listOfPaths.Add(tempName);
                  //infoOut = infoOut + "_Issues:" + issue.DisplayName + ";";
                  //duplicateTemplate(System.IO.Directory.GetCurrentDirectory() + "\\Templates\\NavisworksTemplate.html", tempName);

                  //HTML template open from string saved on function below
                  HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                  htmlDoc.LoadHtml(templateHtml());

                  try
                  {
                     htmlDoc.DocumentNode.SelectSingleNode("//body//td//img").SetAttributeValue("src", "newValue"); //Add input for company logo
                     htmlDoc.DocumentNode.SelectNodes("//body//td//img")[1].SetAttributeValue("src", "ClashImage"); //change for clash image path
                     htmlDoc.DocumentNode.SelectSingleNode("//body//td//h2").InnerHtml = Autodesk.Navisworks.Api.Application.MainDocument.FileName;
                     htmlDoc.DocumentNode.SelectSingleNode("//body//td//h3").InnerHtml = clashTestName;
                     htmlDoc.DocumentNode.SelectSingleNode("//body//td//h4").InnerHtml = "User Input"; //make to be the user input;
                     htmlDoc.DocumentNode.SelectNodes("//body//tr//td//p")[1].InnerHtml = ClashInfoHtml(issue); //Change string to the info to match clash
                  }
                  catch (Exception e)
                  {
                     MessageBox.Show(e.ToString());
                  }


                  //Append to the html list
                  listHtmls.Add(htmlDoc);

                  IronPdf.PdfDocument tempPDF = renderer.RenderHtmlAsPdf(htmlDoc.DocumentNode.OuterHtml);

                  listPDFs.Add(tempPDF);

                  //MessageBox.Show(htmlDoc.DocumentNode.OuterHtml);

                  //htmlDoc.Save(filePath);

               }
               //Combine htnl list into pdf and save

               IronPdf.PdfDocument finalPDF = IronPdf.PdfDocument.Merge(listPDFs);
               try
               {
                  finalPDF.SaveAs(resultDir);
               }
               catch (Exception e)
               {
                  MessageBox.Show("Error on Save As: " + e.ToString());
               }

               //move to prject location

            }


            //no need for this anymore !!
            //deleteFiles(listOfPaths.ToArray());
            //deleteDir(listOfDirs.ToArray());

            
         }
         else
         {
            MessageBox.Show(infoOut);
         }

         MessageBox.Show("Completed!");
         return 0;
        }

      public void duplicateTemplate(string templatePath, string tempPath) {
         //System.IO.File.Move(templatePath, tempPath);
         try
         {
            System.IO.File.Copy(templatePath, tempPath);
         }
         catch (Exception e) {
            MessageBox.Show(e.ToString());
         }
         
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


      public string templateHtml()
      {
         string template = @"<html>
<head>
<meta charset=""utf-8"">
<title>ClashReportTemplate</title>
</head>

<body>
	<table style=""width: 100%"",
		   border =""1"",
		   id = header>
		<tr>
		<td width=""210px"", height=""110px"", align=""left"">
			<img src=""CompanyLogo.png"",
				 style=""width:200px;height:100px;"",
				 align=""left"">
		</td>
		<td align=""center"">
			<h2 id=""modelName"">Model Name</h2>
			<h3 id=""clashTest""> Clash Test Name</h3>
		</td>
		<td width=""210px"", height=""110px"">
			<p align=""center"">Date: <span id=""date""></span><br>
				Time: <span id=""time""</span><br>
			</p>
			<h4 align=""center"">User: Add UserName</h4>
				<script>
					var dt = new Date();
					document.getElementById(""date"").innerHTML = dt.toLocaleDateString();	
					document.getElementById(""time"").innerHTML = dt.toLocaleTimeString();
				</script>
			
		</td>
		</tr>
	</table>
	<table style=""width: 100%"",
		   border =""1"",
		   id = ""ClashInfo"">
		<tr>
		<tr>
			<td id=""clashImage"", width=""400px"", height=""600px"", align=""left""><img src=""CompanyLogo.png"", width=""490px"", height=""590px""></td>
			<td id=clashData align=""right"", valign=""top"">
				<P align=""justify"">
					<b>Name:</b> Clash 1 <br>
					<b>Status: </b> New <br>
					<b>Level: </b> Level x <br>
					<b>Grid Intersection: </b><br>
					<b>Date Found: </b><br>
					<b>Assing to:</b> <br>
					<b>Approved by: </b> <br>
					<b>Approved on:</b> <br>
					<b>Comments: </b><br>
					Get comments from Clash detective
					
					
					
					
				</P>
			</td>
		</tr>
		
	</table>
</body>
</html>";

         return template; 
      }

      public string ClashInfoHtml(Autodesk.Navisworks.Api.SavedItem clash)
      {
         string clashStatus = string.Empty;
         string Z = string.Empty;
         string X = string.Empty;
         string Y = string.Empty;
         string date = string.Empty;
         string assignedTo = string.Empty;
         string approvedBy = string.Empty;
         string approvedOn = string.Empty;
         string comments = string.Empty;

         if (clash.GetType().ToString() == "Autodesk.Navisworks.Api.Clash.ClashResult")
         {
            Autodesk.Navisworks.Api.Clash.ClashResult clashResult = clash as Autodesk.Navisworks.Api.Clash.ClashResult;
            clashStatus = clashResult.Status.ToString();
            Z = clashResult.BoundingBox.Center.Z.ToString();
            Y = clashResult.BoundingBox.Center.Y.ToString();
            X = clashResult.BoundingBox.Center.X.ToString();
            date = clashResult.CreatedTime.ToString();
            assignedTo = clashResult.AssignedTo.ToString();
            approvedBy = clashResult.ApprovedBy.ToString();
            approvedOn = clashResult.ApprovedTime.ToString();
            Autodesk.Navisworks.Api.CommentCollection collection = clashResult.Comments;
            foreach (Autodesk.Navisworks.Api.Comment com in collection)
            {
               comments = comments + $" {com.CreationDate} : {com.Author} comment {com.Id} : {com.ToString()} ||<br>";
            }
         }
         else if (clash.GetType().ToString() == "Autodesk.Navisworks.Api.Clash.ClashResultGroup")
         {
            Autodesk.Navisworks.Api.Clash.ClashResultGroup clashGroup = clash as Autodesk.Navisworks.Api.Clash.ClashResultGroup;
            clashStatus = clashGroup.Status.ToString();
            Z = clashGroup.BoundingBox.Center.Z.ToString();
            Y = clashGroup.BoundingBox.Center.Y.ToString();
            X = clashGroup.BoundingBox.Center.X.ToString();
            date = clashGroup.CreatedTime.ToString();
            assignedTo = clashGroup.AssignedTo.ToString();
            approvedBy = clashGroup.ApprovedBy.ToString();
            approvedOn = clashGroup.ApprovedTime.ToString();
            Autodesk.Navisworks.Api.CommentCollection collection = clashGroup.Comments;
            foreach (Autodesk.Navisworks.Api.Comment com in collection)
            {
               comments = comments + $" {com.CreationDate.ToString()} : {com.Author.ToString()} comment {com.Id.ToString()} : {com.ToString()} ||<br>";
            }
         }
         else
         {
            clashStatus = clash.GetType().ToString();
            MessageBox.Show("Type Error: " + clash.GetType().ToString());
         }

         if (approvedOn == "1/1/1970 12:00:00 AM")
         {
            approvedOn = "N/A";
         }

         string clashName = clash.DisplayName;
         string clashInformation = $@"<p align='justify'> 
                                       <b> Name:</b> {clashName} <br>
                                       <b> Status: </b> {clashStatus}<br>
                                       <b> Height: </b> {Z} <br>
                                       <b> Location: </b>{X} x {Y}<br>
                                       <b> Date Found: </b>{date}<br>
                                       <b> Assing to:</b>{assignedTo}<br>
                                       <b> Approved by: </b>{approvedBy}<br>
                                       <b> Approved on:</b>{approvedOn}<br>
                                       <b> Comments: </b><br>{comments}
                                       </p>";


         return clashInformation;


      }

    }
   

}
