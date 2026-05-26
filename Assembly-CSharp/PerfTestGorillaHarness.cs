using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020003C7 RID: 967
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST && !BETA")]
public class PerfTestGorillaHarness : MonoBehaviour
{
	// Token: 0x06001728 RID: 5928 RVA: 0x00085E18 File Offset: 0x00084018
	private void Awake()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in base.GetComponentsInChildren<PerfTestGorillaSlot>())
		{
			if (perfTestGorillaSlot.slotType == PerfTestGorillaSlot.SlotType.VR_PLAYER)
			{
				this._vrSlot = perfTestGorillaSlot;
			}
			else
			{
				this.dummySlots.Add(perfTestGorillaSlot);
			}
		}
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x00085E5C File Offset: 0x0008405C
	private void Update()
	{
		if (!this._isRecording)
		{
			return;
		}
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			float y = perfTestGorillaSlot.localStartPosition.y + Mathf.Sin(Time.time * this.bounceSpeed) * this.bounceAmplitude;
			perfTestGorillaSlot.transform.localPosition = new Vector3(perfTestGorillaSlot.localStartPosition.x, y, perfTestGorillaSlot.localStartPosition.z);
		}
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x00085F00 File Offset: 0x00084100
	public void StartRecording()
	{
		this._isRecording = true;
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x00085F0C File Offset: 0x0008410C
	public void StopRecording()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			perfTestGorillaSlot.transform.localPosition = perfTestGorillaSlot.localStartPosition;
		}
		this._isRecording = false;
	}

	// Token: 0x04002261 RID: 8801
	public PerfTestGorillaSlot _vrSlot;

	// Token: 0x04002262 RID: 8802
	public List<PerfTestGorillaSlot> dummySlots = new List<PerfTestGorillaSlot>(19);

	// Token: 0x04002263 RID: 8803
	private bool _isRecording;

	// Token: 0x04002264 RID: 8804
	private float _nextRandomMoveTime;

	// Token: 0x04002265 RID: 8805
	private float bounceSpeed = 5f;

	// Token: 0x04002266 RID: 8806
	private float bounceAmplitude = 0.5f;
}
