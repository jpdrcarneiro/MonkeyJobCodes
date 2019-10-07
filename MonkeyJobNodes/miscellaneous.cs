using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using MonkeyJobNodes;

namespace MonkeyJobNodes
{
   public static class Miscellaneous
   {
      /// <summary>
      /// Check if a string is empty or not. Empty string == false; Use this to generate a boolean mask;
      /// </summary>
      /// <param name="stringCont"></param>
      /// <returns name="result">Returns false if the string is empty</returns>
      public static bool isStringEmpty(string stringCont)
      {
         if (stringCont.Length > 1)
         {
            return true;
         }
         else
         {
            return false;
         }

      }

      [MultiReturn(new[] { "BooleanMask", "ParameterSet" })]
      public static Dictionary<string, object> DoesProjectParameterExist(Revit.Elements.DirectShape elemDynamo, string paramName)
      //public static bool DoesProjectParameterExist(Revit.Elements.DirectShape elemDynamo, string paramName)
      {
         if (elemDynamo == null)
         {
            return null;
         }

         int ID = elemDynamo.Id;


         Autodesk.Revit.UI.UIApplication uiapp = RevitServices.Persistence.DocumentManager.Instance.CurrentUIApplication;
         Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
         Autodesk.Revit.DB.Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

         Autodesk.Revit.DB.ParameterSet parameterSet = new Autodesk.Revit.DB.ParameterSet();

         Autodesk.Revit.DB.ElementId elemId = new Autodesk.Revit.DB.ElementId(ID);

         Autodesk.Revit.DB.Element elem = doc.GetElement(elemId);

         parameterSet = elem.Parameters;

         //try
         //{          
         //   parameterset = elem.parameters;
         //}
         //catch (nullreferenceexception error)
         //{
         //   return null;
         //}

         string[] paramNames = GetAllPametersName(parameterSet);

         bool found = false;
         foreach (string name in paramNames)
         {
            if (name == paramName)
            {
               found = true;
            }
         }
         Dictionary<string, object> MultiOutPut = new Dictionary<string, object>();
         MultiOutPut.Add("ParameterSet", parameterSet);
         MultiOutPut.Add("BooleanMask", found);
         //return found;
         return MultiOutPut;
      }

      private static string[] GetAllPametersName(Autodesk.Revit.DB.ParameterSet parameterSet)
      {
         try
         {
            string[] allParametersName = new string[parameterSet.Size];
            int counter = 0;
            foreach (Autodesk.Revit.DB.Parameter param in parameterSet)
            {
               allParametersName[counter] = param.Definition.Name;
               counter++;
            }

            return allParametersName;
         }
         catch (NullReferenceException error)
         {
            string[] allParametersName = new string[1];
            allParametersName[0] = error.ToString();
            return allParametersName;
         }



      }
        /// <summary>
        /// Extract a list of ElementsID with the spacefied key-word on Structural Material
        /// </summary>
        /// <param name="CurrentDocument"></param>
        /// <param name="Material"></param>
        /// <returns name="ElementIDs">Returns ElementIDs as String</returns>
        public static string[] checkStructuralMaterial (Autodesk.Revit.DB.Document CurrentDocument, String Material)
        {
            Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(CurrentDocument);
            Autodesk.Revit.DB.LogicalOrFilter logicFilter = new Autodesk.Revit.DB.LogicalOrFilter(new Autodesk.Revit.DB.ElementIsElementTypeFilter(true), new Autodesk.Revit.DB.ElementIsElementTypeFilter(false));
            IEnumerable < Autodesk.Revit.DB.Element> elements = collector.WherePasses(logicFilter);
            List<string> elemList = new List<string>();
            //string content = string.Empty;
            //int count = 0;
            
            using (var elementsSequence = elements.GetEnumerator())
            {
                while (elementsSequence.MoveNext())
                {
                    Autodesk.Revit.DB.Element elem = elementsSequence.Current;
                    Autodesk.Revit.DB.ParameterSet parameterSet = elem.Parameters;
                    //content = content + elem.Parameters.ToString() + "\r\n"; 
                    //Autodesk.Revit.DB.ElementId[] matIDs = elem.GetMaterialIds(false).ToArray();
                    if (parameterSet.IsEmpty == false && elem.LookupParameter("Structural Material") != null)
                    {
                        if (elem.LookupParameter("Structural Material").AsValueString().Contains(Material))
                        {
                            string nameTemp = elem.LookupParameter("Structural Material").Definition.Name;
                            string valueTemp = elem.LookupParameter("Structural Material").AsValueString();
                            elemList.Add(elem.UniqueId.ToString());
                            //content = content + elem.Id.ToString() + ":" + elem.Name + "___" + nameTemp + ":" + valueTemp + "\r\n";
                        }
                        else if (elem.LookupParameter("Structural Material").AsValueString().Contains("Category"))
                        {
                            if (elem.Category.Material.LookupParameter("Structural Material") != null && elem.Category.Material.LookupParameter("Structural Material").AsValueString().Contains(Material))
                            {
                                string nameTemp = elem.Category.Material.LookupParameter("Structural Material").Definition.Name;
                                string valueTemp = elem.Category.Material.LookupParameter("Structural Material").AsValueString();
                                elemList.Add(elem.UniqueId.ToString());
                                //content = content + elem.Id.ToString() + ":" + elem.Name + "___" + nameTemp + ":" + valueTemp + "\r\n";
                            }

                        }
                        //count++;
                    }
                    else if( elem.GetType() is Autodesk.Revit.DB.Floor)
                    {
                        Autodesk.Revit.DB.Floor floor = elem as Autodesk.Revit.DB.Floor;
                        string floorType = floor.FloorType.ToString();
                        if (floorType.Contains(Material))
                        {
                            elemList.Add(elem.UniqueId.ToString());
                        }

                    }
                    else if (elem.GetType() is Autodesk.Revit.DB.Wall)
                    {
                        Autodesk.Revit.DB.Wall wall = elem as Autodesk.Revit.DB.Wall;
                        string wallType = wall.WallType.ToString();
                        if (wallType.Contains(Material))
                        {
                            elemList.Add(elem.UniqueId.ToString());
                        }
                    }
                    else
                    {
                        continue;
                    }
                    
                }
            }
            //DocOperations.showStringOnScreen("Material IDs", content);
            return elemList.ToArray();
        }

        public static string[] getAllElementsID (Autodesk.Revit.DB.Document currentDocument)
        {
            Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(currentDocument);
            Autodesk.Revit.DB.LogicalOrFilter logicFilter = new Autodesk.Revit.DB.LogicalOrFilter(new Autodesk.Revit.DB.ElementIsElementTypeFilter(true), new Autodesk.Revit.DB.ElementIsElementTypeFilter(false));
            IEnumerable<Autodesk.Revit.DB.Element> elements = collector.WherePasses(logicFilter);
            List<string> elemList = new List<string>();
            //int count = 0;

            using (var elementsSequence = elements.GetEnumerator())
            {
                while (elementsSequence.MoveNext())
                {
                    elemList.Add(elementsSequence.Current.UniqueId.ToString());
                }
            }
            return elemList.ToArray();
        }
        public static bool CanBeHidden(Revit.Elements.Element elem, Autodesk.Revit.DB.View view, Autodesk.Revit.DB.Document currentDoc)
        {
            string idUnique;
            try
            {
                idUnique = elem.UniqueId;
            }
            catch (Exception)
            {
                return false;
            }
            Autodesk.Revit.DB.Element element = currentDoc.GetElement(idUnique);

            if (element.CanBeHidden(view) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string GetElementType (Revit.Elements.Element elem, Autodesk.Revit.DB.Document currentDoc)
        {
            string idUnique;
            try
            {
                idUnique = elem.UniqueId;
            }
            catch (Exception)
            {
                return null;
            }
            Autodesk.Revit.DB.Element element = currentDoc.GetElement(idUnique);

            return element.GetType().ToString();
        }

        


   }
}
