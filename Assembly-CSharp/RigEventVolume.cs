using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004DA RID: 1242
public class RigEventVolume : MonoBehaviour
{
	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001E3B RID: 7739 RVA: 0x000A1DCA File Offset: 0x0009FFCA
	public VRRig[] Rigs
	{
		get
		{
			return this.rigs.ToArray();
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001E3C RID: 7740 RVA: 0x000A1DD7 File Offset: 0x0009FFD7
	public int RigCount
	{
		get
		{
			return this.gameObjects.Keys.Count;
		}
	}

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06001E3D RID: 7741 RVA: 0x000A1DE9 File Offset: 0x0009FFE9
	public bool LocalRigPresent
	{
		get
		{
			return this.localRigPresent;
		}
	}

	// Token: 0x14000043 RID: 67
	// (add) Token: 0x06001E3E RID: 7742 RVA: 0x000A1DF4 File Offset: 0x0009FFF4
	// (remove) Token: 0x06001E3F RID: 7743 RVA: 0x000A1E2C File Offset: 0x000A002C
	public event Action OnCountChanged;

	// Token: 0x06001E40 RID: 7744 RVA: 0x000A1E64 File Offset: 0x000A0064
	private void OnEnable()
	{
		if (this.mode != RigEventVolume.Mode.RELATIVE)
		{
			return;
		}
		if (this.rigCollection != null)
		{
			VRRigCollection vrrigCollection = this.rigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
			VRRigCollection vrrigCollection2 = this.rigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
			return;
		}
		NetworkSystem.Instance.OnPlayerJoined += this.OnNetJoined;
		NetworkSystem.Instance.OnPlayerLeft += this.OnNetLeft;
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x000A1F19 File Offset: 0x000A0119
	private void OnDisable()
	{
		this.OnDestroy();
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x000A1F24 File Offset: 0x000A0124
	private void OnDestroy()
	{
		if (this.mode != RigEventVolume.Mode.RELATIVE)
		{
			return;
		}
		if (this.rigCollection != null)
		{
			VRRigCollection vrrigCollection = this.rigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
			VRRigCollection vrrigCollection2 = this.rigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
			return;
		}
		NetworkSystem.Instance.OnPlayerJoined -= this.OnNetJoined;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnNetLeft;
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x000A1FDC File Offset: 0x000A01DC
	private void OnNetJoined(NetPlayer np)
	{
		int num = (int)((PhotonNetwork.CurrentRoom == null) ? 1 : PhotonNetwork.CurrentRoom.PlayerCount);
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num - 1, num);
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x000A2020 File Offset: 0x000A0220
	private void OnNetLeft(NetPlayer np)
	{
		int num = (int)((PhotonNetwork.CurrentRoom == null) ? 1 : PhotonNetwork.CurrentRoom.PlayerCount);
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num + 1, num);
	}

	// Token: 0x06001E45 RID: 7749 RVA: 0x000A2064 File Offset: 0x000A0264
	private void OnJoined(RigContainer rc)
	{
		int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num - 1, num);
	}

	// Token: 0x06001E46 RID: 7750 RVA: 0x000A20B4 File Offset: 0x000A02B4
	private void OnLeft(RigContainer rc)
	{
		int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num + 1, num);
	}

	// Token: 0x06001E47 RID: 7751 RVA: 0x000A2104 File Offset: 0x000A0304
	private void OnTriggerEnter(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger))
		{
			if (!this.gameObjects.ContainsKey(rigEventVolumeTrigger))
			{
				this.gameObjects.Add(rigEventVolumeTrigger, 0);
				this.rigs.Add(rigEventVolumeTrigger.Rig);
				UnityEvent<VRRig> rigEnters = this.RigEnters;
				if (rigEnters != null)
				{
					rigEnters.Invoke(rigEventVolumeTrigger.Rig);
				}
				if (rigEventVolumeTrigger.Rig == VRRig.LocalRig)
				{
					UnityEvent<VRRig> localRigEnters = this.LocalRigEnters;
					if (localRigEnters != null)
					{
						localRigEnters.Invoke(rigEventVolumeTrigger.Rig);
					}
					this.localRigPresent = true;
				}
				if (this.mode != RigEventVolume.Mode.NONE)
				{
					int num = (this.rigCollection == null) ? ((int)((PhotonNetwork.CurrentRoom == null) ? 1 : PhotonNetwork.CurrentRoom.PlayerCount)) : this.rigCollection.Rigs.Count;
					this.countChanged(this.gameObjects.Count - 1, this.gameObjects.Count, num, num);
					return;
				}
			}
			else
			{
				Dictionary<RigEventVolumeTrigger, int> dictionary = this.gameObjects;
				RigEventVolumeTrigger key = rigEventVolumeTrigger;
				int num2 = dictionary[key];
				dictionary[key] = num2 + 1;
			}
		}
	}

	// Token: 0x06001E48 RID: 7752 RVA: 0x000A2214 File Offset: 0x000A0414
	private void OnTriggerExit(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger))
		{
			Dictionary<RigEventVolumeTrigger, int> dictionary = this.gameObjects;
			RigEventVolumeTrigger key = rigEventVolumeTrigger;
			int num = dictionary[key];
			dictionary[key] = num - 1;
			if (this.gameObjects[rigEventVolumeTrigger] < 0)
			{
				this.gameObjects.Remove(rigEventVolumeTrigger);
				this.rigs.Remove(rigEventVolumeTrigger.Rig);
				UnityEvent<VRRig> rigExits = this.RigExits;
				if (rigExits != null)
				{
					rigExits.Invoke(rigEventVolumeTrigger.Rig);
				}
				if (rigEventVolumeTrigger.Rig == VRRig.LocalRig)
				{
					UnityEvent<VRRig> localRigExits = this.LocalRigExits;
					if (localRigExits != null)
					{
						localRigExits.Invoke(rigEventVolumeTrigger.Rig);
					}
					this.localRigPresent = false;
				}
				if (this.mode != RigEventVolume.Mode.NONE)
				{
					int num2 = (this.rigCollection == null) ? ((int)((PhotonNetwork.CurrentRoom == null) ? 1 : PhotonNetwork.CurrentRoom.PlayerCount)) : this.rigCollection.Rigs.Count;
					this.countChanged(this.gameObjects.Count + 1, this.gameObjects.Count, num2, num2);
				}
			}
		}
	}

	// Token: 0x06001E49 RID: 7753 RVA: 0x000A2324 File Offset: 0x000A0524
	private void countChanged(int oldValue, int newValue, int oldPlayerCount, int newPlayerCount)
	{
		if (newValue > oldValue)
		{
			if ((this.mode == RigEventVolume.Mode.RELATIVE && (float)newValue / (float)newPlayerCount >= this.relThreshold && (float)oldValue / (float)oldPlayerCount < this.relThreshold) || (this.mode == RigEventVolume.Mode.ABSOLUTE && newValue >= this.absThreshold && oldValue < this.absThreshold))
			{
				UnityEvent goesOverThreshold = this.GoesOverThreshold;
				if (goesOverThreshold != null)
				{
					goesOverThreshold.Invoke();
				}
			}
		}
		else if (newValue < oldValue && ((this.mode == RigEventVolume.Mode.RELATIVE && (float)newValue / (float)newPlayerCount < this.relThreshold && (float)oldValue / (float)oldPlayerCount >= this.relThreshold) || (this.mode == RigEventVolume.Mode.ABSOLUTE && newValue < this.absThreshold && oldValue >= this.absThreshold)))
		{
			UnityEvent goesUnderThreshold = this.GoesUnderThreshold;
			if (goesUnderThreshold != null)
			{
				goesUnderThreshold.Invoke();
			}
		}
		Action onCountChanged = this.OnCountChanged;
		if (onCountChanged == null)
		{
			return;
		}
		onCountChanged();
	}

	// Token: 0x0400285F RID: 10335
	private Dictionary<RigEventVolumeTrigger, int> gameObjects = new Dictionary<RigEventVolumeTrigger, int>();

	// Token: 0x04002860 RID: 10336
	[SerializeField]
	private RigEventVolume.Mode mode = RigEventVolume.Mode.ABSOLUTE;

	// Token: 0x04002861 RID: 10337
	[Range(0.05f, 1f)]
	[SerializeField]
	private float relThreshold = 0.05f;

	// Token: 0x04002862 RID: 10338
	[SerializeField]
	private VRRigCollection rigCollection;

	// Token: 0x04002863 RID: 10339
	[Range(1f, 20f)]
	[SerializeField]
	private int absThreshold = 1;

	// Token: 0x04002864 RID: 10340
	[SerializeField]
	private UnityEvent<VRRig> RigEnters;

	// Token: 0x04002865 RID: 10341
	[SerializeField]
	private UnityEvent<VRRig> RigExits;

	// Token: 0x04002866 RID: 10342
	[SerializeField]
	private UnityEvent GoesOverThreshold;

	// Token: 0x04002867 RID: 10343
	[SerializeField]
	private UnityEvent GoesUnderThreshold;

	// Token: 0x04002868 RID: 10344
	[SerializeField]
	private UnityEvent<VRRig> LocalRigEnters;

	// Token: 0x04002869 RID: 10345
	[SerializeField]
	private UnityEvent<VRRig> LocalRigExits;

	// Token: 0x0400286A RID: 10346
	private List<VRRig> rigs = new List<VRRig>();

	// Token: 0x0400286B RID: 10347
	private bool localRigPresent;

	// Token: 0x020004DB RID: 1243
	private enum Mode
	{
		// Token: 0x0400286E RID: 10350
		RELATIVE,
		// Token: 0x0400286F RID: 10351
		ABSOLUTE,
		// Token: 0x04002870 RID: 10352
		NONE
	}
}
