using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleShips.GameComponents;
public class TempScript : MonoBehaviour
{
    [SerializeField] Peg peg;
    public void X()
    {
        peg.Shake();
    }
}
