using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

[CustomPropertyDrawer(typeof(Event), true)]
public class EventDrawer : PropertyDrawer
{
    //Class Infos   
    private static Dictionary<string, Type> classInfos = new Dictionary<string, Type>();

    //Drop Down names
    private static string[] classNames = null;

    //Initializer
    private void InitializeClassInfos()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(Event));
        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
            if (!type.IsAbstract && type.IsSubclassOf(typeof(Event))){
                classInfos.Add(type.Name, type);
            }
        }

        classNames = new string[classInfos.Count];

        int index = 0;
        foreach (string tuple in classInfos.Keys)
        {
            classNames[index] = tuple;
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
        foreach (string name in classNames)
        {
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
        property.managedReferenceValue = Activator.CreateInstance(classInfos[classNames[index]]);

        property.serializedObject.ApplyModifiedProperties();
    }

    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // default height
        float height = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;

        SerializedProperty iterator = property.Copy();

        // add child properties heights
        while (iterator.NextVisible(true))
        {
            if (iterator.propertyPath.StartsWith(property.propertyPath) && iterator.propertyPath != property.propertyPath)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                break;
            }
        } 


        return height;
    }
}
