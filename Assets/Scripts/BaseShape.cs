using UnityEngine;

namespace Phoenix
{
    public abstract class BaseShape : MonoBehaviour
    {
        public abstract void BeforeStart();
        public abstract void BeforeDestroy();
    }
}