using UnityEngine;

namespace Phoenix
{
    public class IncomingShape
    {
        public Transform rootTransform;
        public Shape incomingShape;

        public MicroBot GetRootNode()
        {
            var rootNode = new MicroBot(null, rootTransform, rootTransform.GetComponent<Renderer>().material.color);
            return rootNode;
        }
    }
}