using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecorationMaster.Util;
using UnityEngine;
using DecorationMaster.Attr;
using System.Collections;
using Modding;
using ModCommon;
namespace DecorationMaster.MyBehaviour
{
    public class OneShotBehaviour
    {
        [Decoration("IMG_recoverDash")]
        [Description("给予一次冲刺能力")]
        [Description("give player dash ability once \n yay just like Celeste", "en-us")]
        public class RecoverDash : CustomDecoration
        {
            public static AudioClip clip { get {
                    if (_c)
                        return _c;
                    _c = WavHelper.GetAudioClip("eat_crystal");
                    return _c; } }
            private static AudioClip _c;
            private AudioSource au;
            private bool used;
            private HeroTrigger ht;
            private BoxCollider2D col;
            private void Awake()
            {
                
                au = gameObject.AddComponent<AudioSource>();
                //var sr = gameObject.AddComponent<SpriteRenderer>();
                //var col = gameObject.AddComponent<BoxCollider2D>();
                ht = gameObject.AddComponent<HeroTrigger>();
                ht.HeroEnter = RecoveOneshot;
                transform.localScale *= 2;
            }
            private void Start()
            {
                col = gameObject.GetComponent<BoxCollider2D>();
            }
            private void RecoveOneshot() 
            {
                if (used)
                    return;
                used = true;
                On.HeroController.CanDash += True;
                On.HeroController.HeroDash += RemoveHook;
                ModHooks.Instance.TakeDamageHook += remove;
                au.PlayOneShot(clip);
                StartCoroutine(Consume());
                
                IEnumerator Consume()
                {
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    ht.enabled = false;
                    col.enabled = false;
                    yield return new WaitWhile(() => au.isPlaying);

                    yield return new WaitForSeconds(3);
                    gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    ht.enabled = true;
                    col.enabled = true;
                    used = false;
                }
                
            }

            private int remove(ref int hazardType, int damage)
            {
                if(hazardType>(int)GlobalEnums.HazardType.SPIKES)
                {
                    //Logger.LogDebug("hazard dmg");
                    On.HeroController.CanDash -= True;
                }
                return damage;
            }

            private bool True(On.HeroController.orig_CanDash orig, HeroController self)
            {
                return true;
            }

            private void RemoveHook(On.HeroController.orig_HeroDash orig, HeroController self)
            {
                orig(self);
                On.HeroController.CanDash -= True;
            }
        }
        [Decoration("IMG_recoverJump")]
        [Description("给予一次二段跳能力")]
        [Description("give player double ability once", "en-us")]
        public class RecoverWingJump : CustomDecoration
        {
            public static AudioClip clip => RecoverDash.clip;
            private AudioSource au;
            private HeroTrigger ht;
            private BoxCollider2D col;
            private bool used;
            private void Awake()
            {
                au = gameObject.AddComponent<AudioSource>();
                //var sr = gameObject.AddComponent<SpriteRenderer>();
               // var col = gameObject.AddComponent<BoxCollider2D>();
                ht = gameObject.AddComponent<HeroTrigger>();
                ht.HeroEnter = RecoveOneshot;
                transform.localScale *= 2;
            }
            private void Start()
            {
                col = gameObject.GetComponent<BoxCollider2D>();
            }
            private void RecoveOneshot()
            {
                if (used)
                    return;
                used = true;
                On.HeroController.CanDoubleJump += True;
                On.HeroController.DoDoubleJump += RemoveHook;
                ModHooks.Instance.TakeDamageHook += remove;
                au.PlayOneShot(clip);
                StartCoroutine(Consume());

                IEnumerator Consume()
                {
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    ht.enabled = false;
                    col.enabled = false;
                    yield return new WaitWhile(() => au.isPlaying);

                    yield return new WaitForSeconds(3);
                    gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    ht.enabled = true;
                    col.enabled = true;
                    used = false;

                }
            }
            private int remove(ref int hazardType, int damage)
            {
                if (hazardType > (int)GlobalEnums.HazardType.SPIKES)
                {
                    //Logger.LogDebug("hazard dmg");
                    On.HeroController.CanDoubleJump -= True;
                }
                return damage;
            }
            private bool True(On.HeroController.orig_CanDoubleJump orig, HeroController self)
            {
                return true;
            }

            private void RemoveHook(On.HeroController.orig_DoDoubleJump orig, HeroController self)
            {
                orig(self);
                On.HeroController.CanDoubleJump -= True;
            }
        }
    
        public class OneshotFireball : CustomDecoration
        {
            public static AudioClip clip => RecoverDash.clip;
            private AudioSource au;
            private HeroTrigger ht;
            private BoxCollider2D col;
            private bool used;
            private void Awake()
            {
                au = gameObject.AddComponent<AudioSource>();
                //var sr = gameObject.AddComponent<SpriteRenderer>();
                // var col = gameObject.AddComponent<BoxCollider2D>();
                ht = gameObject.AddComponent<HeroTrigger>();
                ht.HeroEnter = RecoveOneshot;
                
                //transform.localScale *= 2;
            }
            private void Start()
            {
                col = gameObject.GetComponent<BoxCollider2D>();
            }
            private void RecoveOneshot()
            {
                if (used)
                    return;
                used = true;
                ModHooks.Instance.GetPlayerIntHook += Fireball2;
                au.PlayOneShot(clip);
                StartCoroutine(Consume());

                IEnumerator Consume()
                {
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    ht.enabled = false;
                    col.enabled = false;
                    yield return new WaitWhile(() => au.isPlaying);

                    yield return new WaitForSeconds(3);
                    gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    ht.enabled = true;
                    col.enabled = true;
                    used = false;

                }
            }

            private int Fireball2(string intName)
            {
                throw new NotImplementedException();
            }

        }
    }
    
}
