using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class ParticleHitDetectorPlayer : MonoBehaviour
{
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
        BossPresenter hitBossPresenter = other.GetComponentInParent<BossPresenter>();
        if (hitBossPresenter == null)
        {
            return;
        }

        hitBossPresenter.OnHitBullet();

    }

    /*
    private void OnParticleTrigger()
    {
        int count = _particleSystem.GetTriggerParticles(
            ParticleSystemTriggerEventType.Inside,
            _insideParticles
        );

        if (count <= 0) return;

        // かすり通知
        _playerPresenter.OnGrazeBullet();       


    }
    */
}

