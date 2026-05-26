using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200123A RID: 4666
	[RequireComponent(typeof(TransferrableObject))]
	public class SeedPacketHoldable : MonoBehaviour
	{
		// Token: 0x060074BC RID: 29884 RVA: 0x00263D5E File Offset: 0x00261F5E
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.flowerEffectHash = PoolUtils.GameObjHashCode(this.flowerEffectPrefab);
		}

		// Token: 0x060074BD RID: 29885 RVA: 0x00263D80 File Offset: 0x00261F80
		private void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.SyncTriggerEffect;
			}
		}

		// Token: 0x060074BE RID: 29886 RVA: 0x00263E48 File Offset: 0x00262048
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.SyncTriggerEffect;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x060074BF RID: 29887 RVA: 0x00263E97 File Offset: 0x00262097
		private void OnDestroy()
		{
			this.pooledObjects.Clear();
		}

		// Token: 0x060074C0 RID: 29888 RVA: 0x00263EA4 File Offset: 0x002620A4
		private void Update()
		{
			if (!this.transferrableObject.InHand())
			{
				return;
			}
			if (!this.isPouring && Vector3.Angle(base.transform.up, Vector3.down) <= this.pouringAngle)
			{
				this.StartPouring();
				RaycastHit raycastHit;
				if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, this.pouringRaycastDistance, this.raycastLayerMask))
				{
					this.hitPoint = raycastHit.point;
					base.Invoke("SpawnEffect", raycastHit.distance * this.placeEffectDelayMultiplier);
				}
			}
			if (this.isPouring && Time.time - this.pouringStartedTime >= this.cooldown)
			{
				this.isPouring = false;
			}
		}

		// Token: 0x060074C1 RID: 29889 RVA: 0x00263F5D File Offset: 0x0026215D
		private void StartPouring()
		{
			if (this.particles)
			{
				this.particles.Play();
			}
			this.isPouring = true;
			this.pouringStartedTime = Time.time;
		}

		// Token: 0x060074C2 RID: 29890 RVA: 0x00263F8C File Offset: 0x0026218C
		private void SpawnEffect()
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(this.flowerEffectHash, true);
			gameObject.transform.position = this.hitPoint;
			SeedPacketTriggerHandler seedPacketTriggerHandler;
			if (gameObject.TryGetComponent<SeedPacketTriggerHandler>(out seedPacketTriggerHandler))
			{
				this.pooledObjects.Add(seedPacketTriggerHandler);
				seedPacketTriggerHandler.onTriggerEntered.AddListener(new UnityAction<SeedPacketTriggerHandler>(this.SyncTriggerEffectForOthers));
			}
		}

		// Token: 0x060074C3 RID: 29891 RVA: 0x00263FE8 File Offset: 0x002621E8
		private void SyncTriggerEffectForOthers(SeedPacketTriggerHandler seedPacketTriggerHandlerTriggerHandlerEvent)
		{
			int num = this.pooledObjects.IndexOf(seedPacketTriggerHandlerTriggerHandlerEvent);
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					num
				});
			}
		}

		// Token: 0x060074C4 RID: 29892 RVA: 0x0026404C File Offset: 0x0026224C
		private void SyncTriggerEffect(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "SyncTriggerEffect");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			int num = (int)args[0];
			if (num < 0 && num >= this.pooledObjects.Count)
			{
				return;
			}
			this.pooledObjects[num].ToggleEffects();
		}

		// Token: 0x0400862F RID: 34351
		[SerializeField]
		private float cooldown;

		// Token: 0x04008630 RID: 34352
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x04008631 RID: 34353
		[SerializeField]
		private float pouringAngle;

		// Token: 0x04008632 RID: 34354
		[SerializeField]
		private float pouringRaycastDistance = 5f;

		// Token: 0x04008633 RID: 34355
		[SerializeField]
		private LayerMask raycastLayerMask;

		// Token: 0x04008634 RID: 34356
		[SerializeField]
		private float placeEffectDelayMultiplier = 10f;

		// Token: 0x04008635 RID: 34357
		[SerializeField]
		private GameObject flowerEffectPrefab;

		// Token: 0x04008636 RID: 34358
		private List<SeedPacketTriggerHandler> pooledObjects = new List<SeedPacketTriggerHandler>();

		// Token: 0x04008637 RID: 34359
		private CallLimiter callLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04008638 RID: 34360
		private int flowerEffectHash;

		// Token: 0x04008639 RID: 34361
		private Vector3 hitPoint;

		// Token: 0x0400863A RID: 34362
		private TransferrableObject transferrableObject;

		// Token: 0x0400863B RID: 34363
		private bool isPouring = true;

		// Token: 0x0400863C RID: 34364
		private float pouringStartedTime;

		// Token: 0x0400863D RID: 34365
		private RubberDuckEvents _events;
	}
}
