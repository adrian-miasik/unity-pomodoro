using TMPro;
using UnityEngine;

namespace AdrianMiasik
{
    public class WriteVersionNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void Start()
        {
            text.text = Application.version;
        }
    }
}
