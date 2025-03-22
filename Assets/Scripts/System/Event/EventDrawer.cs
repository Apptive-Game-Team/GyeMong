#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace System.Event
{
    [CustomPropertyDrawer(typeof(Event.Event), true)]
    public class EventDrawer : PropertyDrawer
    {
        //Class Infos   
        private static Dictionary<string, Type> classInfos = new Dictionary<string, Type>();

        //Drop Down names
        private static string[] classNames = null;

        //Initializer
        private void InitializeClassInfos()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Event.Event));
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (!type.IsAbstract && type.IsSubclassOf(typeof(Event.Event))){
                    classInfos.Add(type.Name, type);
                }
            }

            classNames = new string[classInfos.Count];

            int index = 0;
            foreach (Type type in classInfos.Values)
            {
                Type baseType = type.BaseType;
                String className = type.Name;
                while (baseType != typeof(Event.Event))
                {
                    className = baseType.Name + "/" + className;
                    baseType = baseType.BaseType;
                }

                classNames[index] = className;
                index++;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            if (classNames == null)
            {
                InitializeClassInfos();
            }

            //initial gui setting
            EditorGUI.BeginProperty(position, label, property);

            // draw default label
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);

            // draw drop down menu
            Rect dropdownRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
            int selectedIndex = GetClassIndex(property);
            int newIndex = EditorGUI.Popup(dropdownRect, "Select Subclass", selectedIndex, classNames);


            // if selected class is change
            if (newIndex != selectedIndex)
            {
                CreateSubclassInstance(property, newIndex);
            }

            // draw selected class
            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, position.height), property, true);

            //end gui setting
            EditorGUI.EndProperty();
        }

        // get current class index
        private int GetClassIndex(SerializedProperty property)
        {
            if (property.managedReferenceValue == null) return -1;

            string targetName = property.managedReferenceValue.GetType().ToString();
            int index = 0;
            foreach (string classPath in classNames)
            {
                string name = classPath.Split('/')[^1];
                if (name.Equals(targetName))
                {
                    return index;
                }
                index++;
            }
        
            return -1; //default value
        }

        // set property to instance of selected class
        private void CreateSubclassInstance(SerializedProperty property, int index)
        {
            property.managedReferenceValue = Activator.CreateInstance(classInfos[classNames[index].Split('/')[^1]]);

            property.serializedObject.ApplyModifiedProperties();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2; // default label height

            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();

            iterator.NextVisible(true); // skip parent property

            while (!SerializedProperty.EqualContents(iterator, endProperty))
            {
                totalHeight += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                iterator.NextVisible(false); // move to next sibling property
            }

            return totalHeight;
        }
    }
}
#endif
