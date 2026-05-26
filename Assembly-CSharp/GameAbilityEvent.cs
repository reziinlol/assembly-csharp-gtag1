using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200071C RID: 1820
[Serializable]
public class GameAbilityEvent
{
	// Token: 0x06002E27 RID: 11815 RVA: 0x000FCEA8 File Offset: 0x000FB0A8
	public void Reset()
	{
		this.played = false;
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x000FCEB4 File Offset: 0x000FB0B4
	public void TryPlay(float abilityTime, AudioSource audioSource)
	{
		if (abilityTime < this.time || this.played)
		{
			return;
		}
		this.played = true;
		if (this.sound.IsValid())
		{
			this.sound.Play(audioSource);
		}
		for (int i = 0; i < this.triggerEvent.Count; i++)
		{
			this.triggerEvent[i].Invoke();
		}
	}

	// Token: 0x04003B42 RID: 15170
	public float time;

	// Token: 0x04003B43 RID: 15171
	public AbilitySound sound;

	// Token: 0x04003B44 RID: 15172
	public List<UnityEvent> triggerEvent;

	// Token: 0x04003B45 RID: 15173
	[NonSerialized]
	public bool played;
}
