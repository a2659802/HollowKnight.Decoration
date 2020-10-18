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
using MonoMod.RuntimeDetour;
using HutongGames.PlayMaker;
using DecorationMaster.MyBehaviour;

namespace DecorationMaster.UI
{
    public class InspectPanel
    {
        
        private static GameObject _canvas;
        private static GameObject Canvas
        {
            get
            {
                if (_canvas != null)
                {
                    return _canvas;
                }
                string bundleN = "canvas2";
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
                _canvas = ab.LoadAsset<GameObject>("canvas2");
                return _canvas;
            }
        }
        private GameObject Panel;
        private List<GameObject> PropPanels = new List<GameObject>();
        private GameObject CurrentCanvas;

        public InspectPanel()
        {

            CurrentCanvas = UnityEngine.Object.Instantiate(Canvas);
            var panel = CurrentCanvas.transform.Find("Panel");
            var propPanel = panel.Find("PropPanel");
            Panel = panel.gameObject;
            var PropPanel = propPanel.gameObject;
            PropPanels.Add(PropPanel);
            PropPanel.transform.Find("Name").GetComponent<Text>().fontSize = 22;
            PropPanel.transform.Find("Name").GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
            PropPanel.transform.Find("Name").GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            PropPanel.transform.Find("Value").GetComponent<InputField>().textComponent.fontSize = 18;
            PropPanel.transform.Find("Value").GetComponent<InputField>().textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            PropPanel.transform.Find("Value").GetComponent<InputField>().textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            UpdateTextDelegate(0);//AddListener(0, UpdateTextDelegate(0));

            UnityEngine.Object.DontDestroyOnLoad(CurrentCanvas);

        }
        public void UpdateName(int idx, string name)
        {
            var PropPanel = PropPanels[idx];
            PropPanel.transform.Find("Name").GetComponent<Text>().text = name;
        }
        public void AppendPropPanel(string name,float min = 0, float max = 1, UnityAction<float> listener = null)
        {
            var prefab = PropPanels[PropPanels.Count - 1];
            var propp = UnityEngine.Object.Instantiate(prefab);
            var idx = PropPanels.Count;
            propp.transform.SetParent(Panel.transform, false);
            var rect = propp.GetComponent<RectTransform>();
            var prefabRect = prefab.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -1 * idx * (prefabRect.rect.height));
            //if (listener == null)
            //    listener = DefaultValueChange;
            PropPanels.Add(propp);

            AddListener(idx, listener);
            UpdateName(idx, name);
            UpdateSliderConstrain(name,idx, min, max);
            Logger.LogDebug($"Add another,{propp.transform.position},{prefab.transform.position}");
            //Canvas.PrintSceneHierarchyTree();
        }
        public void UpdateSliderConstrain(string name,int idx, float min, float max,bool wholeNum = false)
        {
            var PropPanel = PropPanels[idx];
            var slider = PropPanel.transform.Find("Slider").GetComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = wholeNum;

            UpdateName(idx, $"{name}[{min},{max}]");
        }

        public void UpdateValue(int idx, float value)
        {
            var PropPanel = PropPanels[idx];
            var slider = PropPanel.transform.Find("Slider").GetComponent<Slider>();
            slider.value = value;
            PropPanel.transform.Find("Value").GetComponent<InputField>().text = value.ToString();
        }
        public void AddListener(int idx, UnityAction<float> func)
        {
            if (func == null)
                return;
            var PropPanel = PropPanels[idx];
            var slider = PropPanel.transform.Find("Slider").GetComponent<Slider>();
            slider.onValueChanged.AddListener(func);
        }
        internal UnityAction<float> UpdateTextDelegate(int idx)
        {
            var PropPanel = PropPanels[idx];
            var slider = PropPanel.transform.Find("Slider").GetComponent<Slider>();
            var text = PropPanel.transform.Find("Value").GetComponent<InputField>();
            AddListener(idx, ((v) => {
                text.text = v.ToString();
            }));
            text.onValueChanged.AddListener((va) =>
            {
                if (float.Parse(va) >= slider.minValue && float.Parse(va) <= slider.maxValue)
                {
                    slider.value = float.Parse(va);
                }
                else
                {
                    text.text = slider.minValue.ToString();
                }
            });
            return null;
        }
        public static void DefaultValueChange(float val)
        {
            Logger.LogDebug($"Slider Value Change :{val}");
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(CurrentCanvas);
        }
    }

    
    public static class Inspector
    {
        private static InspectPanel currentEdit;
        public static readonly Dictionary<PropertyInfo, Operation> handler = new Dictionary<PropertyInfo, Operation>();
        public static readonly Dictionary<Type, Dictionary<string,PropertyInfo>> cache_prop = new Dictionary<Type, Dictionary<string,PropertyInfo>>();
        private static Detour _d;
        private static Detour OpLock { 
            get
            {
                if (_d != null)
                    return _d;
                _d = new Detour(typeof(DecorationMaster).GetMethod("OperateItem", BindingFlags.NonPublic | BindingFlags.Instance), typeof(Inspector).GetMethod(nameof(NoOpOperateItem), BindingFlags.NonPublic | BindingFlags.Static));
                _d.Undo();
                ItemManager.Instance.OnChanged += Instance_OnChanged;
                return _d;
            } 
        }
        private static void Instance_OnChanged(CustomDecoration d)
        {
            if(IsToggle())
            {
                Hide();
                Show();
            }
        }

        private static void _reflectProps(Type t, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (cache_prop.ContainsKey(t))
                return;
            var propInfos =t.GetProperties(flags)
                .Where(x =>
                {
                    var handlers = x.GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>();
                    bool handflag = handlers.Any();
                    bool ignoreflag = x.GetCustomAttributes(typeof(InspectIgnoreAttribute), true).OfType<InspectIgnoreAttribute>().Any();
                    if(handflag && (!ignoreflag))
                    {
                        if(!handler.ContainsKey(x))
                            handler.Add(x, handlers.FirstOrDefault().handleType);
                    }

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

                //Logger.LogDebug($"Cache PropInfo:T:{t},name:{p.Name}");
            }
            //Logger.LogDebug($"_reflectProp_resutl:{propInfos.ToArray().Length}");
        }
        public static void Show()
        {
            OpLock.Apply();
            try
            {
                Item item = ItemManager.Instance.currentSelect.GetComponent<CustomDecoration>().item;
                

                if (!cache_prop.ContainsKey(item.GetType()))
                {
                    _reflectProps(item.GetType());
                }
                if (cache_prop.TryGetValue(item.GetType(), out var itemProps))
                {
                    var insp = new InspectPanel();
                    currentEdit = insp;
                    int idx = 0;
                    foreach (var kv in itemProps)
                    {
                        string name = kv.Key;
                        Type propType = kv.Value.PropertyType;
                        object value = kv.Value.GetValue(item, null);
                        value = Convert.ToSingle(value);
                        ConstraintAttribute con = kv.Value.GetCustomAttributes(typeof(ConstraintAttribute), true).OfType<ConstraintAttribute>().FirstOrDefault();

                        LogProp(propType, name, value);

                        if(idx == 0)
                        {
                            insp.UpdateName(idx,name);
                            if(con is IntConstraint)
                            {
                                //Logger.LogDebug($"Check1 {con.Min}-{con.Max}");
                                insp.UpdateSliderConstrain(name,idx, (float)Convert.ChangeType(con.Min, typeof(float)), Convert.ToInt32(con.Max), true);
                            }
                            else if(con is FloatConstraint)
                            {
                                //Logger.LogDebug($"Check2 {con.Min}-{con.Max}");
                                insp.UpdateSliderConstrain(name,idx, (float)(con.Min), (float)(con.Max), false);
                            }
                            else
                            {
                                throw new ArgumentException();
                            }
                            //Logger.LogDebug($"Check3 {value}-{value.GetType()}");
                            insp.UpdateValue(idx, (float)value);
                        }
                        else
                        {
                            insp.AppendPropPanel(name);
                            if (con is IntConstraint)
                            {
                                insp.UpdateSliderConstrain(name,idx, (int)con.Min, (int)con.Max, true);
                            }
                            else if (con is FloatConstraint)
                            {
                                insp.UpdateSliderConstrain(name,idx, (float)con.Min, (float)con.Max, false);
                            }
                            else
                            {
                                throw new ArgumentException();
                            }
                            insp.UpdateValue(idx, (float)value);
                            insp.UpdateTextDelegate(idx);//insp.AddListener(idx, insp.UpdateTextDelegate(idx));

                        }
                        //insp.AddListener(idx, (v) => { kv.Value.SetValue(item, Convert.ChangeType(v, kv.Value.PropertyType), null); });
                        insp.AddListener(idx, (v) => {
                            if (ItemManager.Instance.currentSelect == null)
                                return;
                            object val;
                            try
                            {
                                if (kv.Value.PropertyType.IsSubclassOf(typeof(Enum)))
                                {
                                    val = Enum.Parse(kv.Value.PropertyType, v.ToString("0"));
                                }
                                else
                                    val = Convert.ChangeType(v, kv.Value.PropertyType);
                                ItemManager.Instance.currentSelect.GetComponent<CustomDecoration>().Setup(handler[kv.Value], val);
                            }
                            catch
                            {
                                Logger.LogError("Error occour at Inspect OnValue Chnaged");
                                Hide();
                            }
                        });
                        idx++;
                    }
                }
                else
                {
                    Logger.LogError($"KeyNotFount at cache_prop,{item.GetType()}");
                }
                
            }
            catch(NullReferenceException e)
            {
                Logger.LogError($"NulRef Error at Inspector.Show:{e}");
                OpLock.Undo();
            }
       
        }
        public static void Hide()
        {
            OpLock.Undo();
            currentEdit?.Destroy();
            currentEdit = null;
        }
        private static void NoOpOperateItem(object dm) { }
        internal static void LogProp(Type t,string name,object value)
        {
            Logger.LogDebug($"[{t}]ItemProp:{name}:{value}");
        }

        public static bool ToggleInspect()
        {
            return Input.GetMouseButtonUp((int)MouseButton.Middle);
        }
        public static bool IsToggle()
        {
            return OpLock.IsApplied;
        }
    }
}
