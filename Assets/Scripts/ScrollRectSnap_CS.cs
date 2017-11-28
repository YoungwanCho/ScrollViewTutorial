using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap_CS : MonoBehaviour 
{
    public Button[] bttn;
    public RectTransform contentPanelRT;
    public RectTransform centerRT;
    public int startButton = 1;
    public float completeRevise = 3.0f;
    public float messageRevise = 4.0f;

    private float[] distance;
    private float[] distReposition;
    private float lerpSpeed = 5f;
    private float thresholdLeft = 0.0f;
    private float thresholdRight = 0.0f;
    private int bttnDistance;
    private int minButtonNum;
    private int bttnLength;
    private bool isDragging = false;
    private bool isMessageSend = false;
    private bool isTargetNearsButton = true;



    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        for (int i = 0; i < bttn.Length; i++)
        {
            distReposition[i] = centerRT.position.x - bttn[i].GetComponent<RectTransform>().position.x;
            distance[i] = Mathf.Abs(distReposition[i]);

            if (distReposition[i] > thresholdRight)
            {
                float curX = bttn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = bttn[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX + (bttnLength * bttnDistance), curY);
                bttn[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPos;
            }

            if (distReposition[i] < thresholdLeft)
            {
                float curX = bttn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = bttn[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX - (bttnLength * bttnDistance), curY);
                bttn[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPos;
            }
        }

        if (isTargetNearsButton)
        {
            float minDistance = Mathf.Min(distance);

            for (int a = 0; a < bttn.Length; a++)
            {
                if (minDistance == distance[a])
                {
                    minButtonNum = a;
                }
            }
        }

        if (!isDragging)
        {
            LerpToBttn(-bttn[minButtonNum].GetComponent<RectTransform>().anchoredPosition.x);
        }
    }

    private void Initialize()
    {
        bttnLength = bttn.Length;
        distance = new float[bttnLength];
        distReposition = new float[bttnLength];
        bttnDistance = (int)Mathf.Abs(bttn[1].GetComponent<RectTransform>().anchoredPosition.x - bttn[0].GetComponent<RectTransform>().anchoredPosition.x);
        contentPanelRT.anchoredPosition = new Vector2((startButton - 1) * -bttnDistance, 0);

        float panelWidth = bttnLength * bttnDistance;
        thresholdLeft = -(panelWidth * 0.5f);
        thresholdRight = panelWidth * 0.5f;
    }


    private void LerpToBttn(float position)
    {
        float newX = Mathf.Lerp(contentPanelRT.anchoredPosition.x, position, Time.deltaTime * lerpSpeed);

        if (Mathf.Abs(position - newX) < completeRevise)
        {
            newX = position;
        }

        if (Mathf.Abs(newX) >= Mathf.Abs(position) - messageRevise && Mathf.Abs(newX) <= Mathf.Abs(position) + messageRevise && !isMessageSend)
        {
            isMessageSend = true;
            SendMessageFromButton(minButtonNum);
        }

        Vector2 newPosition = new Vector2(newX, contentPanelRT.anchoredPosition.y);
        contentPanelRT.anchoredPosition = newPosition;
    }

    private void SendMessageFromButton(int buttonIndex)
    {
        Debug.Log("SendMessage : " + buttonIndex); 
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

    public void GoToButton(int buttonIndex)
    {
        isTargetNearsButton = false;
        minButtonNum = buttonIndex - 1;
    }
}
