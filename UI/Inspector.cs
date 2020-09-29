using DecorationMaster.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Modding;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using UnityEngine.UI;
using MonoMod;
using MonoMod.RuntimeDetour;
using Mono.Cecil;
namespace DecorationMaster.UI
{
    public class InspectPanel
    {
        
        private static GameObject _canvas;
        public static GameObject Canvas
        {
            get
            {
                if (_canvas != null)
                {
                    return _canvas;
                }
                string bundleN = "canvas";
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
                _canvas = ab.LoadAsset<GameObject>("canvas");
                return _canvas;
            }
        }
        public GameObject Panel;
        private List<GameObject> PropPanels = new List<GameObject>();

        public InspectPanel()
        {
            var newCanvas = UnityEngine.Object.Instantiate(Canvas);
            var panel = newCanvas.transform.Find("Panel");
            var propPanel = panel.Find("PropPanel");
            Panel = panel.gameObject;
            var PropPanel = propPanel.gameObject;
            PropPanels.Add(PropPanel);

        }
        public void UpdateName(int idx, string name)
        {
            var PropPanel = PropPanels[idx];
            PropPanel.transform.Find("Name").GetComponent<Text>().text = name;
        }
        public void AppendPropPanel(float min = 0, float max = 1, UnityAction<float> listener = null)
        {
            var prefab = PropPanels[PropPanels.Count - 1];
            var propp = UnityEngine.Object.Instantiate(prefab);
            var idx = PropPanels.Count;
            propp.transform.SetParent(Panel.transform, false);
            var rect = propp.GetComponent<RectTransform>();
            var prefabRect = prefab.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -1 * idx * (prefabRect.rect.height));
            if (listener == null)
                listener = DefaultValueChange;
            PropPanels.Add(propp);

            AddListener(idx, listener);
            UpdateName(idx, "Clone1");
            UpdateSliderConstrain(idx, min, max);
            Logger.LogDebug($"Add another,{propp.transform.position},{prefab.transform.position}");
            //Canvas.PrintSceneHierarchyTree();
        }
        public void UpdateSliderConstrain(int idx, float min, float max)
        {
            var PropPanel = PropPanels[idx];
            var slider = PropPanel.transform.Find("Slider").GetComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
        }
        public void UpdateValue(int idx, float value)
        {
            var PropPanel = PropPanels[idx];
            var slider = PropPanel.transform.Find("Slider").GetComponent<Slider>();
            slider.value = value;
            PropPanel.transform.Find("Value").GetComponent<Text>().text = value.ToString();
        }
        public void AddListener(int idx, UnityAction<float> func)
        {
            var PropPanel = PropPanels[idx];
            var slider = PropPanel.transform.Find("Slider").GetComponent<Slider>();
            slider.onValueChanged.AddListener(func);
        }
        public static void DefaultValueChange(float val)
        {
            Logger.LogDebug($"Slider Value Change :{val}");
        }
    }
    class Inspector
    {
        public static readonly Dictionary<Type, Dictionary<string,PropertyInfo>> cache_prop = new Dictionary<Type, Dictionary<string,PropertyInfo>>();
        private static Item CurrentInspect = null;
        private static void _reflectProps(Type t, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            
            var propInfos =t.GetProperties(flags)
                .Where(x =>
                {
                    bool handflag = x.GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>().Any();
                    bool ignoreflag = x.GetCustomAttributes(typeof(InspectIgnoreAttribute), true).OfType<InspectIgnoreAttribute>().Any();
                    return handflag && (!ignoreflag);
                });
            foreach(var p in propInfos)
            {
                if(cache_prop.TryGetValue(t,out var npair))
                {
                    npair.Add(p.Name, p);
                }
                else
                {
                    var d = new Dictionary<string, PropertyInfo>();
                    d.Add(p.Name, p);
                    cache_prop.Add(t, d);
                }

                Logger.LogDebug($"Cache PropInfo:T:{t},name:{p.Name}");
            }
            Logger.LogDebug($"_reflectProp_resutl:{propInfos.ToArray().Length}");
        }
        public static void InspectProps(Item item)
        {
            Logger.LogDebug($"LogProp:{item.GetType()}");
            if (!cache_prop.ContainsKey(item.GetType()))
            {
                //Logger.LogDebug($"Type Not Found:{item.GetType()}");
                _reflectProps(item.GetType());
            }

            if (CurrentInspect != item)
            {
                CurrentInspect = item;
                if (cache_prop.TryGetValue(item.GetType(), out var itemProps))
                {
                    foreach (var kv in itemProps)
                    {
                        string name = kv.Key;
                        Type propType = kv.Value.PropertyType;
                        object value = kv.Value.GetValue(item, null);
                        LogProp(propType, name, value);

                    }
                }
            }

            
            
            Logger.LogDebug("=========================");
        }

        internal static void LogProp(Type t,string name,object value)
        {
            Logger.LogDebug($"[{t}]ItemProp:{name}:{value}");
        }
    }
}
