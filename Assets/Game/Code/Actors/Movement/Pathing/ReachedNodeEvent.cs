using UnityEngine;
using Sirenix.OdinInspector;

namespace ProjectBlocky.Actors.Pathing
{
    public class ReachedNodeEvent : PathingBehavior
    {
        [SerializeField, ValueDropdown("PathingNodeList")]
        private int runWhenReachedPoint = 0;

        public override void Initialize()
        {
            pathingScript.OnReachedPoint += PathingScript_OnReachedPoint;
        }

        private void PathingScript_OnReachedPoint(int point)
        {
            if (runWhenReachedPoint == point)
            {
                ApplyChanges();
            }
        }

#if UNITY_EDITOR
        private ValueDropdownList<int> pathingNodeList;

        public ValueDropdownList<int> PathingNodeList
        {
            get
            {
                if (pathingNodeList == null) pathingNodeList = new ValueDropdownList<int>();

                return pathingNodeList;
            }
        }
        
        protected override void InitializeValues()
        {
            pathingNodeList = pathingScript.PathingNodeList;
        }
#endif
    }
}