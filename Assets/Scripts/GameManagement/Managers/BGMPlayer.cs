using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour {

	public AudioClip bgm;
	private AudioSource bgmAudioSource;

	private void Start()
	{
		bgmAudioSource = GetComponent<AudioSource>();
		PlayBGM();
	}

	public void PlayBGM()
	{
		bgmAudioSource.clip = bgm;
		bgmAudioSource.time = 0;
		bgmAudioSource.loop = true;

		if (bgmAudioSource.clip != null)
			bgmAudioSource.Play();
	}
}
