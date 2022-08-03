using System.Linq;
using UnityEngine;

public class WaterHit : MonoBehaviour
{
    public Transform hitTransform;

    public void HitWater(Vector3 pos)
    {
        GameObject shot = Instantiate(hitTransform.gameObject, pos, hitTransform.rotation);
        shot.tag = "Disposable";
        shot.SetActive(true);
    }
}
