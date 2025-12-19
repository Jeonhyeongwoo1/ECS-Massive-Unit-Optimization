using Unity.Mathematics;
using UnityEngine;

namespace MewVivor.InGame.Entity
{
    public class MoveComponent : MonoBehaviour
    {
        public Vector3 Direction => _direction;
        
        private Rigidbody2D _rigidbody;
        private Transform _indicatorTransform;
        private Transform _pivotTransform;
        private Vector2 _direction;
        private float _moveSpeed;
        private bool _isStop;

        public void Initialize(Rigidbody2D rigidbody2D, Transform indicatorTransform, Transform pivotTransform, float moveSpeed)
        {
            _rigidbody = rigidbody2D;
            _indicatorTransform = indicatorTransform;
            _pivotTransform = pivotTransform;
            _moveSpeed = moveSpeed;
        }
        
        private void FixedUpdate()
        {
            if (_isStop)
            {
                return;
            }
            
            Move();
            Rotate();
        }

        public void SetStop(bool isStop)
        {
            _isStop = isStop;
        }

        public void SetMoveSpeed(float moveSpeed)
        {
            _moveSpeed = moveSpeed;
        }
        
        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }
        
        private void Move()
        {
            Vector3 position = transform.position;
            Vector3 prevPosition = position;
            Vector3 nextPosition = position + (Vector3)_direction;
            Vector3 lerp = Vector3.Lerp(prevPosition, nextPosition,
                Time.fixedDeltaTime * _moveSpeed);
            _rigidbody.MovePosition(lerp);
        }

        private void Rotate()
        {
            if (_direction == Vector2.zero)
            {
                return;
            }
            
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            _indicatorTransform.rotation = Quaternion.Euler(0, 0, angle);
            bool isLeft = math.sign(_direction.x) != 1;
            _pivotTransform.localScale = new Vector3(isLeft ? -1 : 1, 1, 1);
        }
    }
}