using System;
using System.Collections;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Shared.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200123E RID: 4670
	public class ThrowableHoldableCosmetic : TransferrableObject
	{
		// Token: 0x060074E3 RID: 29923 RVA: 0x00264708 File Offset: 0x00262908
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnThrowEvent;
			}
			this.forceBackToDock = false;
		}

		// Token: 0x060074E4 RID: 29924 RVA: 0x002647D4 File Offset: 0x002629D4
		protected override void Awake()
		{
			base.Awake();
			this.projectileHash = PoolUtils.GameObjHashCode(this.projectilePrefab);
			if (this.alternativeProjectilePrefab != null)
			{
				this.alternativeProjectileHash = PoolUtils.GameObjHashCode(this.alternativeProjectilePrefab);
			}
			this.currentProjectileHash = this.projectileHash;
			this.playersEffect = base.GetComponentInChildren<CosmeticEffectsOnPlayers>();
			this.respawnWait = new WaitForSeconds(this.respawnCooldown);
		}

		// Token: 0x060074E5 RID: 29925 RVA: 0x00264840 File Offset: 0x00262A40
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (!this.disableWhenThrown.gameObject.activeSelf)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x060074E6 RID: 29926 RVA: 0x00264860 File Offset: 0x00262A60
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return false;
			}
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
			Vector3 averageVelocity = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand).GetAverageVelocity(true, 0.15f, false);
			float scale = GTPlayer.Instance.scale;
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					position,
					rotation,
					averageVelocity,
					scale
				});
			}
			this.OnThrowLocal(position, rotation, averageVelocity, this.ownerRig);
			return true;
		}

		// Token: 0x060074E7 RID: 29927 RVA: 0x0026495C File Offset: 0x00262B5C
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnThrowEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x060074E8 RID: 29928 RVA: 0x002649B1 File Offset: 0x00262BB1
		public void UseAlternativeProjectile()
		{
			if (this.alternativeProjectilePrefab != null)
			{
				this.currentProjectileHash = this.alternativeProjectileHash;
			}
		}

		// Token: 0x060074E9 RID: 29929 RVA: 0x002649CD File Offset: 0x00262BCD
		public void ForceBackToDock()
		{
			this.forceBackToDock = true;
		}

		// Token: 0x060074EA RID: 29930 RVA: 0x002649D6 File Offset: 0x00262BD6
		private IEnumerator ReEnableAfterDelay(GameObject obj)
		{
			yield return this.respawnWait;
			obj.SetActive(true);
			yield break;
		}

		// Token: 0x060074EB RID: 29931 RVA: 0x002649EC File Offset: 0x00262BEC
		private void OnThrowEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 4)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnThrowEvent");
			if (this.firecrackerCallLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 vector = (Vector3)obj;
					obj = args[1];
					if (obj is Quaternion)
					{
						Quaternion rotation = (Quaternion)obj;
						obj = args[2];
						if (obj is Vector3)
						{
							Vector3 vector2 = (Vector3)obj;
							obj = args[3];
							if (obj is float)
							{
								float value = (float)obj;
								vector2 = this.targetRig.ClampVelocityRelativeToPlayerSafe(vector2, 40f, 100f);
								value.ClampSafe(0.01f, 1f);
								if (!rotation.IsValid())
								{
									return;
								}
								float num = 10000f;
								if (!vector.IsValid(num) || !this.targetRig.IsPositionInRange(vector, 4f))
								{
									return;
								}
								this.OnThrowLocal(vector, rotation, vector2, this.ownerRig);
								return;
							}
						}
					}
				}
			}
		}

		// Token: 0x060074EC RID: 29932 RVA: 0x00264B00 File Offset: 0x00262D00
		private void OnThrowLocal(Vector3 startPos, Quaternion rotation, Vector3 velocity, VRRig ownerRig)
		{
			this.disableWhenThrown.SetActive(false);
			if (this.forceBackToDock)
			{
				this.forceBackToDock = false;
				base.StartCoroutine(this.ReEnableAfterDelay(this.disableWhenThrown));
				return;
			}
			IProjectile component = ObjectPools.instance.Instantiate(this.currentProjectileHash, true).GetComponent<IProjectile>();
			FirecrackerProjectile firecrackerProjectile = component as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
				{
					int state = (int)(this.itemState & (TransferrableObject.ItemStates)(-65));
					firecrackerProjectile.SetTransferrableState(this.networkedStateEvents, state);
				}
				firecrackerProjectile.OnDetonationComplete.AddListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
				firecrackerProjectile.OnDetonationStart.AddListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
			}
			else
			{
				FartBagThrowable fartBagThrowable = component as FartBagThrowable;
				if (fartBagThrowable != null)
				{
					fartBagThrowable.OnDeflated += this.HitComplete;
					fartBagThrowable.ParentTransferable = this;
				}
			}
			component.Launch(startPos, rotation, velocity, 1f, ownerRig, -1);
			this.currentProjectileHash = this.projectileHash;
		}

		// Token: 0x060074ED RID: 29933 RVA: 0x00264BEC File Offset: 0x00262DEC
		private void HitStart(FirecrackerProjectile firecracker, Vector3 contactPos)
		{
			if (firecracker == null)
			{
				return;
			}
			if (this.playersEffect == null)
			{
				return;
			}
			this.playersEffect.ApplyAllEffectsByDistance(contactPos);
		}

		// Token: 0x060074EE RID: 29934 RVA: 0x00264C14 File Offset: 0x00262E14
		private void HitComplete(IProjectile projectile)
		{
			if (projectile == null)
			{
				return;
			}
			if (base.IsLocalObject() && this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.resetOnDocked)
			{
				TransferrableObject.SyncOptions networkedStateEvents = this.networkedStateEvents;
				if (networkedStateEvents != TransferrableObject.SyncOptions.Bool)
				{
					if (networkedStateEvents == TransferrableObject.SyncOptions.Int)
					{
						base.SetItemStateInt(0);
					}
				}
				else
				{
					base.ResetStateBools();
				}
			}
			FirecrackerProjectile firecrackerProjectile = projectile as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				firecrackerProjectile.OnDetonationStart.RemoveListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
				firecrackerProjectile.OnDetonationComplete.RemoveListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
				ObjectPools.instance.Destroy(firecrackerProjectile.gameObject);
			}
			else
			{
				FartBagThrowable fartBagThrowable = projectile as FartBagThrowable;
				if (fartBagThrowable != null)
				{
					fartBagThrowable.OnDeflated -= this.HitComplete;
					ObjectPools.instance.Destroy(fartBagThrowable.gameObject);
				}
			}
			base.StartCoroutine(this.ReEnableAfterDelay(this.disableWhenThrown));
		}

		// Token: 0x04008662 RID: 34402
		[Tooltip("Projectile prefab from the global object pool that gets spawned when this object is thrown")]
		[FormerlySerializedAs("firecrackerProjectilePrefab")]
		[SerializeField]
		private GameObject projectilePrefab;

		// Token: 0x04008663 RID: 34403
		[Tooltip(" A second projectile prefab that will be spawned if UseAlternativeProjectile is called")]
		[SerializeField]
		private GameObject alternativeProjectilePrefab;

		// Token: 0x04008664 RID: 34404
		[Tooltip("Objects on the body that should be hidden when the projectile is spawned")]
		[SerializeField]
		private GameObject disableWhenThrown;

		// Token: 0x04008665 RID: 34405
		private CallLimiter firecrackerCallLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04008666 RID: 34406
		[SerializeField]
		private float respawnCooldown = 1f;

		// Token: 0x04008667 RID: 34407
		private CosmeticEffectsOnPlayers playersEffect;

		// Token: 0x04008668 RID: 34408
		private int projectileHash;

		// Token: 0x04008669 RID: 34409
		private int alternativeProjectileHash;

		// Token: 0x0400866A RID: 34410
		private int currentProjectileHash;

		// Token: 0x0400866B RID: 34411
		private bool forceBackToDock;

		// Token: 0x0400866C RID: 34412
		private WaitForSeconds respawnWait;

		// Token: 0x0400866D RID: 34413
		private RubberDuckEvents _events;
	}
}
