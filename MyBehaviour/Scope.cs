using GlobalEnums;
using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.Attr;
using DecorationMaster.Util;
using DecorationMaster.MyBehaviour;
using DecorationMaster.UI;

namespace DecorationMaster.MyBehaviour
{
    //ref https://github.com/AcridStingray3/HollowKnight.ScreenshotMachine.git 
    //望远镜
    class Scope
    {

        private const float mov_speed = 0.25f;
        private  bool _frozenCamera;
        private  bool _toggled;
        public  IEnumerator MoveCamera(Vector3 movement, PlayerAction keyPressed)
        {

            if (!_frozenCamera)
            {
                GameCameras.instance.cameraController.FreezeInPlace();
                _frozenCamera = true;
            }

            while (keyPressed.IsPressed)
            {

                GameCameras.instance.cameraController.transform.position += movement;
                yield return new WaitForSeconds(0.06f);
            }

        }
        public  void MoveCamV2(Vector3 movement)
        {
            if (!_frozenCamera)
            {
                GameCameras.instance.cameraController.FreezeInPlace();
                _frozenCamera = true;
            }
            GameCameras.instance.cameraController.transform.position += movement;
        }
        private  void RestoreCameraBehaviour()
        {

            GameCameras.instance.cameraController.SetMode(CameraController.CameraMode.FOLLOWING);
            GameCameras.instance.cameraController.PositionToHero(false);
            Transform transform = GameCameras.instance.cameraController.transform;
            Vector3 position = transform.position;
            position += new Vector3(0, 0, -38.1f - position.z);
            transform.position = position;

            _frozenCamera = false;
        }
        public  void RemoveCameraLogic(On.CameraController.orig_LateUpdate orig, CameraController self)
        {
            if (!_frozenCamera)
                orig(self);
        }
        public  void Reset(On.GameManager.SceneLoadInfo.orig_NotifyFetchComplete origNotifyFetchComplete, GameManager.SceneLoadInfo sceneLoadInfo)
        {
            RestoreCameraBehaviour();
            ToggleOff();

            origNotifyFetchComplete(sceneLoadInfo);
        }
        public  void ToggleOn()
        {
            HeroController.instance.damageMode = DamageMode.NO_DAMAGE;
            HeroController.instance.vignette.enabled = false;
            HeroController.instance.RelinquishControl();
            On.CameraController.LateUpdate += RemoveCameraLogic;
            _toggled = true;

        }
        public  void ToggleOff()
        {
            HeroController.instance.damageMode = DamageMode.FULL_DAMAGE;
            HeroController.instance.vignette.enabled = true;
            HeroController.instance.RegainControl();
            On.CameraController.LateUpdate -= RemoveCameraLogic;
            RestoreCameraBehaviour();
            _toggled = false;
        }
    
        public  void Update()
        {
            if (!_toggled)
                return;

            if(InputHandler.Instance.inputActions.up.IsPressed)
            {
                MoveCamV2(new Vector3(0, mov_speed));
                //GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0, mov_speed), InputHandler.Instance.inputActions.up));
            }
            else if (InputHandler.Instance.inputActions.down.IsPressed)
            {
                MoveCamV2(new Vector3(0, -mov_speed));
                //GameManager.instance.StartCoroutine(MoveCamera(new Vector3(0, -mov_speed), InputHandler.Instance.inputActions.down));
            }
            else if (InputHandler.Instance.inputActions.left.IsPressed)
            {
                MoveCamV2(new Vector3(-mov_speed, 0));
                //GameManager.instance.StartCoroutine(MoveCamera(new Vector3(-mov_speed, 0), InputHandler.Instance.inputActions.left));
            }
            else if (InputHandler.Instance.inputActions.right.IsPressed)
            {
                MoveCamV2(new Vector3(mov_speed, 0));
                //GameManager.instance.StartCoroutine(MoveCamera(new Vector3(mov_speed, 0), InputHandler.Instance.inputActions.right));
            }
        }


        [Decoration("binoculars")]
        [Description("An binoculars\n allow you watch farther\n attack for enable\n jump for disable", Language = "en-us")]
        [Description("望远镜\n攻击开启\n跳跃关闭")]
        public class Binoculars : CustomDecoration
        {
            private HeroTrigger ht;
            private BoxCollider2D col;
            private Scope scope;
            private void Awake()
            {
                gameObject.layer = (int)GlobalEnums.PhysLayers.INTERACTIVE_OBJECT;
                col = gameObject.AddComponent<BoxCollider2D>();
                //gameObject.AddComponent<ShowColliders>();
                ht = gameObject.AddComponent<HeroTrigger>();

                ht.HeroAtk = () => {
                    scope = new Scope();
                    scope.ToggleOn();
                    Modding.Logger.LogDebug("Binoculars on");
                };
                var tex = GUIController.Instance.images["binoculars"];
                var sr = gameObject.AddComponent<SpriteRenderer>();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            private void Update()
            {
                scope?.Update();
                if (InputHandler.Instance.inputActions.jump.IsPressed)
                {
                    scope?.ToggleOff();
                    scope = null;
                }
            }
            private void OnDestroy()
            {
                scope?.ToggleOff();
                scope = null;
            }
        }
    }
    
    
}
