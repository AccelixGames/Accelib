using System.Collections.Generic;
using System.IO;
using Accelib.Editor.Architecture;

namespace Accelib.Editor.Steamwork
{
    public static class DepotUtility
    {
        public static void CreateFile(string path, string content)
        {
            var dir = Path.GetDirectoryName(path);
            if(dir == null)
                throw new DirectoryNotFoundException();
            
            if (!Directory.Exists(dir)) 
                Directory.CreateDirectory(dir);
         
            File.WriteAllText(path, content);
        }
        
        public static string GetAppContent(string appId, string desc, string contentRoot, string buildOutput, string liveBranch, List<DepotConfig> depots)
        {
            var vdfContent = "AppBuild\n{\n";

            vdfContent += $"\t\"AppID\" \"{appId}\"\n";
            vdfContent += $"\t\"Desc\" \"{desc}\"\n";
            vdfContent += $"\t\"ContentRoot\" \"{contentRoot}\"\n";
            vdfContent += $"\t\"BuildOutput\" \"{buildOutput}\"\n";
            vdfContent += $"\t\"SetLive\" \"{liveBranch}\"\n";
            vdfContent += "\t\"Preview\" \"0\"\n";
            vdfContent += "\t\"Local\" \"\"\n";
            vdfContent += "\t\"Depots\"\n\t{\n";
            foreach (var depot in depots) 
                if(depot.includeInBuild)
                    vdfContent += $"\t\t\"{depot.depotID}\" \"depot_{depot.depotID}.vdf\"\n";
            vdfContent += "\t}\n}";

            return vdfContent;
        }
        
        public static string GetDepotContent(string appName, string depotId, string localPath)
        {
            var config = "DepotBuild\n{\n";

            config += $"\t\"DepotID\" \"{depotId}\"\n";
            config += "\t\"FileMapping\"\n\t{\n";
            config += $"\t\t\"LocalPath\" \"{localPath}\"\n";
            config += "\t\t\"DepotPath\" \".\"\n";
            config += "\t\t\"Recursive\" \"1\"\n";
            config += "\t}\n";
            config += "\t\"FileExclusion\" \"*.pdb\"\n";
            config += $"\t\"FileExclusion\" \"{appName}_BackUpThisFolder_ButDontShipItWithYourGame*\"\n";
            config += $"\t\"FileExclusion\" \"{appName}_BurstDebugInformation_DoNotShip*\"\n";
            config += "}\n";

            return config;
        }
    }
}