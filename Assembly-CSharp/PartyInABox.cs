using System;
using UnityEngine;

// Token: 0x020002EA RID: 746
public class PartyInABox : MonoBehaviour
{
	// Token: 0x06001302 RID: 4866 RVA: 0x00064B2A File Offset: 0x00062D2A
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x00064B2A File Offset: 0x00062D2A
	private void OnEnable()
	{
		this.Reset();
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x00064B32 File Offset: 0x00062D32
	public void Cranked_ReleaseParty()
	{
		if (!this.parentHoldable.IsLocalObject())
		{
			return;
		}
		this.ReleaseParty();
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x00064B48 File Offset: 0x00062D48
	public void ReleaseParty()
	{
		if (this.isReleased)
		{
			return;
		}
		if (this.parentHoldable.IsLocalObject())
		{
			this.parentHoldable.itemState |= TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(true, this.partyHapticStrength, this.partyHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.partyHapticStrength, this.partyHapticDuration);
		}
		this.isReleased = true;
		this.spring.enabled = true;
		this.anim.Play();
		this.particles.Play();
		this.partyAudio.Play();
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x00064BE4 File Offset: 0x00062DE4
	private void Update()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			return;
		}
		if (this.parentHoldable.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.isReleased)
			{
				this.ReleaseParty();
				return;
			}
		}
		else if (this.isReleased)
		{
			this.Reset();
		}
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x00064C3C File Offset: 0x00062E3C
	public void Reset()
	{
		this.isReleased = false;
		this.parentHoldable.itemState &= (TransferrableObject.ItemStates)(-2);
		this.spring.enabled = false;
		this.anim.Stop();
		foreach (PartyInABox.ForceTransform forceTransform in this.forceTransforms)
		{
			forceTransform.Apply();
		}
	}

	// Token: 0x04001733 RID: 5939
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x04001734 RID: 5940
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x04001735 RID: 5941
	[SerializeField]
	private Animation anim;

	// Token: 0x04001736 RID: 5942
	[SerializeField]
	private SpringyWobbler spring;

	// Token: 0x04001737 RID: 5943
	[SerializeField]
	private AudioSource partyAudio;

	// Token: 0x04001738 RID: 5944
	[SerializeField]
	private float partyHapticStrength;

	// Token: 0x04001739 RID: 5945
	[SerializeField]
	private float partyHapticDuration;

	// Token: 0x0400173A RID: 5946
	private bool isReleased;

	// Token: 0x0400173B RID: 5947
	[SerializeField]
	private PartyInABox.ForceTransform[] forceTransforms;

	// Token: 0x020002EB RID: 747
	[Serializable]
	private struct ForceTransform
	{
		// Token: 0x06001309 RID: 4873 RVA: 0x00064C9F File Offset: 0x00062E9F
		public void Apply()
		{
			this.transform.localPosition = this.localPosition;
			this.transform.localRotation = this.localRotation;
		}

		// Token: 0x0400173C RID: 5948
		public Transform transform;

		// Token: 0x0400173D RID: 5949
		public Vector3 localPosition;

		// Token: 0x0400173E RID: 5950
		public Quaternion localRotation;
	}
}
