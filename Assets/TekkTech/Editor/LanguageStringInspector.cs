#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Assets.TekkTech.Scripts.Utility;
using UnityEngine;
using UnityEditor;

namespace Assets.TekkTech.Editor
{
    [CustomPropertyDrawer(typeof(LocalizedString))]
    public class LanguageStringInspector : PropertyDrawer
    {
        private List<string> categories = new List<string>();
        private List<string> tags = new List<string>();
        [SerializeField]
        private int catIndex = -1, tagsIndex = -1;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Enum.GetNames(typeof(LanguageTags)).Length > 1)
            {
                LanguageTags propertyTag = (LanguageTags)property.FindPropertyRelative("languageTag").intValue;

                Rect labelRect = position;
                labelRect.height = 15f;
                EditorGUI.LabelField(labelRect, "Localized String Tag (Category, Tag)");

                //Alle Categories finden
                categories.Clear();
                foreach (string s in Enum.GetNames(typeof(LanguageTags)))
                {
                    if (s != LanguageTags.None.ToString())
                    {
                        if (!categories.Contains(s.Split('_')[0]))
                            categories.Add(s.Split('_')[0]);
                    }
                    else
                    {
                        categories.Add("None");
                    }
                }
                if (propertyTag != LanguageTags.None && Enum.IsDefined(typeof(LanguageTags), propertyTag))
                    catIndex = categories.IndexOf(propertyTag.ToString().Split('_')[0]);

                if (catIndex >= categories.Count || catIndex < 0)
                    catIndex = 0;

                Rect categoryRect = position;
                categoryRect.x += 10f;
                categoryRect.y += 16f;
                categoryRect.width = position.width / 2 - 10 + 5;
                int lastCatIndex = catIndex;
                catIndex = EditorGUI.Popup(categoryRect, catIndex, categories.ToArray());
                if(lastCatIndex != catIndex)
                    tagsIndex = 0;

                //Alle tags zur ausgewählten Category finden
                tags.Clear();
                foreach (string s in Enum.GetNames(typeof(LanguageTags)))
                {
                    if (s != LanguageTags.None.ToString())
                    {
                        //Nur tags zeigen, die die selbe category haben
                        if (s.Split('_')[0] == categories[catIndex])
                        {
                            if (!tags.Contains(s.Split('_')[1]))
                                tags.Add(s.Split('_')[1]);
                        }
                    }
                    else
                    {
                        tags.Add("None");
                    }
                }

                if (propertyTag != LanguageTags.None && Enum.IsDefined(typeof(LanguageTags), propertyTag))
                    tagsIndex = tags.IndexOf(propertyTag.ToString().Split('_')[1]);

                if (tagsIndex >= tags.Count || tagsIndex < 0)
                    tagsIndex = 0;

                Rect tagsRect = position;
                tagsRect.x += position.width / 2 + 5;
                tagsRect.y += 16f;
                tagsRect.width = position.width / 2 - 5;
                tagsIndex = EditorGUI.Popup(tagsRect, tagsIndex, tags.ToArray());


                //Wenn gesetzt, die property ändern
                if (Enum.IsDefined(typeof(LanguageTags), (categories[catIndex] + "_" + tags[tagsIndex])))
                { 
                    property.FindPropertyRelative("languageTag").intValue = (int)Enum.Parse(typeof(LanguageTags), (categories[catIndex] + "_" + tags[tagsIndex]));
                }
                else
                {
                    property.FindPropertyRelative("languageTag").intValue = (int)LanguageTags.None;
                }
            }
            else
            {
                EditorGUI.LabelField(position, "No tags available.");
                property.FindPropertyRelative("languageTag").intValue = (int)LanguageTags.None;
            }               
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 32f;
        }
    }
}
#endif