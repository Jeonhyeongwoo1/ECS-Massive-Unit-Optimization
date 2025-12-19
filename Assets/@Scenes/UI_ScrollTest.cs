using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public interface IInfiniteScrollItem
{
    void RefreshData(int index);
}

public class UI_ScrollTest : MonoBehaviour
{   
    public ScrollRect scrollRect;
    public GameObject cellPrefab;
    public int cellCount = 10;

    private List<Transform> itemList = new List<Transform>();

    void Start()
    {
        // Content 안에 미리 셀 추가
        for (int i = 0; i < cellCount; i++)
        {
            GameObject go = Instantiate(cellPrefab, scrollRect.content);
            // go.GetComponentInChildren<Text>().text = $"Item {i}";
            itemList.Add(go.transform);
        }

        // 무한 스크롤 초기화
        var infiniteScroll = scrollRect.GetComponent<UI_InfiniteScroll>();
        infiniteScroll.Init();

        // 선택사항: SetNewItems()를 호출해서 셀 재배열 가능
        // infiniteScroll.SetNewItems(ref itemList);
    }
}