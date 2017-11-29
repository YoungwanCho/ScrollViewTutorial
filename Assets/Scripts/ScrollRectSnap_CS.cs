using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap_CS : MonoBehaviour 
{
    public Button[] contents_ = null;
    public RectTransform contentPanelRT_ = null;
    public RectTransform centerRT = null;
    public int startContent_ = 1;
    public float completeRevise_ = 3.0f;
    public float messageRevise_ = 4.0f;

    private RectTransform[] _contentsRT = null;
    private float[] distance = null;
    private float[] distReposition = null;
    private float lerpSpeed = 5f;
    private float thresholdLeft = 0.0f;
    private float thresholdRight = 0.0f;
    private int contentDistance = 0;
    private int minContentNum = 0;
    private int contentLength = 0;
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
        for (int i = 0; i < contentLength; i++)
        {
            distReposition[i] = centerRT.position.x - _contentsRT[i].position.x;
            distance[i] = Mathf.Abs(distReposition[i]);

            if (distReposition[i] > thresholdRight)
            {
                float curX = _contentsRT[i].anchoredPosition.x;
                float curY = _contentsRT[i].anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX + (contentLength * contentDistance), curY);
                _contentsRT[i].anchoredPosition= newAnchoredPos;
            }

            if (distReposition[i] < thresholdLeft)
            {
                float curX = _contentsRT[i].anchoredPosition.x;
                float curY = _contentsRT[i].anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX - (contentLength * contentDistance), curY);
                _contentsRT[i].anchoredPosition = newAnchoredPos;
            }

            if (distance[i] >= contentDistance)
            {
                scale = Vector3.one;
            }
            else
            {
                scaleValue = ((contentDistance - distance[i]) / contentDistance);
                scaleValue *= 0.1f;
                scale = new Vector3(1 + scaleValue, 1 + scaleValue, 1);
            }
            _contentsRT[i].localScale = scale;
        }

        if (isTargetNearsContent)
        {
            float minDistance = Mathf.Min(distance);
            for (int a = 0; a < contentLength; a++)
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
        contentLength = contents_.Length;
        distance = new float[contentLength];
        distReposition = new float[contentLength];
        _contentsRT = new RectTransform[contentLength];

        for (int i = 0; i < contentLength; i++)
        {
            _contentsRT[i] = contents_[i].GetComponent<RectTransform>();
        }

        contentDistance = (int)Mathf.Abs(_contentsRT[1].anchoredPosition.x - _contentsRT[0].anchoredPosition.x);
        contentPanelRT_.anchoredPosition = new Vector2((startContent_ - 1) * -contentDistance, 0);

        float panelWidth = contentLength * contentDistance;
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
}
