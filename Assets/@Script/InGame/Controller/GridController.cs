using System.Collections.Generic;
using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.InGame.Controller
{
    public class Cell
    {
        public readonly HashSet<DropItemController> DropItemList = new();
    }
    
    public class GridController : MonoBehaviour
    {
        public Grid Grid => _grid;
        
        private Grid _grid;
        private Dictionary<Vector3Int, Cell> _cellDict = new();
        private List<DropItemController> _cachedDropItemControllerList = new();
        private List<TileController> _tilemapList = new();
        private TileController _currentTilemapOnPlayer = null;
        
        private void Awake()
        {
            _grid = GetComponentInChildren<Grid>();
            var childs = GetComponentsInChildren<TileController>(true);
            foreach (TileController tile in childs)
            {
                _tilemapList.Add(tile);
            }
        }

        public void AddItem(Vector3 position, DropItemController dropItemController)
        {
            Vector3Int pos = _grid.WorldToCell(position);
            if (!_cellDict.TryGetValue(pos, out Cell itemCell))
            {
                itemCell = new();
                itemCell.DropItemList.Add(dropItemController);
                _cellDict.Add(pos, itemCell);
                return;
            }

            itemCell.DropItemList.Add(dropItemController);
            _cellDict[pos] = itemCell;
        }

        public void RemoveAllItem(DropableItemType dropableItemType)
        {
            foreach (var (key, value) in _cellDict)
            {
                value.DropItemList.RemoveWhere(v => v.DropableItemType == dropableItemType);
            }
        }

        public void RemoveItem(DropItemController item)
        {
            foreach (var keyValuePair in _cellDict)
            {
                if (keyValuePair.Value.DropItemList.Contains(item))
                {
                    keyValuePair.Value.DropItemList.Remove(item);
                }
            }
        }

        public List<DropItemController> GetDropItem(Vector3 position, float range = 3)
        {
            _cachedDropItemControllerList.Clear();
            Vector3Int left = _grid.WorldToCell(position + new Vector3(-range, 0));
            Vector3Int right = _grid.WorldToCell(position + new Vector3(range, 0));
            Vector3Int top = _grid.WorldToCell(position + new Vector3(0, range));
            Vector3Int bottom = _grid.WorldToCell(position + new Vector3(0, -range));

            for (int x = left.x; x <= right.x; x++)
            {
                for (int y = bottom.y; y <= top.y; y++)
                {
                    Vector3Int value = new Vector3Int(x, y, 0);
                    if (!_cellDict.ContainsKey(value))
                    {
                        continue;
                    }
                    if (_cellDict.TryGetValue(value, out var cell))
                    {
                        _cachedDropItemControllerList.AddRange(cell.DropItemList);
                    }
                }
            }

            return _cachedDropItemControllerList;
        }
        
        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            return Grid.WorldToCell(worldPosition);
        }

        public Vector3 CellToWorld(Vector3Int cellPosition)
        {
            return Grid.CellToWorld(cellPosition);
        }
        
        private void Update()
        {
            PlayerController player = Manager.I.Object.Player;
            if (player == null)
            {
                return;
            }
            
            foreach (TileController tilemap in _tilemapList)
            {                
                Vector3Int position = tilemap.GetWorldToCell(player.Position);
                if (tilemap.HasTile(position))
                {
                    if (_currentTilemapOnPlayer == tilemap)
                    {
                        break;
                    }
                    
                    _currentTilemapOnPlayer = tilemap;
                    _currentTilemapOnPlayer.UpdateTile();
                    break;
                }
            }
        }
    }
}
