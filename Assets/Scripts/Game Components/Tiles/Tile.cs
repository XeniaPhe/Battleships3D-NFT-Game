using UnityEngine;
using UnityEngine.EventSystems;
using BattleShips.Utils;

namespace BattleShips.GameComponents.Tiles
{
    [RequireComponent(typeof(CoordinateWrapper))]
    internal abstract class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region Serialized Fields

        [SerializeField] Color enteredColor;
        [SerializeField] protected float temporaryMaskDuration = 0.18f;

        #endregion

        #region Cached Fields

        internal TileData tileData;
        protected SpriteRenderer self;
        protected Color normalColor;
        protected WaitForSeconds maskWait;
        protected static GameBoard board;
        protected bool isTemporarilyPainted;

        #endregion

        protected virtual void Awake()
        {
            self = GetComponent<SpriteRenderer>();
            normalColor = self.color;
            var wrapper = GetComponent<CoordinateWrapper>();
            tileData = new TileData(wrapper.x, wrapper.y);
            maskWait = new WaitForSeconds(temporaryMaskDuration);
        }

        protected virtual void Start()
        {
            if (!board) board = GameBoard.Instance;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            self.color = enteredColor;
            board.EnteredTile = this;
        }
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            board.ClickedTile = this;
        }
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            self.color = normalColor;
            board.EnteredTile = null;
        }

        internal Coordinate GetTileCoordinatesAt(Directions direction) => tileData.Coordinates.GetCoordinatesAt(direction);

        internal bool IsTileInNormalState() => (tileData.tileState == TileState.Normal && !isTemporarilyPainted);

        internal virtual void PaintTemporarily(Color color)
        {
            isTemporarilyPainted = true;
            self.color = color;
        }

        internal virtual void RemoveTemporaryPaint()
        {
            self.color = normalColor;
            isTemporarilyPainted = false;
        }
    }
}