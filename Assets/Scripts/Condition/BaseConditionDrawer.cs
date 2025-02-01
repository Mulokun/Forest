using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Forest.ConditionSystem.Editor
{

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(BaseCondition))]
    public class BaseConditionDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Using BeginProperty / EndProperty on the parent property means that
            //prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            int initialIndent = EditorGUI.indentLevel;
            Rect indentedRect = EditorGUI.IndentedRect(position);
            Type propertyType = property.managedReferenceValue?.GetType();
            string propertyTypeName = propertyType == null ? "None" : propertyType.Name;

            //Dropdown menu to show the different condition classes
            Rect dropdownRect = new Rect(indentedRect.x + 16, indentedRect.y, 200, EditorGUIUtility.singleLineHeight);
            if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(propertyTypeName), FocusType.Keyboard))
            {
                CreateDropdownMenuOfAllInheritedClasses(property, typeof(BaseCondition), propertyTypeName);
            }
            //To keep the indent consistent throughout the items drawn
            EditorGUI.indentLevel = 0;

            //No need to draw the colored rect if the condition is not expanded
            if (propertyType != null && property.isExpanded)
            {
                //Colored vertical rectangle to show the continuity/scope of AND/OR conditions
                Rect coloredRect = new Rect(indentedRect.x + 5, indentedRect.y + 20, 3, indentedRect.height - 30);
                EditorGUI.DrawRect(coloredRect, GetColorBasedOnConditionType(propertyType));
            }

            //Line separator between each condition (even if not expanded as it helps to determine when a condition begins/ends)
            Rect rectToUse = new Rect(indentedRect.x + 15, indentedRect.y + indentedRect.height - 8, indentedRect.width - 30, 0.5f);
            EditorGUI.DrawRect(rectToUse, new Color(85, 85, 85, 0.3f));

            if (property.managedReferenceValue != null)
            {
                EditorGUI.PropertyField(indentedRect, property, GUIContent.none, true);
            }

            EditorGUI.indentLevel = initialIndent;

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }


        /// <param name="property"></param>
        /// <param name="baseType">The baseType that all classes in the DropdownMenu should inherite from.</param>
        /// <param name="baseTypeName"></param>
        private void CreateDropdownMenuOfAllInheritedClasses(SerializedProperty property, Type baseType, string baseTypeName)
        {
            GenericMenu dropdownMenu = new GenericMenu();

            //Creates the None item that corresponds to having no classes selected.
            dropdownMenu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () =>
            {
                property.managedReferenceValue = null;
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            });

            Type solverType = GetConditionSolverTypeGivenProperty(property);

            //Will display all of the inherited types in the dropdown list and check the one that's already selected (if there is one)
            List<Type> allBaseConditionsTypes = GetInheritedClasses(baseType);
            foreach (Type currentBaseConditionType in allBaseConditionsTypes)
            {
                //If the Solver has all the attributes needed for the CurrentBaseCondition to be solved, show that BaseCondition
                //Otherwise, hide it.
                if (CheckIfSolverHasAtLeastOneConditionAttribute(currentBaseConditionType, solverType))
                {
                    dropdownMenu.AddItem(new GUIContent(currentBaseConditionType.Name), baseTypeName.Equals(currentBaseConditionType.Name), () =>
                    {
                        property.managedReferenceValue = currentBaseConditionType.GetConstructor(Type.EmptyTypes).Invoke(null);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    });
                }
            }
            dropdownMenu.ShowAsContext();
        }

        private bool CheckIfSolverHasAtLeastOneConditionAttribute(Type conditionType, Type solverType)
        {
            RequiredSolvingDataAttribute[] conditionAttributes = (RequiredSolvingDataAttribute[])conditionType.GetCustomAttributes<RequiredSolvingDataAttribute>();

            //if the condition doesn't need any contexts (aka AND/OR), then it will be visible in every Condition
            //(hence the cast into an array to be able to retrieve the length)
            if (conditionAttributes.Length > 0)
            {
                return conditionAttributes[0].IsCompatible(solverType);
            }

            return true;
        }

        /// <summary>
        /// Will go through all the fields of the property path to find the generic one (aka the Condition). And from there,
        /// will retrieve the Generic argument (aka the IConditionSolver).
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private Type GetConditionSolverTypeGivenProperty(SerializedProperty property)
        {
            string[] fieldNames = property.propertyPath.Split('.');
            Type currentType = property.serializedObject.targetObject.GetType();
            FieldInfo currentFieldInfo;
            for (int i = 0; i < fieldNames.Length; i++)
            {
                currentFieldInfo = currentType.GetField(fieldNames[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (currentFieldInfo == null)
                {
                    continue;
                }

                //If it's a list (which is a generic type), we retrieve it's generic argument to get the type of element
                if (currentFieldInfo.FieldType.IsGenericType && currentFieldInfo.FieldType.GetGenericTypeDefinition().Equals(typeof(List<>)))
                {
                    currentType = currentFieldInfo.FieldType.GenericTypeArguments[0];
                }
                else
                {
                    currentType = currentFieldInfo.FieldType.IsArray ? currentFieldInfo.FieldType.GetElementType() : currentFieldInfo.FieldType;
                }

                //If it's a Condition<>, we return the GenericTypeArgument as we know it'll be the IConditionSolver
                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition().Equals(typeof(Condition<>)))
                {
                    return currentType.GenericTypeArguments[0];
                }
            }
            Debug.LogError($"Couldn't find an IConditionSolver for the given property. Make sure Conditions are built with a correct solver. | TargetObject: {property.serializedObject.targetObject}");
            return null;
        }

        private Color GetColorBasedOnConditionType(Type conditionType)
        {
            Color colorMultiplier = new Color(0.6f, 0.6f, 0.6f, 1);
            Color colorToReturn = Color.gray;
            if (typeof(OrCondition).Equals(conditionType))
            {
                colorToReturn = Color.yellow * colorMultiplier;
            }
            else if (typeof(AndCondition).Equals(conditionType))
            {
                colorToReturn = Color.green * colorMultiplier;
            }

            return colorToReturn;
        }

        /// <param name="baseType"></param>
        /// <returns>All the types of classes inheriting from the given baseType.</returns>
        private List<Type> GetInheritedClasses(Type baseType, bool canReturnAbstractClasses = false)
        {
            List<Type> listOfCorrectClasses = new List<Type>();

            Type baseTypeToCompare = baseType.IsArray ? baseType.GetElementType() : baseType;

            //We'll go through all assemblies to get all classes inheriting from the baseType so that the User can freely use Assemblies without
            //being required to change the plugin to make it work
            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly currentAssembly in allAssemblies)
            {
                Type[] allTypes = currentAssembly.GetTypes();
                for (int i = 0; i < allTypes.Length; i++)
                {
                    if (allTypes[i].IsClass && (canReturnAbstractClasses || !allTypes[i].IsAbstract) && baseTypeToCompare.IsAssignableFrom(allTypes[i]))
                    {
                        listOfCorrectClasses.Add(allTypes[i]);
                    }
                }
            }
            return listOfCorrectClasses;
        }
    }

#endif
}
