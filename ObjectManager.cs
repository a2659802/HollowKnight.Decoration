﻿using DecorationMaster.Attr;
using DecorationMaster.MyBehaviour;
using DecorationMaster.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using ModCommon.Util;

namespace DecorationMaster
{
    // Create a objectpool name InstantiableObjects which can be access with name
    // but those object are not contains custom monobehaviour
    // to add behaviour, you should make a class with DecorationAttribute which value is name, and then
    // register will automatically add these components to pool prefab
    public static partial class ObjectLoader
    {
        public static readonly Dictionary<(string, Func<GameObject, GameObject>), (string, string)> ObjectList = new Dictionary<(string, Func<GameObject, GameObject>), (string, string)>
        {
            {
                ("saw", null),
                ("White_Palace_18","saw_collection/wp_saw")
            },
            { 
                ("trap_spike",null),("White_Palace_07","wp_trap_spikes")
            },
            {
                ("flip_platform",null),("Mines_31","Mines Platform")
            },
            {
                ("conveyor",null),("Mines_31","conveyor_belt_01")
            },
            {
                ("gate",(go)=>{
                    go.LocateMyFSM("Toll Gate").SetState("Idle");
                    return go;
                }),
                ("Crossroads_03", "_Props/Toll Gate")
            },
            {
                ("lever",(go)=>{
                    PlayMakerFSM playMakerFSM = go.LocateMyFSM("toll switch");
                    FsmutilExt.RemoveAction(playMakerFSM, "Initiate", 4);
                    FsmutilExt.RemoveAction(playMakerFSM, "Initiate", 3);
                    playMakerFSM.SetState("Pause");
                    playMakerFSM.RemoveTransition("Initiate","ACTIVATED");
                    playMakerFSM.Fsm.SaveActions();
                    return go;
                }),
                ("Crossroads_03", "_Props/Toll Gate Switch")
            },
            {
                ("spike",null),
                ("Crossroads_25","Cave Spikes tile")
            },
            {
                ("soul_totem",null),
                ("Crossroads_25","Soul Totem mini_two_horned")
            },
            {
                ("respawn_point",null),
                ("Crossroads_25","Hazard Respawn Trigger v2")
            },
            {
                ("lazer_bug",null),
                ("Mines_05","Crystallised Lazer Bug")
            },
            {
                ("platform_rect",null),
                ("Mines_05","plat_float_08")
            },
            {
                ("crystal_barrel",null),
                ("Mines_05","crystal_barrel")
            },
            {
                ("platform_small",null),
                ("Mines_05","plat_float_03")
            },
            {
                ("crystal",null),
                ("Mines_05","brk_Crystal3")
            },
            {
                ("fly",null), ("White_Palace_18","White Palace Fly")
            },
            {
                ("bounce_shroom",null),
                ("Fungus2_11","Bounce Shroom B")
            },
            {
                ("turret",null),
                ("Fungus2_11","Mushroom Turret")
            },
            {
                ("stomper",null),
                ("Mines_19","_Scenery/stomper_1")
            }
            /*
            {
                ("cameralock",null),
                ("Crossroads_25","CameraLockArea")
            },
            */

            
        };
        public static Dictionary<string, GameObject> InstantiableObjects { get; } = new Dictionary<string, GameObject>();
        public static GameObject CloneDecoration(string key)
        {
            Modding.Logger.LogDebug($"On Clone {key}");
            GameObject go = null;
            if(InstantiableObjects.TryGetValue(key,out GameObject prefab))
            {
                Modding.Logger.LogDebug($"On Clone {prefab.name}");
                Item prefab_item = prefab.GetComponent<CustomDecoration>()?.item;
                Modding.Logger.LogDebug($"On Clone {prefab_item}");
                if (prefab_item == null)
                    return null;
                Item item = prefab_item.Clone() as Item;
                go = Object.Instantiate(prefab);
                go.name = go.name.Replace("(Clone)", "");
                //go.GetComponent<CustomDecoration>().item = item;
                go.GetComponent<CustomDecoration>().Setup(Operation.Serialize, item);
                Modding.Logger.LogDebug($"Clone Item:{prefab_item==null},{item==null}");
            }
            return go;
        }
        public static void Load(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            static GameObject Spawnable(GameObject obj, Func<GameObject, GameObject> modify)
            {
                GameObject go = Object.Instantiate(obj);
                go = modify?.Invoke(go) ?? go;
                Object.DontDestroyOnLoad(go);
                go.transform.localScale = Vector3.one;
                go.SetActive(false);
                return go;
            }

            // ReSharper disable once SuggestVarOrType_DeconstructionDeclarations
            foreach (var ((name, modify), (room, go_name)) in ObjectList)
            {
                if (!preloadedObjects[room].TryGetValue(go_name, out GameObject go))
                {
                    Logger.LogWarn($"[DecorationMaster]: Unable to load GameObject {go_name}");

                    continue;
                }

                InstantiableObjects.Add($"HK_{name}", Spawnable(go, modify));
            }

            static GameObject ImageSpawnable(string imgN)
            {
                var imggo = ImageLoader.CreateImageGo(imgN);
                Object.DontDestroyOnLoad(imggo);
                imggo.transform.localScale = Vector3.one;
                imggo.SetActive(false);
                return imggo;
            }
            ImageLoader.Load();
            foreach(var imgName in ImageLoader.images.Keys)
            {
                InstantiableObjects.Add($"IMG_{imgName}", ImageSpawnable(imgName));
            }

            Logger.LogDebug($"ObjectLoader: Load done,Count:{InstantiableObjects.Count}");
            foreach(var k in InstantiableObjects.Keys)
            {
                Logger.LogDebug(k);
            }
        }
        public static class ImageLoader
        {
            public static readonly Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();
            public static readonly Dictionary<string, byte[]> raw_images = new Dictionary<string, byte[]>();
            public static bool loaded { get; private set; }
            public static void Load()
            {
                if (loaded)
                    return;

                string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (string res in resourceNames)
                {
                    Logger.LogDebug($"Find Embeded Resource:{res}");
                    if (res.EndsWith(".png"))
                    {
                        try
                        {
                            Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res);
                            byte[] buffer = new byte[imageStream.Length];
                            imageStream.Read(buffer, 0, buffer.Length);
                            string[] split = res.Split('.');
                            string internalName = split[split.Length - 2];

                            if (res.Contains("images.objects"))
                            {
                                Texture2D tex = new Texture2D(1, 1);
                                tex.LoadImage(buffer.ToArray());
                                images.Add(internalName, tex);
                            }
                            else
                            {
                                raw_images.Add(internalName, buffer);
                            }
                        }
                        catch
                        {
                            loaded = false;
                        }
                    }
                }
                loaded = true;
            }
            public static GameObject CreateDestroyableTex(Texture2D tex)
            {
                var go = new GameObject { name = "DestroyableTexture" };
                var sprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f));
                go.SetActive(false);
                go.AddComponent<SpriteRenderer>().sprite = sprite;
                go.AddComponent<BoxCollider2D>().size = new Vector2(1.2f, 1.2f);
                go.layer = (int)GlobalEnums.PhysLayers.HERO_ATTACK;
                //To Add some component HERE
                return go;
            }
            public static GameObject CreateImageGo(string texName)
            {
                if (!images.ContainsKey(texName))
                    return null;

                var tex = images[texName];
                var go = CreateDestroyableTex(tex);

                go.name = $"D_IMG_{texName}";
                return go;
            }
        }
        
    }

    public static class BehaviourProcessor
    {
        public static void RegisterBehaviour<T>()
        {
            Type[] behaviours = typeof(T).GetNestedTypes(BindingFlags.Public);
            Type[] items = typeof(ItemDef).GetNestedTypes(BindingFlags.Public | BindingFlags.Instance);
            foreach (Type b in behaviours)
            {
                DecorationAttribute attr = b.GetCustomAttributes(typeof(DecorationAttribute), false).OfType<DecorationAttribute>().FirstOrDefault();
                if (attr == null)
                    continue;

                string poolname = attr.Name;
                if(ObjectLoader.InstantiableObjects.ContainsKey(poolname))
                {
                    GameObject prefab = ObjectLoader.InstantiableObjects[poolname];
                    CustomDecoration d = prefab.AddComponent(b) as CustomDecoration;
                    foreach(Type i in items)
                    {
                        DecorationAttribute i_attr = i.GetCustomAttributes(typeof(DecorationAttribute), false).OfType<DecorationAttribute>().FirstOrDefault();
                        if (i_attr == null)
                            continue;
                        if(i_attr.Name == attr.Name)   //if(i_attr.Equals(attr))
                        {
                            var item = Activator.CreateInstance(i) as Item;
                            item.pname = poolname;
                            d.item = item;
                            Logger.LogDebug($"Match Item-Decoration:{item}-{d}");
                            break;
                        }
                    }
                    if(d.item == null)
                    {
                        Logger.LogWarn($"Could Not Found an Item that match {b.FullName},Attr:{attr.Name},will use default item instance");
                        d.item = new ItemDef.DefaultItem { pname = poolname };
                    }
                }
            }
        }
        public static void RegisterSharedBehaviour<T>()
        {
            var shareAttr = typeof(T).GetCustomAttributes(typeof(DecorationAttribute), false).OfType<DecorationAttribute>();
            foreach(var attr in shareAttr)
            {
                if (attr == null)
                    continue;
                string poolname = attr.Name;
                if (!ObjectLoader.InstantiableObjects.TryGetValue(poolname, out GameObject prefab))
                    continue;
                var d = prefab.AddComponent(typeof(T)) as CustomDecoration;
                var ti = typeof(T).GetNestedTypes(BindingFlags.Public).Where(x => x.IsSubclassOf(typeof(Item))).FirstOrDefault();
                d.item = Activator.CreateInstance(ti) as Item;
                d.item.pname = poolname;
                Logger.LogDebug($"Match {ti}-Decoration:{d}");
            }
        }
    }
}
