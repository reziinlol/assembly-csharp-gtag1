using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000DE2 RID: 3554
[RequireComponent(typeof(NetworkView))]
public class ThrowableSetDressing : TransferrableObject
{
	// Token: 0x1700082E RID: 2094
	// (get) Token: 0x06005702 RID: 22274 RVA: 0x001C2CBF File Offset: 0x001C0EBF
	// (set) Token: 0x06005703 RID: 22275 RVA: 0x001C2CC7 File Offset: 0x001C0EC7
	public bool inInitialPose { get; private set; } = true;

	// Token: 0x06005704 RID: 22276 RVA: 0x001C2CD0 File Offset: 0x001C0ED0
	public override bool ShouldBeKinematic()
	{
		return this.inInitialPose || base.ShouldBeKinematic();
	}

	// Token: 0x06005705 RID: 22277 RVA: 0x001C2CE2 File Offset: 0x001C0EE2
	protected override void Awake()
	{
		base.Awake();
		this.netView = base.GetComponent<NetworkView>();
	}

	// Token: 0x06005706 RID: 22278 RVA: 0x001C2CF6 File Offset: 0x001C0EF6
	protected override void Start()
	{
		base.Start();
		this.respawnAtPos = base.transform.position;
		this.respawnAtRot = base.transform.rotation;
		this.currentState = TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06005707 RID: 22279 RVA: 0x001C2D2B File Offset: 0x001C0F2B
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.inInitialPose = false;
		this.StopRespawnTimer();
	}

	// Token: 0x06005708 RID: 22280 RVA: 0x001C2D42 File Offset: 0x001C0F42
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.StartRespawnTimer(-1f);
		return true;
	}

	// Token: 0x06005709 RID: 22281 RVA: 0x001C2D5C File Offset: 0x001C0F5C
	public override void DropItem()
	{
		base.DropItem();
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x0600570A RID: 22282 RVA: 0x001C2D6F File Offset: 0x001C0F6F
	private void StopRespawnTimer()
	{
		if (this.respawnTimer != null)
		{
			base.StopCoroutine(this.respawnTimer);
			this.respawnTimer = null;
		}
	}

	// Token: 0x0600570B RID: 22283 RVA: 0x001C2D8C File Offset: 0x001C0F8C
	public void SetWillTeleport()
	{
		this.worldShareableInstance.SetWillTeleport();
	}

	// Token: 0x0600570C RID: 22284 RVA: 0x001C2D9C File Offset: 0x001C0F9C
	public void StartRespawnTimer(float overrideTimer = -1f)
	{
		float timerDuration = (overrideTimer != -1f) ? overrideTimer : this.respawnTimerDuration;
		this.StopRespawnTimer();
		if (this.respawnTimerDuration != 0f && (!this.netView.IsValid || this.netView.IsMine))
		{
			this.respawnTimer = base.StartCoroutine(this.RespawnTimerCoroutine(timerDuration));
		}
	}

	// Token: 0x0600570D RID: 22285 RVA: 0x001C2DFB File Offset: 0x001C0FFB
	private IEnumerator RespawnTimerCoroutine(float timerDuration)
	{
		yield return new WaitForSeconds(timerDuration);
		if (base.InHand())
		{
			yield break;
		}
		this.SetWillTeleport();
		base.transform.position = this.respawnAtPos;
		base.transform.rotation = this.respawnAtRot;
		this.inInitialPose = true;
		this.rigidbodyInstance.isKinematic = true;
		yield break;
	}

	// Token: 0x040066FA RID: 26362
	public float respawnTimerDuration;

	// Token: 0x040066FC RID: 26364
	[Tooltip("set this only if this set dressing is using as an ingredient for the magic cauldron - Halloween")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x040066FD RID: 26365
	private float _respawnTimestamp;

	// Token: 0x040066FE RID: 26366
	[SerializeField]
	private CapsuleCollider capsuleCollider;

	// Token: 0x040066FF RID: 26367
	private NetworkView netView;

	// Token: 0x04006700 RID: 26368
	private Vector3 respawnAtPos;

	// Token: 0x04006701 RID: 26369
	private Quaternion respawnAtRot;

	// Token: 0x04006702 RID: 26370
	private Coroutine respawnTimer;
}
