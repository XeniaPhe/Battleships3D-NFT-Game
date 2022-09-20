using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BattleShips.VFX
{
    internal class ShipExploder : MonoBehaviour
    {
        [SerializeField] Transform explosions;
        [SerializeField] Transform fires;
        [SerializeField] List<AudioClip> explosionClips;

        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        internal void ExplodeShip(int slot)
        {
            print(slot);
            GameObject explosion = Instantiate(explosions.GetChild(slot).gameObject, explosions.GetChild(slot).position, explosions.GetChild(slot).rotation);
            explosion.SetActive(true);
            fires.GetChild(slot).gameObject.SetActive(true);
            audioSource.clip = explosionClips[Random.Range(0, explosionClips.Count)];
            audioSource.Play();
        }

        internal void ExplodeEntirely()
        {
            var all = explosions.GetComponentsInChildren<Transform>(true).Where(t => !t.Equals(explosions));

            foreach (var item in all)
            {
                GameObject explosion = Instantiate(item.gameObject, item.position, item.rotation);
                explosion.SetActive(true);
                audioSource.clip = explosionClips[Random.Range(0, explosionClips.Count)];
                audioSource.Play();
            }

            all = fires.GetComponentsInChildren<Transform>(true).Where(t => !t.Equals(fires));

            foreach (var item in all)
            {
                item.gameObject.SetActive(true);
            }
        }
    }
}