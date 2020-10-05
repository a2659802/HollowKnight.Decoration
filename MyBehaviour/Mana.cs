using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.Attr;
using DecorationMaster.UI;
using DecorationMaster.Util;
namespace DecorationMaster.MyBehaviour
{
    public enum ManaType
    {
        U,R,W,B,G,C
    }
    public class Mana
    {
        [Decoration("Mana_Source")]
        public class ManaSource : CustomDecoration
        {
            private static Dictionary<ManaType, Sprite> _sprites;
            public static Dictionary<ManaType, Sprite> Sprites
            {
                get
                {
                    if(_sprites == null)
                    {
                        _sprites = new Dictionary<ManaType, Sprite>();
                        
                        foreach (var m in Enum.GetValues(typeof(ManaType)))
                        {
                            string img_name = $"mana_source_{m.ToString().ToLower()}";
                            if (!GUIController.Instance.images.ContainsKey(img_name))
                                continue;
                            var tex = GUIController.Instance.images[img_name];
                            Sprite ss = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                            _sprites.Add((ManaType)m, ss);
                            DontDestroyOnLoad(ss);
                            Logger.LogDebug($"ManaSourceSpriteAdd:{img_name}");
                        }
                    }
                    return _sprites;
                }
            }
            private SpriteRenderer sr { get {
                    var render = gameObject.GetComponent<SpriteRenderer>();
                    if (render == null)
                    {
                        render = gameObject.AddComponent<SpriteRenderer>();
                    }
                    return render;
                } }

            private void Awake()
            {
                gameObject.layer = (int)GlobalEnums.PhysLayers.PROJECTILES;
                BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
                col.size = Vector2.one;
                col.isTrigger = true;
                

                /*ManaType selfType = ((ManaItem)item).mType;
                Sprite s = Sprites[selfType];
                sr.sprite = s;*/
            }
            private void OnTriggerEnter2D(Collider2D col)
            {
                int layer = col.gameObject.layer;
                if (layer == (int)GlobalEnums.PhysLayers.HERO_BOX)
                {
                    ManaType selfType = ((ManaItem)item).mType;
                    ManaCollector.Instance.Add(selfType);
                    Logger.LogDebug("Eat a mana source");
                    Destroy(gameObject);
                }
                else if(layer == (int)GlobalEnums.PhysLayers.HERO_ATTACK)
                {
                    if (SetupMode)
                        Remove();
                }
               
            }

            [Handle(Operation.SetMana)]
            public void HandleMana(ManaType t)
            {
                
                if (Sprites.TryGetValue(t, out var s))
                {
                    sr.sprite = s;
                }
                else
                {
                    Logger.LogError($"Sprites Contains {t} Key? {Sprites.ContainsKey(t)}");
                }
            }
        }
        
        public class ManaActive : CustomDecoration
        {
            private static Dictionary<ManaType, Sprite> _sprites;
            public static Dictionary<ManaType, Sprite> Sprites
            {
                get
                {
                    if (_sprites == null)
                    {
                        _sprites = new Dictionary<ManaType, Sprite>();

                        foreach (var m in Enum.GetValues(typeof(ManaType)))
                        {
                            string img_name = $"mana_active_{m.ToString().ToLower()}";
                            if (!GUIController.Instance.images.ContainsKey(img_name))
                                continue;
                            var tex = GUIController.Instance.images[img_name];
                            Sprite ss = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                            _sprites.Add((ManaType)m, ss);
                            DontDestroyOnLoad(ss);
                            Logger.LogDebug($"ManaSourceSpriteAdd:{img_name}");
                        }
                    }
                    return _sprites;
                }
            }
            private SpriteRenderer sr
            {
                get
                {
                    var render = gameObject.GetComponent<SpriteRenderer>();
                    if (render == null)
                    {
                        render = gameObject.AddComponent<SpriteRenderer>();
                    }
                    return render;
                }
            }
            
            [Handle(Operation.SetMana)]
            public void HandleMana(ManaType t)
            {

                if (Sprites.TryGetValue(t, out var s))
                {
                    sr.sprite = s;
                }
                else
                {
                    Logger.LogError($"Sprites Contains {t} Key? {Sprites.ContainsKey(t)}");
                }
            }
        }
        [Decoration("Mana_Requirement")]
        public class ManaRequireShower : CustomDecoration
        {
            private Dictionary<ManaType, int> require;
            private GameObject inspect;

            public static GameObject inspect_prefab => ObjectLoader.InstantiableObjects["HK_inspect_region"];
            private void Awake()
            {
              

                var tex = GUIController.Instance.images["require_shower"];
                var sr = gameObject.AddComponent<SpriteRenderer>();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                inspect = Instantiate(inspect_prefab);
                if(SetupMode)
                    inspect.AddComponent<ShowColliders>();
                inspect.transform.SetParent(gameObject.transform);
                inspect.SetActive(true);
                inspect.transform.localPosition = Vector3.up * 2f;
                
            }
            private void Start()
            {
                if(require != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(var kv in require)
                    {
                        sb.Append(kv.Key.ToString()[0], kv.Value);
                    }
                    UpdateDialogue(sb.ToString());
                }
            }
            public void UpdateDialogue(string requireText)
            {
                if (string.IsNullOrEmpty(requireText))
                    return;
                string val = requireText;
                string key = $"ManaRquire_{val.GetHashCode()}";
                if (DLanguage.MyLan.ContainsKey(key))
                    DLanguage.MyLan[key] = val;
                else
                    DLanguage.MyLan.Add(key, val);
                
                inspect.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmString("Game Text Convo").Value = key;
            }
            /*private static Dictionary<ManaType, Sprite> _sprites;
            public static Dictionary<ManaType, Sprite> Sprites
            {
                get
                {
                    if (_sprites == null)
                    {
                        _sprites = new Dictionary<ManaType, Sprite>();

                        foreach (var m in Enum.GetValues(typeof(ManaType)))
                        {
                            string img_name = $"mana_require_{m.ToString().ToLower()}";
                            if (!GUIController.Instance.images.ContainsKey(img_name))
                                continue;
                            var tex = GUIController.Instance.images[img_name];
                            Sprite ss = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                            _sprites.Add((ManaType)m, ss);
                            DontDestroyOnLoad(ss);
                            Logger.LogDebug($"ManaSourceSpriteAdd:{img_name}");
                        }
                    }
                    return _sprites;
                }
            }
            private SpriteRenderer sr
            {
                get
                {
                    var render = gameObject.GetComponent<SpriteRenderer>();
                    if (render == null)
                    {
                        render = gameObject.AddComponent<SpriteRenderer>();
                    }
                    return render;
                }
            }*/

        }
    }
    internal class ManaCollector : MonoBehaviour
    {
        private Dictionary<ManaType, int> mana_pool;
        private int mana_sum = 0;

        private static ManaCollector _instance;
        public static ManaCollector Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<ManaCollector>();
                    if (_instance == null)
                    {
                        Logger.LogWarn(" Couldn't find ManaCollector");

                        GameObject manac = new GameObject();
                        _instance = manac.AddComponent<ManaCollector>();
                        DontDestroyOnLoad(manac);
                        
                        _instance.mana_pool = new Dictionary<ManaType, int>();
                    }
                }
                return _instance;
            }
        }
    
        public void Add(ManaType mType,int amount = 1)
        {
            if(amount < 0)
            {
                throw new ArgumentException();
            }
            if(mana_pool.TryGetValue(mType,out int n))
            {
                mana_pool[mType] = n + amount;
            }
            else
            {
                mana_pool.Add(mType, amount);
            }
            mana_sum += amount;

            Logger.LogDebug($"Mana Pool Add - {mType} amount:{amount}");
        }
        public bool Cost(ManaType mType, int amount = 1)
        {
            if (amount < 0)
            {
                throw new ArgumentException();
            }
            else if (amount == 0)
                return true;

            if(mType != ManaType.C)
            {
                if (mana_pool.TryGetValue(mType, out int n))
                {
                    if (n >= amount)
                    {
                        mana_pool[mType] = n - amount;
                        mana_sum -= amount;
                        return true;
                    }
                }
                return false;
            }
            else // cost any color
            {
                if (mana_sum < amount)
                    return false;
                else
                {
                    int remains = amount;
                    foreach(var kv in mana_pool)
                    {

                        if(kv.Value < remains)
                        {
                            remains -= kv.Value;
                            mana_pool[kv.Key] = 0;
                        }
                        else
                        {
                            mana_pool[kv.Key] -= remains;
                        }
                    }
                    mana_sum -= amount;
                    return true;
                }
            }
            
        }
        public bool TryCost(ManaType mType,int amount = 1)
        {
            if (Cost(mType, amount))
            {
                Add(mType, amount);
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool Cost(Dictionary<ManaType,int> allCost)
        {
            bool flag = true;
            foreach(var cost in allCost)
            {
                flag &= TryCost(cost.Key, cost.Value);
            }
            if(flag)
            {
                foreach (var cost in allCost)
                {
                    flag &= Cost(cost.Key, cost.Value);
                }
            }
            return flag;
        }
        public Dictionary<ManaType, int> GetPool() { return mana_pool; }

    }
}
