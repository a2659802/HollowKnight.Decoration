using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
namespace DecorationMaster.UI
{
    public class GUIController : MonoBehaviour
    {
        public Font trajanBold;
        public Font trajanNormal;
        public Font arial;
        public Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();

        public GameObject canvas;
        private static GUIController _instance;
        private MethodBase _setCursorVisible;
        public MethodBase SetCursorVisible {
            get {
                if (_setCursorVisible != null)
                    return _setCursorVisible;
                _setCursorVisible = typeof(InputHandler).GetMethod("SetCursorVisible", BindingFlags.NonPublic | BindingFlags.Instance);
                return _setCursorVisible;
            }
        }
        public static GUIController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GUIController>();
                    if (_instance == null)
                    {
                        Logger.LogWarn(" Couldn't find GUIController");

                        GameObject GUIObj = new GameObject();
                        _instance = GUIObj.AddComponent<GUIController>();
                        DontDestroyOnLoad(GUIObj);
                    }
                }
                return _instance;
            }
            set
            {
            }
        }
        private void Awake()
        {
            _instance = this;
        }
        public void BuildMenus()
        {
            LoadResources();

            canvas = new GameObject();
            canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvas.AddComponent<GraphicRaycaster>();

            DontDestroyOnLoad(canvas);

            PickPanel.BuildMenu(canvas);

            ItemManager.Instance.SwitchGroup(0);
        }

        private void LoadResources()
        {
            foreach (Font f in Resources.FindObjectsOfTypeAll<Font>())
            {
                if (f != null && f.name == "TrajanPro-Bold")
                {
                    trajanBold = f;
                }

                if (f != null && f.name == "TrajanPro-Regular")
                {
                    trajanNormal = f;
                }

                //Just in case for some reason the computer doesn't have arial
                if (f != null && f.name == "Perpetua")
                {
                    arial = f;
                }

                foreach (string font in Font.GetOSInstalledFontNames())
                {
                    if (font.ToLower().Contains("arial"))
                    {
                        arial = Font.CreateDynamicFontFromOSFont(font, 13);
                        break;
                    }
                }
            }

            if (trajanBold == null || trajanNormal == null || arial == null) Logger.LogError("Could not find game fonts");

            ObjectLoader.ImageLoader.Load();
            foreach(var raw in ObjectLoader.ImageLoader.raw_images)
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(raw.Value);
                images.Add(raw.Key, tex);
            }
        }

        public void Update()
        {
            PickPanel.Update();


        }
    }
}
