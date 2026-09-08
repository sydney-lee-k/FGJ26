using System;
using UnityEngine;
using System.Collections.Generic;

public class NoiseController : MonoBehaviour
{
    public static NoiseController Instance;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float wallMuffleMultiplier = 0.5f;
    [SerializeField] private LayerMask enemyMask;

    [SerializeField] private bool visualizeNoise;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateNoise(Vector3 position, float radius)
    {
        //Get all enemies,
        Collider[] enemyHits = Physics.OverlapSphere(position, radius, enemyMask);
        foreach (var enemyHit in enemyHits)
        {
            EnemyController enemy = enemyHit.GetComponent<EnemyController>();
            if (visualizeNoise) Debug.DrawLine(position, enemy.transform.position, new Color(1,1,0,0.33f),1 );
            
            if (enemy && Vector3Utils.IsWithinDistance(position, enemyHit.transform.position, radius))
            {
                
                RaycastHit[] pathHits = new RaycastHit[10]; //If there are more than 10 objects in the way the enemy probably cant hear you.
                
                Vector3 direction = enemyHit.transform.position - position;

                int hitCount = Physics.RaycastNonAlloc(position, direction.normalized, pathHits, direction.magnitude + 1); //+1 Range for safety buffer. Should be unnecessary but does little harm.
                Array.Sort(pathHits, 0, hitCount, Comparer<RaycastHit>.Create((firstHit, secondHit) => firstHit.distance.CompareTo(secondHit.distance)));

                float totalDistance = 0f;
                float remainingDistance = radius;
                float previousHitDistance = 0f;

                for (int i = 0; i < hitCount; i++)
                {
                    RaycastHit pathHit = pathHits[i];
                    float segmentDistance = pathHit.distance - previousHitDistance;
                    remainingDistance -= segmentDistance;

                    if (remainingDistance <= 0f) break; //Noise ran out due to walls and wont reach
                    totalDistance += segmentDistance;
                    
                    if (pathHit.transform == enemy.transform)
                    {
                        enemy.GoCheck(position);
                        break;
                    } //Enemy alerted, no need to continue

                    if (LayerUtils.Contains(obstacleMask, pathHit.transform))
                    {
                        remainingDistance *= 1-wallMuffleMultiplier;
                    } //Reduce distance due to a wall

                    previousHitDistance = pathHit.distance;
                }
                
                if (visualizeNoise) Debug.DrawRay(position, direction.normalized * totalDistance, new Color(0,1,0), 1f);
                
            }
        }
    } 
    // on hindsight the above could be optimized so that instead of even reaching the enemy, we just check if the wall distance would be enough.
    // We already know how far the enemy is so comparing the distance the sound travels vs that would maybe be a bit more efficient. However I doubt it will matter, but if there's lag then ig look here.
    
}
