using System;
using UnityEngine;
using UnityEngine.Audio;
using Voxels;

// Token: 0x020001F4 RID: 500
public class Voxel_Pickaxe : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000D21 RID: 3361 RVA: 0x00047E52 File Offset: 0x00046052
	// (set) Token: 0x06000D22 RID: 3362 RVA: 0x00047E5A File Offset: 0x0004605A
	public bool Held { get; set; }

	// Token: 0x06000D23 RID: 3363 RVA: 0x00047E63 File Offset: 0x00046063
	private void Reset()
	{
		this._layerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00047E80 File Offset: 0x00046080
	private void Awake()
	{
		this._gameEntity = base.GetComponent<GameEntity>();
		this._layerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		if (this.sound.transform == base.transform)
		{
			Debug.LogError("Audio source for " + base.name + " must be on a separate gameobject!", this);
		}
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x00047EE8 File Offset: 0x000460E8
	private void OnEnable()
	{
		if (this._gameEntity != null)
		{
			GameEntity gameEntity = this._gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartGrabbing));
			GameEntity gameEntity2 = this._gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.StopGrabbing));
		}
		this._isLocal = (base.GetComponentInParent<VRRig>() == VRRig.LocalRig);
		this.ResetVelocity();
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x00047F70 File Offset: 0x00046170
	private void OnDisable()
	{
		if (this._gameEntity != null)
		{
			GameEntity gameEntity = this._gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.StartGrabbing));
			GameEntity gameEntity2 = this._gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.StopGrabbing));
		}
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x00047FDC File Offset: 0x000461DC
	private void FixedUpdate()
	{
		if (!this.Held)
		{
			return;
		}
		for (int i = 0; i < this.points.Length; i++)
		{
			this.UpdateInteractionPoint(ref this.points[i]);
		}
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x00048018 File Offset: 0x00046218
	private void StartGrabbing()
	{
		this.Held = true;
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		this._isLocal = (componentInParent == VRRig.LocalRig);
		this.ResetVelocity();
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x0004804A File Offset: 0x0004624A
	private void StopGrabbing()
	{
		this.Held = false;
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x00048054 File Offset: 0x00046254
	private void ResetVelocity()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			this.points[i].position = (this.points[i].previousPosition = this.points[i].transform.position);
		}
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x000480B0 File Offset: 0x000462B0
	private void UpdateInteractionPoint(ref Voxel_Pickaxe.InteractionPoint point)
	{
		point.previousPosition = point.position;
		point.position = point.transform.position;
		if (Time.time < this._nextHitTime)
		{
			return;
		}
		Vector3 vector = (point.position - point.previousPosition) / Time.fixedDeltaTime;
		float magnitude = vector.magnitude;
		if (magnitude < this.minHitSpeed)
		{
			return;
		}
		bool flag = Vector3.Dot(vector.normalized, point.transform.forward) >= this.alignThreshold;
		RaycastHit hit;
		if (Physics.Linecast(point.previousPosition, point.position, out hit, this._layerMask, QueryTriggerInteraction.Ignore))
		{
			ChunkComponent component = hit.collider.GetComponent<ChunkComponent>();
			if (component && flag && magnitude >= this.minMineSpeed)
			{
				this.Play(this.goodHit, hit.point);
				if (this._isLocal)
				{
					component.World.Mine(hit, this.mine);
				}
			}
			else
			{
				this.Play(this.badHit, hit.point);
			}
			this._nextHitTime = Time.time + this.hitCooldown;
		}
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x000481CC File Offset: 0x000463CC
	private void Play(AudioResource resource, Vector3 position)
	{
		if (!resource)
		{
			return;
		}
		this.sound.Stop();
		this.sound.resource = resource;
		this.sound.transform.position = position;
		this.sound.Play();
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x0004820A File Offset: 0x0004640A
	public void OnEntityInit()
	{
		if (this.sound != null)
		{
			this.sound.transform.parent = null;
		}
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x0004822C File Offset: 0x0004642C
	public void OnEntityDestroy()
	{
		if (ApplicationQuittingState.IsQuitting || this == null || this.sound == null || !base.gameObject.scene.isLoaded)
		{
			return;
		}
		this.sound.transform.parent = base.transform;
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x00048284 File Offset: 0x00046484
	private void OnDrawGizmosSelected()
	{
		if (this.points == null)
		{
			return;
		}
		Gizmos.color = Color.green;
		foreach (Voxel_Pickaxe.InteractionPoint interactionPoint in this.points)
		{
			Gizmos.DrawWireSphere(interactionPoint.transform.position, 0.02f);
			Gizmos.DrawLine(interactionPoint.transform.position, interactionPoint.transform.position + interactionPoint.transform.forward * 0.5f);
		}
	}

	// Token: 0x04000FB8 RID: 4024
	public VoxelAction mine = new VoxelAction
	{
		strength = 1f,
		radius = 0.5f,
		operation = OperationType.Subtract
	};

	// Token: 0x04000FB9 RID: 4025
	public Voxel_Pickaxe.InteractionPoint[] points;

	// Token: 0x04000FBA RID: 4026
	public AudioResource goodHit;

	// Token: 0x04000FBB RID: 4027
	public AudioResource badHit;

	// Token: 0x04000FBC RID: 4028
	public AudioSource sound;

	// Token: 0x04000FBD RID: 4029
	public float hitCooldown = 0.5f;

	// Token: 0x04000FBE RID: 4030
	public float minHitSpeed = 1f;

	// Token: 0x04000FBF RID: 4031
	public float minMineSpeed = 5f;

	// Token: 0x04000FC0 RID: 4032
	public float alignThreshold = 0.7f;

	// Token: 0x04000FC1 RID: 4033
	private GameEntity _gameEntity;

	// Token: 0x04000FC2 RID: 4034
	private int _layerMask;

	// Token: 0x04000FC3 RID: 4035
	private float _nextHitTime;

	// Token: 0x04000FC4 RID: 4036
	private bool _isLocal;

	// Token: 0x020001F5 RID: 501
	[Serializable]
	public struct InteractionPoint
	{
		// Token: 0x04000FC6 RID: 4038
		public Transform transform;

		// Token: 0x04000FC7 RID: 4039
		public Vector3 previousPosition;

		// Token: 0x04000FC8 RID: 4040
		public Vector3 position;
	}
}
