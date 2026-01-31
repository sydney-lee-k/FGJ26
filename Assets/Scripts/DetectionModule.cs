using System;
using System.Linq;
using UnityEngine;

public class DetectionModule : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 10.0f;

    [Space]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;

    public event Action OnDetectedTarget;
    public event Action OnLostTarget;

    public GameObject KnownDetectedTarget { get; private set; }
    public bool IsTargetVisible { get; private set; }

    public void HandleTargetDetection(Actor t_actor, Collider[] t_selfColliders)
    {
        //KnownDetectedTarget = null;

        IsTargetVisible = false;

        Collider[] t_rangeCheck = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);
        foreach (Collider t_collider in t_rangeCheck)
        {
            if (!t_collider.CompareTag("Player")) continue; // Only detect the player
            if (t_selfColliders.Contains(t_collider)) continue; // Ignore self-colliders

            Transform t_target = t_collider.transform;
            Vector3 t_directionToTarget = (t_target.position - transform.position).normalized;
            float t_distanceToTarget = Vector3.Distance(transform.position, t_target.position);

            // Ensure there’s no obstacle blocking line of sight
            if (!Physics.Raycast(transform.position, t_directionToTarget, t_distanceToTarget, obstacleLayer))
            {
                IsTargetVisible = true;
                KnownDetectedTarget = t_target.gameObject;

                OnDetect();

                return;
            }
        }
    }

    public virtual void OnDetect() => OnDetectedTarget?.Invoke();

    public virtual void OnDamaged(GameObject t_damageSource)
    {
        KnownDetectedTarget = t_damageSource;
        OnDetect();
    }

    /*
    public void HearNoise(Vector3 t_noisePosition)
    {
        if (Vector3.Distance(transform.position, t_noisePosition) <= hearingRange)
        {
            OnHearNoise?.Invoke(t_noisePosition);
        }
    }*/
}
