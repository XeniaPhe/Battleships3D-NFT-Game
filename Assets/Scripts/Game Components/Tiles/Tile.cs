using UnityEngine;
using UnityEngine.EventSystems;
using BattleShips.Utils;

namespace BattleShips.GameComponents.Tiles
{
    [RequireComponent(typeof(CoordinateWrapper))]
    internal abstract class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region Serialized Fields

        [SerializeField] protected Color disabledColor;
        [SerializeField] protected float temporaryMaskDuration = 0.18f;

        #endregion

        #region Cached Fields

        protected TileData tileData;
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
            self.color = disabledColor;
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
            self.color = normalColor;
            board.EnteredTile = this;
        }
        public abstract void OnPointerClick(PointerEventData eventData);
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            self.color = disabledColor;
            board.EnteredTile = null;
        }
        internal Coordinate GetTileCoordinatesAt(Directions direction) =>
            direction switch
            {
                Directions.Up => tileData.Coordinates.Up,
                Directions.Down => tileData.Coordinates.Down,
                Directions.Left => tileData.Coordinates.Left,
                Directions.Right => tileData.Coordinates.Right,
                _ => null
            };

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