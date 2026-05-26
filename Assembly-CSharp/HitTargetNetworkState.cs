using System;
using System.Collections;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004A9 RID: 1193
[NetworkBehaviourWeaved(1)]
public class HitTargetNetworkState : NetworkComponent
{
	// Token: 0x06001D04 RID: 7428 RVA: 0x0009D624 File Offset: 0x0009B824
	protected override void Awake()
	{
		base.Awake();
		this.audioPlayer = base.GetComponent<AudioSource>();
		SlingshotProjectileHitNotifier component = base.GetComponent<SlingshotProjectileHitNotifier>();
		if (component != null)
		{
			component.OnProjectileHit += this.ProjectileHitReciever;
			component.OnProjectileCollisionStay += this.ProjectileHitReciever;
			return;
		}
		Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject to increment score");
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x0009D682 File Offset: 0x0009B882
	protected override void Start()
	{
		base.Start();
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x0009D6A5 File Offset: 0x0009B8A5
	private void SetInitialState()
	{
		this.networkedScore.Value = 0;
		this.nextHittableTimestamp = 0f;
		this.audioPlayer.GTStop();
	}

	// Token: 0x06001D07 RID: 7431 RVA: 0x0009D6C9 File Offset: 0x0009B8C9
	public void OnLeftRoom()
	{
		this.SetInitialState();
	}

	// Token: 0x06001D08 RID: 7432 RVA: 0x0009D6D1 File Offset: 0x0009B8D1
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
		this.SetInitialState();
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x0009D6F9 File Offset: 0x0009B8F9
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				this.TargetHit(Vector3.zero, Vector3.one);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06001D0A RID: 7434 RVA: 0x0009D708 File Offset: 0x0009B908
	private void ProjectileHitReciever(SlingshotProjectile projectile, Collision collision)
	{
		this.TargetHit(projectile.launchPosition, collision.contacts[0].point);
	}

	// Token: 0x06001D0B RID: 7435 RVA: 0x0009D728 File Offset: 0x0009B928
	public void TargetHit(Vector3 launchPoint, Vector3 impactPoint)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (Time.time <= this.nextHittableTimestamp)
		{
			return;
		}
		int num = this.networkedScore.Value;
		if (this.scoreIsDistance)
		{
			int num2 = Mathf.RoundToInt((launchPoint - impactPoint).magnitude * 3.28f);
			if (num2 <= num)
			{
				return;
			}
			num = num2;
		}
		else
		{
			num++;
			if (num >= 1000)
			{
				num = 0;
			}
		}
		if (this.resetAfterDuration > 0f && this.resetCoroutine == null)
		{
			this.resetAtTimestamp = Time.time + this.resetAfterDuration;
			this.resetCoroutine = base.StartCoroutine(this.ResetCo());
		}
		this.PlayAudio(this.networkedScore.Value, num);
		this.networkedScore.Value = num;
		this.nextHittableTimestamp = Time.time + (float)this.hitCooldownTime;
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06001D0C RID: 7436 RVA: 0x0009D800 File Offset: 0x0009BA00
	// (set) Token: 0x06001D0D RID: 7437 RVA: 0x0009D826 File Offset: 0x0009BA26
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe int Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = value;
		}
	}

	// Token: 0x06001D0E RID: 7438 RVA: 0x0009D84D File Offset: 0x0009BA4D
	public override void WriteDataFusion()
	{
		this.Data = this.networkedScore.Value;
	}

	// Token: 0x06001D0F RID: 7439 RVA: 0x0009D860 File Offset: 0x0009BA60
	public override void ReadDataFusion()
	{
		int data = this.Data;
		if (data != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, data);
		}
		this.networkedScore.Value = data;
	}

	// Token: 0x06001D10 RID: 7440 RVA: 0x0009D8A0 File Offset: 0x0009BAA0
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.networkedScore.Value);
	}

	// Token: 0x06001D11 RID: 7441 RVA: 0x0009D8C8 File Offset: 0x0009BAC8
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		if (num != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, num);
		}
		this.networkedScore.Value = num;
	}

	// Token: 0x06001D12 RID: 7442 RVA: 0x0009D91B File Offset: 0x0009BB1B
	public void PlayAudio(int oldScore, int newScore)
	{
		if (oldScore > newScore && !this.scoreIsDistance)
		{
			this.audioPlayer.GTPlayOneShot(this.audioClips[1], 1f);
			return;
		}
		this.audioPlayer.GTPlayOneShot(this.audioClips[0], 1f);
	}

	// Token: 0x06001D13 RID: 7443 RVA: 0x0009D95A File Offset: 0x0009BB5A
	private IEnumerator ResetCo()
	{
		while (Time.time < this.resetAtTimestamp)
		{
			yield return new WaitForSeconds(this.resetAtTimestamp - Time.time);
		}
		this.networkedScore.Value = 0;
		this.PlayAudio(this.networkedScore.Value, 0);
		this.resetCoroutine = null;
		yield break;
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x0009D978 File Offset: 0x0009BB78
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x0009D990 File Offset: 0x0009BB90
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0400273B RID: 10043
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x0400273C RID: 10044
	[SerializeField]
	private int hitCooldownTime = 1;

	// Token: 0x0400273D RID: 10045
	[SerializeField]
	private bool testPress;

	// Token: 0x0400273E RID: 10046
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x0400273F RID: 10047
	[SerializeField]
	private bool scoreIsDistance;

	// Token: 0x04002740 RID: 10048
	[SerializeField]
	private float resetAfterDuration;

	// Token: 0x04002741 RID: 10049
	private AudioSource audioPlayer;

	// Token: 0x04002742 RID: 10050
	private float nextHittableTimestamp;

	// Token: 0x04002743 RID: 10051
	private float resetAtTimestamp;

	// Token: 0x04002744 RID: 10052
	private Coroutine resetCoroutine;

	// Token: 0x04002745 RID: 10053
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private int _Data;
}
