using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleShips.Music
{
    public class BackgroundMusic : MonoBehaviour
    {
        public static BackgroundMusic Instance;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else Destroy(this);
        }
    }
}