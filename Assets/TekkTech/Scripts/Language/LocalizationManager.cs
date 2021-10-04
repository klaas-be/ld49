#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.TekkTech.Scripts.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.TekkTech.Scripts.Language
{
    [ExecuteInEditMode]
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager instance;

        //private Languages defaultLanguage = Languages.English;
        [HideInInspector]
        public Languages currentLanguage;
        public static Languages newLanguage;
        
        public bool setLanguageOnStart = true;

        public static readonly string DirectoryPath = Path.DirectorySeparatorChar + "TekkTech" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "LanguageFiles" + Path.DirectorySeparatorChar;
        public static readonly string TagEnumPath = Path.DirectorySeparatorChar + "TekkTech" + Path.DirectorySeparatorChar + "Scripts" + Path.DirectorySeparatorChar + "Language" + Path.DirectorySeparatorChar;

        private static LanguageFile s_loadedLanguageFile;

        private static List<string> fileKeys = new List<string>();

        private static List<LocalizedString> s_registeredLanguageStrings = new List<LocalizedString>();


        private const string KEY_CURRENT_LANGUAGE = "key_current_language";

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }

        private void Start()
        {
            if (setLanguageOnStart)
            {
                ChangeLanguage(currentLanguage);
            }
        }

        public static void ChangeLanguage(Languages to, bool showOnlyTags = false)
        {
            if (instance == null) return;
            LoadLanguageFromFile(to, showOnlyTags);
        }

        public void RegisterLanguageString(LocalizedString ls)
        {
            if (!s_registeredLanguageStrings.Contains(ls))
            {
                s_registeredLanguageStrings.Add(ls);
                ls.LanguageSwitch();
            }
        }
        public void UnregisterLanguageString(LocalizedString ls)
        {
            if (!s_registeredLanguageStrings.Contains(ls))
            {
                s_registeredLanguageStrings.Remove(ls);
            }
        }

        private static void LoadLanguageFromFile(Languages languageToLoad, bool showOnlyTags = false)
        {
            instance.currentLanguage = languageToLoad;
            newLanguage = languageToLoad;
            string filePath = GetFilePathForLanguage(languageToLoad);

            s_loadedLanguageFile = GetLanguageDataFromFile(filePath);
            if (showOnlyTags) 
                s_loadedLanguageFile = new LanguageFile(s_loadedLanguageFile);

            if (s_loadedLanguageFile == null) return;


            foreach (LocalizedString localizedString in s_registeredLanguageStrings)
            {
                localizedString.LanguageSwitch();
            }
        }

        public static LanguageFile GetLanguageDataFromFile(Languages languageToLoad)
        {
            string filePath = GetFilePathForLanguage(languageToLoad);
            return GetLanguageDataFromFile(filePath);
        }

        public static LanguageFile GetLanguageDataFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("TekkTech: Language File " + filePath + " does not exist.");
                return null;
            }

            LanguageFile tempLanguageFile = JsonUtility.FromJson<LanguageFile>(File.ReadAllText(filePath));
            return tempLanguageFile;
        }

        public static bool LanguageFileExists(Languages languages)
        {
            string filePath = GetFilePathForLanguage(languages);
            return File.Exists(filePath);
        }

        public static string GetTextForTag(LanguageTags tag)
        {
            if (s_loadedLanguageFile is null)
                return "";

            string returnText = s_loadedLanguageFile.GetEntryText(tag.ToString());
            if (string.IsNullOrEmpty(returnText))
                return "[" + tag.ToString() + "]";
            return returnText;
        }

        public static void GenerateKeysFromEnum()
        {
            fileKeys.Clear();
            Enum.GetValues(typeof(LanguageTags)).Cast<LanguageTags>().ToList().ForEach(tag =>
            {
                fileKeys.Add(tag.ToString());
            });
        }

        public static void WriteNewKeysToLanguageFile(Languages language, List<string> keys, List<string> content = null)
        {
            string filePath = GetFilePathForLanguage(language);
            LanguageFile languagesFile = new LanguageFile();

            if (File.Exists(filePath))
            {
                languagesFile = JsonUtility.FromJson<LanguageFile>(File.ReadAllText(filePath));
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (content != null)
                    languagesFile.SetEntry(keys[i], new LanguageFileEntry(keys[i], content[i]));
                else
                    languagesFile.SetEntry(keys[i]);
            }

            File.WriteAllText(filePath, languagesFile.GetJsonFormat());
        }

#if UNITY_EDITOR
        public static void RemoveTagsFromLanguageFile(Languages language, List<string> tagsToRemove)
        {
            if (tagsToRemove.Count == 0) return;

            string filePath = GetFilePathForLanguage(language);

            LanguageFile languagesFile = new LanguageFile();

            if (File.Exists(filePath))
            {
                languagesFile = JsonUtility.FromJson<LanguageFile>(File.ReadAllText(filePath));
            }

            tagsToRemove.ForEach(tag => { languagesFile.RemoveEntry(tag); });

            WriteNewLanguageFile(language, languagesFile);
        }

        public static LanguageFile WriteNewLanguageFile(Languages language, LanguageFile file)
        {

            string filePath = GetFilePathForLanguage(language);

            file.fileLanguage = language;

            Directory.CreateDirectory(Application.dataPath + DirectoryPath);
            File.WriteAllText(filePath, file.GetJsonFormat());
            AssetDatabase.Refresh();
            return file;
        }
#endif

        public static void RemoveLanguageFile(Languages language)
        {
            string filePath = GetFilePathForLanguage(language);

            string metaPath = filePath + ".meta";

            File.Delete(filePath);
            File.Delete(metaPath);
        }

        private static string GetFilePathForLanguage(Languages language)
        {
#if UNITY_EDITOR
            return Application.dataPath + DirectoryPath + language.ToString() + ".lang";
#endif
#if UNITY_STANDALONE
            return System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Assets" + DirectoryPath + language.ToString() + ".lang";
#endif
        }
    }
}

