#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.TekkTech.Scripts.Language;
using Assets.TekkTech.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Assets.TekkTech.Editor
{
    public class LanguageTagsControl : EditorWindow
    {
        //ICON Drawing
        private const string ICON_PATH = "Assets/TekkTech/Resources/Icon/TekkTech.png";
        private const string ICON_PATH_SMALL = "Assets/TekkTech/Resources/Icon/TekkTech_small.png";
        public Texture iconBig;

        [MenuItem("Tools/TekkTech/Language Tag Control")]
        public static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(LanguageTagsControl));
            window.minSize = new Vector2(600, 425);

            Texture smallIcon = AssetDatabase.LoadAssetAtPath<Texture>(ICON_PATH_SMALL);
            window.titleContent = new GUIContent("Language Tag Control", smallIcon);

            s_checkedForEnglishFile = false;
        }

        //Toolbar
        private int tab;

        //Dirty Flags
        private bool isDirtyTags;
        private bool isDirtyEng;
        private bool isDirtySecond;

        //First check for eng file
        private static bool s_checkedForEnglishFile;

        private Languages m_loadedLanguage;
        private LanguageFile m_englishFile = new LanguageFile();
        private LanguageFile m_secondLanguageFile = new LanguageFile();

        private Dictionary<string, List<string>> m_categorysAndTagsList =
            new Dictionary<string, List<string>>();

        private List<string> m_newTagList = new List<string>();
        private List<string> m_deleteTagList = new List<string>();

        //Add / Delete Tags
        private int m_selectedIndexCategoryForTags;
        private int m_selectedIndexCategoryForNewTag;
        private bool m_newTagMode;
        private bool m_newCategoryMode;
        private string m_newTagCategory = "";
        private string m_newTag = "";
        private bool canAddNewTag = true;

        private string languageRemoved = "";

        private const string LANGUAGES_RESOURCE_PATH = "/TekkTech/Resources/LanguageFiles/";
        private const string LANGUAGES_ENUM_PATH = "/TekkTech/Scripts/Language/";

        private const int TAG_HEADER_TAG_WIDTH = 400;

        //FeedbackHelpboxen
        private double showAddFeedbackStartTime = -1d;
        private const float MAX_SHOW_FEEDBACK_TIME = 4f;
        private bool showAddFeedback;
        private bool showImportFeedback;
        private bool showRemoveFeedback;

        //Add Language

        private bool drawAllLanguages = true;

        //Delete Language
        private Languages m_languageToDelete;
        private bool m_showSureToDelete;

        //Excel Import
        private string[] excelExampleHeaderSplit;
        private string[] excelExampleColumnSplit;
        private string excelColumnCheckLog = "";
        private ExcelState excelState = ExcelState.Nothing;
        private bool importSettingsFoldout = true;
        private List<string> headerPopupList = new List<string>();
        private List<int> headerPopupSelectedIndex = new List<int>();
        private bool csvGen;

        //Settings
        private string excelPath = "";
        private bool excelIgnoreHeaderLine = true;

        //Editor Prefs Keys
        private const string KEY_EXCEL_PATH = "key_excel_path";
        private const string KEY_EXCEL_IGNORE_HEADER = "key_excel_ignore_header";
        private const string KEY_EXCEL_SEPERATOR = "key_excel_seperator";
        private const string KEY_EXCEL_COMBINER = "key_excel_combiner";

        private enum ExcelState
        {
            Nothing,
            PathSelected,
            Analysing,
            ExcelChecked,
            HasReadFirstColumn,
            HasFilledHeaderList,
            IsTagged
        }


        private string m_newLanguageName = "";
        private bool m_canAddNewLanguage;
        private bool m_canRemoveTagsFromDict;

        private void OnEnable()
        {
            iconBig = AssetDatabase.LoadAssetAtPath<Texture>(ICON_PATH);

            //Load Editor Prefs
            if (EditorPrefs.HasKey(KEY_EXCEL_PATH))
                excelPath = EditorPrefs.GetString(KEY_EXCEL_PATH);
            if (EditorPrefs.HasKey(KEY_EXCEL_IGNORE_HEADER))
                excelIgnoreHeaderLine = EditorPrefs.GetBool(KEY_EXCEL_IGNORE_HEADER);
            if (EditorPrefs.HasKey(KEY_EXCEL_SEPERATOR))
                ExcelManager.excelSeperatorChar = char.Parse(EditorPrefs.GetString(KEY_EXCEL_SEPERATOR));
            if (EditorPrefs.HasKey(KEY_EXCEL_COMBINER))
                ExcelManager.excelStringCombinerChar = char.Parse(EditorPrefs.GetString(KEY_EXCEL_COMBINER));

            if (!CheckForLocalizationManager())
            {
                AddLocalizationManager();
            }
        }

        private void AddLocalizationManager()
        {
            GameObject localizationManagerObj = new GameObject("LocalizationManager");
            localizationManagerObj.AddComponent<LocalizationManager>();

            EditorGUIUtility.PingObject(localizationManagerObj);
        }

        private void OnDisable()
        {
            //Set Editor Prefs
            EditorPrefs.SetString(KEY_EXCEL_PATH, excelPath);
            EditorPrefs.SetBool(KEY_EXCEL_IGNORE_HEADER, excelIgnoreHeaderLine);
            EditorPrefs.SetString(KEY_EXCEL_SEPERATOR, ExcelManager.excelSeperatorChar.ToString());
            EditorPrefs.SetString(KEY_EXCEL_COMBINER, ExcelManager.excelStringCombinerChar.ToString());
        }

        private void OnGUI()
        {
            Color temp = GUI.color;
            GUI.color = new Color(1, 1, 1, 0.15f);
            if (GUI.Button(new Rect(position.width - 61, 1, 60, 60), ""))
            {
                Application.OpenURL("https://tekk.one/tech/language-manager/");
            }
            GUI.color = temp;

            if (!CheckForLocalizationManager())
            {
                EditorGUILayout.HelpBox("Localization Manager not present in hierarchy!", MessageType.Error);
                if (GUILayout.Button("Add Gameobject", GUILayout.Width(position.width - 100)))
                    AddLocalizationManager();

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }

            if (iconBig != null)
            {
                GUI.DrawTexture(new Rect(position.width - 60, 0, 60, 60), iconBig);
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal(GUILayout.Width(position.width - 100));
            GUILayout.Space(55);
            tab = GUILayout.Toolbar(tab, new string[] { "Tags & Content", "Languages", "CSV Options" });
            GUILayout.EndHorizontal();

            switch (tab)
            {
                default:
                case 0:
                    EditorStyles.textField.wordWrap = true;
                    EditorStyles.textField.richText = true;

                    LoadOrCreateEnglishFile();
                    DrawLoadLanguageButton();

                    DrawContentHeader();
                    DrawLanguageContent();

                    DrawCreateNewTag();
                    DrawSaveLocalizationButton();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    break;
                case 1:
                    DrawAddNewLanguageButton();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    DeleteLanguage();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    DrawShowAllLanguages();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    break;
                case 2:
                    DrawExcelImportAndGen();
                    DrawExcelAnalyse();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    break;
            }
        }

        private static bool CheckForLocalizationManager()
        {
            return GameObject.FindObjectOfType<LocalizationManager>() != null;
        }

        private void DrawExcelImportAndGen()
        {
            GUILayout.Space(8);
            if (excelPath.Equals(""))
            {
                excelPath = Application.dataPath;
            }
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                richText = true
            };
            EditorGUILayout.LabelField("Language must be created <b>beforehand</b>.", style);
            EditorGUILayout.LabelField("CSV file needs at least 3 columns (Category, Tag, LocalisedText)");
            EditorGUILayout.LabelField("The csv import will create new tags if they aren't found in the default language.");
            EditorGUILayout.LabelField("Also it will <b>override</b> the LanguageFile if it already exists.", style);
            EditorGUILayout.LabelField("");
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Excel path: ", GUILayout.Width(75));
                EditorGUILayout.LabelField("<b>" + excelPath + "</b>", style);
            }
            EditorGUILayout.EndHorizontal();

            if (csvGen && !EditorApplication.isCompiling)
            {
                GenerateCsv();
                csvGen = false;
            }

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Select csv file"))
                {
                    string importPath = EditorUtility.OpenFilePanel("Localization file to import", Path.GetFullPath(excelPath), "csv");
                    if (!string.IsNullOrEmpty(importPath))
                        excelPath = importPath;
                }

                if (GUILayout.Button("Generate", GUILayout.Width(75)))
                {
                    if (isDirtyTags && !csvGen)
                    {
                        if (EditorUtility.DisplayDialog("Save Tags", "Do you want to save the new tags before you generate the CSV file?", "Save", "Ignore"))
                        {
                            SaveFiles();
                            AssetDatabase.Refresh();
                        }
                    }
                    csvGen = true;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            DrawImportSettings();

            EditorGUILayout.BeginHorizontal();
            if ((excelState == ExcelState.Nothing) && (Path.GetExtension(excelPath) == ".csv" && excelPath != Application.dataPath))
                excelState = ExcelState.PathSelected;

            GUI.enabled = excelState >= ExcelState.PathSelected;
            if (GUILayout.Button("Analyse " + Path.GetFileName(excelPath)))
            {
                excelState = ExcelState.Analysing;
            }
            GUI.enabled = true;


            GUI.enabled = excelState == ExcelState.IsTagged;
            if (GUILayout.Button("Import " + Path.GetFileName(excelPath)))
            {
                int categoryIndex = -1, tagIndex = -1;
                for (int i = 0; i < headerPopupSelectedIndex.Count; i++)
                {
                    if (headerPopupSelectedIndex[i] == 1)
                        categoryIndex = i;
                    if (headerPopupSelectedIndex[i] == 2)
                        tagIndex = i;
                    if (categoryIndex > 0 && tagIndex > 0)
                        break;
                }
                for (int i = 0; i < headerPopupSelectedIndex.Count; i++)
                {
                    if (headerPopupSelectedIndex[i] > 2)
                    {
                        ImportExcelFileAndCreateTags((Languages)(headerPopupSelectedIndex[i] - 3), categoryIndex, tagIndex, i);
                        showImportFeedback = true;
                        showAddFeedbackStartTime = EditorApplication.timeSinceStartup;
                    }
                }

                m_englishFile = LocalizationManager.GetLanguageDataFromFile(Languages.English);

                SaveFiles();
                excelState = ExcelState.Nothing;
                excelPath = Path.GetDirectoryName(excelPath);
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            if (showImportFeedback)
            {
                for (int i = 0; i < headerPopupSelectedIndex.Count; i++)
                {
                    if (headerPopupSelectedIndex[i] > 2)
                    {
                        EditorGUILayout.HelpBox("Imported content into " + ((Languages)headerPopupSelectedIndex[i] - 3).ToString(), MessageType.Info);
                    }
                }
                
                if (showAddFeedbackStartTime + MAX_SHOW_FEEDBACK_TIME <= EditorApplication.timeSinceStartup)
                {
                    showImportFeedback = false;
                }
            }
        }

        private void GenerateCsv()
        {
            string genCsVpath = EditorUtility.SaveFilePanel("Generate CSV File", Path.GetDirectoryName(excelPath), Application.productName + "-Localization", "csv");
            if (!string.IsNullOrEmpty(genCsVpath))
            {
                excelPath = genCsVpath;
                ExcelManager.GenerateCsvFile(excelPath);
            }
        }

        private void DrawImportSettings()
        {
            EditorGUI.indentLevel++;
            importSettingsFoldout = EditorGUILayout.Foldout(importSettingsFoldout, "Advanced settings");

            if (importSettingsFoldout)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(40);
                    GUILayout.Label("Ignore first row (Header)", GUILayout.Width(200));
                    bool lastIgnore = excelIgnoreHeaderLine;
                    excelIgnoreHeaderLine = GUILayout.Toggle(excelIgnoreHeaderLine, "", GUILayout.Width(15));
                    if (lastIgnore != excelIgnoreHeaderLine)
                        excelState = ExcelState.Nothing;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(3);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(40);
                    GUILayout.Label("Seperator char", GUILayout.Width(200));
                    char lastChar = ExcelManager.excelSeperatorChar;
                    ExcelManager.excelSeperatorChar = char.Parse(GUILayout.TextField(ExcelManager.excelSeperatorChar.ToString(), 1, GUILayout.Width(15)));
                    if (lastChar != ExcelManager.excelSeperatorChar)
                        excelState = ExcelState.Nothing;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(40);
                    GUILayout.Label("String combiner char", GUILayout.Width(200));
                    char lastChar = ExcelManager.excelStringCombinerChar;
                    ExcelManager.excelStringCombinerChar = char.Parse(GUILayout.TextField(ExcelManager.excelStringCombinerChar.ToString(), 1, GUILayout.Width(15)));
                    if (lastChar != ExcelManager.excelStringCombinerChar)
                        excelState = ExcelState.Nothing;
                }
                GUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        private void DrawExcelAnalyse()
        {
            if (excelState >= ExcelState.Analysing)
            {
                if (excelState == ExcelState.Analysing)
                {
                    excelColumnCheckLog = ExcelManager.CheckExcel(excelPath);
                    excelState = ExcelState.ExcelChecked;
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                if (excelColumnCheckLog != "")
                {
                    EditorGUILayout.LabelField("  ERROR: " + excelColumnCheckLog);
                }
                else if (excelState == ExcelState.ExcelChecked)
                {
                    excelExampleHeaderSplit = ExcelManager.CsvSplit(ExcelManager.ReadOneLine(excelPath));
                    excelExampleColumnSplit = ExcelManager.CsvSplit(ExcelManager.ReadOneLine(excelPath, excelIgnoreHeaderLine));
                    excelState = ExcelState.HasReadFirstColumn;
                }

                if (excelState >= ExcelState.HasReadFirstColumn)
                {
                    GUILayout.Label("Please define the header - Example column");
                    GUILayout.Space(5);

                    //Header popups generieren
                    int len = excelExampleColumnSplit.Length;
                    if (excelState == ExcelState.HasReadFirstColumn)
                    {
                        headerPopupSelectedIndex.Clear();
                        for (int i = 0; i < len; i++)
                        {
                            if (i == 0)
                                headerPopupSelectedIndex.Add(1);
                            else if (i == 1)
                                headerPopupSelectedIndex.Add(2);
                            else
                                headerPopupSelectedIndex.Add(0);
                        }

                        headerPopupList.Clear();
                        headerPopupList.Add("None");
                        headerPopupList.Add("Category");
                        headerPopupList.Add("Tag");
                        foreach (string language in Enum.GetNames(typeof(Languages)))
                        {
                            headerPopupList.Add(language);
                        }

                        //Header Tags vorbelegen, wenn category, tag oder eine vergebene Sprache gefunden wird
                        for (int i = 0; i < excelExampleHeaderSplit.Length; i++)
                        {
                            if (excelExampleHeaderSplit[i].Equals("Category", StringComparison.CurrentCultureIgnoreCase)
                                || excelExampleHeaderSplit[i].Equals("Categories", StringComparison.CurrentCultureIgnoreCase))
                            {
                                headerPopupSelectedIndex[i] = 1;
                            }
                            if (excelExampleHeaderSplit[i].Equals("Tag", StringComparison.CurrentCultureIgnoreCase)
                                || excelExampleHeaderSplit[i].Equals("Tags", StringComparison.CurrentCultureIgnoreCase))
                            {
                                headerPopupSelectedIndex[i] = 2;
                            }
                            if (Enum.IsDefined(typeof(Languages), excelExampleHeaderSplit[i]))
                            {
                                headerPopupSelectedIndex[i] = (int)Enum.Parse(typeof(Languages), excelExampleHeaderSplit[i]) + 3;
                            }
                        }

                        excelState = ExcelState.HasFilledHeaderList;
                    }

                    //Draw Header wenn gewünscht
                    if (excelIgnoreHeaderLine)
                    {
                        GUILayout.BeginHorizontal();
                        for (int i = 0; i < len; i++)
                        {
                            GUILayout.Space(10);
                            if (headerPopupSelectedIndex[i] == 0)
                                GUI.enabled = false;

                            EditorGUILayout.LabelField(excelExampleHeaderSplit[i], GUILayout.Width(position.width / len - 14));
                            GUI.enabled = true;
                        }
                        GUILayout.EndHorizontal();
                    }

                    //Draw Popups für jede Spalte
                    GUILayout.BeginHorizontal();
                    for (int i = 0; i < len; i++)
                    {
                        headerPopupSelectedIndex[i] = EditorGUILayout.Popup(headerPopupSelectedIndex[i], headerPopupList.ToArray(), GUILayout.Width(position.width / len - 4));
                    }
                    GUILayout.EndHorizontal();

                    //Draw Example Zeile
                    GUILayout.BeginHorizontal();
                    for (int i = 0; i < len; i++)
                    {
                        GUILayout.Space(10);
                        if (headerPopupSelectedIndex[i] == 0)
                            GUI.enabled = false;

                        EditorGUILayout.LabelField(excelExampleColumnSplit[i], GUILayout.Width(position.width / len - 14));
                        GUI.enabled = true;
                    }
                    GUILayout.EndHorizontal();

                    if (CheckExcelHeaderTags())
                    {
                        if (excelState != ExcelState.IsTagged)
                        {
                            excelState = ExcelState.IsTagged;
                        }
                    }
                    else
                    {
                        excelState = ExcelState.HasFilledHeaderList;
                    }
                }
            }
        }

        private bool CheckExcelHeaderTags()
        {
            bool output = true;
            bool hasCategory = false;
            bool hasTag = false;
            bool hasContent = false;
            for (int i = 0; i < headerPopupSelectedIndex.Count; i++)
            {
                if (headerPopupSelectedIndex[i] == 0)
                    continue;

                for (int j = 0; j < headerPopupSelectedIndex.Count; j++)
                {
                    if (i != j && headerPopupSelectedIndex[j] != 0)
                    {
                        output &= headerPopupSelectedIndex[i] != headerPopupSelectedIndex[j];
                        if (!output)
                            return false;
                    }
                }
                if (headerPopupSelectedIndex[i] == 1)
                    hasCategory = true;
                if (headerPopupSelectedIndex[i] == 2)
                    hasTag = true;
                if (headerPopupSelectedIndex[i] > 2)
                    hasContent = true;
            }
            //Wenn hier erreicht wird, sind die header an sich okay - check ob header vergeben sind
            return hasCategory && hasTag && hasContent;
        }

        private int ImportExcelFileAndCreateTags(Languages excelSelectedLanguage, int categoryIndex, int tagNameIndex, int contentIndex)
        {
            LanguageFile newFile = new LanguageFile(m_englishFile, excelSelectedLanguage);
            List<LanguageFileEntry> excelEntries = ExcelManager.ImportExcelFileEntries(excelPath, categoryIndex, tagNameIndex, contentIndex);
            if (excelIgnoreHeaderLine)
            {
                excelEntries.RemoveAt(0);
            }

            //neue keys hinzufügen
            foreach (LanguageFileEntry entry in excelEntries)
            {
                if (!m_newTagList.Contains(entry.textTag))
                    m_newTagList.Add(entry.textTag);

                //Im EnglishFile nur Key hinzufügen, wenn noch nicht da
                m_englishFile.SetEntry(entry.textTag);
                newFile.SetEntry(entry.textTag, entry);
            }

            //neues file schreiben
            LocalizationManager.WriteNewLanguageFile(excelSelectedLanguage, newFile);

            //neues File reloaden, wenn ausgewählt
            if (m_loadedLanguage == excelSelectedLanguage)
            {
                m_secondLanguageFile = LocalizationManager.GetLanguageDataFromFile(excelSelectedLanguage);
            }

            return excelEntries.Count;
        }

        private void LoadOrCreateEnglishFile()
        {
            GUILayout.Space(8);
            if (s_checkedForEnglishFile) return;

            m_englishFile = LoadOrCreateLanguageFile(Languages.English);
            FindAllCategories();
            m_newTagList.Clear();
            s_checkedForEnglishFile = true;
        }

        private void DrawLoadLanguageButton()
        {
            if (Enum.GetNames(typeof(Languages)).Length > 1)
            {
                EditorGUILayout.LabelField("Additional Language to load:");
                GUI.enabled = !isDirtyEng && !isDirtySecond && !isDirtyTags;
                EditorGUILayout.BeginHorizontal();
                {
                    foreach (string language in Enum.GetNames(typeof(Languages)))
                    {
                        if (language == Languages.English.ToString()) continue;

                        if (GUILayout.Button(language))
                        {
                            m_secondLanguageFile =
                                LoadOrCreateLanguageFile((Languages)Enum.Parse(typeof(Languages), language));
                            m_loadedLanguage = m_secondLanguageFile.fileLanguage;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }

        private void DrawContentHeader()
        {
            EditorGUILayout.BeginHorizontal();
            {
                float contentTextSize = Mathf.Clamp(((position.width - TAG_HEADER_TAG_WIDTH) / 2), 100f, 9999f);
                EditorGUILayout.LabelField("LanguageTags", GUILayout.Width(TAG_HEADER_TAG_WIDTH));
                EditorGUILayout.LabelField("English" + (isDirtyEng || isDirtyTags ? "*" : ""), GUILayout.MinWidth(contentTextSize));

                if (m_secondLanguageFile.fileLanguage != Languages.English)
                {
                    EditorGUILayout.LabelField(m_secondLanguageFile.fileLanguage.ToString() + (isDirtySecond || isDirtyTags ? "*" : ""),
                        GUILayout.MinWidth(contentTextSize));
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLanguageContent()
        {
            if (m_canRemoveTagsFromDict)
            {
                m_deleteTagList.ForEach(tag =>
                {
                    if (m_categorysAndTagsList.ContainsKey(GetUntilOrEmpty(tag)))
                    {
                        if (m_categorysAndTagsList[GetUntilOrEmpty(tag)].Count == 1)
                        {
                            m_categorysAndTagsList.Remove(GetUntilOrEmpty(tag));
                        }
                        else
                        {
                            m_categorysAndTagsList[GetUntilOrEmpty(tag)].Remove(tag);
                        }
                    }
                });
                m_canRemoveTagsFromDict = false;
                m_selectedIndexCategoryForTags = 0;
            }

            if (m_categorysAndTagsList.Count == 0) return;

            m_selectedIndexCategoryForTags = EditorGUILayout.Popup("Category", m_selectedIndexCategoryForTags,
                m_categorysAndTagsList.Keys.ToArray(),
                GUILayout.Width(TAG_HEADER_TAG_WIDTH - 20));
            if (m_selectedIndexCategoryForTags >= m_categorysAndTagsList.Count) m_selectedIndexCategoryForTags = 0;

            if (m_categorysAndTagsList.Count == 0) return;

            EditorGUI.indentLevel++;
            float contentTextSize = Mathf.Clamp(((position.width - TAG_HEADER_TAG_WIDTH) / 2), 100f, 9999f);

            m_categorysAndTagsList.ElementAt(m_selectedIndexCategoryForTags).Value.ForEach(tag =>
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                {
                    m_deleteTagList.Add(tag);
                    m_newTagList.Remove(tag);
                    m_englishFile.RemoveEntry(tag);
                    m_canRemoveTagsFromDict = true;
                    isDirtyTags = true;
                    return;
                }

                if (!m_canRemoveTagsFromDict && m_englishFile.Exists(tag))
                {
                    EditorGUILayout.SelectableLabel(tag, GUILayout.Height(15),
                        GUILayout.Width(TAG_HEADER_TAG_WIDTH - 50));

                    LanguageFileEntry englishEntry = m_englishFile.GetEntry(tag);

                    string areaText = EditorGUILayout.TextArea(m_englishFile.GetEntryText(tag),
                        GUILayout.Width(contentTextSize));

                    if (!englishEntry.textContent.Equals(areaText))
                        isDirtyEng = true;

                    englishEntry.textContent = areaText;
                    m_englishFile.SetEntry(tag, englishEntry);


                    if (m_secondLanguageFile.fileLanguage != Languages.English && m_secondLanguageFile.Exists(tag))
                    {
                        LanguageFileEntry secondEntry = m_secondLanguageFile.GetEntry(tag);

                        string areaText2 = EditorGUILayout.TextArea(m_secondLanguageFile.GetEntryText(tag),
                            GUILayout.Width(contentTextSize));

                        if (!secondEntry.textContent.Equals(areaText2))
                            isDirtySecond = true;

                        secondEntry.textContent = areaText2;
                        m_secondLanguageFile.SetEntry(tag, secondEntry);
                    }
                }

                EditorGUILayout.EndHorizontal();
            });
            EditorGUI.indentLevel--;
        }

        private void DrawCreateNewTag()
        {
            GUILayout.Space(5);
            if (!m_newTagMode)
            {
                GUILayout.Space(21);
                if (GUILayout.Button("New Tag", GUILayout.Width(Mathf.Clamp(position.width / 2f, 75f, TAG_HEADER_TAG_WIDTH - 20f))))
                {
                    m_newTagMode = true;
                    m_newCategoryMode = false;
                    m_selectedIndexCategoryForNewTag = m_selectedIndexCategoryForTags;
                }
            }
            else
            {

                EditorGUILayout.BeginHorizontal(GUILayout.Width(TAG_HEADER_TAG_WIDTH));
                {
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Category");

                        if (!m_newCategoryMode && m_categorysAndTagsList.Count > 0)
                        {
                            m_selectedIndexCategoryForNewTag = EditorGUILayout.Popup(m_selectedIndexCategoryForNewTag,
                                m_categorysAndTagsList.Keys.ToArray());

                            if (m_categorysAndTagsList.Count > m_selectedIndexCategoryForNewTag &&
                                m_selectedIndexCategoryForNewTag != -1)
                            {
                                m_newTagCategory = m_categorysAndTagsList.ElementAt(m_selectedIndexCategoryForNewTag).Key;
                            }
                        }
                        else
                        {
                            m_newTagCategory = EditorGUILayout.TextField(m_newTagCategory);
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    {
                        GUILayout.Space(21);
                        if (!m_newCategoryMode && m_categorysAndTagsList.Count > 0)
                        {
                            if (GUILayout.Button("+", GUILayout.Width(20)))
                            {
                                m_newCategoryMode = true;
                                m_newTagCategory = "";
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Tag Name");
                        m_newTag = EditorGUILayout.TextField(m_newTag);
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    GUILayout.Space(21);
                    EditorGUILayout.BeginHorizontal();

                    //Bestimmen ob man ein neues Tag hinzufügen kann
                    canAddNewTag = (!m_englishFile.Exists(m_newTagCategory + "_" + m_newTag)
                                    && !string.IsNullOrEmpty(m_newTagCategory)
                                    && !string.IsNullOrEmpty(m_newTag))

                                   && TextFieldCheck(ref m_newTagCategory, true)
                                   && TextFieldCheck(ref m_newTag);

                    GUI.enabled = canAddNewTag;
                    if (GUILayout.Button("Add", GUILayout.Width(35)))
                    {
                        AddNewTag(m_newTagCategory, m_newTag);

                        m_selectedIndexCategoryForTags = m_categorysAndTagsList.Keys.ToList().IndexOf(m_newTagCategory);

                        GUI.FocusControl(null);
                        m_newTagMode = false;
                        m_newCategoryMode = false;
                        m_newTag = "";
                        m_newTagCategory = "";
                        isDirtyTags = true;
                    }

                    GUI.enabled = true;

                    if (GUILayout.Button("Reset"))
                    {
                        GUI.FocusControl(null);
                        m_newTagMode = false;
                        m_newCategoryMode = false;
                        m_newTag = "";
                        m_newTagCategory = "";
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
                if (!TextFieldCheck(ref m_newTagCategory, true)
                    || !TextFieldCheck(ref m_newTag))
                {
                    EditorGUILayout.HelpBox("Tags are only allowed to consist of characters and numbers. \nCategories cant have a number in first position.", MessageType.Warning);
                }
            }
        }

        private bool TextFieldCheck(ref string textContent, bool noNumbersInFirstPlace = false)
        {        
            if (string.IsNullOrEmpty(textContent))
                return true;

            //Buchstaben und Zahlen check
            bool output = Regex.Match(textContent, @"[^a-zA-Z\d]").Length == 0;

            //Check ob in dem ersten Zeichen eine Zahl steht
            if (noNumbersInFirstPlace)
            {
                output &= Regex.Match(textContent, @"^\d*").Length == 0;
            }

            return output;
        }

        private void DrawSaveLocalizationButton()
        {
            GUILayout.Space(5);
            GUI.enabled = isDirtyTags || isDirtyEng || isDirtySecond;
            if (GUILayout.Button("Save Localization"))
            {
                SaveFiles();
            }
            GUI.enabled = true;
        }

        private void DrawAddNewLanguageButton()
        {
            GUILayout.Space(8);
            EditorGUILayout.LabelField("Input new Language name:");

            EditorGUI.BeginChangeCheck();
            m_newLanguageName = EditorGUILayout.TextField(m_newLanguageName);
            if (EditorGUI.EndChangeCheck())
            {
                m_showSureToDelete = false;
                m_languageToDelete = Languages.English;
            }

            if (showAddFeedback)
            {
                EditorGUILayout.HelpBox(m_newLanguageName + " added", MessageType.Info);
                
                if (showAddFeedbackStartTime + MAX_SHOW_FEEDBACK_TIME <= EditorApplication.timeSinceStartup)
                {
                    showAddFeedback = false;
                }
            }

            m_canAddNewLanguage = !string.IsNullOrEmpty(m_newLanguageName) 
                                  && !Enum.IsDefined(typeof(Languages), m_newLanguageName)
                                  && TextFieldCheck(ref m_newLanguageName, true); 
            string addButtonText = "Add new language";
            if (Enum.IsDefined(typeof(Languages), m_newLanguageName))
                addButtonText = "Language already exists";
            if (!TextFieldCheck(ref m_newLanguageName, true))
                addButtonText = "Illegal characters detected";

            GUI.enabled = m_canAddNewLanguage;
            if (GUILayout.Button(addButtonText))
            {
                List<string> possibleLanguage = Enum.GetNames(typeof(Languages)).ToList();
                possibleLanguage.Add(m_newLanguageName);

                EnumGenerator.GenerateEnumFromList(Application.dataPath + LANGUAGES_ENUM_PATH, "Languages",
                    possibleLanguage);

                showAddFeedback = true;
                showAddFeedbackStartTime = EditorApplication.timeSinceStartup;
            }
            GUI.enabled = true;

            if (!TextFieldCheck(ref m_newLanguageName, true))
            {
                EditorGUILayout.HelpBox("Languages are only allowed to consist of characters and numbers. \nNo numbers in the first position.", MessageType.Warning);
            }
        }

        private void DeleteLanguage()
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Select Language to delete:");

            EditorGUI.BeginChangeCheck();
            m_languageToDelete = (Languages)EditorGUILayout.EnumPopup(m_languageToDelete);
            if (EditorGUI.EndChangeCheck())
            {
                m_showSureToDelete = false;
            }

            GUI.enabled = m_languageToDelete != Languages.English && !m_showSureToDelete;
            if (GUILayout.Button("Delete Language"))
            {
                m_showSureToDelete = true;
            }

            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            GUILayout.Space(position.width / 4);
            if (m_showSureToDelete)
            {
                if (GUILayout.Button("Are you sure?", GUILayout.Width(position.width / 2)))
                {
                    m_showSureToDelete = false;
                    showRemoveFeedback = true;
                    showAddFeedbackStartTime = EditorApplication.timeSinceStartup;

                    if (m_loadedLanguage == m_languageToDelete)
                    {
                        m_secondLanguageFile = new LanguageFile();
                    }

                    List<string> currentEnum = Enum.GetNames(typeof(Languages)).ToList();
                    currentEnum.RemoveAt((int)m_languageToDelete);

                    languageRemoved = m_languageToDelete.ToString();

                    LocalizationManager.RemoveLanguageFile(m_languageToDelete);
                    EnumGenerator.GenerateEnumFromList(Application.dataPath + LANGUAGES_ENUM_PATH, "Languages",
                        currentEnum);

                    m_languageToDelete = Languages.English;
                    AssetDatabase.Refresh();
                }
            }
            GUILayout.EndHorizontal();


            if (showRemoveFeedback)
            {
                EditorGUILayout.HelpBox(languageRemoved + " removed", MessageType.Info);
                
                if (showAddFeedbackStartTime + MAX_SHOW_FEEDBACK_TIME <= EditorApplication.timeSinceStartup)
                {
                    showRemoveFeedback = false;
                    languageRemoved = "";
                }
            }
        }

        private void DrawShowAllLanguages()
        {
            drawAllLanguages = EditorGUILayout.Foldout(drawAllLanguages, "Languages:");
            if (drawAllLanguages)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < Enum.GetNames(typeof(Languages)).Length; i++)
                {
                    EditorGUILayout.LabelField(((Languages)i).ToString());
                }
                EditorGUI.indentLevel--;
            }
        }

        //HELPER METHODEN

        private string GetUntilOrEmpty(string text, string stopAt = "_")
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return string.Empty;
        }

        private LanguageTags GetTagFromString(string tagString)
        {
            if (Enum.IsDefined(typeof(LanguageTags), tagString))
            {
                return (LanguageTags)Enum.Parse(typeof(LanguageTags), tagString);
            }

            return LanguageTags.None;
        }

        private void FindAllCategories()
        {
            if (m_englishFile.entries.Count == 0) return;

            m_englishFile.entries.ForEach(entry =>
            {
                string category = GetUntilOrEmpty(entry.textTag);
                if (m_categorysAndTagsList.ContainsKey(category))
                {
                    m_categorysAndTagsList[category].Add(entry.textTag);
                }
                else
                {
                    m_categorysAndTagsList.Add(category, new List<string>() { entry.textTag });
                }
            });
        }

        private string AddNewTag(string category, string tag)
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(tag)) return string.Empty;

            string formedTag = category + "_" + tag;
            m_englishFile.SetEntry(formedTag);

            if (m_categorysAndTagsList.ContainsKey(category))
            {
                m_categorysAndTagsList[category].Add(formedTag);
            }
            else
            {
                m_categorysAndTagsList.Add(category, new List<string>() { formedTag });
            }

            m_newTagList.Add(formedTag);

            return formedTag;
        }

        private LanguageFile LoadOrCreateLanguageFile(Languages languageToLoad)
        {
            string path = Application.dataPath + LANGUAGES_RESOURCE_PATH + languageToLoad.ToString() + ".lang";
            if (!File.Exists(path))
            {
                LanguageFile newFile = new LanguageFile(m_englishFile, languageToLoad);
                return LocalizationManager.WriteNewLanguageFile(languageToLoad, newFile);
            }

            LanguageFile file = LocalizationManager.GetLanguageDataFromFile(path);
            return file;
        }

        private void SaveFiles()
        {
            bool isRefreshEnum = false;
            GUI.FocusControl(null);
            LocalizationManager.WriteNewLanguageFile(Languages.English, m_englishFile);

            if (m_secondLanguageFile.fileLanguage != Languages.English)
            {
                LocalizationManager.WriteNewLanguageFile(m_loadedLanguage, m_secondLanguageFile);
            }

            if (m_newTagList.Count > 0)
            {
                for (int i = 0; i < Enum.GetNames(typeof(Languages)).Length; i++)
                {
                    if ((Languages)i != Languages.English)
                    {
                        LanguageFile otherFile = LoadOrCreateLanguageFile((Languages)i);

                        LocalizationManager.WriteNewKeysToLanguageFile(otherFile.fileLanguage, m_newTagList, null);
                    }
                }

                isRefreshEnum = true;
            }

            if (m_deleteTagList.Count > 0)
            {
                for (int i = 0; i < Enum.GetNames(typeof(Languages)).Length; i++)
                    LocalizationManager.RemoveTagsFromLanguageFile((Languages)i, m_deleteTagList);

                m_deleteTagList.Clear();
                isRefreshEnum = true;
            }

            if (isRefreshEnum)
                RefreshTagEnum();

            if (Enum.IsDefined(typeof(Languages), (int)m_loadedLanguage))
                m_secondLanguageFile = LoadOrCreateLanguageFile(m_loadedLanguage);

            m_newTagList.Clear();

            isDirtyTags = false;
            isDirtyEng = false;
            isDirtySecond = false;
        }

        private void RefreshTagEnum()
        {
            List<string> tags = new List<string>();
            tags.AddRange(m_englishFile.GetTags());
            tags.Sort();
            tags.Insert(0, "None");

            EnumGenerator.GenerateLanguageFileTagsFromListWithPrevious(Application.dataPath + LocalizationManager.TagEnumPath, tags);
        }
    }
}
#endif