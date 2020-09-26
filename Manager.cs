using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using ModCommon;
using DecorationMaster.Attr;
using System.Reflection;
using DecorationMaster.UI;

namespace DecorationMaster
{
    public class ItemManager
    {
        private static ItemManager _instance;
        private  GameObject _setup_flag_backing;
        private static GameObject currentSelect;
        public delegate void GroupSwitchHandler(string[] nextGroup);
        public event GroupSwitchHandler GroupSwitchEventHandler;
        private  GameObject SetupFlag { get
            {
                if (_setup_flag_backing != null)
                    return _setup_flag_backing;
                SetupFlagInit();
                return _setup_flag_backing;
            }
            set
            {
                _setup_flag_backing = value;
            }
        }
        private static Dictionary<int, string[]> group = new Dictionary<int, string[]>();
        public const int GroupMax = 6;
        public int CurrentGroup { get; private set; } = 1;
        public bool setupMode { get; private set; }
        public static ItemManager Instance {
            get
            {
                if (_instance == null)
                    _instance = new ItemManager();
                return _instance;

             } 
        }
        private ItemManager()
        {
            if (SetupFlag == null)
                SetupFlagInit();

            var deGo = ObjectLoader.InstantiableObjects.Where(x => x.Value.GetComponent<CustomDecoration>() != null);
            var Names = deGo.Select(x => x.Key);
            int group_idx = 0;
            
            while (Names.Any())
            {
                group_idx++;
                var a = Names.Take(GroupMax).ToArray();
                Names = Names.Skip(GroupMax);
                group.Add(group_idx, a);
            }

            Logger.LogDebug("Group Info");
            foreach(var kv in group)
            {
                Logger.LogDebug($"{kv.Key}:");
                foreach(var s in kv.Value)
                {
                    Logger.LogDebug(s);
                }
            }
        }
        public int SwitchGroup(int span = 1)
        {
            var next_group = CurrentGroup + span;
            if (!group.ContainsKey(next_group))
                next_group = 1;
            CurrentGroup = next_group;
            SetupFlag.GetComponent<TextMeshPro>().text = $"Editing [{CurrentGroup}]";

            GroupSwitchEventHandler?.Invoke(group[CurrentGroup]);

            return CurrentGroup;
        }
        public bool ToggleSetup()
        {
            if(HeroController.instance == null)
            {
                setupMode = false;
                return setupMode;
            }
            setupMode = !setupMode;
            if (setupMode)
            {
                SetupFlag.SetActive(true);
                SetupFlag.transform.SetParent(HeroController.instance.transform);
                SetupFlag.transform.localPosition = new Vector3(0, 1.5f);
            }
            else
            {
                SetupFlag.SetActive(false);
            }
            PickPanel.SetActive(setupMode);
            return setupMode;
        }
        public CustomDecoration Select(int idx)
        {
            if (!setupMode)
                return null;
            if (idx < 1 || idx > group[CurrentGroup].Length)
                return null;
            
            if (currentSelect != null)
                return currentSelect.GetComponent<CustomDecoration>();

            string poolname = group[CurrentGroup][idx - 1];
            Logger.LogDebug($"Selected {idx},{poolname}");
            GameObject go = ObjectLoader.CloneDecoration(poolname);

            currentSelect = go;
            Logger.LogDebug($"CURRENT Null?{go?.name}");
            
            CustomDecoration cd = go?.GetComponent<CustomDecoration>();
            Logger.LogDebug($"Decoration Null?{cd == null}");
            Logger.LogDebug($"Item Null?{cd?.item == null},Prefab Item Null?{ObjectLoader.InstantiableObjects[poolname].GetComponent<CustomDecoration>()?.item == null}");
            go?.SetActive(true);

            Test.TestGo(go);
            return cd;
        }
        public void Operate(Operation op,object val)
        {
            if (currentSelect == null)
                return;
            var d = currentSelect.GetComponent<CustomDecoration>();
            d.Setup(op, val);
        }
        public void AddCurrent()
        {
            if (currentSelect == null)
                return;
            Operate(Operation.ADD, null);
            currentSelect = null;
        }
        public void RemoveCurrent()
        {
            if (currentSelect == null)
                return;
            currentSelect.SetActive(false);
            Object.DestroyImmediate(currentSelect);
            currentSelect = null;
        }
        private void SetupFlagInit()
        {
            if (_setup_flag_backing != null)
                Object.DestroyImmediate(_setup_flag_backing);
            SetupFlag = new GameObject("text");
            var text = SetupFlag.AddComponent<TextMeshPro>();
            text.text = $"Editing:[{CurrentGroup}]";
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 28;
            text.outlineColor = Color.black;
            text.outlineWidth = 0.1f;
            SetupFlag.AddComponent<KeepWorldScalePositive>();
            //SetupFlag.AddComponent<HookCursor>();
            SetupFlag.AddComponent<MyCursor>();
            SetupFlag.transform.SetScaleX(0.2f);
            SetupFlag.transform.SetScaleY(0.2f);
            SetupFlag.SetActive(false);
            Object.DontDestroyOnLoad(SetupFlag);

        }
        private class HookCursor : MonoBehaviour
        {
            private void OnEnable()
            {
                On.InputHandler.OnGUI += InputHandler_OnGUI;
                var m = GameManager.instance.inputHandler.GetType().GetMethod("SetCursorVisible", BindingFlags.NonPublic | BindingFlags.Instance);
                m.Invoke(GameManager.instance.inputHandler, new object[] { true, });
            }

            private void InputHandler_OnGUI(On.InputHandler.orig_OnGUI orig, InputHandler self) { }

            private void OnDisable()
            {
                On.InputHandler.OnGUI -= InputHandler_OnGUI;
            }
        }
        
    }
    public class MyCursor : MonoBehaviour
    {
        //private static GameObject arrow;
        private static bool draw = false;
        private static Texture2D cursorTexture;
        public static Vector3 CursorPosition;
        private void Awake()
        {
            var tex = GUIController.Instance.images["arrow"];
            cursorTexture = tex;
        }
        private void OnGUI()
        {
            if (!draw)
                return;
            var mousePos = Input.mousePosition;
            GUI.DrawTexture(new Rect(mousePos.x, Screen.height - mousePos.y, cursorTexture.width, cursorTexture.height), cursorTexture);
            var screenPos = Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0));
            mousePos.z = screenPos.z;
            CursorPosition = Camera.main.ScreenToWorldPoint(mousePos);
        }
        
        private void OnEnable()
        {
            draw = true;
        }
        private void OnDisable()
        {
            draw = false;
        }

    }
}
