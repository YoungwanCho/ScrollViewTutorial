using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap_CS : MonoBehaviour 
{

    public RectTransform panel;
    public Button[] bttn;
    public RectTransform center;

    private float[] distance;
    private bool dragging = false;
    private int bttnDistance;
    private int minButtonNum;

    private void Start()
    {
        int bttnLength = bttn.Length;
        distance = new float[bttnLength];

        bttnDistance = (int)Mathf.Abs(bttn[1].GetComponent<RectTransform>().anchoredPosition.x - bttn[0].GetComponent<RectTransform>().anchoredPosition.x);
    }

    private void Update()
    {
        for (int i = 0; i < bttn.Length; i++)
        {
            distance[i] = Mathf.Abs(center.transform.position.x - bttn[i].transform.position.x);
        }
    }

}
