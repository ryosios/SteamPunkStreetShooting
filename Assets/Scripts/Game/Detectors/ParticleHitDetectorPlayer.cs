using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class ParticleHitDetectorPlayer : MonoBehaviour
{
    /// <summary>ボス判定に使うタグ</summary>
    [SerializeField] private string _bossTag = "Boss";

    private ParticleSystem _particleSystem;

    private readonly List<ParticleSystem.Particle> _insideParticles = new();

    private void Awake()
    {
        if (_particleSystem == null)
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        //var trigger = _particleSystem.trigger;

        // 既存をクリア
        //trigger.SetCollider(0, null);

        // 0番に登録
        //trigger.SetCollider(0, _bossManager.GrazeCollider);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!HasTagInParent(other.transform, _bossTag))
        {
            return;
        }

        MessageBroker.Default.Publish(new PlayerBulletBossHitDetectedEvent());

    }

    private bool HasTagInParent(Transform target, string tagName)
    {
        if (target == null || string.IsNullOrEmpty(tagName))
        {
            return false;
        }

        Transform current = target;
        while (current != null)
        {
            if (current.CompareTag(tagName))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}

