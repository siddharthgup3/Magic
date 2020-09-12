using UnityEngine;

namespace Phoenix
{
    public class OutgoingShape
    {
        public Transform targetTransform;
        public Shape targetShape;

        public OutgoingShape(Transform targetTransform, Shape targetShape)
        {
            this.targetTransform = targetTransform;
            this.targetShape = targetShape;
        }
    }
}