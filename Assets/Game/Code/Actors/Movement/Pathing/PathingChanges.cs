using UnityEngine;
using Sirenix.OdinInspector;

namespace ProjectBlocky.Actors.Pathing
{
    [System.Serializable]
    public class PathingChanges : IChanges<PathingAutoMovement>
    {
        [SerializeField, HorizontalGroup("Group6"), HideLabel]
        private bool useDirection = false;
        [SerializeField, EnableIf("useDirection"), HorizontalGroup("Group6"), LabelWidth(150)]
        private PathingAutoMovement.Direction setDirection = PathingAutoMovement.Direction.Forward;

        [SerializeField, HorizontalGroup("Group7"), HideLabel]
        private bool useIsLooping = false;
        [SerializeField, EnableIf("useDirection"), HorizontalGroup("Group7"), LabelWidth(150)]
        private bool isLooping = false;

        [SerializeField, HorizontalGroup("Group9"), HideLabel]
        private bool useDelayMovement = false;
        [SerializeField, MinValue(0), EnableIf("useDelayMovement"), HorizontalGroup("Group9"), LabelWidth(150)]
        private float delayMovement = 0;

        [SerializeField, HorizontalGroup("Group10"), HideLabel]
        private bool usePause = false;
        [SerializeField, EnableIf("usePause"), HorizontalGroup("Group10"), LabelWidth(150)]
        private bool pause = false;



        //Applications

        public void ApplyChanges(PathingAutoMovement target)
        {
            if (useIsLooping)
            {
                target.IsLooping = isLooping;
            }

            if (useDirection)
            {
                target.LoopDirection = setDirection;
            }
            
            if (useDelayMovement)
            {
                target.DelayTimer = delayMovement;
            }

            if (usePause)
            {
                target.IsPaused = pause;
            }
        }

#if UNITY_EDITOR
        public void InitializeValues(PathingAutoMovement target) { }
#endif
    }
}