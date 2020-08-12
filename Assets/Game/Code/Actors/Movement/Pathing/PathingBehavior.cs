using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace ProjectBlocky.Actors.Pathing
{

    [System.Serializable]
    public abstract class PathingBehavior
    {
        [SerializeField, HideInInspector]
        protected PathingAutoMovement pathingScript;

        [Space()]
        [SerializeField, ValidateInput("ValidateChanges"), ListDrawerSettings(Expanded = true, ShowIndexLabels = false), LabelText("Actions"), PropertyOrder(100)]
        private List<IChanges<PathingAutoMovement>> changes = new List<IChanges<PathingAutoMovement>>();

        public virtual void Initialize() { }
        public virtual void Update() { }

        protected void ApplyChanges()
        {
            for (int i = 0; i < changes.Count; i++)
            {
                changes[i].ApplyChanges(pathingScript);
            }
        }
        //todo figure out how to instantiate this after creation

#if UNITY_EDITOR

        public virtual void DrawGizmos() { }

        private bool isInitialized = false;
        public void InitializeValues(PathingAutoMovement pathingScript)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                this.pathingScript = pathingScript;
                InitializeValues();
            }
        }

        private bool ValidateChanges(List<IChanges<PathingAutoMovement>> changes)
        {
            for (int i = 0; i < changes.Count; i++)
            {
                changes[i].InitializeValues(pathingScript);
            }
            return true;
            // todo peep.
        }

        /// <summary>
        /// Custom Initialize for Editor.
        /// </summary>
        protected virtual void InitializeValues() { }
#endif
    }
}