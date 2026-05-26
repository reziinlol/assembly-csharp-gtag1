using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200059A RID: 1434
public class RubberDuckEvents : MonoBehaviour
{
	// Token: 0x06002455 RID: 9301 RVA: 0x000C3708 File Offset: 0x000C1908
	public void Init(NetPlayer player)
	{
		string text = player.UserId;
		if (string.IsNullOrEmpty(text))
		{
			bool isLocal = player.IsLocal;
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (isLocal && instance != null)
			{
				text = instance.GetPlayFabPlayerId();
			}
			else
			{
				text = player.NickName;
			}
		}
		this.PlayerIdString = text + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.Dispose();
		this.Activate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Activate"));
		this.Deactivate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Deactivate"));
		this.Activate.reliable = true;
		this.Deactivate.reliable = true;
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x000C37E2 File Offset: 0x000C19E2
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

	// Token: 0x06002457 RID: 9303 RVA: 0x000C3805 File Offset: 0x000C1A05
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

	// Token: 0x06002458 RID: 9304 RVA: 0x000C3828 File Offset: 0x000C1A28
	private void OnDestroy()
	{
		this.Dispose();
	}

	// Token: 0x06002459 RID: 9305 RVA: 0x000C3830 File Offset: 0x000C1A30
	public void Dispose()
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

	// Token: 0x04002FC1 RID: 12225
	public int PlayerId;

	// Token: 0x04002FC2 RID: 12226
	public string PlayerIdString;

	// Token: 0x04002FC3 RID: 12227
	public PhotonEvent Activate;

	// Token: 0x04002FC4 RID: 12228
	public PhotonEvent Deactivate;
}
