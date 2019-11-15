using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using System.Collections.Generic;
using System;


namespace MonkeyJobNodes
{
   /// <summary>
   /// Codes created to automize reptitive work that a monkey can do
   /// </summary>

   public class UnderDevelopment
   {
      /// <summary>
      /// Get Input from SelectModelElement and covert into Revit Solid or Dynamo Solid
      /// </summary>
      /// <param name="directShapes"> Select Model Element or Select Model Elements</param>
      /// <param name="removeEmptyVolumes"> Bollean true or false</param>
      /// <returns></returns>
      [MultiReturn(new[] { "RevitSolid", "DynamoSolid" })]
      public Dictionary<string, object> GetSolids(Revit.Elements.DirectShape directShapes, bool removeEmptyVolumes = false)
      {
         Autodesk.Revit.DB.Element elem;

         try
         {
            elem = directShapes.InternalElement;
         }
         catch
         {
            return null;
         }

         //Element[] selElements = elem as Element[];


         List<Autodesk.Revit.DB.Solid> geomSolid = new List<Autodesk.Revit.DB.Solid>();
         List<Autodesk.DesignScript.Geometry.Solid> dynSolids = new List<Autodesk.DesignScript.Geometry.Solid>();

         Autodesk.Revit.DB.GeometryElement temp = elem.get_Geometry(new Autodesk.Revit.DB.Options());

         IEnumerator<Autodesk.Revit.DB.GeometryObject> enumeratorTemp = temp.GetEnumerator();
         //temp.Dispose();
         while (enumeratorTemp.MoveNext())
         {
            Autodesk.Revit.DB.GeometryObject geometryObject = enumeratorTemp.Current;
            Type type = geometryObject.GetType();
            var propertyInfo = geometryObject.GetType().GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var materialValue = propertyInfo.GetValue(0);
            if (type.Name == "Solid")
            {
               Autodesk.Revit.DB.Solid tempSolid = geometryObject as Autodesk.Revit.DB.Solid;
               Autodesk.DesignScript.Geometry.Solid dynamoTempSolid = tempSolid.ToProtoType();
               //Revit.GeometryConversion.RevitToProtoSolid revitToProtoSolid = new RevitToProtoSolid(tempSolid);
               //Autodesk.Revit.DB.Surface[] surfaces = new Autodesk.Revit.DB.Surface[tempFaces.Size];

               if (tempSolid.Volume > 0 && removeEmptyVolumes == true)
               {
                  geomSolid.Add(tempSolid);
                  dynSolids.Add(dynamoTempSolid);

               }
               else if (removeEmptyVolumes == false)
               {
                  geomSolid.Add(tempSolid);
                  dynSolids.Add(dynamoTempSolid);
               }
               tempSolid.Dispose();
               geometryObject.Dispose();
            }
         }
         enumeratorTemp.Dispose();
         Autodesk.Revit.DB.Solid[] geomSolids = geomSolid.ToArray();
         Autodesk.DesignScript.Geometry.Solid[] dynSolids2 = dynSolids.ToArray();
         Dictionary<string, object> MultiOutPut = new Dictionary<string, object>
             {
                {"RevitSolid", geomSolids},
               {"DynamoSolid", dynSolids2}
             };

         return MultiOutPut;

      }


      /// <summary>
      /// Convert Revit API Solid into Dynamo Solid
      /// </summary>
      /// <param name="solid">Revit API solid</param>
      /// <returns></returns>
      public static Autodesk.DesignScript.Geometry.Solid ConvertToDynamoSolid(Autodesk.Revit.DB.Solid solid)
      {

         Autodesk.DesignScript.Geometry.Solid tempSolid = solid.ToProtoType();
         return tempSolid;
      }
      /// <summary>
      /// Extract volume and a Dynamo Bounding Box from a Revit API Solid
      /// </summary>
      /// <param name="geomSolid"> Revit API solid</param>
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

      /// <summary>
      /// Get Revit API Solid volume, reutrn double
      /// </summary>
      /// <param name="geomSolid"> Revit API solid</param>
      /// <returns name="Volume"> Revit API solid volume (double)</returns>
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
      /// <summary>
      /// Convert Revit API Solid in a Dynamo Bounding Box.
      /// </summary>
      /// <param name="solidGeom"> Revit API solid geometry</param>
      /// <returns name = "Bounding Box"> Dynamo Bounding Box</returns>
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
      public static Autodesk.Revit.DB.Category ElementCategory(Autodesk.Revit.DB.Element elem, string parameterName)
      {
         string result = "";
         if (elem.LookupParameter(parameterName) != null)
         {
            return null;
         }
         else
         {
            //Autodesk.Revit.DB.ElementId typeId = elem.GetTypeId();

            //Autodesk.Revit.UI.UIApplication uiapp = RevitServices.Persistence.DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //Autodesk.Revit.DB.Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            //Autodesk.Revit.DB.ElementType elementType = doc.GetElement(typeId) as Autodesk.Revit.DB.ElementType;
            //string familyName = elementType.FamilyName;

            Autodesk.Revit.DB.Category category = elem.Category;

            //Autodesk.Revit.DB.Parameter newParam = new Autodesk.Revit.DB.Parameter(parameterName);

            //elem.Parameters.Insert =

            return category;
         }

      }

      public static Dictionary<string, object> EqualElementsDataSets() {
         return null;
      }




   }

}


