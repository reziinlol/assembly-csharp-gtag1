using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200059F RID: 1439
public class UseableObjectEvents : MonoBehaviour
{
	// Token: 0x0600247A RID: 9338 RVA: 0x000C3DF4 File Offset: 0x000C1FF4
	public void Init(NetPlayer player)
	{
		bool isLocal = player.IsLocal;
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		string str;
		if (isLocal && instance != null)
		{
			str = instance.GetPlayFabPlayerId();
		}
		else
		{
			str = player.NickName;
		}
		this.PlayerIdString = str + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.DisposeEvents();
		this.Activate = new PhotonEvent(this.PlayerId.ToString() + ".Activate");
		this.Deactivate = new PhotonEvent(this.PlayerId.ToString() + ".Deactivate");
		this.Activate.reliable = false;
		this.Deactivate.reliable = false;
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x000C3EB5 File Offset: 0x000C20B5
	private void OnEnable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Enable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Enable();
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000C3ED8 File Offset: 0x000C20D8
	private void OnDisable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Disable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Disable();
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x000C3EFB File Offset: 0x000C20FB
	private void OnDestroy()
	{
		this.DisposeEvents();
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x000C3F03 File Offset: 0x000C2103
	private void DisposeEvents()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Dispose();
		}
		this.Activate = null;
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate != null)
		{
			deactivate.Dispose();
		}
		this.Deactivate = null;
	}

	// Token: 0x04002FE1 RID: 12257
	[NonSerialized]
	private string PlayerIdString;

	// Token: 0x04002FE2 RID: 12258
	[NonSerialized]
	private int PlayerId;

	// Token: 0x04002FE3 RID: 12259
	public PhotonEvent Activate;

	// Token: 0x04002FE4 RID: 12260
	public PhotonEvent Deactivate;
}
