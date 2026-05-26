using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x02001127 RID: 4391
	[RequireComponent(typeof(TransferrableObject))]
	public class CosmeticParticleSurfaceEffect : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06006F80 RID: 28544 RVA: 0x00246367 File Offset: 0x00244567
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			if (this.surfaceEffectPrefab != null)
			{
				this.surfaceEffectHash = PoolUtils.GameObjHashCode(this.surfaceEffectPrefab);
			}
		}

		// Token: 0x06006F81 RID: 28545 RVA: 0x00246394 File Offset: 0x00244594
		private void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				this.owner = ((this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null));
				if (this.owner != null)
				{
					this._events.Init(this.owner);
					this.isLocal = this.owner.IsLocal;
				}
			}
			if (this._events != null)
			{
				this._events.Activate.reliable = true;
				this._events.Deactivate.reliable = true;
				this._events.Activate += this.OnSpawnReplicated;
				this._events.Deactivate += this.OnTriggerEffectReplicated;
			}
			if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
			{
				return;
			}
			if (this.surfaceEffectHash != 0)
			{
				this._pool = ObjectPools.instance.GetPoolByHash(this.surfaceEffectHash);
				if (this._pool != null)
				{
					this.foundPool = true;
				}
				else
				{
					GTDev.LogError<string>("CosmeticParticleSurfaceEffect " + base.gameObject.name + " no Object pool found for surface effect prefab. Has it been added to Global Object Pools?", null);
				}
			}
			this.spawnCallLimiter.Reset();
			this.destroyCallLimiter.Reset();
			this.lastHitTime = float.MinValue;
		}

		// Token: 0x06006F82 RID: 28546 RVA: 0x0024654C File Offset: 0x0024474C
		private void OnDisable()
		{
			this.StopParticles();
			if (this._events != null)
			{
				this._events.Activate -= this.OnSpawnReplicated;
				this._events.Deactivate -= this.OnTriggerEffectReplicated;
				this._events.Dispose();
				this._events = null;
			}
			this.surfaceEffectNum.Clear();
			foreach (SeedPacketTriggerHandler seedPacketTriggerHandler in this.surfaceEffects)
			{
				if (!(seedPacketTriggerHandler == null))
				{
					seedPacketTriggerHandler.onTriggerEntered.RemoveListener(new UnityAction<SeedPacketTriggerHandler>(this.OnTriggerEffectLocal));
				}
			}
			this.surfaceEffects.Clear();
		}

		// Token: 0x06006F83 RID: 28547 RVA: 0x00246638 File Offset: 0x00244838
		private void OnDestroy()
		{
			this.surfaceEffectNum.Clear();
			this.surfaceEffects.Clear();
		}

		// Token: 0x06006F84 RID: 28548 RVA: 0x00246650 File Offset: 0x00244850
		public void StartParticles()
		{
			if (!this.isSpawning)
			{
				this.isSpawning = true;
				this.particleStartedTime = Time.time;
				if (!this.particles.isPlaying)
				{
					this.particles.Play();
				}
			}
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006F85 RID: 28549 RVA: 0x002466A0 File Offset: 0x002448A0
		public void StopParticles()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			this.isSpawning = false;
			this.particleStartedTime = float.MinValue;
			this.lastHitTime = float.MinValue;
			if (this.particles.isPlaying)
			{
				this.particles.Stop();
			}
		}

		// Token: 0x17000AA2 RID: 2722
		// (get) Token: 0x06006F86 RID: 28550 RVA: 0x002466F0 File Offset: 0x002448F0
		// (set) Token: 0x06006F87 RID: 28551 RVA: 0x002466F8 File Offset: 0x002448F8
		public bool TickRunning { get; set; }

		// Token: 0x06006F88 RID: 28552 RVA: 0x00246704 File Offset: 0x00244904
		public void Tick()
		{
			if (this.transferrableObject == null || !this.transferrableObject.InHand())
			{
				this.StopParticles();
				return;
			}
			if (this.isSpawning && this.stopAfterSeconds > 0f && Time.time >= this.particleStartedTime + this.stopAfterSeconds)
			{
				this.StopParticles();
				return;
			}
			if (!this.isLocal)
			{
				return;
			}
			if (this.isSpawning && Time.time > this.placeEffectCooldown + this.lastHitTime)
			{
				int num = Physics.RaycastNonAlloc(this.rayCastOrigin.position, this.useWorldDirection ? this.worldDirection : this.rayCastOrigin.forward, this.hits, this.rayCastDistance, this.rayCastLayerMask, QueryTriggerInteraction.Ignore);
				if (num > 0)
				{
					int num2 = 0;
					float distance = this.hits[num2].distance;
					for (int i = 1; i < num; i++)
					{
						if (this.hits[i].distance < distance)
						{
							num2 = i;
							distance = this.hits[i].distance;
						}
					}
					this.hitPoint = this.hits[num2];
					this.lastHitTime = Time.time;
					base.Invoke("SpawnEffect", distance * this.placeEffectDelayMultiplier);
				}
			}
		}

		// Token: 0x06006F89 RID: 28553 RVA: 0x00246850 File Offset: 0x00244A50
		private void SpawnEffect()
		{
			if (!this.isLocal)
			{
				return;
			}
			long num = BitPackUtils.PackWorldPosForNetwork(this.hitPoint.point);
			long num2 = BitPackUtils.PackWorldPosForNetwork(this.hitPoint.normal);
			int num3 = this.currentEffect;
			this.currentEffect++;
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					num,
					num2,
					num3
				});
			}
			this.SpawnLocal(this.hitPoint.point, this.hitPoint.normal, num3);
		}

		// Token: 0x06006F8A RID: 28554 RVA: 0x00246914 File Offset: 0x00244B14
		private void OnSpawnReplicated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (!this || sender != target || this.owner == null || info.senderID != this.owner.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnSpawnReplicated");
			if (!this.spawnCallLimiter.CheckCallTime(Time.time) || args.Length != 3 || !(args[0] is long) || !(args[1] is long) || !(args[2] is int))
			{
				return;
			}
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork((long)args[0]);
			Vector3 vector2 = BitPackUtils.UnpackWorldPosFromNetwork((long)args[1]);
			float num = 10000f;
			if (vector.IsValid(num))
			{
				float num2 = 10000f;
				if (vector2.IsValid(num2))
				{
					if (Vector3.Distance(this.rayCastOrigin.position, vector) > this.rayCastDistance + 2f)
					{
						return;
					}
					vector2.Normalize();
					if (vector2 == Vector3.zero)
					{
						vector2 = Vector3.up;
					}
					int identifier = (int)args[2];
					this.SpawnLocal(vector, vector2, identifier);
					return;
				}
			}
		}

		// Token: 0x06006F8B RID: 28555 RVA: 0x00246A1C File Offset: 0x00244C1C
		private void SpawnLocal(Vector3 position, Vector3 up, int identifier)
		{
			if (this.surfaceEffectHash != 0 && !this.foundPool)
			{
				this._pool = ObjectPools.instance.GetPoolByHash(this.surfaceEffectHash);
				if (this._pool == null)
				{
					return;
				}
				this.foundPool = true;
			}
			if (this.foundPool && this._pool.GetInactiveCount() > 0)
			{
				this.ClearOldObjects();
				GameObject gameObject = this._pool.Instantiate(true);
				gameObject.transform.position = position;
				gameObject.transform.up = up;
				SeedPacketTriggerHandler seedPacketTriggerHandler;
				if (gameObject.TryGetComponent<SeedPacketTriggerHandler>(out seedPacketTriggerHandler))
				{
					int num = this.surfaceEffects.IndexOf(seedPacketTriggerHandler);
					if (num >= 0)
					{
						this.surfaceEffectNum[num] = identifier;
					}
					else
					{
						this.surfaceEffectNum.Add(identifier);
						this.surfaceEffects.Add(seedPacketTriggerHandler);
					}
					seedPacketTriggerHandler.onTriggerEntered.AddListener(new UnityAction<SeedPacketTriggerHandler>(this.OnTriggerEffectLocal));
				}
			}
		}

		// Token: 0x06006F8C RID: 28556 RVA: 0x00246B00 File Offset: 0x00244D00
		private void ClearOldObjects()
		{
			for (int i = this.surfaceEffects.Count - 1; i >= 0; i--)
			{
				if (this.surfaceEffects[i] == null)
				{
					this.surfaceEffects.RemoveAt(i);
					this.surfaceEffectNum.RemoveAt(i);
				}
				else if (!this.surfaceEffects[i].gameObject.activeSelf)
				{
					this.surfaceEffects[i].onTriggerEntered.RemoveListener(new UnityAction<SeedPacketTriggerHandler>(this.OnTriggerEffectLocal));
					this.surfaceEffects.RemoveAt(i);
					this.surfaceEffectNum.RemoveAt(i);
				}
			}
		}

		// Token: 0x06006F8D RID: 28557 RVA: 0x00246BAC File Offset: 0x00244DAC
		private void OnTriggerEffectLocal(SeedPacketTriggerHandler seedPacketTriggerHandlerTriggerHandlerEvent)
		{
			int num = this.surfaceEffects.IndexOf(seedPacketTriggerHandlerTriggerHandlerEvent);
			if (num >= 0 && num < this.surfaceEffectNum.Count)
			{
				int num2 = this.surfaceEffectNum[num];
				if (PhotonNetwork.InRoom && this._events != null && this._events.Deactivate != null)
				{
					this._events.Deactivate.RaiseOthers(new object[]
					{
						num2
					});
				}
				this.surfaceEffects.RemoveAt(num);
				this.surfaceEffectNum.RemoveAt(num);
			}
		}

		// Token: 0x06006F8E RID: 28558 RVA: 0x00246C44 File Offset: 0x00244E44
		private void OnTriggerEffectReplicated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnTriggerEffectReplicated");
			if (!this.destroyCallLimiter.CheckCallTime(Time.time) || args.Length != 1 || !(args[0] is int))
			{
				return;
			}
			this.ClearOldObjects();
			int item = (int)args[0];
			int num = this.surfaceEffectNum.IndexOf(item);
			if (num >= 0 && num < this.surfaceEffects.Count)
			{
				SeedPacketTriggerHandler seedPacketTriggerHandler = this.surfaceEffects[num];
				if (seedPacketTriggerHandler != null)
				{
					seedPacketTriggerHandler.ToggleEffects();
					seedPacketTriggerHandler.onTriggerEntered.RemoveListener(new UnityAction<SeedPacketTriggerHandler>(this.OnTriggerEffectLocal));
				}
				this.surfaceEffects.RemoveAt(num);
				this.surfaceEffectNum.RemoveAt(num);
			}
		}

		// Token: 0x04007F5A RID: 32602
		[Tooltip("autoStop particle system this many seconds after starting")]
		[SerializeField]
		private float stopAfterSeconds = 3f;

		// Token: 0x04007F5B RID: 32603
		[Tooltip("particle system to play on start particles")]
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x04007F5C RID: 32604
		[Tooltip("Distance in meters to check for a surface hit")]
		[SerializeField]
		private float rayCastDistance = 20f;

		// Token: 0x04007F5D RID: 32605
		[Tooltip("The position for the start of the rayCast.\nThe forward (z+) axis of this transform will be used as the rayCast direction\nThis should visually line up with the spawned particles")]
		[SerializeField]
		private Transform rayCastOrigin;

		// Token: 0x04007F5E RID: 32606
		[Tooltip("Use a world direction vector for the raycast instead of the rayCastOrigin forward?")]
		[SerializeField]
		private bool useWorldDirection;

		// Token: 0x04007F5F RID: 32607
		[SerializeField]
		private Vector3 worldDirection = Vector3.down;

		// Token: 0x04007F60 RID: 32608
		[Tooltip("Layers to check for surface collision")]
		[SerializeField]
		private LayerMask rayCastLayerMask = 513;

		// Token: 0x04007F61 RID: 32609
		[Tooltip("Prefab from the global object pool to spawn on surface hit\nIf it should be destroyed on touch, add a SeedPacketTriggerHandler to the prefab")]
		[SerializeField]
		private GameObject surfaceEffectPrefab;

		// Token: 0x04007F62 RID: 32610
		[Tooltip("Seconds per meter to wait before spawning a surface effect on hit.\n A good value would be somewhat close to 1/particle velocity ")]
		[SerializeField]
		private float placeEffectDelayMultiplier = 3f;

		// Token: 0x04007F63 RID: 32611
		[Tooltip("Time to wait between spawning surface effects")]
		[SerializeField]
		private float placeEffectCooldown = 2f;

		// Token: 0x04007F64 RID: 32612
		private float particleStartedTime;

		// Token: 0x04007F65 RID: 32613
		private bool isSpawning;

		// Token: 0x04007F66 RID: 32614
		private float lastHitTime = float.MinValue;

		// Token: 0x04007F67 RID: 32615
		private RaycastHit hitPoint;

		// Token: 0x04007F68 RID: 32616
		private RaycastHit[] hits = new RaycastHit[5];

		// Token: 0x04007F69 RID: 32617
		private TransferrableObject transferrableObject;

		// Token: 0x04007F6A RID: 32618
		private bool isLocal;

		// Token: 0x04007F6B RID: 32619
		private NetPlayer owner;

		// Token: 0x04007F6C RID: 32620
		private int surfaceEffectHash;

		// Token: 0x04007F6D RID: 32621
		private RubberDuckEvents _events;

		// Token: 0x04007F6E RID: 32622
		private CallLimiter spawnCallLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04007F6F RID: 32623
		private CallLimiter destroyCallLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04007F70 RID: 32624
		private SinglePool _pool;

		// Token: 0x04007F71 RID: 32625
		private bool foundPool;

		// Token: 0x04007F72 RID: 32626
		private int currentEffect;

		// Token: 0x04007F73 RID: 32627
		private List<int> surfaceEffectNum = new List<int>();

		// Token: 0x04007F74 RID: 32628
		private List<SeedPacketTriggerHandler> surfaceEffects = new List<SeedPacketTriggerHandler>(10);
	}
}
