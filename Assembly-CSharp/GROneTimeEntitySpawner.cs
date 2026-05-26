using System;
using UnityEngine;

// Token: 0x020007B4 RID: 1972
public class GROneTimeEntitySpawner : MonoBehaviour
{
	// Token: 0x06003236 RID: 12854 RVA: 0x00113B33 File Offset: 0x00111D33
	private void Start()
	{
		if (this.EntityPrefab == null)
		{
			Debug.Log("Can't  spawn null entity", this);
		}
		base.Invoke("TrySpawn", this.SpawnDelay);
	}

	// Token: 0x06003237 RID: 12855 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Update()
	{
	}

	// Token: 0x06003238 RID: 12856 RVA: 0x00113B60 File Offset: 0x00111D60
	private void TrySpawn()
	{
		if (!this.bHasSpawned && this.EntityPrefab != null)
		{
			Debug.Log("trying to spawn entity" + this.EntityPrefab.name, this);
			GameEntityManager gameEntityManager = this.reactor.grManager.gameEntityManager;
			if (gameEntityManager.IsAuthority())
			{
				if (!gameEntityManager.IsZoneActive())
				{
					Debug.Log("delaying spawn attempt because zone not active", this);
					base.Invoke("TrySpawn", 0.2f);
					return;
				}
				Debug.Log("trying to spawn entity", this);
				gameEntityManager.RequestCreateItem(this.EntityPrefab.name.GetStaticHash(), base.transform.position + new Vector3(0f, 0f, 0f), base.transform.rotation, 0L);
				this.bHasSpawned = true;
			}
		}
	}

	// Token: 0x04004130 RID: 16688
	public GhostReactor reactor;

	// Token: 0x04004131 RID: 16689
	public GameEntity EntityPrefab;

	// Token: 0x04004132 RID: 16690
	private bool bHasSpawned;

	// Token: 0x04004133 RID: 16691
	private float SpawnDelay = 3f;
}
