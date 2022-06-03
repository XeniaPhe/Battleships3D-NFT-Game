using UnityEngine;

namespace BattleShips.GameComponents
{
    internal class Peg : MonoBehaviour
    {
        #region Serialized Fields

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
        [SerializeField] float resetTime;
        [SerializeField] internal bool isWhitePeg;

        #endregion

        #region Cached Fields

        float t;
        bool initialized = false;
        Vector3 posOffset;
        Vector3 rotOffset;
        Vector3 dragSpeed;
        Vector3 dragAmount;
        Vector3 rotationSpeed;
        Vector3 rotationAmount;

        #endregion

        private void Awake()
        {
            initialized = false;
        }

        internal void InitializeRandom(Vector3? posOffset = null,Vector3? rotOffset = null)
        {
            dragSpeed = new Vector3(Random.Range(dragSpeedMin, dragSpeedMax), Random.Range(dragSpeedMin, dragSpeedMax), Random.Range(dragSpeedMin, dragSpeedMax));
            dragAmount = new Vector3(Random.Range(dragAmountMin, dragAmountMax), Random.Range(dragAmountMin, dragAmountMax), Random.Range(dragAmountMin, dragAmountMax));
            rotationSpeed = new Vector3(Random.Range(rotationSpeedMin, rotationSpeedMax), Random.Range(rotationSpeedMin, rotationSpeedMax), Random.Range(rotationSpeedMin, rotationSpeedMax));
            rotationAmount = new Vector3(Random.Range(rotationAmountMin, rotationAmountMax), Random.Range(rotationAmountMin, rotationAmountMax), Random.Range(rotationAmountMin, rotationAmountMax));
            randomRotation = Mathf.Abs(randomRotation);
            randomTranslation = Mathf.Abs(randomTranslation);

            this.posOffset = posOffset ?? transform.position;
            this.rotOffset = rotOffset ?? transform.rotation.eulerAngles;
            initialized = true;
        }

        private void Update()
        {
            if (!initialized) return;

            t = Time.time;

            transform.position = posOffset + new Vector3(CalculateMotion(dragSpeed.x, dragAmount.x),
                CalculateMotion(dragSpeed.y, dragAmount.y), CalculateMotion(dragSpeed.z, dragAmount.z)) + GetRandom(randomTranslation);
            transform.rotation = Quaternion.Euler(rotOffset + new Vector3(CalculateMotion(rotationSpeed.x, rotationAmount.x),
                CalculateMotion(rotationSpeed.y, rotationAmount.y), CalculateMotion(rotationSpeed.z, rotationAmount.z)) + GetRandom(randomRotation));
        }

        float CalculateMotion(float speed,float amount) => Mathf.Sin(t * speed) * amount;

        //We can always remove that if fps drops immensely as more pegs get placed
        Vector3 GetRandom(float distance) => new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), Random.Range(-distance, distance));
    }
}