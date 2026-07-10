using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class ParticleHitDetector : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private readonly List<ParticleSystem.Particle> _insideParticles = new();

    private PlayerPresenter _playerPresenter;

    private void Awake()
    {
        if (_particleSystem == null)
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        _playerPresenter = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPresenter>();

        var trigger = _particleSystem.trigger;

        // 既存をクリア
        trigger.SetCollider(0, null);

        // 0番に登録
        trigger.SetCollider(0, _playerPresenter.GrazeCollider);
    }

    private void OnParticleCollision(GameObject other)
    {
        PlayerPresenter player = other.GetComponentInParent<PlayerPresenter>();
        if (_playerPresenter == null) return;

        if (_playerPresenter.IsParryActive)
        {
            _playerPresenter.OnParryBullet();
            
        }
        else
        {
            _playerPresenter.OnHitBullet();
        }

    }

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

}

