using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class World : MonoBehaviour
{
    [SerializeField] private string contentViewUpdateName = string.Empty;

    public void ContentViewUpdate(GameObject content, int dataOrderIndex)
    {
        content.SendMessage(contentViewUpdateName, dataOrderIndex.ToString());
    }
}
