using System.Collections;
using UnityEngine;

namespace BattleShips.VFX
{
    internal class SelfDestructor : MonoBehaviour
    {
        [SerializeField] float destructionTime = 4f;

        private void Awake()
        {
            SelfDestruct();
        }

        private IEnumerator SelfDestruct()
        {
            yield return new WaitForSeconds(destructionTime);
            Destroy(gameObject);
        }
    }
}