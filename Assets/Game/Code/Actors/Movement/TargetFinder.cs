using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace ProjectBlocky.Actors
{
    /// <summary>
    /// Searches a list of possible targets for a target
    /// </summary>
    public class TargetFinder : MonoBehaviour
    {
        //+ Serialized

        [SerializeField, Tooltip("Include the players in targetsToSearchFor")]
        private bool usePlayerAsTargets = true;
        [SerializeField, Header("For manually adding targets")]
        private List<Transform> targetsToSearchFor = null;

        [SerializeField, HideInInspector, Header("If left empty, it will send targets to all matching interfaces on object")]
        private List<IAssignTargets> assignScripts = new List<IAssignTargets>();
        [SerializeField]
        private float DistanceToStartFollowing = 4.5f;
        [SerializeField]
        private ReturnOption returnOption = ReturnOption.ReturnClosestTarget;

        [Header("Line of Sight")]
        [SerializeField]
        private bool useLineOfSight = true;
        [SerializeField, EnableIf("useLineOfSight")]
        private LayerMask LineOfSightBlockers = 0;

        //+ Cached

        private enum ReturnOption : byte
        {
            ReturnClosestTarget = 0,
            ReturnAllTargets = 1
        }

        private RaycastHit2D[] rayChecker = new RaycastHit2D[1];
        private List<Transform> cachedTransforms = new List<Transform>();

        private void Awake()
        {
            if (assignScripts.Count == 0)
            {
                assignScripts = new List<IAssignTargets>(GetComponents<IAssignTargets>());
            }

            InvokeRepeating("CheckForTargets", 0, 0.1f);
        }

        /// <summary>
        /// Using multipleTargets?
        /// Using update if closer target?
        /// </summary>
        private void CheckForTargets()
        {
            cachedTransforms.Clear();
            Vector2 transformPosition = transform.position;
            var targetCount = targetsToSearchFor.Count;
            float garbage;

            switch (returnOption)
            {
                case ReturnOption.ReturnAllTargets:

                    // Check players if using players
                    if (usePlayerAsTargets)
                    {
                        var players = PlayerManager.Instance.ActivePlayerTransforms;
                        var playerCount = players.Count;

                        for (int i = 0; i < playerCount; i++)
                        {
                            if (ValidateTarget(transformPosition, players[i].position, out garbage))
                            {
                                cachedTransforms.Add(players[i]); // save transform
                            }
                        }
                    }
                    
                    for (int i = 0; i < targetCount; i++)
                    {
                        if (ValidateTarget(transformPosition, targetsToSearchFor[i].position, out garbage))
                        {
                            cachedTransforms.Add(targetsToSearchFor[i]); // save transform
                        }
                    }

                    if (cachedTransforms.Count > 0)
                    {
                        for (int i = 0; i < assignScripts.Count; i++)
                        {
                            assignScripts[i].AssignTargets(cachedTransforms);
                        }
                    }
                    break;

                case ReturnOption.ReturnClosestTarget:

                    float closestTargetMag = 10000;
                    Transform closestTarget = null;
                    bool foundtarget = false;
                    float targetMagnitude;

                    // Check players if using players
                    if (usePlayerAsTargets)
                    {
                        var players = PlayerManager.Instance.ActivePlayerTransforms;
                        var playerCount = players.Count;

                        for (int i = 0; i < playerCount; i++)
                        {
                            if (ValidateTarget(transformPosition, players[i].position, out targetMagnitude))
                            {
                                if (targetMagnitude < closestTargetMag) // Save closest target
                                {
                                    foundtarget = true;
                                    closestTarget = players[i];
                                    closestTargetMag = targetMagnitude;
                                }
                            }
                        }
                    }
                    
                    for (int i = 0; i < targetCount; i++)
                    {
                        if (ValidateTarget(transformPosition, targetsToSearchFor[i].position, out targetMagnitude))
                        {
                            if (targetMagnitude < closestTargetMag) // Save closest target
                            {
                                foundtarget = true;
                                closestTarget = targetsToSearchFor[i];
                                closestTargetMag = targetMagnitude;
                            }
                        }
                    }

                    if (foundtarget)
                    {
                        for (int i = 0; i < assignScripts.Count; i++)
                        {
                            cachedTransforms.Add(closestTarget);
                            assignScripts[i].AssignTargets(cachedTransforms);
                        }
                    }
                    break;
            }
        }
        
        private bool ValidateTarget(Vector2 transformPosition, Vector2 targetPosition, out float magnitude)
        {
            Vector2 direction = targetPosition - transformPosition;
            magnitude = direction.magnitude;
            direction = direction.normalized;

            // If target in distance
            if (magnitude < DistanceToStartFollowing)
            {
                // If line of sight/not using
                if (!useLineOfSight || (useLineOfSight && TargetInLineOfSight(transformPosition, direction, magnitude)))
                {
                    // Target can be followed
                    return true;
                }
            }

            return false;

        }

        private bool TargetInLineOfSight(Vector2 transformPosition, Vector2 directionToTarget, float magnitude)
        {
            return (0 == Physics2D.RaycastNonAlloc(transformPosition, directionToTarget, rayChecker, magnitude, LineOfSightBlockers));
        }

#if UNITY_EDITOR
        [ShowInInspector]
        public List<IAssignTargets> AssignTargetToScripts
        {
            get
            {
                return assignScripts;
            }
            set
            {
                if (value != null)
                {
                    assignScripts = value;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, DistanceToStartFollowing);
        }
#endif
    }
}