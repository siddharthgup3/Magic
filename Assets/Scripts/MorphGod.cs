using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

#pragma warning disable 0649

namespace Phoenix
{
    public class MorphGod : SerializedMonoBehaviour
    {
        private MorphManager morphManager;

        [ShowInInspector] [ReadOnly] public Queue<MicroBot> botPool;
        [ShowInInspector] [ReadOnly] private Queue<MicroBot> bonesPool;

        [ShowInInspector] [ReadOnly] private bool changingShape;
        [ShowInInspector] [ReadOnly] private List<MicroBot> finalList;
        [ShowInInspector] [ReadOnly] private List<MicroBot> refList;


        [ShowInInspector] [ReadOnly] private List<Material> finalListRenderers;
        [ShowInInspector] [ReadOnly] private List<Material> refListRenderers;

        private float progress;
        
        
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private void Start()
        {
            morphManager = FindObjectOfType<MorphManager>();
            LeanTween.init(2000);
        }


        [Button("Begin Shape Change")]
        public void ShapeChange(IncomingShape[] incomingShapes, OutgoingShape[] outputShapes)
        {
            #region Calculating Input Shapes

            List<MicroBot> involvedBots = new List<MicroBot>();
            if (incomingShapes != null)
            {
                foreach (var incomingShape in incomingShapes)
                {
                    incomingShape.rootTransform.TryGetComponent(out IMorphable _morph);
                    if (_morph != null)
                    {
                        var script = incomingShape.rootTransform.GetComponent<BaseShape>();
                        script.BeforeDestroy();
                        Destroy(script);
                    }

                    morphManager.activeTreeRoots.Remove(incomingShape.GetRootNode());

                    var rootNode = incomingShape.GetRootNode(); //Get root node
                    rootNode.FillTreeFromRoot(); //Fill tree according to root node.
                    List<MicroBot> listOfNodesInShape = rootNode.ReverseOrderTraversal();
                    involvedBots.AddRange(listOfNodesInShape);
                }
            }

            foreach (var botInTransaction in involvedBots)
            {
                botInTransaction.botTransform.SetParent(null);
                // botInTransaction.childrenBots.Clear();
                //TODO: Confirm I should clear the parent/child over here.
            }

            #endregion

            #region Calculating Output Shapes

            List<MicroBot> targetReferenceBotList = new List<MicroBot>();
            if (outputShapes != null)
            {
                foreach (var outputShape in outputShapes)
                {
                    targetReferenceBotList.AddRange(outputShape.targetShape.rootBot.GetBFS());
                }
            }

            #endregion

            #region Balancing transaction

            if (involvedBots.Count < targetReferenceBotList.Count)
            {
                for (int i = involvedBots.Count; i < targetReferenceBotList.Count; i++)
                {
                    if (botPool.Count > 0)
                    {
                        var temp = botPool.Dequeue();
                        involvedBots.Add(temp);
                    }
                    else
                        throw new Exception("Ran out of pooled objects.");
                }
            }
            else if (involvedBots.Count > targetReferenceBotList.Count)
            {
                while (involvedBots.Count > targetReferenceBotList.Count)
                {
                    var temp = involvedBots[involvedBots.Count - 1];
                    involvedBots.RemoveAt(involvedBots.Count - 1);
                    temp.botTransform.SetParent(transform);
                    temp.childrenBots.Clear();
                    temp.parentBot = null;
                    //TODO: Make separate function to get rid of back to pool objects
                    LeanTween.move(temp.botTransform.gameObject,
                        new Vector3(Random.Range(50, -50), 0, Random.Range(50, -50)), 1f);
                    botPool.Enqueue(temp);
                }
            }

            #endregion

            if (outputShapes == null)
                return;

            Shape[] targetShapeSOArray = new Shape[outputShapes.Length];
            int j = 0;
            foreach (var outgoingShape in outputShapes)
            {
                targetShapeSOArray[j++] = outgoingShape.targetShape;
            }


            var finalBotList = involvedBots.ListToTrees(targetShapeSOArray, out var newOutputTrees);

            morphManager.activeTreeRoots.AddRange(newOutputTrees);

            if (involvedBots.Count != finalBotList.Count)
                throw new Exception("LINE 195");


            refList = targetReferenceBotList;
            finalList = finalBotList;


            finalListRenderers = new List<Material>();
            refListRenderers = new List<Material>();
            for (int i = 0; i < finalList.Count; i++)
            {
                if (finalList[i].botTransform.TryGetComponent(out Renderer _finalRender) &&
                    refList[i].botTransform.TryGetComponent(out Renderer _refRender))
                {
                    _finalRender.enabled = _refRender.enabled;
                    finalListRenderers.Add(_finalRender.material);
                    refListRenderers.Add(_refRender.sharedMaterial);
                }
            }

            progress = 0f;
            changingShape = true;
            StartCoroutine(Bleh());
        }

        private void Update()
        {
            if (changingShape)
            {
                progress += Time.deltaTime;
                for (int i = 0; i < finalList.Count; i++)
                {
                    finalList[i].botTransform.position = Vector3.MoveTowards(finalList[i].botTransform.position,
                        refList[i].botTransform.position, 1);

                    finalList[i].botTransform.eulerAngles = Vector3.MoveTowards(finalList[i].botTransform.eulerAngles,
                        refList[i].botTransform.eulerAngles, 1);

                    if(refListRenderers[i]==null)
                        throw new Exception("ref null");
                    if(finalListRenderers[i]==null)
                        throw new Exception("final null");
                    
                    var _color = Color.Lerp(finalListRenderers[i].color, refListRenderers[i].color, progress);
                    finalListRenderers[i].SetColor(BaseColor, _color * 2);
                    finalListRenderers[i].SetColor(EmissionColor, _color * 2);
                }
            }
        }

        private IEnumerator Bleh()
        {
            yield return new WaitForSeconds(5f);
            Debug.Log($"Ending morph");
            changingShape = false;
        }
    }
}

/*
            botPool = new Queue<MicroBot>();
            for (int i = 0; i < 1000; i++)
            {
                var newPoolObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newPoolObject.transform.SetParent(poolGodTransform);
                newPoolObject.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
                newPoolObject.TryGetComponent(out Renderer _r);
                _r.material.color = Random.ColorHSV();
                newPoolObject.transform.position =
                    new Vector3(Random.Range(50, -50), 0, Random.Range(50, -50));
                newPoolObject.transform.eulerAngles =
                    new Vector3(Random.Range(90, -90), Random.Range(90, -90), Random.Range(90, -90));
                newPoolObject.name = i.ToString() + " Pool";
                newPoolObject.SetActive(true);
                botPool.Enqueue(new MicroBot(null, newPoolObject.transform,
                    _r.material.color));
            }
            */