using Sirenix.OdinInspector;
using UnityEngine;

namespace Phoenix
{
    [CreateAssetMenu(fileName = "new Shape", menuName = "MicroBots/Shape", order = 0)]
    public class Shape : SerializedScriptableObject
    {
        [ReadOnly] public MicroBot rootBot;
        [ReadOnly] public int botsInShape;
        
    }
}