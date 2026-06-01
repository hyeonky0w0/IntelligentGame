using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KarmaOrbController : MonoBehaviour
{
    [Header("이펙트")]
    public ParticleSystem hitParticle;
    public AudioClip hitSFX;
    public AudioClip damageSFX;

    bool _resolved = false;
    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 velocity)
    {
        _rb.linearVelocity = velocity;
    }

    void OnCollisionEnter(Collision other)
    {
        if (_resolved) return;

        Debug.Log($"[KarmaOrb] 충돌: {other.gameObject.name} / Tag: {other.gameObject.tag}");

        if (other.gameObject.CompareTag("Ground"))
        {
            _resolved = true;
            Debug.Log("[KarmaOrb] Ground 충돌 → LoseLife 호출");

            // Instance 없으면 자동 생성
            LifeManager.GetOrCreate().LoseLife();

            if (damageSFX != null)
                AudioSource.PlayClipAtPoint(damageSFX, transform.position);
            Destroy(gameObject);
        }
    }

    public void Deflect()
    {
        if (_resolved) return;
        _resolved = true;
        _rb.isKinematic = true;

        Debug.Log("[KarmaOrb] Deflect — 격파");

        if (hitParticle != null)
        {
            hitParticle.transform.SetParent(null);
            hitParticle.Play();
        }
        if (hitSFX != null)
            AudioSource.PlayClipAtPoint(hitSFX, transform.position);

        MiniGameUIManager.Instance?.AddDeflectCount();
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        if (_resolved) return;
        _resolved = true;
        Debug.Log("[KarmaOrb] 화면 밖 이탈 → LoseLife 호출");
        LifeManager.GetOrCreate().LoseLife();
        Destroy(gameObject);
    }
}