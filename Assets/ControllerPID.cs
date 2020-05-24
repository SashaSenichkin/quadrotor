using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace quadrotor
{
    public class ControllerPID : MonoBehaviour
    {
        public Transform[] PropellerPoints;
        private Rigidbody rigbody;

        /// <summary>
        /// power (max H)
        /// </summary>
        public float MaxAcceleration;

        public float AirResist = 400;
        public float gravityConstant = 10f;
        private float MaxAngle { get => 40; } // Mathf.Acos(gravityConstant / (4 * Acceleration))

        [Header("PID")]
        public PID angleControllerX = new PID();
        public PID angleControllerY = new PID();
        public PID angleControllerZ = new PID();

        private Vector3 CopterGeneralForce = Vector3.zero;
        private Dictionary<Transform, float> PropellerThrusts;

        /// <summary>
        /// joystick emulator
        /// </summary>
        private float InputHorizontal = 0f;
        private float InputVertical = 0f;
        private float InputThrust = 0f;
        private bool KeepAltitude = true;

        public void SetHorizontal(float value)
        {
            InputHorizontal = value;
        }
        public void SetVertical(float value)
        {
            InputVertical = value;
        }
        public void SetThrust(float value)
        {
            InputThrust = value * MaxAcceleration;
        }
        public void SetKeepAltitude(bool value)
        {
            KeepAltitude = value;
        }


        public float ControlRotate(float targetAngle, float currentAngle, PID controller)
        {
            float dt = Time.fixedDeltaTime;
            float angleError = Mathf.DeltaAngle(currentAngle, targetAngle);
            var result = controller.Calculate(angleError, dt);
            return result;
        }

        //public Vector2 ControlForce(Vector2 movement)
        //{
        //    Vector2 MV = new Vector2();

        //    if (movement != Vector2.zero)
        //    {
        //        if (_flame != null)
        //            _flame.SetActive(true);
        //    }
        //    else
        //    {
        //        if (_flame != null)
        //            _flame.SetActive(false);
        //    }

        //    MV = movement * _thrust;
        //    return MV;
        //}

        void Start()
        {
            rigbody = GetComponent<Rigidbody>();
            if (PropellerPoints.Length == 0)
            {
                Debug.LogError("no propellers Included");
            }

            PropellerThrusts = new Dictionary<Transform, float>();

            foreach (var item in PropellerPoints)
                PropellerThrusts.Add(item, 0f);

            //rigbody.AddRelativeTorque(new Vector3(10,0,0));//debug
        }


        void FixedUpdate()
        {
            ClearBuffers();

            float torqueX = ControlRotate(InputVertical * MaxAngle, transform.eulerAngles.x, angleControllerX);
            float torqueY = ControlRotate(0, transform.eulerAngles.y, angleControllerY);
            float torqueZ = -ControlRotate(InputHorizontal * MaxAngle, transform.eulerAngles.z, angleControllerZ);

            torqueX = torqueX > 1 ? 1 : torqueX;
            torqueY = torqueY > 1 ? 1 : torqueY;
            torqueZ = torqueZ > 1 ? 1 : torqueZ;

            AddTorque(new Vector3(torqueX, torqueY, torqueZ));
            EnviromentResistance();


            foreach (var item in PropellerThrusts)
            {
                var thrust = item.Value;
                rigbody.AddForceAtPosition (thrust * InputThrust * transform.up * Time.fixedDeltaTime, item.Key.position);
            }

            rigbody.AddForce(CopterGeneralForce * Time.fixedDeltaTime);
        }
        private void AddTorque(Vector3 torque)
        {
            foreach (var point in PropellerPoints)
            {
                PropellerThrusts[point] += (transform.position.z - point.position.z) * torque.x;
                PropellerThrusts[point] += (transform.position.x - point.position.x) * torque.z;
            }
        }

        private void GravityCompensation()
        {
            var addToAllPropellers = (rigbody.mass * gravityConstant / InputThrust - PropellerThrusts.Sum(x => x.Value)) / 4;
            foreach (var point in PropellerPoints)
                PropellerThrusts[point] += addToAllPropellers;
        }

        /// <summary>
        /// i want to see vector, so did't use rigid.gravity
        /// </summary>
        private void AddGravity()
        {
            CopterGeneralForce += Vector3.down * rigbody.mass * gravityConstant;
        }
        private void EnviromentResistance()
        {
            CopterGeneralForce += new Vector3(CalculateResistance(rigbody.velocity.x),
                                              CalculateResistance(rigbody.velocity.y),
                                              CalculateResistance(rigbody.velocity.z));

            rigbody.AddRelativeTorque(new Vector3(CalculateResistance(rigbody.angularVelocity.x),
                                                  CalculateResistance(rigbody.angularVelocity.y),
                                                  CalculateResistance(rigbody.angularVelocity.z)));
        }

        float CalculateResistance(float axisSpeed)
        {
            return - Math.Sign(axisSpeed) * axisSpeed * axisSpeed / AirResist;
        }

        private void OnDrawGizmos()
        {
            if (PropellerThrusts == null)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, CopterGeneralForce + transform.position);

            Gizmos.color = Color.red;

            foreach (var item in PropellerThrusts)
            {
                var thrust = item.Value;
                Gizmos.DrawLine(item.Key.position, transform.up * InputThrust * thrust + item.Key.position);
            }

        }
        void ClearBuffers()
        {
            foreach (var point in PropellerPoints)
                PropellerThrusts[point] = 0;

            CopterGeneralForce = Vector3.zero;
        }

    }
}
