using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFire : MonoBehaviour
{
    public Transform shots;
    public int setSlot = 1;
    public bool fire;
    public AudioSource shipAudioSource;
    public List<AudioClip> shipFireClips;

    public void FireFromShip(int slot)
    {
        GameObject shot = Instantiate(shots.GetChild(slot - 1).gameObject, shots.GetChild(slot - 1).position, shots.GetChild(slot - 1).rotation);
        shot.SetActive(true);
        shipAudioSource.clip = shipFireClips[Random.Range(0, shipFireClips.Count)];
        shipAudioSource.Play();
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
