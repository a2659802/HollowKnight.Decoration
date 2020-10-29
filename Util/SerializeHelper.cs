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

        public static void SaveGlobalSettings(object data)
        {
            var settings = data;

            if (settings is null)
                return;

            //Log("Saving Global Settings");

            if (File.Exists(GLOBAL_FILE_DIR + ".bak"))
            {
                File.Delete(GLOBAL_FILE_DIR + ".bak");
            }

            if (File.Exists(GLOBAL_FILE_DIR))
            {
                File.Move(GLOBAL_FILE_DIR, GLOBAL_FILE_DIR + ".bak");
            }

            using (FileStream fileStream = File.Create(GLOBAL_FILE_DIR))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    try
                    {
                        writer.Write
                        (
                            JsonConvert.SerializeObject
                            (
                                settings,
                                Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.Auto,
                                }
                            )
                        );
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
            }

            

            
        }
        public static T LoadGlobalSettings<T>()
        {
            var _globalSettingsPath = GLOBAL_FILE_DIR;
            if (!File.Exists(_globalSettingsPath))
                return default;

            Log("Loading Global Settings");

            using (FileStream fileStream = File.OpenRead(_globalSettingsPath))
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
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto,
                            }
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
        static void Log(object o) => Logger.Log($"[SerializeHelper]{o}");
        static void LogError(object o) => Logger.LogError($"[SerializeHelper]{o}");
    
        public static void SaveSceneSettings(object data,string sceneName)
        {
            string dir = Path.Combine(DATA_DIR, sceneName);

            using (FileStream fileStream = File.Create(dir))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    try
                    {
                        writer.Write
                        (
                            JsonConvert.SerializeObject
                            (
                                data,
                                Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.Auto,
                                }
                            )
                        );
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
            }
        }
        public static T LoadSceneSettings<T>(string sceneName)
        {
            string dir = Path.Combine(DATA_DIR, sceneName);
            if (!File.Exists(dir))
                return default;

            Log($"Loading {sceneName} Settings");

            using (FileStream fileStream = File.OpenRead(dir))
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
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto,
                            }
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
