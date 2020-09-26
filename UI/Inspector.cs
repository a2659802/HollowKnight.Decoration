using DecorationMaster.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Modding;
using UnityEngine;

namespace DecorationMaster.UI
{
    public static class InspectPanel
    {
        private static CanvasPanel panel;
        public const int PageMax = 6;
        public static void BuildMenu(GameObject canvas)
        {
            var img_inspect_border = GUIController.Instance.images["InspectorBorder"];
            var img_back = GUIController.Instance.images["back"];
            panel = new CanvasPanel(canvas, new Texture2D(1, 1), new Vector2(1920 - img_inspect_border.width, (1080 - img_inspect_border.height) / 2), Vector2.zero, new Rect(0, 0, 1, 1));

            panel.AddPanel("background", img_back, Vector2.zero, Vector2.zero, new Rect(0, 0, img_back.width, img_back.height));
            panel.AddPanel("Border", img_inspect_border, Vector2.zero, Vector2.zero, new Rect(0, 0, img_inspect_border.width, img_inspect_border.height));
            panel.AddPanel("PropSpace", new Texture2D(1, 1), Vector2.zero, Vector2.zero, new Rect(0, 0, 1, 1));

        }
        public static void AddItem(Type t,string name,object val)
        {
            var img_prop_item = GUIController.Instance.images["InspectItem"];
            if (panel.GetPanel("PropSpace") == null)
                panel.AddPanel("PropSpace", new Texture2D(1, 1), Vector2.zero, Vector2.zero, new Rect(0, 0, 1, 1));

            int i;
            for(i=0;i<PageMax;i++)
            {
                if (panel.GetPanel("PropSpace").GetPanel($"Prop_{i}") == null)
                    break;
            }
            var propPanel = panel.GetPanel("PropSpace").AddPanel($"Prop_{i}", img_prop_item, new Vector2(0, i * img_prop_item.height), Vector2.zero, new Rect(0, 0, img_prop_item.width, img_prop_item.height));
            propPanel.AddText($"Prop_{i}", name, Vector2.zero, Vector2.zero, GUIController.Instance.arial,24,FontStyle.Bold);
            propPanel.AddSlider($"Prop_{i}", new Vector2(img_prop_item.width / 2, 0), Vector2.zero, new Rect(0, 0, img_prop_item.width / 2, img_prop_item.height));
        }
        public static void Update()
        {

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
                        InspectPanel.AddItem(propType, name, value);
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
