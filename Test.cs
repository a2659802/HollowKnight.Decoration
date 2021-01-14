using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using TMPro;
using DecorationMaster.Util;
using ModCommon;
using DecorationMaster.UI;
using System.Collections;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityEngine.SceneManagement;

namespace DecorationMaster
{
    public class Test
    {
		class TestBehaviour : MonoBehaviour
		{
			private void Update()
            {
				 
            }
		}
		public Test()
        {
            Modding.Logger.LogDebug("Start Test");
			
			Modding.Logger.LogDebug("End Test");
			//Assetfile
		}


        public static void TestGo(GameObject go)
        {
			var door = GameObject.Find("GG_Challenge_Door");

			door.transform.position = HeroController.instance.transform.position;
		}
		public static void TestOnce()
        {
			
        }
		public IEnumerator Dump(string sceneName = null)
		{

			List<string> scenes = new List<string>();
			for (int j = 0; j < USceneManager.sceneCountInBuildSettings; j++)
			{
				string scenePath = SceneUtility.GetScenePathByBuildIndex(j);
				string name = Path.GetFileNameWithoutExtension(scenePath);
				scenes.Add(name);
				var load = USceneManager.LoadSceneAsync(j, LoadSceneMode.Single);
				while (!load.isDone)
				{
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForSeconds(0.2f);
				Scene s = USceneManager.GetActiveScene();
				StringBuilder sb = new StringBuilder();
				foreach (var g in s.GetRootGameObjects())
                {
					Visit(g.transform, 0, null,sb);
                }
				try
				{
					var fs = File.Create($"Z:\\1\\{s.name}.txt");
					StreamWriter sw = new StreamWriter(fs);
					sw.Write(sb.ToString());
					sw.Close();
					fs.Close();
				}
				catch { }
				
				//
			}
			var load_ = USceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
			while (!load_.isDone)
			{
				yield return new WaitForEndOfFrame();
			}
			yield return USceneManager.LoadSceneAsync("Quit_To_Menu");
			while (USceneManager.GetActiveScene().name != Constants.MENU_SCENE)
			{
				yield return new WaitForEndOfFrame();
			}

			
			
		}
		public static void Visit(Transform t,int depth,Transform father,StringBuilder sb)
        {
			if (t == null)
				return;
			string prefix = "";
			for(int i=0;i<depth;i++)
            {
				prefix += "   ";
            }
			string name = father?.name +"/" +t.name;
			string log = $"{prefix}{name}--->layer:{t.gameObject.layer}({((GlobalEnums.PhysLayers)t.gameObject.layer)})";
			sb.AppendLine(log);
			foreach(Transform child in t)
            {
				Visit(child, depth + 1,t,sb);
            }
        }
	}
	
    
	/*
	public class MyTinkEffect : MonoBehaviour
	{
		// Token: 0x06001F5C RID: 8028 RVA: 0x000B93E6 File Offset: 0x000B77E6
		private void Awake()
		{
			this.boxCollider = base.gameObject.GetComponent<BoxCollider2D>();
		}

		// Token: 0x06001F5D RID: 8029 RVA: 0x000B93F9 File Offset: 0x000B77F9
		private void Start()
		{
			this.gameCam = GameCameras.instance;
		}

		// Token: 0x06001F5E RID: 8030 RVA: 0x000B9408 File Offset: 0x000B7808
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.tag == "Nail Attack")
			{
				if (Time.time < this.nextTinkTime)
				{
					return;
				}
				this.nextTinkTime = Time.time + 0.25f;
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy");
				float degrees = (!(playMakerFSM != null)) ? 0f : playMakerFSM.FsmVariables.FindFsmFloat("direction").Value;
				if (this.gameCam)
				{
					this.gameCam.cameraShakeFSM.SendEvent("EnemyKillShake");
				}
				Vector3 position = new Vector3(0f, 0f, 0f);
				Vector3 euler = new Vector3(0f, 0f, 0f);
				Vector3 position2 = HeroController.instance.transform.position;
				Vector3 position3 = collision.gameObject.transform.position;
				bool flag = this.boxCollider != null;
				if (this.useNailPosition)
				{
					flag = false;
				}
				Vector2 vector = Vector2.zero;
				float num = 0f;
				float num2 = 0f;
				if (flag)
				{
					vector = base.transform.TransformPoint(this.boxCollider.offset);
					num = this.boxCollider.bounds.size.x * 0.5f;
					num2 = this.boxCollider.bounds.size.y * 0.5f;
				}
				int cardinalDirection = DirectionUtils.GetCardinalDirection(degrees);
				if (cardinalDirection == 0)
				{
					HeroController.instance.RecoilLeft();
					if (this.sendDirectionalFSMEvents)
					{
						this.fsm.SendEvent("TINK RIGHT");
					}
					if (flag)
					{
						position = new Vector3(vector.x - num, position3.y, 0.002f);
					}
					else
					{
						position = new Vector3(position2.x + 2f, position2.y, 0.002f);
					}
				}
				else if (cardinalDirection == 1)
				{
					HeroController.instance.RecoilDown();
					if (this.sendDirectionalFSMEvents)
					{
						this.fsm.SendEvent("TINK UP");
					}
					if (flag)
					{
						position = new Vector3(position3.x, Mathf.Max(vector.y - num2, position3.y), 0.002f);
					}
					else
					{
						position = new Vector3(position2.x, position2.y + 2f, 0.002f);
					}
					euler = new Vector3(0f, 0f, 90f);
				}
				else if (cardinalDirection == 2)
				{
					HeroController.instance.RecoilRight();
					if (this.sendDirectionalFSMEvents)
					{
						this.fsm.SendEvent("TINK LEFT");
					}
					if (flag)
					{
						position = new Vector3(vector.x + num, position3.y, 0.002f);
					}
					else
					{
						position = new Vector3(position2.x - 2f, position2.y, 0.002f);
					}
					euler = new Vector3(0f, 0f, 180f);
				}
				else
				{
					if (this.sendDirectionalFSMEvents)
					{
						this.fsm.SendEvent("TINK DOWN");
					}
					if (flag)
					{
						position = new Vector3(position3.x, Mathf.Min(vector.y + num2, position3.y), 0.002f);
					}
					else
					{
						position = new Vector3(position2.x, position2.y - 2f, 0.002f);
					}
					euler = new Vector3(0f, 0f, 270f);
				}
				GameObject gameObject = this.blockEffect.Spawn(position, Quaternion.Euler(euler));
				gameObject.GetComponent<AudioSource>().pitch = pitch;
				pitch -= 0.05f;
				if (this.sendFSMEvent)
				{
					this.fsm.SendEvent(this.FSMEvent);
				}
			}
		}
		public float pitch = 1;

		// Token: 0x040020E8 RID: 8424
		public GameObject blockEffect;

		// Token: 0x040020E9 RID: 8425
		public bool useNailPosition;

		// Token: 0x040020EA RID: 8426
		public bool sendFSMEvent;

		// Token: 0x040020EB RID: 8427
		public string FSMEvent;

		// Token: 0x040020EC RID: 8428
		public PlayMakerFSM fsm;

		// Token: 0x040020ED RID: 8429
		public bool sendDirectionalFSMEvents;

		// Token: 0x040020EE RID: 8430
		private BoxCollider2D boxCollider;

		// Token: 0x040020EF RID: 8431
		private bool hasBoxCollider;

		// Token: 0x040020F0 RID: 8432
		private HeroController heroController;

		// Token: 0x040020F1 RID: 8433
		private GameCameras gameCam;

		// Token: 0x040020F2 RID: 8434
		private Vector2 centre;

		// Token: 0x040020F3 RID: 8435
		private float halfWidth;

		// Token: 0x040020F4 RID: 8436
		private float halfHeight;

		// Token: 0x040020F5 RID: 8437
		private const float repeatDelay = 0.25f;

		// Token: 0x040020F6 RID: 8438
		private float nextTinkTime;
	}
	*/
}
