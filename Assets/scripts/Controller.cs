using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace quadrotor
{
    //rigid engines case
    public class Controller : MonoBehaviour, IController
    {
        public Transform[] PropellerPoints;
        private Rigidbody rigbody;

        /// <summary>
        /// power (max H)
        /// </summary>
        public float Acceleration;

        public float AirResist = 400;
        public float gravityConstant = 10f;

        private Vector3 CopterGeneralForce = Vector3.zero;
        private Dictionary<Transform, float> PropellerThrusts;


        public float InputHorizontal { get; set; }
        public float InputVertical { get; set; }
        public float InputThrust { get; set; }

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
        }


        void FixedUpdate()
        {
            ClearBuffers();

            //AddGravity();
            //if (KeepAltitude)
            //    GravityCompensation();

            EnviromentResistance();
            UserInteractions();


            foreach (var item in PropellerThrusts)
            {
                var thrust = item.Value;

                if (item.Value < 0)
                {
                    Debug.LogWarning("incorrect thrust " + item.Value);
                    thrust = 0;
                }
                else if (item.Value > 1)
                {
                    Debug.LogWarning("incorrect thrust " + item.Value);
                    thrust = 1;
                }

                rigbody.AddForceAtPosition(thrust * Acceleration * transform.up * Time.fixedDeltaTime, item.Key.position);
            }

            rigbody.AddForce(CopterGeneralForce * Time.fixedDeltaTime);
        }
        private void UserInteractions()
        {
            foreach (var point in PropellerPoints)
            {
                PropellerThrusts[point] += (transform.position.z - point.position.z + Mathf.Sin(transform.rotation.eulerAngles.x * Mathf.PI / 180f)) * InputVertical;
                PropellerThrusts[point] += (transform.position.x - point.position.x - Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.PI / 180f)) * InputHorizontal;


            }
        }

        private void GravityCompensation()
        {
            var addToAllPropellers = (rigbody.mass * gravityConstant / Acceleration - PropellerThrusts.Sum(x => x.Value)) / 4;
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

                if (item.Value < 0)
                    thrust = 0;
                else if (item.Value > 1)
                    thrust = 1;
                Gizmos.DrawLine(item.Key.position, transform.up * Acceleration * thrust + item.Key.position);
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
