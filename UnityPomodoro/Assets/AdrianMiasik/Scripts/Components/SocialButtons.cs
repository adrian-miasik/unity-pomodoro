using System.Collections.Generic;
using AdrianMiasik.Components.Core;
using UnityEngine;

namespace AdrianMiasik.Components
{
    public class SocialButtons : MonoBehaviour
    {
        [SerializeField] private List<ClickButton> buttons = new List<ClickButton>();
    }
}