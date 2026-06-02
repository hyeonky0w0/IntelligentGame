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


        if (other.gameObject.CompareTag("Ground"))
        {
            _resolved = true;

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
        LifeManager.GetOrCreate().LoseLife();
        Destroy(gameObject);
    }
}