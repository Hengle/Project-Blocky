using UnityEngine;
using Sirenix.OdinInspector;

namespace ProjectBlocky.Actors.Pathing
{
    [System.Serializable, HideReferenceObjectPicker]
    public class NodeChanges : IChanges<PathingAutoMovement>
    {
        //+ Data

        [SerializeField, ValueDropdown("PathingNodeList"), LabelText("Node")]
        private int indexID = 0;

        [HorizontalGroup("Group0"), HideLabel]
        [SerializeField]
        private bool useEnabled = false;
        [SerializeField, EnableIf("useEnabled"), HorizontalGroup("Group0")]
        private bool enabled = false;

        [SerializeField, HorizontalGroup("Group1"), HideLabel]
        private bool usePosition = false;
        [SerializeField, EnableIf("usePosition"), HorizontalGroup("Group1")]
        private float positionX = 0;
        [SerializeField, EnableIf("usePosition"), HorizontalGroup("Group1")]
        private float positionY = 0;

        [SerializeField, HorizontalGroup("Group2"), HideLabel]
        private bool useForwardSpeed = false;
        [SerializeField, MinValue(0), EnableIf("useForwardSpeed"), HorizontalGroup("Group2"), LabelWidth(150)]
        private float forwardSpeedMultiplier = 1;

        [SerializeField, HorizontalGroup("Group3"), HideLabel]
        private bool useBackwardSpeed = false;
        [SerializeField, MinValue(0), EnableIf("useBackwardSpeed"), HorizontalGroup("Group3"), LabelWidth(150)]
        private float backwardSpeedMultiplier = 1;

        [SerializeField, HorizontalGroup("Group4"), HideLabel]
        private bool useForwardDelay = false;
        [SerializeField, MinValue(0), EnableIf("useForwardDelay"), HorizontalGroup("Group4"), LabelWidth(150), Tooltip("Delay when reaching this node")]
        private float forwardDelayTime = 0;

        [SerializeField, HorizontalGroup("Group5"), HideLabel]
        private bool useBackwardDelay = false;
        [SerializeField, MinValue(0), EnableIf("useBackwardDelay"), HorizontalGroup("Group5"), LabelWidth(150), Tooltip("Delay when reaching this node")]
        private float backwardDelayTime = 0;

        [SerializeField, HorizontalGroup("Group6"), HideLabel]
        private bool usePause = false;
        [SerializeField, EnableIf("usePause"), HorizontalGroup("Group6"), LabelWidth(150), Tooltip("Will stop movement when it reaches next node"), LabelText("Pause on Node")]
        private bool usePauseWhenReachedNode = false;


        //+ Logic

#if UNITY_EDITOR

        // Create node index dropdown menu
        private ValueDropdownList<int> pathingNodeList;

        private bool isInitialized = false;

        public ValueDropdownList<int> PathingNodeList
        {
            get
            {
                if (pathingNodeList == null) pathingNodeList = new ValueDropdownList<int>();

                return pathingNodeList;
            }
        }

        public void InitializeValues(PathingAutoMovement target)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                pathingNodeList = target.PathingNodeList;
            }
        }
#endif

        public void ApplyChanges(PathingAutoMovement target)
        {
            PathingNode node = target.PathingNodes[indexID];

            if (useEnabled)
            {
                node.IsEnabled = enabled;
            }

            if (usePosition)
            {
                node.Position = new Vector2(positionX, positionY);
            }

            if (useForwardSpeed)
            {
                node.ForwardSpeedMultiplier = forwardSpeedMultiplier;
            }
            if (useBackwardSpeed)
            {
                node.BackwardSpeedMultiplier = backwardSpeedMultiplier;
            }

            if (useForwardDelay)
            {
                node.ForwardDelayTime = forwardDelayTime;
            }
            if (useBackwardDelay)
            {
                node.BackwardDelayTime = backwardDelayTime;
            }

            if (usePause)
            {
                node.PauseAtNode = usePauseWhenReachedNode;
            }
        }
    }
}