using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MewVivor.InGame.Controller
{
    public enum TileDirectionType
    {
        LeftTop,
        Top,
        RightTop,
        LeftMiddle,
        Middle,
        RightMiddle,
        LeftBottom,
        Bottom,
        RightBottom,
    }
    
    [Serializable]
    public struct TileInfoData
    {
        public TileDirectionType tileDirectionType;
        public TileController tileController;
    }
    
    public class TileController : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private List<TileInfoData> _tileInfoDataList;

        private void Awake()
        {
            TryGetComponent(out _tilemap);
        }

        public bool HasTile(Vector3Int position)
        {
            return _tilemap.HasTile(position);
        }

        public Vector3Int GetWorldToCell(Vector3 position)
        {
            return _tilemap.WorldToCell(position);
        }
        
        public void UpdateTile()
        {
            BoundsInt bounds = _tilemap.cellBounds;
            Vector3 cellSize = _tilemap.cellSize;

            int widthInCells = bounds.size.x;
            int heightInCells = bounds.size.y;

            float tilemapWidth = widthInCells * cellSize.x;
            float tilemapHeight = heightInCells * cellSize.y;
            Vector3 position = transform.position;
            
            //내 현재의 transform position을 기준으로 tileMap관련 구하기
            foreach (TileInfoData tileInfoData in _tileInfoDataList)
            {
                TileController tile = tileInfoData.tileController;
                Vector3 newPosition = Vector3.zero;
                switch (tileInfoData.tileDirectionType)
                {
                    case TileDirectionType.LeftTop:
                        newPosition = position + new Vector3(-tilemapWidth, tilemapHeight);
                        tile.transform.position = newPosition; 
                        break;
                    case TileDirectionType.Top:
                        newPosition = position + new Vector3(0, tilemapHeight);
                        tile.transform.position = newPosition;
                        break;
                    case TileDirectionType.RightTop:
                        newPosition = position + new Vector3(tilemapWidth, tilemapHeight);
                        tile.transform.position = newPosition;
                        break;
                    case TileDirectionType.LeftMiddle:
                        newPosition = position + new Vector3(-tilemapWidth, 0);
                        tile.transform.position = newPosition;
                        break;
                    case TileDirectionType.Middle:
                        break;
                    case TileDirectionType.RightMiddle:
                        newPosition = position + new Vector3(tilemapWidth, 0);
                        tile.transform.position = newPosition;
                        break;
                    case TileDirectionType.LeftBottom:
                        newPosition = position + new Vector3(-tilemapWidth, -tilemapHeight);
                        tile.transform.position = newPosition;
                        break;
                    case TileDirectionType.Bottom:
                        newPosition = position + new Vector3(0, -tilemapHeight);
                        tile.transform.position = newPosition;
                        break;
                    case TileDirectionType.RightBottom:
                        newPosition = position + new Vector3(tilemapWidth, -tilemapHeight);
                        tile.transform.position = newPosition;
                        break;
                }
            }   
        }
    }
}