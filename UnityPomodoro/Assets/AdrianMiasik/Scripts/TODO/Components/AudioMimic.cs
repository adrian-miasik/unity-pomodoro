using UnityEngine;

// Singleton
namespace AdrianMiasik.Components
{
    public class AudioMimic : MonoBehaviour
    {
        private static AudioMimic instance;
        public static AudioMimic Instance => instance;

        [SerializeField] private AudioSource source;

        private void Awake()
        {
            // If an instance had already been set and if the instance is not this...
            if (instance != null && instance != this)
            {
                // Remove this instance
                Destroy(gameObject);
                return;
            }

            // Otherwise, treat this as our instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    
        public void PlaySound(AudioClip _clipToMimic)
        {
            source.Stop();
            source.clip = _clipToMimic;
            source.Play();
        }
    }
}
