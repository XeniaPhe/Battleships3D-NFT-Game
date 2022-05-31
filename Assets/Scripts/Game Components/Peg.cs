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

        [Header("Shake Settings")]
        [SerializeField] float shakeRotationMin;
        [SerializeField] float shakeRotationMax;
        [SerializeField] float shakeRotationSpeed;
        [SerializeField] int shakePeriodCount;
        [SerializeField] Axis shakeAxis;

        #endregion

        #region Cached Fields
        float t;
        float shakeDuration;
        float shakeBeginningTime;
        bool isShaking;

        Vector3 dragSpeed;
        Vector3 dragAmount;
        Vector3 rotationSpeed;
        Vector3 rotationAmount;


        #endregion

        private void Awake()
        {
            randomRotation = Mathf.Abs(randomRotation);
            randomTranslation = Mathf.Abs(randomTranslation);
            InitializeRandom();
        }

        internal void InitializeRandom()
        {
            dragSpeed = new Vector3(Random.Range(dragSpeedMin, dragSpeedMax), Random.Range(dragSpeedMin, dragSpeedMax), Random.Range(dragSpeedMin, dragSpeedMax));
            dragAmount = new Vector3(Random.Range(dragAmountMin, dragAmountMax), Random.Range(dragAmountMin, dragAmountMax), Random.Range(dragAmountMin, dragAmountMax));
            rotationSpeed = new Vector3(Random.Range(rotationSpeedMin, rotationSpeedMax), Random.Range(rotationSpeedMin, rotationSpeedMax), Random.Range(rotationSpeedMin, rotationSpeedMax));
            rotationAmount = new Vector3(Random.Range(rotationAmountMin, rotationAmountMax), Random.Range(rotationAmountMin, rotationAmountMax), Random.Range(rotationAmountMin, rotationAmountMax));
        }

        private void Update()
        {
            t = Time.time;

            if(isShaking)
            {



                //if (t - shakeBeginningTime > shakeDuration)
                //{
                //    isShaking = false;
                //    InitializeRandom();
                //}
            }
            else
            {
                transform.position = new Vector3(CalculateMotion(dragSpeed.x, dragAmount.x),
                    CalculateMotion(dragSpeed.y, dragAmount.y), CalculateMotion(dragSpeed.z, dragAmount.z)) + GetRandom(randomTranslation);
                transform.rotation = Quaternion.Euler(new Vector3(CalculateMotion(rotationSpeed.x, rotationAmount.x),
                    CalculateMotion(rotationSpeed.y, rotationAmount.y), CalculateMotion(rotationSpeed.z, rotationAmount.z)) + GetRandom(randomRotation));
            }
        }
        internal void Shake()
        {
            shakeBeginningTime = t;
            isShaking = true;


        }

        float CalculateMotion(float speed,float amount) => Mathf.Sin(t * speed) * amount;

        //We can always remove that if fps drops immensely as more pegs get placed
        Vector3 GetRandom(float distance) => new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), Random.Range(-distance, distance));
        
    }
}