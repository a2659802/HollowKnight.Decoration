using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.MyBehaviour;
using DecorationMaster.Attr;
using System.Reflection;

namespace DecorationMaster.UI
{
    public static class DescriptionPanel
    {
        private static CanvasPanel panel;
        private static CanvasPanel desc_panel;
        private static CanvasText desc;

        public static void BuildMenu(GameObject canvas)
        {
            panel = new CanvasPanel(canvas, new Texture2D(1, 1), new Vector2(0, 0), Vector2.zero, new Rect(0, 0, 1, 1));
            var tex = GUIController.Instance.images["hidebtnbg"];
            panel.AddButton("hide", tex,new Vector2((1920-790)/2,10), Vector2.zero, HideClicked, new Rect(0,0,tex.width,tex.height), GUIController.Instance.arial, "<", 25);

            desc_panel = panel.AddPanel("DescArea", new Texture2D(1, 1), new Vector2(0, 300), new Vector2(1, 1), new Rect(0, 0, 1, 1));

            desc = desc_panel.AddText("Desc", "This is a Description", Vector2.zero, Vector2.zero, GUIController.Instance.arial, 26);
            ItemManager.Instance.OnChanged += ChangeDesc;
            Logger.LogDebug("DescPanel Built");

        }
        private static void HideClicked(string btn)
        {
            //Logger.LogDebug("Hide Desc");
            desc_panel.SetActive(!desc_panel.active, true);

            panel.GetButton("hide").UpdateText(desc_panel.active ? "<" : ">");
            DecorationMaster.instance.Settings.showDesc = desc_panel.active;

        }
        public static void SetActive(bool b)
        {
            bool show = DecorationMaster.instance.Settings.showDesc;
            if (b)
                panel.SetActive(true, show);
            else
                panel.SetActive(false, true);
        }
        public static void UpdateDesc(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            desc.UpdateText(text);
        }
        public static void ChangeDesc(CustomDecoration d)
        {
            string desc = null;
            if(Language.Language.CurrentLanguage() == Language.LanguageCode.ZH)
            {
                var cn = d.GetType().GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().Where(x => x.IsChinese()).FirstOrDefault();
                if (cn == null)
                    desc = "该物品没有相关说明";
                else
                    desc = cn.Text;

                desc = $"{desc}\n\n属性说明：\n";

                var desc_prop = d.item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(x => x.GetCustomAttributes(typeof(DescriptionAttribute), true).OfType<DescriptionAttribute>()
               .Where(y => y.IsChinese()).Any());
                foreach(var p in desc_prop)
                {
                    var prop_text = p.GetCustomAttributes(typeof(DescriptionAttribute), true).OfType<DescriptionAttribute>().FirstOrDefault().Text;
                    var prop_name = p.Name;
                    desc += $"{prop_name}:{prop_text}\n";
                }
                
            }
            else
            {
                var en = d.GetType().GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().Where(x => !x.IsChinese()).FirstOrDefault();
                if (en == null)
                    desc = "there is no description for this item";
                else
                    desc = en.Text;
            }
            UpdateDesc(desc);
            //Logger.LogDebug($"Desc:{desc}");
        }
    }
}
