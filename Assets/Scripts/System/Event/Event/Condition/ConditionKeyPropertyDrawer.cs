#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace System.Event.Event.Condition
{
    [CustomPropertyDrawer(typeof(ConditionKey))]
    public class ConditionKeyPropertyDrawer : PropertyDrawer
    {
        private int _selected = -1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect textFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            string currentValue = property.stringValue;

            EditorGUI.BeginChangeCheck();
            string newValue = EditorGUI.TextField(textFieldRect, label, currentValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = newValue;
            }

            // recommend filtering
            if (!string.IsNullOrEmpty(newValue))
            {
                var filtered = ConditionKeyRepository.ConditionKeys
                    .Where(s => s.ToLower().Contains(newValue.ToLower()) && s != newValue)
                    .Take(5) // 최대 5개
                    .ToList();

                if (filtered.Count > 0)
                {
                    float y = textFieldRect.yMax + 2;
                    for (int i = 0; i < filtered.Count; i++)
                    {
                        var suggestion = filtered[i];
                        Rect optionRect = new Rect(position.x + 15, y, position.width - 15, EditorGUIUtility.singleLineHeight);
                        if (GUI.Button(optionRect, suggestion, EditorStyles.miniButton))
                        {
                            property.stringValue = suggestion;
                            GUI.FocusControl(null);
                            break;
                        }
                        y += EditorGUIUtility.singleLineHeight + 2;
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (string.IsNullOrEmpty(property.stringValue)) return base.GetPropertyHeight(property, label);
            int count = ConditionKeyRepository.ConditionKeys.Count(s => s.ToLower().Contains(property.stringValue.ToLower()) && s != property.stringValue);
            count = Mathf.Min(count, 5);
            return base.GetPropertyHeight(property, label) + (EditorGUIUtility.singleLineHeight + 2) * count;
        }
    }
}
#endif