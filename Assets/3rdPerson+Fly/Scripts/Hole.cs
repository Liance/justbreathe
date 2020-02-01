using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private bool isReleasingOxygen = false;

    public bool IsReleasingOxygen { get => isReleasingOxygen; set => isReleasingOxygen = value; }

    private HoleGenerator holeGenerator;
    private ParticleSystem particles;
    private float timeToRelease = 5f;
    private float timer = 0f;
    private void Awake()
    {
        holeGenerator = GetComponentInParent<HoleGenerator>();
        particles = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(IsReleasingOxygen && timer >= timeToRelease)
        {
            GameEnvironmentManager.OxygenLevel--;
            timer = 0f;
        }
    }

    public void SetHoleParticlesActive(bool isActive)
    {
        if(particles)
        {
            if(isActive)
            {
                if(particles.isStopped) particles.Play();
            }
            else
            {
                if(particles.isPlaying)
                {
                    particles.Stop();
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hole!!!!");
        if(other.gameObject.tag == "Player")
        {
            isReleasingOxygen = false;
            if(holeGenerator)
            {
                holeGenerator.SetInactive(gameObject);
                SetHoleParticlesActive(false);
            }

        }
    }
}
