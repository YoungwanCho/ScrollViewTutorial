using UnityEngine;
using UnityEngine.UI;

public class Content : MonoBehaviour
{
    public Text text_ = null;

    public void UpdateView(string text)
    {
        text_.text = text;
    }
}
