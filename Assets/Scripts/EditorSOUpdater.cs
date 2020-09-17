using Sirenix.OdinInspector;
using UnityEngine;

namespace Phoenix
{
    public class EditorSOUpdater : SerializedMonoBehaviour
    {
        public GameObject prefabRootObject;
        public Shape targetSO;

        [Button("Fill SO")]
        public void UpdateSO()
        {
            MicroBot rootBot;
            if (prefabRootObject.TryGetComponent(out Renderer _r))
            {
                rootBot = new MicroBot(null, prefabRootObject.transform,
                    prefabRootObject.GetComponent<Renderer>().sharedMaterial.color);
                
            }
            else
                rootBot = new MicroBot(null, prefabRootObject.transform);

            rootBot.FillTreeFromRoot();

            targetSO.rootBot = rootBot;
            targetSO.botsInShape = prefabRootObject.GetComponentsInChildren<Renderer>().Length;
            targetSO.bonesInShape = prefabRootObject.GetComponentsInChildren<Transform>().Length - targetSO.botsInShape;
        }
    }
}