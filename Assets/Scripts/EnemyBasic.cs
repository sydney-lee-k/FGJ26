using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    public enum AIState
    {
        Idle,
        Chase,
        Attack
    }

    public float attackStopDistanceRatio = 0.5f;

    public AIState CurrentState { get; private set; }
    private EnemyController m_enemyController;

    private void Start()
    {
        m_enemyController = GetComponent<EnemyController>();

        m_enemyController.onDetectedTarget = OnDetectedTarget;

        ChangeState(AIState.Idle);
    }

    private void Update()
    {
        UpdateStateTransitions();
        UpdateCurrentState();
    }

    private void UpdateStateTransitions()
    {
        switch (CurrentState)
        {
            case AIState.Chase:
                if (m_enemyController.IsTargetVisible && m_enemyController.IsTargetInAttackRange())
                {
                    ChangeState(AIState.Attack);
                    m_enemyController.SetNavDestination(transform.position);
                }
                break;

            case AIState.Attack:
                if (!m_enemyController.IsTargetInAttackRange())
                {
                    ChangeState(AIState.Chase);
                }
                break;
        }
    }

    private void UpdateCurrentState()
    {
        switch (CurrentState)
        {
            case AIState.Idle:
                // Nothing to do while idle
                break;

            case AIState.Chase:
                m_enemyController.SetNavDestination(m_enemyController.KnownDetectedTarget.transform.position);
                m_enemyController.RotateTowards(m_enemyController.KnownDetectedTarget.transform.position);
                break;

            case AIState.Attack:
                if (Vector3.Distance(m_enemyController.KnownDetectedTarget.transform.position,
                        m_enemyController.DetectionModule.transform.position)
                    >= (attackStopDistanceRatio * m_enemyController.attackRange))
                {
                    m_enemyController.SetNavDestination(m_enemyController.KnownDetectedTarget.transform.position);
                }
                else
                {
                    m_enemyController.SetNavDestination(transform.position);
                }

                m_enemyController.RotateTowards(m_enemyController.KnownDetectedTarget.transform.position);
                m_enemyController.TryAttack(m_enemyController.KnownDetectedTarget.transform.position);
                break;
        }
    }

    private void ChangeState(AIState t_newState)
    {
        CurrentState = t_newState;
    }

    private void OnDetectedTarget()
    {
        if (CurrentState == AIState.Idle)
        {
            CurrentState = AIState.Chase;
        }
    }
}