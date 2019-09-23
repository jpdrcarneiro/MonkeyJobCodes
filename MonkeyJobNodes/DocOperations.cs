using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.UI;

namespace MonkeyJobNodes
{
   public class DocOperations
   {
      /// <summary>
      /// Get Essential documents to run nodes
      /// </summary>
      /// <returns></returns>
      [MultiReturn(new[] { "UiApplication", "Application", "UiDocument", "CurrentDocument" })]
      public Dictionary<string, object> docOperationsNeeds()
      {
         Autodesk.Revit.UI.UIApplication uiapp = RevitServices.Persistence.DocumentManager.Instance.CurrentUIApplication;
         Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
         Autodesk.Revit.UI.UIDocument uiDoc = uiapp.ActiveUIDocument;
         Autodesk.Revit.DB.Document currentDoc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;


         Dictionary<string, object> multiOutput = new Dictionary<string, object>();
         multiOutput.Add("UiApplication", uiapp);
         multiOutput.Add("Application", app);
         multiOutput.Add("UiDocument", uiDoc);
         multiOutput.Add("CurrentDocument", currentDoc);

         return multiOutput;
      }
      /// <summary>
      /// Open a Transaction
      /// </summary>
      /// <param name="currentDoc"></param>
      /// <param name="transactionName"></param>
      /// <returns></returns>
      public Transaction openTransaction(Autodesk.Revit.DB.Document currentDoc, string transactionName) {
         Transaction transaction = new Transaction(currentDoc, transactionName);
         return transaction;
      }
      /// <summary>
      /// Commit change to the document
      /// </summary>
      /// <param name="transaction">Output from openTransaction node</param>
      /// <param name="runIt"> True to run commit action</param>
      /// <returns></returns>
      public string CommitTransaction(Transaction transaction, bool runIt = false)
      {
         if (runIt == true)
         {
            try
            {
               transaction.Commit();
               return "Transaction Committed";
            }
            catch (Exception e)
            {
               string result = "Error: " + e.Message;
               return result;
            }
         }
         else
         {
            return "not set to run";
         }
         

      }
      /// <summary>
      /// This node open, save, and close a project or list of projects.
      /// </summary>
      /// <param name="filePath">The project of list of project paths</param>
      /// <param name="audit">Open in Audit mode (True or False); Default = False</param>
      /// <param name="detachFromCentral">Open detaching from central model (True or False); Default = True</param>
      /// <param name="deleteLinks">After the document is open, the code can remove all revit links (True or False); Default = False</param>
      /// <returns name = "result"> Shows which method used to save file. If an error occure, it returns the exception information </returns>
      public string OpenSaveCloseDocumentFile(string filePath, bool audit = false, bool detachFromCentral = true, bool deleteLinks = false)
      {
         Autodesk.Revit.UI.UIApplication uiapp = RevitServices.Persistence.DocumentManager.Instance.CurrentUIApplication;
         Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
         Autodesk.Revit.UI.UIDocument uiDoc;
         Autodesk.Revit.DB.Document appDoc;
         //string docTitle = string.Empty;
         //instantiate open options for user to pick to audit or not
         OpenOptions openOpts = new OpenOptions();
         SaveAsOptions saveAsOpts = new SaveAsOptions();
         saveAsOpts.OverwriteExistingFile = true;
         WorksharingSaveAsOptions worksharingSaveAs = new WorksharingSaveAsOptions();
         worksharingSaveAs.SaveAsCentral = true;
         Autodesk.Revit.UI.UISaveAsOptions saveOpts = new Autodesk.Revit.UI.UISaveAsOptions();
         saveOpts.ShowOverwriteWarning = false;
         openOpts.Audit = audit;



         if (detachFromCentral == false)
         {
            openOpts.DetachFromCentralOption = DetachFromCentralOption.DoNotDetach;
         }
         else
         {
            openOpts.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
         }

         //convert string to model path for open
         ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
         string result;
         try
         {
            appDoc = app.OpenDocumentFile(modelPath, openOpts);
            if (deleteLinks == true)
            {
               DeleteRevitLinks(appDoc);
            }
            if (appDoc.IsWorkshared == true)
            {
               saveAsOpts.SetWorksharingOptions(worksharingSaveAs);
               appDoc.SaveAs(filePath, saveAsOpts);
               appDoc.Close();
               result = "appDoc Closed - Workshare Save";
            }
            else
            {
               appDoc.SaveAs(filePath, saveAsOpts);
               appDoc.Close();
               result = "appDoc Closed - Regular Save";
            }



         }
         catch (Exception f)
         {
            result = f.ToString();

            try
            {
               //docOpened = app.OpenDocumentFile(modelPath, openOpts); Autodesk.Revit.ApplicationServices.Application.OpenDocumentFile(modelPath, openOpts);
               uiDoc = uiapp.OpenAndActivateDocument(modelPath, openOpts, false);
               //docOpened.SaveAs(filePath, saveOpts);
               uiDoc.SaveAs(saveOpts);
               uiDoc.SaveAndClose();
               //docOpened.Close(true);
               result = "UIapp Closed";
            }
            catch (Exception e)
            {
               result = result + "\n" + e.ToString();
            }

         }

         //nothing

         return result;
      }

      /// <summary>
      /// It is used inside OpenDocumentFile Node, not for Dynamo use;
      /// </summary>
      /// <param name="document"> Open Document file, Autodesk.Revit.DB.Document</param>
      /// <returns name="result"> Shows if the link removal worked or not</returns>
      public string DeleteRevitLinks(Autodesk.Revit.DB.Document document)//, Autodesk.Revit.UI.UIDocument uiDoc)
      {
         string result = "None";
         Autodesk.Revit.DB.FilteredElementCollector elements = new Autodesk.Revit.DB.FilteredElementCollector(document);
         Autodesk.Revit.DB.Element[] revitLinks = elements.OfCategory(BuiltInCategory.OST_RvtLinks).ToArray<Autodesk.Revit.DB.Element>();
           // Autodesk.Revit.DB.Element[] dwgLinks = elements.OfCategory(CADL).ToArray<Autodesk.Revit.DB.Element>();
         Transaction transaction = new Transaction(document, "DeleteLinks");
         if (revitLinks.Length > 0)
         {
            transaction.Start();
            for (int i = 0; i < revitLinks.Length; i++)
            {
               Autodesk.Revit.DB.ElementId elementId;
               //revitLinks[i].Unload(); 
               try
               {
                  elementId = revitLinks[i].Id;
               }
               catch (Exception)
               {
                  continue;
               }
               document.Delete(elementId);
               result = "success";
            }
            transaction.Commit();
         }
         else
         {
            result = "No link on the model";
         }
         return result;


      }

      /// <summary>
      /// Convert Dynamo Select Models Elements into Revit API elements
      /// </summary>
      /// <param name="directShape">Input from Select Model Elements</param>
      /// <returns>Autodesk.Revit.DB.Element - Check Revit API documentation</returns>
      [MultiReturn(new[] { "ElementID", "RevitApiElement", "BoundingBox", "CenterBoundingBox", "TempID" }) ]
      public static Dictionary<string,object> ConvertToAPIElement(Revit.Elements.DirectShape directShape)
      {
         if (directShape == null)
         {
            return null;
         }
         else
         {

            int ID = directShape.Id;


            Autodesk.Revit.UI.UIApplication uiapp = RevitServices.Persistence.DocumentManager.Instance.CurrentUIApplication;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Autodesk.Revit.DB.Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            //Autodesk.Revit.DB.ParameterSet parameterSet = new Autodesk.Revit.DB.ParameterSet();

            Autodesk.Revit.DB.ElementId elemId = new Autodesk.Revit.DB.ElementId(ID);

            Autodesk.Revit.DB.Element API_element = doc.GetElement(elemId);

            Dictionary<string, Object> multiOut = new Dictionary<string, object>();

            Autodesk.Revit.DB.BoundingBoxXYZ bounding = API_element.get_BoundingBox(doc.ActiveView);
            Autodesk.Revit.DB.XYZ centerPoint = (bounding.Max + bounding.Min) / 2;
            string[] values = Regex.Split(centerPoint.ToString(), @"\D+");
                string tempID = string.Empty;
                foreach(string value in values)
                {
                    tempID = tempID + value;
                }

            //Autodesk.Revit.DB.Location elemLocation = API_element.Location;

            multiOut.Add("ElementID", ID);
            multiOut.Add("RevitApiElement", API_element);
            multiOut.Add("BoundingBox", bounding);
            multiOut.Add("CenterBoundingBox", centerPoint);
            multiOut.Add("TempID", tempID);

            return multiOut;
         }

      }
      /// <summary>
      /// Convert a string data into dataTable; 
      /// </summary>
      /// <param name="Data"></param>
      /// <param name="Headers"></param>
      /// <param name="primaryKey"></param>
      /// <returns name="DataTable">Returns a data table</returns>
      public static DataTable GenerateDataTable(string[] Data, string Headers, string primaryKey)
      {
            if (Data == null)
            {
                return null;
            }
         DataTable DataTable = new DataTable("previousData");
         DataTable.Clear();
         string[] HeadersArray = Headers.Split(',');
            DataColumn[] keys = new DataColumn[HeadersArray.Length];
         foreach (string header in HeadersArray)
         {
                DataColumn column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = header;
                DataTable.Columns.Add(column);
                try
                {
                    if (HeadersArray.Any(primaryKey.Contains))
                    {
                        DataTable.PrimaryKey = new DataColumn[] { DataTable.Columns[primaryKey] };
                    }
                }
                catch (Exception e)
                {
                    showExceptionOnScreen(e);
                }

         }
         for (int i = 0; i < Data.Length; i++)
         {
            string[] temp = Data[i].Split(',');
            DataRow _drow = DataTable.NewRow();
            for (int j = 0; j < temp.Length; j++)
            {
               _drow[HeadersArray[j]] = temp[j];
            }
                try
                {
                    DataTable.Rows.Add(_drow);
                }
                catch (Exception e)
                {
                    showExceptionOnScreen(e);
                    continue;
                }
         }
            return DataTable;
         
      }
      /// <summary>
      /// Output the number of rows and columns to check the dataTable
      /// </summary>
      /// <param name="dataTable">Input from a C# DataTable</param>
      /// <returns name="dataTableInformation">Data table number of rows and columns </returns>
      public static string[] GetDataTableInfo(DataTable dataTable)
      {
         if (dataTable == null)
            {
                return null;
            }
         List<string> data = new List<string>();
         data.Add("Number of Columns: ");
         data.Add(dataTable.Columns.Count.ToString());
         data.Add("Number of rows: ");
         data.Add(dataTable.Rows.Count.ToString());
         return data.ToArray();
      }

      public static DataTable MergeDataTables(DataTable table1, DataTable table2, bool preserveChanges)
      {
         if (table1 == null || table2 == null)
            {
                return null;
            }
         //table1.PrimaryKey = new DataColumn[] { table1.Columns[primaryKey] };
         //create primary key		
         table1.Merge(table2, preserveChanges, MissingSchemaAction.AddWithKey);
         return table1;
      }
        public static string[] DataTableContent(DataTable dataTable)
        {
            if(dataTable == null)
            {
                return null;
            }
            List<string> content = new List<string>();
            foreach(DataRow row in dataTable.Rows)
            {
                object[] data = row.ItemArray;
                string temp = string.Empty;
                for(int i = 0; i < data.Length; i++)
                {
                    if (i == data.Length - 1)
                    {
                        temp = temp + data[i].ToString();
                    }
                    else
                    {
                        temp = temp + data[i].ToString() + ",";
                    }
                }
                content.Add(temp);
            }
            return content.ToArray();
        }

        private static void showExceptionOnScreen(Exception e) {
            string dialogTitle = e.GetType().ToString();
            string exceptionDetails = e.ToString();
            TaskDialog exception = new TaskDialog(dialogTitle);
            exception.MainInstruction = dialogTitle;
            exception.MainContent = exceptionDetails;
            exception.CommonButtons = TaskDialogCommonButtons.Close;
            exception.DefaultButton = TaskDialogResult.Close;
            TaskDialogResult tResult = exception.Show();


        }
        

   }

   
}
