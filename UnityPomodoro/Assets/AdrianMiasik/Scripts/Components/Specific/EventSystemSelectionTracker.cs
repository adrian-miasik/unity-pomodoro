using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AdrianMiasik.Components.Specific
{
    public class EventSystemSelectionTracker : MonoBehaviour
    {
        [SerializeField] private EventSystem m_eventSystem;

        private GameObject m_lastSelectedGameObject;
        public UnityEvent<GameObject, GameObject> OnSelectionChange;

        private void Update()
        {
            GameObject currentSelectedGameObject = m_eventSystem.currentSelectedGameObject;

            if (currentSelectedGameObject != m_lastSelectedGameObject)
            {
                OnSelectionChange?.Invoke(m_lastSelectedGameObject, currentSelectedGameObject);
                m_lastSelectedGameObject = currentSelectedGameObject;
            }
        }
    }
}