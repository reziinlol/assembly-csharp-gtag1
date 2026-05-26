using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200049D RID: 1181
public class GorillaReportButton : MonoBehaviour
{
	// Token: 0x06001C96 RID: 7318 RVA: 0x0009AD6E File Offset: 0x00098F6E
	public void AssignParentLine(GorillaPlayerScoreboardLine parent)
	{
		this.parentLine = parent;
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x0009AD78 File Offset: 0x00098F78
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time)
		{
			this.isOn = !this.isOn;
			this.UpdateColor();
			this.selected = !this.selected;
			this.touchTime = Time.time;
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 0.05f);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
				{
					67,
					false,
					0.05f
				});
			}
		}
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x0009AE6B File Offset: 0x0009906B
	private void OnTriggerExit(Collider other)
	{
		if (this.metaReportType != GorillaReportButton.MetaReportReason.Cancel)
		{
			other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null;
		}
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x0009AE83 File Offset: 0x00099083
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
	}

	// Token: 0x040026A1 RID: 9889
	public GorillaReportButton.MetaReportReason metaReportType;

	// Token: 0x040026A2 RID: 9890
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x040026A3 RID: 9891
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x040026A4 RID: 9892
	public bool isOn;

	// Token: 0x040026A5 RID: 9893
	public Material offMaterial;

	// Token: 0x040026A6 RID: 9894
	public Material onMaterial;

	// Token: 0x040026A7 RID: 9895
	public string offText;

	// Token: 0x040026A8 RID: 9896
	public string onText;

	// Token: 0x040026A9 RID: 9897
	public Text myText;

	// Token: 0x040026AA RID: 9898
	public float debounceTime = 0.25f;

	// Token: 0x040026AB RID: 9899
	public float touchTime;

	// Token: 0x040026AC RID: 9900
	public bool testPress;

	// Token: 0x040026AD RID: 9901
	public bool selected;

	// Token: 0x0200049E RID: 1182
	[SerializeField]
	public enum MetaReportReason
	{
		// Token: 0x040026AF RID: 9903
		HateSpeech,
		// Token: 0x040026B0 RID: 9904
		Cheating,
		// Token: 0x040026B1 RID: 9905
		Toxicity,
		// Token: 0x040026B2 RID: 9906
		Bullying,
		// Token: 0x040026B3 RID: 9907
		Doxing,
		// Token: 0x040026B4 RID: 9908
		Impersonation,
		// Token: 0x040026B5 RID: 9909
		Submit,
		// Token: 0x040026B6 RID: 9910
		Cancel
	}
}
