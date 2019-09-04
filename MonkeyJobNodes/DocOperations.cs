using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

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
         multiOutput.Add("AppDocument", currentDoc);

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
      /// <returns name="ElementAPI">Autodesk.Revit.DB.Element - Check Revit API documentation</returns>
      [MultiReturn(new[] { "ElementID", "RevitApiElement", "BoundingBox", "CenterBoundingBox" }) ]
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

            //Autodesk.Revit.DB.Location elemLocation = API_element.Location;

            multiOut.Add("ElementID", ID);
            multiOut.Add("RevitApiElement", API_element);
            multiOut.Add("BoundingBox", bounding);
            multiOut.Add("CenterBoundingBox", centerPoint);

            return multiOut;
         }

      }
      public static DataTable GenerateDataTable(string[] Data, string Headers)
      {
         DataTable previousDataTable = new DataTable("previousData");
         previousDataTable.Clear();
         string[] oldHeaders = Headers.Split(',');
         foreach (string header in oldHeaders)
         {
            previousDataTable.Columns.Add(header);
         }
         for (int i = 0; i < Data.Length; i++)
         {
            string[] temp = Data[i].Split(',');
            DataRow _drow = previousDataTable.NewRow();
            for (int j = 0; j < temp.Length; j++)
            {
               _drow[oldHeaders[j]] = temp[j];
            }
            previousDataTable.Rows.Add(_drow);
         }
         //Console.Write(previousDataTable.ToString());		
         return previousDataTable;
      }
      public static string[] GetDataTableInfo(DataTable table)
      {
         List<string> data = new List<string>();
         data.Add("Number of Columns: ");
         data.Add(table.Columns.Count.ToString());
         data.Add("Number of rows: ");
         data.Add(table.Rows.Count.ToString());
         return data.ToArray();
      }
      public static DataTable MergedDataTables(DataTable table1, DataTable table2, bool preserveChanges, string primaryKey)
      {
         table1.PrimaryKey = new DataColumn[] { table1.Columns[primaryKey] };
         table2.PrimaryKey = new DataColumn[] { table2.Columns[primaryKey] };
         //create primary key		
         table1.Merge(table2, preserveChanges, MissingSchemaAction.Add);
         return table1;
      }


   }

   
}
