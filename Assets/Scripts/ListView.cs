using System;
using UnityEngine;

public class ListView : MonoBehaviour 
{
    public RectTransform contentPanelRT_ = null;
    public RectTransform centerRT = null;
    public GameObject baseContent_ = null;
    public int contentLength_ = 0;   
    public int contentDistance_ = 0;
    public int startContent_ = 1;
    public float completeRevise_ = 3.0f;
    public float messageRevise_ = 4.0f;

    private RectTransform[] _contentsRT = null;
    private int _leftmostDataIndex = 0;
    private int _minContentNum = 0;
    private float[] _distance = null;
    private float[] _distReposition = null;
    private float _lerpSpeed = 5f;
    private float _thresholdLeft = 0.0f;
    private float _thresholdRight = 0.0f;
    private bool _isDragging = false;
    private bool _isMessageSend = false;
    private bool _isTargetNearsContent = true;
    private Vector3 _scale = Vector3.one;
    private float _scaleValue = 0.0f;

    private bool _isInit = false;
    private Func<int> GetContentDataCount = null;
    private Action<GameObject, int> UpdateContentView = null;

    public void Initialize(Func<int> getContentDataCount, Action<GameObject, int> updateContentView)
    {
        this.GetContentDataCount = getContentDataCount;
        this.UpdateContentView = updateContentView;

        _distance = new float[contentLength_];
        _distReposition = new float[contentLength_];
        _contentsRT = new RectTransform[contentLength_];

        CreateContent(baseContent_);
        contentPanelRT_.anchoredPosition = new Vector2((startContent_ - 1) * -contentDistance_, 0);

        float panelWidth = contentLength_ * contentDistance_;
        _thresholdLeft = -(panelWidth * 0.5f);
        _thresholdRight = panelWidth * 0.5f;

        _isInit = true;
    }

    public void Update()
    {
        if (!_isInit)
            return;
        
        for (int i = 0; i < contentLength_; i++)
        {
            _distReposition[i] = centerRT.position.x - _contentsRT[i].position.x;
            _distance[i] = Mathf.Abs(_distReposition[i]);

            if (_distReposition[i] > _thresholdRight && _leftmostDataIndex + contentLength_ < GetContentDataCount())
            {
                float curX = _contentsRT[i].anchoredPosition.x;
                float curY = _contentsRT[i].anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX + (contentLength_ * contentDistance_), curY);
                _contentsRT[i].anchoredPosition = newAnchoredPos;
                _leftmostDataIndex++;
                UpdateContentView(_contentsRT[i].gameObject, _leftmostDataIndex + contentLength_ - 1);

            }

            if (_distReposition[i] < _thresholdLeft && _leftmostDataIndex > 0)
            {
                float curX = _contentsRT[i].anchoredPosition.x;
                float curY = _contentsRT[i].anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX - (contentLength_ * contentDistance_), curY);
                _contentsRT[i].anchoredPosition = newAnchoredPos;
                _leftmostDataIndex--;
                UpdateContentView(_contentsRT[i].gameObject, _leftmostDataIndex);
            }

            if (_distance[i] >= contentDistance_)
            {
                _scale = Vector3.one;
            }
            else
            {
                _scaleValue = ((contentDistance_ - _distance[i]) / contentDistance_);
                _scaleValue *= 0.1f;
                _scale = new Vector3(1 + _scaleValue, 1 + _scaleValue, 1);
            }
            _contentsRT[i].localScale = _scale;
        }

        if (_isTargetNearsContent)
        {
            float minDistance = Mathf.Min(_distance);
            for (int a = 0; a < contentLength_; a++)
            {
                if (minDistance == _distance[a])
                {
                    _minContentNum = a;
                }
            }
        }

        if (!_isDragging)
        {
            LerpToContent(-_contentsRT[_minContentNum].anchoredPosition.x);
        }
    }

    private void LerpToContent(float position)
    {
        float newX = Mathf.Lerp(contentPanelRT_.anchoredPosition.x, position, Time.deltaTime * _lerpSpeed);

        if (Mathf.Abs(position - newX) < completeRevise_)
        {
            newX = position;
        }

        if (Mathf.Abs(newX) >= Mathf.Abs(position) - messageRevise_ && Mathf.Abs(newX) <= Mathf.Abs(position) + messageRevise_ && !_isMessageSend)
        {
            _isMessageSend = true;
            SendMessageFromContent(_minContentNum);
        }

        Vector2 newPosition = new Vector2(newX, contentPanelRT_.anchoredPosition.y);
        contentPanelRT_.anchoredPosition = newPosition;
    }

    private void SendMessageFromContent(int contentIndex)
    {
        Debug.Log("SendMessage : " + contentIndex); 
    }

    public void StartDrag()
    {
        _isMessageSend = false;
        _isDragging = true;
        _isTargetNearsContent = true;
    }

    public void EndDrag()
    {
        _isDragging = false; 
    }

    public void GoToContent(int contentIndex)
    {
        _isTargetNearsContent = false;
        _minContentNum = contentIndex - 1;
    }

    public void CreateContent(GameObject baseObject)
    {
        if (baseObject.GetComponent<RectTransform>() == null)
        {
            Debug.LogWarningFormat("There is no Rect Transform component in the scroll view content.");
            baseObject.AddComponent<RectTransform>();
        }

        GameObject go = null;
        for (int i = 0; i < contentLength_; i++)
        {
            go = Instantiate<GameObject>(baseObject);
            _contentsRT[i] = go.GetComponent<RectTransform>();
            _contentsRT[i].SetParent(contentPanelRT_);
            _contentsRT[i].anchoredPosition = new Vector3(i * contentDistance_, 0, 0);
            _contentsRT[i].localRotation = Quaternion.identity;
            _contentsRT[i].localScale = Vector3.one;
            go.SetActive(true);
            UpdateContentView(go, i);
        }
        baseObject.SetActive(false);
    }
}
