using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractableDoor : MonoBehaviour
{

    [SerializeField] private GameObject player;

    [SerializeField] private GameObject levelStartingPoint;

    [SerializeField] private Animator animator;
    [SerializeField] private MovementController mc;    


    public void OnDoorInteract()
    {
        StartCoroutine(teleportCoroutine());        
    }

    private IEnumerator teleportCoroutine()
    {

        mc.enabled = false;
        animator.Play(0);
        player.transform.position = levelStartingPoint.transform.position;
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);
        mc.enabled = true;
    }
}
