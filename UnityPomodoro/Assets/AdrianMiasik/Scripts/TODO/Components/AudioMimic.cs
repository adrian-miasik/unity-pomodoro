using UnityEngine;

// Singleton
namespace AdrianMiasik.Components
{
    public class AudioMimic : MonoBehaviour
    {
        private static AudioMimic _instance;
        public static AudioMimic Instance => _instance;

        [SerializeField] private AudioSource m_source;

        private void Awake()
        {
            // If an instance had already been set and if the instance is not this...
            if (_instance != null && _instance != this)
            {
                // Remove this instance
                Destroy(gameObject);
                return;
            }

            // Otherwise, treat this as our instance
            _instance = this;
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
