using UnityEngine;

namespace MewVivor.View
{
    public abstract class BaseUI : MonoBehaviour
    {
        public bool IsInitialize { get; protected set; }

        public virtual void Initialize()
        {
            if (IsInitialize)
            {
                return;
            }

            IsInitialize = true;
        }

    }
}