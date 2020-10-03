using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
namespace DecorationMaster.Util
{
    public class NameDisp
    {
        private static GameObject _name_prefab;
        public static GameObject Prefab
        {
            get
            {
                if (_name_prefab != null)
                    return _name_prefab;
                _name_prefab = new GameObject("name_prefab");
                _name_prefab.layer = 0;
                var text = _name_prefab.AddComponent<TextMeshPro>();
                text.text = "NULL";
                text.fontSize = 28;
                text.outlineColor = Color.black;
                text.outlineWidth = 0.1f;
                _name_prefab.AddComponent<KeepWorldScalePositive>();
                _name_prefab.transform.SetScaleX(0.2f);
                _name_prefab.transform.SetScaleY(0.2f);
                _name_prefab.SetActive(false);
                Object.DontDestroyOnLoad(_name_prefab);

                return _name_prefab;
            }
        }
        public static GameObject Create(GameObject parent,string name)
        {
            return Create(parent.transform, name);
        }
        public static GameObject Create(Transform t, string name = null, int minLayer = 0, float size = 1)
        {
            if (t == null)
                return null;
            GameObject go = t.gameObject;
            if (go.layer < minLayer)
                return null;

            var nameobj = Object.Instantiate(Prefab);
            var text = nameobj.GetComponent<TextMeshPro>();
            text.text = string.IsNullOrEmpty(name) ? (!string.IsNullOrEmpty(go.name) ? go.name : "NULL") : name;
            nameobj.transform.SetParent(t);
            nameobj.transform.localPosition = new Vector3(0, 1.5f);
            nameobj.SetActive(true);
            nameobj.name = $"Disp[{text.text}]";
            nameobj.transform.localScale *= size;
            return nameobj;
        }
        public static GameObject Create(Transform t, Vector3 localpos, string name = null, int minLayer = 0, float size = 1)
        {
            if (t == null)
                return null;
            GameObject go = t.gameObject;
            if (go.layer < minLayer)
                return null;

            var nameobj = Object.Instantiate(Prefab);
            var text = nameobj.GetComponent<TextMeshPro>();
            text.text = string.IsNullOrEmpty(name) ? (!string.IsNullOrEmpty(go.name) ? go.name : "NULL") : name;
            nameobj.transform.SetParent(t);
            nameobj.transform.localPosition = localpos;
            nameobj.SetActive(true);
            nameobj.name = $"Disp[{text.text}]";
            nameobj.transform.localScale *= size;
            return nameobj;
        }
    }
}
