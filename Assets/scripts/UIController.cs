using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace quadrotor
{
    public class UIController:MonoBehaviour
    {
        public Slider Vertical;
        public Slider Horizontal;
        public Slider Thrust;
        public Slider CameraAngle;

        public GameObject ControllerGO;
        public IController Controller;

        public float InrementRate;
        private void Start()
        {
            Vertical.onValueChanged.AddListener((x) => { if (Mathf.Abs(Controller.InputVertical - x) > 0.01f) Controller.InputVertical = x; });
            Horizontal.onValueChanged.AddListener((x) => { if (Mathf.Abs(Controller.InputHorizontal - x) > 0.01f) Controller.InputHorizontal = x; });
            Thrust.onValueChanged.AddListener((x) => { if (Mathf.Abs(Controller.InputThrust - x) > 0.01f) Controller.InputThrust = x; });
            CameraAngle.onValueChanged.AddListener((x) => { if (Mathf.Abs(Controller.InputCameraAngle - x) > 0.01f) Controller.InputCameraAngle = x; });

            Controller = ControllerGO.GetComponent<IController>();
            if (Controller == null)
                throw new Exception("can't find model script");
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                Thrust.value += InrementRate * Time.fixedDeltaTime;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                Thrust.value -= InrementRate * Time.fixedDeltaTime;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                Vertical.value += InrementRate * Time.fixedDeltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Vertical.value -= InrementRate * Time.fixedDeltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Horizontal.value += InrementRate * Time.fixedDeltaTime;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                Horizontal.value -= InrementRate * Time.fixedDeltaTime;
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                CameraAngle.value -= InrementRate * Time.fixedDeltaTime;
            }
            else if (Input.GetKey(KeyCode.X))
            {
                CameraAngle.value += InrementRate * Time.fixedDeltaTime;
            }
        }
    }
}
