using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200127E RID: 4734
	public class FartBagThrowable : MonoBehaviour, IProjectile
	{
		// Token: 0x17000B75 RID: 2933
		// (get) Token: 0x060076AF RID: 30383 RVA: 0x0026EB8B File Offset: 0x0026CD8B
		// (set) Token: 0x060076B0 RID: 30384 RVA: 0x0026EB93 File Offset: 0x0026CD93
		public TransferrableObject ParentTransferable { get; set; }

		// Token: 0x140000C3 RID: 195
		// (add) Token: 0x060076B1 RID: 30385 RVA: 0x0026EB9C File Offset: 0x0026CD9C
		// (remove) Token: 0x060076B2 RID: 30386 RVA: 0x0026EBD4 File Offset: 0x0026CDD4
		public event Action<IProjectile> OnDeflated;

		// Token: 0x060076B3 RID: 30387 RVA: 0x0026EC0C File Offset: 0x0026CE0C
		private void OnEnable()
		{
			this.placedOnFloor = false;
			this.deflated = false;
			this.handContactPoint = Vector3.negativeInfinity;
			this.handNormalVector = Vector3.zero;
			this.timeCreated = float.PositiveInfinity;
			this.placedOnFloorTime = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.ResetBlend();
			}
		}

		// Token: 0x060076B4 RID: 30388 RVA: 0x0026EC6B File Offset: 0x0026CE6B
		private void Update()
		{
			if (Time.time - this.timeCreated > this.forceDestroyAfterSec)
			{
				this.DeflateLocal();
			}
		}

		// Token: 0x060076B5 RID: 30389 RVA: 0x0026EC88 File Offset: 0x0026CE88
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
			this.rigidbody.linearVelocity = velocity;
			this.timeCreated = Time.time;
			this.InitialPhotonEvent();
		}

		// Token: 0x060076B6 RID: 30390 RVA: 0x0026ECE8 File Offset: 0x0026CEE8
		private void InitialPhotonEvent()
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			if (this.ParentTransferable)
			{
				NetPlayer netPlayer = (this.ParentTransferable.myOnlineRig != null) ? this.ParentTransferable.myOnlineRig.creator : ((this.ParentTransferable.myRig != null) ? (this.ParentTransferable.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (this._events != null && netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.DeflateEvent;
			}
		}

		// Token: 0x060076B7 RID: 30391 RVA: 0x0026EDBC File Offset: 0x0026CFBC
		private void OnTriggerEnter(Collider other)
		{
			if ((this.handLayerMask.value & 1 << other.gameObject.layer) != 0)
			{
				if (!this.placedOnFloor)
				{
					return;
				}
				this.handContactPoint = other.ClosestPoint(base.transform.position);
				this.handNormalVector = (this.handContactPoint - base.transform.position).normalized;
				if (Time.time - this.placedOnFloorTime > 0.3f)
				{
					this.Deflate();
				}
			}
		}

		// Token: 0x060076B8 RID: 30392 RVA: 0x0026EE44 File Offset: 0x0026D044
		private void OnCollisionEnter(Collision other)
		{
			if ((this.floorLayerMask.value & 1 << other.gameObject.layer) != 0)
			{
				this.placedOnFloor = true;
				this.placedOnFloorTime = Time.time;
				Vector3 normal = other.contacts[0].normal;
				base.transform.position = other.contacts[0].point + normal * this.placementOffset;
				Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
				base.transform.rotation = rotation;
			}
		}

		// Token: 0x060076B9 RID: 30393 RVA: 0x0026EEEC File Offset: 0x0026D0EC
		private void Deflate()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					this.handContactPoint,
					this.handNormalVector
				});
			}
			this.DeflateLocal();
		}

		// Token: 0x060076BA RID: 30394 RVA: 0x0026EF5C File Offset: 0x0026D15C
		private void DeflateEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 2)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "DeflateEvent");
			if (this.callLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 position = (Vector3)obj;
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector = (Vector3)obj;
						float num = 10000f;
						if (!vector.IsValid(num))
						{
							return;
						}
						num = 10000f;
						if (!position.IsValid(num) || !this.ParentTransferable.targetRig.IsPositionInRange(position, 4f))
						{
							return;
						}
						this.handNormalVector = vector;
						this.handContactPoint = position;
						this.DeflateLocal();
						return;
					}
				}
			}
		}

		// Token: 0x060076BB RID: 30395 RVA: 0x0026F00C File Offset: 0x0026D20C
		private void DeflateLocal()
		{
			if (this.deflated)
			{
				return;
			}
			GameObject gameObject = ObjectPools.instance.Instantiate(this.deflationEffect, this.handContactPoint, true);
			gameObject.transform.up = this.handNormalVector;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer componentInChildren = gameObject.GetComponentInChildren<SoundBankPlayer>();
			if (componentInChildren.soundBank)
			{
				componentInChildren.Play();
			}
			this.placedOnFloor = false;
			this.timeCreated = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.FullyBlend();
			}
			this.deflated = true;
			base.Invoke("DisableObject", this.destroyWhenDeflateDelay);
		}

		// Token: 0x060076BC RID: 30396 RVA: 0x0026F0BB File Offset: 0x0026D2BB
		private void DisableObject()
		{
			Action<IProjectile> onDeflated = this.OnDeflated;
			if (onDeflated != null)
			{
				onDeflated(this);
			}
			this.deflated = false;
		}

		// Token: 0x060076BD RID: 30397 RVA: 0x0026F0D8 File Offset: 0x0026D2D8
		private void OnDestroy()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.DeflateEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x040088BA RID: 35002
		[SerializeField]
		private GameObject deflationEffect;

		// Token: 0x040088BB RID: 35003
		[SerializeField]
		private float destroyWhenDeflateDelay = 3f;

		// Token: 0x040088BC RID: 35004
		[SerializeField]
		private float forceDestroyAfterSec = 10f;

		// Token: 0x040088BD RID: 35005
		[SerializeField]
		private float placementOffset = 0.2f;

		// Token: 0x040088BE RID: 35006
		[SerializeField]
		private UpdateBlendShapeCosmetic updateBlendShapeCosmetic;

		// Token: 0x040088BF RID: 35007
		[SerializeField]
		private LayerMask floorLayerMask;

		// Token: 0x040088C0 RID: 35008
		[SerializeField]
		private LayerMask handLayerMask;

		// Token: 0x040088C1 RID: 35009
		[SerializeField]
		private Rigidbody rigidbody;

		// Token: 0x040088C2 RID: 35010
		private bool placedOnFloor;

		// Token: 0x040088C3 RID: 35011
		private float placedOnFloorTime;

		// Token: 0x040088C4 RID: 35012
		private float timeCreated;

		// Token: 0x040088C5 RID: 35013
		private bool deflated;

		// Token: 0x040088C6 RID: 35014
		private Vector3 handContactPoint;

		// Token: 0x040088C7 RID: 35015
		private Vector3 handNormalVector;

		// Token: 0x040088C8 RID: 35016
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x040088CB RID: 35019
		private RubberDuckEvents _events;
	}
}
