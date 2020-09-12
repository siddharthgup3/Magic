using UnityEngine;

namespace Phoenix
{
    public struct TransformStruct
    {
        public Vector3 position;
        public Vector3 eulerAngles;

        public TransformStruct(Vector3 position, Vector3 eulerAngles)
        {
            this.position = position;
            this.eulerAngles = eulerAngles;
        }
    }
}