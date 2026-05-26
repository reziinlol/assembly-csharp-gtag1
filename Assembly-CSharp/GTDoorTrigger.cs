using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x02000345 RID: 837
public class GTDoorTrigger : MonoBehaviour
{
	// Token: 0x1700020A RID: 522
	// (get) Token: 0x0600149E RID: 5278 RVA: 0x0006DFA1 File Offset: 0x0006C1A1
	public int overlapCount
	{
		get
		{
			return this.overlappingColliders.Count;
		}
	}

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x0600149F RID: 5279 RVA: 0x0006DFAE File Offset: 0x0006C1AE
	public bool TriggeredThisFrame
	{
		get
		{
			return this.lastTriggeredFrame == Time.frameCount;
		}
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x0006DFC0 File Offset: 0x0006C1C0
	public void ValidateOverlappingColliders()
	{
		for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
		{
			if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
			{
				this.overlappingColliders.RemoveAt(i);
			}
		}
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x0006E030 File Offset: 0x0006C230
	private void OnTriggerEnter(Collider other)
	{
		if (!this.overlappingColliders.Contains(other))
		{
			this.overlappingColliders.Add(other);
		}
		this.lastTriggeredFrame = Time.frameCount;
		this.TriggeredEvent.Invoke();
		if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
		{
			this.timeline.Play();
		}
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x0006E0B4 File Offset: 0x0006C2B4
	private void OnTriggerExit(Collider other)
	{
		this.overlappingColliders.Remove(other);
	}

	// Token: 0x04001954 RID: 6484
	[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
	public PlayableDirector timeline;

	// Token: 0x04001955 RID: 6485
	private int lastTriggeredFrame = -1;

	// Token: 0x04001956 RID: 6486
	private List<Collider> overlappingColliders = new List<Collider>(20);

	// Token: 0x04001957 RID: 6487
	internal UnityEvent TriggeredEvent = new UnityEvent();
}
