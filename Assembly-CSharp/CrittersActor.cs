using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000047 RID: 71
public class CrittersActor : MonoBehaviour
{
	// Token: 0x14000003 RID: 3
	// (add) Token: 0x0600012A RID: 298 RVA: 0x00006C98 File Offset: 0x00004E98
	// (remove) Token: 0x0600012B RID: 299 RVA: 0x00006CD0 File Offset: 0x00004ED0
	public event Action<CrittersActor> OnGrabbedChild;

	// Token: 0x0600012C RID: 300 RVA: 0x00006D08 File Offset: 0x00004F08
	public virtual void UpdateAverageSpeed()
	{
		this.averageSpeed[this.averageSpeedIndex] = (base.transform.position - this.lastPosition).magnitude;
		this.averageSpeedIndex++;
		this.averageSpeedIndex %= 6;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600012D RID: 301 RVA: 0x00006D6D File Offset: 0x00004F6D
	public float GetAverageSpeed
	{
		get
		{
			return (this.averageSpeed[0] + this.averageSpeed[1] + this.averageSpeed[2] + this.averageSpeed[3] + this.averageSpeed[4] + this.averageSpeed[5]) / 6f;
		}
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00006DAA File Offset: 0x00004FAA
	protected virtual void Awake()
	{
		this._isOnPlayerDefault = this.isOnPlayer;
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00006DB8 File Offset: 0x00004FB8
	public virtual void Initialize()
	{
		if (this.defaultParentTransform == null)
		{
			this.SetDefaultParent(base.transform.parent);
		}
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		if (this.rb == null)
		{
			Debug.LogError("I should have a rigidbody, but I don't!", base.gameObject);
		}
		this.wasEnabled = false;
		this.isEnabled = true;
		this.TogglePhysics(this.usesRB);
		if (!this.rb.isKinematic)
		{
			this.rb.linearVelocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
		}
		if (this.resetPhysicsOnSpawn)
		{
			this.rb.linearVelocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
			this.lastImpulseVelocity = Vector3.zero;
		}
		if (this.subObjectIndex >= 0 && this.subObjectIndex < this.subObjects.Length)
		{
			for (int i = 0; i < this.subObjects.Length; i++)
			{
				this.subObjects[i].SetActive(i == this.subObjectIndex);
			}
		}
		this.colliders = new Collider[50];
		if (this.preventDespawnUntilGrabbed)
		{
			this.isDespawnBlocked = true;
			this.despawnTime = 0.0;
		}
		else
		{
			this.isDespawnBlocked = false;
			this.despawnTime = (double)this.despawnDelay + (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		}
		this.rb.includeLayers = 0;
		this.rb.excludeLayers = CrittersManager.instance.containerLayer;
	}

	// Token: 0x06000130 RID: 304 RVA: 0x00006F56 File Offset: 0x00005156
	public virtual void OnEnable()
	{
		CrittersManager.RegisterActor(this);
		this.Initialize();
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00006F64 File Offset: 0x00005164
	public virtual void OnDisable()
	{
		this.CleanupActor();
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00006F6C File Offset: 0x0000516C
	public virtual string GetActorSubtype()
	{
		if (this.subObjectIndex >= 0 && this.subObjectIndex < this.subObjects.Length)
		{
			return this.subObjects[this.subObjectIndex].name;
		}
		return base.name;
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00006FA0 File Offset: 0x000051A0
	protected virtual void CleanupActor()
	{
		CrittersManager.DeregisterActor(this);
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
		}
		for (int i = 0; i < this.subObjects.Length; i++)
		{
			if (this.subObjects[i].activeSelf)
			{
				this.subObjects[i].transform.localRotation = Quaternion.identity;
				this.subObjects[i].transform.localPosition = Vector3.zero;
				this.subObjects[i].SetActive(false);
			}
		}
		this.ReleasedEvent.Invoke(this);
		this.ReleasedEvent.RemoveAllListeners();
		this.isEnabled = false;
		this.wasEnabled = true;
		this.isOnPlayer = this._isOnPlayerDefault;
		this.rigPlayerId = -1;
		this.rigIndex = -1;
		this.despawnTime = 0.0;
		this.isDespawnBlocked = false;
		this.rb.isKinematic = false;
		if (this.parentActorId >= 0)
		{
			this.AttemptRemoveStoredObjectCollider(this.parentActorId, false);
		}
		this.parentActorId = -1;
		this.parentActor = null;
		this.lastParentActorId = -1;
		this.isGrabDisabled = false;
		this.lastGrabbedPlayer = -1;
		this.lastImpulsePosition = Vector3.zero;
		this.lastImpulseVelocity = Vector3.zero;
		this.lastImpulseQuaternion = Quaternion.identity;
		this.lastImpulseTime = -1.0;
		this.localLastImpulse = -1.0;
		this.updatedSinceLastFrame = false;
		this.localCanStore = false;
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00007114 File Offset: 0x00005314
	public virtual bool ProcessLocal()
	{
		this.updatedSinceLastFrame |= (this.isEnabled != this.wasEnabled || this.parentActorId != this.lastParentActorId);
		this.lastParentActorId = this.parentActorId;
		this.wasEnabled = this.isEnabled;
		return this.updatedSinceLastFrame;
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00007170 File Offset: 0x00005370
	public virtual void ProcessRemote()
	{
		bool flag = this.forceUpdate;
		this.forceUpdate = false;
		if (base.gameObject.activeSelf != this.isEnabled)
		{
			base.gameObject.SetActive(this.isEnabled);
		}
		if (!this.isEnabled)
		{
			return;
		}
		bool flag2 = this.lastParentActorId == this.parentActorId || this.isOnPlayer || this.isSceneActor;
		bool flag3 = this.lastImpulseTime == this.localLastImpulse;
		if (flag2 && flag3 && !flag)
		{
			return;
		}
		if (!flag2)
		{
			if (this.lastParentActorId >= 0)
			{
				this.AttemptRemoveStoredObjectCollider(this.lastParentActorId, true);
			}
			this.lastParentActorId = this.parentActorId;
			if (this.parentActorId >= 0)
			{
				CrittersActor crittersActor;
				if (!CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
				{
					return;
				}
				this.parentActor = crittersActor.transform;
				base.transform.SetParent(this.parentActor, true);
				this.SetImpulse();
				if (crittersActor is CrittersBag)
				{
					((CrittersBag)crittersActor).AddStoredObjectCollider(this);
				}
				if (crittersActor.isOnPlayer)
				{
					this.lastGrabbedPlayer = crittersActor.rigPlayerId;
				}
				crittersActor.RemoteGrabbed(this);
				return;
			}
			else if (this.parentActorId == -1)
			{
				this.parentActor = null;
				this.SetTransformToDefaultParent(false);
				this.HandleRemoteReleased();
				this.SetImpulse();
				return;
			}
		}
		else
		{
			this.SetImpulse();
		}
	}

	// Token: 0x06000136 RID: 310 RVA: 0x000072BC File Offset: 0x000054BC
	public virtual void SetImpulse()
	{
		if (this.isOnPlayer || this.isSceneActor)
		{
			return;
		}
		this.localLastImpulse = this.lastImpulseTime;
		this.MoveActor(this.lastImpulsePosition, this.lastImpulseQuaternion, this.parentActorId >= 0, false, true);
		this.TogglePhysics(this.usesRB && this.parentActorId == -1);
		if (!this.rb.isKinematic)
		{
			this.rb.linearVelocity = this.lastImpulseVelocity;
			this.rb.angularVelocity = this.lastImpulseAngularVelocity;
		}
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00007350 File Offset: 0x00005550
	public virtual void TogglePhysics(bool enable)
	{
		if (enable)
		{
			this.rb.isKinematic = false;
			this.rb.interpolation = RigidbodyInterpolation.Interpolate;
			this.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			return;
		}
		this.rb.isKinematic = true;
		this.rb.interpolation = RigidbodyInterpolation.None;
		this.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
	}

	// Token: 0x06000138 RID: 312 RVA: 0x000073AC File Offset: 0x000055AC
	public void AddPlayerCrittersActorDataToList(ref List<object> objList)
	{
		objList.Add(this.actorId);
		objList.Add(this.isOnPlayer);
		objList.Add(this.rigPlayerId);
		objList.Add(this.rigIndex);
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00007404 File Offset: 0x00005604
	public virtual int AddActorDataToList(ref List<object> objList)
	{
		objList.Add(this.actorId);
		objList.Add(this.lastImpulseTime);
		objList.Add(this.lastImpulsePosition);
		objList.Add(this.lastImpulseVelocity);
		objList.Add(this.lastImpulseAngularVelocity);
		objList.Add(this.lastImpulseQuaternion);
		objList.Add(this.parentActorId);
		objList.Add(this.isEnabled);
		objList.Add(this.subObjectIndex);
		return this.BaseActorDataLength();
	}

	// Token: 0x0600013A RID: 314 RVA: 0x000074B9 File Offset: 0x000056B9
	public int BaseActorDataLength()
	{
		return 9;
	}

	// Token: 0x0600013B RID: 315 RVA: 0x000074B9 File Offset: 0x000056B9
	public virtual int TotalActorDataLength()
	{
		return 9;
	}

	// Token: 0x0600013C RID: 316 RVA: 0x000074C0 File Offset: 0x000056C0
	public virtual int UpdateFromRPC(object[] data, int startingIndex)
	{
		double value;
		if (!CrittersManager.ValidateDataType<double>(data[startingIndex + 1], out value))
		{
			return this.BaseActorDataLength();
		}
		Vector3 vector;
		if (!CrittersManager.ValidateDataType<Vector3>(data[startingIndex + 2], out vector))
		{
			return this.BaseActorDataLength();
		}
		Vector3 vector2;
		if (!CrittersManager.ValidateDataType<Vector3>(data[startingIndex + 3], out vector2))
		{
			return this.BaseActorDataLength();
		}
		Vector3 vector3;
		if (!CrittersManager.ValidateDataType<Vector3>(data[startingIndex + 4], out vector3))
		{
			return this.BaseActorDataLength();
		}
		Quaternion quaternion;
		if (!CrittersManager.ValidateDataType<Quaternion>(data[startingIndex + 5], out quaternion))
		{
			return this.BaseActorDataLength();
		}
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 6], out num))
		{
			return this.BaseActorDataLength();
		}
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex + 7], out flag))
		{
			return this.BaseActorDataLength();
		}
		int num2;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 8], out num2))
		{
			return this.BaseActorDataLength();
		}
		this.lastImpulseTime = value.GetFinite();
		ref this.lastImpulsePosition.SetValueSafe(vector);
		ref this.lastImpulseVelocity.SetValueSafe(vector2);
		ref this.lastImpulseAngularVelocity.SetValueSafe(vector3);
		ref this.lastImpulseQuaternion.SetValueSafe(quaternion);
		this.parentActorId = num;
		this.isEnabled = flag;
		this.subObjectIndex = num2;
		this.forceUpdate = true;
		if (this.isEnabled)
		{
			base.gameObject.SetActive(true);
		}
		for (int i = 0; i < this.subObjects.Length; i++)
		{
			this.subObjects[i].SetActive(i == this.subObjectIndex);
		}
		return this.BaseActorDataLength();
	}

	// Token: 0x0600013D RID: 317 RVA: 0x00007620 File Offset: 0x00005820
	public int UpdatePlayerCrittersActorFromRPC(object[] data, int startingIndex)
	{
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex + 1], out flag))
		{
			return 4;
		}
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 2], out num))
		{
			return 4;
		}
		int num2;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 3], out num2))
		{
			return 4;
		}
		this.isOnPlayer = flag;
		this.rigPlayerId = num;
		this.rigIndex = num2;
		if (this.rigPlayerId == -1 && CrittersManager.instance.guard.currentOwner != null)
		{
			this.rigPlayerId = CrittersManager.instance.guard.currentOwner.ActorNumber;
		}
		this.PlacePlayerCrittersActor();
		return 4;
	}

	// Token: 0x0600013E RID: 318 RVA: 0x000076B4 File Offset: 0x000058B4
	public virtual bool UpdateSpecificActor(PhotonStream stream)
	{
		double num;
		Vector3 vector;
		Vector3 vector2;
		Vector3 vector3;
		Quaternion quaternion;
		int num2;
		bool flag;
		int num3;
		if (!(CrittersManager.ValidateDataType<double>(stream.ReceiveNext(), out num) & CrittersManager.ValidateDataType<Vector3>(stream.ReceiveNext(), out vector) & CrittersManager.ValidateDataType<Vector3>(stream.ReceiveNext(), out vector2) & CrittersManager.ValidateDataType<Vector3>(stream.ReceiveNext(), out vector3) & CrittersManager.ValidateDataType<Quaternion>(stream.ReceiveNext(), out quaternion) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num2) & CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num3)))
		{
			return false;
		}
		float num4 = 10000f;
		if (vector.IsValid(num4))
		{
			ref this.lastImpulsePosition.SetValueSafe(vector);
		}
		num4 = 10000f;
		if (vector2.IsValid(num4))
		{
			ref this.lastImpulseVelocity.SetValueSafe(vector2);
		}
		if (quaternion.IsValid())
		{
			ref this.lastImpulseQuaternion.SetValueSafe(quaternion);
		}
		num4 = 10000f;
		if (vector3.IsValid(num4))
		{
			ref this.lastImpulseAngularVelocity.SetValueSafe(vector3);
		}
		if (num2 >= -1 && num2 < CrittersManager.instance.universalActorId)
		{
			this.parentActorId = num2;
		}
		if (num3 < this.subObjects.Length)
		{
			this.subObjectIndex = num3;
		}
		this.isEnabled = flag;
		this.lastImpulseTime = num;
		if (this.isEnabled != base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(this.isEnabled);
		}
		if (this.isEnabled && this.subObjectIndex >= 0)
		{
			this.subObjects[this.subObjectIndex].SetActive(true);
		}
		else if (!this.isEnabled && this.subObjectIndex >= 0)
		{
			this.subObjects[this.subObjectIndex].SetActive(false);
		}
		return true;
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00007858 File Offset: 0x00005A58
	public virtual void SendDataByCrittersActorType(PhotonStream stream)
	{
		stream.SendNext(this.actorId);
		stream.SendNext(this.lastImpulseTime);
		stream.SendNext(this.lastImpulsePosition);
		stream.SendNext(this.lastImpulseVelocity);
		stream.SendNext(this.lastImpulseAngularVelocity);
		stream.SendNext(this.lastImpulseQuaternion);
		stream.SendNext(this.parentActorId);
		stream.SendNext(this.isEnabled);
		stream.SendNext(this.subObjectIndex);
		this.updatedSinceLastFrame = false;
	}

	// Token: 0x06000140 RID: 320 RVA: 0x00007905 File Offset: 0x00005B05
	public virtual void OnHover(bool isLeft)
	{
		GorillaTagger.Instance.StartVibration(isLeft, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00007932 File Offset: 0x00005B32
	public virtual bool CanBeGrabbed(CrittersActor grabbedBy)
	{
		return !this.isGrabDisabled && this.grabbable;
	}

	// Token: 0x06000142 RID: 322 RVA: 0x00007944 File Offset: 0x00005B44
	public static CrittersActor GetRootActor(int actorId)
	{
		CrittersActor crittersActor;
		if (!CrittersManager.instance.actorById.TryGetValue(actorId, out crittersActor))
		{
			return null;
		}
		if (crittersActor.parentActorId > -1)
		{
			return CrittersActor.GetRootActor(crittersActor.parentActorId);
		}
		return crittersActor;
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00007980 File Offset: 0x00005B80
	public static CrittersActor GetParentActor(int actorId)
	{
		CrittersActor result;
		if (CrittersManager.instance.actorById.TryGetValue(actorId, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06000144 RID: 324 RVA: 0x000079A8 File Offset: 0x00005BA8
	public bool AllowGrabbingActor(CrittersActor grabbedBy)
	{
		if (this.parentActorId == -1)
		{
			return true;
		}
		if (grabbedBy.crittersActorType != CrittersActor.CrittersActorType.Grabber)
		{
			return true;
		}
		CrittersActor rootActor = CrittersActor.GetRootActor(grabbedBy.actorId);
		CrittersActor crittersActor;
		if (CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
		{
			if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.Bag)
			{
				if (!CrittersManager.instance.allowGrabbingFromBags)
				{
					CrittersActor rootActor2 = CrittersActor.GetRootActor(this.actorId);
					Debug.Log(string.Format("Grieffing - FromBag {0} == {1} || {2} == -1 || {3} == -1  - ", new object[]
					{
						rootActor2.rigPlayerId,
						rootActor.rigPlayerId,
						crittersActor.parentActorId,
						rootActor.rigPlayerId
					}) + string.Format(" {0}", rootActor2.rigPlayerId == rootActor.rigPlayerId || rootActor2.rigPlayerId == -1 || rootActor.rigPlayerId == -1));
					return rootActor2.rigPlayerId == rootActor.rigPlayerId || rootActor2.rigPlayerId == -1 || rootActor.rigPlayerId == -1;
				}
			}
			else if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.BodyAttachPoint)
			{
				if (!CrittersManager.instance.allowGrabbingEntireBag)
				{
					Debug.Log(string.Format("Grieffing - EntireBag {0} == {1} || {2} == -1 || {3} == -1  -  {4}", new object[]
					{
						crittersActor.rigPlayerId,
						rootActor.rigPlayerId,
						crittersActor.parentActorId,
						rootActor.rigPlayerId,
						crittersActor.rigPlayerId == rootActor.rigPlayerId || crittersActor.rigPlayerId == -1 || rootActor.rigPlayerId == -1
					}));
					return crittersActor.rigPlayerId == rootActor.rigPlayerId || crittersActor.rigPlayerId == -1 || rootActor.rigPlayerId == -1;
				}
			}
			else if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.Grabber && !CrittersManager.instance.allowGrabbingOutOfHands)
			{
				Debug.Log(string.Format("Grieffing - InHand {0} == {1} || {2} == -1 || {3} == -1  -  {4}", new object[]
				{
					crittersActor.rigPlayerId,
					rootActor.rigPlayerId,
					crittersActor.parentActorId,
					rootActor.rigPlayerId,
					crittersActor.rigPlayerId == rootActor.rigPlayerId || crittersActor.rigPlayerId == -1 || rootActor.rigPlayerId == -1
				}));
				return crittersActor.rigPlayerId == rootActor.rigPlayerId || crittersActor.rigPlayerId == -1 || rootActor.rigPlayerId == -1;
			}
		}
		return true;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00007C3C File Offset: 0x00005E3C
	public bool IsCurrentlyAttachedToBag()
	{
		CrittersActor crittersActor;
		return CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor) && crittersActor.crittersActorType == CrittersActor.CrittersActorType.Bag;
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00007C70 File Offset: 0x00005E70
	public void SetTransformToDefaultParent(bool resetOrigin = false)
	{
		if (this.IsNull())
		{
			return;
		}
		base.transform.SetParent(this.defaultParentTransform, true);
		if (resetOrigin)
		{
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x06000147 RID: 327 RVA: 0x00007CB0 File Offset: 0x00005EB0
	public void SetDefaultParent(Transform newDefaultParent)
	{
		this.defaultParentTransform = newDefaultParent;
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00007CB9 File Offset: 0x00005EB9
	protected virtual void RemoteGrabbed(CrittersActor actor)
	{
		Action<CrittersActor> onGrabbedChild = this.OnGrabbedChild;
		if (onGrabbedChild != null)
		{
			onGrabbedChild(actor);
		}
		actor.RemoteGrabbedBy(this);
	}

	// Token: 0x06000149 RID: 329 RVA: 0x00007CD4 File Offset: 0x00005ED4
	protected virtual void RemoteGrabbedBy(CrittersActor grabbingActor)
	{
		this.GlobalGrabbedBy(grabbingActor);
	}

	// Token: 0x0600014A RID: 330 RVA: 0x00007CE0 File Offset: 0x00005EE0
	public virtual void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		this.GlobalGrabbedBy(grabbingActor);
		if (this.parentActorId >= 0)
		{
			this.AttemptRemoveStoredObjectCollider(this.parentActorId, true);
		}
		this.isGrabDisabled = disableGrabbing;
		this.parentActorId = grabbingActor.actorId;
		if (grabbingActor.isOnPlayer)
		{
			this.lastGrabbedPlayer = grabbingActor.rigPlayerId;
		}
		base.transform.SetParent(grabbingActor.transform, true);
		if (localRotation.w == 0f && localRotation.x == 0f && localRotation.y == 0f && localRotation.z == 0f)
		{
			localRotation = Quaternion.identity;
		}
		if (positionOverride)
		{
			this.MoveActor(localOffset, localRotation, true, false, true);
		}
		this.UpdateImpulses(true, true);
		this.rb.isKinematic = true;
		this.rb.interpolation = RigidbodyInterpolation.None;
		this.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
		if (CrittersManager.instance.IsNotNull() && PhotonNetwork.InRoom && !CrittersManager.instance.LocalAuthority())
		{
			CrittersManager.instance.SendRPC("RemoteCrittersActorGrabbedby", CrittersManager.instance.guard.currentOwner, new object[]
			{
				this.actorId,
				grabbingActor.actorId,
				this.lastImpulseQuaternion,
				this.lastImpulsePosition,
				this.isGrabDisabled
			});
		}
		Action<CrittersActor> onGrabbedChild = grabbingActor.OnGrabbedChild;
		if (onGrabbedChild != null)
		{
			onGrabbedChild(this);
		}
		this.AttemptAddStoredObjectCollider(grabbingActor);
	}

	// Token: 0x0600014B RID: 331 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void GlobalGrabbedBy(CrittersActor grabbingActor)
	{
	}

	// Token: 0x0600014C RID: 332 RVA: 0x00007E67 File Offset: 0x00006067
	protected virtual void HandleRemoteReleased()
	{
		this.DisconnectJoint();
	}

	// Token: 0x0600014D RID: 333 RVA: 0x00007E70 File Offset: 0x00006070
	public virtual void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
	{
		if (this.parentActorId >= 0)
		{
			this.AttemptRemoveStoredObjectCollider(this.parentActorId, true);
		}
		this.isGrabDisabled = false;
		this.parentActorId = -1;
		if (this.equipmentStorable)
		{
			this.localCanStore = false;
		}
		this.DisconnectJoint();
		this.SetTransformToDefaultParent(false);
		if (rotation.w == 0f && rotation.x == 0f && rotation.y == 0f && rotation.z == 0f)
		{
			rotation = Quaternion.identity;
		}
		if (!keepWorldPosition)
		{
			if (position.sqrMagnitude > 1f)
			{
				this.MoveActor(position, rotation, false, false, true);
			}
			else
			{
				GTDev.Log<string>(string.Format("Release called for: {0}, but sent in suspicious position data: {1}", base.name, position), null);
			}
		}
		if (this.despawnWhenIdle)
		{
			if (this.preventDespawnUntilGrabbed)
			{
				this.isDespawnBlocked = false;
			}
			this.despawnTime = (double)this.despawnDelay + (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		}
		this.UpdateImpulses(false, false);
		this.SetImpulseVelocity(impulseVelocity, impulseAngularVelocity);
		this.TogglePhysics(this.usesRB);
		this.SetImpulse();
		if (CrittersManager.instance.IsNotNull() && PhotonNetwork.InRoom && !CrittersManager.instance.LocalAuthority())
		{
			CrittersManager.instance.SendRPC("RemoteCritterActorReleased", CrittersManager.instance.guard.currentOwner, new object[]
			{
				this.actorId,
				false,
				rotation,
				position,
				impulseVelocity,
				impulseAngularVelocity
			});
		}
		this.ReleasedEvent.Invoke(this);
		this.ReleasedEvent.RemoveAllListeners();
	}

	// Token: 0x0600014E RID: 334 RVA: 0x00008030 File Offset: 0x00006230
	public void PlacePlayerCrittersActor()
	{
		if (this.rigIndex == -1)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
			return;
		}
		RigContainer rigContainer;
		CrittersRigActorSetup crittersRigActorSetup;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(this.rigPlayerId), out rigContainer) || !CrittersManager.instance.rigSetupByRig.TryGetValue(rigContainer.Rig, out crittersRigActorSetup))
		{
			rigContainer != null;
			return;
		}
		if (this.rigPlayerId == NetworkSystem.Instance.LocalPlayer.ActorNumber && !CrittersManager.instance.rigSetupByRig.TryGetValue(GorillaTagger.Instance.offlineVRRig, out crittersRigActorSetup))
		{
			return;
		}
		if (this.rigIndex < 0 || this.rigIndex >= crittersRigActorSetup.rigActors.Length)
		{
			return;
		}
		base.gameObject.SetActive(true);
		base.transform.parent = crittersRigActorSetup.rigActors[this.rigIndex].location;
		this.MoveActor(Vector3.zero, Quaternion.identity, true, true, true);
		crittersRigActorSetup.rigActors[this.rigIndex] = new CrittersRigActorSetup.RigActor
		{
			actorSet = this,
			location = crittersRigActorSetup.rigActors[this.rigIndex].location,
			type = crittersRigActorSetup.rigActors[this.rigIndex].type,
			subIndex = crittersRigActorSetup.rigActors[this.rigIndex].subIndex
		};
	}

	// Token: 0x0600014F RID: 335 RVA: 0x000081AC File Offset: 0x000063AC
	public void MoveActor(Vector3 position, Quaternion rotation, bool local = false, bool updateImpulses = true, bool updateImpulseTime = true)
	{
		bool isKinematic = this.rb.isKinematic;
		this.TogglePhysics(false);
		if (local)
		{
			base.transform.localRotation = rotation;
			base.transform.localPosition = position;
			if (updateImpulses)
			{
				this.UpdateImpulses(true, updateImpulseTime);
			}
		}
		else
		{
			base.transform.rotation = rotation.normalized;
			base.transform.position = position;
			if (updateImpulses)
			{
				this.UpdateImpulses(false, updateImpulseTime);
			}
		}
		if (!isKinematic)
		{
			this.TogglePhysics(true);
		}
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000822C File Offset: 0x0000642C
	public void UpdateImpulses(bool local = false, bool updateTime = false)
	{
		if (local)
		{
			this.lastImpulsePosition = base.transform.localPosition;
			this.lastImpulseQuaternion = base.transform.localRotation;
		}
		else
		{
			this.lastImpulsePosition = base.transform.position;
			this.lastImpulseQuaternion = base.transform.rotation;
		}
		if (updateTime)
		{
			this.SetImpulseTime();
		}
	}

	// Token: 0x06000151 RID: 337 RVA: 0x0000828B File Offset: 0x0000648B
	public void UpdateImpulseVelocity()
	{
		if (this.rb)
		{
			this.lastImpulseVelocity = this.rb.linearVelocity;
			this.lastImpulseAngularVelocity = this.rb.angularVelocity;
		}
	}

	// Token: 0x06000152 RID: 338 RVA: 0x000082BC File Offset: 0x000064BC
	public virtual void CalculateFear(CrittersPawn critter, float multiplier)
	{
		critter.IncreaseFear(this.FearCurve.Evaluate(Vector3.Distance(critter.transform.position, base.transform.position) / this.maxRangeOfFearAttraction) * multiplier * this.FearAmount * Time.deltaTime, this);
	}

	// Token: 0x06000153 RID: 339 RVA: 0x0000830C File Offset: 0x0000650C
	public virtual void CalculateAttraction(CrittersPawn critter, float multiplier)
	{
		critter.IncreaseAttraction(this.AttractionCurve.Evaluate(Vector3.Distance(critter.transform.position, base.transform.position) / this.maxRangeOfFearAttraction) * multiplier * this.AttractionAmount * Time.deltaTime, this);
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000835C File Offset: 0x0000655C
	public void SetImpulseVelocity(Vector3 velocity, Vector3 angularVelocity)
	{
		this.lastImpulseVelocity = velocity;
		this.lastImpulseAngularVelocity = angularVelocity;
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000836C File Offset: 0x0000656C
	public void SetImpulseTime()
	{
		this.lastImpulseTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
	}

	// Token: 0x06000156 RID: 342 RVA: 0x00008388 File Offset: 0x00006588
	public virtual bool ShouldDespawn()
	{
		return this.despawnWhenIdle && this.parentActorId == -1 && !this.isDespawnBlocked && 0.0 < this.despawnTime && this.despawnTime <= (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
	}

	// Token: 0x06000157 RID: 343 RVA: 0x000083DE File Offset: 0x000065DE
	public void RemoveDespawnBlock()
	{
		if (this.despawnWhenIdle)
		{
			this.isDespawnBlocked = false;
			this.despawnTime = (double)this.despawnDelay + (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		}
	}

	// Token: 0x06000158 RID: 344 RVA: 0x00008414 File Offset: 0x00006614
	public virtual bool CheckStorable()
	{
		if (!this.localCanStore)
		{
			return false;
		}
		Vector3 b = this.storeCollider.transform.up * MathF.Max(0f, this.storeCollider.height / 2f - this.storeCollider.radius);
		int num = Physics.OverlapCapsuleNonAlloc(this.storeCollider.transform.position + b, this.storeCollider.transform.position - b, this.storeCollider.radius, this.colliders, CrittersManager.instance.containerLayer, QueryTriggerInteraction.Collide);
		bool flag = false;
		CrittersBag crittersBag = null;
		bool flag2 = true;
		CrittersActor crittersActor = null;
		if (this.lastGrabbedPlayer == PhotonNetwork.LocalPlayer.ActorNumber && CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor) && crittersActor.GetAverageSpeed > CrittersManager.instance.MaxAttachSpeed)
		{
			return false;
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				CrittersActor component = this.colliders[i].attachedRigidbody.GetComponent<CrittersActor>();
				if (!(component == null) && !(component == this))
				{
					CrittersBag crittersBag2 = component as CrittersBag;
					if (!(crittersBag2 == null))
					{
						if (crittersBag2 == this.lastStoredObject)
						{
							flag = true;
							flag2 = false;
							break;
						}
						if (crittersBag2.IsActorValidStore(this))
						{
							if (crittersBag2.attachableCollider != this.colliders[i] && !this.colliders[i].isTrigger)
							{
								Vector3 vector;
								float num2;
								Physics.ComputePenetration(this.colliders[i], this.colliders[i].transform.position, this.colliders[i].transform.rotation, this.storeCollider, this.storeCollider.transform.position, this.storeCollider.transform.rotation, out vector, out num2);
								if (num2 >= CrittersManager.instance.overlapDistanceMax)
								{
									flag2 = false;
									break;
								}
							}
							else
							{
								crittersBag = crittersBag2;
							}
						}
					}
				}
			}
		}
		if (crittersBag.IsNotNull() && flag2)
		{
			if (crittersActor.IsNotNull())
			{
				CrittersGrabber crittersGrabber = crittersActor as CrittersGrabber;
				if (crittersGrabber.IsNotNull())
				{
					GorillaTagger.Instance.StartVibration(crittersGrabber.isLeft, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				}
			}
			this.GrabbedBy(crittersBag, false, default(Quaternion), default(Vector3), false);
			this.localCanStore = false;
			this.lastStoredObject = crittersBag;
			this.DisconnectJoint();
			return true;
		}
		if (!flag)
		{
			this.lastStoredObject = null;
		}
		return false;
	}

	// Token: 0x06000159 RID: 345 RVA: 0x000086CC File Offset: 0x000068CC
	public void SetJointRigid(Rigidbody rbToConnect)
	{
		if (this.joint != null)
		{
			return;
		}
		string str = "Critters SetJointRigid ";
		GameObject gameObject = base.gameObject;
		Debug.Log(str + ((gameObject != null) ? gameObject.ToString() : null));
		this.CreateJoint(rbToConnect, false);
		this.joint.xMotion = ConfigurableJointMotion.Locked;
		this.joint.yMotion = ConfigurableJointMotion.Locked;
		this.joint.zMotion = ConfigurableJointMotion.Locked;
		this.joint.angularXMotion = ConfigurableJointMotion.Locked;
		this.joint.angularYMotion = ConfigurableJointMotion.Locked;
		this.joint.angularZMotion = ConfigurableJointMotion.Locked;
		this.rb.mass = CrittersManager.instance.heavyMass;
		this.TogglePhysics(true);
	}

	// Token: 0x0600015A RID: 346 RVA: 0x00008778 File Offset: 0x00006978
	public void SetJointSoft(Rigidbody rbToConnect)
	{
		if (this.joint != null)
		{
			return;
		}
		string str = "Critters SetJointSoft ";
		GameObject gameObject = base.gameObject;
		Debug.Log(str + ((gameObject != null) ? gameObject.ToString() : null));
		this.CreateJoint(rbToConnect, true);
		this.joint.xMotion = ConfigurableJointMotion.Limited;
		this.joint.yMotion = ConfigurableJointMotion.Limited;
		this.joint.zMotion = ConfigurableJointMotion.Limited;
		this.joint.angularXMotion = ConfigurableJointMotion.Limited;
		this.joint.angularYMotion = ConfigurableJointMotion.Limited;
		this.joint.angularZMotion = ConfigurableJointMotion.Limited;
		this.rb.mass = CrittersManager.instance.lightMass;
		this.TogglePhysics(true);
	}

	// Token: 0x0600015B RID: 347 RVA: 0x00008824 File Offset: 0x00006A24
	private void CreateJoint(Rigidbody rbToConnect, bool setParentNull = true)
	{
		if (this.joint != null)
		{
			return;
		}
		this.joint = base.gameObject.AddComponent<ConfigurableJoint>();
		this.drive = new JointDrive
		{
			positionSpring = CrittersManager.instance.springForce,
			positionDamper = CrittersManager.instance.damperForce,
			maximumForce = 10000f
		};
		this.angularDrive = new JointDrive
		{
			positionSpring = CrittersManager.instance.springAngularForce,
			positionDamper = CrittersManager.instance.damperAngularForce,
			maximumForce = 10000f
		};
		this.linearLimitDrive = new SoftJointLimit
		{
			limit = CrittersManager.instance.springForce
		};
		this.linearLimitSpringDrive = new SoftJointLimitSpring
		{
			spring = CrittersManager.instance.springForce
		};
		this.joint.linearLimit = this.linearLimitDrive;
		this.joint.linearLimitSpring = this.linearLimitSpringDrive;
		this.joint.angularYLimit = this.joint.linearLimit;
		this.joint.angularZLimit = this.joint.linearLimit;
		this.joint.angularXDrive = this.angularDrive;
		this.joint.angularYZDrive = this.angularDrive;
		this.joint.xDrive = this.drive;
		this.joint.yDrive = this.drive;
		this.joint.zDrive = this.drive;
		this.joint.autoConfigureConnectedAnchor = true;
		this.joint.enableCollision = false;
		this.joint.connectedBody = rbToConnect;
		this.rb.excludeLayers = CrittersManager.instance.movementLayers;
		this.rb.useGravity = false;
		if (setParentNull)
		{
			base.transform.SetParent(null, true);
		}
	}

	// Token: 0x0600015C RID: 348 RVA: 0x00008A14 File Offset: 0x00006C14
	public void DisconnectJoint()
	{
		this.rb.excludeLayers = CrittersManager.instance.containerLayer;
		this.rb.useGravity = true;
		if (this.joint != null)
		{
			Object.Destroy(this.joint);
		}
		this.joint = null;
		if (this.parentActorId != -1)
		{
			CrittersActor crittersActor;
			CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor);
			base.transform.SetParent(crittersActor.transform, true);
			this.MoveActor(this.lastImpulsePosition, this.lastImpulseQuaternion, true, false, true);
			this.TogglePhysics(false);
		}
	}

	// Token: 0x0600015D RID: 349 RVA: 0x00008AB8 File Offset: 0x00006CB8
	public void AttemptRemoveStoredObjectCollider(int oldParentId, bool playSound = true)
	{
		CrittersActor crittersActor;
		if (CrittersManager.instance.actorById.TryGetValue(oldParentId, out crittersActor) && crittersActor is CrittersBag)
		{
			((CrittersBag)crittersActor).RemoveStoredObjectCollider(this, playSound);
		}
	}

	// Token: 0x0600015E RID: 350 RVA: 0x00008AF0 File Offset: 0x00006CF0
	public void AttemptAddStoredObjectCollider(CrittersActor actor)
	{
		if (actor is CrittersBag)
		{
			((CrittersBag)actor).AddStoredObjectCollider(this);
		}
	}

	// Token: 0x0600015F RID: 351 RVA: 0x00008B06 File Offset: 0x00006D06
	public bool AttemptSetEquipmentStorable()
	{
		if (!this.equipmentStorable)
		{
			return false;
		}
		this.localCanStore = true;
		return true;
	}

	// Token: 0x0400012D RID: 301
	public CrittersActor.CrittersActorType crittersActorType;

	// Token: 0x0400012E RID: 302
	public bool isSceneActor;

	// Token: 0x0400012F RID: 303
	public bool isOnPlayer;

	// Token: 0x04000130 RID: 304
	[NonSerialized]
	protected bool _isOnPlayerDefault;

	// Token: 0x04000131 RID: 305
	public int rigPlayerId;

	// Token: 0x04000132 RID: 306
	public int rigIndex;

	// Token: 0x04000133 RID: 307
	public bool grabbable;

	// Token: 0x04000134 RID: 308
	protected bool isGrabDisabled;

	// Token: 0x04000135 RID: 309
	public int lastGrabbedPlayer;

	// Token: 0x04000136 RID: 310
	public UnityEvent<CrittersActor> ReleasedEvent;

	// Token: 0x04000138 RID: 312
	public Rigidbody rb;

	// Token: 0x04000139 RID: 313
	[NonSerialized]
	public int actorId;

	// Token: 0x0400013A RID: 314
	[NonSerialized]
	protected Transform defaultParentTransform;

	// Token: 0x0400013B RID: 315
	[NonSerialized]
	public int parentActorId = -1;

	// Token: 0x0400013C RID: 316
	[NonSerialized]
	protected int lastParentActorId;

	// Token: 0x0400013D RID: 317
	[NonSerialized]
	public Vector3 lastImpulsePosition;

	// Token: 0x0400013E RID: 318
	[NonSerialized]
	public Vector3 lastImpulseVelocity;

	// Token: 0x0400013F RID: 319
	[NonSerialized]
	public Vector3 lastImpulseAngularVelocity;

	// Token: 0x04000140 RID: 320
	[NonSerialized]
	public Quaternion lastImpulseQuaternion;

	// Token: 0x04000141 RID: 321
	[NonSerialized]
	public double lastImpulseTime;

	// Token: 0x04000142 RID: 322
	[NonSerialized]
	public bool updatedSinceLastFrame;

	// Token: 0x04000143 RID: 323
	public bool isEnabled = true;

	// Token: 0x04000144 RID: 324
	public bool wasEnabled = true;

	// Token: 0x04000145 RID: 325
	[NonSerialized]
	protected double localLastImpulse;

	// Token: 0x04000146 RID: 326
	[NonSerialized]
	protected Transform parentActor;

	// Token: 0x04000147 RID: 327
	public GameObject[] subObjects;

	// Token: 0x04000148 RID: 328
	public int subObjectIndex = -1;

	// Token: 0x04000149 RID: 329
	public bool usesRB;

	// Token: 0x0400014A RID: 330
	public bool resetPhysicsOnSpawn;

	// Token: 0x0400014B RID: 331
	public bool despawnWhenIdle;

	// Token: 0x0400014C RID: 332
	public bool preventDespawnUntilGrabbed;

	// Token: 0x0400014D RID: 333
	public int despawnDelay;

	// Token: 0x0400014E RID: 334
	public double despawnTime;

	// Token: 0x0400014F RID: 335
	public bool isDespawnBlocked;

	// Token: 0x04000150 RID: 336
	public bool equipmentStorable;

	// Token: 0x04000151 RID: 337
	public bool localCanStore;

	// Token: 0x04000152 RID: 338
	public CrittersActor lastStoredObject;

	// Token: 0x04000153 RID: 339
	public CapsuleCollider storeCollider;

	// Token: 0x04000154 RID: 340
	[NonSerialized]
	public Collider[] colliders;

	// Token: 0x04000155 RID: 341
	[NonSerialized]
	public ConfigurableJoint joint;

	// Token: 0x04000156 RID: 342
	[NonSerialized]
	public float timeLastTouched;

	// Token: 0x04000157 RID: 343
	private JointDrive drive;

	// Token: 0x04000158 RID: 344
	private JointDrive angularDrive;

	// Token: 0x04000159 RID: 345
	private SoftJointLimit linearLimitDrive;

	// Token: 0x0400015A RID: 346
	private SoftJointLimitSpring linearLimitSpringDrive;

	// Token: 0x0400015B RID: 347
	public CapsuleCollider equipmentStoreTriggerCollider;

	// Token: 0x0400015C RID: 348
	public bool disconnectJointFlag;

	// Token: 0x0400015D RID: 349
	public bool forceUpdate;

	// Token: 0x0400015E RID: 350
	public float FearAmount = 1f;

	// Token: 0x0400015F RID: 351
	public AnimationCurve FearCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x04000160 RID: 352
	public float AttractionAmount = 1f;

	// Token: 0x04000161 RID: 353
	public AnimationCurve AttractionCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x04000162 RID: 354
	[FormerlySerializedAs("maxDetectionDistance")]
	public float maxRangeOfFearAttraction = 3f;

	// Token: 0x04000163 RID: 355
	protected float[] averageSpeed = new float[6];

	// Token: 0x04000164 RID: 356
	protected int averageSpeedIndex;

	// Token: 0x04000165 RID: 357
	private Vector3 lastPosition = Vector3.zero;

	// Token: 0x02000048 RID: 72
	public enum CrittersActorType
	{
		// Token: 0x04000167 RID: 359
		Creature,
		// Token: 0x04000168 RID: 360
		Food,
		// Token: 0x04000169 RID: 361
		LoudNoise,
		// Token: 0x0400016A RID: 362
		BrightLight,
		// Token: 0x0400016B RID: 363
		Darkness,
		// Token: 0x0400016C RID: 364
		HidingArea,
		// Token: 0x0400016D RID: 365
		Disappear,
		// Token: 0x0400016E RID: 366
		Spawn,
		// Token: 0x0400016F RID: 367
		Player,
		// Token: 0x04000170 RID: 368
		Grabber,
		// Token: 0x04000171 RID: 369
		Cage,
		// Token: 0x04000172 RID: 370
		FoodSpawner,
		// Token: 0x04000173 RID: 371
		AttachPoint,
		// Token: 0x04000174 RID: 372
		StunBomb,
		// Token: 0x04000175 RID: 373
		Bag,
		// Token: 0x04000176 RID: 374
		BodyAttachPoint,
		// Token: 0x04000177 RID: 375
		NoiseMaker,
		// Token: 0x04000178 RID: 376
		StickyTrap,
		// Token: 0x04000179 RID: 377
		StickyGoo
	}
}
