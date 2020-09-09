using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Phoenix
{
    public static class TreeExtensions
    {
        public static void FillTreeFromRoot(this MicroBot rootBot)
        {
            MicroBot[] tempChildNodes = new MicroBot[rootBot.botTransform.childCount]; //instant child count

            int i = 0;
            foreach (Transform directChildTransform in rootBot.botTransform)
            {
                MicroBot childNode;
                if (!directChildTransform.TryGetComponent(out Renderer _r))
                {
                    _r = directChildTransform.gameObject.AddComponent<MeshRenderer>();
                    _r.enabled = false;
                }
                
                if(_r.enabled)
                    childNode= new MicroBot(rootBot, directChildTransform, _r.sharedMaterial.color);                              //Init all child nodes with this transform as parent
                else
                    childNode = new MicroBot(rootBot, directChildTransform);
                
                tempChildNodes[i++] = childNode;
                childNode.FillTreeFromRoot(); //Start recursion
            }

            rootBot.childrenBots = tempChildNodes.ToList();
        }

        public static List<MicroBot> ReverseOrderTraversal(this MicroBot rootBot)
        {
            List<MicroBot> finalBotList = new List<MicroBot>();

            Stack<MicroBot> nodeStack = new Stack<MicroBot>();
            Queue<MicroBot> nodeQueue = new Queue<MicroBot>();

            nodeQueue.Enqueue(rootBot);

            while (nodeQueue.Count > 0)
            {
                var temp = nodeQueue.Dequeue();
                nodeStack.Push(temp);
                foreach (var childBot in temp.childrenBots)
                {
                    nodeQueue.Enqueue(childBot);
                }
            }

            while (nodeStack.Count > 0)
            {
                var temp = nodeStack.Pop();
                finalBotList.Add(temp);
            }

            return finalBotList;
        }

        public static List<MicroBot> GetBFS(this MicroBot rootBot)
        {
            List<MicroBot> finalBotList = new List<MicroBot>();

            Queue<MicroBot> botQueue = new Queue<MicroBot>();
            botQueue.Enqueue(rootBot);
            finalBotList.Add(rootBot);
            while (botQueue.Count > 0)
            {
                var tempBot = botQueue.Dequeue();

                foreach (var childBot in tempBot.childrenBots)
                {
                    finalBotList.Add(childBot);
                    botQueue.Enqueue(childBot);
                }
            }
            return finalBotList;
        }

        public static List<MicroBot> ListToTrees(this List<MicroBot> incomingBotsList, Shape[] outShapes,
            out List<MicroBot> outgoingTrees)
        {
            outgoingTrees = new List<MicroBot>();
            var finalBotList = new List<MicroBot>();
            foreach (Shape shapeSO in outShapes)
            {
                var newTree = DeepCopyTree(shapeSO.rootBot, null);
                outgoingTrees.Add(newTree);
                finalBotList.AddRange(newTree.GetBFS());
            }


            for (int i = 0; i < incomingBotsList.Count; i++)
            {
                finalBotList[i].botTransform = incomingBotsList[i].botTransform;
                //Null conditionals not advisable for unity objects.
                finalBotList[i].botTransform
                    .SetParent(finalBotList[i].parentBot == null ? null : finalBotList[i].parentBot.botTransform);
            }

            return finalBotList;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static MicroBot DeepCopyTree(MicroBot bot, MicroBot parent = null)
        {
            if (bot == null)
                return null;

            var newBot = new MicroBot(parent, bot.botTransform, bot.botColor);
            foreach (var childBot in bot.childrenBots)
            {
                newBot.childrenBots.Add(DeepCopyTree(childBot, newBot));
            }

            return newBot;
        }
    }
}