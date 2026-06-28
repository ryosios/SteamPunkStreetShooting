using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class ParticleHitDetector : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private readonly List<ParticleSystem.Particle> _insideParticles = new();

    private PlayerManager _playerManager;

    private void Awake()
    {
        if (_particleSystem == null)
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        _playerManager = GameObject.FindGameObjectWithTag("Player")
                                     .GetComponent<PlayerManager>();

        var trigger = _particleSystem.trigger;

        // 既存をクリア
        trigger.SetCollider(0, null);

        // 0番に登録
        trigger.SetCollider(0, _playerManager.GrazeCollider);
    }

    private void OnParticleCollision(GameObject other)
    {

        if (_playerManager == null) return;

        _playerManager.OnHitBullet();
    }

    private void OnParticleTrigger()
    {
        int count = _particleSystem.GetTriggerParticles(
            ParticleSystemTriggerEventType.Inside,
            _insideParticles
        );

        if (count <= 0) return;

        // かすり通知
        _playerManager.OnGrazeBullet();

        
    }

}
