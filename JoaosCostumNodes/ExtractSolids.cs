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

namespace JoaosCustomNodes
{

   public class ExtractSolids
   {
      public static Autodesk.Revit.DB.Solid[] GetGeometry(Revit.Elements.DirectShape directShapes, bool removeEmptyVolumes = false)
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

         GeometryElement temp = elem.get_Geometry(new Options());

         IEnumerator<GeometryObject> enumeratorTemp = temp.GetEnumerator();
         //temp.Dispose();
         while (enumeratorTemp.MoveNext())
         {
            GeometryObject geometryObject = enumeratorTemp.Current;
            Type type = geometryObject.GetType();
            var propertyInfo = geometryObject.GetType().GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var materialValue = propertyInfo.GetValue(0);
            if (type.Name == "Solid")
            {
               Autodesk.Revit.DB.Solid tempSolid = geometryObject as Autodesk.Revit.DB.Solid;
               Autodesk.Revit.DB.FaceArray tempFaces = tempSolid.Faces;
               //Autodesk.Revit.DB.Surface[] surfaces = new Autodesk.Revit.DB.Surface[tempFaces.Size];

               for (int i = 0; i < tempFaces.Size; i++)
               {
                  Autodesk.Revit.DB.Face face = tempFaces.get_Item(i);
                  //surfaces[i] = face.GetSurface();

               }
               //IEnumerable<Autodesk.Revit.DB.Surface> D_surfaces = (Autodesk.Revit.DB.Surface[])Enum.GetValues(typeof(Autodesk.Revit.DB.Surface));

               //Autodesk.DesignScript.Geometry.Surface D_face = Autodesk.DesignScript.Geometry.Surface.ByPerimeterPoints();
               //Autodesk.DesignScript.Geometry.Solid D_Solid = Autodesk.DesignScript.Geometry.Solid.ByJoinedSurfaces(D_surfaces as);
               if (tempSolid.Volume > 0 && removeEmptyVolumes == true)
               {
                  geomSolid.Add(tempSolid);

               }
               else
               {
                  geomSolid.Add(tempSolid);
               }
               tempSolid.Dispose();
               geometryObject.Dispose();
            }
         }
         enumeratorTemp.Dispose();


         return geomSolid.ToArray();
      }

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
         elemVolume = geomSolid.Volume;
         geomSolid.Dispose();
         return elemVolume;
      }
      public static Autodesk.DesignScript.Geometry.BoundingBox bboxSquare(Autodesk.Revit.DB.Solid solidGeom)
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

   }


}
