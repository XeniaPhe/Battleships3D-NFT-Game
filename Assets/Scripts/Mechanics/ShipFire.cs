using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipFire : MonoBehaviour
{
    public Transform shots;
    public int setSlot = 1;
    public bool fire;
    public AudioSource shipAudioSource;
    public List<AudioClip> shipFireClips;

    public void FireFromShip(int slot)
    {
        GameObject shot = Instantiate(shots.GetChild(slot - 1).gameObject, shots.GetChild(slot - 1).position, shots.GetChild(slot - 1).rotation,transform);
        shot.SetActive(true);
        shipAudioSource.clip = shipFireClips[Random.Range(0, shipFireClips.Count)];
        shipAudioSource.Play();
    }

    public void FireFromAll(Transform disposableParent, Func<float,IEnumerator> dispose)
    {
        var all = shots.GetComponentsInChildren<Transform>(true).Where(t => !t.Equals(shots));

        foreach (var item in all)
        {
            GameObject shot = Instantiate(item.gameObject, item.position, item.rotation,disposableParent);
            shot.SetActive(true);
            shipAudioSource.clip = shipFireClips[Random.Range(0, shipFireClips.Count)];
            shipAudioSource.Play();
        }

        StartCoroutine(dispose.Invoke(2.5f));
    }

    private void Update()
    {
        if(fire)
        {
            FireFromShip(setSlot);
            fire = false;
        }
    }
}