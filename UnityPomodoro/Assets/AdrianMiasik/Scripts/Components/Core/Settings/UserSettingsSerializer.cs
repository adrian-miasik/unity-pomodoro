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
    /// Saves/loads our user settings to/from either our persistent data paths (local storage) or Steam cloud (remote
    /// storage). The data we save/load are: (<see cref="SystemSettings"/> and <see cref="TimerSettings"/>).
    /// </summary>
    public static class UserSettingsSerializer
    {
        private const string DataExtension = ".dat";

#if UNITY_EDITOR
#if USER_SETTINGS_EVENT_LOGS
        /// <summary>
        /// Prints all of our Steam cloud remote files into the console.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Steam Cloud (Remote Storage)/Print all Steam cloud files")]
        public static void PrintAllSteamCloudFiles()
        {
            if (SteamClient.IsValid && SteamRemoteStorage.IsCloudEnabled)
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
#endif

        /// <summary>
        /// Deletes/Wipes all of our Steam cloud remote files from the cloud.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Steam Cloud (Remote Storage)/Delete all Steam cloud files")]
        public static void DeleteAllSteamCloudFiles()
        {
            if (SteamClient.IsValid && SteamRemoteStorage.IsCloudEnabled)
            {
                if (SteamRemoteStorage.FileCount <= 0)
                {
#if USER_SETTINGS_EVENT_LOGS
                    Debug.Log("No remote Steam cloud files found.");
#endif
                    return;
                }
                
                foreach (string file in SteamRemoteStorage.Files)
                {
                    SteamRemoteStorage.FileDelete(file);
                }
                
#if USER_SETTINGS_EVENT_LOGS
                Debug.Log("Steam Cloud: All Steam cloud files have been deleted successfully!");
            }
            else
            {
                Debug.LogWarning("Steam Client not found. Please init Steam Manager by entering play mode" +
                                 " and try again.");
#endif
            }
        }
#endif

        /// <summary>
        /// Saves the provided file to both the Steam cloud (remote storage) and local storage.
        /// </summary>
        public static void SaveSettingsFile<T>(T type, string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            string persistentFilePath = Application.persistentDataPath + "/" + fileName + DataExtension;
#if USER_SETTINGS_EVENT_LOGS
            string simpleFilePath = fileName + DataExtension;
#endif

            // If the Steam client is enabled and found...
            if (SteamClient.IsValid && SteamRemoteStorage.IsCloudEnabled)
            {
                // Save the provided type into the Steam cloud (remote storage).
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, type);
#if USER_SETTINGS_EVENT_LOGS
                if (SteamRemoteStorage.FileWrite(simpleFilePath, ms.ToArray()))
                {
                    Debug.Log("Steam Cloud: " + simpleFilePath + " settings file uploaded successfully.");
                }
                else
                {
                    Debug.LogAssertion("Steam Cloud: " + simpleFilePath + " upload has failed.");
                }
#endif
                ms.Close();
            }
            
            // Save file to our local storage.
            FileStream fs = File.Create(persistentFilePath);
            bf.Serialize(fs, type);
            fs.Close();
            
#if USER_SETTINGS_EVENT_LOGS
            Debug.Log("Local Storage: " + simpleFilePath + " settings file saved successfully.");
#endif
        }
        
        private enum MostRecentSaveLocation
        {
            /// <summary>
            /// Unable to determine saves because the Steam client isn't active.
            /// </summary>
            UNABLE_TO_DETERMINE_STEAM_OFFLINE,
            
            /// <summary>
            /// Unable to determine saves because no remote storage was found.
            /// I.e. The Steam Cloud is empty.
            /// </summary>
            UNABLE_TO_DETERMINE_STEAM_CLOUD_EMPTY,

            /// <summary>
            /// Unable to determine saves because no local storage file was found.
            /// </summary>
            UNABLE_TO_DETERMINE_LOCAL_STORAGE_EMPTY,

            /// <summary>
            /// The Local storage contains the most recent save.
            /// </summary>
            LOCAL_STORAGE,

            /// <summary>
            /// The steam cloud contains the most recent save.
            /// </summary>
            STEAM_CLOUD,

            /// <summary>
            /// The local storage and steam cloud were both saved at the same time.
            /// </summary>
            LOCAL_STORAGE_AND_STEAM_CLOUD
        }

        public static T LoadSettings<T>(string fileName) where T : class
        {
            MostRecentSaveLocation recentSave = FetchMostRecentSaveMethod(fileName);
            BinaryFormatter bf = new BinaryFormatter();
            string persistentFilePath = Application.persistentDataPath + "/" + fileName + DataExtension;
            string simpleFilePath = fileName + DataExtension;
            
            switch (recentSave)
            {
                case MostRecentSaveLocation.UNABLE_TO_DETERMINE_STEAM_OFFLINE:
                    // Load local storage save
                    if (File.Exists(persistentFilePath))
                    {
                        FileStream fs = File.Open(persistentFilePath, FileMode.Open);
                        T file = bf.Deserialize(fs) as T;
                        fs.Close();
                
#if USER_SETTINGS_EVENT_LOGS
                        Debug.Log("Local Storage: " + simpleFilePath + " loaded successfully!");
#endif
                        
                        // Return local save
                        return file;
                    }
                    break;
                
                case MostRecentSaveLocation.UNABLE_TO_DETERMINE_STEAM_CLOUD_EMPTY:
                case MostRecentSaveLocation.LOCAL_STORAGE:
                    // Load local storage save
                    if (File.Exists(persistentFilePath))
                    {
                        FileStream fs = File.Open(persistentFilePath, FileMode.Open);
                        T file = bf.Deserialize(fs) as T;
                        fs.Close();
                
#if USER_SETTINGS_EVENT_LOGS
                        Debug.Log("Local Storage: " + simpleFilePath + " loaded successfully!");
#endif
                        
                        // If Steam cloud is running, save local storage to Steam cloud.
                        if (SteamClient.IsValid && SteamRemoteStorage.IsCloudEnabled)
                        {
                            // Overwrite Steam cloud file with local storage version.
                            MemoryStream ms = new MemoryStream();
                            bf.Serialize(ms, file);
#if USER_SETTINGS_EVENT_LOGS
                            if (SteamRemoteStorage.FileWrite(simpleFilePath, ms.ToArray()))
                            {
                                Debug.Log("Local Storage -> Steam Cloud: Saved local storage file " + simpleFilePath 
                                    + " to Steam Cloud for future fallback.");
                            }
                            else
                            {
                                Debug.LogError("Steam Cloud: Failed upload. Unable to upload " + simpleFilePath);
                            }
#endif
                            ms.Close();

                            // Write save again to local storage to match Steam cloud modified time.
                            fs = File.Create(persistentFilePath);
                            bf.Serialize(fs, file);
                            fs.Close();
                        }
                        
                        // Return local save
                        return file;
                    }
                    break;
                
                case MostRecentSaveLocation.UNABLE_TO_DETERMINE_LOCAL_STORAGE_EMPTY:
                case MostRecentSaveLocation.STEAM_CLOUD:
                    // Load Steam cloud save
                    if (SteamClient.IsValid && SteamRemoteStorage.IsCloudEnabled)
                    {
                        // Validate Steam cloud Save data...
                        if (SteamRemoteStorage.FileExists(simpleFilePath))
                        {
                            // Fetch Steam Cloud file...
                            byte[] steamCloudFile = SteamRemoteStorage.FileRead(simpleFilePath);
                            
                            // Convert byte array back into T object
                            MemoryStream ms = new MemoryStream();
                            bf = new BinaryFormatter();
                            ms.Write(steamCloudFile, 0, steamCloudFile.Length);
                            ms.Seek(0, SeekOrigin.Begin);
                            T file = bf.Deserialize(ms) as T;

#if USER_SETTINGS_EVENT_LOGS
                            Debug.Log("Steam Cloud: " + simpleFilePath + " downloaded successfully!");
#endif
                            
                            // Overwrite local storage file with Steam cloud version.
                            FileStream fs = File.Create(persistentFilePath);
                            bf.Serialize(fs, file);
                            fs.Close();
                            
#if USER_SETTINGS_EVENT_LOGS
                            Debug.Log("Steam Cloud -> Local Storage: Saved Steam cloud file " + simpleFilePath + 
                                      " to local storage for future fallback.");
#endif
                            
                            // Write save again to Steam cloud to match local storage modified time.
                            bf.Serialize(ms, file);
                            SteamRemoteStorage.FileWrite(simpleFilePath, ms.ToArray());
                            ms.Close();
                            
                            // Return cloud save
                            return file;
                        }
                    }
                    break;
                
                case MostRecentSaveLocation.LOCAL_STORAGE_AND_STEAM_CLOUD:
                    // Load local storage save
                    if (File.Exists(persistentFilePath))
                    {
                        FileStream fs = File.Open(persistentFilePath, FileMode.Open);
                        T file = bf.Deserialize(fs) as T;
                        fs.Close();
                
#if USER_SETTINGS_EVENT_LOGS
                        Debug.Log("Local Storage: " + simpleFilePath + " loaded successfully!");
#endif

                        // Return local save
                        return file;
                    }
                    break;
            }
            
#if USER_SETTINGS_EVENT_LOGS
            Debug.LogWarning("No " + simpleFilePath + " file could be found.");
#endif
            return null;
        }

        /// <summary>
        /// Calculates and determines which save location was written to last. (Steam Cloud/Local Storage)
        /// See the <see cref="MostRecentSaveLocation"/> enum for details.
        /// </summary>
        /// <returns></returns>
        private static MostRecentSaveLocation FetchMostRecentSaveMethod(string fileName)
        {
            string persistentFilePath = Application.persistentDataPath + "/" + fileName + DataExtension;
            string simpleFilePath = fileName + DataExtension;
            
            if (!SteamClient.IsValid || !SteamRemoteStorage.IsCloudEnabled)
            {
                return MostRecentSaveLocation.UNABLE_TO_DETERMINE_STEAM_OFFLINE;
            }
            
            // Fetch user data remote storage directory
            string steamUserRemoteSystemSettingsPath = FetchUserDataRemoteDirectory() + "\\" + simpleFilePath;
                    
            // Check validity of user data remote storage directory...
            if (File.Exists(steamUserRemoteSystemSettingsPath))
            {
                // Check if local storage exists...
                if (!File.Exists(persistentFilePath))
                {
                    return MostRecentSaveLocation.UNABLE_TO_DETERMINE_LOCAL_STORAGE_EMPTY;
                }
                
                // Cache last accessed / modified file times of our SYSTEM settings files.
                DateTime steamRemoteWriteTime = File.GetLastWriteTime(steamUserRemoteSystemSettingsPath);
                DateTime localStorageWriteTime = File.GetLastWriteTime(persistentFilePath);
                
                // Debug.Log("Steam file time: " + steamRemoteWriteTime);
                // Debug.Log("Local file time: " + localStorageWriteTime);
                
                if (steamRemoteWriteTime > localStorageWriteTime)
                {
#if USER_SETTINGS_EVENT_LOGS
                    Debug.Log("Most recent " + simpleFilePath + " file: Steam Cloud");
#endif
                    return MostRecentSaveLocation.STEAM_CLOUD;
                }

                if (steamRemoteWriteTime.TrimMilliseconds() == localStorageWriteTime.TrimMilliseconds())
                {
#if USER_SETTINGS_EVENT_LOGS
                    Debug.Log("Both Steam cloud & local storage " + simpleFilePath + " files written at the " +
                              "same time.");
#endif
                    return MostRecentSaveLocation.LOCAL_STORAGE_AND_STEAM_CLOUD;
                }
                
#if USER_SETTINGS_EVENT_LOGS
                Debug.Log("Most recent " + simpleFilePath + " file: Local Storage");
#endif
                return MostRecentSaveLocation.LOCAL_STORAGE;
            }

            return MostRecentSaveLocation.UNABLE_TO_DETERMINE_STEAM_CLOUD_EMPTY;
        }

        /// <summary>
        /// Returns the steam userdata remote storage directory.
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

        public static void DeleteSettingsFile(string fileName)
        {
            DeleteRemoteFile(fileName);
            DeleteLocalFile(fileName);
        }
        
        /// <summary>
        /// Deletes the provided Steam cloud file (remote storage).
        /// </summary>
        private static void DeleteRemoteFile(string fileName)
        {
            string simpleFilePath = fileName + DataExtension;
            
            if (SteamClient.IsValid && SteamRemoteStorage.IsCloudEnabled)
            {
                // Attempt to remove Steam Cloud save...
                if (SteamRemoteStorage.FileExists(simpleFilePath))
                {
                    // Wipe remote storage
                    SteamRemoteStorage.FileDelete(simpleFilePath);

#if USER_SETTINGS_EVENT_LOGS
                    Debug.Log("Steam Cloud: " + simpleFilePath + " file deleted successfully.");
                }
                else
                {
                    Debug.LogWarning("Steam Cloud: " + simpleFilePath + " file not found.");
#endif
                }
            }
#if USER_SETTINGS_EVENT_LOGS
            else
            {
                Debug.LogWarning("Steam Client not found. Please init Steam Manager by entering play mode" +
                                 " and try again.");
            }
#endif
        }

        /// <summary>
        /// Deletes the provided local storage file.
        /// </summary>
        private static void DeleteLocalFile(string fileName)
        {
            string persistentFilePath = Application.persistentDataPath + "/" + fileName + DataExtension;
            
#if USER_SETTINGS_EVENT_LOGS
            string simpleFilePath = fileName + DataExtension;
#endif
            
            if (File.Exists(persistentFilePath))
            {
                File.Delete(persistentFilePath);
                
#if USER_SETTINGS_EVENT_LOGS
                Debug.Log("Local Storage: " + simpleFilePath + " file deleted successfully.");
            }
            else
            {
                Debug.LogWarning("Local Storage: "+ simpleFilePath + " file not found.");
#endif
            }
        }
        
#if UNITY_EDITOR
        // Unity Editor Menu Methods
        
        /// <summary>
        /// Deletes all settings files from Steam cloud (remote storage) and local storage.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Delete all settings files")]
        private static void DeleteAllSettingsFiles()
        {
            DeleteSettingsFile("system-settings");
            DeleteSettingsFile("timer-settings");
        }
        
        /// <summary>
        /// Deletes the system settings file from Steam cloud (remote storage) and local storage.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Delete both 'system-settings' files")]
        private static void DeleteAllSystemSettingsFiles()
        {
            DeleteSettingsFile("system-settings");
        }

        /// <summary>
        /// Deletes the system settings file from local storage.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Local Storage/Delete 'system-settings' file")]
        private static void DeleteLocalSystemSettingsFile()
        {
            DeleteLocalFile("system-settings");
        }

        /// <summary>
        /// Deletes the system settings file from the Steam cloud (remote storage).
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Steam Cloud (Remote Storage)/Delete 'system-settings' file")]
        private static void DeleteRemoteSystemSettingsFile()
        {
            DeleteRemoteFile("system-settings");
        }
        
        /// <summary>
        /// Deletes the timer settings file from Steam cloud (remote storage) and local storage.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Delete both 'timer-settings' files")]
        private static void DeleteAllTimerSettingsFiles()
        {
            DeleteSettingsFile("timer-settings");
        }

        /// <summary>
        /// Deletes the timer settings file from local storage.
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Local Storage/Delete 'timer-settings' file")]
        private static void DeleteLocalTimerSettingsFile()
        {
            DeleteLocalFile("timer-settings");
        }

        /// <summary>
        /// Deletes the timer settings file from the Steam cloud (remote storage).
        /// </summary>
        [MenuItem("Adrian Miasik/Settings/Steam Cloud (Remote Storage)/Delete 'timer-settings' file")]
        private static void DeleteRemoteTimerSettingsFile()
        {
            DeleteRemoteFile("timer-settings");
        }
#endif
    }
}