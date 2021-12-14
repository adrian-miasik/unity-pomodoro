using AdrianMiasik.Components.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdrianMiasik.Components
{
    public class TwoChoiceDialog : MonoBehaviour
    {
        [SerializeField] private Image backgroundBox;
        [SerializeField] private TMP_Text topLabel;
        [SerializeField] private TMP_Text botLabel;
        [SerializeField] private ClickButton cancel;
        [SerializeField] private ClickButton submit;

        public void Initialize()
        {
            
        }
    }
}
