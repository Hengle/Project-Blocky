using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBlocky.Actors.Pathing
{
    [HideMonoScript]
    [RequireComponentWarning(typeof(MovementController))]
    public partial class PathingAutoMovement : SerializedMonoBehaviour, IPointMovement
    {
        private MovementController movementController;

        [Space]
        [OdinSerialize, LabelText("Events"), ListDrawerSettings(Expanded = true, ShowIndexLabels = false, NumberOfItemsPerPage = 1), ValidateInput("ValidateBehaviors")]
        private List<PathingBehavior> customBehaviors = new List<PathingBehavior>();

        [SerializeField, HideInInspector]
        private List<PathingNode> pathingNodes = new List<PathingNode>();

        public List<PathingNode> PathingNodes
        {
            get
            {
                return pathingNodes;
            }
        }

        public enum Direction : byte
        {
            Forward,
            Backward
        }

        [SerializeField, EnableIf("isLooping")]
        private Direction loopDirection;

        public Direction LoopDirection
        {
            set
            {
                loopDirection = value;
                moveDirection = value;
            }
        }

        private Direction moveDirection = Direction.Forward;
        [SerializeField, EnableIf("Loopable")]
        private bool isLooping = false;

        public bool IsLooping
        {
            set
            {
                if (value == false)
                {
                    isLooping = false;
                }
                else if (Loopable())
                {
                    isLooping = true;
                }
                else
                {
                    isLooping = false;
                }
            }
        }

        [SerializeField, HideInInspector]
        private int loopIndex;

        private int currentNode;

        private int targetNode = 0;
        public int TargetNode
        {
            get
            {
                return targetNode;
            }
        }


        private float delayTimer;
        public float DelayTimer
        {
            set
            {
                delayTimer = value;
            }
        }

        private bool isPaused = false;
        public bool IsPaused
        {
            get
            {
                return isPaused;
            }
            set
            {
                isPaused = value;
            }
        }

        [ExecuteInEditMode]
        private void Awake()
        {
            movementController = GetComponent<MovementController>();
            
            if (pathingNodes.Count > 1)
            {
                ReachedPoint();
            }
            for (int i = 0; i < customBehaviors.Count; i++)
            {
                customBehaviors[i].Initialize();
            }
        }
        
        private void FixedUpdate()
        {
            for (int i = 0; i < customBehaviors.Count; i++)
            {
                customBehaviors[i].Update();
            }

            if (!isPaused)
            {
                if (delayTimer > 0)
                {
                    delayTimer -= Time.fixedDeltaTime;
                }
                else if (pathingNodes.Count > 1)
                {
                    var currentPathingNode = PathingNodes[currentNode];
                    var speedMultiplier = moveDirection == Direction.Forward ? currentPathingNode.ForwardSpeedMultiplier : currentPathingNode.BackwardSpeedMultiplier;
                    movementController.AddForceTowardsPointWithSpeedMultiplier(pathingNodes[targetNode].Position, speedMultiplier, this);
                }
            }
        }

        public event ReachedPointEvent OnReachedPoint;
        public delegate void ReachedPointEvent(int point);

        public NewPoint ReachedPoint()
        {
            OnReachedPoint?.Invoke(targetNode);

            isPaused = pathingNodes[targetNode].PauseAtNode;

            return SetNextNode();

        }
        
        private NewPoint SetNextNode()
        {
            currentNode = targetNode;

            NextNode(ref targetNode, ref moveDirection);

            var currentPathNode = pathingNodes[currentNode];
            if (moveDirection == Direction.Forward)
            {
                delayTimer = currentPathNode.ForwardDelayTime;
            }
            else
            {
                delayTimer = currentPathNode.BackwardDelayTime;
            }

            if (delayTimer <= 0 || !IsPaused)
            {
                return new NewPoint(pathingNodes[targetNode].Position);
            }
            else
            {
                return new NewPoint(false);
            }
        }

        private void NextNode(ref int newNode, ref Direction newDirection)
        {
            int lastIndex = pathingNodes.Count - 1;
            int firstIndex = 0;

            // check for disabled nodes
            while (pathingNodes[firstIndex].IsEnabled == false)
            {
                firstIndex++;
            }
            while (pathingNodes[lastIndex].IsEnabled == false)
            {
                lastIndex--;
            }

            // if loop and reach loop point
            if (isLooping)
            {
                // force reset path
                if (newNode < loopIndex && loopDirection == Direction.Backward)
                {
                    newDirection = Direction.Forward;
                }
                else
                {
                    newDirection = loopDirection;
                }

                // get under node
                if (loopDirection == Direction.Backward && newNode == loopIndex)
                {
                    newNode = lastIndex;
                }
                else if (loopDirection == Direction.Forward && newNode == lastIndex)
                {
                    newNode = loopIndex;
                }
            }
            // change direction if reached end
            else
            {
                if (newDirection == Direction.Forward && newNode >= lastIndex)
                {
                    newDirection = Direction.Backward;
                }
                else if (newDirection == Direction.Backward && newNode <= firstIndex)
                {
                    newDirection = Direction.Forward;
                }
            }

            // get next node
            if (newDirection == Direction.Forward)
            {
                do
                {
                    newNode++;
                } while (pathingNodes[newNode].IsEnabled == false);
            }
            else
            {
                do
                {
                    newNode--;
                } while (pathingNodes[newNode].IsEnabled == false);
            }
        }

        private bool switchLoopOnce = false;
        public bool Loopable()
        {
            if (pathingNodes.Count > 2)
            {
                int lastIndex = pathingNodes.Count - 1;
                while (pathingNodes[lastIndex].IsEnabled == false)
                {
                    lastIndex--;
                }

                for (int i = 0; i < pathingNodes.Count - 3; i++)
                {
                    if (pathingNodes[i].Position == pathingNodes[lastIndex].Position && pathingNodes[i].IsEnabled == true)
                    {
                        if (!switchLoopOnce)
                        {
                            switchLoopOnce = true;
                            isLooping = true;
                        }

                        loopIndex = i;
                        return true;
                    }
                }
            }
            switchLoopOnce = false;
            isLooping = false;
            return false;
        }
    }
}