using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Phoenix
{
    public class SphereBoss : BaseShape
    {
        private Transform playerObject;
        private float maxDist = 10f;
        private List<MicroBot> allSphereBots;
        [ReadOnly] [ShowInInspector] private List<MicroBot> renderedBots;
        private Vector3[] directions;
        [ShowInInspector] [ReadOnly] private float originalRadius;

        [SerializeField] private bool isDancing;
        [SerializeField] private bool isPerlinDancing;
        [SerializeField] private bool isTrippyDancing;
        [SerializeField] private bool blehDancing;
        private bool firstTrip = true;

        private float delta = 1f;
        private float otherDelta = 2f;

        [SerializeField] [Range(0,400)]private int movingBotsSize;
        private Transform[] movingBots;
        private int index = 0;

        public override void BeforeStart()
        {
            throw new System.NotImplementedException();
        }

        private void Start()
        {
            // playerObject = GameObject.FindWithTag("Player").transform;

            allSphereBots = new List<MicroBot>();
            renderedBots = new List<MicroBot>();

            if (TryGetComponent(out Renderer _r))
            {
                var newRoot = new MicroBot(null, transform, _r.material.color);
                allSphereBots = newRoot.GetBFS();
                renderedBots = newRoot.GetRenderedBots();
            }
            else
            {
                var newRoot = new MicroBot(null, transform);
                allSphereBots = newRoot.GetBFS();
                renderedBots = newRoot.GetRenderedBots();
            }

            Debug.Log($"Rendered bots count = {renderedBots.Count}");
            originalRadius = Vector3.Distance(transform.position,
                renderedBots[renderedBots.Count - 1].botTransform.position);
            directions = new Vector3[renderedBots.Count];
            for (int i = 0; i < renderedBots.Count; i++)
            {
                directions[i] = transform.position - renderedBots[i].botTransform.position;
            }
        }

        private void Update()
        {
            /*          Vector3 offset = playerObject.position - transform.position;
                      offset.y = 0;
                      if (offset.sqrMagnitude > maxDist)
                      {
                          var _position = transform.position;
                          Vector3 oldPos = _position;
                          oldPos.y = 0;
                          oldPos = Vector3.MoveTowards(oldPos, new Vector3(_position.x, 0, _position.z), 5);
                          _position = new Vector3(oldPos.x, _position.y, oldPos.z);
                          transform.position = _position;
                      }
          */
            if (isDancing)
                Dance();
            else if (isPerlinDancing)
                PerlinDance();
            else if (isTrippyDancing)
                TrippyDance();
            else if (blehDancing)
                FinalAttemptAtTrippyDance();
                
        }

        private void Dance()
        {
            firstTrip = true;
            for (int i = 0; i < renderedBots.Count; i++)
            {
                var temp = transform.position - renderedBots[i].botTransform.position;
                if (temp.magnitude > originalRadius) //temp.sqrMagnitude > distance * distance
                    directions[i] *= -1;

                renderedBots[i].botTransform.position += directions[i] * (1f * Time.deltaTime);
            }
        }

        private void PerlinDance()
        {
            firstTrip = true;
            for (int i = 0; i < renderedBots.Count; i++)
            {
                Vector3 pos = renderedBots[i].botTransform.position;
                var theta = Mathf.Atan(pos.y / pos.x);
                var omega = Mathf.Atan(Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.y, 2)) / pos.z);

                delta += Time.deltaTime;
                if (delta > 2)
                    delta = 1;

                var rho = Mathf.PerlinNoise(theta * delta, omega * delta);

                rho = rho * originalRadius;

                var newPos = transform.position +
                             (renderedBots[i].botTransform.position - transform.position).normalized * rho;
                renderedBots[i].botTransform.position = Vector3.MoveTowards(renderedBots[i].botTransform.position,
                    newPos, 1 * Time.deltaTime);
                //rho *= 5;
                //renderedBots[i].botTransform.position = Vector3.MoveTowards(renderedBots[i].botTransform.position,transform.position - dirToCentre.normalized * rho, 3f * Time.deltaTime);
                //var currentRadius = dirToCentre.sqrMagnitude;
                //var dirToCentre = transform.position - renderedBots[i].botTransform.position;
            }
        }

        private void TrippyDance()
        {
            if (firstTrip)
            {
                firstTrip = false;
                for (int i = 0; i < renderedBots.Count; i++)
                {
                    var botTransform = renderedBots[i].botTransform;
                    var dirToMove = (botTransform.position - transform.position).normalized;
                    botTransform.position = transform.position + dirToMove * Random.Range(0.1f, originalRadius);
                }
            }
            else
            {
                for (int i = 0; i < renderedBots.Count; i++)
                {
                    var temp = transform.position - renderedBots[i].botTransform.position;
                    if (temp.magnitude > originalRadius) //temp.sqrMagnitude > distance * distance
                        directions[i] *= -1;

                    renderedBots[i].botTransform.position += directions[i] * (1f * Time.deltaTime);
                }
            }
        }

        private void FinalAttemptAtTrippyDance()
        {
            otherDelta += Time.deltaTime;
            if (otherDelta > 2f)
            {
                otherDelta = 0;
                var bleh = GetRandomArrayWithChunkPosition(index++);
                Array.Resize(ref movingBots, movingBotsSize);
                movingBots = bleh;
            }
            for (int i = 0; i < movingBots.Length; i++)
            {
                var temp = transform.position - movingBots[i].position;
                if (temp.magnitude > originalRadius) //temp.sqrMagnitude > distance * distance
                    directions[i] *= -1;

                movingBots[i].position += directions[i] * (1f * Time.deltaTime);
            }
        }

        public override void BeforeDestroy()
        {
            throw new System.NotImplementedException();
        }

        private Transform[] GetRandomArrayWithChunkPosition(int indexOnChunk)
        {
            var chunkSize = renderedBots.Count / movingBotsSize;
            indexOnChunk %= chunkSize;
            Transform[] newMovingBots = new Transform[movingBotsSize];
            int j = 0;
            for (int i = indexOnChunk; i < renderedBots.Count; i += chunkSize)
            {
                if (j < movingBotsSize)
                    newMovingBots[j++] = renderedBots[i].botTransform;
                else
                {
                    if (j == movingBots.Length - 1)
                        break;
                    else
                    {
                        //Some number which updates i in such a way that it iterates the loop without duplicates
                        i = (indexOnChunk + 1) % chunkSize;
                    }
                }
            }
            return newMovingBots;
        }
    }
}