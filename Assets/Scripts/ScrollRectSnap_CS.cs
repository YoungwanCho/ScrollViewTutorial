using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap_CS : MonoBehaviour 
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
    private float[] distance = null;
    private float[] distReposition = null;
    private float lerpSpeed = 5f;
    private float thresholdLeft = 0.0f;
    private float thresholdRight = 0.0f;

    private int minContentNum = 0;
    private bool isDragging = false;
    private bool isMessageSend = false;
    private bool isTargetNearsContent = true;

    private Vector3 scale = Vector3.one;
    private float scaleValue = 0.0f;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        for (int i = 0; i < contentLength_; i++)
        {
            distReposition[i] = centerRT.position.x - _contentsRT[i].position.x;
            distance[i] = Mathf.Abs(distReposition[i]);

            if (distReposition[i] > thresholdRight)
            {
                float curX = _contentsRT[i].anchoredPosition.x;
                float curY = _contentsRT[i].anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX + (contentLength_ * contentDistance_), curY);
                _contentsRT[i].anchoredPosition= newAnchoredPos;
            }

            if (distReposition[i] < thresholdLeft)
            {
                float curX = _contentsRT[i].anchoredPosition.x;
                float curY = _contentsRT[i].anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX - (contentLength_ * contentDistance_), curY);
                _contentsRT[i].anchoredPosition = newAnchoredPos;
            }

            if (distance[i] >= contentDistance_)
            {
                scale = Vector3.one;
            }
            else
            {
                scaleValue = ((contentDistance_ - distance[i]) / contentDistance_);
                scaleValue *= 0.1f;
                scale = new Vector3(1 + scaleValue, 1 + scaleValue, 1);
            }
            _contentsRT[i].localScale = scale;
        }

        if (isTargetNearsContent)
        {
            float minDistance = Mathf.Min(distance);
            for (int a = 0; a < contentLength_; a++)
            {
                if (minDistance == distance[a])
                {
                    minContentNum = a;
                }
            }
        }

        if (!isDragging)
        {
            LerpToContent(-_contentsRT[minContentNum].anchoredPosition.x);
        }
    }

    private void Initialize()
    {
        distance = new float[contentLength_];
        distReposition = new float[contentLength_];
        _contentsRT = new RectTransform[contentLength_];

        CreateContent(baseContent_);
        contentPanelRT_.anchoredPosition = new Vector2((startContent_ - 1) * -contentDistance_, 0);

        float panelWidth = contentLength_ * contentDistance_;
        thresholdLeft = -(panelWidth * 0.5f);
        thresholdRight = panelWidth * 0.5f;
    }

    private void LerpToContent(float position)
    {
        float newX = Mathf.Lerp(contentPanelRT_.anchoredPosition.x, position, Time.deltaTime * lerpSpeed);

        if (Mathf.Abs(position - newX) < completeRevise_)
        {
            newX = position;
        }

        if (Mathf.Abs(newX) >= Mathf.Abs(position) - messageRevise_ && Mathf.Abs(newX) <= Mathf.Abs(position) + messageRevise_ && !isMessageSend)
        {
            isMessageSend = true;
            SendMessageFromContent(minContentNum);
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
        isMessageSend = false;
        isDragging = true;
        isTargetNearsContent = true;
    }

    public void EndDrag()
    {
        isDragging = false; 
    }

    public void GoToContent(int contentIndex)
    {
        isTargetNearsContent = false;
        minContentNum = contentIndex - 1;
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
        }
        GameObject.Destroy(baseObject);
    }
}
