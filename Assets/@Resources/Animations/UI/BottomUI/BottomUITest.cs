using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BottomUITest : MonoBehaviour
{
    Animator animator;
    [SerializeField] BottomTestButton[] testButton;
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();

        for (int i = 0; i < testButton.Length; i++)
        {
            testButton[i].bottomUITest = this;
        }
    }

    public void AnimPlay(string _title)
    {
        animator.Play(_title);
    }
}
