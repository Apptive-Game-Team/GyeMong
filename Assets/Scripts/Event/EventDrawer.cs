using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Event), true)]
public class EventDrawer : PropertyDrawer
{
    //Drop Down names
    private static readonly string[] classNames = { "Event", "WarpPortalEvent" };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
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
        if (property.managedReferenceValue is WarpPortalEvent) return 1;
        if (property.managedReferenceValue is Event) return 0;
        
        return -1; //default value
    }

    // set property to instance of selected class
    private void CreateSubclassInstance(SerializedProperty property, int index)
    {
        switch (index)
        {
            case 0:
                property.managedReferenceValue = new Event();
                break;
            case 1:
                property.managedReferenceValue = new WarpPortalEvent();
                break;
        }
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
