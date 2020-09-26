using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DecorationMaster.UI
{
    class InputElements
    {
    }
    /*public class FloatInput : Slider
    {
        public FloatInput()
        {
            minValue = 0;
            maxValue = 5;
        }
        public FloatInput(float min,float max)
        {
            minValue = min;
            maxValue = max;
        }
        public void Bind(UnityAction<float> e)
        {
            onValueChanged.AddListener(e);
        }
    }*/
    public class CanvasSlider
    {
        private GameObject sliderObj;
        public Slider slider;
        public bool active;
        public CanvasSlider(GameObject parent, Vector2 pos, Vector2 size, Rect bgSubSection)
        {
            if (size.x == 0 || size.y == 0)
            {
                size = new Vector2(bgSubSection.width, bgSubSection.height);
            }

            sliderObj = new GameObject();
            //sliderObj.AddComponent<CanvasRenderer>();
            RectTransform sliderTransform = sliderObj.AddComponent<RectTransform>();
            slider = sliderObj.AddComponent<Slider>();
            
            sliderObj.transform.SetParent(parent.transform, false);

            sliderTransform.SetScaleX(size.x / bgSubSection.width);
            sliderTransform.SetScaleY(size.y / bgSubSection.height);

            Vector2 position = new Vector2((pos.x + ((size.x / bgSubSection.width) * bgSubSection.width) / 2f) / 1920f, (1080f - (pos.y + ((size.y / bgSubSection.height) * bgSubSection.height) / 2f)) / 1080f);
            sliderTransform.anchorMin = position;
            sliderTransform.anchorMax = position;

            Object.DontDestroyOnLoad(sliderObj);

            active = true;

            slider.minValue = 0;
            slider.maxValue = 3;
            
            
        }

        public void AddSlideEvent(UnityAction<float> e)
        {
            slider.onValueChanged.AddListener(e);
        }
        public void SetConstrain(float min,float max)
        {
            slider.minValue = min;
            slider.maxValue = max;
        }
    }
}
