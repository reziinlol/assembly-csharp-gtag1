using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Fusion;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

// Token: 0x020001E8 RID: 488
[NetworkBehaviourWeaved(0)]
public class RandomCarveableObject : NetworkComponent
{
	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000CC3 RID: 3267 RVA: 0x000462A3 File Offset: 0x000444A3
	private bool HasAuthority
	{
		get
		{
			return VoxelManager.HasAuthority;
		}
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x000462AC File Offset: 0x000444AC
	private new void Start()
	{
		if (this.world == null)
		{
			this.world = base.GetComponentInParent<VoxelWorld>();
		}
		this.spawnFX.SetActive(false);
		this.world.SetWorldBounds(new UnityEngine.BoundsInt(Vector3Int.zero, (Chunk.DefaultSize - 1).ToVectorInt()));
		for (int i = this.spawnPoint.childCount - 1; i >= 0; i--)
		{
			JamUtil.Destroy(this.spawnPoint.GetChild(i).gameObject);
		}
		this.proximityTrigger.OnCountChanged += this.OnPlayerCountChanged;
		this.OnPlayerCountChanged();
		this.Init();
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x0004634F File Offset: 0x0004454F
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.proximityTrigger.OnCountChanged -= this.OnPlayerCountChanged;
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x0004636E File Offset: 0x0004456E
	private void Init()
	{
		if (this.HasAuthority)
		{
			this.SpawnRandomCarveable();
			return;
		}
		if (this._version != 0 && this.IsValidPrefab(this._carveableIndex))
		{
			this.SpawnCarveable(this._carveableIndex);
		}
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x000463A1 File Offset: 0x000445A1
	private void ClearWorld()
	{
		this.world.SetVoxels(this.world.WorldBounds, 0, this.materialId, false);
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x000463C1 File Offset: 0x000445C1
	private void FillBounds()
	{
		this.SetBoundsDensity(byte.MaxValue);
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x000463CE File Offset: 0x000445CE
	private void SetBoundsDensity(byte density)
	{
		this.world.SetVoxels(this._voxels, density, this.materialId, true);
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x000463EC File Offset: 0x000445EC
	private void CollectVoxelSet()
	{
		Bounds localBounds = this._carveable.GetComponentInChildren<Renderer>().localBounds;
		Vector3 min = localBounds.min;
		Vector3 max = localBounds.max;
		HashSet<int3> hashSet = new HashSet<int3>();
		for (float num = min.x; num <= max.x; num += 0.1f)
		{
			for (float num2 = min.y; num2 <= max.y; num2 += 0.1f)
			{
				for (float num3 = min.z; num3 <= max.z; num3 += 0.1f)
				{
					hashSet.Add(this.<CollectVoxelSet>g__GetVoxel|23_0(num, num2, num3));
				}
			}
		}
		this._voxels = hashSet.ToArray<int3>();
		this._voxelBounds = VoxelWorld.GetBoundsFor(this._voxels);
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x000464AC File Offset: 0x000446AC
	private void SetCarveable(int index)
	{
		this._carveableIndex = index;
		for (int i = this.spawnPoint.childCount - 1; i >= 0; i--)
		{
			JamUtil.Destroy(this.spawnPoint.GetChild(i).gameObject);
		}
		this._carveable = UnityEngine.Object.Instantiate<GameObject>(this.prefabs[this._carveableIndex], this.spawnPoint.position, this.spawnPoint.rotation, this.spawnPoint);
		this._carveable.transform.localScale = Vector3.one;
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x00046537 File Offset: 0x00044737
	private void OnPlayerCountChanged()
	{
		this.SetCanRequestNewCarveable(this.proximityTrigger.RigCount == 0);
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0004654D File Offset: 0x0004474D
	private void SetCanRequestNewCarveable(bool active)
	{
		if (this._buttonConfigured && this._canRequestNewCarveable == active)
		{
			return;
		}
		this.button.isOn = active;
		this.button.UpdateColor();
		this._buttonConfigured = true;
		this._canRequestNewCarveable = active;
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x00046586 File Offset: 0x00044786
	public void RequestSpawnRandomCarveable()
	{
		if (!this._canRequestNewCarveable)
		{
			return;
		}
		Debug.Log("RequestSpawnRandomCarveable()");
		if (this.HasAuthority)
		{
			this.SpawnRandomCarveable();
			return;
		}
		base.GetView.RPC("RPC_SpawnRandomCarveable", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x000465C0 File Offset: 0x000447C0
	private bool IsValidAuthorityRPC(PhotonMessageInfo info)
	{
		return VoxelManager.HasAuthority && this.spamCheck.CheckCallTime(Time.unscaledTime);
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x000465DB File Offset: 0x000447DB
	[PunRPC]
	public void RPC_SpawnRandomCarveable(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RPC_SpawnRandomCarveable");
		if (!this.IsValidAuthorityRPC(info))
		{
			return;
		}
		this.SpawnRandomCarveable();
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x000465F8 File Offset: 0x000447F8
	private void SpawnRandomCarveable()
	{
		this.SpawnCarveable(UnityEngine.Random.Range(0, this.prefabs.Length));
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00046610 File Offset: 0x00044810
	private void SpawnCarveable(int index)
	{
		RandomCarveableObject.<SpawnCarveable>d__31 <SpawnCarveable>d__;
		<SpawnCarveable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SpawnCarveable>d__.<>4__this = this;
		<SpawnCarveable>d__.index = index;
		<SpawnCarveable>d__.<>1__state = -1;
		<SpawnCarveable>d__.<>t__builder.Start<RandomCarveableObject.<SpawnCarveable>d__31>(ref <SpawnCarveable>d__);
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0004664F File Offset: 0x0004484F
	private void IncrementVersion()
	{
		this._version++;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x0004665F File Offset: 0x0004485F
	private bool IsValidPrefab(int index)
	{
		return index >= 0 && index < this.prefabs.Length;
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00046672 File Offset: 0x00044872
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this._carveableIndex);
		stream.SendNext(this._version);
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00046698 File Offset: 0x00044898
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		int num2 = (int)stream.ReceiveNext();
		if (this._carveableIndex == num && this._version == num2)
		{
			return;
		}
		this.OnStateChange(num, num2);
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x000466EC File Offset: 0x000448EC
	public void OnStateChange(int newIndex, int newVersion)
	{
		this._carveableIndex = newIndex;
		this._version = newVersion;
		this.IsValidPrefab(this._carveableIndex);
		if (this.IsValidPrefab(this._carveableIndex))
		{
			this.SpawnCarveable(this._carveableIndex);
		}
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00046757 File Offset: 0x00044957
	[CompilerGenerated]
	private int3 <CollectVoxelSet>g__GetVoxel|23_0(float x, float y, float z)
	{
		return this.world.GetVoxelForWorldPosition(this._carveable.transform.TransformPoint(new Vector3(x, y, z)));
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x00002B07 File Offset: 0x00000D07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x00002B13 File Offset: 0x00000D13
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04000F6E RID: 3950
	[SerializeField]
	private Transform spawnPoint;

	// Token: 0x04000F6F RID: 3951
	[SerializeField]
	private GameObject[] prefabs;

	// Token: 0x04000F70 RID: 3952
	[SerializeField]
	private byte materialId;

	// Token: 0x04000F71 RID: 3953
	[SerializeField]
	private VoxelWorld world;

	// Token: 0x04000F72 RID: 3954
	[SerializeField]
	private GameObject spawnFX;

	// Token: 0x04000F73 RID: 3955
	[SerializeField]
	private CallLimiter spamCheck = new CallLimiter(1, 1f, 0.5f);

	// Token: 0x04000F74 RID: 3956
	[SerializeField]
	private RigEventVolume proximityTrigger;

	// Token: 0x04000F75 RID: 3957
	[SerializeField]
	private GorillaPressableButton button;

	// Token: 0x04000F76 RID: 3958
	private int _carveableIndex = -1;

	// Token: 0x04000F77 RID: 3959
	private int _version;

	// Token: 0x04000F78 RID: 3960
	private GameObject _carveable;

	// Token: 0x04000F79 RID: 3961
	private int3[] _voxels;

	// Token: 0x04000F7A RID: 3962
	private UnityEngine.BoundsInt _voxelBounds;

	// Token: 0x04000F7B RID: 3963
	private bool _buttonConfigured;

	// Token: 0x04000F7C RID: 3964
	private bool _canRequestNewCarveable;
}
