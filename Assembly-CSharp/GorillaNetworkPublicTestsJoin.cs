using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000C84 RID: 3204
public class GorillaNetworkPublicTestsJoin : GorillaTriggerBox, ITickSystemPost
{
	// Token: 0x17000775 RID: 1909
	// (get) Token: 0x06004F88 RID: 20360 RVA: 0x001A577C File Offset: 0x001A397C
	// (set) Token: 0x06004F89 RID: 20361 RVA: 0x001A5784 File Offset: 0x001A3984
	public bool PostTickRunning { get; set; }

	// Token: 0x06004F8A RID: 20362 RVA: 0x001A578D File Offset: 0x001A398D
	public void Awake()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06004F8B RID: 20363 RVA: 0x001A5798 File Offset: 0x001A3998
	public void PostTick()
	{
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic && !this.waiting && !MonkeAgent.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				if ((GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f) && !this.waiting && !MonkeAgent.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				float magnitude = (GTPlayer.Instance.transform.position - this.lastPosition).magnitude;
			}
			this.lastPosition = GTPlayer.Instance.transform.position;
		}
		catch
		{
		}
	}

	// Token: 0x06004F8C RID: 20364 RVA: 0x001A58CC File Offset: 0x001A3ACC
	private IEnumerator GracePeriod()
	{
		this.waiting = true;
		yield return new WaitForSeconds(30f);
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic)
				{
					MonkeAgent.instance.SendReport("gorvity bisdabled", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f)
				{
					MonkeAgent.instance.SendReport(string.Concat(new string[]
					{
						"jimp 2mcuh.",
						GTPlayer.Instance.jumpMultiplier.ToString(),
						".",
						GTPlayer.Instance.maxJumpSpeed.ToString(),
						"."
					}), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GorillaTagger.Instance.sphereCastRadius > 0.04f)
				{
					MonkeAgent.instance.SendReport("wack rad. " + GorillaTagger.Instance.sphereCastRadius.ToString(), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
			}
			this.waiting = false;
			yield break;
		}
		catch
		{
			yield break;
		}
		yield break;
	}

	// Token: 0x04006146 RID: 24902
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04006147 RID: 24903
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04006148 RID: 24904
	public string gameModeName;

	// Token: 0x04006149 RID: 24905
	public PhotonNetworkController photonNetworkController;

	// Token: 0x0400614A RID: 24906
	public string componentTypeToAdd;

	// Token: 0x0400614B RID: 24907
	public GameObject componentTarget;

	// Token: 0x0400614C RID: 24908
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x0400614D RID: 24909
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x0400614E RID: 24910
	private Transform tosPition;

	// Token: 0x0400614F RID: 24911
	private Transform othsTosPosition;

	// Token: 0x04006150 RID: 24912
	private PhotonView fotVew;

	// Token: 0x04006151 RID: 24913
	private bool waiting;

	// Token: 0x04006152 RID: 24914
	private Vector3 lastPosition;

	// Token: 0x04006153 RID: 24915
	private VRRig tempRig;
}
