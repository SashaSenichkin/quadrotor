using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace quadrotor
{
    public interface IController
    {

        /// <summary>
        /// joystick emulator
        /// </summary>
        float InputHorizontal { get; set; }
        float InputVertical { get; set; }
        float InputThrust { get; set; }
    }
}
