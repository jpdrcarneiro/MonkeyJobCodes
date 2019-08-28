using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MonkeyJobNodes
{
   public class DocOperations
   {
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
      /// <returns>Autodesk.Revit.DB.Element - Check Revit API documentation</returns>
      public static Autodesk.Revit.DB.Element ConvertToAPIElement(Revit.Elements.DirectShape directShape)
      {
         if (directShape == null)
         {
            return null;
         }

         int ID = directShape.Id;


         Autodesk.Revit.UI.UIApplication uiapp = RevitServices.Persistence.DocumentManager.Instance.CurrentUIApplication;
         Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
         Autodesk.Revit.DB.Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

         Autodesk.Revit.DB.ParameterSet parameterSet = new Autodesk.Revit.DB.ParameterSet();

         Autodesk.Revit.DB.ElementId elemId = new Autodesk.Revit.DB.ElementId(ID);

         Autodesk.Revit.DB.Element API_element = doc.GetElement(elemId);

         return API_element;
      }


   }

   
}
