using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipHit : MonoBehaviour
{
    public Transform hits;
    public Transform fires;
    public int setSlot = 1;
    public bool hit;
    public AudioSource shipAudioSource;
    public List<AudioClip> shipHitClips;

    public void HitShip(int slot)
    {
        GameObject shot = Instantiate(hits.GetChild(slot - 1).gameObject, hits.GetChild(slot - 1).position, hits.GetChild(slot - 1).rotation);
        shot.SetActive(true);
        fires.GetChild(slot - 1).gameObject.SetActive(true);
        shipAudioSource.clip = shipHitClips[Random.Range(0, shipHitClips.Count)];
        shipAudioSource.Play();
    }

    public void HitEntirely()
    {
        fires.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(p => p.gameObject.SetActive(true));
    }

    private void Update()
    {
        if (hit)
        {
            HitShip(setSlot);
            hit = false;
        }
    }
}
