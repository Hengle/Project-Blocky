#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using Unity.Mathematics;
using ProjectBlocky;

namespace ProjectBlocky.Actors.Pathing
{
    //++ Grid Position Handle

    [CustomEditor(typeof(PathingAutoMovement))]
    public class GridPositionHandle : OdinEditor
    {
        PathingAutoMovement pathingScript;

        private List<PathingNode> PathingNodes
        {
            get
            {
                return pathingScript.PathingNodes;
            }
        }

        private void Awake()
        {
            pathingScript = target as PathingAutoMovement;
        }

        private void OnSceneGUI()
        {
            // Fix Node count to always be > 0
            if (PathingNodes.Count == 0)
            {
                PathingNodes.Add(new PathingNode(pathingScript, pathingScript.transform.position));
                pathingScript.RemakeNodeDropdownCount();
            }

            Event currentEvent = Event.current;
            bool isMouseEvent = (currentEvent.type == EventType.MouseDown && currentEvent.button == 1);
            
            if (!Application.isPlaying)
            {
                // Move nodes to new transform position
                if (pathingScript.transform.hasChanged)
                {
                    var nodeOffset = (Vector2)pathingScript.transform.position - PathingNodes[0].Position;
                    var nodeCount = PathingNodes.Count;

                    for (int i = 0; i < nodeCount; i++)
                    {
                        PathingNodes[i].Position += nodeOffset;
                    }
                }
            }

            for (int i = 0; i < PathingNodes.Count; i++)
            {
                //allow nodes to be moved
                MoveNode(i);

                //label nodes
                Handles.Label(PathingNodes[i].Position + new Vector2(-0.09f, 0.13f), i.ToString(), SirenixGUIStyles.BoldLabelCentered);

                // Check for click
                if (isMouseEvent && new Rect(PathingNodes[i].Position - (Vector2.one / 4), Vector2.one / 2).Contains(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin))
                {
                    if (currentEvent.control) //Holding Control
                    {
                        pathingScript.SelectedNodeIndex = i; // Select node
                    }
                    else
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Add"), false, AddNode, i.ToString());
                        if (i != 0) // If not first node
                        {
                            menu.AddItem(new GUIContent("Delete"), false, DeleteNode, i.ToString());
                        }
                        menu.ShowAsContext();
                    }
                }
            }
        }
        
        private void MoveNode(int index)
        {
            var node = pathingScript.PathingNodes[index];

            EditorGUI.BeginChangeCheck();

            Vector3 newTargetPosition;
            if (node.IsEnabled == true)
            {
                Handles.color = Color.white;
                newTargetPosition = Handles.FreeMoveHandle(node.Position, Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereHandleCap);
            }
            else
            {
                Handles.color = Color.gray;
                newTargetPosition = Handles.FreeMoveHandle(node.Position, Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereHandleCap);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pathingScript, "Change Look At Target Position");

                node.Position = CollisionUtils.SnapToTile(newTargetPosition);
                if (index == 0) { pathingScript.transform.position = node.Position; }
            }
        }

        /// <summary>
        /// Adds a pathing node
        /// </summary>
        /// <param name="obj">Boxxed int index for pathing node insertion</param>
        private void AddNode(object obj)
        {
            Undo.RecordObject(pathingScript, "Added node");
            var index = int.Parse(obj as string);

            // Fix selected node
            if (index + 1 >= pathingScript.SelectedNodeIndex) { pathingScript.SelectedNodeIndex++; }

            pathingScript.PathingNodes.Insert(index + 1, new PathingNode(pathingScript, pathingScript.PathingNodes[index].Position));

            pathingScript.RemakeNodeDropdownCount();
        }

        /// <summary>
        /// Deletes a pathing node
        /// </summary>
        /// <param name="obj">Boxxed int index to delete from pathing nodes</param>
        private void DeleteNode(object obj)
        {
            Undo.RecordObject(pathingScript, "Removed node");
            var index = int.Parse(obj as string);

            // Fix selected node
            if (index == pathingScript.SelectedNodeIndex) { pathingScript.SelectedNodeIndex = -1; }
            else if (index < pathingScript.SelectedNodeIndex) { pathingScript.SelectedNodeIndex--; }

            pathingScript.PathingNodes.RemoveAt(index);

            pathingScript.RemakeNodeDropdownCount();
        }
    }


    //++ Pathing Auto Movement
    
    public partial class PathingAutoMovement
    {
        private bool ValidateBehaviors(List<PathingBehavior> behaviors)
        {
            if (!Application.isPlaying)
            {
                // construct behaviors and changes
                for (int i = 0; i < behaviors.Count; i++)
                {
                    behaviors[i].InitializeValues(this);
                }
            }

            // Todo Validate behavior for multiple components.
            return true;
        }

        [HideInInspector]
        private int selectedNodeIndex = -1;

        public int SelectedNodeIndex
        {
            get
            {
                return selectedNodeIndex;
            }
            set
            {
                selectedNodeIndex = value;

                Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
            }
        }

        [TitleGroup("Selected Node", "Press Ctrl + Right Click in Scene to select node."), ShowInInspector]
        private PathingNode SelectedNode
        {
            get
            {
                if (selectedNodeIndex == -1) { return null; }
                else return PathingNodes[selectedNodeIndex];
            }
            set
            {
                PathingNodes[selectedNodeIndex] = value;
            }
        }

        private bool LoopingStart
        {
            get
            {
                Vector2 lastPoint = pathingNodes[pathingNodes.Count - 1].Position;
                if (lastPoint == (Vector2)transform.position)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Clamps target & current nodes between count.
        /// Remakes the dropdown list.
        /// </summary>
        public void RemakeNodeDropdownCount()
        {
            if (PathingNodeList == null) pathingNodeList = new ValueDropdownList<int>();
            PathingNodeList.Clear();

            for (int i = 0; i < pathingNodes.Count; i++)
            {
                PathingNodeList.Add("Node " + i, i);
            }
        }

        private ValueDropdownList<int> pathingNodeList = new ValueDropdownList<int>();
        
        public ValueDropdownList<int> PathingNodeList
        {
            get
            {
                return pathingNodeList;
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw gizmos for custom behaviors
            for (int i = 0; i < customBehaviors.Count; i++)
            {
                customBehaviors[i].DrawGizmos();
            }

            var node = SelectedNode;
            if (node != null)
            {
                Gizmos.DrawWireSphere(node.Position, 0.5f);
            }

            //Draw Link Nodes
            if (pathingNodes.Count > 1)
            {
                for (int i = 0; i + 1 < PathingNodes.Count; i++)
                {
                    if (pathingNodes[i].IsEnabled == true && pathingNodes[i + 1].IsEnabled == true)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(PathingNodes[i].Position, PathingNodes[i + 1].Position);
                    }
                    else
                    {
                        Gizmos.color = Color.grey;
                        Gizmos.DrawLine(PathingNodes[i].Position, PathingNodes[i + 1].Position);
                    }

                }

                Gizmos.color = Color.white;
                for (int i = 0; i + 2 < PathingNodes.Count; i++)
                {
                    if (pathingNodes[i + 1].IsEnabled == false)
                    {
                        int nextFreeNode = i + 2;
                        while (pathingNodes[nextFreeNode].IsEnabled == false)
                        {
                            nextFreeNode++;
                        }
                        Gizmos.DrawLine(PathingNodes[i].Position, PathingNodes[nextFreeNode].Position);
                        i = nextFreeNode - 1;
                    }
                }
            }
        }

        [Button(ButtonHeight = 20, Name = "Reset")]
        void ResetScript()
        {
            Undo.RecordObject(this, "Change Look At Target Position");

            SelectedNodeIndex = -1;

            PathingNodes.Clear();
            customBehaviors.Clear();

            pathingNodes.Add(new PathingNode(this, transform.position));
            RemakeNodeDropdownCount();
        }
    }
}

#endif