using UnityEngine;

namespace BattleShips.GameComponents
{
    internal class Peg : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Continuous Motion Settings")]
        [SerializeField] float dragSpeedMin;
        [SerializeField] float dragSpeedMax;
        [SerializeField] float dragAmountMin;
        [SerializeField] float dragAmountMax;
        [SerializeField] float rotationSpeedMin;
        [SerializeField] float rotationSpeedMax;
        [SerializeField] float rotationAmountMin;
        [SerializeField] float rotationAmountMax;
        [SerializeField] float randomTranslation;
        [SerializeField] float randomRotation;
        #endregion

        #region Cached Fields

        float t;
        bool initialized = false;
        Vector3 dragSpeed;
        Vector3 dragAmount;
        Vector3 rotationSpeed;
        Vector3 rotationAmount;

        #endregion

        private void Awake()
        {
            initialized = false;
        }

        internal void InitializeRandom()
        {
            dragSpeed = new Vector3(Random.Range(dragSpeedMin, dragSpeedMax), Random.Range(dragSpeedMin, dragSpeedMax), Random.Range(dragSpeedMin, dragSpeedMax));
            dragAmount = new Vector3(Random.Range(dragAmountMin, dragAmountMax), Random.Range(dragAmountMin, dragAmountMax), Random.Range(dragAmountMin, dragAmountMax));
            rotationSpeed = new Vector3(Random.Range(rotationSpeedMin, rotationSpeedMax), Random.Range(rotationSpeedMin, rotationSpeedMax), Random.Range(rotationSpeedMin, rotationSpeedMax));
            rotationAmount = new Vector3(Random.Range(rotationAmountMin, rotationAmountMax), Random.Range(rotationAmountMin, rotationAmountMax), Random.Range(rotationAmountMin, rotationAmountMax));
            randomRotation = Mathf.Abs(randomRotation);
            randomTranslation = Mathf.Abs(randomTranslation);
            initialized = true;
        }

        private void Update()
        {
            if (!initialized) return;

            t = Time.time;

            transform.position = new Vector3(CalculateMotion(dragSpeed.x, dragAmount.x),
                CalculateMotion(dragSpeed.y, dragAmount.y), CalculateMotion(dragSpeed.z, dragAmount.z)) + GetRandom(randomTranslation);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(CalculateMotion(rotationSpeed.x, rotationAmount.x),
                CalculateMotion(rotationSpeed.y, rotationAmount.y), CalculateMotion(rotationSpeed.z, rotationAmount.z)) + GetRandom(randomRotation));
        }

        float CalculateMotion(float speed,float amount) => Mathf.Sin(t * speed) * amount;

        //We can always remove that if fps drops immensely as more pegs get placed
        Vector3 GetRandom(float distance) => new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), Random.Range(-distance, distance));
    }
}