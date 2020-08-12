using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBlocky.Actors
{

    /// <summary>
    /// Use MovementController Add and Remove Speed Multiplier methods to interact.
    /// </summary>
    public interface ISpeedMultiplier
    {

        byte ID { get; }
        float SpeedMultiplier { get; set; }

        void InitializeSpeedMultiplier(MovementController movementController, byte ID);
    }
}