using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using Modding;
using UnityEngine;
using ModCommon;
using System.Collections;
using DecorationMaster.MyBehaviour;
namespace DecorationMaster.UI
{
    public static class PickPanel
    {
        public static GameObject[] Prefabs = new GameObject[1];

        private static CanvasPanel panel;
        private static CanvasImage selected;
        public static int current_focus { get; private set; } = -1;
        public static int current_group_count = 0;
        public static void BuildMenu(GameObject canvas)
        {
            panel = new CanvasPanel(canvas, new Texture2D(1, 1), new Vector2(0,0),Vector2.zero, new Rect(0, 0, 1, 1));

            //panel.AddText("Panel Label", "Pick", new Vector2(0f, -20f), Vector2.zero, GUIController.Instance.trajanBold, 30);
            //pickupBorder

            var img_pickborder = GUIController.Instance.images["pickupBorder"];
            var img_itemborder = GUIController.Instance.images["itemBorder"];
            var img_selected = GUIController.Instance.images["selected"];
            var img_defaultIcon = GUIController.Instance.images["defaultIcon"];
            var img_savedborder = GUIController.Instance.images["savedBorder"];

            panel.AddPanel("Border", img_pickborder, new Vector2((1920f - img_pickborder.width) / 2, 0), Vector2.zero, new Rect(0,0, img_pickborder.width, img_pickborder.height));
            Vector2 itemborderOffset = Vector2.zero;
            for(int i=1;i<=ItemManager.GroupMax;i++)
            {
                var itemPanel = panel.GetPanel("Border").AddPanel($"ItemBorder{i}", img_itemborder, itemborderOffset, Vector2.zero, new Rect(0, 0, img_itemborder.width, img_itemborder.height));
                itemPanel.AddButton($"Item_{i}", img_defaultIcon, Vector2.zero, Vector2.zero, ItemClicked, new Rect(0, 0, img_defaultIcon.width, img_defaultIcon.height),GUIController.Instance.arial,$"Item_{i}");
                itemborderOffset.x += img_itemborder.width + 3f;
            }
            itemborderOffset.x += img_savedborder.width + 3f;
            var prefabPanel = panel.GetPanel("Border").AddPanel($"PrefabBorder{1}", img_savedborder, itemborderOffset, Vector2.zero, new Rect(0, 0, img_savedborder.width, img_savedborder.height));
            prefabPanel.AddButton($"Prefab_{1}", img_defaultIcon, Vector2.zero, Vector2.zero, PrefabClicked, new Rect(0, 0, img_defaultIcon.width, img_defaultIcon.height), GUIController.Instance.arial, $"Prefab_{1}");
            
            selected = panel.GetPanel("Border").AddImage("selected", img_selected, Vector2.zero, Vector2.zero, new Rect(0, 0, img_selected.width, img_selected.height));

            ItemManager.Instance.GroupSwitchEventHandler += UpdateItemList;
            LogDebug("PickPanel Built");

        }
        public static void Focus(int i=0)
        {
            var itemPanel = panel.GetPanel("Border").GetPanel($"ItemBorder{i}");
            if (itemPanel == null)
                return;

            Vector2 pos = itemPanel.GetPosition();
            selected.SetPosition(pos);
            current_focus = i;
            selected.SetActive(true);
        }
        public static void UnFocus()
        {
            current_focus = -1;
            selected.SetActive(false);
        }
        public static int SelectFocus()
        {
            if(current_focus >= 100)
            {
                var pidx = current_focus % 100;
                var prefab = pidx < Prefabs.Length ? Prefabs[pidx] : null;
                ItemManager.Instance.Select(prefab);
            }
            return current_focus;
        }
        private static void ItemClicked(string btnName)
        {
            int itemIdx = Convert.ToInt32(btnName.Split('_')[1]);
            LogDebug(itemIdx);
            HeroController.instance.StartCoroutine(WaitFrame());

            IEnumerator WaitFrame()
            {
                yield return new WaitForEndOfFrame();
                Focus(itemIdx);
            }
        }
        private static void _setPrefab(GameObject go)
        {
            if (Prefabs[0] != null)
                UnityEngine.Object.Destroy(Prefabs[0]);
            Prefabs[0] = go;
            UnityEngine.Object.DontDestroyOnLoad(go);
        }
        public static void SetPrefab(Texture2D tex,GameObject go)
        {
            GetPrefabButton(1).UpdateSprite(tex, new Rect(0, 0, tex.width, tex.height));
            _setPrefab(go);
        }
        public static void SetPrefab(Sprite s,GameObject go)
        {
            GetPrefabButton(1).UpdateSprite(s, new Rect());
            _setPrefab(go);

        }
        public static void SetCurrentToPrefab()
        {
            GameObject go = ItemManager.Instance.currentSelect;
            if (go == null)
                return;
            int index = -1;
            string pn = go.GetComponent<CustomDecoration>().item.pname;
            string[] pnames = ItemManager.group[ItemManager.Instance.CurrentGroup];
            for(int i=0;i<pnames.Length;i++)
            {
                if(pnames[i] == pn)
                {
                    index = i;
                    break;
                }
            }
            go = go.GetComponent<CustomDecoration>().Setup(Operation.COPY, null) as GameObject;
            go?.SetActive(false);
            Sprite s = GetButton(index + 1).GetSprite();
            SetPrefab(s, go);
        }

        private static void PrefabClicked(string btnName)
        {
            if (Prefabs[0] == null)
                return;
            HeroController.instance.StartCoroutine(WaitFrame());

            var pos = panel.GetPanel("Border").GetPanel($"PrefabBorder{1}").GetPosition();
            selected.SetPosition(pos);
            current_focus = 100;
            selected.SetActive(true);

            IEnumerator WaitFrame()
            {
                yield return new WaitForEndOfFrame();
                ItemManager.Instance.Select(Prefabs[0]);
            }

        }
        public static bool ActiveSelf()
        {
            return panel.active;
        }
        public static void SetActive(bool b)
        {
            panel.SetActive(b, true);
            selected.SetActive(false);
        }
        public static void Update()
        {
            if (panel == null)
            {
                return;
            }

            if (DecorationMaster.GM == null ||DecorationMaster.GM.IsNonGameplayScene())
            {
                if (panel.active)
                {
                    SetActive(false);
                }

                return;
            }
            
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SetActive(!panel.active);
                
                Logger.LogDebug("Toggle PickPanel");
            }

            if (panel.active)
            {
                if (Input.GetMouseButtonUp((int)MouseButton.Right)) 
                {
                    UnFocus();
                }
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    SetCurrentToPrefab();
                }
            }
        }
    
        
        private static CanvasButton GetButton(int i = 1)
        {
            var btn = panel.GetPanel("Border").GetPanel($"ItemBorder{i}").GetButton($"Item_{i}");
            return btn;
        }
        private static CanvasButton GetPrefabButton(int i = 1)
        {
            var btn = panel.GetPanel("Border").GetPanel($"PrefabBorder{i}").GetButton($"Prefab_{i}");
            return btn;
        }
    
        public static void UpdateItemList(string[] poolNames)
        {
            SetActive(true);
            UnFocus();
            current_group_count = poolNames.Length;
            for(int i=0;i<poolNames.Length;i++)
            {
                string p = poolNames[i];
                LogDebug(p);
                if(p.StartsWith("IMG_")) //apply itself images
                {
                    string imgName = p.Split('_')[1];
                    var tex = ObjectLoader.ImageLoader.images[imgName];
                    GetButton(i+1)?.UpdateSprite(tex, new Rect(0,0,tex.width,tex.height));
                    GetButton(i + 1)?.UpdateText("");
                }
                else
                {
                    if(GUIController.Instance.images.TryGetValue(p,out var customTex)) //apply custom imgage
                    {
                        GetButton(i + 1)?.UpdateSprite(customTex, new Rect(0, 0, customTex.width, customTex.height));
                        GetButton(i + 1)?.UpdateText("");
                    }
                    else
                    {
                        var objprefab = ObjectLoader.InstantiableObjects[p];
                        Sprite s = objprefab.GetComponent<SpriteRenderer>()?.sprite;
                        var btn = GetButton(i + 1); 
                        if (s != null) //try to apply gameobject's sprite
                        {
                            btn.UpdateSprite(s, new Rect());
                        }
                        else // apply empty image
                        {
                            var img_defaultIcon = GUIController.Instance.images["defaultIcon"];
                            GetButton(i + 1)?.UpdateSprite(img_defaultIcon, new Rect(0, 0, img_defaultIcon.width, img_defaultIcon.height));
                        }
                        GetButton(i + 1)?.UpdateText(p);
                    }
                }
                
                GetButton(i + 1)?.SetActive(true);
            }
            for(int i=poolNames.Length+1;i<= ItemManager.GroupMax;i++) //hide unused button
            {
                GetButton(i)?.SetActive(false);
            }
            if(!ItemManager.Instance.setupMode)
            {
                SetActive(false);
            }
        }

        private static void LogDebug(object o)
        {
            Logger.LogDebug($"[PickPanel]{o}");
        }
    }
}
