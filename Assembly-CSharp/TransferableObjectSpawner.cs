using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000916 RID: 2326
public class TransferableObjectSpawner : MonoBehaviour
{
	// Token: 0x06003CDA RID: 15578 RVA: 0x0014B234 File Offset: 0x00149434
	public void Awake()
	{
		GameObject[] transferrableObjectsToSpawn = this.TransferrableObjectsToSpawn;
		for (int i = 0; i < transferrableObjectsToSpawn.Length; i++)
		{
			TransferrableObject componentInChildren = transferrableObjectsToSpawn[i].GetComponentInChildren<TransferrableObject>();
			if (componentInChildren.IsNotNull())
			{
				this.objectsToSpawn.Add(componentInChildren);
			}
			else
			{
				Debug.LogError("Failed to add object " + componentInChildren.gameObject.name + " - missing a Transferrable object");
			}
		}
	}

	// Token: 0x06003CDB RID: 15579 RVA: 0x0014B294 File Offset: 0x00149494
	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			return;
		}
		foreach (GameObject gameObject in this.TransferrableObjectsToSpawn)
		{
			TransferrableObject componentInChildren = gameObject.GetComponentInChildren<TransferrableObject>();
			if (componentInChildren.IsNull())
			{
				Debug.LogError(string.Concat(new string[]
				{
					base.name,
					" at path ",
					this.GetComponentPath(int.MaxValue),
					" has ",
					gameObject.name,
					" assigned to TransferrableObjectsToSpawn collection, but it does not have a TransferrableObject component.  It will not spawn."
				}));
			}
			else if (componentInChildren.worldShareableInstance == null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					base.name,
					" at path ",
					this.GetComponentPath(int.MaxValue),
					" has ",
					gameObject.name,
					" assigned to TransferrableObjectsToSpawn collection, but it's worldShareableInstance is null."
				}));
			}
		}
	}

	// Token: 0x06003CDC RID: 15580 RVA: 0x0014B377 File Offset: 0x00149577
	public void Update()
	{
		if (this.spawnTrigger == TransferableObjectSpawner.SpawnTrigger.Timer && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && PhotonNetwork.Time > this.lastSpawnTime + this.SpawnDelay)
		{
			this.SpawnTransferrableObject();
			this.lastSpawnTime = PhotonNetwork.Time;
		}
	}

	// Token: 0x06003CDD RID: 15581 RVA: 0x0014B3B4 File Offset: 0x001495B4
	private bool SpawnOnGround()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position + Random.insideUnitCircle.x0y() * this.spawnRadius, Vector3.down), out raycastHit, 3f, this.groundRaycastMask))
		{
			this.spawnPosition = raycastHit.point;
			this.spawnRotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
			return true;
		}
		return false;
	}

	// Token: 0x06003CDE RID: 15582 RVA: 0x0014B430 File Offset: 0x00149630
	private void SpawnAtCurrentLocation()
	{
		this.spawnPosition = base.transform.position;
		this.spawnRotation = base.transform.rotation;
	}

	// Token: 0x06003CDF RID: 15583 RVA: 0x0014B454 File Offset: 0x00149654
	public void SpawnTransferrableObject()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		TransferableObjectSpawner.SpawnMode spawnMode = this.spawnMode;
		if (spawnMode != TransferableObjectSpawner.SpawnMode.OnGround)
		{
			if (spawnMode != TransferableObjectSpawner.SpawnMode.AtCurrentTransform)
			{
				return;
			}
			this.SpawnAtCurrentLocation();
		}
		else if (!this.SpawnOnGround())
		{
			return;
		}
		TransferrableObject transferrableObject = null;
		int num = 0;
		foreach (TransferrableObject transferrableObject2 in this.objectsToSpawn)
		{
			if (!transferrableObject2.InHand())
			{
				num++;
				if (Random.Range(0, num) == 0)
				{
					transferrableObject = transferrableObject2;
				}
			}
		}
		if (transferrableObject != null)
		{
			if (!transferrableObject.IsLocalOwnedWorldShareable)
			{
				transferrableObject.WorldShareableRequestOwnership();
			}
			if (transferrableObject.worldShareableInstance != null)
			{
				transferrableObject.transform.SetPositionAndRotation(this.spawnPosition, this.spawnRotation);
				transferrableObject.worldShareableInstance.SetWillTeleport();
				return;
			}
			Debug.LogError("WorldShareableInstance for " + transferrableObject.name + " is null");
		}
	}

	// Token: 0x04004D69 RID: 19817
	private Vector3 spawnPosition = Vector3.zero;

	// Token: 0x04004D6A RID: 19818
	private Quaternion spawnRotation = Quaternion.identity;

	// Token: 0x04004D6B RID: 19819
	[SerializeField]
	private GameObject[] TransferrableObjectsToSpawn;

	// Token: 0x04004D6C RID: 19820
	private List<TransferrableObject> objectsToSpawn = new List<TransferrableObject>();

	// Token: 0x04004D6D RID: 19821
	[SerializeField]
	private TransferableObjectSpawner.SpawnMode spawnMode;

	// Token: 0x04004D6E RID: 19822
	[SerializeField]
	private TransferableObjectSpawner.SpawnTrigger spawnTrigger;

	// Token: 0x04004D6F RID: 19823
	[SerializeField]
	private double SpawnDelay = 5.0;

	// Token: 0x04004D70 RID: 19824
	private double lastSpawnTime;

	// Token: 0x04004D71 RID: 19825
	[SerializeField]
	private LayerMask groundRaycastMask = LayerMask.NameToLayer("Gorilla Object");

	// Token: 0x04004D72 RID: 19826
	[SerializeField]
	private float spawnRadius = 0.5f;

	// Token: 0x02000917 RID: 2327
	private enum SpawnMode
	{
		// Token: 0x04004D74 RID: 19828
		OnGround,
		// Token: 0x04004D75 RID: 19829
		AtCurrentTransform
	}

	// Token: 0x02000918 RID: 2328
	private enum SpawnTrigger
	{
		// Token: 0x04004D77 RID: 19831
		Timer
	}
}
