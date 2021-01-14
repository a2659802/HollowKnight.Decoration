using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.MyBehaviour;
using HutongGames.PlayMaker;
using System.Runtime.InteropServices;
using System.Collections;
using DecorationMaster.UI;

namespace DecorationMaster
{
    class Block
    {
        enum BlockOp
        {
            COPY,
            DELETE,
        }
        public List<GameObject> InRangeObjs = new List<GameObject>();
        public Vector2 start = Vector2.one * -1;
        public Vector2 end;
        private GameObject tmp;
        private static Block _instance;
        private BlockOp op = BlockOp.COPY;
        public static Block Instance { get {
                if (_instance == null)
                    _instance = new Block();
                return _instance;
            } }
        private Block()
        {
        }
        private class LineShower : MonoBehaviour
        {
            public LineRenderer line;
            private void Awake()
            {
                line = gameObject.AddComponent<LineRenderer>();
                line.material = new Material(Shader.Find("Particles/Additive"));
                line.positionCount = 5;
                line.startColor = Color.green;
                line.endColor = Color.green;
                line.startWidth = 0.08f;
                line.endWidth = 0.08f;
                line.sortingOrder = 2000;
            }
            private void Update()
            {

                if(Input.GetMouseButtonUp((int)MouseButton.Left))
                {
                    Instance.EndSelect(DecorationMaster.GetMousePos());
                }
                else if(Input.GetMouseButtonUp((int)MouseButton.Right))
                {
                    MyCursor.cursorTexture = GUIController.Instance.images["arrow"];
                    Destroy(gameObject);
                }
                
                if(Input.GetMouseButtonDown((int)MouseButton.Left))
                {
                    Instance.StartSelect(DecorationMaster.GetMousePos());
                }

                if (Instance.start.x < 0)
                    return;
                var mouse = DecorationMaster.GetMousePos();
                line.SetPositions(new Vector3[] {
                    Instance.start,
                    new Vector2(mouse.x,Instance.start.y),
                    mouse,
                    new Vector2(Instance.start.x,mouse.y),
                    Instance.start,
                });
                //Logger.LogDebug(GetScreenPos(mouse));
            }
        }
        private class BlockMover : MonoBehaviour
        {
            private IEnumerator Start()
            {
                yield return null;
                yield return null;

                foreach (Transform t in transform)
                    t.GetComponent<CustomDecoration>().enabled = false;
            }
            private void Update()
            {
                transform.position = DecorationMaster.GetMousePos();
                if(Input.GetMouseButtonUp((int)MouseButton.Right))
                {
                    Destroy(gameObject);
                }
                else if (Input.GetMouseButtonUp((int)MouseButton.Left))
                {
                    SetupAll();
                }
            }
            private void SetupAll()
            {
                var list = new List<GameObject>();
                foreach(Transform t in transform)
                {
                    list.Add(t.gameObject);
                }
                foreach(var go in list)
                {
                    go.transform.SetParent(null);

                    var cd = go.GetComponent<CustomDecoration>();
                    cd.Setup(Operation.SetPos, (Vector2)go.transform.position);
                    //cd.Setup(Operation.ADD,null);
                    cd.enabled = true;
                }
                ItemManager.Instance.AddBlock(list);
                Destroy(gameObject);
            }
        }
        public void Select()
        {
            if (Inspector.IsToggle())
                Inspector.Hide();
            ItemManager.Instance.RemoveCurrent();
            start = Vector2.one * -1;
            UnityEngine.Object.Destroy(tmp);
            tmp = new GameObject();
            tmp.AddComponent<LineShower>();
            MyCursor.cursorTexture = GUIController.Instance.images["arrow2"];
        }
        public void StartSelect(Vector2 Pos)
        {
            start = Pos;
        }
        public void EndSelect(Vector2 Pos)
        {

            UnityEngine.Object.Destroy(tmp);
            start = GetScreenPos(start);
            end = GetScreenPos(Pos);
            if (end.x <= start.x || end.y >= start.y)
            {
                return;
            }

            SelectInRange();
            if(op == BlockOp.COPY)
                CopyInRange();
            else if(op == BlockOp.DELETE)
            {
                //TODO
            }
            MyCursor.cursorTexture = GUIController.Instance.images["arrow"];
        }
        private void SelectInRange()
        {
            var gos = UnityEngine.Object.FindObjectsOfType<CustomDecoration>().Select(x => x.gameObject);
            InRangeObjs.Clear();
            foreach (var go in gos)
            {
                if (is_in_range(GetScreenPos(go)))
                {
                    InRangeObjs.Add(go);
                    //go.GetComponent<CustomDecoration>().enabled = false;
                    //Logger.Log(go.name);
                }
            }
        }
        private void CopyInRange()
        {
            //var x = (int)(end.x + start.x) / 2;
            //var y = (int)(start.y + end.y) / 2;
            //SetCursorPos(x, 1080 - y);

            //Logger.LogDebug($"Start:{start}.End:{end},Mid:{x},{y}");
            tmp = new GameObject();
            tmp.transform.position = DecorationMaster.GetMousePos();
            tmp.AddComponent<SpriteRenderer>().sprite = Sprite.Create(new Texture2D(20, 20), new Rect(0, 0, 20, 20), Vector2.one * 0.5f);

            foreach (var go in InRangeObjs)
            {
                var clone = go.GetComponent<CustomDecoration>().CopySelf();
                clone.SetActive(true);
                clone.transform.SetParent(tmp.transform);
            }

            tmp.AddComponent<BlockMover>();
        }
        private bool is_in_range(Vector2 dest)
        {
            return (dest.x <= end.x && dest.x >= start.x && dest.y <= start.y && dest.y >= end.y);
        }
        public static Vector2 GetScreenPos(GameObject go)
        {
            return GetScreenPos(go.transform.position);
        }
        public static Vector2 GetScreenPos(Vector3 pos)
        {
            return Camera.main.WorldToScreenPoint(pos);
        }

        //[DllImport("user32.dll")]
        //public static extern int SetCursorPos(int x, int y);
    }
}
