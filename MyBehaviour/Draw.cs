using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.Attr;
namespace DecorationMaster.MyBehaviour
{

    public class Draw
    {
        [Serializable]
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
        }
    
        [Decoration("terrain_point")]
        [Description("用来描绘边界的点，会自动和上一个点连起来形成一个区域分界线")]
        public class TerrianPoint:CustomDecoration
        {
            [Serializable]
            public class PointItem : Item
            {
                public int seq = 0;

                [FloatConstraint(0, 1)]
                public float R { get; set; } = 1f;
                [FloatConstraint(0, 1)]
                public float G { get; set; } = 1f;
                [FloatConstraint(0, 1)]
                public float B { get; set; } = 1f;
            }

            public const float linewidth = .5f;
            private LineRenderer lr;
            
            private void Awake()
            {

                var points = FindObjectsOfType<TerrianPoint>();//.Where(x => (((PointItem)(x.item)).seq == selfSeq - 1)).FirstOrDefault();
                ((PointItem)item).seq = points.Length;
                points = points.OrderByDescending(x => get_seq(x)).ToArray();
                if (points.Length < 2)
                    return;

                TerrianPoint lastPoint = points[1];
                if (get_seq(lastPoint) != (get_seq(this) - 1))
                    return;

                var line1 = gameObject.AddComponent<LineRenderer>();
                line1.material = new Material(Shader.Find("Particles/Additive"));
                line1.positionCount = 2;
                line1.startWidth = linewidth;
                line1.endWidth = linewidth;
                line1.startColor = Color.yellow;
                line1.endColor = Color.yellow;
                line1.SetPosition(0, lastPoint.transform.position);
                lr = line1;
            }
            private static int get_seq(TerrianPoint tp)
            {
                return ((PointItem)tp.item).seq;
            }
            public override void HandlePos(Vector2 val)
            {
                base.HandlePos(val);
                if (lr == null)
                    return;
                lr.SetPosition(1, transform.position);
            }
        }
    }
}
