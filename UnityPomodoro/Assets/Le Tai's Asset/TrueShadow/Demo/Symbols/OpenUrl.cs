using UnityEngine;
using UnityEngine.UI;

public class OpenUrl : MonoBehaviour
{
    public string buttonURL;
    Button                                      button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button)
            button.onClick.AddListener(() => Open(buttonURL));
    }

    public void Open(string url)
    {
        Application.OpenURL(url);
    }
}
