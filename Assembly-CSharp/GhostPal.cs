using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class GhostPal : MonoBehaviour
{
	// Token: 0x060004F5 RID: 1269 RVA: 0x0001B7BC File Offset: 0x000199BC
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.trailingPosition = base.transform.position;
		this.triggerAudioClipIndex = this.triggerAudioClips.GetRandomIndex<AudioClip>();
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x0001B7F8 File Offset: 0x000199F8
	private IEnumerator BounceOnTrigger()
	{
		float startTime = Time.time;
		while (Time.time - startTime < this.bounceOnTrigger[this.bounceOnTrigger.length - 1].time)
		{
			this.bounceHeight = this.bounceOnTrigger.Evaluate(Time.time - startTime);
			yield return null;
		}
		this.bounceHeight = 0f;
		yield break;
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x0001B808 File Offset: 0x00019A08
	private void LateUpdate()
	{
		Vector3 position = this.rig.bodyTransform.position;
		Vector3 vector = base.transform.parent.position - position;
		float num = vector.y * 0.5f + this.orbitHeight;
		vector.y = 0f;
		float d = vector.magnitude + this.minDistanceFromPlayer;
		vector = vector.normalized * d;
		vector.y = num + this.bounceHeight;
		double num2 = (double)this.orbitSpeed * (PhotonNetwork.InRoom ? ((PhotonNetwork.Time - (double)this.rig.OwningNetPlayer.UserId.GetStaticHash()) * (double)((this.rig.OwningNetPlayer.ActorNumber % 2 == 0) ? 1 : -1)) : Time.timeAsDouble);
		Vector3 b = new Vector3(this.orbitRadius * (float)Math.Cos(num2), 0f, this.orbitRadius * (float)Math.Sin(num2));
		Vector3 vector2 = position + vector + b;
		Vector3 a = vector2 - this.rig.head.rigTarget.position;
		if (Vector3.Dot(this.rig.head.rigTarget.forward, a.normalized) >= this.lookAtDotProductMin)
		{
			this.lookAtTime = Mathf.Min(this.lookAtTime + Time.deltaTime, Mathf.Max(this.rotateTowardsPlayerFromLookTime[this.rotateTowardsPlayerFromLookTime.length - 1].time, this.minLookTimeToTrigger));
			if (this.lookAtTime >= this.minLookTimeToTrigger && !this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.friendlyAnimID);
				this.bounceCoroutine = base.StartCoroutine(this.BounceOnTrigger());
				this.triggerAudioSource.pitch = Random.Range(this.triggerAudioPitchMinMax.x, this.triggerAudioPitchMinMax.y);
				this.triggerAudioSource.clip = this.triggerAudioClips[this.triggerAudioClipIndex];
				this.triggerAudioSource.GTPlay();
				this.triggerAudioClipIndex = (this.triggerAudioClipIndex + Random.Range(0, this.triggerAudioClips.Length - 1)) % this.triggerAudioClips.Length;
				this.hasTriggered = true;
			}
		}
		else
		{
			this.lookAtTime = Mathf.Max(this.lookAtTime - Time.deltaTime, 0f);
			if (this.lookAtTime < this.minLookTimeToTrigger && this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.neutralAnimID);
				this.hasTriggered = false;
			}
		}
		if ((vector2 - this.trailingPosition).sqrMagnitude > 0.1f)
		{
			float t = 1f - Mathf.Exp(-this.faceMovementDirectionStrength * Time.deltaTime);
			this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector2, t);
		}
		Quaternion rotation = Quaternion.Slerp(Quaternion.LookRotation(vector2 - this.trailingPosition, Vector3.up), Quaternion.LookRotation(-a, Vector3.up), this.rotateTowardsPlayerFromLookTime.Evaluate(this.lookAtTime));
		base.transform.SetPositionAndRotation(vector2, rotation);
	}

	// Token: 0x0400059F RID: 1439
	[SerializeField]
	private float minDistanceFromPlayer = 1f;

	// Token: 0x040005A0 RID: 1440
	[SerializeField]
	private float orbitRadius = 1f;

	// Token: 0x040005A1 RID: 1441
	[SerializeField]
	private float orbitHeight = 1f;

	// Token: 0x040005A2 RID: 1442
	[SerializeField]
	private float orbitSpeed = 0.1f;

	// Token: 0x040005A3 RID: 1443
	[SerializeField]
	private float faceMovementDirectionStrength = 1f;

	// Token: 0x040005A4 RID: 1444
	[Space]
	[SerializeField]
	private float lookAtDotProductMin = 0.95f;

	// Token: 0x040005A5 RID: 1445
	[SerializeField]
	private AnimationCurve rotateTowardsPlayerFromLookTime;

	// Token: 0x040005A6 RID: 1446
	[SerializeField]
	private float minLookTimeToTrigger = 2f;

	// Token: 0x040005A7 RID: 1447
	[SerializeField]
	private AnimationCurve bounceOnTrigger;

	// Token: 0x040005A8 RID: 1448
	[SerializeField]
	private AudioSource triggerAudioSource;

	// Token: 0x040005A9 RID: 1449
	[SerializeField]
	private Vector2 triggerAudioPitchMinMax = new Vector2(0.9f, 1.1f);

	// Token: 0x040005AA RID: 1450
	[SerializeField]
	private AudioClip[] triggerAudioClips;

	// Token: 0x040005AB RID: 1451
	private VRRig rig;

	// Token: 0x040005AC RID: 1452
	private Animator animator;

	// Token: 0x040005AD RID: 1453
	private float lookAtTime;

	// Token: 0x040005AE RID: 1454
	private bool hasTriggered;

	// Token: 0x040005AF RID: 1455
	private Coroutine bounceCoroutine;

	// Token: 0x040005B0 RID: 1456
	private float bounceHeight;

	// Token: 0x040005B1 RID: 1457
	private Vector3 trailingPosition;

	// Token: 0x040005B2 RID: 1458
	private int triggerAudioClipIndex;

	// Token: 0x040005B3 RID: 1459
	private int neutralAnimID = Animator.StringToHash("Neutral");

	// Token: 0x040005B4 RID: 1460
	private int friendlyAnimID = Animator.StringToHash("Friendly");
}
