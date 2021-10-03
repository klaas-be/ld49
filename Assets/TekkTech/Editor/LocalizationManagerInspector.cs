#if UNITY_EDITOR
using System.IO;
using Assets.TekkTech.Scripts.Language;
using UnityEditor;
using UnityEngine;

namespace Assets.TekkTech.Editor
{
    [CustomEditor(typeof(LocalizationManager))]
    public class LocalizationManagerInspector : UnityEditor.Editor
    {
        private void OnEnable()
        {
            LocalizationManager.GenerateKeysFromEnum();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Languages currentLanguage = (Languages)serializedObject.FindProperty("currentLanguage").enumValueIndex;

            LocalizationManager.newLanguage = (Languages)EditorGUILayout.EnumPopup("Set Language to", LocalizationManager.newLanguage);
            if (LocalizationManager.newLanguage != currentLanguage)
            {
                LocalizationManager.ChangeLanguage(LocalizationManager.newLanguage);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Show LanguageTagControl Window"))
            {
                LanguageTagsControl.ShowWindow();
            }
        }
    }
}
#endif