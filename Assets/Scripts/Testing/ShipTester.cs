using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BattleShips.GameComponents.Testing
{
    internal class ShipTester : MonoBehaviour
    {
        [SerializeField] Button fireToggle;
        [SerializeField] Button shootButton;
        [SerializeField] Button hitButton;
        [SerializeField] Transform disposables;

        List<ParticleSystem> fires;
        List<ShipFire> guns;
        List<ShipHit> hits;

        bool playing = true;

        private void Awake()
        {
            if ((guns is null || hits is null || fires is null) && fireToggle && shootButton && hitButton)
            {
                guns = FindObjectsOfType<ShipFire>().ToList();
                hits = guns.Select(g => g.GetComponent<ShipHit>()).ToList();
                fires = hits.SelectMany(h => h.GetComponentsInChildren<ParticleSystem>().Where(p => p.name.Contains("Fire"))).ToList();
                ToggleFire();
                shootButton.onClick.AddListener(Shoot);
                hitButton.onClick.AddListener(Hit);
                fireToggle.onClick.AddListener(ToggleFire);
            }
        }

        private void ToggleFire()
        {
            fires.ForEach((ParticleSystem vfx) =>
            {
                if (playing)
                {
                    vfx.gameObject.SetActive(false);
                }
                else
                {
                    vfx.gameObject.SetActive(true);
                }
            });

            playing = !playing;
        }

        private void Shoot()
        {
            guns.ForEach(g => g.FireFromAll(disposables,DisposeAll));
        }

        private void Hit()
        {
            hits.ForEach(h => h.HitEntirely(disposables,DisposeAll));
        }

        IEnumerator DisposeAll(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            disposables.GetComponentsInChildren<Transform>().Where(t => !t.Equals(disposables)).ToList().ForEach(t => Destroy(t.gameObject));
        }
    }
}
