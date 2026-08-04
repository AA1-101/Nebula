using UnityEngine;

namespace Nebula.Modules
{
    public static class ConfigurationManager
    {
        public static void Load()
        {
            string dataPath = Application.persistentDataPath;
            string nebulaDataPath = Path.Combine(dataPath, "Nebula-Data");
            string configPath = Path.Combine(nebulaDataPath, "config.json");

            if (!Directory.Exists(nebulaDataPath))
            {
                Directory.CreateDirectory(nebulaDataPath);
                Main.Logger.LogInfo($"Creating Data Folder at: {nebulaDataPath}");
            }
            if (!File.Exists(configPath))
            {
                File.Create(configPath);
                Main.Logger.LogInfo($"Creating config.json at: {configPath}");
                
            }              
                                           
        }
                
    }
}
