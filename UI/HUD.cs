using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DecorationMaster.UI
{
    public static class HUD
    {
        private static CanvasPanel panel;
        private static float offset = 0;
        public static void AddBindIcon(GameObject parent)
        {
            if (panel.GetImage($"{parent.name}_{parent.GetHashCode()}") != null)
                return;
            try
            {
                var tex = GetBindIconTex(parent);
                var icon = panel.AddImage($"{parent.name}_{parent.GetHashCode()}", tex, new Vector2(offset, 0), new Vector2(tex.width / 3, tex.height / 3), new Rect(0, 0, tex.width, tex.height));
                icon.SetColor(Color.yellow);
                offset += tex.width / 3;
                parent.AddComponent<Attach>();

                Logger.LogDebug("Added Hud Icon" + $",{parent.name}_{parent.GetHashCode()}");
            }
            catch
            { }
            
        }
        public static void BuildMenu(GameObject canvas)
        {
            panel = new CanvasPanel(canvas, new Texture2D(1, 1), new Vector2(150, 230), Vector2.zero, new Rect(0, 0, 1, 1));
        }
        private static Sprite GetBindIcon(GameObject go)
        {
            return go.GetComponent<SpriteRenderer>()?.sprite;
        }
        private static Texture2D GetBindIconTex(GameObject go)
        {
            return GetBindIcon(go)?.texture;
        }
        private class Attach : MonoBehaviour
        {
            private void OnDisable()
            {
                var parent = gameObject;
                panel.GetImage($"{parent.name}_{parent.GetHashCode()}")?.Destroy();
                offset -= GetBindIconTex(gameObject).width / 3;
                Logger.LogDebug("remove icon");
            }
        }
    }
}
