using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000C82 RID: 3202
public class GorillaNetworkPublicTestJoin2 : GorillaTriggerBox
{
	// Token: 0x06004F7E RID: 20350 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Awake()
	{
	}

	// Token: 0x06004F7F RID: 20351 RVA: 0x001A5464 File Offset: 0x001A3664
	public void LateUpdate()
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

	// Token: 0x06004F80 RID: 20352 RVA: 0x001A5598 File Offset: 0x001A3798
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

	// Token: 0x04006135 RID: 24885
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04006136 RID: 24886
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04006137 RID: 24887
	public string gameModeName;

	// Token: 0x04006138 RID: 24888
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04006139 RID: 24889
	public string componentTypeToAdd;

	// Token: 0x0400613A RID: 24890
	public GameObject componentTarget;

	// Token: 0x0400613B RID: 24891
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x0400613C RID: 24892
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x0400613D RID: 24893
	private Transform tosPition;

	// Token: 0x0400613E RID: 24894
	private Transform othsTosPosition;

	// Token: 0x0400613F RID: 24895
	private PhotonView fotVew;

	// Token: 0x04006140 RID: 24896
	private bool waiting;

	// Token: 0x04006141 RID: 24897
	private Vector3 lastPosition;

	// Token: 0x04006142 RID: 24898
	private VRRig tempRig;
}
