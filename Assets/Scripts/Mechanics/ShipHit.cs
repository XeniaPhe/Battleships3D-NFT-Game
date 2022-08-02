using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        var all = hits.GetComponentsInChildren<Transform>(true).Where(t => !t.Equals(hits));

        foreach (var item in all)
        {
            GameObject hit = Instantiate(item.gameObject, item.position, item.rotation);
            hit.SetActive(true);
            shipAudioSource.clip = shipHitClips[Random.Range(0, shipHitClips.Count)];
            shipAudioSource.Play();
        }

        all = fires.GetComponentsInChildren<Transform>(true).Where(t => !t.Equals(fires));

        foreach (var item in all)
        {
            GameObject fire = Instantiate(item.gameObject, item.position, item.rotation);
            fire.SetActive(true);
        }
    }

    public void HitEntirely(Transform disposableParent,Func<float,IEnumerator> dispose)
    {
        var all = hits.GetComponentsInChildren<Transform>(true).Where(t => !t.Equals(hits));

        foreach (var item in all)
        {
            GameObject hit = Instantiate(item.gameObject, item.position, item.rotation,disposableParent);
            hit.SetActive(true);
            shipAudioSource.clip = shipHitClips[Random.Range(0, shipHitClips.Count)];
            shipAudioSource.Play();
        }

        all = fires.GetComponentsInChildren<Transform>(true).Where(t => !t.Equals(fires));

        foreach (var item in all)
        {
            GameObject fire = Instantiate(item.gameObject, item.position, item.rotation,disposableParent);
            fire.SetActive(true);
        }

        StartCoroutine(dispose.Invoke(4f));
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
