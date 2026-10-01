using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{

    public static FMODEvents instance {  get; private set; }

    [field: Header("Gun SFX")]
    [field: SerializeField] public EventReference gunShot {  get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootstep { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (!transform.parent) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Trying to create a duplicate FMODEvents on " + gameObject.name);
            Destroy(gameObject);
            return;
        }
    }
}
