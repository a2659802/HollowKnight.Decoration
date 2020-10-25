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
using DecorationMaster.UI;
using DecorationMaster.Util;

namespace DecorationMaster
{
    public delegate int SelectItem();
    public class DecorationMaster : Mod,ITogglableMod
    {
        private float autoSaveTimer = 0;
        private static GameManager _gm;
        private GameObject UIObj;

        public static DecorationMaster instance;
        public SelectItem SelectGetter;
        internal static GameManager GM => _gm != null ? _gm : (_gm = GameManager.instance);
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            instance = this;

            //new Test();
            //return;
            Logger.Log("Load Json");
            ItemSettings global = SerializeHelper.LoadGlobalSettings<ItemSettings>();
            if (global != null)
            {
                if (global.mod_version > Version)
                    throw new FileLoadException("Try To Load an newer json data in an older mod,please update mod");
                ItemData = global;
                Logger.Log("Loaded Json");
                
            }

            #region Init GameObject
            ObjectLoader.Load(preloadedObjects);
            BehaviourProcessor.RegisterBehaviour<Draw>();
            BehaviourProcessor.RegisterBehaviour<OtherBehaviour>();
            BehaviourProcessor.RegisterBehaviour<AreaBehaviour>();
            BehaviourProcessor.RegisterBehaviour<MovablePlatform>();
            BehaviourProcessor.RegisterBehaviour<ModifyGameItem>();
            BehaviourProcessor.RegisterBehaviour<Mana>();
            BehaviourProcessor.RegisterBehaviour<AudioBehaviours>();
            BehaviourProcessor.RegisterBehaviour<OneShotBehaviour>();
            BehaviourProcessor.RegisterSharedBehaviour<DefaultBehaviour>();
            BehaviourProcessor.RegisterSharedBehaviour<UnVisableBehaviour>();
            #endregion

            UIObj = new GameObject();
            UIObj.AddComponent<GUIController>();
            UnityEngine.Object.DontDestroyOnLoad(UIObj);
            GUIController.Instance.BuildMenus();

            #region SetupCallBack
            SelectGetter = GetKeyPress;
            SelectGetter += PickPanel.SelectFocus;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SpawnFromSettings;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += ShowRespawn;
            ModHooks.Instance.LanguageGetHook += DLanguage.MyLanguage;
            ModHooks.Instance.ApplicationQuitHook += SaveJson;
            if (Settings.CreateMode)
                ModHooks.Instance.HeroUpdateHook += OperateItem;
            #endregion

            UserLicense.ShowLicense();
            
        }

        private void SaveJson()
        {
            ItemData.mod_version = Version;
            SerializeHelper.SaveGlobalSettings(ItemData);
            Logger.LogDebug("Save Json");
        }

        private void ShowRespawn(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name.Contains("Menu_Title"))
                return;
            if (!ItemManager.Instance.setupMode)
                return;
            GameManager.instance.StartCoroutine(WaitSceneLoad(arg0));
            IEnumerator WaitSceneLoad(Scene arg0)
            {
                yield return new WaitUntil(() => (arg0.isLoaded));
                var triggers = UnityEngine.Object.FindObjectsOfType<HazardRespawnTrigger>();
                foreach(var t in triggers)
                {
                    t.gameObject.AddComponent<ShowColliders>();
                }
                Logger.LogDebug($"found respawn :{triggers.Length}");
            }
        }

        private void SpawnFromSettings(Scene arg0, LoadSceneMode arg1)
        {
            Logger.LogDebug($"Item Count:{ItemData.items.Count}");
            if (arg0.name.Contains("Menu_Title"))
                return;
            if (ItemData.items.Count > 0)
            {
                GameManager.instance.StartCoroutine(WaitSceneLoad(arg0));
            }
            IEnumerator WaitSceneLoad(Scene arg0)
            {
                int count = 0;
                Logger.LogDebug("Try to spawn setting");
                string sceneName = arg0.name;
                yield return new WaitUntil(() => (arg0.isLoaded));
                foreach (var r in ItemData.items)
                {
                    if (r.sceneName != sceneName)
                        continue;

                    var poolname = r.pname;
                    try
                    {
                        var decorationGo = ObjectLoader.CloneDecoration(poolname,r);
                        if (decorationGo != null)
                        {
                            //decorationGo.GetComponent<CustomDecoration>().Setup(Operation.Serialize, r);
                            count++;
                        }
                    }
                    catch
                    {
                        Logger.LogError($"Spawn Failed When Try to Spawn {poolname}");
                    }
                }
                
                Modding.Logger.LogDebug($"All Fine,Spawn {count} in {sceneName}");
            }
        }

        private Vector2 mousePos;
        private void OperateItem()
        {
            if(ItemManager.Instance.setupMode)
            {
                autoSaveTimer += Time.deltaTime;
                if(autoSaveTimer > 60*3)
                {
                    SaveJson();
                    autoSaveTimer = 0;
                    Logger.LogDebug("Auto Save");
                }
            }
            //Test.TestOnce();

            if (Input.GetKeyDown(ToggleEdit))    // Toggle Edit Model
            {
                ItemManager.Instance.ToggleSetup();
            }
            if (Input.GetKeyDown(SwitchGroup))   // Switch Select Group
            {
                ItemManager.Instance.SwitchGroup();
            }

            //Vector2 cur_mousePos = GetMousePos();   //Update Mouse Pos
            Vector2 cur_mousePos = MyCursor.CursorPosition;
            if (cur_mousePos != mousePos)
            {
                mousePos = cur_mousePos;
                ItemManager.Instance.Operate(Operation.SetPos, mousePos);
            }

            if(GM != null && !GM.isPaused && !GM.IsInSceneTransition)
            {
                if (Input.GetMouseButtonUp((int)MouseButton.Left)) // Confirm Go
                {
                    ItemManager.Instance.AddCurrent();
                }
                else if (Input.GetMouseButtonUp((int)MouseButton.Right)) // Discard Go
                {
                    ItemManager.Instance.RemoveCurrent();
                }
            }
            int idx = -1;
            foreach (SelectItem selector in SelectGetter.GetInvocationList())// Get user Selection
            {
                int res = selector.Invoke();
                if (res != -1)
                    idx = res;
            }
            if (ItemManager.Instance.Select(idx) == null)
                return;

        }

        private static int GetKeyPress()
        {
            for(int i=1;i<= ItemManager.GroupMax; i++)
            {
                if( Input.GetKeyDown(KeyCode.Alpha0 + i) )
                {
                    return i;
                }
                    
            }
           
            /*
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
            }*/

            return -1;
        }
        public static Vector3 GetMousePos()
        {
            //var screenPos = Camera.main.WorldToScreenPoint(HeroController.instance.transform.position);
            var screenPos = Camera.main.WorldToScreenPoint(new Vector3(0,0,0));
            var mousePosOnScreen = Input.mousePosition;
            mousePosOnScreen.z = screenPos.z;
            return Camera.main.ScreenToWorldPoint(mousePosOnScreen);
        }
        public void Unload()
        {
            SaveJson();
            #region RemoveCallBack
            SelectGetter = null;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SpawnFromSettings;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= ShowRespawn;
            ModHooks.Instance.LanguageGetHook -= DLanguage.MyLanguage;
            ModHooks.Instance.ApplicationQuitHook -= SaveJson;
            ModHooks.Instance.HeroUpdateHook -= OperateItem;
            UnityEngine.Object.Destroy(UIObj);
            #endregion

        }

        public GlobalModSettings Settings = new GlobalModSettings();
        public ItemSettings ItemData = new ItemSettings();
        public override ModSettings GlobalSettings
        {
            get => Settings;
            set => Settings = (GlobalModSettings)value;
        }
        public override List<(string, string)> GetPreloadNames() => ObjectLoader.ObjectList.Values.ToList();
        public override string GetVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            string ver = Version.ToString("0.000");

            using SHA1 sha1 = SHA1.Create();
            using FileStream stream = File.OpenRead(asm.Location);

            byte[] hashBytes = sha1.ComputeHash(stream);

            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            return $"{ver}-{hash.Substring(0, 6)}";
        }
        public KeyCode ToggleEdit => Settings.ToggleEditKey;
        public KeyCode SwitchGroup => Settings.SwitchGroupKey;

        public const float Version = 0.21f;
    }
    public static class Logger
    {
        public static void Log(object obj) => DecorationMaster.instance.Log(obj);
        public static void LogDebug(object obj) => DecorationMaster.instance.LogDebug(obj);
        public static void LogWarn(object obj) => DecorationMaster.instance.LogWarn(obj);
        public static void LogError(object obj) => DecorationMaster.instance.LogError(obj);
    }
}
