using UnityEngine;

namespace BattleShips.VFX
{
    internal class ExplosionCreator : MonoBehaviour
    {
        [SerializeField] Transform shipExplosion;
        [SerializeField] Transform waterExplosion;

        internal void Explode(Vector3 pos,ExplosionType type)
        {
            GameObject explosion;
            if(type == ExplosionType.ShipExplosion)
                explosion = Instantiate(shipExplosion.gameObject, pos, shipExplosion.rotation);
            else
                explosion = Instantiate(waterExplosion.gameObject,pos, waterExplosion.rotation);

            explosion.SetActive(true);
        }
    }
}