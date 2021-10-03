#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Assets.TekkTech.Scripts.Utility
{
    public class EnumGenerator
    {
        public static void GenerateEnumFromList(string filePath, string enumName, List<string> enumValuesList)
        {
            if (enumValuesList.Count == 0) return;
        
            StreamWriter sw = new StreamWriter(filePath + "/" + enumName + ".cs", false);
        
            sw.WriteLine("public enum " + enumName);
            sw.WriteLine("{");

            for (int i = 0; i < enumValuesList.Count; i++)
            {
                sw.Write("\t");
                sw.Write(enumValuesList[i]);
                if (i < enumValuesList.Count - 1)
                {
                    sw.WriteLine(",");
                }
            }
        
            sw.WriteLine("");
            sw.WriteLine("}");
            sw.Close();
        
            AssetDatabase.Refresh();
        }

        public static void GenerateLanguageFileTagsFromListWithPrevious(string filePath, List<string> enumValuesList)
        {
            if (enumValuesList.Count == 0) return;
        
            Dictionary<string, int> currentIntAllocation = new Dictionary<string, int>();
            int biggestValue = 0;
        
            enumValuesList.ForEach(value =>
            {
                if (Enum.IsDefined(typeof(LanguageTags), value))
                {
                    int index = (int) (LanguageTags) Enum.Parse(typeof(LanguageTags), value);
                    currentIntAllocation.Add(value, index);

                    if (index > biggestValue)
                    {
                        biggestValue = index;
                    }
                }
            });
        
            StreamWriter sw = new StreamWriter(filePath + "/LanguageTags.cs", false);
        
            sw.WriteLine("public enum LanguageTags");
            sw.WriteLine("{");

            for (int i = 0; i < enumValuesList.Count; i++)
            {
                sw.Write("\t");
                sw.Write(enumValuesList[i]);
                if (currentIntAllocation.ContainsKey(enumValuesList[i]))
                {
                    sw.Write("=" + currentIntAllocation[enumValuesList[i]]);
                }
                else
                {
                    biggestValue++;
                    sw.Write("=" + biggestValue);
                }

                if (i < enumValuesList.Count - 1)
                {
                    sw.WriteLine(",");
                }
            }
        
            sw.WriteLine("");
            sw.WriteLine("}");
            sw.Close();
        
            AssetDatabase.Refresh();
        }
    }
}
#endif