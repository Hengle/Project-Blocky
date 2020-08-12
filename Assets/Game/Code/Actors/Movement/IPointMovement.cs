using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBlocky.Actors
{

    public interface IPointMovement
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        NewPoint ReachedPoint();
    }

    public struct NewPoint
    {
        public Vector2 Point;
        public bool HasPoint;

        public NewPoint(Vector2 point)
        {
            Point = point;
            HasPoint = true;
        }

        public NewPoint(bool hasPoint = false)
        {
            Point = Vector2.zero;
            HasPoint = hasPoint;
        }
    }
}