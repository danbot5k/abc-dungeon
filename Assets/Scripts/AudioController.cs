using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public AudioSource sfxSource, musicSource;

    public static AudioController instance = null;

    public float lowPitchRange = .95f, highPitchRange = 1.05f;



	// Use this for initialization
	void Awake () {
		if(instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
	}

	
	public void PlaySingle(AudioClip clip) {
        sfxSource.clip = clip;
        sfxSource.Play();
    }
    
    public void RandomizeSfx(params AudioClip[] clips) {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        sfxSource.pitch = randomPitch;
        sfxSource.clip = clips[randomIndex];
        sfxSource.Play();
    }
    
}
