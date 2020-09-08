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
            MicroBot rootBot = new MicroBot(null,prefabRootObject.transform,prefabRootObject.GetComponent<Renderer>().sharedMaterial.color);
            rootBot.FillTreeFromRoot();

            targetSO.rootBot = rootBot;
            targetSO.botsInShape = prefabRootObject.GetComponentsInChildren<Transform>().Length;
        }
    }
}