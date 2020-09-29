using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using Modding;
using UnityEngine;
using ModCommon;
namespace DecorationMaster.UI
{
    public static class PickPanel
    {
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

            panel.AddPanel("Border", img_pickborder, new Vector2((1920f - img_pickborder.width) / 2, 0), Vector2.zero, new Rect(0,0, img_pickborder.width, img_pickborder.height));
            Vector2 itemborderOffset = Vector2.zero;
            for(int i=1;i<=ItemManager.GroupMax;i++)
            {
                var itemPanel = panel.GetPanel("Border").AddPanel($"ItemBorder{i}", img_itemborder, itemborderOffset, Vector2.zero, new Rect(0, 0, img_itemborder.width, img_itemborder.height));
                itemborderOffset.x += img_itemborder.width + 3f;
                itemPanel.AddButton($"Item_{i}", img_defaultIcon, Vector2.zero, Vector2.zero, ItemClicked, new Rect(0, 0, img_defaultIcon.width, img_defaultIcon.height),GUIController.Instance.arial,$"Item_{i}");
            }
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
            return current_focus;
        }
        private static void ItemClicked(string btnName)
        {
            int itemIdx = Convert.ToInt32(btnName.Split('_')[1]);
            LogDebug(itemIdx);

            Focus(itemIdx);
        }
        public static bool ActiveSelf()
        {
            return panel.active;
        }
        public static void SetActive(bool b)
        {
            panel.SetActive(b, true);
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
                    panel.SetActive(false, true);
                }

                return;
            }
            
            if (Input.GetKeyDown(KeyCode.F5))
            {
                panel.SetActive(!panel.active, true);
                Logger.LogDebug("Toggle PickPanel");
            }

            if (panel.active)
            {
                if (Input.GetMouseButtonUp((int)MouseButton.Right)) 
                {
                    UnFocus();
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {

                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {

                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {

                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    //current_selected = (current_selected + 1) % ItemManager.GroupMax;
                    //Focus(current_selected);
                }
            }
        }
    
        public static void SetItemTexture(int i,Texture2D tex)
        {
            var btn = panel.GetPanel("Border").GetPanel($"ItemBorder{i}").GetButton($"Item_{i}");
            if(btn == null)
            {
                Logger.LogWarn($"Item{i} not Found");
                return;
            }
            if(tex == null)
            {
                Logger.LogError($"Item{i}'s texture2d null");
                return;
            }
            btn.UpdateSprite(tex, new Rect(0,0,tex.width,tex.height));
        }
        public static void SetBtnActive(int i,bool b)
        {
            var btn = panel.GetPanel("Border").GetPanel($"ItemBorder{i}").GetButton($"Item_{i}");
            if (btn == null)
            {
                Logger.LogWarn($"Item{i} not Found");
                return;
            }
            btn.SetActive(b);
        }
        public static void SetItemTxt(int i,string text="")
        {
            var btn = panel.GetPanel("Border").GetPanel($"ItemBorder{i}").GetButton($"Item_{i}");
            if (btn == null)
            {
                Logger.LogWarn($"Item{i} not Found");
                return;
            }
            btn.UpdateText(text);
        }
    
        public static void UpdateItemList(string[] poolNames)
        {
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
                    SetItemTexture(i + 1, tex);
                }
                else
                {
                    if(GUIController.Instance.images.TryGetValue(p,out var customTex)) //apply custom imgage
                    {
                        SetItemTexture(i + 1, customTex);
                    }
                    else
                    {
                        var objprefab = ObjectLoader.InstantiableObjects[p];
                        Sprite s = objprefab.GetComponent<SpriteRenderer>()?.sprite;
                        var btn = panel.GetPanel("Border").GetPanel($"ItemBorder{i + 1}").GetButton($"Item_{i + 1}");
                        if (s != null) //try to apply gameobject's sprite
                            btn.UpdateSprite(s, new Rect());
                        else // apply empty image
                        {
                            var img_defaultIcon = GUIController.Instance.images["defaultIcon"];
                            SetItemTexture(i + 1, img_defaultIcon);
                        }
                    }
                }
                SetItemTxt(i + 1, p); // update text
                SetBtnActive(i + 1 ,true);
            }
            for(int i=poolNames.Length+1;i<= ItemManager.GroupMax;i++) //hide unused button
            {
                SetBtnActive(i, false);
            }
        }

        private static void LogDebug(object o)
        {
            Logger.LogDebug($"[PickPanel]{o}");
        }
    }
}
