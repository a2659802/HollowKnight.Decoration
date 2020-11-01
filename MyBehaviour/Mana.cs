using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.Attr;
using DecorationMaster.UI;
using DecorationMaster.Util;
using System.Collections;
using Random = UnityEngine.Random;
using Modding;
namespace DecorationMaster.MyBehaviour
{
    public enum ManaType //U,R,W,B,G,C-> WUBRGC
    {
        W,U,B,R,G,C
    }
    //These items are just for meme, not take too serious
    public class Mana
    {
        [MemeDecoration]
        [Description("魔力源，魔力源的颜色有5种，分别是蓝白绿红黑。\n接触到可以收集它")]
        [Description("Mana Source. mana is the Magic thing. it has five color.\n when hero touch it will collecte them", "en-us")]
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
                            //Logger.LogDebug($"ManaSourceSpriteAdd:{img_name}");
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

                var t = gameObject.AddComponent<HeroTrigger>();
                t.HeroEnter = HeroEnter;
                t.HeroAtk = HeroAtk;
            }
            private void HeroEnter()
            {
                ManaType selfType = ((ManaItem)item).mType;
                ManaCollector.Instance.Add(selfType);
                //Logger.LogDebug("Eat a mana source");
                Destroy(gameObject);
            }
            private void HeroAtk()
            {
                if (SetupMode)
                    Remove();
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
        
        public class ManaActive : MonoBehaviour
        {
            private float dt = 0;
            private int angle = 0;
            public CircleCollider2D col;
            private Vector3 target = Vector3.zero;
            public Vector3 rot = new Vector3(0, 0, 15);
            public ManaType MType { get; set; }
            
            private static Dictionary<ManaType, Sprite> _sprites;
            private static Dictionary<ManaType, Sprite> _sprites_highlight;
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
                            //Logger.LogDebug($"ManaSourceSpriteAdd:{img_name}");
                        }
                    }
                    return _sprites;
                }
            }
            public static Dictionary<ManaType, Sprite> SpritesHighlight
            {
                get
                {
                    if (_sprites_highlight == null)
                    {
                        _sprites_highlight = new Dictionary<ManaType, Sprite>();

                        foreach (var m in Enum.GetValues(typeof(ManaType)))
                        {
                            string img_name = $"mana_highlight_{m.ToString().ToLower()}";
                            if (!GUIController.Instance.images.ContainsKey(img_name))
                                continue;
                            var tex = GUIController.Instance.images[img_name];
                            Sprite ss = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                            _sprites_highlight.Add((ManaType)m, ss);
                            DontDestroyOnLoad(ss);
                            //Logger.LogDebug($"ManaSourceSpriteAdd:{img_name}");
                        }
                    }
                    return _sprites_highlight;
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
                        render.sortingOrder += 50;
                    }
                    return render;
                }
            }
            public void HandleMana(ManaType t,bool highlight = false)
            {
                var dict = highlight ? SpritesHighlight : Sprites;
                if (dict.TryGetValue(t, out var s))
                {
                    sr.sprite = s;
                }
                else
                {
                    Logger.LogError($"Sprites Contains {t} Key? {Sprites.ContainsKey(t)}");
                }
                MType = t;
            }
            public static ManaActive Create(Transform parent,ManaType t,bool highlight = false)
            {
                var go = new GameObject("mana_active");
                go.AddComponent<ManaActive>().HandleMana(t, highlight);
                go.transform.SetParent(parent);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector2.one;
                return go.GetComponent<ManaActive>();
            }
            private void Awake()
            {
                var go = gameObject;
                go.layer = (int)GlobalEnums.PhysLayers.HERO_ATTACK;
                col = go.AddComponent<CircleCollider2D>();
                col.radius = 0.05f;
                col.enabled = false;
            }
            private void Update()
            { 

                dt += Time.deltaTime;
                gameObject.transform.Rotate(Time.deltaTime * rot * 10);
                    
                if (dt > 1f)
                {
                    angle = Random.Range(0, 360);
                    dt = 0;
                    float randR = Random.Range(0, 10);
                    float x = randR * Mathf.Cos(angle * Mathf.PI / 180f);
                    float y = randR * Mathf.Sin(angle * Mathf.PI / 180f);
                    target.x = x;
                    target.y = y;
                }
                gameObject.transform.localPosition = Vector2.MoveTowards(gameObject.transform.localPosition, target, Time.deltaTime * 0.5f);
            }
        
            public void CostSelf()
            {
                Logger.LogDebug($"[ManaActive]Cost Self {MType}");
                Destroy(gameObject);
            }
            
        }

        [MemeDecoration]
        [Description("魔力吸收装置，收集所需魔力可以触发魔法门。\n红色的圈表示你身上携带的法术力不满足要求\n注意：无色法术力可以用任意有色法术力代替\n此物品尚未完成")]
        [Description("mana machine, it needs manas for working", "en-us")]
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

                var trigger = gameObject.AddComponent<CircleTrigger>();
                
                trigger.Check = () =>
                {
                    int gateNum = ((ItemDef.KeyGateItem)item).Number;
                    if(ManaCollector.Instance.Cost(require))
                    {
                        Logger.LogDebug("Mana Cost Success");
                        foreach(var g in FindObjectsOfType<ManaGate>().Where(x=>x.GateNum == gateNum))
                        {
                            g.Open?.Invoke();
                        }
                        StartCoroutine(Unlock());
                        IEnumerator Unlock()
                        {
                            yield return new WaitForSeconds(0.5f);
                            for(int i=50;i>20;i--)
                            {
                                sr.color = new Color(1, 1, 1, i / 50f);
                                yield return new WaitForSeconds(0.02f);
                            }
                            trigger.Check = null;
                            
                        }
                        
                        return true;
                    }
                    else
                    {
                        Logger.LogDebug("Mana Cost Failed");
                    }
                    return false;
                };
            }
            private void Start()
            {
                string reqStr = ((ItemDef.ManaRequirement)item).Requires;
                if (!string.IsNullOrEmpty(reqStr))
                {
                    Dictionary<ManaType, int> r = new Dictionary<ManaType, int>();
                    try
                    {
                        for (int i = 0; i < reqStr.Length; i++)
                        {
                            char c = reqStr[i];
                            ManaType t = (ManaType)Enum.Parse(typeof(ManaType), c.ToString());
                            if (r.ContainsKey(t))
                            {
                                r[t]++;
                            }
                            else
                            {
                                r.Add(t, 1);
                            }
                        }
                        require = r;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("ManaRequire Parse Error");
                        Logger.LogError(e);
                    }
                }
                else
                {
                    Logger.LogDebug("Not Set Requirement");
                }
                if (require == null)
                {
                    require = new Dictionary<ManaType, int> { { ManaType.U, 2 }, { ManaType.G, 3 }, { ManaType.R, 1 }, { ManaType.C, 3 }, };
                    ((ItemDef.ManaRequirement)item).Requires = "UUGGGRCCC";
                }
                UpdateRequire();
            }
            public void UpdateRequire()
            {
                if (require != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var kv in require)
                    {
                        string color = "<#ffffff>";
                        switch (kv.Key)
                        {
                            case ManaType.U:
                                color = "<#67c1f5>";
                                break;
                            case ManaType.R:
                                color = "<#f85555>";
                                break;
                            case ManaType.B:
                                color = "<#848484>";
                                break;
                            case ManaType.G:
                                color = "<#25b569>";
                                break;
                            case ManaType.W:
                                color = "<#fcfcc1>";
                                break;
                            case ManaType.C:
                                color = "<#c0c0c0>";
                                break;
                        }
                        sb.Append(color);
                        sb.Append(kv.Key.ToString()[0], kv.Value);
                        sb.Append("</color>");
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
            
        }

        [MemeDecoration]
        [Description("魔法门，需要用魔力吸收装置才能打开")]
        [Description("mana gate, which can open by mana machine", "en-us")]
        [Decoration("Mana_Wall")]
        public class ManaWall : Resizeable
        {
          
            private ManaGate gate;
            private SpriteRenderer sr;
            private float t = 0;
            private void Awake()
            {
                sr = gameObject.AddComponent<SpriteRenderer>();
                var tex = GUIController.Instance.images["seal_wall"];
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                gameObject.AddComponent<BoxCollider2D>().size = new Vector2(1, 4.5f);
                gameObject.layer = 0; // 8
                if (gate == null) 
                    gate = gameObject.AddComponent<ManaGate>();
                gate.Open = () => Destroy(gameObject);

                UnVisableBehaviour.AttackReact.Create(gameObject);
            }
            private void Update()
            {
                t += Time.deltaTime;
                float a = 0.4f * Mathf.Cos(t * 2) + 0.6f;
                sr.color = new Color(1, 1, 1, a);
            }

            [Handle(Operation.SetGate)]
            public void HandleGate(int num)
            {
                if (gate == null)
                    gate = gameObject.AddComponent<ManaGate>();
                gate.GateNum = num;
            }
        }
        public class ManaGate : MonoBehaviour
        {
            public int GateNum;
            public Action Open;
            public Action Close;
        }
    }
    internal class ManaCollector : MonoBehaviour
    {
        private Dictionary<ManaType, int> mana_pool;
        private List<Mana.ManaActive> manas = new List<Mana.ManaActive>();
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
        
        private void Awake()
        {
            if(HeroController.instance != null)
            {
                gameObject.transform.SetParent(HeroController.instance.transform);
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.AddComponent<KeepWorldScalePositive>();
                ModHooks.Instance.AfterPlayerDeadHook += ClearManaPool;
                GameManager.instance.ResetSemiPersistentObjects += ClearManaPool;
            }
        }


        private void OnDestroy()
        {
            GameManager.instance.ResetSemiPersistentObjects -= ClearManaPool;
            ModHooks.Instance.AfterPlayerDeadHook -= ClearManaPool;
        }
        private void ClearManaPool()
        {
            foreach (var m in manas)
                Destroy(m);
            mana_sum = 0;
            mana_pool = new Dictionary<ManaType, int>();
            Destroy(gameObject);
            Logger.LogDebug("Destroy Mana Collecotr on death");
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
            for (int i = 0; i < amount; i++)
                manas.Add(Mana.ManaActive.Create(gameObject.transform, mType));
            Logger.LogDebug($"Mana Pool Add - {mType} amount:{amount}");
        }
        private void _remove_mana_go(ManaType t,int amount = 1)
        {
            List<Mana.ManaActive> toRemove = null;
            if(t != ManaType.C)
            {
                toRemove = manas.Where(x => (x != null && x.MType == t)).ToList();
            }
            else
            {
                toRemove = manas.Where(x => (x != null && x.MType != t)).ToList();
            }
            for(int i=0;i<amount;i++)
            {
                //GameObject target = toRemove[0].gameObject;
                manas.Remove(toRemove[i]);
                toRemove[i].CostSelf();
                //toRemove.Remove(toRemove[0]);
            }
        }
        private bool Cost(ManaType mType, int amount = 1)
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
                        _remove_mana_go(mType, amount);
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
                    foreach(var k in mana_pool.Keys.ToArray())
                    {

                        if(mana_pool[k] < remains)
                        {
                            remains -= mana_pool[k];
                            _remove_mana_go(k, mana_pool[k]);
                            mana_pool[k] = 0;
                        }
                        else
                        {
                            mana_pool[k] -= remains;
                            _remove_mana_go(k, remains);
                            remains = 0;
                            break;
                        }
                    }
                    mana_sum -= amount;
                    return true;
                }
            }
            
        }
        /*public bool TryCost(ManaType mType,int amount = 1)
        {
            if(mType != ManaType.C)
            {
                if (Cost(mType, amount))
                {
                    Add(mType, amount);
                    return true;
                }
            }
            else if(mana_sum >= amount)
            {
                return true;
            }
            return false;

        }*/

        public bool TryCost(Dictionary<ManaType,int> allCost)
        {
            var colorful = allCost.Where(x => x.Key != ManaType.C);
            //var colorless = allCost.Where(x => x.Key == ManaType.C);
            int colorfulAmount = 0;
            bool flag = true;
            foreach(var kv in colorful)
            {
                flag &= (mana_pool.ContainsKey(kv.Key) && mana_pool[kv.Key] >= kv.Value);
                if (!flag)
                    break;
                colorfulAmount += kv.Value;
            }
            if (!flag)
                return false;
            if (allCost.ContainsKey(ManaType.C))
            {
                int colorlessAmount = allCost[ManaType.C];
                if (colorlessAmount <= (mana_sum - colorfulAmount))
                    return true;
            }
            else
                return true;
            return false;
        }
        public bool Cost(Dictionary<ManaType,int> allCost)
        {
            bool flag = true;
            if (allCost == null)
                return true;
            /*foreach(var cost in allCost)
            {
                flag &= TryCost(cost.Key, cost.Value);
                Logger.LogDebug($"Try ManaCost: {cost.Key},{cost.Value},result:{flag}");
            }*/
            flag &= TryCost(allCost);

            if(flag)
            {
                foreach (var cost in allCost)
                {
                    flag &= Cost(cost.Key, cost.Value);
                   
                    Logger.LogDebug($"ManaCost: {cost.Key},{cost.Value},result:{flag}");
                }
            }
            
            return flag;
        }
        public Dictionary<ManaType, int> GetPool() { return mana_pool; }

    }

    public class CircleTrigger : MonoBehaviour
    {
        public GameObject circle;
        public SpriteRenderer sr;
        public delegate bool ValidCheck();
        public ValidCheck Check;
        private void Awake()
        {
            circle = new GameObject("circle");
            circle.transform.SetParent(gameObject.transform);
            circle.layer = (int)GlobalEnums.PhysLayers.PROJECTILES;
            var tex = GUIController.Instance.images["circle"];
            Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            sr = circle.AddComponent<SpriteRenderer>();
            sr.sprite = s;
            var col = circle.AddComponent<CircleCollider2D>();
            col.radius = 0.45f;
            col.isTrigger = true;
            var t = circle.AddComponent<HeroTrigger>();
            t.HeroEnter = Enter;
            t.HeroExit = Exit;
            
            circle.transform.localScale = Vector3.one * 3;
            circle.transform.localPosition = Vector3.zero;
            sr.enabled = false;
            //circle.AddComponent<ShowColliders>();
            //sr.enabled = false;
        }
        private void Enter()
        {
            //Logger.LogDebug("Hero Enter");
            if (Check != null)
            {
                sr.enabled = true;
                if(Check())
                {
                    sr.color = Color.green;
                }
                else
                {
                    sr.color = Color.red;
                }
                
            }
        }
        private void Exit()
        {
            sr.enabled = false;
        }
    }
    public class HeroTrigger : MonoBehaviour
    {
        public Action HeroEnter;
        public Action HeroAtk;
        public Action HeroExit;
        public Action HeroStay;
        private void OnTriggerEnter2D(Collider2D col)
        {
            int layer = col.gameObject.layer;
            if (layer == (int)GlobalEnums.PhysLayers.HERO_BOX || layer == (int)GlobalEnums.PhysLayers.PLAYER)
            {
                HeroEnter?.Invoke();
            }
            else if (layer == (int)GlobalEnums.PhysLayers.HERO_ATTACK && col.name.Contains("Slash"))
            {
                HeroAtk?.Invoke();
            }
        }
        private void OnTriggerExit2D(Collider2D col)
        {
            HeroExit?.Invoke();
        }
        private void OnTriggerStay2D(Collider2D col)
        {
            HeroStay?.Invoke();
        }
    }
}
