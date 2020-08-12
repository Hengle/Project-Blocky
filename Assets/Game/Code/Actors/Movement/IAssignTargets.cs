using UnityEngine;
using System.Collections.Generic;

namespace ProjectBlocky.Actors
{
    public interface IAssignTargets
    {
        void AssignTargets(List<Transform> targets);
    }
}