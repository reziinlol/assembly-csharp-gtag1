using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002C0 RID: 704
public class ElfLauncherElf : MonoBehaviour
{
	// Token: 0x0600122D RID: 4653 RVA: 0x00061748 File Offset: 0x0005F948
	private void OnEnable()
	{
		base.StartCoroutine(this.ReturnToPoolAfterDelayCo());
	}

	// Token: 0x0600122E RID: 4654 RVA: 0x00061757 File Offset: 0x0005F957
	private IEnumerator ReturnToPoolAfterDelayCo()
	{
		yield return new WaitForSeconds(this.destroyAfterDuration);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x00061766 File Offset: 0x0005F966
	private void OnCollisionEnter(Collision collision)
	{
		if (this.bounceAudioCoolingDownUntilTimestamp > Time.time)
		{
			return;
		}
		this.bounceAudio.Play();
		this.bounceAudioCoolingDownUntilTimestamp = Time.time + this.bounceAudioCooldownDuration;
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x00061793 File Offset: 0x0005F993
	private void FixedUpdate()
	{
		this.rb.AddForce(base.transform.lossyScale.x * Physics.gravity * this.rb.mass, ForceMode.Force);
	}

	// Token: 0x0400160B RID: 5643
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x0400160C RID: 5644
	[SerializeField]
	private SoundBankPlayer bounceAudio;

	// Token: 0x0400160D RID: 5645
	[SerializeField]
	private float bounceAudioCooldownDuration;

	// Token: 0x0400160E RID: 5646
	[SerializeField]
	private float destroyAfterDuration;

	// Token: 0x0400160F RID: 5647
	private float bounceAudioCoolingDownUntilTimestamp;
}
