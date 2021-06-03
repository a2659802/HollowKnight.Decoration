using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace DecorationMaster.Util
{
    public static class SerializeHelper
    {
        public static string DATA_DIR { get {
                string DIR;
                if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.MacOSX)
                {
                    DIR = Path.GetFullPath(Application.dataPath + "/Managed/Mods/DecorationMasterData");
                }
                else
                {
                    DIR = Path.GetFullPath(Application.dataPath + "/Resources/Data/Managed/Mods/DecorationMasterData");
                }
                if (!Directory.Exists(DIR))
                {
                    Directory.CreateDirectory(DIR);
                }
                return DIR;
            } }
        public static string GLOBAL_FILE_DIR => Path.Combine(DATA_DIR, "global.json");

        public static void SaveGlobalSettings(object data) => Serialize(data, GLOBAL_FILE_DIR);
        public static T LoadGlobalSettings<T>() => DeSerialize<T>(GLOBAL_FILE_DIR);
        public static void SaveSceneSettings(object data, string sceneName) => Serialize(data, Path.Combine(DATA_DIR, sceneName + ".json"));
        public static T LoadSceneSettings<T>(string sceneName) => DeSerialize<T>(Path.Combine(DATA_DIR, sceneName + ".json"));

        static void Log(object o) => Logger.Log($"[SerializeHelper]{o}");
        static void LogError(object o) => Logger.LogError($"[SerializeHelper]{o}");
    
        public static void Serialize(object o,string path)
        {
            JsonSerializerSettings currentSettings = new JsonSerializerSettings();
            currentSettings.Formatting = Formatting.Indented;
            currentSettings.TypeNameHandling = TypeNameHandling.All;
            currentSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            currentSettings.Converters.Add(new UnityStructConverter());

            using (FileStream fileStream = File.Create(path))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    try
                    {
                        writer.Write
                        (
                            JsonConvert.SerializeObject
                            (
                                o,
                                currentSettings
                            )
                        ) ;
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
            }
        }
        public static T DeSerialize<T>(string path)
        {
            if (!File.Exists(path))
                return default;

            JsonSerializerSettings currentSettings = new JsonSerializerSettings();
            currentSettings.Formatting = Formatting.Indented;
            currentSettings.TypeNameHandling = TypeNameHandling.All;
            
            currentSettings.Converters.Add(new UnityStructConverter());

            using (FileStream fileStream = File.OpenRead(path))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    string json = reader.ReadToEnd();
                    Type settingsType = typeof(T);
                    T settings = default;

                    try
                    {
                        settings = (T)JsonConvert.DeserializeObject(
                            json,
                            settingsType,
                            currentSettings
                        );
                    }
                    catch (Exception e)
                    {
                        LogError("Failed to load settings using Json.Net.");
                        LogError(e);
                    }

                    return settings;
                }
            }
        }
    }
}
