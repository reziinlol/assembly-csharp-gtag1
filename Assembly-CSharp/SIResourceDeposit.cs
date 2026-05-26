using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200015D RID: 349
public class SIResourceDeposit : MonoBehaviour, ISIResourceDeposit
{
	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000935 RID: 2357 RVA: 0x00031BAE File Offset: 0x0002FDAE
	public bool IsAuthority
	{
		get
		{
			return this.SIManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000936 RID: 2358 RVA: 0x00031BC0 File Offset: 0x0002FDC0
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.superInfection.siManager;
		}
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00031BD0 File Offset: 0x0002FDD0
	private void OnEnable()
	{
		if (this._displayResources == null || this._displayResources.Count == 0)
		{
			List<SIResource> resourcePrefabs = this.superInfection.ResourcePrefabs;
			if (resourcePrefabs != null && resourcePrefabs.Count > 0)
			{
				this._displayResources = new List<GameObject>();
				for (int i = 0; i < Mathf.Min(resourcePrefabs.Count, this.resourceDisplays.Length); i++)
				{
					GameObject gameObject = resourcePrefabs[i].gameObject;
					bool activeSelf = gameObject.activeSelf;
					try
					{
						if (activeSelf)
						{
							gameObject.SetActive(false);
						}
						GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, this.resourceDisplays[i].transform);
						gameObject2.transform.localScale = new Vector3(0.27f, 0.27f, 0.27f);
						this._displayResources.Add(gameObject2);
						foreach (MonoBehaviour monoBehaviour in gameObject2.GetComponentsInChildren<MonoBehaviour>(true))
						{
							monoBehaviour.enabled = false;
							Object.Destroy(monoBehaviour);
						}
						Rigidbody component = gameObject2.GetComponent<Rigidbody>();
						if (component != null)
						{
							Object.Destroy(component);
						}
						gameObject2.SetLayerRecursively(UnityLayer.Default);
						gameObject2.SetActive(true);
					}
					finally
					{
						if (activeSelf)
						{
							gameObject.SetActive(true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00031D14 File Offset: 0x0002FF14
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.netPlayer != null)
		{
			stream.SendNext(this.netPlayer.ActorNr);
		}
		else
		{
			stream.SendNext(-1);
		}
		stream.SendNext((int)this.netResourceType);
		stream.SendNext((int)this.netLimitedDepositType);
		stream.SendNext(this.netShowPopup);
		this.netShowPopup = false;
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00031D90 File Offset: 0x0002FF90
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.netPlayer = SIPlayer.Get((int)stream.ReceiveNext());
		this.netResourceType = (SIResource.ResourceType)((int)stream.ReceiveNext());
		this.netLimitedDepositType = (SIResource.LimitedDepositType)((int)stream.ReceiveNext());
		if ((bool)stream.ReceiveNext())
		{
			this.LocalShowPopup(this.netPlayer, this.netResourceType, this.netLimitedDepositType);
		}
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00031DFC File Offset: 0x0002FFFC
	private void LocalShowPopup(SIPlayer player, SIResource.ResourceType resourceType, SIResource.LimitedDepositType limitedDepositType)
	{
		if (limitedDepositType == SIResource.LimitedDepositType.None)
		{
			this.depositBin.SetActive(true);
		}
		this.popupScreen.EnableAndResetTimer();
		this.depositText.text = string.Format("{0} COLLECTED {1}\n(TOTAL {2})", player.gamePlayer.rig.Creator.SanitizedNickName, resourceType.GetName<SIResource.ResourceType>(), player.GetResourceAmount(resourceType));
		this.depositImage.sprite = ((resourceType == SIResource.ResourceType.TechPoint) ? this.resourceImageSprites[0] : this.resourceImageSprites[1]);
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00031E80 File Offset: 0x00030080
	public void ResourceDeposited(SIResource resource)
	{
		bool flag = false;
		if (resource.lastPlayerHeld.gamePlayer.IsLocal() && !resource.localDeposited)
		{
			this.AuthShowPopup(resource);
			resource.HandleDepositLocal(resource.lastPlayerHeld);
			resource.lastPlayerHeld.GatherResource(resource.type, resource.limitedDepositType, 1);
			this.superInfection.siManager.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.ResourceDepositDeposited, new object[]
			{
				resource.myGameEntity.GetNetId(),
				this.index
			});
			flag = true;
		}
		if (this.superInfection.siManager.gameEntityManager.IsAuthority())
		{
			resource.HandleDepositAuth(resource.lastPlayerHeld);
			this.superInfection.siManager.gameEntityManager.RequestDestroyItem(resource.myGameEntity.id);
			this.AuthShowPopup(resource);
			flag = true;
		}
		if (flag)
		{
			this.LocalShowPopup(resource.lastPlayerHeld, resource.type, resource.limitedDepositType);
		}
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x00031F75 File Offset: 0x00030175
	private void AuthShowPopup(SIResource resource)
	{
		this.netPlayer = resource.lastPlayerHeld;
		this.netResourceType = resource.type;
		this.netLimitedDepositType = resource.limitedDepositType;
		this.netShowPopup = true;
	}

	// Token: 0x04000B48 RID: 2888
	public int index;

	// Token: 0x04000B49 RID: 2889
	public Text depositText;

	// Token: 0x04000B4A RID: 2890
	public Image depositImage;

	// Token: 0x04000B4B RID: 2891
	public DisableGameObjectDelayed popupScreen;

	// Token: 0x04000B4C RID: 2892
	public SuperInfection superInfection;

	// Token: 0x04000B4D RID: 2893
	public Sprite[] resourceImageSprites;

	// Token: 0x04000B4E RID: 2894
	public GameObject depositBin;

	// Token: 0x04000B4F RID: 2895
	[SerializeField]
	private Transform[] resourceDisplays;

	// Token: 0x04000B50 RID: 2896
	public SIPlayer netPlayer;

	// Token: 0x04000B51 RID: 2897
	public SIResource.ResourceType netResourceType;

	// Token: 0x04000B52 RID: 2898
	public SIResource.LimitedDepositType netLimitedDepositType;

	// Token: 0x04000B53 RID: 2899
	private bool netShowPopup;

	// Token: 0x04000B54 RID: 2900
	public List<SIUIPlayerQuestDisplay> questDisplays;

	// Token: 0x04000B55 RID: 2901
	private List<GameObject> _displayResources;
}
