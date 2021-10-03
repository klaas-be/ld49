using System;
using System.Collections.Generic;
using System.IO;
using Assets.TekkTech.Scripts.Language;

namespace Assets.TekkTech.Scripts.Utility
{
    public static class ExcelManager
    {
        public static char excelSeperatorChar = ',';
        public static char excelStringCombinerChar = '"';

        public static string ReadOneLine(string path, bool ignoreFirstLine = false)
        {
            StreamReader sr = new StreamReader(path);

            if (ignoreFirstLine)
            {
                sr.ReadLine();
            }
            string output = sr.ReadLine();

            sr.Close();

            return output;
        }

        public static string CheckExcel(string path)
        {
            int index = 0;
            StreamReader sr = new StreamReader(path);

            while (!sr.EndOfStream)
            {
                string column = sr.ReadLine();
                if (column != null && !column.Contains(excelSeperatorChar.ToString()))
                {
                    sr.Close();
                    return "Split seperator "+ excelSeperatorChar + " not found - line " + index;
                }

                string[] splitColumn = CsvSplit(column);
                if (splitColumn.Length < 3)
                {
                    sr.Close();
                    return "Too few columns. - line " + index;
                }

                index++;
            }

            sr.Close();
            return "";
        }

        public static void GenerateCsvFile(string path)
        {
            StreamWriter sw = new StreamWriter(path);

            string line = "Category"+ excelSeperatorChar + "Tag" + excelSeperatorChar;
            //string adder = "";
            foreach (string s in Enum.GetNames(typeof(Languages)))
            {
                line += s + excelSeperatorChar;
                //adder += excelSeperatorChar;
            }
            line = line.Remove(line.Length - 1, 1);
            //adder = adder.Remove(adder.Length - 1, 1);

            sw.WriteLine(line);

            List<LanguageFile> files = GetAllLanguageFiles();

            foreach (string s in Enum.GetNames(typeof(LanguageTags)))
            {
                if (s == LanguageTags.None.ToString())
                    continue;

                line = ExtractCategory(s) + excelSeperatorChar + ExtractTag(s) + excelSeperatorChar;// + adder;

                foreach (LanguageFile file in files)
                {
                    if (file != null)
                        line += excelStringCombinerChar + file.GetEntryText(s) + excelStringCombinerChar + excelSeperatorChar;
                    else
                        line += excelSeperatorChar;
                }
                line = line.Remove(line.Length - 1, 1);

                sw.WriteLine(line);
            }
            sw.Close();

            files.Clear();
            GC.Collect(); //?
        }
    
        private static List<LanguageFile> GetAllLanguageFiles()
        {
            List<LanguageFile> files = new List<LanguageFile>();
            foreach (string lang in Enum.GetNames(typeof(Languages)))
            {
                files.Add(LocalizationManager.GetLanguageDataFromFile((Languages)Enum.Parse(typeof(Languages), lang)));
            }
            return files;
        }

        public static string ExtractCategory(string fullTag)
        {
            return fullTag.Split('_')[0];
        }
        public static string ExtractTag(string fullTag)
        {
            return fullTag.Split('_')[1];
        }

        public static List<LanguageFileEntry> ImportExcelFileEntries(string path, int categoryIndex, int tagNameIndex, int contentIndex, bool ignoreHeader = false)
        {
            List<LanguageFileEntry> entriesInExcel = new List<LanguageFileEntry>();

            StreamReader sr = new StreamReader(path);
            while (!sr.EndOfStream)
            {
                if (ignoreHeader) sr.ReadLine();

                string[] column = CsvSplit(sr.ReadLine());
                string tag = column[categoryIndex] + "_" + column[tagNameIndex];
                entriesInExcel.Add(new LanguageFileEntry(tag, column[contentIndex]));
            }
            sr.Close();
            return entriesInExcel;
        }

        public static string[] CsvSplit(string line)
        {
            List<string> output = new List<string>();

            string split = "";
            bool isInBrakets = false;
            bool isEndOfSplit = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == excelStringCombinerChar
                    && !isInBrakets)
                {
                    isInBrakets = true;
                    continue;
                }

                if (line[i] == excelStringCombinerChar
                    && isInBrakets)
                {
                    if (i+1 < line.Length)
                    {
                        if (line[i + 1] == excelStringCombinerChar)
                        {
                            split += line[i];
                            i++;
                            continue;
                        }

                        if (line[i + 1] == excelSeperatorChar)
                        {
                            isInBrakets = false; 
                            isEndOfSplit = true;                       
                            continue;
                        }
                    }
                    else
                    {
                        isInBrakets = false;
                        isEndOfSplit = true;
                        continue;
                    }
                }

                if (line[i] != excelSeperatorChar)
                {
                    split += line[i];
                }
                else
                {
                    if (isInBrakets)
                    {
                        split += line[i];
                    }
                    else
                    {
                        isEndOfSplit = true;
                    }
                }

                if (!isEndOfSplit) continue;

                output.Add(split);
                split = string.Empty; 
                isEndOfSplit = false;
            }

            output.Add(split);
            return output.ToArray();
        }
    }
}
