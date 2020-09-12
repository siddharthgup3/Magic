using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace Phoenix
{
    public class Testing : SerializedMonoBehaviour
    {
        public Transform leftHead;
        public Transform rightHead;
        public Transform middleHead;
        public int rigSize;
        public GameObject[] rigObjects;
        
        private GameObject bleh;    
        
        [ShowInInspector] [ReadOnly] private Animator _animator;
        [ShowInInspector] [ReadOnly] private RigBuilder _rigBuilder;
        
        [Button("Bla")]
        private void Start()
        {
            _animator = this.gameObject.AddComponent<Animator>();
            _rigBuilder = this.gameObject.AddComponent<RigBuilder>();
            
            rigObjects = new GameObject[rigSize];
            for (int i = 0; i < rigSize; i++)
            {
                
            }
        
        }
    

    }
}