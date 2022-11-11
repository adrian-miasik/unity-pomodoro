using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AdrianMiasik.Components.Core.Helpers;
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

        /// <summary>
        /// Prints all of our Steam Cloud remote files into the console.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Steam Cloud/Print all Steam cloud files")]
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

        /// <summary>
        /// Deletes/Wipes all of our Steam Cloud remote files from the cloud.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Steam Cloud/Delete all Steam cloud files")]
        public static void DeleteAllSteamCloudFiles()
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
                    SteamRemoteStorage.FileDelete(file);
                }

                Debug.Log("Steam Cloud: All Steam cloud files have been deleted successfully!");
            }
            else
            {
                Debug.LogWarning("Steam Client not found. Please init Steam Manager by entering play mode" +
                                 " and try again.");
            }
        }

        /// <summary>
        /// Saves the provided SystemSettings to both the Steam Cloud and Local Storage.
        /// </summary>
        /// <param name="systemSettings"></param>
        public static void SaveSystemSettings(SystemSettings systemSettings)
        {
            BinaryFormatter bf = new BinaryFormatter();
            
            // If Steam Client is enabled and found: Save system settings to Steam Cloud...
            if (SteamClient.IsValid)
            {
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, systemSettings);

                SteamRemoteStorage.FileWrite(SteamSystemSettingsPath, ms.ToArray());
                ms.Close();
                
                Debug.Log("Steam Cloud: SYSTEM settings file uploaded successfully.");
            }
            
            // Also save system settings to local storage...
            FileStream fs = File.Create(Application.persistentDataPath + "/system-settings" + DataExtension);
            bf.Serialize(fs, systemSettings);
            fs.Close();
            
            Debug.Log("Local Storage: SYSTEM settings file saved successfully.");
        }
        
        /// <summary>
        /// Saves the provided TimerSettings to our Local Storage. TODO: Add Steam Cloud Support
        /// </summary>
        /// <param name="timerSettings"></param>
        public static void SaveTimerSettings(TimerSettings timerSettings)
        {
            // TODO: Steam Cloud support
            BinaryFormatter bf = new BinaryFormatter();
            
            // System settings
            FileStream fs = File.Create(Application.persistentDataPath + "/timer-settings" + DataExtension);
            bf.Serialize(fs, timerSettings);

            fs.Close();
            Debug.Log("Timer Settings Saved.");
        }

        /// <summary>
        /// Returns the TimerSettings saved in our data file/local storage. TODO: Add Steam Cloud Support
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
                // Debug.Log("Loaded Timer Settings Successfully!");
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
                    
                    // Override local storage SYSTEM settings file
                    FileStream fs = File.Create(Application.persistentDataPath + "/system-settings" + DataExtension);
                    bf.Serialize(fs, systemSettings);
                    fs.Close();
                    Debug.Log("Steam Cloud -> Local Storage: Saved Steam Cloud SYSTEM settings file to local " +
                              "storage for future fallback.");
                    
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

                // Override Steam Cloud SYSTEM settings file...
                if (SteamClient.IsValid)
                {
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, systemSettings);
                    SteamRemoteStorage.FileWrite(SteamSystemSettingsPath, ms.ToArray());
                    ms.Close();
                    Debug.Log("Local Storage -> Steam Cloud: Saved local storage SYSTEM settings file to Steam" +
                              " Cloud for future fallback.");
                }

                return systemSettings;
            }

            Debug.LogWarning("Local Storage: SYSTEM settings file not found.");
            return null;
        }

        private enum MostRecentSaveLocation
        {
            UNABLE_TO_DETERMINE,
            LOCAL_STORAGE,
            STEAM_CLOUD,
            LOCAL_STORAGE_AND_STEAM_CLOUD
        }
        
        private static MostRecentSaveLocation FetchMostRecentSaveMethod()
        {
            if (!SteamClient.IsValid)
            {
                return MostRecentSaveLocation.UNABLE_TO_DETERMINE;
            }
            
            // Fetch user data remote storage directory
            string steamUserRemoteSystemSettingsPath = FetchUserDataRemoteDirectory() 
                                                       + "\\" + SteamSystemSettingsPath;
                    
            // Check validity of user data remote storage directory...
            if (File.Exists(steamUserRemoteSystemSettingsPath))
            {
                // Cache last accessed / modified file times of our SYSTEM settings files.
                DateTime steamRemoteWriteTime = File.GetLastWriteTime(steamUserRemoteSystemSettingsPath);
                DateTime localStorageWriteTime = File.GetLastWriteTime(Application.persistentDataPath +
                                                                       "/system-settings" + DataExtension);
                
                // Debug.Log("Steam file time: " + steamRemoteWriteTime);
                // Debug.Log("Local file time: " + localStorageWriteTime);
                
                if (steamRemoteWriteTime > localStorageWriteTime)
                {
                    Debug.Log("Most recent SYSTEM file: Steam Cloud");
                    return MostRecentSaveLocation.STEAM_CLOUD;

                }

                if (steamRemoteWriteTime.TrimMilliseconds() == localStorageWriteTime.TrimMilliseconds())
                {
                    Debug.Log("Both Steam cloud & local storage SYSTEM files written at the same time.");
                    return MostRecentSaveLocation.LOCAL_STORAGE_AND_STEAM_CLOUD;
                }
                
                Debug.Log("Most recent SYSTEM file: Local Storage");
                return MostRecentSaveLocation.LOCAL_STORAGE;
            }

            return MostRecentSaveLocation.UNABLE_TO_DETERMINE;
        }

        /// <summary>
        /// Returns the remote storage directory for this app and user.
        /// </summary>
        /// <example>Returns something similar to: 'C:\Program Files (x86)\Steam\userdata\1007343656\2173940\remote'
        /// </example>
        /// <returns></returns>
        private static string FetchUserDataRemoteDirectory()
        {
            // Get install directory
            string result = SteamApps.AppInstallDir();

            // Ignore everything after "steamapps" including itself...
            int indexToIgnoreAfter = result.IndexOf("steamapps", StringComparison.Ordinal);
            if (indexToIgnoreAfter >= 0)
            {
                result = result.Substring(0, indexToIgnoreAfter);
                
                // Find remote storage directory
                result += "userdata";
                result += "\\" + SteamClient.SteamId.AccountId;
                result += "\\" + SteamClient.AppId;
                result += "\\" + "remote";
            }

            return result;
        }

        [MenuItem("Adrian Miasik/Settings/Delete all SYSTEM settings files")]
        public static void WipeSystemSettings()
        {
            DeleteSteamCloudSystemSettings();
            DeleteLocalSystemSettings();
        }

        /// <summary>
        /// Deletes the Steam Cloud saved system settings.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Steam Cloud/Delete SYSTEM settings file")]
        private static void DeleteSteamCloudSystemSettings()
        {
            if (SteamClient.IsValid)
            {
                // Attempt to remove Steam Cloud save...
                if (SteamRemoteStorage.FileExists(SteamSystemSettingsPath))
                {
                    // Wipe remote storage
                    SteamRemoteStorage.FileDelete(SteamSystemSettingsPath);

                    Debug.Log("Steam Cloud: SYSTEM settings file deleted successfully.");
                }
                else
                {
                    Debug.LogWarning("Steam Cloud: SYSTEM settings file not found.");
                }
            }
            else
            {
                Debug.LogWarning("Steam Client not found. Please init Steam Manager by entering play mode" +
                                 " and try again.");
            }
        }

        /// <summary>
        /// Deletes the Local Storage saved SYSTEM settings.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Local Storage/Delete SYSTEM settings file")]
        private static void DeleteLocalSystemSettings()
        {
            if (File.Exists(Application.persistentDataPath + "/system-settings" + DataExtension))
            {
                File.Delete(Application.persistentDataPath + "/system-settings" + DataExtension);
                
                Debug.Log("Local Storage: SYSTEM settings file deleted successfully.");
            }
            else
            {
                Debug.LogWarning("Local Storage: SYSTEM settings file not found.");
            }
        }

        /// <summary>
        /// Deletes the Local Storage saved TIMER settings.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Local Storage/Delete TIMER settings file")]
        public static void WipeTimerSettings()
        {
            // TODO: Steam Cloud support
            if (File.Exists(Application.persistentDataPath + "/timer-settings" + DataExtension))
            {
                File.Delete(Application.persistentDataPath + "/timer-settings" + DataExtension);
            }
        }
    }
}