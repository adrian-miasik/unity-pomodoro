using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdrianMiasik.Editor
{
    public class CloudBuildRenamer : EditorWindow
    {
        [SerializeField] private VisualTreeAsset uxmlTree;

        private TextField unityCloudBuildsDirectoryTextField;
        private string unityCloudBuildsDirectory;
        
        [MenuItem("Window/UI Toolkit/CloudBuildRenamer")]
        public static void ShowExample()
        {
            CloudBuildRenamer wnd = GetWindow<CloudBuildRenamer>();
            wnd.titleContent = new GUIContent("Cloud Build Renamer");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            
            // Import UXML
            rootVisualElement.Add(uxmlTree.Instantiate());

            // Cache text field
            TextField ucbDirectoryField = rootVisualElement.Q<TextField>("unity-cloud-build-dir");
            unityCloudBuildsDirectoryTextField = ucbDirectoryField;

            // Hook up browse button on click
            VisualElement browseButton = rootVisualElement.Q<Button>("browse-button");
            browseButton.RegisterCallback<ClickEvent>(BrowseButtonOnClick);
            
            // Hook up rename button on click
            VisualElement renameButton = rootVisualElement.Q<Button>("rename-button");
            renameButton.RegisterCallback<ClickEvent>(RenameButtonOnClick);
        }
        
        /// <summary>
        /// Invoked when the browse button is pressed/clicked.
        /// </summary>
        /// <param name="evt"></param>
        private void BrowseButtonOnClick(ClickEvent evt)
        {
            unityCloudBuildsDirectory = EditorUtility.OpenFolderPanel("Unity Cloud Builds Directory", "", "");
            unityCloudBuildsDirectoryTextField.SetValueWithoutNotify(unityCloudBuildsDirectory);
        }
        
        /// <summary>
        /// Invoked when the rename button is pressed/clicked.
        /// </summary>
        /// <param name="evt"></param>
        private void RenameButtonOnClick(ClickEvent evt)
        {
            int numberOfFilesRenamed = 0;
            
            if (!string.IsNullOrEmpty(unityCloudBuildsDirectory))
            {
                Debug.Log("Executing UCB rename on path: " + unityCloudBuildsDirectory);
                
                // Fetch zip files
                string[] files = Directory
                    .EnumerateFiles(unityCloudBuildsDirectory, "*.*")
                    .Where(file => file.ToLower().EndsWith("zip"))
                    .ToArray();
                
                foreach (string s in files)
                {
                    // Copy path
                    string correctedName = s;
                    
                    // Remove .zip extension
                    correctedName = correctedName.Replace(".zip", "");
                    
                    // Remove username
                    correctedName = correctedName.Replace("adrian-miasik-", "");

                    // Replace 'default' keyword with version number
                    correctedName = correctedName.Replace("default", Application.version);

                    // Remove trailing digits
                    char[] digits = new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                    correctedName = correctedName.TrimEnd(digits);
                    
                    // If we have a single trailing 'space' after our digit trim...
                    if (correctedName[^1].CompareTo('-') == 0)
                    {
                        // Remove extra trailing 'space'
                        correctedName = correctedName.TrimEnd('-');
                    }
                    
                    // Re-add extension
                    correctedName += ".zip";

                    if (s != correctedName)
                    {
                        Debug.Log("Renaming: '" +  s + "' to '" + correctedName + "'.");
                        File.Move(s, correctedName);
                        numberOfFilesRenamed++;
                    }
                }

                if (numberOfFilesRenamed > 0)
                {
                    Debug.Log("Unity Cloud Builds have been renamed successfully! (" + numberOfFilesRenamed 
                        + ")");
                }
                else
                {
                    Debug.LogWarning("No Unity Cloud Build naming conventions found.");
                }
            }
            else
            {
                Debug.LogWarning("No asset path provided!");
            }
        }
    }
}