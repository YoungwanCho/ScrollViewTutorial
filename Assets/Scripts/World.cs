using UnityEngine;

public class World : MonoBehaviour
{
    public ListView listview_ = null;
    [SerializeField] private string contentViewUpdateName = string.Empty;

    public void Start()
    {
        listview_.Initialize(this.GetContentDataCount, this.ContentViewUpdate);
    }

    public void ContentViewUpdate(GameObject content, int dataOrderIndex)
    {
        content.SendMessage(contentViewUpdateName, dataOrderIndex.ToString());
    }

    public int GetContentDataCount()
    {
        return 40;
    }
}
