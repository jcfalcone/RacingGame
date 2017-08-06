using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPowerUp : MonoBehaviour {

    bool usedPowerUp = false;


    [SerializeField]
    [Range(1f, 20f)]
    float restartTime = 10f;

    [SerializeField]
    GameObject modelPowerUp;

    [SerializeField]
    GameObject modelParticle;

    [SerializeField]
    ParticleSystem deathParticle;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car") && !usedPowerUp)
        {
            usedPowerUp = true;
            other.SendMessageUpwards("RandomItem", SendMessageOptions.DontRequireReceiver);

            modelPowerUp.SetActive(false);
            modelParticle.SetActive(false);
            deathParticle.Play();

            StartCoroutine(reset());
        }
    }

    IEnumerator reset()
    {
        yield return new WaitForSeconds(this.restartTime);
        usedPowerUp = false;
        modelPowerUp.SetActive(true);
        modelParticle.SetActive(true);
    }
}
