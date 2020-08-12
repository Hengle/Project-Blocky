using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectBlocky.Actors.Pathing
{

    [System.Serializable]
    [HideLabel, InlineProperty, HideReferenceObjectPicker]
    public class PathingNode
    {
        [SerializeField, HideInInspector]
        private bool isEnabled = true;
        [SerializeField, HideInInspector]
        private Vector2 position;
        [SerializeField, HideInInspector, MinValue(0)]
        private float forwardSpeedMultiplier = 1;
        [SerializeField, HideInInspector, MinValue(0)]
        private float backwardSpeedMultiplier = 1;
        [SerializeField, MinValue(0), SuffixLabel("seconds", true)]
        private float forwardDelayTime = 0;
        [SerializeField, MinValue(0), SuffixLabel("seconds", true)]
        private float backwardDelayTime = 0;
        [SerializeField, HideInInspector]
        private PathingAutoMovement parent;
        [SerializeField, HideInInspector]
        private bool pauseAtNode = false;
        
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        [ShowInInspector]
        public float ForwardSpeedMultiplier
        {
            get
            {
                return forwardSpeedMultiplier;
            }
            set
            {

                forwardSpeedMultiplier = math.max(0, value);
            }
        }

        [ShowInInspector]
        public float BackwardSpeedMultiplier
        {
            get
            {
                return backwardSpeedMultiplier;
            }
            set
            {

                backwardSpeedMultiplier = math.max(0, value);
            }
        }

        public float ForwardDelayTime
        {
            get
            {
                return forwardDelayTime;
            }

            set
            {
                forwardDelayTime = math.max(0, value);
            }
        }

        public float BackwardDelayTime
        {
            get
            {
                return backwardDelayTime;
            }

            set
            {
                backwardDelayTime = math.max(0, value);
            }
        }

        [ShowInInspector]
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                parent.Loopable();
            }
        }
        
       [ShowInInspector]
        public bool PauseAtNode
        {
            set
            {
                pauseAtNode = value;
            }
            get
            {
                return pauseAtNode;
            }
        }


        public PathingNode(PathingAutoMovement parent, Vector2 position)
        {
            this.parent = parent;
            this.position = position;
        }
    }
}