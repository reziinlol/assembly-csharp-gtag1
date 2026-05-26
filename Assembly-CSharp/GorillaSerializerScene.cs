using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000834 RID: 2100
internal class GorillaSerializerScene : GorillaSerializer, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x170004B4 RID: 1204
	// (get) Token: 0x060035F8 RID: 13816 RVA: 0x0012ACB9 File Offset: 0x00128EB9
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x060035F9 RID: 13817 RVA: 0x0012ACC8 File Offset: 0x00128EC8
	protected virtual void Start()
	{
		if (!this.targetComponent.IsNull())
		{
			IGorillaSerializeableScene gorillaSerializeableScene = this.targetComponent as IGorillaSerializeableScene;
			if (gorillaSerializeableScene != null)
			{
				gorillaSerializeableScene.OnSceneLinking(this);
				this.serializeTarget = gorillaSerializeableScene;
				this.sceneSerializeTarget = gorillaSerializeableScene;
				this.successfullInstantiate = true;
				this.photonView.AddCallbackTarget(this);
				return;
			}
		}
		Debug.LogError("GorillaSerializerscene: missing target component or invalid target", base.gameObject);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060035FA RID: 13818 RVA: 0x0012AD36 File Offset: 0x00128F36
	private void OnEnable()
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		if (!this.validDisable)
		{
			this.validDisable = true;
			return;
		}
		this.OnValidEnable();
	}

	// Token: 0x060035FB RID: 13819 RVA: 0x0012AD57 File Offset: 0x00128F57
	protected virtual void OnValidEnable()
	{
		this.sceneSerializeTarget.OnNetworkObjectEnable();
	}

	// Token: 0x060035FC RID: 13820 RVA: 0x0012AD64 File Offset: 0x00128F64
	private void OnDisable()
	{
		if (!this.successfullInstantiate || !this.validDisable)
		{
			return;
		}
		this.OnValidDisable();
	}

	// Token: 0x060035FD RID: 13821 RVA: 0x0012AD7D File Offset: 0x00128F7D
	protected virtual void OnValidDisable()
	{
		this.sceneSerializeTarget.OnNetworkObjectDisable();
	}

	// Token: 0x060035FE RID: 13822 RVA: 0x0012AD8C File Offset: 0x00128F8C
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		MonkeAgent.instance.SendReport("bad net obj creation", info.Sender.UserId, info.Sender.NickName);
		if (info.photonView.IsMine)
		{
			PhotonNetwork.Destroy(info.photonView);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060035FF RID: 13823 RVA: 0x0012ADE4 File Offset: 0x00128FE4
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.validDisable = false;
	}

	// Token: 0x06003600 RID: 13824 RVA: 0x0012ADED File Offset: 0x00128FED
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		if (!this.transferrable)
		{
			return info.Sender == PhotonNetwork.MasterClient;
		}
		return base.ValidOnSerialize(stream, info);
	}

	// Token: 0x04004697 RID: 18071
	[SerializeField]
	private bool transferrable;

	// Token: 0x04004698 RID: 18072
	[SerializeField]
	private MonoBehaviour targetComponent;

	// Token: 0x04004699 RID: 18073
	private IGorillaSerializeableScene sceneSerializeTarget;

	// Token: 0x0400469A RID: 18074
	protected bool validDisable = true;
}
