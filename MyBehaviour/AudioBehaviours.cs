using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.Attr;
using DecorationMaster.Util;
using DecorationMaster.UI;
namespace DecorationMaster.MyBehaviour
{

    public class AudioBehaviours
    {
        public const int NoteMax = 10;
        public static AudioClip[] _n;
        public static AudioClip[] Notes { get {
                if (_n!=null)
                    return _n;
                _n = new AudioClip[NoteMax];
                for(int i=0;i<NoteMax;i++)
                {
                    _n[i] = WavHelper.GetAudioClip($"note_{i + 1}");
                }
                return _n;
            } }
       
        [Decoration("note_platform")]
        public class AuidoPlatform : CustomDecoration
        {
            public AudioSource au;
            private void Awake()
            {
                au = gameObject.AddComponent<AudioSource>();
                var col = gameObject.AddComponent<BoxCollider2D>();
                var sr = gameObject.AddComponent<SpriteRenderer>();
                col.offset = new Vector2(0.009658813f, -0.2943649f);
                col.size = new Vector2(2.669769f, 0.3687286f);
                gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                //col.isTrigger = true;
                gameObject.layer = 8;
                var tex = GUIController.Instance.images["pianokey"];
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            private void Start()
            {
                //transform.position += new Vector3(0, 0, 1);
            }
            private void OnCollisionEnter2D(Collision2D col)
            {
                //Logger.LogDebug(col.gameObject.name);
                if (col.gameObject.layer == 9)
                {
                    var n = ((ItemDef.AuidoItem)item).Note;
                    AudioClip clip = Notes[n-1];
                    au.PlayOneShot(clip,0.8f+(n/(float)NoteMax)*0.2f);
                }
            }
        }
    }
}
