using System.Collections.Generic;
using UnityEngine;

public class StrawberrySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject strawberryPrefab;
    public Transform canvasTransform;

    [Header("Beat Pairs")]
    public List<StrawberryBeatPair> beatPairs = new List<StrawberryBeatPair>();

    private BGMManager _bgmManager;
    private int _nextIndex = 0;

    void Start()
    {
        _bgmManager = FindObjectOfType<BGMManager>();
    }



    void Update()
    {
        if (_nextIndex >= beatPairs.Count) return;
        if (_bgmManager == null) return;

        float currentTime = _bgmManager.GetCurrentTime();

        if (currentTime >= beatPairs[_nextIndex].shadowTime)
        {
            SpawnStrawberry(beatPairs[_nextIndex]);
            _nextIndex++;
        }
    }

    void SpawnStrawberry(StrawberryBeatPair pair)
    {
        GameObject obj = Instantiate(strawberryPrefab, canvasTransform);

        GameObject scissors = GameObject.Find("Cursor_Scissors");
        if (scissors != null)
            obj.transform.SetSiblingIndex(scissors.transform.GetSiblingIndex());


        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pair.position;
        }

        StrawberryObject strawObj = obj.GetComponent<StrawberryObject>();
        if (strawObj != null)
            strawObj.Init(pair.hitTime, pair.lastHitTime, _bgmManager);
    }
}