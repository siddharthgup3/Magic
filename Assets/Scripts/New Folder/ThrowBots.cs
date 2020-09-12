using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Phoenix
{
    public class ThrowBots : MonoBehaviour
    {
        private MorphGod morphGod;
        private List<TransformStruct> randomTargets;
        private List<MicroBot> poolToList;

        private bool moving;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private void Start()
        {
            morphGod = FindObjectOfType<MorphGod>();
            randomTargets = new List<TransformStruct>();
            morphGod.botPool = new Queue<MicroBot>();
            poolToList = new List<MicroBot>();
            for (int i = 0; i < 1000; i++)
            {
                var newPoolObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newPoolObject.transform.SetParent(morphGod.gameObject.transform);
                newPoolObject.transform.position = transform.position;
                newPoolObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                newPoolObject.TryGetComponent(out Renderer _r);
                var _color = Random.ColorHSV();
                _r.material.color = _color;
                _r.material.EnableKeyword("_EMISSION");
                _r.material.EnableKeyword("_EMISSIONCOLOR");
                _r.material.EnableKeyword("_EmissionColor");
                _r.material.SetColor(EmissionColor,_color * 2f);

                randomTargets.Add(new TransformStruct(new Vector3(Random.Range(30, -30), 0, Random.Range(30, -30)),
                    new Vector3(Random.Range(90, -90), Random.Range(90, -90), Random.Range(90, -90))));
                
                newPoolObject.name = i.ToString() + " Pool";
                newPoolObject.SetActive(true);
                morphGod.botPool.Enqueue(new MicroBot(null, newPoolObject.transform,
                    _r.material.color));
            }

            
            poolToList = morphGod.botPool.ToList();
            moving = true;
            Invoke(nameof(StopInit), 5f);
        }

        private void Update()
        {
            if (moving)
            {
                for (int i = 0; i < poolToList.Count; i++)
                {
                    poolToList[i].botTransform.position = Vector3.MoveTowards(poolToList[i].botTransform.position,
                        randomTargets[i].position, 20f * Time.deltaTime);
                    poolToList[i].botTransform.eulerAngles = Vector3.MoveTowards(poolToList[i].botTransform.eulerAngles,
                        randomTargets[i].eulerAngles, 20f * Time.deltaTime);
                
                }
            }
        }

        private void StopInit()
        {
            moving = false;
        }
    }
}
/*
                newPoolObject.transform.position =
                    
                newPoolObject.transform.eulerAngles =
                    new Vector3(Random.Range(90, -90), Random.Range(90, -90), Random.Range(90, -90));
                    */