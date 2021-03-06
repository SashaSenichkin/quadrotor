﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace quadrotor
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/PID_controller
    /// </summary>
    public class PID
    {
        public float Kp, Ki, Kd;

        private float lastError;
        private float P, I, D;

        public PID(float pFactor = 5f, float iFactor = 0, float dFactor = 3f)
        {
            Kp = pFactor;
            Ki = iFactor;
            Kd = dFactor;
        }

        public float Calculate(float error, float dt)
        {
            P = error;
            I += error * dt;
            D = (error - lastError) / dt;
            lastError = error;

            float CO = P * Kp + I * Ki + D * Kd;
            return CO;
        }
    }
}
