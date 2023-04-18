using AdrianMiasik.Components.Specific.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AdrianMiasik
{
    public class AddressablesManager : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<UnityEngine.AudioClip> m_addressableAudio;

        [SerializeField] private OptionSetAlarmSound m_optionSetAlarmSound;

        public void Initialize()
        {
            // Load custom audio
            AsyncOperationHandle<AudioClip> addressableHandle = m_addressableAudio.LoadAssetAsync<AudioClip>();
            addressableHandle.Completed += AddressableHandleOnCompleted;
        }

        private void AddressableHandleOnCompleted(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Succeeded in loading your addressable: " + handle.Result.name);
                m_optionSetAlarmSound.AddCustomDropdownSoundOption(handle.Result, "my_custom_sound");
            }
            else
            {
                Debug.Log("Failed to load your addressables.");
            }
        }
    }
}