using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    public enum AIState
    {
        Idle,
        Chase,
        Attack
    }

    [SerializeField] private float attackStopDistanceRatio = 0.5f;

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
                if (m_enemyController.IsTargetVisible &&
                    m_enemyController.IsTargetInAttackRange())
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
        var target = m_enemyController.KnownDetectedTarget;
        if (target == null)
        {
            ChangeState(AIState.Idle);
            return;
        }

        switch (CurrentState)
        {
            case AIState.Idle:
                // IMPORTANT: never fire while idle
                m_enemyController.ReleaseTrigger();
                break;

            case AIState.Chase:
                m_enemyController.ReleaseTrigger();

                m_enemyController.SetNavDestination(target.transform.position);
                m_enemyController.RotateTowards(target.transform.position);
                break;

            case AIState.Attack:
                float distance = Vector3.Distance(
                    target.transform.position,
                    m_enemyController.DetectionModule.transform.position
                );

                if (distance >= attackStopDistanceRatio * m_enemyController.attackRange)
                {
                    m_enemyController.SetNavDestination(target.transform.position);
                }
                else
                {
                    m_enemyController.SetNavDestination(transform.position);
                }

                m_enemyController.RotateTowards(target.transform.position);

                // HOLD trigger while attacking
                m_enemyController.TryAttack(target.transform.position);
                break;
        }
    }

    private void ChangeState(AIState newState)
    {
        if (CurrentState == newState)
            return;

        // Release trigger when leaving Attack
        if (CurrentState == AIState.Attack)
            m_enemyController.ReleaseTrigger();

        CurrentState = newState;
    }

    private void OnDetectedTarget()
    {
        if (CurrentState == AIState.Idle)
            ChangeState(AIState.Chase);
    }
}