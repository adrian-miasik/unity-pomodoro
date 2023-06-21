using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AdrianMiasik.Components.Core.Todo
{
    /// <summary>
    /// This manager adds ListItems to the TodoList
    /// </summary>

public class TodoListManager : MonoBehaviour
{
    public GameObject listItemPrefab;
    public Transform contentTransform;
    public Button addButton;
    public TMP_InputField inputField;
    private VerticalLayoutGroup layoutGroup;

    private void Start()
    {
        layoutGroup = contentTransform.GetComponent<VerticalLayoutGroup>();
        addButton.onClick.AddListener(AddListItem);
    }

    private void AddListItem()
    {
        string inputText = inputField.text;
        GameObject newListItem = Instantiate(listItemPrefab, contentTransform);
        ListItem ListItem = newListItem.GetComponent<ListItem>();
        ListItem.Initialize(inputText);
    }
}
}