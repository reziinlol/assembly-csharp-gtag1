using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002FB RID: 763
public class TriggerOnJump : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06001372 RID: 4978 RVA: 0x00066900 File Offset: 0x00064B00
	private void OnEnable()
	{
		if (this.myRig.IsNull())
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		if (this._events == null && this.myRig != null && this.myRig.Creator != null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			this._events.Init(this.myRig.creator);
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnActivate;
		}
		bool flag = !PhotonNetwork.InRoom && this.myRig != null && this.myRig.isOfflineVRRig;
		RigContainer rigContainer;
		bool flag2 = PhotonNetwork.InRoom && this.myRig != null && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer != null && rigContainer.Rig != null && rigContainer.Rig == this.myRig;
		if (flag || flag2)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x00066A28 File Offset: 0x00064C28
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
		this.playerOnGround = false;
		this.jumpStartTime = 0f;
		this.lastActivationTime = 0f;
		this.waitingForGrounding = false;
		if (this._events != null)
		{
			this._events.Activate -= this.OnActivate;
			Object.Destroy(this._events);
			this._events = null;
		}
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x00066AA1 File Offset: 0x00064CA1
	private void OnActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "OnJumpActivate");
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		this.onJumping.Invoke();
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x00066ADC File Offset: 0x00064CDC
	public void Tick()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			bool flag = this.playerOnGround;
			this.playerOnGround = (instance.BodyOnGround || instance.IsHandTouching(true) || instance.IsHandTouching(false));
			float time = Time.time;
			if (this.playerOnGround)
			{
				this.waitingForGrounding = false;
			}
			if (!this.playerOnGround && flag)
			{
				this.jumpStartTime = time;
			}
			if (!this.playerOnGround && !this.waitingForGrounding && instance.RigidbodyVelocity.sqrMagnitude > this.minJumpStrength * this.minJumpStrength && instance.RigidbodyVelocity.y > this.minJumpVertical && time > this.jumpStartTime + this.minJumpTime)
			{
				this.waitingForGrounding = true;
				if (time > this.lastActivationTime + this.cooldownTime)
				{
					this.lastActivationTime = time;
					if (PhotonNetwork.InRoom)
					{
						this._events.Activate.RaiseAll(Array.Empty<object>());
						return;
					}
					this.onJumping.Invoke();
				}
			}
		}
	}

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x06001376 RID: 4982 RVA: 0x00066BE8 File Offset: 0x00064DE8
	// (set) Token: 0x06001377 RID: 4983 RVA: 0x00066BF0 File Offset: 0x00064DF0
	public bool TickRunning { get; set; }

	// Token: 0x040017D9 RID: 6105
	[SerializeField]
	private float minJumpStrength = 1f;

	// Token: 0x040017DA RID: 6106
	[SerializeField]
	private float minJumpVertical = 1f;

	// Token: 0x040017DB RID: 6107
	[SerializeField]
	private float cooldownTime = 1f;

	// Token: 0x040017DC RID: 6108
	[SerializeField]
	private UnityEvent onJumping;

	// Token: 0x040017DD RID: 6109
	private RubberDuckEvents _events;

	// Token: 0x040017DE RID: 6110
	private bool playerOnGround;

	// Token: 0x040017DF RID: 6111
	private float minJumpTime = 0.05f;

	// Token: 0x040017E0 RID: 6112
	private bool waitingForGrounding;

	// Token: 0x040017E1 RID: 6113
	private float jumpStartTime;

	// Token: 0x040017E2 RID: 6114
	private float lastActivationTime;

	// Token: 0x040017E3 RID: 6115
	private VRRig myRig;
}
