using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

#pragma warning disable 0649

namespace Phoenix
{
    public class MorphGod : SerializedMonoBehaviour
    {
        [ShowInInspector] [ReadOnly] private Queue<MicroBot> botPool;
        [ShowInInspector] [ReadOnly] private bool changingShape;
        [ShowInInspector] [ReadOnly] private List<MicroBot> finalList;
        [ShowInInspector] [ReadOnly] private List<MicroBot> refList;

        private void Start()
        {
            LeanTween.init(2000);

            botPool = new Queue<MicroBot>();
            for (int i = 0; i < 1000; i++)
            {
                var newPoolObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newPoolObject.transform.SetParent(transform);
                newPoolObject.GetComponent<Renderer>().material.color = Random.ColorHSV();
                newPoolObject.transform.position =
                    new Vector3(Random.Range(50, -50), 0, Random.Range(50, -50));
                newPoolObject.transform.eulerAngles =
                    new Vector3(Random.Range(90, -90), Random.Range(90, -90), Random.Range(90, -90));
                newPoolObject.name = i.ToString() + " Pool";
                newPoolObject.SetActive(true);
                botPool.Enqueue(new MicroBot(null, newPoolObject.transform,
                    newPoolObject.GetComponent<Renderer>().material.color));
            }
        }

        [Button("Begin Shape Change")]
        public void ShapeChange(IncomingShape[] incomingShapes, OutgoingShape[] outputShapes)
        {
            Profiler.BeginSample("Micken");

            List<MicroBot> involvedBots = new List<MicroBot>();

            #region Calculating Input Shapes

            if (incomingShapes != null)
            {
                foreach (var incomingShape in incomingShapes)
                {
                    var rootNode = incomingShape.GetRootNode(); //Get root node
                    rootNode.FillTreeFromRoot(); //Fill tree according to root node.
                    List<MicroBot> listOfNodesInShape = rootNode.ReverseOrderTraversal();
                    involvedBots.AddRange(listOfNodesInShape);
                }
            }

            foreach (var botInTransaction in involvedBots)
            {
                // Debug.Log($"Name = {botInTransaction.botTransform.name}", botInTransaction.botTransform.gameObject);
                botInTransaction.botTransform.SetParent(null);
                //  botInTransaction.parentBot = null;
                // botInTransaction.childrenBots.Clear();
                //TODO: Confirm I should clear the parent/child over here.
            }

            #endregion

            List<MicroBot> targetReferenceBotList = new List<MicroBot>();

            #region Calculating Output Shapes

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
                    LeanTween.move(temp.botTransform.gameObject,
                        new Vector3(Random.Range(50, -50), 0, Random.Range(50, -50)), 1f);
                    botPool.Enqueue(temp);
                }
            }
        
            #endregion
            if (outputShapes ==null)
                return;
            
            #region Magical Area

            Shape[] targetShapeSOArray = new Shape[outputShapes.Length];
            int j = 0;
            foreach (var outgoingShape in outputShapes)
            {
                targetShapeSOArray[j++] = outgoingShape.targetShape;
            }


            List<MicroBot> newOutputTrees;
            List<MicroBot> finalBotList;


            finalBotList = TreeExtensions.ListToTrees(involvedBots, targetShapeSOArray, out newOutputTrees);

            #endregion


            if (involvedBots.Count != finalBotList.Count)
            {
                Debug.Log($"invovled count = {involvedBots.Count} and the final bot count = {finalBotList.Count}");
                throw new Exception("LINE 195");
            }

            refList = targetReferenceBotList;
            finalList = finalBotList;

            /*
            for (int i = 0; i < involvedBots.Count; i++)
            {
                LeanTween.move(finalBotList[i].botTransform.gameObject, targetReferenceBotList[i].botTransform.position,
                    3f);
               // LeanTween.scale(finalBotList[i].botTransform.gameObject, targetReferenceBotList[i].botTransform.localScale, 3f);
                LeanTween.rotate(finalBotList[i].botTransform.gameObject,
                    targetReferenceBotList[i].botTransform.eulerAngles,
                    3f);
                LeanTween.color(finalBotList[i].botTransform.gameObject, targetReferenceBotList[i].botColor, 3f);
            }
            */
            changingShape = true;
            StartCoroutine(Bleh());
        }

        private void Update()
        {
            if (changingShape)
            {
                for (int i = 0; i < finalList.Count; i++)
                {
                    finalList[i].botTransform.position = Vector3.MoveTowards(finalList[i].botTransform.position,
                        refList[i].botTransform.position, 1);
                    finalList[i].botTransform.eulerAngles = Vector3.MoveTowards(finalList[i].botTransform.eulerAngles,
                        refList[i].botTransform.eulerAngles, 1);
                    //finalList[i].botTransform. = Vector3.MoveTowards( finalList[i].botTransform.eulerAngles,refList[i].botTransform.eulerAngles, 3);
                }
            }
        }


        IEnumerator Bleh()
        {
            yield return new WaitForSeconds(3f);
           // Profiler.EndSample("Micken");
            changingShape = false;
        }
    }
}