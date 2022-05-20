using UnityEngine;
using BattleShips.GameComponents;
using BattleShips.Management;

namespace BattleShips.GUI
{
    [RequireComponent(typeof(GameShipWrapperButton))]
    internal class TemporaryShipWrapper : MonoBehaviour
    {
        [SerializeField] Ship ship;
        GameShipWrapperButton button;

        private void Start()
        {
            button = GetComponent<GameShipWrapperButton>();
            button.onClick.AddListener(() => { ShipPlacementTool.Instance.SelectShip(ship); });
        }
    }
}