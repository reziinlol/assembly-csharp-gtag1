using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000030 RID: 48
[RequireComponent(typeof(AudioSource))]
public class PlayAudioSourceDelay : MonoBehaviour
{
	// Token: 0x060000B0 RID: 176 RVA: 0x0000545D File Offset: 0x0000365D
	public IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(this._delay);
		base.GetComponent<AudioSource>().GTPlay();
		yield break;
	}

	// Token: 0x040000CD RID: 205
	[SerializeField]
	private float _delay;
}
