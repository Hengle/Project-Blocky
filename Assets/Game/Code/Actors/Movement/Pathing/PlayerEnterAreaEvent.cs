using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectBlocky.Actors.Pathing
{

    [System.Serializable]
    public class PlayerAndRectEvents : PathingBehavior
    {
        private enum EventType : byte
        {
            OnRectEnter = 0,
            OnRectStay = 1,
            OnRectExit = 3
        }

        [SerializeField, PropertyOrder(1)]
        private EventType eventType = EventType.OnRectEnter;

        [SerializeField, PropertyOrder(3)]
        private Rect playerEnterZone = new Rect();

        private bool playerWasInsideRect = false;

        public override void Update()
        {
            List<Transform> playerTransforms = PlayerManager.Instance.ActivePlayerTransforms;

            for (int i = 0; i < playerTransforms.Count; i++)
            {
                bool rectContainsPLayer = playerEnterZone.Contains((Vector2)playerTransforms[i].position, true);

                if (eventType == EventType.OnRectStay && rectContainsPLayer)
                {
                    ApplyChanges();
                }
                else
                {
                    // If player Enter
                    if (rectContainsPLayer && !playerWasInsideRect ||
                        // If player Exit
                        (!rectContainsPLayer && playerWasInsideRect))
                    {
                        ApplyChanges();
                    }

                    playerWasInsideRect = rectContainsPLayer;
                }

            }
        }

#if UNITY_EDITOR


        [PropertyOrder(2)]
        [OnValueChanged("UpdateSizeOfRect"), ShowInInspector, Header("Changing the padding will remake the rect")]
        private float initialPadding = 0.5f;

        [ShowInInspector, HorizontalGroup("Group0")]
        private float Top
        {
            get
            {
                return playerEnterZone.yMax;
            }
            set
            {
                playerEnterZone.yMax = value;
            }
        }

        [ShowInInspector, HorizontalGroup("Group0")]
        private float Bottom
        {
            get
            {
                return playerEnterZone.yMin;
            }
            set
            {
                playerEnterZone.yMin = value;
            }
        }

        [ShowInInspector, HorizontalGroup("Group1")]
        private float Left
        {
            get
            {
                return playerEnterZone.xMin;
            }
            set
            {
                playerEnterZone.xMin = value;
            }
        }

        [ShowInInspector, HorizontalGroup("Group1")]
        private float Right
        {
            get
            {
                return playerEnterZone.xMax;
            }
            set
            {
                playerEnterZone.xMax = value;
            }
        }

        //test 
        private void UpdateSizeOfRect()
        {
            var nodes = pathingScript.PathingNodes;
            var nodeCount = nodes.Count;

            var startPosition = nodes[0].Position;
            var targetPosition = nodes[0].Position;

            for (int i = 1; i < nodeCount; i++)
            {
                var currentPoint = nodes[i].Position;

                if (currentPoint.x < startPosition.x)
                {
                    startPosition.x = currentPoint.x;
                }
                if (currentPoint.y > startPosition.y)
                {
                    startPosition.y = currentPoint.y;
                }
                if (currentPoint.x > targetPosition.x)
                {
                    targetPosition.x = currentPoint.x;
                }
                if (currentPoint.y < targetPosition.y)
                {
                    targetPosition.y = currentPoint.y;
                }
            }

            startPosition += new Vector2(-initialPadding, initialPadding);
            targetPosition += new Vector2(initialPadding, -initialPadding);

            playerEnterZone = new Rect(startPosition, targetPosition - startPosition);
        }

        private static readonly Color32 transparent = new Color32(220, 0, 0, 32);
        
        protected override void InitializeValues()
        {
            if (playerEnterZone.size == Vector2.zero)
            {
                UpdateSizeOfRect();
            }
        }

        public override void DrawGizmos()
        {
            UnityEditor.Handles.DrawSolidRectangleWithOutline(playerEnterZone, transparent, Color.red);
        }
#endif
    }
}