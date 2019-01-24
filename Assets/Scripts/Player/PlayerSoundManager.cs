using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
	public AudioClip[] basicAttackHitSounds;
	public AudioClip[] basicAttackMissSounds;

	private AudioSource playerAudioSource;

	private void Start()
	{
		playerAudioSource = GetComponent<AudioSource>();
	}

	public void PlayBasicAttackHitSound()
	{
		var randomIndex = Random.Range(0, basicAttackHitSounds.Length);
		playerAudioSource.PlayOneShot(basicAttackHitSounds[randomIndex]);
	}

	public void PlayBasicAttackMissSound()
	{
		var randomIndex = Random.Range(0, basicAttackMissSounds.Length);
		playerAudioSource.PlayOneShot(basicAttackMissSounds[randomIndex]);
	}

	public void PlayChargeAttackHitSound()
	{
		var randomIndex = 1;//Random.Range(0, basicAttackHitSounds.Length);
		playerAudioSource.PlayOneShot(basicAttackHitSounds[randomIndex]);
	}

	public void PlayChargeAttackMissSound()
	{
		var randomIndex = Random.Range(0, basicAttackMissSounds.Length);
		playerAudioSource.PlayOneShot(basicAttackMissSounds[randomIndex]);
	}

	public void PlayJumpAttackHitSound()
	{
		var randomIndex = 1;//Random.Range(0, basicAttackHitSounds.Length);
		playerAudioSource.PlayOneShot(basicAttackHitSounds[randomIndex]);
	}

	public void PlayJumpAttackMissSound()
	{
		var randomIndex = Random.Range(0, basicAttackMissSounds.Length);
		playerAudioSource.PlayOneShot(basicAttackMissSounds[randomIndex]);
	}

	public void PlaySlideAttackHitSound()
	{
		var randomIndex = 1;//Random.Range(0, basicAttackHitSounds.Length);
		playerAudioSource.PlayOneShot(basicAttackHitSounds[randomIndex]);
	}

	public void PlaySlideAttackMissSound()
	{
		var randomIndex = Random.Range(0, basicAttackMissSounds.Length);
		playerAudioSource.PlayOneShot(basicAttackMissSounds[randomIndex]);
	}
}
