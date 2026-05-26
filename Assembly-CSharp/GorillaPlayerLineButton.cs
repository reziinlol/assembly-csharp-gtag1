using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000A0D RID: 2573
public class GorillaPlayerLineButton : MonoBehaviour
{
	// Token: 0x060041C1 RID: 16833 RVA: 0x0015F7C8 File Offset: 0x0015D9C8
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
	}

	// Token: 0x060041C2 RID: 16834 RVA: 0x0015F7DE File Offset: 0x0015D9DE
	private void OnDisable()
	{
		if (Application.isEditor)
		{
			base.StopAllCoroutines();
		}
	}

	// Token: 0x060041C3 RID: 16835 RVA: 0x0015F7ED File Offset: 0x0015D9ED
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
				{
					this.isOn = !this.isOn;
				}
				this.parentLine.PressButton(this.isOn, this.buttonType);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x060041C4 RID: 16836 RVA: 0x0015F7FC File Offset: 0x0015D9FC
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
			{
				if (this.isAutoOn)
				{
					this.isOn = false;
				}
				else
				{
					this.isOn = !this.isOn;
				}
			}
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute || this.buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech || this.buttonType == GorillaPlayerLineButton.ButtonType.Cheating || this.buttonType == GorillaPlayerLineButton.ButtonType.Cancel || this.parentLine.canPressNextReportButton)
			{
				this.parentLine.PressButton(this.isOn, this.buttonType);
				if (component != null)
				{
					GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
					GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
					if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
						{
							67,
							component.isLeftHand,
							0.05f
						});
					}
				}
			}
		}
	}

	// Token: 0x060041C5 RID: 16837 RVA: 0x0015F96C File Offset: 0x0015DB6C
	private void OnTriggerExit(Collider other)
	{
		if (this.buttonType != GorillaPlayerLineButton.ButtonType.Mute && other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.parentLine.canPressNextReportButton = true;
		}
	}

	// Token: 0x060041C6 RID: 16838 RVA: 0x0015F994 File Offset: 0x0015DB94
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		if (this.isAutoOn)
		{
			base.GetComponent<MeshRenderer>().material = this.autoOnMaterial;
			this.myText.text = this.autoOnText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x04005377 RID: 21367
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x04005378 RID: 21368
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x04005379 RID: 21369
	public bool isOn;

	// Token: 0x0400537A RID: 21370
	public bool isAutoOn;

	// Token: 0x0400537B RID: 21371
	public Material offMaterial;

	// Token: 0x0400537C RID: 21372
	public Material onMaterial;

	// Token: 0x0400537D RID: 21373
	public Material autoOnMaterial;

	// Token: 0x0400537E RID: 21374
	public string offText;

	// Token: 0x0400537F RID: 21375
	public string onText;

	// Token: 0x04005380 RID: 21376
	public string autoOnText;

	// Token: 0x04005381 RID: 21377
	public Text myText;

	// Token: 0x04005382 RID: 21378
	public float debounceTime = 0.25f;

	// Token: 0x04005383 RID: 21379
	public float touchTime;

	// Token: 0x04005384 RID: 21380
	public bool testPress;

	// Token: 0x02000A0E RID: 2574
	public enum ButtonType
	{
		// Token: 0x04005386 RID: 21382
		HateSpeech,
		// Token: 0x04005387 RID: 21383
		Cheating,
		// Token: 0x04005388 RID: 21384
		Toxicity,
		// Token: 0x04005389 RID: 21385
		Mute,
		// Token: 0x0400538A RID: 21386
		Report,
		// Token: 0x0400538B RID: 21387
		Cancel
	}
}
