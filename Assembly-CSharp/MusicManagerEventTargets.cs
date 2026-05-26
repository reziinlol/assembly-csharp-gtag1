using System;
using UnityEngine;

// Token: 0x02000423 RID: 1059
public class MusicManagerEventTargets : MonoBehaviour
{
	// Token: 0x06001937 RID: 6455 RVA: 0x0008DCA0 File Offset: 0x0008BEA0
	public void StopAllMusic()
	{
		this.StopAllMusic(null);
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x0008DCA9 File Offset: 0x0008BEA9
	public void StopAllMusic(AudioClip clip)
	{
		MusicManager.StopAllMusic(clip);
	}
}
