using UnityEngine;

namespace AdrianMiasik.Components.Core
{
    /// <summary>
    /// A Singleton class that allows us to transfer one <see cref="AudioClip"/> from an about to be hidden/destroyed
    /// gameobject and play it here instead. Limited to only 1 sound mimic at the moment.
    /// </summary>
    public class AudioMimic : MonoBehaviour
    {
        public static AudioMimic Instance { get; private set; }

        [SerializeField] private AudioSource m_source;

        private void Awake()
        {
            // If an instance had already been set and if the instance is not this...
            if (Instance != null && Instance != this)
            {
                // Remove this instance
                Destroy(gameObject);
                return;
            }

            // Otherwise, treat this as our instance
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    
        public void PlaySound(AudioClip clipToMimic)
        {
            m_source.Stop();
            m_source.clip = clipToMimic;
            m_source.Play();
        }
    }
}
