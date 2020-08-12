using ProjectBlocky.Actors;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBlocky.Actors
{
    [HideMonoScript]
    [RequireComponentWarning(typeof(MovementController))]
    [RequireComponent(typeof(CollisionTarget))]
    public class BlockMovement : MonoBehaviour, IPointMovement
    {
        //+ Inspector values

        [SerializeField, HideInInspector]
        private new bool enabled;

        [ShowInInspector]
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                currentDirection = CollisionDirection.None;
                StopPushing();
                enabled = value;
            }
        }

		[HideInInspector]
		[SerializeField]
        private int pushingDelay = 20;

		[ShowInInspector]
		[SuffixLabel("Frames", true)]
		public int PushingDelay
		{
			get
			{
				return pushingDelay;
			}
			set
			{
				pushingDelay = value;
			}
		}

        public float SpeedMultiplier = 1;
        public bool Slide = false;

        //+ Cache data

        private MovementController movementController;
        private CollisionTarget collisionTarget;
        private Vector2 startPosition;

        //+ States

        // Moving

        private Vector2 destination;

        /// <summary>
        /// The direction the object is currently moving.
        /// </summary>
        private CollisionDirection currentDirection = CollisionDirection.None;

        // Pushing

        private bool isPushing = false;

        /// <summary>
        /// Current frames until the object will move.
        /// </summary>
        private int pushingCounter = 0;

        /// <summary>
        /// The direction the player is pushing on the object.
        /// </summary>
        private CollisionDirection pushingDirection = CollisionDirection.None;
 

        private void StopPushing(Collision2D col = null)
        {
            isPushing = false;
            pushingCounter = 0;
        }

        private void StartPushing(CollisionDirection direction)
        {
            if (enabled)
            {
                isPushing = true;
                pushingDirection = direction;
            }
        }
        private void StartMoving(CollisionDirection direction)
        {
            currentDirection = direction;
            destination += GetVectorDirection(direction);
        }

        private void Awake()
        {
            movementController = GetComponent<MovementController>();
            collisionTarget = GetComponent<CollisionTarget>();

            collisionTarget.OnDirectionalCollision += (collisionInformation) =>
            {
                StartPushing(collisionInformation.DirectionTo);
            };
            collisionTarget.OnExitDirectional += StopPushing;
            collisionTarget.OnExitCollision += StopPushing;

            destination = transform.position;
            startPosition = destination;
        }
        
        /// <summary>
        /// Move Transform to position
        /// </summary>
        public void SetTransformPosition(Vector2 position)
        {
            destination = position;
            transform.position = position;
        }

        /// <summary>
        /// Reset transform to original position
        /// </summary>
        public void ResetPosition()
        {
            SetTransformPosition(startPosition);
        }

        private void FixedUpdate()
        {
            // Pushing on object
            if (isPushing)
            {
                pushingCounter++;

                if (currentDirection == CollisionDirection.None && pushingCounter >= pushingDelay && !IsWallInDirection(pushingDirection))
                {
                    StartMoving(pushingDirection);
                }
            }

            // Move object
            if (currentDirection != CollisionDirection.None)
            {
                movementController.AddForceTowardsPointWithSpeedMultiplier(destination, SpeedMultiplier, this);
            }
        }


        public NewPoint ReachedPoint()
        {
            if (Enabled && Slide && !IsWallInDirection(currentDirection))
            {
                destination += GetVectorDirection(currentDirection);
                return new NewPoint(destination);
            }

            currentDirection = CollisionDirection.None;
            return new NewPoint(false);
        }

        /// <summary>
        /// Converts an Enum direction into a Vector2 direction.
        /// </summary>
        private Vector2 GetVectorDirection(CollisionDirection direction)
        {
            switch (direction)
            {
                case CollisionDirection.Up:
                    return Vector2.up;
                case CollisionDirection.Down:
                    return Vector2.down;
                case CollisionDirection.Left:
                    return Vector2.left;
                case CollisionDirection.Right:
                    return Vector2.right;
                case CollisionDirection.None:
                    return Vector2.zero;
            }
            return Vector2.zero;
        }

		private List<RaycastHit2D> cachedRayHIts = new List<RaycastHit2D>();
        private bool IsWallInDirection(CollisionDirection direction)
        {
			return movementController.PerformRays(GetVectorDirection(direction), MovementController.RayTypes.EdgeRays | MovementController.RayTypes.MiddleRay, ref cachedRayHIts).HasPoint;
		}
    }
}