using MewVivor.Managers;
using UnityEngine;

namespace MewVivor.InGame.Entity
{
    public class LifecycleTimer : MonoBehaviour
    {
        [SerializeField] private float _timer;
        [SerializeField] private bool _isPooling;
        
        private void Start()
        {
            Invoke(nameof(Destroy), _timer);
        }

        private void Destroy()
        {
            if (_isPooling)
            {
                Manager.I.Pool.ReleaseObject(transform.name, gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}