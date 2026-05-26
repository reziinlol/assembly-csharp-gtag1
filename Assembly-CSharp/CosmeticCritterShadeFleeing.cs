using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class CosmeticCritterShadeFleeing : CosmeticCritter
{
	// Token: 0x060004D7 RID: 1239 RVA: 0x0001AEBB File Offset: 0x000190BB
	public override void OnSpawn()
	{
		this.spawnFX.Play();
		this.spawnAudioSource.clip = this.spawnAudioClips.GetRandomItem<AudioClip>();
		this.spawnAudioSource.GTPlay();
		this.pullVector = Vector3.zero;
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0001AEF4 File Offset: 0x000190F4
	public void SetFleePosition(Vector3 position, Vector3 fleeFrom)
	{
		this.origin = position;
		Vector3 vector = position - fleeFrom;
		this.fleeForward = vector.normalized;
		this.fleeRight = Vector3.Cross(this.fleeForward, Vector3.up);
		this.fleeUp = Vector3.Cross(this.fleeForward, this.fleeRight);
		this.trailingPosition = position + vector.normalized * 3f;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0001AF68 File Offset: 0x00019168
	public override void SetRandomVariables()
	{
		float num = 0f;
		for (int i = 0; i < this.modelSwaps.Length; i++)
		{
			num += this.modelSwaps[i].relativeProbability;
			this.modelSwaps[i].gameObject.SetActive(false);
		}
		float num2 = Random.value * num;
		for (int j = 0; j < this.modelSwaps.Length; j++)
		{
			if (num2 < this.modelSwaps[j].relativeProbability)
			{
				this.modelSwaps[j].gameObject.SetActive(true);
				break;
			}
			num2 -= this.modelSwaps[j].relativeProbability;
		}
		this.fleeBobFrequencyXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.x, Random.Range(-1f, 1f) * this.fleeBobFrequencyXYMax.y);
		this.fleeBobMagnitudeXY = new Vector2(Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.x, Random.Range(-1f, 1f) * this.fleeBobMagnitudeXYMax.y);
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0001B088 File Offset: 0x00019288
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		Vector3 vector = this.origin + num * this.fleeForward + this.pullVector + Mathf.Sin(this.fleeBobFrequencyXY.x * num) * this.fleeBobMagnitudeXY.x * this.fleeRight + Mathf.Sin(this.fleeBobFrequencyXY.y * num) * this.fleeBobMagnitudeXY.y * this.fleeUp;
		Quaternion rotation = Quaternion.LookRotation((vector - this.trailingPosition).normalized, Vector3.up);
		this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector, 0.05f);
		base.transform.SetPositionAndRotation(vector, rotation);
		this.animator.SetFloat(this.animatorProperty, Mathf.Sin(num * 3f) * 0.5f + 0.5f);
	}

	// Token: 0x04000558 RID: 1368
	[Tooltip("Randomly selects one of these models when spawned, accounting for relative probabilities. For example, if one model has a probability of 1 and another a probability of 2, the second is twice as likely to be picked (and thus will be picked 67% of the time).")]
	[SerializeField]
	private CosmeticCritterShadeFleeing.ModelSwap[] modelSwaps;

	// Token: 0x04000559 RID: 1369
	[Space]
	[Tooltip("Despawn the Shade after it has fled (fleed?) this many meters.")]
	[SerializeField]
	private float fleeDistanceToDespawn = 10f;

	// Token: 0x0400055A RID: 1370
	[Tooltip("Flee away from the spotter at this many meters per second.")]
	[SerializeField]
	private float fleeSpeed;

	// Token: 0x0400055B RID: 1371
	[Tooltip("The maximum strength the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobMagnitudeXYMax;

	// Token: 0x0400055C RID: 1372
	[Tooltip("The maximum frequency the shade can move bob around in the horizontal and vertical axes, with final value chosen randomly.")]
	[SerializeField]
	private Vector2 fleeBobFrequencyXYMax;

	// Token: 0x0400055D RID: 1373
	[SerializeField]
	private Animator animator;

	// Token: 0x0400055E RID: 1374
	[SerializeField]
	private ParticleSystem spawnFX;

	// Token: 0x0400055F RID: 1375
	[SerializeField]
	private AudioSource spawnAudioSource;

	// Token: 0x04000560 RID: 1376
	[SerializeField]
	private AudioClip[] spawnAudioClips;

	// Token: 0x04000561 RID: 1377
	[HideInInspector]
	public Vector3 pullVector;

	// Token: 0x04000562 RID: 1378
	private Vector3 origin;

	// Token: 0x04000563 RID: 1379
	private Vector3 fleeForward;

	// Token: 0x04000564 RID: 1380
	private Vector3 fleeRight;

	// Token: 0x04000565 RID: 1381
	private Vector3 fleeUp = Vector3.up;

	// Token: 0x04000566 RID: 1382
	private Vector2 fleeBobFrequencyXY;

	// Token: 0x04000567 RID: 1383
	private Vector2 fleeBobMagnitudeXY;

	// Token: 0x04000568 RID: 1384
	private Vector3 trailingPosition;

	// Token: 0x04000569 RID: 1385
	private float closestCatcherDistance;

	// Token: 0x0400056A RID: 1386
	private int animatorProperty = Animator.StringToHash("Distance");

	// Token: 0x020000C7 RID: 199
	[Serializable]
	private class ModelSwap
	{
		// Token: 0x0400056B RID: 1387
		public float relativeProbability;

		// Token: 0x0400056C RID: 1388
		public GameObject gameObject;
	}
}
