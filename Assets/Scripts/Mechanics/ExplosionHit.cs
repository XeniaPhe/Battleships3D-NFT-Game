using System.Linq;
using UnityEngine;

public class ExplosionHit : MonoBehaviour
{
    public Transform hitTransform;

    public void HitExplosion(Vector3 pos)
    {
        GameObject shot = Instantiate(hitTransform.gameObject, pos, hitTransform.rotation);
        shot.tag = "Disposable";
        shot.SetActive(true);
    }
}
