using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class WriteVersionNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private string prefixString = "v";

        private void Start()
        {
            text.text = prefixString + Application.version;
        }
    }
}
