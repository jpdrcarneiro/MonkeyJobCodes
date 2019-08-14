using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Revit.Elements;
using Revit.GeometryConversion;
using RevitServices.Persistence;


namespace JoaosCustomNodes
{

   public class ExtractSolids
   {
      //[MultiReturn(new[] { "RevitSolid", "DynamoSolid" })]
      //public static Dictionary<Autodesk.Revit.DB.Solid[], Autodesk.DesignScript.Geometry.Solid[]> GetGeometry(Revit.Elements.DirectShape directShapes, bool removeEmptyVolumes = false)
      //{
      //   Autodesk.Revit.DB.Element elem;

      //   try
      //   {
      //      elem = directShapes.InternalElement;
      //   }
      //   catch
      //   {
      //      return null;
      //   }

      //   //Element[] selElements = elem as Element[];


      //   List<Autodesk.Revit.DB.Solid> geomSolid = new List<Autodesk.Revit.DB.Solid>();
      //   List<Autodesk.DesignScript.Geometry.Solid> dynSolids = new List<Autodesk.DesignScript.Geometry.Solid>();

      //   GeometryElement temp = elem.get_Geometry(new Options());

      //   IEnumerator<GeometryObject> enumeratorTemp = temp.GetEnumerator();
      //   //temp.Dispose();
      //   while (enumeratorTemp.MoveNext())
      //   {
      //      GeometryObject geometryObject = enumeratorTemp.Current;
      //      Type type = geometryObject.GetType();
      //      var propertyInfo = geometryObject.GetType().GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      //      var materialValue = propertyInfo.GetValue(0);
      //      if (type.Name == "Solid")
      //      {
      //         Autodesk.Revit.DB.Solid tempSolid = geometryObject as Autodesk.Revit.DB.Solid;
      //         Autodesk.DesignScript.Geometry.Solid dynamoTempSolid = tempSolid.ToProtoType();
      //         //Autodesk.Revit.DB.Surface[] surfaces = new Autodesk.Revit.DB.Surface[tempFaces.Size];

      //         if (tempSolid.Volume > 0 && removeEmptyVolumes == true)
      //         {
      //            geomSolid.Add(tempSolid);
      //            dynSolids.Add(dynamoTempSolid);

      //         }
      //         else if (removeEmptyVolumes == false)
      //         {
      //            geomSolid.Add(tempSolid);
      //            dynSolids.Add(dynamoTempSolid);
      //         }
      //         tempSolid.Dispose();
      //         geometryObject.Dispose();
      //      }
      //   }
      //   enumeratorTemp.Dispose();
      //   Autodesk.Revit.DB.Solid[] geomSolids = geomSolid.ToArray();
      //   Autodesk.DesignScript.Geometry.Solid[] dynSolids2 = dynSolids.ToArray();
      //       Dictionary<Autodesk.Revit.DB.Solid[], Autodesk.DesignScript.Geometry.Solid[]> MultiOutPut = new Dictionary<Autodesk.Revit.DB.Solid[], Autodesk.DesignScript.Geometry.Solid[]>
      //       {
      //          { "RevitSolid", geomSolids },
      //          {"DynamoSolid", dynSolids2 }
      //       };

      //      return MultiOutPut;

      //}

      public static Autodesk.DesignScript.Geometry.Solid ConvertToDynamoSolid(Autodesk.Revit.DB.Solid solid)
      {

         Autodesk.DesignScript.Geometry.Solid tempSolid = solid.ToProtoType();
         return tempSolid;
      }

      [MultiReturn(new[] { "Volume", "Bounding Box" })]
      public static Dictionary<string, object> GetVolumeAndBoundingBox(Autodesk.Revit.DB.Solid geomSolid)
      {
         double volume = GetGeometryVolumes(geomSolid);
         Autodesk.DesignScript.Geometry.BoundingBox boundingBox = bboxSquare(geomSolid);

         Dictionary<string, object> MultiOutPut = new Dictionary<string, object>
         {
            {"Volume", volume},
            {"Bounding Box", boundingBox }
         };
         return MultiOutPut;
      }


      public static double GetGeometryVolumes(Autodesk.Revit.DB.Solid geomSolid)
      {
         double elemVolume = new double();
         try
         {
            elemVolume = geomSolid.Volume;
            geomSolid.Dispose();
            return elemVolume;
         }

         catch
         {
            return 0.00;
         }
      }
      public static Autodesk.DesignScript.Geometry.BoundingBox bboxSquare(Autodesk.Revit.DB.Solid solidGeom)
      {
         try
         {
            Autodesk.Revit.DB.BoundingBoxXYZ bbox = solidGeom.GetBoundingBox();
            Autodesk.DesignScript.Geometry.Point bboxMin = Autodesk.DesignScript.Geometry.Point.ByCoordinates(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
            Autodesk.DesignScript.Geometry.Point bboxMax = Autodesk.DesignScript.Geometry.Point.ByCoordinates(bbox.Max.X, bbox.Max.Y, bbox.Max.Z);
            bbox.Dispose();
            Autodesk.DesignScript.Geometry.BoundingBox boundingBox = BoundingBox.ByCorners(bboxMin, bboxMax);
            bboxMin.Dispose();
            bboxMax.Dispose();
            return boundingBox;
         }
         catch
         {
            return null;
         }

      }

      public string OpenDocumentFile(string filePath, bool audit = false, bool detachFromCentral = true, bool deleteLinks = false)
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
            catch (Exception e) {
               result = result + "\n" + e.ToString();
            }

         }

            //nothing

         return result;
      }

      public string DeleteRevitLinks(Autodesk.Revit.DB.Document document)
      {
         string result = "None";
         Autodesk.Revit.DB.FilteredElementCollector elements = new Autodesk.Revit.DB.FilteredElementCollector(document).OfClass(typeof(Autodesk.Revit.DB.RevitLinkType));
         Autodesk.Revit.DB.FilteredElementCollector elements2 = new Autodesk.Revit.DB.FilteredElementCollector(document).OfClass(typeof(Autodesk.Revit.DB.RevitLinkInstance));
         Autodesk.Revit.DB.Element[] revitLinks = elements.ToElements().ToArray<Autodesk.Revit.DB.Element>();
         revitLinks = revitLinks + elements2.ToElements().ToArray<Autodesk.Revit.DB.Element>();
        if (revitLinks.Length > 0)
         {
            for (int i = 0; i < revitLinks.Length; i++)
            {
            //revitLinks[i].Unload();
            Autodesk.Revit.DB.ElementId elemId = revitLinks[i].Id;
               try
               {
                   document.Delete(elemId);
                    result = "success";
               }
               catch (Exception e)
               {
                     result = e.ToString();
               }
            }   
        }
        else
         {
                result = "No link on the model";
         }
         return result;


      }


   }
}


