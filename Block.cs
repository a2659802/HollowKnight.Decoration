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
using DecorationMaster.Util;
namespace DecorationMaster
{
    public class Block
    {
        public enum BlockOp
        {
            COPY,
            MOVE,
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
            private bool hold = false;
            public Action Cancel;
            public Action Confirm;
            private void Awake()
            {
                gameObject.name = "BlockMover";
            }
            private IEnumerator Start()
            {
                yield return null;
                yield return null;

                foreach (Transform t in transform)
                    t.GetComponent<CustomDecoration>().enabled = false;
            }
            private void Update()
            {
                if (DecorationMaster.GM.isPaused || DecorationMaster.GM.IsInSceneTransition)
                    return;
                if(!hold)
                    transform.position = DecorationMaster.GetMousePos();
                if(Input.GetMouseButtonUp((int)MouseButton.Right))
                {
                    hold = true;
                    if (Cancel == null)
                        Destroy(gameObject);
                    else
                        Cancel.Invoke();
                }
                else if (Input.GetMouseButtonUp((int)MouseButton.Left))
                {
                    hold = true;
                    if (Confirm == null)
                        SetupAll();
                    else
                        Confirm.Invoke();
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
            private void OnDestroy()
            {
                foreach(Transform t in transform)
                {
                    t.GetComponent<CustomDecoration>()?.Remove();
                }
            }
        }
        public void Select(BlockOp op)
        {
            if (Inspector.IsToggle())
                Inspector.Hide();
            ItemManager.Instance.RemoveCurrent();
            start = Vector2.one * -1;
            UnityEngine.Object.Destroy(tmp);
            this.op = op;
            tmp = new GameObject();
            tmp.AddComponent<LineShower>();
            Texture2D cursorTex;
            switch (op)
            {
                case BlockOp.COPY:
                    cursorTex = GUIController.Instance.images["arrow_copy"];
                    break;
                case BlockOp.MOVE:
                    cursorTex = GUIController.Instance.images["arrow_mov"];
                    break;
                case BlockOp.DELETE:
                    cursorTex = GUIController.Instance.images["arrow_del"];
                    break;
                default:
                    cursorTex = GUIController.Instance.images["arrow2"];
                    break;
            }
            MyCursor.cursorTexture = cursorTex;
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

            if(InRangeObjs.Count<1) //select nothing
            {
                return;
            }

            if (op == BlockOp.COPY)
                CopyInRange();
            else if (op == BlockOp.MOVE)
                MoveInRange();
            else if (op == BlockOp.DELETE)
            {
                DeleteInRange();
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
        private void MoveInRange()
        {
            tmp = new GameObject();
            tmp.transform.position = DecorationMaster.GetMousePos();
            tmp.AddComponent<SpriteRenderer>().sprite = Sprite.Create(new Texture2D(20, 20), new Rect(0, 0, 20, 20), Vector2.one * 0.5f);

            foreach (var go in InRangeObjs)
            {
                //var clone = go.GetComponent<CustomDecoration>().CopySelf();
                //clone.SetActive(true);
                //clone.transform.SetParent(tmp.transform);
                go.transform.SetParent(tmp.transform);
            }
            var oriPos = tmp.transform.position;
            var mover = tmp.AddComponent<BlockMover>();
            mover.Cancel = () => {
                Logger.LogDebug("[Block] Move Cancel");
                tmp.transform.position = oriPos;
                ReleaseAll(tmp.transform);
                UnityEngine.Object.Destroy(tmp);
                
            };
            mover.Confirm = () =>
            {
                var list = new List<GameObject>();
                foreach (Transform t in tmp.transform)
                {
                    list.Add(t.gameObject);
                }
                foreach (var go in list)
                {
                    go.transform.SetParent(null);

                    var cd = go.GetComponent<CustomDecoration>();
                    cd.Setup(Operation.SetPos, (Vector2)go.transform.position);
                    //cd.Setup(Operation.ADD,null);
                    cd.enabled = true;
                }
                list.Clear();
                //ItemManager.Instance.AddBlock(list);
                UnityEngine.Object.Destroy(tmp);
            };
        }
        private void DeleteInRange()
        {
            tmp = new GameObject();
            tmp.transform.position = DecorationMaster.GetMousePos();
            tmp.AddComponent<SpriteRenderer>().sprite = Sprite.Create(new Texture2D(20, 20), new Rect(0, 0, 20, 20), Vector2.one * 0.5f);

            foreach (var go in InRangeObjs)
            {
                //var clone = go.GetComponent<CustomDecoration>().CopySelf();
                //clone.SetActive(true);
                //clone.transform.SetParent(tmp.transform);
                go.transform.SetParent(tmp.transform);
            }

            var oriPos = tmp.transform.position;
            tmp.AddComponent<BlockMover>().Cancel = () => {
                tmp.transform.position = oriPos;
                ReleaseAll(tmp.transform);
                UnityEngine.Object.Destroy(tmp);
            };
            tmp.GetComponent<BlockMover>().Confirm = () =>
            {
                UnityEngine.Object.Destroy(tmp);
            };
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

        public static void ReleaseAll(Transform parent)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in parent)
            {
                children.Add(child);
            }
            foreach (var child in children)
            {
                child.SetParent(null);
                child.GetComponent<CustomDecoration>().enabled = true;
                Logger.LogDebug("Reset Pos");
            }
            children.Clear();
        }
        //[DllImport("user32.dll")]
        //public static extern int SetCursorPos(int x, int y);
    }
}
