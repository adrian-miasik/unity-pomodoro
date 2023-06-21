using System;
using AdrianMiasik.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AdrianMiasik.Components.Base
{
    /// <summary>
    /// A <see cref="ThemeElement"/> page that displays/hides it's associated content.
    /// <remarks>
    /// Intended to be used with <see cref="ItemSelector{T}"/>, also see <seealso cref="SidebarPages"/>.
    /// </remarks>
    /// </summary>
    public class Page : ThemeElement
    {
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private Animation m_pageTurning;
        [SerializeField] private AnimationClip m_pageEntry;
        [SerializeField] private AnimationClip m_pageExit;

        private Action onPageAnimCompletion;

        private bool isAnimating;
        public bool isOpen;

        private void Update()
        {
            if (!IsInitialized())
            {
                return;
            }

            if (isAnimating)
            {
                if (!m_pageTurning.isPlaying)
                {
                    isAnimating = false;
                    
                    onPageAnimCompletion?.Invoke();

                    if (m_pageTurning.clip == m_pageExit)
                    {
                        gameObject.SetActive(false);
                    }
                    onPageAnimCompletion = null;
                }
            }
        }

        public override void ColorUpdate(Theme theme)
        {
#if !UNITY_EDITOR // Ignore for Unity Editor for our Media Creator
            // Skip the color update if this page isn't open.
            if (!isOpen)
            {
                return;
            }
#endif

            m_title.color = theme.GetCurrentColorScheme().m_foreground;
        }

        public virtual void Refresh()
        {
            if (!IsPageOpen())
            {
                return;
            }
            ColorUpdate(Timer.GetTheme());
        }
        
        /// <summary>
        /// Displays this page to the user.
        /// </summary>
        /// <param name="onAnimationCompletion">What do you want to do when the page turning animation is completed?</param>
        public virtual void Show(Action onAnimationCompletion)
        {
            gameObject.SetActive(true);

            onPageAnimCompletion = () =>
            {
                onAnimationCompletion?.Invoke();
            };

            m_pageTurning.clip = m_pageEntry;
            m_pageTurning.Play();
            isAnimating = true;
            isOpen = true;
            
            ColorUpdate(Timer.GetTheme());
        }
        
        /// <summary>
        /// Hides this page away from the user.
        /// </summary>
        /// <param name="onAnimationCompletion">What do you want to do when the page turning animation is completed?</param>
        public virtual void Hide(Action onAnimationCompletion)
        {
            onPageAnimCompletion = () =>
            {
                onAnimationCompletion?.Invoke();
            };
            
            m_pageTurning.clip = m_pageExit;
            m_pageTurning.Play();
            isAnimating = true;
            isOpen = false;
        }
        
        /// <summary>
        /// Is this page currently open and visible?
        /// </summary>
        /// <returns></returns>
        public bool IsPageOpen()
        {
            return isOpen;
        }
    }
}