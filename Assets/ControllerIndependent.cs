using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace quadrotor
{
    //independent engines case
    public class ControllerIndependent : MonoBehaviour 
    {
        public Transform[] PropellerPoints;
        Rigidbody rigbody;
        public float Acceleration;
        public float AirResist = 400;


        // Start is called before the first frame update
        void Start()
        {
            rigbody = GetComponent<Rigidbody>();
            if (PropellerPoints.Length == 0)
            {
                Debug.LogError("no propellers Included");
            }
        }

        // Update is called once per frame
        void Update()
        {
            EnviromentResistance();

            if (!Input.anyKey)
                return;

            var lift = 0f;
            if (Input.GetKey(KeyCode.Q))
                lift = 1;

            foreach (var item in PropellerPoints)
            {
                rigbody.AddForceAtPosition(new Vector3(Input.GetAxis("Horizontal"), lift, Input.GetAxis("Vertical")) * Acceleration * Time.deltaTime, item.position);
            }

        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Drag_(physics)
        /// </summary>
        private void EnviromentResistance()
        {
            rigbody.velocity = new Vector3(rigbody.velocity.x * rigbody.velocity.x,
                                             rigbody.velocity.y * rigbody.velocity.y,
                                             rigbody.velocity.z * rigbody.velocity.z) / AirResist;
        }
    }
}
