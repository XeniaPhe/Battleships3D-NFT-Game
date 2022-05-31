using UnityEngine;

namespace BattleShips.GameComponents
{
    internal class Peg : MonoBehaviour
    {
        [SerializeField] Vector3 dragSpeed;
        [SerializeField] Vector3 dragAmount;
        [SerializeField] Vector3 rotationSpeed;
        [SerializeField] Vector3 rotationAmount;
        [SerializeField] float randomTranslation;
        [SerializeField] float randomRotation;
        float t;

        private void Awake()
        {
            randomRotation = Mathf.Abs(randomRotation);
            randomTranslation = Mathf.Abs(randomTranslation);
        }
        private void Update()
        {
            t = Time.time;
            transform.position = new Vector3(CalculateMotion(t, dragSpeed.x, dragAmount.x),
                CalculateMotion(t, dragSpeed.y, dragAmount.y), CalculateMotion(t, dragSpeed.z, dragAmount.z)) + GetRandom(randomTranslation);
            transform.rotation = Quaternion.Euler(new Vector3( CalculateMotion(t,rotationSpeed.x,rotationAmount.x),
                CalculateMotion(t,rotationSpeed.y,rotationAmount.y),CalculateMotion(t,rotationSpeed.z,rotationAmount.z)) + GetRandom(randomRotation));
        }

        float CalculateMotion(float time,float speed,float amount) => Mathf.Sin(time * speed) * amount;

        Vector3 GetRandom(float distance)
        {
            return new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), Random.Range(-distance, distance));
        }
    }
}