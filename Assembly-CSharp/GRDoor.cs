using System;
using UnityEngine;

// Token: 0x020007E0 RID: 2016
[Serializable]
public class GRDoor
{
	// Token: 0x06003374 RID: 13172 RVA: 0x0011B7C3 File Offset: 0x001199C3
	public void Setup()
	{
		this.doorState = GRDoor.DoorState.Closed;
	}

	// Token: 0x06003375 RID: 13173 RVA: 0x0011B7CC File Offset: 0x001199CC
	public void SetDoorState(GRDoor.DoorState newState)
	{
		if (newState == this.doorState)
		{
			return;
		}
		this.doorState = newState;
		if (this.doorState == GRDoor.DoorState.Closed)
		{
			this.animation.clip = this.closeAnim;
			this.animation.Play();
			this.closeDoorSound.Play(null);
			return;
		}
		this.animation.clip = this.openAnim;
		this.animation.Play();
		this.openDoorSound.Play(null);
	}

	// Token: 0x04004328 RID: 17192
	public GRDoor.DoorState doorState;

	// Token: 0x04004329 RID: 17193
	public Animation animation;

	// Token: 0x0400432A RID: 17194
	public AnimationClip openAnim;

	// Token: 0x0400432B RID: 17195
	public AnimationClip closeAnim;

	// Token: 0x0400432C RID: 17196
	public AbilitySound openDoorSound;

	// Token: 0x0400432D RID: 17197
	public AbilitySound closeDoorSound;

	// Token: 0x020007E1 RID: 2017
	public enum DoorState
	{
		// Token: 0x0400432F RID: 17199
		Closed,
		// Token: 0x04004330 RID: 17200
		Open
	}
}
