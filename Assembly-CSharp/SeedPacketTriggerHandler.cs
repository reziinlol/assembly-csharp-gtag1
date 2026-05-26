using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000590 RID: 1424
[RequireComponent(typeof(OnTriggerEventsCosmetic))]
public class SeedPacketTriggerHandler : MonoBehaviour
{
	// Token: 0x06002412 RID: 9234 RVA: 0x000C1AEF File Offset: 0x000BFCEF
	public void OnTriggerEntered()
	{
		if (this.toggleOnceOnly && this.triggerEntered)
		{
			return;
		}
		this.triggerEntered = true;
		UnityEvent<SeedPacketTriggerHandler> unityEvent = this.onTriggerEntered;
		if (unityEvent != null)
		{
			unityEvent.Invoke(this);
		}
		this.ToggleEffects();
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000C1B24 File Offset: 0x000BFD24
	public void ToggleEffects()
	{
		if (this.particleToPlay)
		{
			this.particleToPlay.Play();
		}
		if (this.soundBankPlayer)
		{
			this.soundBankPlayer.Play();
		}
		if (this.destroyOnTriggerEnter)
		{
			if (this.destroyDelay > 0f)
			{
				base.Invoke("Destroy", this.destroyDelay);
				return;
			}
			this.Destroy();
		}
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x000C1B8E File Offset: 0x000BFD8E
	private void Destroy()
	{
		this.triggerEntered = false;
		if (ObjectPools.instance.DoesPoolExist(base.gameObject))
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04002F53 RID: 12115
	[SerializeField]
	private ParticleSystem particleToPlay;

	// Token: 0x04002F54 RID: 12116
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04002F55 RID: 12117
	[SerializeField]
	private bool destroyOnTriggerEnter;

	// Token: 0x04002F56 RID: 12118
	[SerializeField]
	private float destroyDelay = 1f;

	// Token: 0x04002F57 RID: 12119
	[SerializeField]
	private bool toggleOnceOnly;

	// Token: 0x04002F58 RID: 12120
	[HideInInspector]
	public UnityEvent<SeedPacketTriggerHandler> onTriggerEntered;

	// Token: 0x04002F59 RID: 12121
	private bool triggerEntered;
}
