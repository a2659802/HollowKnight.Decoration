using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DecorationMaster;
using UnityEngine;

namespace DecorationMaster.UI
{
    public class UserLicense
    {
        private static bool agreeLicense => DecorationMaster.instance.Settings.agreeLicense;
        public static void ShowLicense()
        {
            if(!agreeLicense)
            {
                string bundleN = "userlicense";
                AssetBundle ab = null;  // You probably want this to be defined somewhere more global.
                Assembly asm = Assembly.GetExecutingAssembly();
                foreach (string res in asm.GetManifestResourceNames()) 
                {
                    using (Stream s = asm.GetManifestResourceStream(res))
                    {
                        if (s == null) continue;
                        byte[] buffer = new byte[s.Length];
                        s.Read(buffer, 0, buffer.Length);
                        s.Dispose();
                        string bundleName = Path.GetExtension(res).Substring(1);
                        if (bundleName != bundleN) continue;
                        Logger.Log("Loading bundle " + bundleName);
                        ab = AssetBundle.LoadFromMemory(buffer); // Store this somewhere you can access again.
                    }
                }
                var _canvas = ab.LoadAsset<GameObject>("userlicense");
                UnityEngine.Object.Instantiate(_canvas);
                Logger.Log("Show User License");
            }
        }

    }
}
