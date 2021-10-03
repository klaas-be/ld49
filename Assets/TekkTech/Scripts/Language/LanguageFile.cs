using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.TekkTech.Scripts.Language
{
    [Serializable]
    public class LanguageFile
    {
        [SerializeField] public Languages fileLanguage = Languages.English;
        [SerializeField] public List<LanguageFileEntry> entries = new List<LanguageFileEntry>();

        public LanguageFile()
        {
        }

        public LanguageFile(LanguageFile structFile, Languages language = Languages.English)
        {
            fileLanguage = language != Languages.English ? structFile.fileLanguage : language;
            structFile.entries.ForEach(entry => { entries.Add(new LanguageFileEntry(entry.textTag, "")); });
        }

        public LanguageFile(string key, string content)
        {
            AddEntry(key, content);
        }

        public string GetJsonFormat()
        {
            return JsonUtility.ToJson(this, true);
        }


        public void RemoveEntry(int index)
        {
            entries.RemoveAt(index);
        }

        public void RemoveEntry(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;
            if (!this.Exists(tag)) return;

            entries.RemoveAt(entries.FindIndex(e => e.textTag == tag));
        }

        public LanguageFileEntry GetEntry(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return new LanguageFileEntry(tag, "");
            if (!this.Exists(tag)) return new LanguageFileEntry(tag, "");

            LanguageFileEntry entry = entries[entries.FindIndex(e => e.textTag == tag)];
            return entry;
        }

        public string GetEntryText(string key)
        {
            string content = "";
            entries.ForEach(entry =>
            {
                if (entry.textTag == key)
                {
                    content = entry.textContent;
                }
            });
            return content;
        }

        public void SetEntry(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;
            if (!this.Exists(tag))
            {
                AddEntry(tag, string.Empty);
            }
        }

        public void SetEntry(string tag, LanguageFileEntry entry)
        {
            if (string.IsNullOrEmpty(tag)) return;
            if (!this.Exists(tag))
            {
                AddEntry(tag, entry.textContent);
            }
            else
            {
                entries[entries.FindIndex(e => string.Equals(e.textTag, tag, StringComparison.CurrentCultureIgnoreCase))] = entry;
            }
        }

        private void AddEntry(string key, string content)
        {
            entries.Add(new LanguageFileEntry(key, content));
        }

        public List<string> GetTags()
        {
            List<string> keys = new List<string>();

            entries.ForEach(entry => { keys.Add(entry.textTag); });

            return keys;
        }

        public List<string> GetContents()
        {
            List<string> contents = new List<string>();

            entries.ForEach(entry => { contents.Add(entry.textContent); });

            return contents;
        }

        public void EmptyContent()
        {
            entries.ForEach(entry => { entry.textContent = ""; });
        }

        public bool Exists(string tag)
        {
            bool exists = false;
            entries.ForEach(entry =>
            {
                exists |= string.Equals(entry.textTag, tag, StringComparison.CurrentCultureIgnoreCase);
            });
            return exists;
        }

        public bool Exists(string category, string tagname)
        {
            return Exists(category + "_" + tagname);
        }

    }

    [Serializable]
    public struct LanguageFileEntry
    {
        [SerializeField] public string textTag;
        [SerializeField] public string textContent;

        public LanguageFileEntry(string tag, string content)
        {
            textTag = tag;
            textContent = content;
        }
    }
}