using DecorationMaster.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecorationMaster.Util;
using UnityEngine;

namespace DecorationMaster.MyBehaviour
{
    class Particle
    {
        [Decoration("HK_crystal_dropping")]
        public class CrystalDropping : CustomDecoration
        {
            private void Awake()
            {
                UnVisableBehaviour.AttackReact.Create(gameObject);
                var ps = gameObject.GetComponent<ParticleSystem>();
                var collision = ps.collision;
                var main = ps.main;
                var emit = ps.emission;
                //collision.quality = ParticleSystemCollisionQuality.High;
                collision.sendCollisionMessages = true;
                collision.collidesWith = LayerMask.GetMask(new string[]{"Player","Terrain" });
                main.startColor = new Color(1,0.4f,0.5f);
                main.startLifetime = 4;
                main.startSize = new ParticleSystem.MinMaxCurve(1.2f, 1.4f);
                emit.rateOverTime = 1;
            }
            private void OnParticleCollision(GameObject other)
            {
                if(other.layer == 9)
                {
                    HeroController.instance.TakeDamage(null, 0, 1, 1);
                }
            }

            [Handle(Operation.SetSpeed)]
            public void HandleXspeed(int spd)
            {
                var ps = gameObject.GetComponent<ParticleSystem>();
                var v = ps.velocityOverLifetime;
                v.x = new ParticleSystem.MinMaxCurve(spd, spd);
                v.enabled = true;
            }
            [Handle(Operation.SetRate)]
            public void HandleRate(int rate)
            {
                var ps = gameObject.GetComponent<ParticleSystem>();
                var e = ps.emission;
                e.rateOverTime = rate;
            }

            [Serializable]
            public class ParticleItem : Item
            {
                [Handle(Operation.SetSpeed)]
                [IntConstraint(-8, 8)]
                public int xspeed { get; set; } = -5;

                [Handle(Operation.SetRate)]
                [IntConstraint(1, 5)]
                public int rate { get; set; } = 1;
            }
        }
    }
}
