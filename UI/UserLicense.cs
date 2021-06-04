using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DecorationMaster;
using UnityEngine;
using UnityEngine.UI;

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
                AssetBundle ab = null;  
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
                        ab = AssetBundle.LoadFromMemory(buffer); 
                    }
                }
                var _canvas = ab.LoadAsset<GameObject>("userlicense");
                var panel = UnityEngine.Object.Instantiate(_canvas).transform.GetChild(0);
                if(Language.Language.CurrentLanguage() != Language.LanguageCode.ZH)
                {
                    panel.GetComponentInChildren<Text>().text = "No Charging\n No Paying \n Public\n\n This Mod is free, any level design whit it should not charge \n\n\n\nMod Author:a2659802";
                }
                {
                    var tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(0.8f, 0, 0, 0.4f));
                    tex.Apply();
                    new CanvasButton(panel.gameObject, "close", tex, new Vector2(1920 - 60, 15), Vector2.one * 15, new Rect(0, 0, 1, 1))
                        .AddClickEvent(_ =>
                        {
                            UnityEngine.Object.Destroy(panel.transform.parent.gameObject);
                        });
                }
                Logger.Log("Show User License");
            }
        }

    }
}
