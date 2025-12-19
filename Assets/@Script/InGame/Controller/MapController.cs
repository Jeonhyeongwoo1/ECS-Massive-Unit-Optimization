using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MewVivor.Controller;
using MewVivor.InGame.Controller;
using UnityEngine;

namespace MewVivor.InGame.Entity
{
    public class MapController : MonoBehaviour
    {
        public GridController Grid => _grid;
        
        private GridController _grid;
        
        private void Awake()
        {
            _grid = GetComponentInChildren<GridController>();
        }

        public void AddItemInGrid(Vector3 position, DropItemController dropItemController)
        {
            _grid.AddItem(position, dropItemController);
        }
    }
}