using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using DecorationMaster.MyBehaviour;
using HutongGames.PlayMaker;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DecorationMaster
{
    public class DecorationMaster : Mod//,ITogglableMod
    {
        public static DecorationMaster instance;
        
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            instance = this;
            ObjectLoader.Load(preloadedObjects);
            BehaviourProcessor.RegisterBehaviour<OtherBehaviour>();
            BehaviourProcessor.RegisterBehaviour<AreaBehaviour>();
            ModHooks.Instance.HeroUpdateHook += OperateItem;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SpawnFromSettings;

        }

        private void SpawnFromSettings(Scene arg0, LoadSceneMode arg1)
        {
            Logger.LogDebug($"Item Count:{Settings.items.Count}");
            if (Settings.items.Count > 0)
            {
                GameManager.instance.StartCoroutine(WaitSceneLoad(arg0));
            }
            IEnumerator WaitSceneLoad(Scene arg0)
            {
                Logger.LogDebug("Try to spawn setting");
                string sceneName = arg0.name;
                yield return new WaitUntil(() => (arg0.isLoaded));
                foreach (var r in Settings.items)
                {
                    if (r.sceneName != sceneName)
                        continue;

                    var poolname = r.pname;
                    //var prefab = ObjectLoader.InstantiableObjects[poolname];
                    var decorationGo = ObjectLoader.CloneDecoration(poolname);
                    decorationGo.GetComponent<CustomDecoration>().HandleInit(r);

                }
                Modding.Logger.LogDebug("All Fine");
            }
        }

        private Vector2 mousePos;
        private void OperateItem()
        {
            if (Input.GetKeyDown(ToggleEdit))    // Toggle Edit Model
            {
                ItemManager.Instance.ToggleSetup();
            }
            if (Input.GetKeyDown(SwitchGroup))   // Switch Select Group
            {
                ItemManager.Instance.SwitchGroup();
            }

            int idx = GetKeyPress();    // Get user Selection
            ItemManager.Instance.Select(idx);

            var cur_mousePos = GetMousePos();   //Update Mouse Pos
            if(cur_mousePos != mousePos)
            {
                mousePos = cur_mousePos;
                ItemManager.Instance.Operate(Operation.SetPos, mousePos);
            }

            if (Input.GetMouseButtonDown((int)MouseButton.Middle))  // rotate GO
            {
                //manager.Current.transform.eulerAngles += new Vector3(0, 0, 90);
                //ItemManager.Instance.Operate(Operation)
            }
            else if (Input.GetMouseButtonUp((int)MouseButton.Left)) // Confirm Go
            {
                ItemManager.Instance.AddCurrent();
            }
            else if (Input.GetMouseButtonUp((int)MouseButton.Right)) // Discard Go
            {
                ItemManager.Instance.RemoveCurrent();
            }
        }

        private static int GetKeyPress()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                return 1;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                return 2;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                return 3;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                return 4;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                return 5;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                return 6;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                return 7;
            }
            return -1;
        }
        private static Vector2 GetMousePos()
        {
            var screenPos = Camera.main.WorldToScreenPoint(HeroController.instance.transform.position);
            var mousePosOnScreen = Input.mousePosition;
            mousePosOnScreen.z = screenPos.z;
            return Camera.main.ScreenToWorldPoint(mousePosOnScreen);
        }
        public void Unload()
        {

        }
        public GlobalModSettings Settings = new GlobalModSettings();
        public override ModSettings GlobalSettings
        {
            get => Settings;
            set => Settings = (GlobalModSettings)value;
        }
        public override List<(string, string)> GetPreloadNames() => ObjectLoader.ObjectList.Values.ToList();
        public override string GetVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            string ver = asm.GetName().Version.ToString();

            using SHA1 sha1 = SHA1.Create();
            using FileStream stream = File.OpenRead(asm.Location);

            byte[] hashBytes = sha1.ComputeHash(stream);

            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            return $"{ver}-{hash.Substring(0, 6)}";
        }
        public const KeyCode ToggleEdit = KeyCode.Keypad7;
        public const KeyCode SwitchGroup = KeyCode.Keypad8;
    }
    public static class Logger
    {
        public static void Log(object obj) => DecorationMaster.instance.Log(obj);
        public static void LogDebug(object obj) => DecorationMaster.instance.LogDebug(obj);
        public static void LogWarn(object obj) => DecorationMaster.instance.LogWarn(obj);
        public static void LogError(object obj) => DecorationMaster.instance.LogError(obj);
    }
}
