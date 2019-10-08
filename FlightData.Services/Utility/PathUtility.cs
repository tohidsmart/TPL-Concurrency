using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlightData.Services.Utility
{
    public static class PathUtility
    {
      
        public static string GetRootPath()
        {
            string rootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\.."));
            return rootPath;
        }

        public static string GetSubFolderPath(string relativePath)
        {
            string subFolderPath = Path.Combine(GetRootPath(), relativePath);
            return subFolderPath;
        }

        public static string RawFolder
        {
            get
            {
                return GetSubFolderPath(@"FlightData.Consumer\ProcessRoot\RAW");
            }
        }

        public static string ExceptionFolder
        {
            get
            {
                return GetSubFolderPath(@"FlightData.Consumer\ProcessRoot\Exception");
            }
        }

        public static string CuratedFolder
        {
            get
            {
                return GetSubFolderPath(@"FlightData.Consumer\ProcessRoot\Curated");
            }
        }

        public static string InputFolder
        {
            get
            {
                return GetSubFolderPath(@"FlightData.Consumer\ProcessRoot\Input");
            }
        }

        public static string LogFolder
        {
            get
            {
                return GetSubFolderPath(@"Logs");
            }
        }

    }
}
