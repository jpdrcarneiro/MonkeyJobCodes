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
        /// <summary>
        /// This is a switch to drive data direction.
        /// </summary>
        /// <param name="data">Any Variable that you want to control its direction</param>
        /// <param name="isDebug">Boolean that determine direction</param>
        /// <returns></returns>
        [MultiReturn(new[] { "Debug", "Release" })]
        public static Dictionary<string, object> DataSwitch(object data, bool isDebug = false)
        {
            Dictionary<string, object> multiOutPut = new Dictionary<string, object>();
            if (isDebug == true)
            {
                multiOutPut.Add("Debug", data);
                multiOutPut.Add("Release", null);
            }
            else
            {
                multiOutPut.Add("Debug", null);
                multiOutPut.Add("Release", data);
            }
            return multiOutPut;

        }




    }
}
