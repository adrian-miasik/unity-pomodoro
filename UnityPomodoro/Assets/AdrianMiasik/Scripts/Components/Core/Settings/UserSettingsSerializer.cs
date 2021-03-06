using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace AdrianMiasik.Components.Core.Settings
{
    /// <summary>
    /// Saves/loads our user settings to/from our persistent data paths respectively. (<see cref="SystemSettings"/> and
    /// <see cref="TimerSettings"/>)
    /// </summary>
    public static class UserSettingsSerializer
    {
        private static string dataExtension = ".dat";
        
        /// <summary>
        /// Creates two data files in our persistent path. One for system settings, and one for timer settings.
        /// </summary>
        /// <param name="systemSettings"></param>
        /// <param name="timerSettings"></param>
        public static void Save(SystemSettings systemSettings, TimerSettings timerSettings)
        {
            BinaryFormatter bf = new BinaryFormatter();
            
            // System settings
            FileStream fs = File.Create(Application.persistentDataPath + "/system-settings" + dataExtension);
            bf.Serialize(fs, systemSettings);
            
            // Timer settings
            fs = File.Create(Application.persistentDataPath + "/timer-settings" + dataExtension);
            bf.Serialize(fs, timerSettings);
            
            fs.Close();

            Debug.Log("Creating files.");
        }

        public static void SaveSystemSettings(SystemSettings systemSettings)
        {
            BinaryFormatter bf = new BinaryFormatter();
            
            // System settings
            FileStream fs = File.Create(Application.persistentDataPath + "/system-settings" + dataExtension);
            bf.Serialize(fs, systemSettings);

            fs.Close();
            
#if USER_SETTINGS_EVENT_LOGS
            Debug.Log("System Settings Saved.");
#endif
        }
        
        public static void SaveTimerSettings(TimerSettings timerSettings)
        {
            BinaryFormatter bf = new BinaryFormatter();
            
            // System settings
            FileStream fs = File.Create(Application.persistentDataPath + "/timer-settings" + dataExtension);
            bf.Serialize(fs, timerSettings);

            fs.Close();
            
#if USER_SETTINGS_EVENT_LOGS
            Debug.Log("Timer Settings Saved.");
#endif
        }

        /// <summary>
        /// Returns the timer settings saved in  our data file.
        /// </summary>
        /// <returns></returns>
        public static TimerSettings LoadTimerSettings()
        {
            if (File.Exists(Application.persistentDataPath + "/timer-settings" + dataExtension))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(Application.persistentDataPath + "/timer-settings" + dataExtension,
                    FileMode.Open);
                TimerSettings timerSettings = bf.Deserialize(fs) as TimerSettings;
                fs.Close();
#if USER_SETTINGS_EVENT_LOGS
                Debug.Log("Loaded Timer Settings Successfully!");
#endif
                return timerSettings;
            }

            Debug.LogWarning("File not found. Unable to load timer settings.");
            return null;
        }

        /// <summary>
        /// Returns the system settings saved in our data file.
        /// </summary>
        /// <returns></returns>
        public static SystemSettings LoadSystemSettings()
        {
            if (File.Exists(Application.persistentDataPath + "/system-settings" + dataExtension))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(Application.persistentDataPath + "/system-settings" + dataExtension,
                    FileMode.Open);
                SystemSettings systemSettings = bf.Deserialize(fs) as SystemSettings;
                fs.Close();
#if USER_SETTINGS_EVENT_LOGS
                Debug.Log("Loaded System Settings Successfully!");
#endif
                return systemSettings;
            }

            Debug.LogWarning("File not found. Unable to load system settings.");
            return null;
        }

        public static void WipeSystemSettings()
        {
            if (File.Exists(Application.persistentDataPath + "/system-settings" + dataExtension))
            {
                File.Delete(Application.persistentDataPath + "/system-settings" + dataExtension);
            }
        }

        public static void WipeTimerSettings()
        {
            if (File.Exists(Application.persistentDataPath + "/timer-settings" + dataExtension))
            {
                File.Delete(Application.persistentDataPath + "/timer-settings" + dataExtension);
            }
        }
    }
}