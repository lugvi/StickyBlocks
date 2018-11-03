using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Mask))]
[RequireComponent(typeof(ScrollRect))]
public class ScrollSnapRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    
    private int startingPage;

    // Threshold time for fast swipe in seconds
    public float fastSwipeThresholdTime = 0.3f;

    // Threshold time for fast swipe in (unscaled) pixels
    public int fastSwipeThresholdDistance = 100;

    //How fast will page lerp to target position
    public float decelerationRate = 10f;


    // fast swipes should be fast and short. If too long, then it is not fast swipe
    private int _fastSwipeThresholdMaxLimit;

    private ScrollRect _scrollRectComponent;
    private RectTransform _scrollRectRect;
    private RectTransform _container;

    // private bool _horizontal;

    // number of pages in container
    private int _pageCount;
    public static int _currentPage;

    // whether lerping is in progress and target lerp position
    private bool _lerp;
    private Vector2 _lerpTo;

    // target position of every page
    private List<Vector2> _pagePositions = new List<Vector2>();

    // in draggging, when dragging started and where it started
    private bool _dragging;
    private float _timeStamp;
    private Vector2 _startPosition;

    // for showing small page icons
    private bool _showPageSelection = false;
    private int _previousPageSelectionIndex;
    // container with Image components - one Image for each page
    private List<Image> _pageSelectionImages;


    private static ScrollSnapRect instance;

    public static ScrollSnapRect Instance { get { return instance; } }

    void Awake()
    {
        instance = this;
        Screen.orientation = ScreenOrientation.Portrait;
        // Debug.Log("SCREEN " +Screen.orientation);
        // Screen.autorotateToPortrait = true;
    }


    void Start()
    {
        // if(PrefsManager.LastWorld > 0){
        //     startingPage = PrefsManager.LastWorld;
        //     Debug.Log(startingPage + "WAT?");
        // }else{
            startingPage = 1;
            SetLastPage();
        // }

        _scrollRectComponent = GetComponent<ScrollRect>();
        _scrollRectRect = GetComponent<RectTransform>();
        _container = _scrollRectComponent.content;
        _pageCount = _container.childCount;

        // // is it horizontal or vertical scrollrect
        // if (_scrollRectComponent.horizontal && !_scrollRectComponent.vertical)
        // {
        //     _horizontal = true;
        // }
        // else if (!_scrollRectComponent.horizontal && _scrollRectComponent.vertical)
        // {
        //     _horizontal = false;
        // }
        // else
        // {
        //     Debug.LogWarning("Confusing setting of horizontal/vertical direction. Default set to horizontal.");
        //     _horizontal = true;
        // }

        _lerp = false;

        // init
        SetPagePositions();
        SetPage(startingPage);
        SetPageSelection(startingPage);

    }


    void Update()
    {
        // if moving to target position
        if (_lerp)
        {
            // prevent overshooting with values greater than 1
            float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
            _container.anchoredPosition = Vector2.Lerp(_container.anchoredPosition, _lerpTo, decelerate);
            // time to stop lerping?
            if (Vector2.SqrMagnitude(_container.anchoredPosition - _lerpTo) < 0.1f)
            {
                // snap to target and stop lerping
                _container.anchoredPosition = _lerpTo;
                _lerp = false;
                // clear also any scrollrect move that may interfere with our lerping
                _scrollRectComponent.velocity = Vector2.zero;
            }

            // switches selection icon exactly to correct page
            if (_showPageSelection)
            {
                SetPageSelection(GetNearestPage());
            }
        }

        // if(_currentPage == 1 && !particlesPlayed){
        //     particlesPlayed = true;
        //     player.SetActive(true);
        //     player.GetComponent<ParticleSystem>().Play();
        // }
    }


    private void SetPagePositions()
    {
        int width = 0;
        int height = 0;
        int offsetX = 0;
        int containerWidth = 0;
        int containerHeight = 0;
        
        Debug.Log("SCREEN " +Screen.orientation);


        width = (int) _scrollRectRect.rect.width;

        offsetX = width / 2 - (int)Screen.width / 2;

        containerWidth = width * _pageCount - (int) Screen.width;


        height = (int) _scrollRectRect.rect.height;
        containerHeight = height;



        _fastSwipeThresholdMaxLimit = width;


        // set width of container
        // Vector2 newSize = new Vector2(Screen.width*_pageCount, Screen.height);
        // _container.sizeDelta = newSize;
        Vector2 newPosition = new Vector2(containerWidth / 2, containerHeight / 2);
        _container.anchoredPosition = newPosition;

        // delete any previous settings
        _pagePositions.Clear();

        // iterate through all container childern and set their positions
        for (int i = 0; i < _pageCount; i++)
        {
            RectTransform child = _container.GetChild(i).GetComponent<RectTransform>();
            Vector2 childPosition;

            childPosition = new Vector2(i * width - containerWidth / 2 + offsetX, 0f);

            child.sizeDelta = new Vector2(width, height);
            
            child.anchoredPosition = childPosition;

            _pagePositions.Add(-childPosition);
        }
    }


    private void SetPage(int aPageIndex)
    {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        _container.anchoredPosition = _pagePositions[aPageIndex];
        _currentPage = aPageIndex;
    }


    private void LerpToPage(int aPageIndex)
    {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        _lerpTo = _pagePositions[aPageIndex];
        _lerp = true;
        _currentPage = aPageIndex;
    }

    private void SetPageSelection(int aPageIndex)
    {
        // nothing to change
        if (_previousPageSelectionIndex == aPageIndex)
        {
            return;
        }

        // // unselect old
        // if (_previousPageSelectionIndex >= 0) {
        //     _pageSelectionImages[_previousPageSelectionIndex].sprite = unselectedPage;
        //     _pageSelectionImages[_previousPageSelectionIndex].SetNativeSize();
        // }

        // // select new
        // _pageSelectionImages[aPageIndex].sprite = selectedPage;
        // _pageSelectionImages[aPageIndex].SetNativeSize();

        _previousPageSelectionIndex = aPageIndex;
    }


    public void NextScreen()
    {
        LerpToPage(_currentPage + 1);
    }


    public void PreviousScreen()
    {
        LerpToPage(_currentPage - 1);
    }


    private int GetNearestPage()
    {
        // based on distance from current position, find nearest page
        Vector2 currentPosition = _container.anchoredPosition;

        float distance = float.MaxValue;
        int nearestPage = _currentPage;

        for (int i = 0; i < _pagePositions.Count; i++)
        {
            float testDist = Vector2.SqrMagnitude(currentPosition - _pagePositions[i]);
            if (testDist < distance)
            {
                distance = testDist;
                nearestPage = i;
            }
        }

        return nearestPage;
    }

    public void SetLastPage()
    {
        //GameManager.Instance.LastMainMenuState = _currentPage;
    }


    public void OnBeginDrag(PointerEventData aEventData)
    {
        // if currently lerping, then stop it as user is draging

        _lerp = false;
        // not dragging yet
        _dragging = false;

    }


    public void OnEndDrag(PointerEventData aEventData)
    {
        // how much was container's content dragged
        float difference;
       
            difference = _startPosition.x - _container.anchoredPosition.x;


        // test for fast swipe - swipe that moves only +/-1 item
        if (Time.unscaledTime - _timeStamp < fastSwipeThresholdTime &&
            Mathf.Abs(difference) > fastSwipeThresholdDistance &&
            Mathf.Abs(difference) < _fastSwipeThresholdMaxLimit)
        {
            if (difference > 0)
            {
                NextScreen();
            }
            else
            {
                PreviousScreen();
            }
        }
        else
        {
            // if not fast time, look to which page we got to
            LerpToPage(GetNearestPage());
        }

        _dragging = false;
    }


    public void OnDrag(PointerEventData aEventData)
    {
        if (!_dragging)
        {
            // dragging started
            _dragging = true;
            // save time - unscaled so pausing with Time.scale should not affect it
            _timeStamp = Time.unscaledTime;
            // save current position of cointainer
            _startPosition = _container.anchoredPosition;
        }
        else
        {
            if (_showPageSelection)
            {
                SetPageSelection(GetNearestPage());
            }
        }
    }

}
