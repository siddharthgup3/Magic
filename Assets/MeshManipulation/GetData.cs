using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

public class GetData : SerializedMonoBehaviour
{

    [Button("GetMeshData")]
    public void GetMeshData()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = skinnedMeshRenderer.sharedMesh;

        NativeArray<byte> bonesPerVertex = mesh.GetBonesPerVertex();
        NativeArray<BoneWeight1> boneWeights = mesh.GetAllBoneWeights();

        int boneIndex = 0;

        for (int vertIndex = 0; vertIndex < mesh.vertexCount; vertIndex++)
        {
            int boneCountForThisVertex = bonesPerVertex[vertIndex];
            Debug.Log($"This ({vertIndex}) vertex has {boneCountForThisVertex} bones");

            for (int i = 0; i < boneCountForThisVertex; i++)
            {
                var currentBoneWeight = boneWeights[boneIndex];
                Debug.Log($"Bone weight {currentBoneWeight.boneIndex} == {currentBoneWeight.weight}");
                boneIndex++;
            }
        }
        
        Debug.Log($"");
    }
}
