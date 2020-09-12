using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using ModCommon;
namespace DecorationMaster
{
    public class ItemManager
    {
        private static ItemManager _instance;
        private static GameObject currentSelect;
        private static GameObject SetupFlag;
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

            //go.PrintSceneHierarchyTree();
            //ObjectLoader.InstantiableObjects[poolname].PrintSceneHierarchyTree();
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
            var decoration = currentSelect.GetComponent<CustomDecoration>();
            decoration.Add();
            currentSelect = null;
        }
        public void RemoveCurrent()
        {
            currentSelect.SetActive(false);
            Object.DestroyImmediate(currentSelect);
            currentSelect = null;
        }
        private void SetupFlagInit()
        {
            if (SetupFlag != null)
                Object.DestroyImmediate(SetupFlag);
            SetupFlag = new GameObject("text");
            var text = SetupFlag.AddComponent<TextMeshPro>();
            text.text = $"Editing:[{CurrentGroup}]";
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 28;
            text.outlineColor = Color.black;
            text.outlineWidth = 0.1f;
            SetupFlag.AddComponent<KeepWorldScalePositive>();
            SetupFlag.transform.SetScaleX(0.2f);
            SetupFlag.transform.SetScaleY(0.2f);
            SetupFlag.SetActive(false);
            Object.DontDestroyOnLoad(SetupFlag);
        }
    }
}
