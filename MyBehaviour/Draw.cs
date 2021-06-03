using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.Attr;
using System.Collections;

namespace DecorationMaster.MyBehaviour
{

    public class Draw
    {
        /*[Serializable]
        public struct PColor
        {
            public float r;
            public float g;
            public float b;
            public float a;
            public PColor(float r,float g,float b,float a)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }
            public PColor(float r, float g, float b) : this(r, g, b, 1) { }

            public static implicit operator Color(PColor c)
            {
                return new Color(c.r, c.g, c.b, c.a);
            }
            public static implicit operator PColor(Color c)
            {
                return new PColor(c.r, c.g, c.b, c.a);
            }
        }*/

        [Serializable]
        public class PointItem : ColorItem
        {
            public int seq = 0;
        }
        [AdvanceDecoration]
        [Decoration("IMG_TerrainPoint")]
        [Description("用来描绘边界的点,有碰撞，会自动和上一个点连起来形成一个区域分界线")]
        [Description("in-game drawing point, you can use them to draw edge collider", "en-us")]
        public class TerrianPoint:CustomDecoration
        {
            public const float linewidth = .07f;
            private LineRenderer lr;
            private EdgeCollider2D col;

            private void Awake()
            {
                gameObject.AddComponent<NonBouncer>();
                if (get_seq(this) == 0)
                {
                    var points = FindObjectsOfType<TerrianPoint>().Where(x => x != this).OrderByDescending(x => get_seq(x)).ToArray();
                    if (points != null && points.Length > 0)
                    {
                        ((PointItem)item).seq = get_seq(points[0]) + 1;
                    }
                    else
                    {
                        ((PointItem)item).seq = 1;
                    }
                }
            }
            private void Start()
            {
                TerrianPoint lastPoint = FindObjectsOfType<TerrianPoint>().Where(x => (get_seq(this) - 1) == get_seq(x)).FirstOrDefault();
                if (lastPoint == null)
                    return;

                var line1 = gameObject.AddComponent<LineRenderer>();
                line1.material = new Material(Shader.Find("Particles/Additive"));
                line1.positionCount = 2;
                line1.startWidth = linewidth;
                line1.endWidth = linewidth;
                line1.startColor = get_color(lastPoint);
                line1.endColor = get_color(this);
                line1.SetPosition(0, lastPoint.transform.position);
                lr = line1;

                var child = new GameObject("collider");
                child.layer = (int)GlobalEnums.PhysLayers.TERRAIN;
                child.transform.SetParent(transform);
                child.transform.localPosition = Vector3.zero;
                col = child.AddComponent<EdgeCollider2D>();

                HandlePos(transform.position);
            }
            private static int get_seq(TerrianPoint tp)
            {
                return ((PointItem)tp.item).seq;
            }
            private static Color get_color(TerrianPoint tp)
            {
                var i = ((PointItem)tp.item);
                return new Color(i.R, i.G, i.B, i.A);
            }
            public override void HandlePos(Vector2 val)
            {
                base.HandlePos(val);
                if (lr == null)
                    return;
                lr.SetPosition(1, transform.position);
                col.points = new Vector2[] { Vector2.zero, lr.GetPosition(0)-transform.position };
            }
            public override GameObject CopySelf(object self = null)
            {
                var item_clone = item.Clone() as Item;
                ((PointItem)item_clone).seq = 0;
                var clone = ObjectLoader.CloneDecoration(item_clone);
                return clone;
            }
            [Handle(Operation.SetColorR)]
            [Handle(Operation.SetColorG)]
            [Handle(Operation.SetColorB)]
            [Handle(Operation.SetColorA)]
            public void HandleColors(float val)
            {
                if (lr == null)
                    return;
                lr.endColor = get_color(this);
            }
        }

        [AdvanceDecoration]
        [Decoration("IMG_PaintPoint")]
        [Description("纯粹用来绘制的点，没有任何碰撞效果\n建议用来给给关卡绘制简单的提示")]
        [Description("in-game drawing point, but without collision", "en-us")]
        public class PaintPoint : CustomDecoration
        {
            public const float linewidth = .05f;
            private LineRenderer lr;

            private void Awake()
            {
                gameObject.AddComponent<NonBouncer>();

                if(get_seq(this)==0)
                {
                    var points = FindObjectsOfType<PaintPoint>().Where(x => x != this).OrderByDescending(x => get_seq(x)).ToArray();
                    if (points != null && points.Length > 0)
                    {
                        ((PointItem)item).seq = get_seq(points[0]) + 1;
                    }
                    else
                    {
                        ((PointItem)item).seq = 1;
                    }
                }
            }
            private void Start()
            {

                PaintPoint lastPoint = FindObjectsOfType<PaintPoint>().Where(x => (get_seq(this)-1) == get_seq(x)).FirstOrDefault();
                if (lastPoint == null)
                    return;

                var line1 = gameObject.AddComponent<LineRenderer>();
                line1.material = new Material(Shader.Find("Particles/Additive"));
                line1.positionCount = 2;
                line1.startWidth = linewidth;
                line1.endWidth = linewidth;
                line1.startColor = get_color(lastPoint);
                line1.endColor = get_color(this);
                line1.SetPosition(0, lastPoint.transform.position);
                lr = line1;

                HandlePos(transform.position);

            }
            private static int get_seq(PaintPoint tp)
            {
                return ((PointItem)tp.item).seq;
            }
            private static Color get_color(PaintPoint tp)
            {
                var i = ((PointItem)tp.item);
                return new Color(i.R, i.G, i.B, i.A);
            }
            public override void HandlePos(Vector2 val)
            {
                base.HandlePos(val);

                if (lr == null)
                    return;
                lr.SetPosition(1, transform.position);

            }

            public override GameObject CopySelf(object self = null)
            {
                var item_clone = item.Clone() as Item;
                ((PointItem)item_clone).seq = 0;
                var clone = ObjectLoader.CloneDecoration(item_clone);
                return clone;
            }

            [Handle(Operation.SetColorR)]
            [Handle(Operation.SetColorG)]
            [Handle(Operation.SetColorB)]
            [Handle(Operation.SetColorA)]
            public void HandleColors(float val)
            {
                if (lr == null)
                    return;
                lr.endColor = get_color(this);
            }


        }


    }
}
