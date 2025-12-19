/// Credit Tomasz Schelenz 
/// Sourced from - https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/issues/81/infinite-scrollrect
/// Demo - https://www.youtube.com/watch?v=uVTV7Udx78k  - configures automatically.  - works in both vertical and horizontal (but not both at the same time)  - drag and drop  - can be initialized by code (in case you populate your scrollview content from code)
/// Updated by Febo Zodiaco - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/349/magnticinfinitescroll

using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Infinite scroll view with automatic configuration 
    /// 
    /// Fields
    /// - InitByUSer - in case your scrollrect is populated from code, you can explicitly Initialize the infinite scroll after your scroll is ready
    /// by calling Init() method
    /// 
    /// Notes
    /// - does not work in both vertical and horizontal orientation at the same time.
    /// - in order to work it disables layout components and size fitter if present(automatically)
    /// 
    /// </summary>
    [AddComponentMenu("UI/Extensions/UI Infinite Scroll")]
    public class UI_InfiniteScroll : MonoBehaviour
    {
        public Action<GameObject, int> OnItemVisible; // 콜백을 외부에서 등록
        
        //if true user will need to call Init() method manually (in case the contend of the scrollview is generated from code or requires special initialization)
        [Tooltip("If false, will Init automatically, otherwise you need to call Init() method")]
        public bool InitByUser = false;
        protected ScrollRect _scrollRect;
        private ContentSizeFitter _contentSizeFitter;
        private VerticalLayoutGroup _verticalLayoutGroup;
        private HorizontalLayoutGroup _horizontalLayoutGroup;
        private GridLayoutGroup _gridLayoutGroup;
        protected bool _isVertical = false;
        protected bool _isHorizontal = false;
        private float _disableMarginX = 0;
        private float _disableMarginY = 0;
        private bool _hasDisabledGridComponents = false;
        protected List<RectTransform> items = new List<RectTransform>();
        private Vector2 _newAnchoredPosition = Vector2.zero;
        //TO DISABLE FLICKERING OBJECT WHEN SCROLL VIEW IS IDLE IN BETWEEN OBJECTS
        private float _threshold = 100f;
        private int _itemCount = 0;
        private float _recordOffsetX = 0;
        private float _recordOffsetY = 0;
        private int _loopIndexOffset = 0; // 추가: 순환 시 인덱스 조정용

        protected virtual void Awake()
        {
            if (!InitByUser)
                Init();
        }

        public virtual void SetNewItems(ref List<Transform> newItems)
        {
            if (_scrollRect != null)
            {
                if (_scrollRect.content == null && newItems == null)
                {
                    return;
                }

                if (items != null)
                {
                    items.Clear();
                }

                for (int i = _scrollRect.content.childCount - 1; i >= 0; i--)
                {
                    Transform child = _scrollRect.content.GetChild(i);
                    child.SetParent(null);
                    GameObject.DestroyImmediate(child.gameObject);
                }

                foreach (Transform newItem in newItems)
                {
                    newItem.SetParent(_scrollRect.content);
                }

                SetItems();
            }
        }

        private void SetItems()
        {
            for (int i = 0; i < _scrollRect.content.childCount; i++)
            {
                items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
            }

            _itemCount = _scrollRect.content.childCount;
        }

        public void Init()
        {
            if (GetComponent<ScrollRect>() != null)
            {
                _scrollRect = GetComponent<ScrollRect>();
                _scrollRect.onValueChanged.AddListener(OnScroll);
                _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

                if (_scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
                {
                    _verticalLayoutGroup = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
                {
                    _horizontalLayoutGroup = _scrollRect.content.GetComponent<HorizontalLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<GridLayoutGroup>() != null)
                {
                    _gridLayoutGroup = _scrollRect.content.GetComponent<GridLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<ContentSizeFitter>() != null)
                {
                    _contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
                }

                _isHorizontal = _scrollRect.horizontal;
                _isVertical = _scrollRect.vertical;

                if (_isHorizontal && _isVertical)
                {
                    Debug.LogError("UI_InfiniteScroll doesn't support scrolling in both directions, please choose one direction (horizontal or vertical)");
                }

                SetItems();
            }
            else
            {
                Debug.LogError("UI_InfiniteScroll => No ScrollRect component found");
            }
        }

        void DisableGridComponents()
        {
            if (_isVertical)
            {
                _recordOffsetY = items[1].GetComponent<RectTransform>().anchoredPosition.y - items[0].GetComponent<RectTransform>().anchoredPosition.y;
                if (_recordOffsetY < 0)
                {
                    _recordOffsetY *= -1;
                }
                _disableMarginY = _recordOffsetY * _itemCount / 2;
            }
            if (_isHorizontal)
            {
                _recordOffsetX = items[1].GetComponent<RectTransform>().anchoredPosition.x - items[0].GetComponent<RectTransform>().anchoredPosition.x;
                if (_recordOffsetX < 0)
                {
                    _recordOffsetX *= -1;
                }
                _disableMarginX = _recordOffsetX * _itemCount / 2;
            }

            if (_verticalLayoutGroup)
            {
                _verticalLayoutGroup.enabled = false;
            }
            if (_horizontalLayoutGroup)
            {
                _horizontalLayoutGroup.enabled = false;
            }
            if (_contentSizeFitter)
            {
                _contentSizeFitter.enabled = false;
            }
            if (_gridLayoutGroup)
            {
                _gridLayoutGroup.enabled = false;
            }
            _hasDisabledGridComponents = true;
        }
        
        
        public void OnScroll(Vector2 pos)
        {
            if (!_hasDisabledGridComponents)
                DisableGridComponents();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var viewPos = _scrollRect.transform.InverseTransformPoint(item.position);

                if (_isVertical)
                {
                    if (viewPos.y > _disableMarginY + _threshold)
                    {
                        // Recycle to bottom
                        _newAnchoredPosition = item.anchoredPosition;
                        _newAnchoredPosition.y -= _itemCount * _recordOffsetY;
                        item.anchoredPosition = _newAnchoredPosition;
                        item.SetAsFirstSibling();
                        _loopIndexOffset--;
                    }
                    else if (viewPos.y < -_disableMarginY)
                    {
                        // Recycle to top
                        _newAnchoredPosition = item.anchoredPosition;
                        _newAnchoredPosition.y += _itemCount * _recordOffsetY;
                        item.anchoredPosition = _newAnchoredPosition;
                        item.SetAsLastSibling();
                        _loopIndexOffset++;
                    }
                }

                // ✅ 여기서 Viewport 안에 들어온 경우 콜백 호출
                if (Mathf.Abs(viewPos.y) < _disableMarginY) // 즉, 중심에 가까운 영역일 때
                {
                    int realIndex = i + (_loopIndexOffset * _itemCount);
                    OnItemVisible?.Invoke(item.gameObject, realIndex);

                    // 또는 인터페이스로도 가능
                    item.GetComponent<IInfiniteScrollItem>()?.RefreshData(realIndex);
                }
            }
        }
    }
}

public interface IInfiniteScrollItem
{
    void RefreshData(int index);
}
