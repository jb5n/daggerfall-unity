// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Wenzil.Console;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Text manager singleton class.
    /// </summary>
    public class TextManager : MonoBehaviour
    {
        #region Fields

        const string textFolderName = "Text";
        const string textColumn = "text";

        public string textRSCCollection = string.Empty;

        Dictionary<string, Table> textDatabases = new Dictionary<string, Table>();

        #endregion

        #region Unity

        private void Awake()
        {
            EnumerateTextDatabases();    
        }

        private void Start()
        {
            ConsoleCommandsDatabase.RegisterCommand(Locale_Print.name, Locale_Print.description, Locale_Print.usage, Locale_Print.Execute);
            ConsoleCommandsDatabase.RegisterCommand(Locale_Set.name, Locale_Set.description, Locale_Set.usage, Locale_Set.Execute);
            ConsoleCommandsDatabase.RegisterCommand(Locale_Debug.name, Locale_Debug.description, Locale_Debug.usage, Locale_Debug.Execute);
        }

        #endregion

        #region Text Database Methods

        /// <summary>
        /// Checks if text database table was found enumerated StreamingAssets/Text folder.
        /// </summary>
        /// <param name="databaseName">Name of database.</param>
        /// <returns>True if database was enumerated.</returns>
        public bool HasDatabase(string databaseName)
        {
            return textDatabases.ContainsKey(databaseName);
        }

        /// <summary>
        /// Checks if both text database and text key exists.
        /// </summary>
        /// <param name="databaseName">Name of database.</param>
        /// <param name="key">Key of text in database.</param>
        /// <returns>True if both database and text key enumerated.</returns>
        public bool HasText(string databaseName, string key)
        {
            if (!HasDatabase(databaseName))
                return false;

            return textDatabases[databaseName].HasValue(key);
        }

        /// <summary>
        /// Gets text value from database.
        /// </summary>
        /// <param name="databaseName">Name of text database.</param>
        /// <param name="key">Key of text in database.</param>
        /// <returns>Text if found, otherwise return an error string instead.</returns>
        public string GetText(string databaseName, string key)
        {
            // Show an error if text not found
            if (!HasText(databaseName, key))
                return "<TextError-NotFound>";

            return textDatabases[databaseName].GetValue(textColumn, key);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Enumerate all available text databases.
        /// </summary>
        protected void EnumerateTextDatabases()
        {
            // Get all text files in target path
            Debug.Log("TextManager enumerating text databases.");
            string path = Path.Combine(Application.streamingAssetsPath, textFolderName);
            string[] files = Directory.GetFiles(path, "*.txt");

            // Attempt to read each file as a table with a text schema
            foreach (string file in files)
            {
                try
                {
                    // Create table from text file
                    Table table = new Table(File.ReadAllLines(file));

                    // Get database key from filename
                    string databaseName = Path.GetFileNameWithoutExtension(file);
                    if (HasDatabase(databaseName))
                        throw new Exception(string.Format("TextManager database name {0} already exists.", databaseName));

                    // Assign database to collection
                    textDatabases.Add(databaseName, table);
                    Debug.LogFormat("TextManager read text database table {0} with {1} rows", databaseName, table.RowCount);
                }
                catch (Exception ex)
                {
                    Debug.LogFormat("TextManager unable to parse text database table {0} with exception message {1}", file, ex.Message);
                    continue;
                }
            }
        }

        #endregion

        #region Singleton

        static TextManager instance = null;
        public static TextManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindSingleton(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "TextManager";
                        instance = go.AddComponent<TextManager>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        public static bool FindSingleton(out TextManager singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType<TextManager>();
            if (singletonOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate TextManager GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        #endregion

        #region Console Commands

        private static class Locale_Print
        {
            public static readonly string name = "locale_print";
            public static readonly string description = "Output available locales.";
            public static readonly string usage = "locale_print";

            public static string Execute(params string[] args)
            {
                int index = 0;
                string output = string.Empty;
                var locales = LocalizationSettings.AvailableLocales.Locales;
                foreach (var locale in locales)
                {
                    output += string.Format("{0}. {1}\n", index++, locale.name);
                }
                output += string.Format("Current locale: {0}", LocalizationSettings.SelectedLocale.name);

                return output;
            }
        }

        private static class Locale_Set
        {
            public static readonly string name = "locale_set";
            public static readonly string description = "Sets current locale. Use local_print to list avilable locales.";
            public static readonly string usage = "locale_set <index>";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1)
                    return string.Format("Usage: {0}", usage);

                int index;
                if (!int.TryParse(args[0], out index))
                    return usage;

                var locales = LocalizationSettings.AvailableLocales.Locales;
                if (index < 0 || index > locales.Count - 1)
                    return "Locale index out of range.";

                return "Not Implemented.";

                // TODO: Set locale
                //return string.Format("Set locale to '{0}'", locale.name);
            }
        }

        private static class Locale_Debug
        {
            public static readonly string name = "locale_debug";
            public static readonly string description = "Enables verbose localization debug to player log.";
            public static readonly string usage = "locale_debug on|off";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1)
                    return string.Format("Usage: {0}", usage);

                if (args[0] == "on")
                {
                    DaggerfallUnity.Instance.TextProvider.EnableLocalizedStringDebug(true);
                    return "Localization debug enabled";
                }
                else if (args[0] == "off")
                {
                    DaggerfallUnity.Instance.TextProvider.EnableLocalizedStringDebug(false);
                    return "Localization debug disabled";
                }
                else
                {
                    return usage;
                }
            }
        }

        #endregion
    }
}
