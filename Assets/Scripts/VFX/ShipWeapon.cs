using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BattleShips.VFX
{
    internal class ShipWeapon : MonoBehaviour
    {
        [SerializeField] Transform shots;
        [SerializeField] List<AudioClip> fireClips;

        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        internal void FireFromShip(int slot)
        {
            GameObject shot = Instantiate(shots.GetChild(slot).gameObject, shots.GetChild(slot).position, shots.GetChild(slot).rotation);
            shot.tag = "Disposable";
            shot.SetActive(true);
            audioSource.clip = fireClips[Random.Range(0, fireClips.Count)];
            audioSource.Play();
        }

        internal void FireFromAll(Transform disposableParent, Func<float, IEnumerator> dispose)
        {
            var all = shots.GetComponentsInChildren<Transform>(true).Where(t => !t.Equals(shots));

            foreach (var item in all)
            {
                GameObject shot = Instantiate(item.gameObject, item.position, item.rotation, disposableParent);
                shot.SetActive(true);
                audioSource.clip = fireClips[Random.Range(0, fireClips.Count)];
                audioSource.Play();
            }

            StartCoroutine(dispose.Invoke(2.5f));
        }
    }
}