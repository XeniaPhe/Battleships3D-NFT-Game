using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleShips.GameComponents.Tiles
{
    internal abstract class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region Serialized Fields

        [SerializeField] protected SpriteRenderer mask;
        [SerializeField] protected Color maskColor;
        [SerializeField] protected Color shipColor;
        [SerializeField] protected float temporaryMaskDuration = 0.18f;
        [SerializeField] protected TileData tileData;

        #endregion

        #region Cached Fields

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
            mask.color = maskColor;
            maskWait = new WaitForSeconds(temporaryMaskDuration);
        }

        protected virtual void Start()
        {
            if (!board) board = GameBoard.Instance;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            mask.gameObject.SetActive(true);
            board.EnteredTile = this;
        }
        public abstract void OnPointerClick(PointerEventData eventData);
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            mask.gameObject.SetActive(false);
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