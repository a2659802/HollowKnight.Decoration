using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using DecorationMaster.Attr;
using System.Runtime.Serialization;
using System.Collections;
using DecorationMaster.Util;
using MonoMod.RuntimeDetour;
using UnityEngine.SceneManagement;
using GlobalEnums;
using TMPro;

namespace DecorationMaster.MyBehaviour.Gem
{
    [Description("传送点\nIdentity是自身标识名字\nDestination是目的地的标识名")]
    [Description("Teleport\n input identity and destination\n destination is target's identity","en-us")]
    [Decoration("IMG_TP")]
    public class TransitionGem : CustomDecoration
    {
        /*public class MyFade
        {
            public MyFade()
            {
                var go = new GameObject();
                go.AddComponent<FadeBehaviour>();
            }
            class FadeBehaviour : MonoBehaviour
            {
                Texture2D tex;

                float duration = 3f;
                float timer;
                void Awake()
                {
                    timer = duration;
                    tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(0,0,0,1));
                    tex.Apply();
                    DontDestroyOnLoad(gameObject);
                }
                void Start()
                {
                    // Destroy(gameObject, 1);
                }
                void OnGUI()
                {
                    if(timer>0.05)
                    {
                        timer -= Time.deltaTime;
                        tex.SetPixel(0, 0, new Color(0, 0, 0, timer / duration));
                        tex.Apply();
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                    
                    //GUI.Label(new Rect(-10f, -10f, Screen.width + 10, Screen.height + 10), tex, style);
                    GUI.DrawTexture(new Rect(-10f, -10f, Screen.width + 10, Screen.height + 10), tex);
                }
            }
        }*/
        
        private static HeroController hc => HeroController.instance;
        private static GameManager gm => GameManager.instance;
        private Vector2 targetPos;
        private GameObject textDisp;
        private HeroTrigger ht;
        private BoxCollider2D col;
        private SpriteRenderer sr;
        private static Detour prevent_level_activated;
        private static bool loading = false;
        private TPItem tpitem => (TPItem)item;
        public static Dictionary<string, string> map = new Dictionary<string, string>();
        private static void UpdateDict(string id, string scene)
        {
            map[id] = scene;
        }

        [Serializable]
        [GlobalItem]
        public class TPItem : Item
        {
            [StringConstraint(1, 32)]
            [Handle(Operation.SetId)]
            public string Identity { get; set; } = "";

            [StringConstraint(1, 32)]
            [Handle(Operation.SetDst)]
            public string Destination { get; set; } = "";

            [OnDeserialized]
            public void AfterSerialize(StreamingContext context)
            {
                Logger.LogDebug("On AfterSerialize:" + Identity + Destination);
                UpdateDict(Identity, sceneName);
            }
        }
        /*
        private void CreateGateway(string gateName, Vector2 pos, Vector2 size, string toScene, string entryGate,
                                   bool right, bool left, bool onlyOut, GameManager.SceneLoadVisualizations vis)
        {

            GameObject gate = new GameObject(gateName);
            gate.transform.SetPosition2D(pos);
            var tp = gate.AddComponent<TransitionPoint>();
            if (!onlyOut)
            {
                var bc = gate.AddComponent<BoxCollider2D>();
                bc.size = size;
                bc.isTrigger = true;
                tp.targetScene = toScene;
                tp.entryPoint = entryGate;
            }
            tp.alwaysEnterLeft = left;
            tp.alwaysEnterRight = right;
            GameObject rm = new GameObject("Hazard Respawn Marker");
            rm.transform.parent = tp.transform;
            rm.transform.position = new Vector2(rm.transform.position.x - 3f, rm.transform.position.y);
            var tmp = rm.AddComponent<HazardRespawnMarker>();
            tp.respawnMarker = rm.GetComponent<HazardRespawnMarker>();
            tp.sceneLoadVisualization = vis;
        }
        private static void aa()
        {
            var go = new GameObject();
            UnityEngine.Object.Destroy(go, 2);
            go.AddComponent<CameraFade>().StartFade(new Color(0,0,0.5f,0.7f), 2);

            UnityEngine.Object.FindObjectOfType(typeof(TransitionGem));
            var gateName = "right test";
            var pos = HeroController.instance.transform.position + new Vector3(2, 0, 0);
            var onlyOut = false;
            var size = Vector2.one;
            var toScene = "Abyss_09";
            var entryGate = "left1";
            var left = false;
            var right = true;
            var vis = GameManager.SceneLoadVisualizations.Default;
            GameObject gate = new GameObject(gateName);
            gate.transform.SetPosition2D(pos);
            var tp = gate.AddComponent<TransitionPoint>();
            
            tp.alwaysEnterLeft = left;
            tp.alwaysEnterRight = right;
            GameObject rm = new GameObject("Hazard Respawn Marker");
            rm.transform.parent = tp.transform;
            rm.transform.position = new Vector2(rm.transform.position.x - 3f, rm.transform.position.y);
            var tmp = rm.AddComponent<HazardRespawnMarker>();
            tp.respawnMarker = rm.GetComponent<HazardRespawnMarker>();
            tp.sceneLoadVisualization = vis;


            if (!onlyOut)
            {
                var bc = gate.AddComponent<BoxCollider2D>();
                bc.size = size;
                bc.isTrigger = true;
                tp.targetScene = toScene;
                tp.entryPoint = entryGate;
            }

            HeroController.instance.StartCoroutine(HeroController.instance.EnterScene(tp, 0.5f));

            GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
            {
                SceneName = toScene,
                EntryGateName = entryGate,
                HeroLeaveDirection = GlobalEnums.GatePosition.right,
                EntryDelay = 0,
                WaitForSceneTransitionCameraFade = true,
                PreventCameraFadeOut = false,
                Visualization = vis,
                AlwaysUnloadUnusedAssets = false,
                forceWaitFetch = false
            });
        }

        private static void Out(Vector2 pos, bool transitionpoint = false)
        {
            if(!transitionpoint)
            {
                IEnumerator Spawn()
                {
                    HeroController.instance.transform.position = pos;
                    yield return new WaitForSeconds(1);
                    HeroController.instance.transform.position = pos;
                    HeroController.instance.SetHazardRespawn(pos, true);
                }
                HeroController.instance.StartCoroutine(Spawn());
                //HeroController.instance.transform.SetPosition2D(pos);
                //HeroController.instance.SetHazardRespawn(pos, true);
                return;
            }
            var gateName = "tp top";
            var vis = GameManager.SceneLoadVisualizations.Default;
            GameObject gate = new GameObject(gateName);
            gate.transform.SetPosition2D(pos);
            var tp = gate.AddComponent<TransitionPoint>();
            GameObject rm = new GameObject("Hazard Respawn Marker");
            rm.transform.parent = tp.transform;
            rm.transform.position = pos;
            var tmp = rm.AddComponent<HazardRespawnMarker>();
            tp.respawnMarker = tmp;
            tp.sceneLoadVisualization = vis;

            HeroController.instance.StartCoroutine(HeroController.instance.EnterScene(tp, 0.5f));
        }
        private void Load_Complete()
        {
            IEnumerator Spawn()
            {

                HeroController.instance.transform.position = targetPos;
                yield return new WaitForSeconds(0.2f);
                HeroController.instance.transform.position = targetPos;
                HeroController.instance.SetHazardRespawn(targetPos, true);
                afterTP();
            }
            HeroController.instance.StartCoroutine(Spawn());
            //fix hero pos

        }

        private void Loaded(AsyncOperation obj)
        {
            IEnumerator Spawn()
            {

                HeroController.instance.transform.position = targetPos;
                yield return new WaitForSeconds(0.1f);
                HeroController.instance.transform.position = targetPos;
                HeroController.instance.SetHazardRespawn(targetPos, true);
                afterTP();
            }

            HeroController.instance.StartCoroutine(Spawn());
            new MyFade();
        }
        */
        private static void beforeTP()
        {
            hc.acceptingInput = false;
            PrivateHelper.Call(hc, "ResetMotion");//hc.ResetMotion();
            PrivateHelper.SetField(hc, "airDashed", false);//hc.airDashed = false;
            PrivateHelper.SetField(hc, "doubleJumped", false);//hc.doubleJumped = false;
            hc.ResetHardLandingTimer();
            PrivateHelper.Call(hc, "ResetAttacksDash");//hc.ResetAttacksDash();
            hc.AffectedByGravity(false);
            
        }
        private static void afterTP()
        {
            PrivateHelper.Call(hc, "SetStartingMotionState", false);//hc.SetStartingMotionState(false);
            hc.AffectedByGravity(true);
            hc.acceptingInput = true;          
        }

        //actual do teleport
        public void TP()
        {
            if (loading)
                return;
            loading = true;
            Consume();
            //var tpitem = (TPItem)item;
            //get dst id
            var dstid = tpitem.Destination;
            //get scene of dst
            map.TryGetValue(dstid, out string scene);
            if(string.IsNullOrEmpty(scene))
            {
                Logger.LogError("Tp Dst Null");
                return;
            }
            TPItem target = null;
            var targetSceneItems = DecorationMaster.instance.ItemData.items;
            foreach(var i in targetSceneItems)
            {
                if (i.GetType() != typeof(TPItem))
                    continue;

                if(((TPItem)i).Identity == dstid)
                {
                    target = (TPItem)i;
                    break;
                }
            }
            if(target == null)
            {
                Logger.LogError("Tp Obj Null");
                return;
            }
            //get dst position
            targetPos = target.position;

            if (GameManager.instance.isPaused)
            {
                UnPause();
            }

            beforeTP();

            if(GameManager.instance.sceneName != scene)
            {
                prevent_level_activated.Apply();

                GameCameras.instance.cameraController.FreezeInPlace(true);
                GameCameras.instance.cameraController.FadeOut(CameraFadeType.LEVEL_TRANSITION);
                //GameManager.instance.nextSceneName = scene;
                //var tmp = new GameObject();
                //Destroy(tmp, 1);
                //tmp.AddComponent<CameraFade>().FadeToBlack(0.25f);
                string exitingScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                //UnityEngine.SceneManagement.SceneManager.LoadScene(scene, UnityEngine.SceneManagement.LoadSceneMode.Single);
                var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                //async.completed += Loaded;
                async.completed += (o) =>
                {
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(exitingScene).completed += (_) =>
                    {
                        
                        
                        gm.SetupSceneRefs(true);
                        gm.BeginScene();
                        if (gm.gameMap != null)
                        {
                            gm.gameMap.GetComponent<GameMap>().LevelReady();
                        }
                        hc.vignetteFSM.SendEvent("RESET");
                        hc.cState.transitioning = false;
                        PrivateHelper.SetField(hc, "transitionState", HeroTransitionState.WAITING_TO_TRANSITION);
                        afterTP();
                        HeroController.instance.transform.position = targetPos;
                        HeroController.instance.SetHazardRespawn(targetPos, true);
                        gm.SetState(GameState.PLAYING);
                        PrivateHelper.SetField(gm, "hasFinishedEnteringScene", true);

                        GameCameras.instance.SceneInit();
                        GameCameras.instance.cameraController.FadeSceneIn();

                        loading = false;
                        prevent_level_activated.Undo();
                    };
                    
                };
                /*
                On.SceneLoad.Begin += SceneLoad_Begin;
                wait_load = true;
                GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
                {
                    SceneName = scene,
                    EntryGateName = $"tp_{dstid}",
                    HeroLeaveDirection = GlobalEnums.GatePosition.unknown,
                    EntryDelay = 0,
                    WaitForSceneTransitionCameraFade = true,
                    PreventCameraFadeOut = false,
                    Visualization = GameManager.SceneLoadVisualizations.Default,
                    AlwaysUnloadUnusedAssets = true,
                    forceWaitFetch = false
                });*/

                return;
            }


            //consume target gem prevent tp loop
            var go = GameObject.Find($"tp_{dstid}");
            go?.GetComponent<TransitionGem>()?.Consume();

            //set hero pos  and respawn pos
            //Out(pos,false);
            HeroController.instance.transform.position = targetPos;
            HeroController.instance.SetHazardRespawn(targetPos, true);
            afterTP();
            loading = false;

        }

        private void UnPause()
        {
            InputHandler.Instance.PreventPause();
            GameManager.instance.isPaused = false;
            UIManager.instance.SetState(GlobalEnums.UIState.PLAYING);
            GameManager.instance.SetState(GlobalEnums.GameState.PLAYING);
            HeroController.instance.UnPause();
            TimeController.GenericTimeScale = 1f;
        }
        [Handle(Operation.SetId)]
        public void HandleId(string id)
        {
            //map.Remove(tpitem.Identity);// remove old(x) item先调用setup，所以无法获取到旧数据
            UpdateDict(id, GameManager.instance.sceneName);
            gameObject.name = $"tp_{id}";

            if (textDisp != null && !string.IsNullOrEmpty(tpitem.Identity) && !string.IsNullOrEmpty(tpitem.Destination))
            {
                map.TryGetValue(tpitem.Identity, out string from);
                map.TryGetValue(tpitem.Destination, out string to);
                textDisp.GetComponent<TextMeshPro>().text = $"{tpitem.Identity}({from})==>{tpitem.Destination}({to})";
            }
        }

        public void Awake()
        {
            sr = gameObject.GetComponent<SpriteRenderer>();
            col = gameObject.GetComponent<BoxCollider2D>();

            ht = gameObject.AddComponent<HeroTrigger>();

            if (prevent_level_activated == null)
            {
                var from = typeof(GameManager).GetMethod("LevelActivated", BindingFlags.NonPublic | BindingFlags.Instance);
                var to = typeof(TransitionGem).GetMethod("Noop", BindingFlags.NonPublic | BindingFlags.Static);
                Logger.LogDebug($"[Test Detour]{from}=>{to}");
                prevent_level_activated = new Detour(from, to);
                prevent_level_activated.Undo();
            }

            transform.localScale *= 1.5f;
        }
        private static void Noop(GameManager gm, Scene sceneFrom, Scene sceneTo)
        {
            if (gm.startedOnThisScene && gm.IsGameplayScene())
            {
                PrivateHelper.SetField(gm, "tilemapDirty", true);//gm.tilemapDirty = true;
            }
            gm.SetupSceneRefs(true);
            Logger.LogDebug("Prevent GM Method Call");
        }
        public void Consume()
        {
            Logger.LogDebug($"Consume {gameObject.name}");
            StartCoroutine(doConsume());
            IEnumerator doConsume()
            {
                col.enabled = false;
                sr.color = new Color(1, 1, 1, 0.5f);
                yield return new WaitForSeconds(2);
                sr.color = new Color(1, 1, 1, 1);
                ht.HeroEnter = TP;
                col.enabled = true;
            }
        }
        public void Start()
        {
            Consume();
            if (ItemManager.Instance.setupMode && !string.IsNullOrEmpty(tpitem.Identity) && !string.IsNullOrEmpty(tpitem.Destination))
            {
                map.TryGetValue(tpitem.Identity, out string from);
                map.TryGetValue(tpitem.Destination, out string to);
                textDisp = NameDisp.Create(transform, $"{tpitem.Identity}({from})==>{tpitem.Destination}({to})",0,0.6f);
            }
        }

        //[Handle(Operation.SetDst)]
        //public void HandleDst(string dstid)
        //{

        //}

        
    }
    
}
