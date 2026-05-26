using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200123C RID: 4668
	public class StickObjectToPlayer : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000B2B RID: 2859
		// (get) Token: 0x060074D5 RID: 29909 RVA: 0x00264262 File Offset: 0x00262462
		// (set) Token: 0x060074D6 RID: 29910 RVA: 0x0026426A File Offset: 0x0026246A
		public bool TickRunning { get; set; }

		// Token: 0x060074D7 RID: 29911 RVA: 0x00264273 File Offset: 0x00262473
		public void Tick()
		{
			if (!this.canSpawn && Time.time - this.lastSpawnedTime >= this.cooldown)
			{
				this.canSpawn = true;
			}
		}

		// Token: 0x060074D8 RID: 29912 RVA: 0x00264298 File Offset: 0x00262498
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
			this.canSpawn = true;
		}

		// Token: 0x060074D9 RID: 29913 RVA: 0x00019E47 File Offset: 0x00018047
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x060074DA RID: 29914 RVA: 0x002642A7 File Offset: 0x002624A7
		public void SetOwner(NetPlayer player)
		{
			this.ownerPlayer = player;
		}

		// Token: 0x060074DB RID: 29915 RVA: 0x002642B0 File Offset: 0x002624B0
		private Transform MakeOrGetStickyContainer(Transform parent)
		{
			Transform transform = parent;
			foreach (Transform transform2 in parent.GetComponentsInChildren<Transform>(true))
			{
				if (!this.firstPersonView && transform2.CompareTag(this.parentTag))
				{
					transform = transform2;
					break;
				}
			}
			string text = "StickyObjects_" + this.objectToSpawn.name;
			Transform transform3 = transform.Find(text);
			if (transform3 != null)
			{
				return transform3;
			}
			GameObject gameObject = new GameObject(text);
			gameObject.transform.SetParent(transform, false);
			return gameObject.transform;
		}

		// Token: 0x060074DC RID: 29916 RVA: 0x0026433C File Offset: 0x0026253C
		public void Stick(bool leftHand, Collider other)
		{
			if (!this.canSpawn || other == null || !base.enabled)
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (!componentInParent)
			{
				return;
			}
			if (this.ownerPlayer != null && componentInParent.creator == this.ownerPlayer)
			{
				return;
			}
			Vector3 a = (this.spawnerRigidbody != null) ? this.spawnerRigidbody.linearVelocity : Vector3.zero;
			Vector3 b = Time.fixedDeltaTime * 2f * a;
			Vector3 vector = b.normalized;
			if (vector == Vector3.zero)
			{
				vector = base.transform.forward;
				b = vector * 0.01f;
			}
			Vector3 vector2 = base.transform.position - b;
			Vector3 a2;
			if (this.alignToHitNormal)
			{
				float magnitude = b.magnitude;
				RaycastHit raycastHit;
				if (other.Raycast(new Ray(vector2, vector), out raycastHit, 2f * magnitude))
				{
					a2 = raycastHit.point;
				}
				else
				{
					a2 = other.ClosestPoint(vector2);
				}
			}
			else
			{
				a2 = other.ClosestPoint(vector2);
			}
			Vector3 vector3 = this.GetSpawnPosition(this.spawnLocation, componentInParent).TransformPoint(this.positionOffset);
			if ((a2 - vector3).magnitude <= this.stickRadius * componentInParent.scaleFactor)
			{
				if (NetworkSystem.Instance.LocalPlayer == componentInParent.creator)
				{
					if (this.firstPersonView && this.spawnLocation == StickObjectToPlayer.SpawnLocation.Head)
					{
						this.StickFirstPersonView();
					}
				}
				else
				{
					if (!this.thirdPersonView)
					{
						return;
					}
					Transform parent = this.MakeOrGetStickyContainer(componentInParent.transform);
					this.StickTo(parent, vector3, this.localEulerAngles);
				}
				UnityEvent onStickShared = this.OnStickShared;
				if (onStickShared == null)
				{
					return;
				}
				onStickShared.Invoke();
			}
		}

		// Token: 0x060074DD RID: 29917 RVA: 0x002644E8 File Offset: 0x002626E8
		private void StickFirstPersonView()
		{
			Transform cosmeticsHeadTarget = GTPlayer.Instance.CosmeticsHeadTarget;
			Vector3 position = cosmeticsHeadTarget.TransformPoint(this.FPVOffset);
			Transform parent = this.MakeOrGetStickyContainer(cosmeticsHeadTarget);
			this.StickTo(parent, position, this.FPVlocalEulerAngles);
		}

		// Token: 0x060074DE RID: 29918 RVA: 0x00264524 File Offset: 0x00262724
		private void StickTo(Transform parent, Vector3 position, Vector3 eulerAngle)
		{
			int num = 0;
			for (int i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i).gameObject.activeInHierarchy)
				{
					num++;
				}
			}
			if (num >= this.maxActiveStickies)
			{
				return;
			}
			this.stickyObject = ObjectPools.instance.Instantiate(this.objectToSpawn, true);
			if (this.stickyObject == null)
			{
				return;
			}
			this.stickyObject.transform.SetParent(parent, false);
			this.stickyObject.transform.position = position;
			this.stickyObject.transform.localEulerAngles = eulerAngle;
			this.lastSpawnedTime = Time.time;
			this.canSpawn = false;
		}

		// Token: 0x060074DF RID: 29919 RVA: 0x002645D4 File Offset: 0x002627D4
		private Transform GetSpawnPosition(StickObjectToPlayer.SpawnLocation spawnType, VRRig hitRig)
		{
			switch (spawnType)
			{
			case StickObjectToPlayer.SpawnLocation.Head:
				return hitRig.head.rigTarget.transform;
			case StickObjectToPlayer.SpawnLocation.RightHand:
				return hitRig.rightHand.rigTarget.transform;
			case StickObjectToPlayer.SpawnLocation.LeftHand:
				return hitRig.leftHand.rigTarget.transform;
			default:
				return null;
			}
		}

		// Token: 0x060074E0 RID: 29920 RVA: 0x0026462C File Offset: 0x0026282C
		public void Debug_StickToLocalPlayer()
		{
			Vector3 position = this.GetSpawnPosition(this.spawnLocation, VRRig.LocalRig).TransformPoint(this.positionOffset);
			this.StickTo(VRRig.LocalRig.transform, position, this.localEulerAngles);
		}

		// Token: 0x060074E1 RID: 29921 RVA: 0x0026466D File Offset: 0x0026286D
		public void Debug_StickToLocalPlayerFPV()
		{
			this.StickFirstPersonView();
		}

		// Token: 0x0400864A RID: 34378
		[Header("Shared Settings")]
		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject objectToSpawn;

		// Token: 0x0400864B RID: 34379
		[Tooltip("Optional: how many objects can be active at once")]
		[SerializeField]
		private int maxActiveStickies = 1;

		// Token: 0x0400864C RID: 34380
		[SerializeField]
		private StickObjectToPlayer.SpawnLocation spawnLocation;

		// Token: 0x0400864D RID: 34381
		[SerializeField]
		private float stickRadius = 0.5f;

		// Token: 0x0400864E RID: 34382
		[SerializeField]
		private bool alignToHitNormal = true;

		// Token: 0x0400864F RID: 34383
		[SerializeField]
		private Rigidbody spawnerRigidbody;

		// Token: 0x04008650 RID: 34384
		[SerializeField]
		private string parentTag = "GorillaHead";

		// Token: 0x04008651 RID: 34385
		[SerializeField]
		private float cooldown;

		// Token: 0x04008652 RID: 34386
		[Header("Third Person View")]
		[Tooltip("If you are only interested in the FPV, don't check this box so that others don't see it.")]
		[SerializeField]
		private bool thirdPersonView = true;

		// Token: 0x04008653 RID: 34387
		[SerializeField]
		private Vector3 positionOffset = new Vector3(0f, 0.02f, 0.17f);

		// Token: 0x04008654 RID: 34388
		[Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
		[SerializeField]
		private Vector3 localEulerAngles = Vector3.zero;

		// Token: 0x04008655 RID: 34389
		[Header("First Person View")]
		[SerializeField]
		private bool firstPersonView;

		// Token: 0x04008656 RID: 34390
		[SerializeField]
		private Vector3 FPVOffset = new Vector3(0f, 0.02f, 0.17f);

		// Token: 0x04008657 RID: 34391
		[Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
		[SerializeField]
		private Vector3 FPVlocalEulerAngles = Vector3.zero;

		// Token: 0x04008658 RID: 34392
		[Header("Events")]
		public UnityEvent OnStickShared;

		// Token: 0x04008659 RID: 34393
		private GameObject stickyObject;

		// Token: 0x0400865A RID: 34394
		private float lastSpawnedTime;

		// Token: 0x0400865B RID: 34395
		private bool canSpawn = true;

		// Token: 0x0400865C RID: 34396
		private NetPlayer ownerPlayer;

		// Token: 0x0200123D RID: 4669
		private enum SpawnLocation
		{
			// Token: 0x0400865F RID: 34399
			Head,
			// Token: 0x04008660 RID: 34400
			RightHand,
			// Token: 0x04008661 RID: 34401
			LeftHand
		}
	}
}
