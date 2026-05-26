using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxels;

// Token: 0x020001F0 RID: 496
public class VoxelInteractor : MonoBehaviour
{
	// Token: 0x06000D0C RID: 3340 RVA: 0x0004792B File Offset: 0x00045B2B
	private void OnDisable()
	{
		this.StopOngoingAction();
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00047933 File Offset: 0x00045B33
	public void StartOngoingAction()
	{
		if (this._active)
		{
			return;
		}
		if (this._actionRoutine != null)
		{
			base.StopCoroutine(this._actionRoutine);
		}
		this._active = true;
		this._actionRoutine = base.StartCoroutine(this.DoContinuousAction());
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x0004796B File Offset: 0x00045B6B
	public void StopOngoingAction()
	{
		if (this._actionRoutine != null)
		{
			base.StopCoroutine(this._actionRoutine);
			this._actionRoutine = null;
		}
		this._active = false;
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00047990 File Offset: 0x00045B90
	public void PerformAction()
	{
		if (Time.time < this._nextActionTime)
		{
			return;
		}
		RaycastHit hit;
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * this.rayLength, out hit, this.layerMask, QueryTriggerInteraction.Ignore))
		{
			ChunkComponent component = hit.collider.GetComponent<ChunkComponent>();
			if (component)
			{
				component.World.Mine(hit, this.action);
			}
		}
		this._nextActionTime = Time.time + this.cooldown;
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00047A2C File Offset: 0x00045C2C
	public void PerformActionOmnidirectional()
	{
		if (Time.time < this._nextActionTime)
		{
			return;
		}
		if (VoxelInteractor._hitWorlds == null)
		{
			VoxelInteractor._hitWorlds = new List<VoxelWorld>();
		}
		if (VoxelInteractor._hitColliders == null)
		{
			VoxelInteractor._hitColliders = new Collider[5];
		}
		VoxelInteractor._hitWorlds.Clear();
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.action.radius, VoxelInteractor._hitColliders, this.layerMask);
		if (num == VoxelInteractor._hitColliders.Length)
		{
			Array.Resize<Collider>(ref VoxelInteractor._hitColliders, VoxelInteractor._hitColliders.Length * 2);
		}
		for (int i = 0; i < num; i++)
		{
			ChunkComponent chunkComponent;
			if (VoxelInteractor._hitColliders[i].TryGetComponent<ChunkComponent>(out chunkComponent) && !VoxelInteractor._hitWorlds.Contains(chunkComponent.World))
			{
				VoxelInteractor._hitWorlds.Add(chunkComponent.World);
				chunkComponent.World.PerformAction(base.transform.position, this.action);
			}
		}
		this._nextActionTime = Time.time + this.cooldown;
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00047B29 File Offset: 0x00045D29
	private IEnumerator DoContinuousAction()
	{
		while (this._active)
		{
			while (Time.time < this._nextActionTime)
			{
				yield return null;
			}
			if (this._active)
			{
				this.PerformAction();
			}
		}
		this._actionRoutine = null;
		yield break;
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00047B38 File Offset: 0x00045D38
	public bool ApplyVoxelAction(Collision collision)
	{
		ChunkComponent component = collision.gameObject.GetComponent<ChunkComponent>();
		if (component)
		{
			component.World.Mine(collision, this.action);
		}
		return component;
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00047B74 File Offset: 0x00045D74
	public bool ApplyVoxelAction(RaycastHit hit)
	{
		ChunkComponent component = hit.collider.GetComponent<ChunkComponent>();
		if (component)
		{
			component.World.Mine(hit, this.action);
		}
		return component;
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00047BB0 File Offset: 0x00045DB0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Vector3 vector = base.transform.position + base.transform.forward * this.rayLength;
		Gizmos.DrawLine(base.transform.position, vector);
		Gizmos.DrawWireSphere(base.transform.position, 0.01f);
		Gizmos.DrawWireSphere(vector, 0.01f);
	}

	// Token: 0x04000FA3 RID: 4003
	[SerializeField]
	private LayerMask layerMask = 1;

	// Token: 0x04000FA4 RID: 4004
	[SerializeField]
	private float rayLength = 0.1f;

	// Token: 0x04000FA5 RID: 4005
	[SerializeField]
	private float cooldown = 0.25f;

	// Token: 0x04000FA6 RID: 4006
	[SerializeField]
	private VoxelAction action = new VoxelAction
	{
		strength = 0.5f,
		radius = 0.5f,
		operation = OperationType.Subtract
	};

	// Token: 0x04000FA7 RID: 4007
	private bool _active;

	// Token: 0x04000FA8 RID: 4008
	private Coroutine _actionRoutine;

	// Token: 0x04000FA9 RID: 4009
	private float _nextActionTime;

	// Token: 0x04000FAA RID: 4010
	[OnEnterPlay_SetNull]
	private static List<VoxelWorld> _hitWorlds;

	// Token: 0x04000FAB RID: 4011
	[OnEnterPlay_SetNull]
	private static Collider[] _hitColliders;
}
