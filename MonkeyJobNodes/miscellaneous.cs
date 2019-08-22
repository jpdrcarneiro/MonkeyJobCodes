using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


   }
}
