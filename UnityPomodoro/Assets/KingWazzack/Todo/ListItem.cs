using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KingWazzack.Todo
{
    /// <summary>
    /// Todo ListItem Functionality.
    /// Gets data on creation through parameters and can destroy itself
    /// </summary>
    public class ListItem : MonoBehaviour
    {
        public Button doneButton;
        public void Initialize(string todoString)
        {
            TextMeshProUGUI listItemText = gameObject.transform.Find("ListItemText").GetComponent<TextMeshProUGUI>();
            listItemText.text = todoString;
        }

        private void Start()
        {
            doneButton.onClick.AddListener(DestroySelf);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

    }
}