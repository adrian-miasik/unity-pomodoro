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
        private const string DataExtension = ".dat";

        public static void SaveSystemSettings(SystemSettings systemSettings)
        {
            BinaryFormatter bf = new BinaryFormatter();
            
            // Attempt to save the provided system settings to the Steam Cloud...
            if (SteamManager.Initialized)
            {
                string steamSystemSettingsPath = Application.persistentDataPath + "/" + SteamUser.GetSteamID() +
                                                 "/system-settings" + DataExtension;

                // Create Steam User ID cloud save directory
                if (!Directory.Exists(Application.persistentDataPath + "/" + SteamUser.GetSteamID()))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/" + SteamUser.GetSteamID());
                }
                
                // Convert system settings to memory stream for easy byte array conversion using .ToArray().
                MemoryStream ms = new MemoryStream();
                FileStream fs = File.Create(steamSystemSettingsPath);
                bf.Serialize(fs, systemSettings);
                fs.CopyTo(ms);
                fs.Close();
                
                // Save Steam Cloud file
                SteamRemoteStorage.FileWrite(steamSystemSettingsPath, ms.ToArray(), ms.Capacity);
            }
            // Otherwise, use local storage.
            else
            {
                // System settings
                FileStream fs = File.Create(Application.persistentDataPath + "/system-settings" + DataExtension);
                bf.Serialize(fs, systemSettings);

                fs.Close();
            }

#if USER_SETTINGS_EVENT_LOGS
            Debug.Log("System Settings Saved.");
#endif
        }
        
        public static void SaveTimerSettings(TimerSettings timerSettings)
        {
            BinaryFormatter bf = new BinaryFormatter();
            
            // System settings
            FileStream fs = File.Create(Application.persistentDataPath + "/timer-settings" + DataExtension);
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
            if (File.Exists(Application.persistentDataPath + "/timer-settings" + DataExtension))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(Application.persistentDataPath + "/timer-settings" + DataExtension,
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
            if (File.Exists(Application.persistentDataPath + "/system-settings" + DataExtension))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(Application.persistentDataPath + "/system-settings" + DataExtension,
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
            if (File.Exists(Application.persistentDataPath + "/system-settings" + DataExtension))
            {
                File.Delete(Application.persistentDataPath + "/system-settings" + DataExtension);
            }
        }

        public static void WipeTimerSettings()
        {
            if (File.Exists(Application.persistentDataPath + "/timer-settings" + DataExtension))
            {
                File.Delete(Application.persistentDataPath + "/timer-settings" + DataExtension);
            }
        }
    }
}