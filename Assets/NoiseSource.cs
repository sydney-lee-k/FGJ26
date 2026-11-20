using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    public bool SourceInView;

    public void CreateNoise(Vector3 position, float radius, bool alert = false, bool spawnVisual = true)
    {
        Debug.Log(!SourceInView + ", " + spawnVisual);
        NoiseController.Instance.CreateNoise(position, radius, alert, !SourceInView && spawnVisual);
    }
}
