using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class ParticleHitDetector : MonoBehaviour
{
    /// <summary>プレイヤー判定に使うタグ</summary>
    [SerializeField] private string _playerTag = "Player";

    /// <summary>グレイズ判定Colliderを探すためのタグ</summary>
    [SerializeField] private string _playerGrazeTag = "PlayerGraze";

    private ParticleSystem _particleSystem;

    private readonly List<ParticleSystem.Particle> _insideParticles = new();

    private void Awake()
    {
        if (_particleSystem == null)
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        SetGrazeColliderFromTag();
    }

    /// <summary>
    /// グレイズ判定に使うColliderをParticleSystem Triggerへ登録
    /// </summary>
    /// <param name="grazeCollider">グレイズ判定用Transform。</param>
    public void SetGrazeCollider(Transform grazeCollider)
    {
        if (_particleSystem == null)
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        if (_particleSystem == null || grazeCollider == null)
        {
            return;
        }

        var trigger = _particleSystem.trigger;

        // 既存をクリア
        trigger.SetCollider(0, null);

        // 0番に登録
        trigger.SetCollider(0, grazeCollider);
    }

    /// <summary>
    /// タグからグレイズ判定Colliderを探してParticleSystem Triggerへ登録
    /// </summary>
    private void SetGrazeColliderFromTag()
    {
        if (string.IsNullOrEmpty(_playerGrazeTag))
        {
            return;
        }

        GameObject grazeObject = GameObject.FindGameObjectWithTag(_playerGrazeTag);
        if (grazeObject == null)
        {
            return;
        }

        SetGrazeCollider(grazeObject.transform);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!HasTagInParent(other.transform, _playerTag))
        {
            return;
        }

        MessageBroker.Default.Publish(new EnemyBulletPlayerHitDetectedEvent());
    }

    private void OnParticleTrigger()
    {
        int count = _particleSystem.GetTriggerParticles(
            ParticleSystemTriggerEventType.Inside,
            _insideParticles
        );

        if (count <= 0) return;

        // かすり通知
        MessageBroker.Default.Publish(new EnemyBulletPlayerGrazeDetectedEvent());


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

