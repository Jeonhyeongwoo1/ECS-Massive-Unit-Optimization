using UnityEngine;
using UnityEngine.UI;

public class BottomTestButton : MonoBehaviour
{
    public string title;
    public BottomUITest bottomUITest;
    Button btn;
    void Start()
    {
        btn = this.gameObject.GetComponent<Button>();
        btn.onClick.AddListener(GetBtn);
    }

    void GetBtn()
    {
        bottomUITest.AnimPlay(title);
    }

}
