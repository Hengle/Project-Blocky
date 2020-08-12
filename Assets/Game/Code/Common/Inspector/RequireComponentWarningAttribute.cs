#if UNITY_EDITOR
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Sirenix.OdinInspector
{
    /// <summary>
    /// Displays a warning if your game object is missing a component type.
    /// Works with abstract classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireComponentWarningAttribute : Attribute
    {
        public readonly Type[] RequiredTypes;
        public StringBuilder Message;

        public RequireComponentWarningAttribute(params Type[] types)
        {
            RequiredTypes = types;
            var typeLength = types.Length;
            StringBuilder myMessage = (typeLength == 1) ? new StringBuilder("Requires component: ") : new StringBuilder("Requires Components: ");
            for (int i = 0; i < typeLength; i++)
            {
                myMessage.AppendFormat("{0} & ", types[i]);
            }
            Message = new StringBuilder(myMessage.Length);
        }
    }
}


namespace Sirenix.OdinInspector.Editor.Drawers
{
    [ResolverPriority(-10.0)]
    public class RequireComponentWarningProcessor : OdinPropertyProcessor
    {
        private List<string> typeNames = new List<string>();
        private static readonly MemberInfo InjectedMemberInfo = typeof(RequireComponentWarningProcessor).GetMember("InjectedMember", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).First<MemberInfo>();

        private static void InjectedMember()
        {
        }

        private RequireComponentWarningAttribute GetClassDefinedRequireComponent(InspectorProperty property)
        {
            if (property.Tree.SecretRootProperty == property)
            {
                return property.Attributes.GetAttribute<RequireComponentWarningAttribute>();
            }
            else
            {
                return property.ValueEntry.TypeOfValue.GetAttribute<RequireComponentWarningAttribute>();
            }
        }

        public override bool CanProcessForProperty(InspectorProperty property)
        {
            return this.GetClassDefinedRequireComponent(property) != null;
        }

        public override void ProcessMemberProperties(List<InspectorPropertyInfo> memberInfos)
        {
            var refAttribute = GetClassDefinedRequireComponent(base.Property);
            var refObject = (base.Property.Tree.WeakTargets[0] as MonoBehaviour).gameObject;
            var requiredTypes = refAttribute.RequiredTypes;
            var typesLength = requiredTypes.Length;
            var myMessage = refAttribute.Message;

            // Check if Types exist
            var isMissingType = false;
            typeNames.Clear();
            for (int i = 0; i < typesLength; i++)
            {
                if (refObject.GetComponent(requiredTypes[i]) == null)
                {
                    typeNames.Add(requiredTypes[i].Name);
                    isMissingType = true;
                }
            }

            // Build string
            if (isMissingType)
            {
                myMessage.Clear();
                myMessage = (typesLength == 1) ? new StringBuilder("Requires component: ") : new StringBuilder("Requires Components: ");

                var stringLength = typeNames.Count;
                for (int i = 0; i < stringLength - 1; i++)
                {
                    myMessage.AppendFormat("{0}  |   ", typeNames[i]);
                }
                myMessage.AppendFormat("{0}.", typeNames[stringLength - 1]);
                
                InspectorPropertyInfo item = InspectorPropertyInfo.CreateForMember(RequireComponentWarningProcessor.InjectedMemberInfo, false, SerializationBackend.None, new Attribute[]
                {
                    new OnInspectorGUIAttribute(),
                    new InfoBoxAttribute(myMessage.ToString(), InfoMessageType.Error, null),
                    new PropertyOrderAttribute(-100000)
                });
                memberInfos.Insert(0, item);
            }
        }
    }
}
#endif