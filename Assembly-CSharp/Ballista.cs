using System;
using System.Collections;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001D3 RID: 467
public class Ballista : MonoBehaviourPun
{
	// Token: 0x06000C74 RID: 3188 RVA: 0x000440CA File Offset: 0x000422CA
	public void TriggerLoad()
	{
		this.animator.SetTrigger(this.loadTriggerHash);
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x000440DD File Offset: 0x000422DD
	public void TriggerFire()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000C76 RID: 3190 RVA: 0x000440F0 File Offset: 0x000422F0
	private float LaunchSpeed
	{
		get
		{
			if (!this.useSpeedOptions)
			{
				return this.launchSpeed;
			}
			return this.speedOptions[this.currentSpeedIndex];
		}
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00044110 File Offset: 0x00042310
	private void Awake()
	{
		this.launchDirection = this.launchEnd.position - this.launchStart.position;
		this.launchRampDistance = this.launchDirection.magnitude;
		this.launchDirection /= this.launchRampDistance;
		this.collidingLayer = LayerMask.NameToLayer("Default");
		this.notCollidingLayer = LayerMask.NameToLayer("Prop");
		this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
		this.animator.SetFloat(this.pitchParamHash, this.pitch);
		this.appliedAnimatorPitch = this.pitch;
		this.RefreshButtonColors();
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x000441C0 File Offset: 0x000423C0
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
		{
			if (this.prevStateHash == this.fireStateHash)
			{
				this.fireCompleteTime = Time.time;
			}
			if (Time.time - this.fireCompleteTime > this.reloadDelay)
			{
				this.animator.SetTrigger(this.loadTriggerHash);
				this.loadStartTime = Time.time;
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash)
		{
			if (Time.time - this.loadStartTime > this.loadTime)
			{
				if (this.playerInTrigger)
				{
					GTPlayer instance = GTPlayer.Instance;
					Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance);
					Vector3 b = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
					Vector3 b2 = playerBodyCenterPosition - b;
					Vector3 a = Vector3.Lerp(Vector3.zero, b2, Mathf.Exp(-this.playerPullInRate * deltaTime));
					instance.transform.position = instance.transform.position + (a - b2);
					this.playerReadyToFire = (a.sqrMagnitude < this.playerReadyToFireDist * this.playerReadyToFireDist);
				}
				else
				{
					this.playerReadyToFire = false;
				}
				if (this.playerReadyToFire)
				{
					if (PhotonNetwork.InRoom)
					{
						base.photonView.RPC("FireBallistaRPC", RpcTarget.Others, Array.Empty<object>());
					}
					this.FireLocal();
				}
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.fireStateHash && !this.playerLaunched && (this.playerReadyToFire || this.playerInTrigger))
		{
			float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			GTPlayer instance2 = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition2 = this.GetPlayerBodyCenterPosition(instance2);
			float b3 = Vector3.Dot(playerBodyCenterPosition2 - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			float num2 = 0.25f / this.launchRampDistance;
			float num3 = Mathf.Max(num + num2, b3);
			float d = num3 * this.launchRampDistance;
			Vector3 a2 = this.launchDirection * d + this.launchStart.position;
			instance2.transform.position + (a2 - playerBodyCenterPosition2);
			instance2.transform.position = instance2.transform.position + (a2 - playerBodyCenterPosition2);
			instance2.SetPlayerVelocity(Vector3.zero);
			if (num3 >= 1f)
			{
				this.playerLaunched = true;
				instance2.SetPlayerVelocity(this.LaunchSpeed * this.launchDirection);
				instance2.SetMaximumSlipThisFrame();
			}
		}
		this.prevStateHash = currentAnimatorStateInfo.shortNameHash;
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x000444B5 File Offset: 0x000426B5
	private void FireLocal()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
		this.playerLaunched = false;
		if (this.debugDrawTrajectoryOnLaunch)
		{
			this.DebugDrawTrajectory(8f);
		}
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x000444E4 File Offset: 0x000426E4
	private Vector3 GetPlayerBodyCenterPosition(GTPlayer player)
	{
		return player.headCollider.transform.position + Quaternion.Euler(0f, player.headCollider.transform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, -0.15f) + Vector3.down * 0.4f;
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00044560 File Offset: 0x00042760
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = true;
		}
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x00044594 File Offset: 0x00042794
	private void OnTriggerExit(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = false;
		}
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x000445C5 File Offset: 0x000427C5
	[PunRPC]
	public void FireBallistaRPC(PhotonMessageInfo info)
	{
		this.FireLocal();
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x000445D0 File Offset: 0x000427D0
	private void UpdatePredictionLine()
	{
		float d = 0.033333335f;
		Vector3 vector = this.launchEnd.position;
		Vector3 a = (this.launchEnd.position - this.launchStart.position).normalized * this.LaunchSpeed;
		for (int i = 0; i < 240; i++)
		{
			this.predictionLinePoints[i] = vector;
			vector += a * d;
			a += Vector3.down * 9.8f * d;
		}
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0004466A File Offset: 0x0004286A
	private IEnumerator DebugDrawTrajectory(float duration)
	{
		this.UpdatePredictionLine();
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
			DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00044680 File Offset: 0x00042880
	private void OnDrawGizmosSelected()
	{
		if (this.launchStart != null && this.launchEnd != null)
		{
			this.UpdatePredictionLine();
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.launchStart.position, this.launchEnd.position);
			Gizmos.DrawLineList(this.predictionLinePoints);
		}
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x000446E4 File Offset: 0x000428E4
	public void RefreshButtonColors()
	{
		this.speedZeroButton.isOn = (this.currentSpeedIndex == 0);
		this.speedZeroButton.UpdateColor();
		this.speedOneButton.isOn = (this.currentSpeedIndex == 1);
		this.speedOneButton.UpdateColor();
		this.speedTwoButton.isOn = (this.currentSpeedIndex == 2);
		this.speedTwoButton.UpdateColor();
		this.speedThreeButton.isOn = (this.currentSpeedIndex == 3);
		this.speedThreeButton.UpdateColor();
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0004476D File Offset: 0x0004296D
	public void SetSpeedIndex(int index)
	{
		this.currentSpeedIndex = index;
		this.RefreshButtonColors();
	}

	// Token: 0x04000F12 RID: 3858
	public Animator animator;

	// Token: 0x04000F13 RID: 3859
	public Transform launchStart;

	// Token: 0x04000F14 RID: 3860
	public Transform launchEnd;

	// Token: 0x04000F15 RID: 3861
	public Transform launchBone;

	// Token: 0x04000F16 RID: 3862
	public float reloadDelay = 1f;

	// Token: 0x04000F17 RID: 3863
	public float loadTime = 1.933f;

	// Token: 0x04000F18 RID: 3864
	public float playerMagnetismStrength = 3f;

	// Token: 0x04000F19 RID: 3865
	public float launchSpeed = 20f;

	// Token: 0x04000F1A RID: 3866
	[Range(0f, 1f)]
	public float pitch;

	// Token: 0x04000F1B RID: 3867
	private bool useSpeedOptions;

	// Token: 0x04000F1C RID: 3868
	public float[] speedOptions = new float[]
	{
		10f,
		15f,
		20f,
		25f
	};

	// Token: 0x04000F1D RID: 3869
	public int currentSpeedIndex;

	// Token: 0x04000F1E RID: 3870
	public GorillaPressableButton speedZeroButton;

	// Token: 0x04000F1F RID: 3871
	public GorillaPressableButton speedOneButton;

	// Token: 0x04000F20 RID: 3872
	public GorillaPressableButton speedTwoButton;

	// Token: 0x04000F21 RID: 3873
	public GorillaPressableButton speedThreeButton;

	// Token: 0x04000F22 RID: 3874
	private bool debugDrawTrajectoryOnLaunch;

	// Token: 0x04000F23 RID: 3875
	private int loadTriggerHash = Animator.StringToHash("Load");

	// Token: 0x04000F24 RID: 3876
	private int fireTriggerHash = Animator.StringToHash("Fire");

	// Token: 0x04000F25 RID: 3877
	private int pitchParamHash = Animator.StringToHash("Pitch");

	// Token: 0x04000F26 RID: 3878
	private int idleStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000F27 RID: 3879
	private int loadStateHash = Animator.StringToHash("Load");

	// Token: 0x04000F28 RID: 3880
	private int fireStateHash = Animator.StringToHash("Fire");

	// Token: 0x04000F29 RID: 3881
	private int prevStateHash = Animator.StringToHash("Idle");

	// Token: 0x04000F2A RID: 3882
	private float fireCompleteTime;

	// Token: 0x04000F2B RID: 3883
	private float loadStartTime;

	// Token: 0x04000F2C RID: 3884
	private bool playerInTrigger;

	// Token: 0x04000F2D RID: 3885
	private bool playerReadyToFire;

	// Token: 0x04000F2E RID: 3886
	private bool playerLaunched;

	// Token: 0x04000F2F RID: 3887
	private float playerReadyToFireDist = 0.1f;

	// Token: 0x04000F30 RID: 3888
	private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

	// Token: 0x04000F31 RID: 3889
	private Vector3 launchDirection;

	// Token: 0x04000F32 RID: 3890
	private float launchRampDistance;

	// Token: 0x04000F33 RID: 3891
	private int collidingLayer;

	// Token: 0x04000F34 RID: 3892
	private int notCollidingLayer;

	// Token: 0x04000F35 RID: 3893
	private float playerPullInRate;

	// Token: 0x04000F36 RID: 3894
	private float appliedAnimatorPitch;

	// Token: 0x04000F37 RID: 3895
	private const int predictionLineSamples = 240;

	// Token: 0x04000F38 RID: 3896
	private Vector3[] predictionLinePoints = new Vector3[240];
}
