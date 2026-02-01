using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private MovementController controller;

    [SerializeField, Range(0f, 5f)] private float blendSpeed;

    [SerializeField] private Animator m_animator;

    private readonly int MoveParameterHash = Animator.StringToHash("Moving");
    private readonly int ForwardParameterHash = Animator.StringToHash("Forward");
    private readonly int RightParameterHash = Animator.StringToHash("Right");

    private void Update()
    {
        float t_forward = Mathf.MoveTowards(m_animator.GetFloat(ForwardParameterHash), controller.animForward, blendSpeed * Time.deltaTime);
        float t_right = Mathf.MoveTowards(m_animator.GetFloat(RightParameterHash), controller.animRight, blendSpeed * Time.deltaTime);

        m_animator.SetBool(MoveParameterHash, inputReader.HasInput);
        m_animator.SetFloat(ForwardParameterHash, t_forward);
        m_animator.SetFloat(RightParameterHash, t_right);
    }
}
