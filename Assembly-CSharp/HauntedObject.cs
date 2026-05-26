using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020008E0 RID: 2272
public class HauntedObject : MonoBehaviour
{
	// Token: 0x06003B71 RID: 15217 RVA: 0x00145BA0 File Offset: 0x00143DA0
	private void Awake()
	{
		this.lurkerGhost = GameObject.FindGameObjectWithTag("LurkerGhost");
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.wanderingGhost = GameObject.FindGameObjectWithTag("WanderingGhost");
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.animators = base.transform.GetComponentsInChildren<Animator>();
	}

	// Token: 0x06003B72 RID: 15218 RVA: 0x00145C5C File Offset: 0x00143E5C
	private void OnDestroy()
	{
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
	}

	// Token: 0x06003B73 RID: 15219 RVA: 0x00145CE7 File Offset: 0x00143EE7
	private void Start()
	{
		this.initialPos = base.transform.position;
		this.passedTime = 0f;
		this.lightPassedTime = 0f;
	}

	// Token: 0x06003B74 RID: 15220 RVA: 0x00145D10 File Offset: 0x00143F10
	private void TriggerEffects(GameObject go)
	{
		if (base.gameObject != go)
		{
			return;
		}
		if (this.rattle)
		{
			base.StartCoroutine(this.Shake());
		}
		if (this.audioSource && this.hauntedSound)
		{
			this.audioSource.GTPlayOneShot(this.hauntedSound, 1f);
		}
		if (this.FBXprefab)
		{
			ObjectPools.instance.Instantiate(this.FBXprefab, base.transform.position, true);
		}
		if (this.TurnOffLight != null)
		{
			base.StartCoroutine(this.TurnOff());
		}
		foreach (Animator animator in this.animators)
		{
			if (animator)
			{
				animator.SetTrigger(HauntedObject._animHaunted);
			}
		}
	}

	// Token: 0x06003B75 RID: 15221 RVA: 0x00145DE2 File Offset: 0x00143FE2
	private IEnumerator Shake()
	{
		while (this.passedTime < this.duration)
		{
			this.passedTime += Time.deltaTime;
			base.transform.position = new Vector3(this.initialPos.x + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.y + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.z);
			yield return null;
		}
		this.passedTime = 0f;
		yield break;
	}

	// Token: 0x06003B76 RID: 15222 RVA: 0x00145DF1 File Offset: 0x00143FF1
	private IEnumerator TurnOff()
	{
		this.TurnOffLight.gameObject.SetActive(false);
		while (this.lightPassedTime < this.TurnOffDuration)
		{
			this.lightPassedTime += Time.deltaTime;
			yield return null;
		}
		this.TurnOffLight.SetActive(true);
		this.lightPassedTime = 0f;
		yield break;
	}

	// Token: 0x04004BF6 RID: 19446
	private static readonly int _animHaunted = Animator.StringToHash("Haunted");

	// Token: 0x04004BF7 RID: 19447
	private const string _lurkerGhost = "LurkerGhost";

	// Token: 0x04004BF8 RID: 19448
	private const string _wanderingGhost = "WanderingGhost";

	// Token: 0x04004BF9 RID: 19449
	[Tooltip("If this box is checked, then object will rattle when hunted")]
	public bool rattle;

	// Token: 0x04004BFA RID: 19450
	public float speed = 60f;

	// Token: 0x04004BFB RID: 19451
	public float amount = 0.01f;

	// Token: 0x04004BFC RID: 19452
	public float duration = 1f;

	// Token: 0x04004BFD RID: 19453
	[FormerlySerializedAs("FBX")]
	public GameObject FBXprefab;

	// Token: 0x04004BFE RID: 19454
	[Tooltip("Use to turn off a game object like candle flames when hunted")]
	public GameObject TurnOffLight;

	// Token: 0x04004BFF RID: 19455
	public float TurnOffDuration = 2f;

	// Token: 0x04004C00 RID: 19456
	private Vector3 initialPos;

	// Token: 0x04004C01 RID: 19457
	private float passedTime;

	// Token: 0x04004C02 RID: 19458
	private float lightPassedTime;

	// Token: 0x04004C03 RID: 19459
	private GameObject lurkerGhost;

	// Token: 0x04004C04 RID: 19460
	private GameObject wanderingGhost;

	// Token: 0x04004C05 RID: 19461
	private Animator[] animators;

	// Token: 0x04004C06 RID: 19462
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004C07 RID: 19463
	[FormerlySerializedAs("rattlingSound")]
	public AudioClip hauntedSound;
}
