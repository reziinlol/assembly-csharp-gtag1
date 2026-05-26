using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200032F RID: 815
public static class GTAudioSourceExtensions
{
	// Token: 0x0600141F RID: 5151 RVA: 0x0006C8B3 File Offset: 0x0006AAB3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayOneShot(this AudioSource audioSource, IList<AudioClip> clips, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)], volumeScale);
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0006C8CE File Offset: 0x0006AACE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayOneShot(this AudioSource audioSource, AudioClip clip, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clip, volumeScale);
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x0006C8D8 File Offset: 0x0006AAD8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlay(this AudioSource audioSource)
	{
		audioSource.Play();
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x0006C8E0 File Offset: 0x0006AAE0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlay(this AudioSource audioSource, ulong delay)
	{
		audioSource.Play(delay);
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x0006C8E9 File Offset: 0x0006AAE9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPause(this AudioSource audioSource)
	{
		audioSource.Pause();
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0006C8F1 File Offset: 0x0006AAF1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTUnPause(this AudioSource audioSource)
	{
		audioSource.UnPause();
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0006C8F9 File Offset: 0x0006AAF9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTStop(this AudioSource audioSource)
	{
		audioSource.Stop();
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x0006C901 File Offset: 0x0006AB01
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayDelayed(this AudioSource audioSource, float delay)
	{
		audioSource.PlayDelayed(delay);
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x0006C90A File Offset: 0x0006AB0A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayScheduled(this AudioSource audioSource, double time)
	{
		audioSource.PlayScheduled(time);
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x0006C913 File Offset: 0x0006AB13
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position)
	{
		AudioSource.PlayClipAtPoint(clip, position);
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x0006C91C File Offset: 0x0006AB1C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
	{
		AudioSource.PlayClipAtPoint(clip, position, volume);
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x000028C5 File Offset: 0x00000AC5
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	private static void _BetaLogIfAudioSourceIsNotActiveAndEnabled(AudioSource audioSource)
	{
	}
}
