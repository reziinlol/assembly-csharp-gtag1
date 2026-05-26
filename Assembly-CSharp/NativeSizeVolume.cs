using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000427 RID: 1063
public class NativeSizeVolume : MonoBehaviour
{
	// Token: 0x06001943 RID: 6467 RVA: 0x0008DD30 File Offset: 0x0008BF30
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onEnterAction = this.OnEnterAction;
		if (onEnterAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onEnterAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x0008DD88 File Offset: 0x0008BF88
	private void OnTriggerExit(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onExitAction = this.OnExitAction;
		if (onExitAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onExitAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x04002441 RID: 9281
	[SerializeField]
	private Collider triggerVolume;

	// Token: 0x04002442 RID: 9282
	[SerializeField]
	private NativeSizeChangerSettings settings;

	// Token: 0x04002443 RID: 9283
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnEnterAction;

	// Token: 0x04002444 RID: 9284
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnExitAction;

	// Token: 0x02000428 RID: 1064
	[Serializable]
	private enum NativeSizeVolumeAction
	{
		// Token: 0x04002446 RID: 9286
		None,
		// Token: 0x04002447 RID: 9287
		ApplySettings,
		// Token: 0x04002448 RID: 9288
		ResetSize
	}
}
