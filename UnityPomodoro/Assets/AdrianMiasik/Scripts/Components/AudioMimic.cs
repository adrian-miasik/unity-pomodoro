using UnityEngine;

// Singleton
namespace AdrianMiasik.Components
{
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
