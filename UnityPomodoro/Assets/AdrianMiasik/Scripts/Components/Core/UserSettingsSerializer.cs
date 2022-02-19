using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    public static class UserSettingsSerializer
    {
        private static string userSettingsFile = "/UnityPomodoroUserSettings.dat";
        
        public static void Save(TimerSettings settingsToSave)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            FileStream fStream = File.Create(Application.persistentDataPath + userSettingsFile);
            bFormatter.Serialize(fStream, settingsToSave);
            fStream.Close();
        }

        public static TimerSettings Load()
        {
            if (File.Exists(Application.persistentDataPath + userSettingsFile))
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                FileStream fStream = File.Open(Application.persistentDataPath + userSettingsFile, FileMode.Open);
                TimerSettings userSettings = bFormatter.Deserialize(fStream) as TimerSettings;
                fStream.Close();
                Debug.Log("Loaded Timer Settings Successfully!");
                return userSettings;
            }

            Debug.LogWarning("File not found. Unable to load user settings.");
            return null;
        }
    }
}