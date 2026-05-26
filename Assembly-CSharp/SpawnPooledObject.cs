using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020009D2 RID: 2514
public class SpawnPooledObject : MonoBehaviour
{
	// Token: 0x06004065 RID: 16485 RVA: 0x00158246 File Offset: 0x00156446
	private void Awake()
	{
		if (this._pooledObject == null)
		{
			return;
		}
		this._pooledObjectHash = PoolUtils.GameObjHashCode(this._pooledObject);
	}

	// Token: 0x06004066 RID: 16486 RVA: 0x00158268 File Offset: 0x00156468
	public void SpawnObject()
	{
		if (!this.ShouldSpawn())
		{
			return;
		}
		if (this._pooledObject == null || this._spawnLocation == null)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this._pooledObjectHash, true);
		gameObject.transform.position = this.SpawnLocation();
		gameObject.transform.rotation = this.SpawnRotation();
		gameObject.transform.localScale = base.transform.lossyScale;
	}

	// Token: 0x06004067 RID: 16487 RVA: 0x001582E3 File Offset: 0x001564E3
	private Vector3 SpawnLocation()
	{
		return this._spawnLocation.transform.position + this.offset;
	}

	// Token: 0x06004068 RID: 16488 RVA: 0x00158300 File Offset: 0x00156500
	private Quaternion SpawnRotation()
	{
		Quaternion result = this._spawnLocation.transform.rotation;
		if (this.facePlayer)
		{
			result = Quaternion.LookRotation(GTPlayer.Instance.headCollider.transform.position - this._spawnLocation.transform.position);
		}
		if (this.upright)
		{
			result.eulerAngles = new Vector3(0f, result.eulerAngles.y, 0f);
		}
		return result;
	}

	// Token: 0x06004069 RID: 16489 RVA: 0x00158380 File Offset: 0x00156580
	private bool ShouldSpawn()
	{
		return Random.Range(0, 100) < this.chanceToSpawn;
	}

	// Token: 0x040050EF RID: 20719
	[SerializeField]
	private Transform _spawnLocation;

	// Token: 0x040050F0 RID: 20720
	[SerializeField]
	private GameObject _pooledObject;

	// Token: 0x040050F1 RID: 20721
	[FormerlySerializedAs("_offset")]
	public Vector3 offset;

	// Token: 0x040050F2 RID: 20722
	[FormerlySerializedAs("_upright")]
	public bool upright;

	// Token: 0x040050F3 RID: 20723
	[FormerlySerializedAs("_facePlayer")]
	public bool facePlayer;

	// Token: 0x040050F4 RID: 20724
	[FormerlySerializedAs("_chanceToSpawn")]
	[Range(0f, 100f)]
	public int chanceToSpawn = 100;

	// Token: 0x040050F5 RID: 20725
	private int _pooledObjectHash;
}
