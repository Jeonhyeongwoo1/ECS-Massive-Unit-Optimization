using System;
using UnityEngine;

public class UI_BlockCanvas : MonoBehaviour
{
    private static UI_BlockCanvas _uiBlockCanvas;

    public static UI_BlockCanvas I
    {
        get
        {
            if (_uiBlockCanvas == null)
            {
                _uiBlockCanvas = FindAnyObjectByType<UI_BlockCanvas>();
            }

            return _uiBlockCanvas;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _uiBlockCanvas = this;
        gameObject.SetActive(false);
    }

    public void ShowAndHideBlockCanvas(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}
