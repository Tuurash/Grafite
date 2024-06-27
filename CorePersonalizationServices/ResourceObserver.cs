using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorePersonalizationServices;

public static class ResourceObserver
{

   public static string ReadFullFile(string path)
   {
      //get current path
      string currentDirectory = System.IO.Directory.GetCurrentDirectory();

      string file = System.IO.File.ReadAllText(path);
      return file;
   }
}
