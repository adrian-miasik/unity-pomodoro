using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Steamworks;
using UnityEditor;
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
        private const string SteamSystemSettingsPath = "system-settings" + DataExtension;

        [MenuItem("Adrian Miasik/Steam/Print Steam Cloud Files")]
        public static void PrintAllSteamCloudFiles()
        {
            if (SteamClient.IsValid)
            {
                if (SteamRemoteStorage.FileCount <= 0)
                {
                    Debug.Log("No remote Steam files found.");
                    return;
                }

                foreach (string file in SteamRemoteStorage.Files)
                {
                    Debug.Log($"{file}" +
                              $" ({SteamRemoteStorage.FileSize(file)} " +
                              $"{SteamRemoteStorage.FileTime( file )})");
                }
            }
            else
            {
                Debug.LogWarning("Steam Client not found. Please init Steam Manager by entering play mode" +
                                 " and try again.");
            }
        }

        [MenuItem("Adrian Miasik/Steam/Delete Steam Cloud Files")]
        public static void DeleteAllSteamCloudFiles()
        {
            if (SteamClient.IsValid)
            {
                foreach (string file in SteamRemoteStorage.Files)
                {
                    SteamRemoteStorage.FileDelete(file);
                }
            }
            else
            {
                Debug.LogWarning("Steam Client not found. Please init Steam Manager by entering play mode" +
                                 " and try again.");
            }
        }

        public static void SaveSystemSettings(SystemSettings systemSettings)
        {
            // If Steam Client is enabled and found: Save system settings to Steam Cloud...
            if (SteamClient.IsValid)
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, systemSettings);

                SteamRemoteStorage.FileWrite(SteamSystemSettingsPath, ms.ToArray());
                ms.Close();
                
                Debug.Log("Steam Cloud: SYSTEM settings file uploaded successfully.");
            }
            // Otherwise, use local storage...
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
            
                // System settings
                FileStream fs = File.Create(Application.persistentDataPath + "/system-settings" + DataExtension);
                bf.Serialize(fs, systemSettings);

                fs.Close();
                
                Debug.Log("Local Storage: SYSTEM settings file saved.");
            }
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
            // If Steam Client is enabled and found: Attempt to load settings from Steam Cloud...
            if (SteamClient.IsValid)
            {
                // Validate Steam Cloud Save data...
                if (SteamRemoteStorage.FileExists(SteamSystemSettingsPath))
                {
                    // Fetch Steam Cloud file...
                    byte[] steamCloudFile = SteamRemoteStorage.FileRead(SteamSystemSettingsPath);
                    
                    // Convert byte array back into object (SystemSettings)
                    MemoryStream ms = new MemoryStream();
                    BinaryFormatter bf = new BinaryFormatter();
                    ms.Write(steamCloudFile, 0, steamCloudFile.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    SystemSettings systemSettings = bf.Deserialize(ms) as SystemSettings;
                    ms.Close();
                    
                    Debug.Log("Steam Cloud: SYSTEM settings file downloaded successfully!");
                    
                    return systemSettings;
                }
                
                Debug.LogWarning("Steam Cloud: SYSTEM settings file not found. " +
                                 "Falling back/checking local storage...");
            }
            
            // Otherwise, load local storage system settings...
            if (File.Exists(Application.persistentDataPath + "/system-settings" + DataExtension))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(Application.persistentDataPath + "/system-settings" + DataExtension,
                    FileMode.Open);
                SystemSettings systemSettings = bf.Deserialize(fs) as SystemSettings;
                fs.Close();
                
                Debug.Log("Local Storage: SYSTEM settings file loaded successfully!");
                return systemSettings;
            }

            Debug.LogWarning("Local Storage: SYSTEM settings file not found.");
            return null;
        }

        public static void WipeSystemSettings()
        {
            if (SteamClient.IsValid)
            {
                // Attempt to remove Steam Cloud save...
                if (SteamRemoteStorage.FileExists(SteamSystemSettingsPath))
                {
                    // Wipe remote storage
                    SteamRemoteStorage.FileDelete(SteamSystemSettingsPath);

                    Debug.Log("Steam Cloud: SYSTEM settings file deleted successfully.");
                    
                    // Early exit
                    return;
                }
            }
            
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