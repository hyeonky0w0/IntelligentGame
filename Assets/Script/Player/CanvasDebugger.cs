using UnityEngine;

public class CanvasDebugger : MonoBehaviour
{
    void Start()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);
        foreach (Canvas c in allCanvases)
        {
            Debug.Log($"Canvas: {c.name} | active: {c.gameObject.activeInHierarchy}");
        }
    }
}