using UnityEngine;

public class FootprintParticle : MonoBehaviour
{
    private ParticleSystem pSystem;
    [SerializeField] private float updateDistance = 1f;
    [SerializeField] private Vector3 offset;

    private ParticleSystem.EmitParams emitParameters;
    private Vector3 previousPoint;
    private Transform rotationFollow;
    
    private void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
        emitParameters = new ParticleSystem.EmitParams();
        previousPoint = transform.position;
        
        rotationFollow = transform.parent ?  transform.parent : transform;
    }

    private void Update()
    {
        if (!Vector3Utils.IsWithinDistance(previousPoint, transform.position, updateDistance))
        {
            Vector3 direction = transform.position - previousPoint;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                emitParameters.position = transform.position;
                //Some manual values for now. Edit these to match whatever texture / shader we use for footsteps.
                emitParameters.rotation3D = offset + new Vector3(-90f, 0f, rotationFollow.eulerAngles.y-90);

                pSystem.Emit(emitParameters, 1);
            }
            previousPoint = transform.position;
        }
    }
}