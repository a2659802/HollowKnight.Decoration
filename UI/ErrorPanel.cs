using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace DecorationMaster.UI
{
    public class ErrorPanel
    {
        private static CanvasPanel panel;
        public ErrorPanel(string msg)
        {
            GUIController.Instance.LoadResources();
            var canvas = new GameObject();
            canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvas.AddComponent<GraphicRaycaster>();
            panel = new CanvasPanel(canvas, new Texture2D(1, 1), new Vector2(0, 0), Vector2.zero, new Rect(0, 0, 1, 1));
            panel.AddText("Desc", $"DecorationMaster Load Json data Error:\n{msg}", new Vector2(300,0), Vector2.zero, GUIController.Instance.arial, 28);
            panel.SetActive(true, true);

            Logger.LogError(msg);
        }       
    }
}
