using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessfulImageUploadEvent : MonoBehaviour
{
    public AudioSource imageUpload_audioSource;

    public AudioClip[] localAudioClip_List;

    public ParticleSystem localImageUpload_particle;

    public ItemEditStateMachine itemEditStateMachine;
    public void SuccessfulLocalImageUploadSoundEvent()
    {

        imageUpload_audioSource.pitch = Random.Range(0.9f, 1.1f);
        imageUpload_audioSource.PlayOneShot(localAudioClip_List[Random.Range(0, localAudioClip_List.Length)], 1.0f);
        imageUpload_audioSource.Play();

        localImageUpload_particle.transform.position = itemEditStateMachine.currentObjectEditing.transform.position;
        localImageUpload_particle.GetComponent<ParticleSystemRenderer>().material = itemEditStateMachine.currentObjectEditing.GetComponent<PosterMeshCreator>().meshRenderer.material;
        Invoke("PlayParticles", 0.1f); // SHOULD USE A COROUTINE TO CHECK INSTEAD!!!
    }

    void PlayParticles()
    {
    localImageUpload_particle.Play();       
    }

}
