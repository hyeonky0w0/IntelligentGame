using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpEggSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject jumpEggPrefab;
    public Transform canvasTransform;

    [Header("스폰 위치")]
    public Vector2 spawnPosition = new Vector2(-250f, 100f);

    [Header("Miss 판정 유예 시간 (hitTime 이후 이 시간 지나면 ForceMiss)")]
    public float missGrace = 0.4f;

    [Header("Beat 타이밍")]
    public List<JumpEggBeat> beats = new List<JumpEggBeat>();

    private BGMManager _bgm;
    private int _nextIndex = 0;
    private List<JumpEgg> _activeEggs = new List<JumpEgg>();

    void Start()
    {
        _bgm = BGMManager.Instance;

        // 43초 따따따 따따따
        beats.Add(new JumpEggBeat { hitTime = 43.026f });
        beats.Add(new JumpEggBeat { hitTime = 43.250f });
        beats.Add(new JumpEggBeat { hitTime = 43.441f });
        beats.Add(new JumpEggBeat { hitTime = 43.880f });
        beats.Add(new JumpEggBeat { hitTime = 44.100f });
        beats.Add(new JumpEggBeat { hitTime = 44.305f });

        // 46초 따아~ 따 따아~ 따
        beats.Add(new JumpEggBeat { hitTime = 46.498f });
        beats.Add(new JumpEggBeat { hitTime = 46.660f });
        beats.Add(new JumpEggBeat { hitTime = 46.885f });
        beats.Add(new JumpEggBeat { hitTime = 47.307f });
        beats.Add(new JumpEggBeat { hitTime = 47.516f });
        beats.Add(new JumpEggBeat { hitTime = 47.732f });

        // 50초 (따~~묵음~~)따따 (따~~묵음~~)따따
        beats.Add(new JumpEggBeat { hitTime = 49.950f });
        beats.Add(new JumpEggBeat { hitTime = 50.122f });
        beats.Add(new JumpEggBeat { hitTime = 50.403f });
        beats.Add(new JumpEggBeat { hitTime = 50.749f });
        beats.Add(new JumpEggBeat { hitTime = 50.965f });
        beats.Add(new JumpEggBeat { hitTime = 51.174f });

        beats.Sort((a, b) => a.hitTime.CompareTo(b.hitTime));
    }

    void Update()
    {
        if (_bgm == null) return;

        float current = _bgm.GetCurrentTime();

        while (_nextIndex < beats.Count &&
               current >= beats[_nextIndex].hitTime - 0.3f)
        {
            SpawnEgg(beats[_nextIndex]);
            _nextIndex++;
        }

        foreach (JumpEgg egg in _activeEggs)
        {
            if (egg == null) continue;
            if (!egg.IsJudged && current > egg.HitTime + missGrace)
                egg.ForceMiss();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryHitNextEgg(current);
        }

        _activeEggs.RemoveAll(e => e == null);
    }

    void SpawnEgg(JumpEggBeat beat)
    {
        GameObject obj = Instantiate(jumpEggPrefab, canvasTransform);
        RectTransform rt = obj.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = spawnPosition;
        }

        JumpEgg egg = obj.GetComponent<JumpEgg>();
        if (egg != null)
        {
            egg.Init(beat.hitTime, _bgm);
            _activeEggs.Add(egg);
        }
    }

    void TryHitNextEgg(float currentTime)
    {
        JumpEgg target = null;
        float minDiff = float.MaxValue;

        foreach (JumpEgg egg in _activeEggs)
        {
            if (egg == null || egg.IsJudged) continue;
            float diff = Mathf.Abs(currentTime - egg.HitTime);
            if (diff < minDiff)
            {
                minDiff = diff;
                target = egg;
            }
        }

        if (target != null)
            target.TryHit();
    }
}