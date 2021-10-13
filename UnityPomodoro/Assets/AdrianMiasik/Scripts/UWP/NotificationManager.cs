using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using UnityEngine.WSA;
#endif

namespace AdrianMiasik.UWP
{
    public class NotificationManager : MonoBehaviour
    {
        // UWP
        [Header("Toast")]
        [SerializeField] private TextAsset xmlToast;

        public void ShowToast()
        {
#if ENABLE_WINMD_SUPPORT
            Toast toast = Toast.Create(xmlToast.text);
            toast.Show();
#endif
        }
    }
}