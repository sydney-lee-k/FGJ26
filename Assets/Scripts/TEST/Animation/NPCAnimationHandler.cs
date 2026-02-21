using UnityEngine;

public class NPCAnimationHandler : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        Dance();
    }

    private void Dance()
    {
        anim.SetInteger("DanceIndex", Random.Range(0, 12));
        anim.speed = Random.Range(0.7f, 1.3f);
        anim.SetTrigger("Dance");
    }
}
