﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Roguelike.Utilities;
using UnityEngine;

namespace Roguelike.Localization
{
    public class CSVLoader
    {
        private readonly char _lineSeparator = '\n';
        private readonly char _fieldSurround = '"';
        private readonly char _fieldEnding = '\r';
        private readonly string _fieldSeparator = "\",\"";
        
        private TextAsset _csvFile;

        public void LoadCSV(string csvPath)
        {
            _csvFile = Resources.Load<TextAsset>(csvPath);
        }

        public Dictionary<string, string> GetDictionaryValues(string attributeId)
        {
            Dictionary<string, string> dictionary = new();

            string[] lines = _csvFile.text.Split(_lineSeparator);
            string[] headers = lines[0].Split(_fieldSeparator);
            int attributeIndex = -1;

            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains(attributeId))
                {
                    attributeIndex = i;
                    break;
                }
            }
            
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] fields = line.Split(_fieldSeparator);

                for (int j = 0; j < fields.Length; j++)
                {
                    fields[j] = fields[j].TrimStart(' ', _fieldSurround);
                    fields[j] = fields[j].TrimEnd(_fieldSurround, _fieldEnding);
                }

                if (fields.Length > attributeIndex)
                {
                    string key = fields[0];

                    if (dictionary.ContainsKey(key))
                        continue;

                    string value = fields[attributeIndex];

                    dictionary.Add(key, value);
                }
            }

            return dictionary;
        }

#if UNITY_EDITOR
        public void Add(string filePath, string key, string value)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"\n\"{key}\",\"{value}\"");
            
            Language[] languages = EnumExtensions.GetValues<Language>();

            for (int i = 1; i < languages.Length; i++) 
                stringBuilder.Append(",");

            File.AppendAllText(filePath, stringBuilder.ToString());

            UnityEditor.AssetDatabase.Refresh();
        }

        public void Remove(string filePath, string key)
        {
            string[] lines = _csvFile.text.Split(_lineSeparator);
            string[] keys = new string[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                keys[i] = line.Split(_fieldSeparator, StringSplitOptions.None)[0];
            }

            int index = -1;

            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].Contains(key))
                {
                    index = i;
                    break;
                }
            }

            if (index > -1)
            {
                string[] newLines = lines.Where(s => s != lines[index]).ToArray();
                string replaced = string.Join(_lineSeparator.ToString(), newLines);
                File.WriteAllText(filePath, replaced);
            }
        }

        public void Edit(string filePath, string key, string value)
        {
            Remove(filePath, key);
            Add(filePath, key, value);
        }

        public void SortByKeyNames(string filePath)
        {
            string[] lines = _csvFile.text.Split(_lineSeparator);

            lines = lines
                .Select(line => new Regex(@"\r*")
                .Replace(line, string.Empty))
                .ToArray();

            IEnumerable<string> data = lines
                .Where(line => string.IsNullOrEmpty(line) == false)
                .Skip(1)
                .Select(line => new {Fields = line.Split(_fieldSeparator), Line = line})
                .OrderBy(x => x.Fields[0])
                .Select(x => x.Line);

            File.WriteAllLines(filePath, lines.Take(1).Concat(data));
        }
#endif
    }
}