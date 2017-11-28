using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap_CS : MonoBehaviour 
{
    public Button[] contents;
    public RectTransform contentPanelRT;
    public RectTransform centerRT;
    public int startContent = 1;
    public float completeRevise = 3.0f;
    public float messageRevise = 4.0f;

    private float[] distance;
    private float[] distReposition;
    private float lerpSpeed = 5f;
    private float thresholdLeft = 0.0f;
    private float thresholdRight = 0.0f;
    private int contentDistance;
    private int minContentNum;
    private int contentLength;
    private bool isDragging = false;
    private bool isMessageSend = false;
    private bool isTargetNearsButton = true;


    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        for (int i = 0; i < contents.Length; i++)
        {
            distReposition[i] = centerRT.position.x - contents[i].GetComponent<RectTransform>().position.x;
            distance[i] = Mathf.Abs(distReposition[i]);

            if (distReposition[i] > thresholdRight)
            {
                float curX = contents[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = contents[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX + (contentLength * contentDistance), curY);
                contents[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPos;
            }

            if (distReposition[i] < thresholdLeft)
            {
                float curX = contents[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = contents[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX - (contentLength * contentDistance), curY);
                contents[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPos;
            }
        }

        if (isTargetNearsButton)
        {
            float minDistance = Mathf.Min(distance);

            for (int a = 0; a < contents.Length; a++)
            {
                if (minDistance == distance[a])
                {
                    minContentNum = a;
                }
            }
        }

        if (!isDragging)
        {
            LerpToContent(-contents[minContentNum].GetComponent<RectTransform>().anchoredPosition.x);
        }
    }

    private void Initialize()
    {
        contentLength = contents.Length;
        distance = new float[contentLength];
        distReposition = new float[contentLength];
        contentDistance = (int)Mathf.Abs(contents[1].GetComponent<RectTransform>().anchoredPosition.x - contents[0].GetComponent<RectTransform>().anchoredPosition.x);
        contentPanelRT.anchoredPosition = new Vector2((startContent - 1) * -contentDistance, 0);

        float panelWidth = contentLength * contentDistance;
        thresholdLeft = -(panelWidth * 0.5f);
        thresholdRight = panelWidth * 0.5f;
    }


    private void LerpToContent(float position)
    {
        float newX = Mathf.Lerp(contentPanelRT.anchoredPosition.x, position, Time.deltaTime * lerpSpeed);

        if (Mathf.Abs(position - newX) < completeRevise)
        {
            newX = position;
        }

        if (Mathf.Abs(newX) >= Mathf.Abs(position) - messageRevise && Mathf.Abs(newX) <= Mathf.Abs(position) + messageRevise && !isMessageSend)
        {
            isMessageSend = true;
            SendMessageFromContent(minContentNum);
        }

        Vector2 newPosition = new Vector2(newX, contentPanelRT.anchoredPosition.y);
        contentPanelRT.anchoredPosition = newPosition;
    }

    private void SendMessageFromContent(int contentIndex)
    {
        Debug.Log("SendMessage : " + contentIndex); 
    }

    public void StartDrag()
    {
        isMessageSend = false;
        isDragging = true;
        isTargetNearsButton = true;
    }

    public void EndDrag()
    {
        isDragging = false; 
    }

    public void GoToContent(int contentIndex)
    {
        isTargetNearsButton = false;
        minContentNum = contentIndex - 1;
    }
}
