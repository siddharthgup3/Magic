using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ParticleSystemTest : SerializedMonoBehaviour
{
    public Material material;
    
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh mesh;

    private ParticleSystem particles;
    private ParticleSystem.MainModule particlesMain;
    private ParticleSystemRenderer particlesRenderer;
    private ParticleSystem.EmissionModule particlesEmission;

    private int maxParticles;

    private ParticleSystem.Particle[] allParticles;
    
    
    //Get vertices in List
    //
    private void Start()
    {
        
        mesh = (Mesh) Instantiate(skinnedMeshRenderer.sharedMesh);
        
        skinnedMeshRenderer.sharedMesh = mesh;

        maxParticles = mesh.vertices.Length;
        

        particles = gameObject.AddComponent<ParticleSystem>();
        particlesMain = particles.main;

        //particlesMain.duration = 10f;
        
        particlesMain.loop = true;
        particlesMain.startLifetime = 10;
        particlesMain.maxParticles = maxParticles;
        particlesMain.simulationSpace = ParticleSystemSimulationSpace.World;
        particlesMain.startSpeed = 0f;
        particlesMain.startSize = 0.2f;

        particlesRenderer = GetComponent<ParticleSystemRenderer>();
        particlesRenderer.material = material;

        particlesEmission = particles.emission;
        particlesEmission.enabled = true;
        particlesEmission.rateOverTime = maxParticles;
        
        
        mesh = new Mesh();
        
        
    }

    private void Update()
    {
        allParticles = new ParticleSystem.Particle[particles.particleCount];
        int count = particles.GetParticles(allParticles);

        for (int i = 0; i < allParticles.Length; i++)
        {
            allParticles[i].position = transform.TransformPoint(mesh.vertices[i]);
        }    
        particles.SetParticles(allParticles,count);
    }

    private void LateUpdate()
    {
        skinnedMeshRenderer.BakeMesh(mesh);
    }
}
