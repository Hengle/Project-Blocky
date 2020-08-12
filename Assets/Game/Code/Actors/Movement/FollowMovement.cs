using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectBlocky;
using Unity.Mathematics;
using Sirenix.OdinInspector;

namespace ProjectBlocky.Actors
{

    [HideMonoScript]
    [RequireComponentWarning(typeof(TargetFinder), typeof(MovementController))]
    public class FollowMovement : MonoBehaviour, IPointMovement, IAssignTargets
    {
        [Header("Pathing")]
        [SerializeField]
        private bool usePathing = true;
        [SerializeField, EnableIf("usePathing")]
        private LayerMask GoAroundObjects = 0;

        private MovementController movementController;

        private RaycastHit2D[] rayChecker = new RaycastHit2D[1];

        private Vector2 targetPosition;
        private Vector2 currentPoint;
        private Transform currentTarget;

        // states
        private bool followingTarget;
        private bool isActive = true;

        private void Awake()
        {
            movementController = GetComponent<MovementController>();
        }

        private void OnEnable()
        {
            isActive = true;
        }
        private void OnDisable()
        {
            isActive = false;
        }

        private void FixedUpdate()
        {
            if (followingTarget && isActive)
            {
                if (usePathing)
                {
                    movementController.AddForceTowardsPoint(currentPoint, this);
                }
                else
                {
                    movementController.AddForceTowardsPoint(targetPosition, this);
                }
            }
        }

        private Vector2 previousPoint = new Vector2(100, 100);
        public NewPoint ReachedPoint()
        {
            var transformPosition = (Vector2)transform.position;

            // If reached target or can't move, stop.
            if ((transformPosition - targetPosition).sqrMagnitude <= 0.01f || (transformPosition - previousPoint).sqrMagnitude <= 0.01f)
            {
                followingTarget = false;
                return new NewPoint(false);
            }
            // If in same block as target, move to target
            else if ((CollisionUtils.SnapToTile(transformPosition) - CollisionUtils.SnapToTile(targetPosition)).sqrMagnitude <= 0.01f)
            {
                previousPoint = currentPoint;
                currentPoint = targetPosition;
                return new NewPoint(targetPosition);
            }
            else // Move to next tile
            {
                previousPoint = currentPoint;
                currentPoint = GetPointToTarget(targetPosition);
                return new NewPoint(currentPoint);
            }
        }

        private Unity.Mathematics.Random randomNum = new Unity.Mathematics.Random(1);
        
        private Vector2 GetPointToTarget(Vector2 target)
        {

            Vector2 snapTransformPosition = CollisionUtils.SnapToTile(transform.position);
            Vector2 directionToTarget = target - snapTransformPosition;

            float absX = math.abs(directionToTarget.x);
            float absY = math.abs(directionToTarget.y);

            if (absX < 0.5f)
            {
                directionToTarget.x = 0;
            }
            if (absY < 0.5)
            {
                directionToTarget.y = 0;
            }

            //Vector2 verticalDirection = Vector2.zero;
            Vector2 firstDirection = Vector2.zero;
            Vector2 secondDirection = Vector2.zero;

            if (absX > absY)
            {
                firstDirection = new Vector2(directionToTarget.x, 0).normalized;
                secondDirection = new Vector2(0, directionToTarget.y).normalized;
            }
            else if (absY > absX)
            {
                firstDirection = new Vector2(0, directionToTarget.y).normalized;
                secondDirection = new Vector2(directionToTarget.x, 0).normalized;
            }
            else
            {
                if (randomNum.NextInt(2) == 0)
                {
                    firstDirection = new Vector2(directionToTarget.x, 0).normalized;
                    secondDirection = new Vector2(0, directionToTarget.y).normalized;
                }
                else
                {
                    firstDirection = new Vector2(0, directionToTarget.y).normalized;
                    secondDirection = new Vector2(directionToTarget.x, 0).normalized;
                }
            }

            bool first = !WallInDirection(snapTransformPosition, firstDirection, 1);
            bool second = !WallInDirection(snapTransformPosition, secondDirection, 1);
            //bool vertical = !WallInDirection(snapTransformPosition, verticalDirection, 1.414214f);

            //if (vertical && first && second)
            //{
            //    return snapTransformPosition + verticalDirection;
            //}
            //else 
            if (first)
            {
                return snapTransformPosition + firstDirection;
            }
            else if (second)
            {
                return snapTransformPosition + secondDirection;
            }
            else
            {
                return snapTransformPosition;
            }
        }

        /// <param name="direction">Normalized Direction</param>
        private bool WallInDirection(Vector2 transformPosition, Vector2 direction, float distance)
        {
            return (Physics2D.RaycastNonAlloc(transformPosition, direction, rayChecker, distance, GoAroundObjects) != 0);
        }

        public void AssignTargets(List<Transform> targets)
        {
            targetPosition = targets[0].position;

            if (!followingTarget)
            {
                followingTarget = true;

                if (usePathing)
                {
                    var transformPosition = (Vector2)transform.position;

                    // If in same block as target, move to target
                    if ((CollisionUtils.SnapToTile(transformPosition) - CollisionUtils.SnapToTile(targetPosition)).sqrMagnitude <= 0.01f)
                    {
                        previousPoint = currentPoint;
                        currentPoint = targetPosition;
                    }
                    else // Move to next tile
                    {
                        previousPoint = currentPoint;
                        currentPoint = GetPointToTarget(targetPosition);
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (followingTarget)
            {
                var pos = transform.position;
                Gizmos.color = Color.white;
                Gizmos.DrawLine(pos, targetPosition);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(pos, currentPoint);
            }
        }
#endif
    }
}