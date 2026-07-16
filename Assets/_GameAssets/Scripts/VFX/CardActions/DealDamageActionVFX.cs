using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class DealDamageActionVFX : NetworkBehaviour
{
    [SerializeField] private GameObject travelVFX;
    [SerializeField] private GameObject onHitVFX;

    private void OnEnable()
    {
        if(onHitVFX)
        {
            onHitVFX.SetActive(false);
        }
    }

    public async Task DoVFX(Vector3 origin, Vector3 target, float travelTime)
    {
        if(travelVFX)
        {
            if(travelTime > 0f)
            {
                travelVFX.SetActive(true);

                var t = 0f;
                do
                {
                    transform.position = Vector3.Lerp(origin, target, t);
                    t += Time.deltaTime / travelTime;
                    await Task.Yield();
                }
                while (t <= 1f);
            }

            travelVFX.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"No travel VFX set!");
            await Task.Delay((int)(travelTime * 1000));
        }

        if(onHitVFX)
        {
            onHitVFX.SetActive(true);
        }

        Destroy(gameObject, 5f);
    }
}
