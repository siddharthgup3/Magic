using System;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

#pragma warning disable 0649


namespace Phoenix
{
    public class MorphManager : SerializedMonoBehaviour
    {
        [ReadOnly] public List<MicroBot> activeTreeRoots;
        
        
        private MorphGod morphGod;
        public ShapeType shapeType;                //Set in inspector to test for now
        
        public Transform target;

        public Shape circle;
        public Shape grapple;
        public Shape grep;
        private void Start()
        {
            morphGod = FindObjectOfType<MorphGod>();
            activeTreeRoots = new List<MicroBot>();

            Invoke(nameof(Bleh),5f);
        }

        private void Bleh()
        {
            Debug.Log($"Starting the morphing!!");

            OutgoingShape[] outgoingShapes = new OutgoingShape[1];
            Shape x;
            switch (shapeType)
            {
                case ShapeType.Circle: x = circle;
                    break;
                case ShapeType.Grapple : x = grapple;
                    break;
                case ShapeType.Rge : x = grep;
                    break;
                default: x = circle;
                    Debug.Log("Default value as circle");
                    break;
            }
            outgoingShapes[0] = new OutgoingShape(target, x);
            morphGod.ShapeChange(null, outgoingShapes);
            
        }

        
    }
}