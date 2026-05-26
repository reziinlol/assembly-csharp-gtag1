using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200071D RID: 1821
[Serializable]
public class GameAbilityEvents
{
	// Token: 0x06002E2A RID: 11818 RVA: 0x000FCF1C File Offset: 0x000FB11C
	public void Reset()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].Reset();
		}
	}

	// Token: 0x06002E2B RID: 11819 RVA: 0x000FCF50 File Offset: 0x000FB150
	public void OnAbilityStart(float abilityTime, AudioSource audioSource)
	{
		this.startEvent.TryPlay(abilityTime, (this.startEvent.sound.audioSource == null) ? audioSource : this.startEvent.sound.audioSource);
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x000FCF89 File Offset: 0x000FB189
	public void OnAbilityStop(float abilityTime, AudioSource audioSource)
	{
		this.stopEvent.TryPlay(abilityTime, (this.stopEvent.sound.audioSource == null) ? audioSource : this.stopEvent.sound.audioSource);
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x000FCFC4 File Offset: 0x000FB1C4
	public void TryPlay(float abilityTime, AudioSource audioSource)
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].TryPlay(abilityTime, (this.events[i].sound.audioSource == null) ? audioSource : this.events[i].sound.audioSource);
		}
	}

	// Token: 0x04003B46 RID: 15174
	public GameAbilityEvent startEvent;

	// Token: 0x04003B47 RID: 15175
	public GameAbilityEvent stopEvent;

	// Token: 0x04003B48 RID: 15176
	public List<GameAbilityEvent> events;
}
