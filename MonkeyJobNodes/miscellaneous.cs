using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

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


   }
}
