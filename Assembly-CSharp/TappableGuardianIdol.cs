using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020009E4 RID: 2532
[DisallowMultipleComponent]
public class TappableGuardianIdol : Tappable
{
	// Token: 0x17000600 RID: 1536
	// (get) Token: 0x060040D8 RID: 16600 RVA: 0x0015ADE9 File Offset: 0x00158FE9
	// (set) Token: 0x060040D9 RID: 16601 RVA: 0x0015ADF1 File Offset: 0x00158FF1
	public bool isChangingPositions { get; private set; }

	// Token: 0x060040DA RID: 16602 RVA: 0x0015ADFA File Offset: 0x00158FFA
	protected override void OnEnable()
	{
		base.OnEnable();
		this._colliderBaseRadius = this.tapCollision.radius;
	}

	// Token: 0x060040DB RID: 16603 RVA: 0x0015AE13 File Offset: 0x00159013
	protected override void OnDisable()
	{
		base.OnDisable();
		this.isChangingPositions = false;
		this._activationState = -1;
		this.isActivationReady = true;
		this.tapCollision.radius = this._colliderBaseRadius;
	}

	// Token: 0x060040DC RID: 16604 RVA: 0x0015AE41 File Offset: 0x00159041
	public void OnZoneActiveStateChanged(bool zoneActive)
	{
		this._zoneIsActive = zoneActive;
		this.idolVisualRoot.SetActive(this._zoneIsActive);
	}

	// Token: 0x060040DD RID: 16605 RVA: 0x0015AE5C File Offset: 0x0015905C
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (info.Sender.IsLocal)
		{
			this.zoneManager.SetScaleCenterPoint(base.transform);
		}
		if (!this.isChangingPositions)
		{
			if (!this.zoneManager.IsZoneValid())
			{
				return;
			}
			RigContainer rigContainer;
			if (PhotonNetwork.LocalPlayer.IsMasterClient && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				if (Vector3.Magnitude(rigContainer.Rig.transform.position - base.transform.position) > this.requiredTapDistance + Mathf.Epsilon)
				{
					return;
				}
				this.zoneManager.IdolWasTapped(info.Sender);
			}
			if (!this.zoneManager.IsPlayerGuardian(info.Sender))
			{
				this.tapFX.Play();
			}
		}
	}

	// Token: 0x060040DE RID: 16606 RVA: 0x0015AF24 File Offset: 0x00159124
	public void SetPosition(Vector3 position)
	{
		base.transform.position = position + new Vector3(0f, this.activeHeight, 0f);
		this.UpdateStageActivatedObjects();
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		base.StartCoroutine(this.<SetPosition>g__Unshrink|49_0());
	}

	// Token: 0x060040DF RID: 16607 RVA: 0x0015AF86 File Offset: 0x00159186
	public void MovePositions(Vector3 finalPosition)
	{
		if (this.isChangingPositions)
		{
			return;
		}
		this.transitionPos = finalPosition + this.fallStartOffset;
		this.finalPos = finalPosition;
		base.StartCoroutine(this.TransitionToNextIdol());
	}

	// Token: 0x060040E0 RID: 16608 RVA: 0x0015AFB8 File Offset: 0x001591B8
	public void UpdateActivationProgress(float rawProgress, bool progressing)
	{
		this.isActivationReady = !progressing;
		if (rawProgress <= 0f && !progressing)
		{
			if (this._activationState >= 0)
			{
				if (this._activationRoutine != null)
				{
					base.StopCoroutine(this._activationRoutine);
					this._activationRoutine = null;
				}
				this.idolMeshRoot.transform.localScale = Vector3.one;
			}
			this._activationState = -1;
			this.UpdateStageActivatedObjects();
			this._audio.GTStop();
			return;
		}
		int num = (int)rawProgress;
		progressing &= (this._activationStageSounds.Length > num);
		if (this._activationState == num || !progressing)
		{
			return;
		}
		if (this._activationRoutine != null)
		{
			base.StopCoroutine(this._activationRoutine);
		}
		this._activationRoutine = base.StartCoroutine(this.ShowActivationEffect());
		this._activationState = num;
		this.UpdateStageActivatedObjects();
		TappableGuardianIdol.IdolActivationSound idolActivationSound = this._activationStageSounds[num];
		this._audio.GTPlayOneShot(idolActivationSound.activation, this._audio.volume);
		this._audio.clip = idolActivationSound.loop;
		this._audio.loop = true;
		this._audio.GTPlay();
	}

	// Token: 0x060040E1 RID: 16609 RVA: 0x0015B0CF File Offset: 0x001592CF
	public void StartLookingAround()
	{
		if (this._lookRoutine != null)
		{
			base.StopCoroutine(this._lookRoutine);
		}
		this._lookRoutine = base.StartCoroutine(this.DoLookingAround());
	}

	// Token: 0x060040E2 RID: 16610 RVA: 0x0015B0F7 File Offset: 0x001592F7
	public void StopLookingAround()
	{
		if (this._lookRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this._lookRoutine);
		this._lookRoot.localRotation = Quaternion.identity;
		this._lookRoutine = null;
	}

	// Token: 0x060040E3 RID: 16611 RVA: 0x0015B125 File Offset: 0x00159325
	private IEnumerator DoLookingAround()
	{
		TappableGuardianIdol.<>c__DisplayClass54_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.nextLookTime = Time.time;
		CS$<>8__locals1._lookDirection = this._lookRoot.rotation;
		yield return null;
		for (;;)
		{
			if (Time.time >= CS$<>8__locals1.nextLookTime)
			{
				this.<DoLookingAround>g__PickLookTarget|54_0(ref CS$<>8__locals1);
			}
			this._lookRoot.rotation = Quaternion.Slerp(this._lookRoot.rotation, CS$<>8__locals1._lookDirection, Time.deltaTime * Mathf.Max(1f, (float)this._activationState * this._baseLookRate));
			yield return null;
		}
		yield break;
	}

	// Token: 0x060040E4 RID: 16612 RVA: 0x0015B134 File Offset: 0x00159334
	private void UpdateStageActivatedObjects()
	{
		foreach (TappableGuardianIdol.StageActivatedObject stageActivatedObject in this._stageActivatedObjects)
		{
			stageActivatedObject.UpdateActiveState(this._activationState);
		}
	}

	// Token: 0x060040E5 RID: 16613 RVA: 0x0015B16B File Offset: 0x0015936B
	private IEnumerator ShowActivationEffect()
	{
		float bulgeDuration = 1f;
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / bulgeDuration;
			float num = Mathf.Lerp(1f, this.bulgeScale, this.bulgeCurve.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		this._activationRoutine = null;
		yield break;
	}

	// Token: 0x060040E6 RID: 16614 RVA: 0x0015B17A File Offset: 0x0015937A
	private IEnumerator TransitionToNextIdol()
	{
		this.isChangingPositions = true;
		this._audio.GTStop();
		if (this.knockbackOnTrigger)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		if (this.explodeFX)
		{
			ObjectPools.instance.Instantiate(this.explodeFX, base.transform.position, true);
		}
		this.UpdateActivationProgress(-1f, false);
		this.idolMeshRoot.SetActive(false);
		this.tapCollision.enabled = false;
		base.transform.position = this.transitionPos;
		yield return new WaitForSeconds(this.floatDuration);
		this.idolMeshRoot.SetActive(true);
		this.tapCollision.enabled = true;
		if (this.startFallFX)
		{
			ObjectPools.instance.Instantiate(this.startFallFX, this.transitionPos, true);
		}
		this._audio.GTPlayOneShot(this._descentSound, 1f);
		this.trailFX.Play();
		float fall = 0f;
		Vector3 startPos = this.transitionPos;
		Vector3 destinationPos = this.finalPos;
		while (fall < this.fallDuration)
		{
			fall += Time.deltaTime;
			base.transform.position = Vector3.Lerp(startPos, destinationPos, fall / this.fallDuration);
			yield return null;
		}
		base.transform.position = destinationPos;
		this.trailFX.Stop();
		if (this.landedFX)
		{
			ObjectPools.instance.Instantiate(this.landedFX, destinationPos, true);
		}
		if (this.knockbackOnLand)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		yield return new WaitForSeconds(this.inactiveDuration);
		this._audio.GTPlayOneShot(this._activateSound, this._audio.volume);
		float activateLerp = 0f;
		startPos = this.finalPos;
		destinationPos = this.finalPos + new Vector3(0f, this.activeHeight, 0f);
		AnimationCurve animCurve = AnimationCurves.EaseInOutQuad;
		while (activateLerp < 1f)
		{
			activateLerp = Mathf.Clamp01(activateLerp + Time.deltaTime / this.activationDuration);
			base.transform.position = Vector3.Lerp(startPos, destinationPos, animCurve.Evaluate(activateLerp));
			yield return null;
		}
		if (this.activatedFX)
		{
			ObjectPools.instance.Instantiate(this.activatedFX, base.transform.position, true);
		}
		if (this.knockbackOnActivate)
		{
			this.zoneManager.TriggerIdolKnockback();
		}
		this.isChangingPositions = false;
		yield break;
	}

	// Token: 0x060040E7 RID: 16615 RVA: 0x0015B189 File Offset: 0x00159389
	private float EaseInOut(float input)
	{
		if (input >= 0.5f)
		{
			return 1f - Mathf.Pow(-2f * input + 2f, 3f) / 2f;
		}
		return 4f * input * input * input;
	}

	// Token: 0x060040E9 RID: 16617 RVA: 0x0015B2C0 File Offset: 0x001594C0
	[CompilerGenerated]
	private IEnumerator <SetPosition>g__Unshrink|49_0()
	{
		float lerpVal = 0f;
		float growDuration = 0.5f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / growDuration;
			float num = Mathf.Lerp(0f, 1f, AnimationCurves.EaseOutQuad.Evaluate(lerpVal));
			this.idolMeshRoot.transform.localScale = Vector3.one * num;
			this.tapCollision.radius = this._colliderBaseRadius * num;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060040EA RID: 16618 RVA: 0x0015B2D0 File Offset: 0x001594D0
	[CompilerGenerated]
	private void <DoLookingAround>g__PickLookTarget|54_0(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		Transform transform = this.<DoLookingAround>g__GetClosestPlayerPosition|54_2(ref A_1);
		A_1._lookDirection = (transform ? Quaternion.LookRotation(transform.position - this._lookRoot.position) : Quaternion.Euler((float)Random.Range(-15, 15), this._lookRoot.rotation.eulerAngles.y + (float)Random.Range(-45, 45), 0f));
		this.<DoLookingAround>g__SetLookTime|54_1(ref A_1);
	}

	// Token: 0x060040EB RID: 16619 RVA: 0x0015B34E File Offset: 0x0015954E
	[CompilerGenerated]
	private void <DoLookingAround>g__SetLookTime|54_1(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		A_1.nextLookTime = Time.time + this._lookInterval / (float)this._activationState * 0.5f + Random.value;
	}

	// Token: 0x060040EC RID: 16620 RVA: 0x0015B378 File Offset: 0x00159578
	[CompilerGenerated]
	private Transform <DoLookingAround>g__GetClosestPlayerPosition|54_2(ref TappableGuardianIdol.<>c__DisplayClass54_0 A_1)
	{
		if (Random.value < this._randomLookChance)
		{
			return null;
		}
		Vector3 position = base.transform.position;
		float num = float.MaxValue;
		Transform result = null;
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			if (!rigContainer.IsNull())
			{
				bool flag = rigContainer.Creator == this.zoneManager.CurrentGuardian;
				float num2 = Vector3.SqrMagnitude(rigContainer.transform.position - position) * (float)(flag ? 100 : 1);
				if (num2 < num)
				{
					num = num2;
					result = rigContainer.transform;
				}
			}
		}
		return result;
	}

	// Token: 0x0400516F RID: 20847
	[SerializeField]
	private GorillaGuardianZoneManager zoneManager;

	// Token: 0x04005170 RID: 20848
	[SerializeField]
	private float floatDuration = 2f;

	// Token: 0x04005171 RID: 20849
	[SerializeField]
	private float fallDuration = 1.5f;

	// Token: 0x04005172 RID: 20850
	[SerializeField]
	private float inactiveDuration = 2f;

	// Token: 0x04005173 RID: 20851
	[SerializeField]
	private float activationDuration = 1f;

	// Token: 0x04005174 RID: 20852
	[SerializeField]
	private float activeHeight = 1f;

	// Token: 0x04005175 RID: 20853
	[SerializeField]
	private bool knockbackOnTrigger;

	// Token: 0x04005176 RID: 20854
	[SerializeField]
	private bool knockbackOnLand = true;

	// Token: 0x04005177 RID: 20855
	[SerializeField]
	private bool knockbackOnActivate;

	// Token: 0x04005178 RID: 20856
	[SerializeField]
	private Vector3 fallStartOffset = new Vector3(3f, 20f, 3f);

	// Token: 0x04005179 RID: 20857
	[SerializeField]
	private ParticleSystem trailFX;

	// Token: 0x0400517A RID: 20858
	[SerializeField]
	private ParticleSystem tapFX;

	// Token: 0x0400517B RID: 20859
	[SerializeField]
	private GameObject explodeFX;

	// Token: 0x0400517C RID: 20860
	[SerializeField]
	private GameObject startFallFX;

	// Token: 0x0400517D RID: 20861
	[SerializeField]
	private GameObject landedFX;

	// Token: 0x0400517E RID: 20862
	[SerializeField]
	private GameObject activatedFX;

	// Token: 0x0400517F RID: 20863
	[SerializeField]
	private SphereCollider tapCollision;

	// Token: 0x04005180 RID: 20864
	[SerializeField]
	private GameObject idolVisualRoot;

	// Token: 0x04005181 RID: 20865
	[SerializeField]
	private GameObject idolMeshRoot;

	// Token: 0x04005182 RID: 20866
	[SerializeField]
	private AnimationCurve bulgeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04005183 RID: 20867
	[SerializeField]
	private float bulgeScale = 1.1f;

	// Token: 0x04005184 RID: 20868
	[SerializeField]
	private AudioSource _audio;

	// Token: 0x04005185 RID: 20869
	[SerializeField]
	private AudioClip[] _descentSound;

	// Token: 0x04005186 RID: 20870
	[SerializeField]
	private AudioClip[] _activateSound;

	// Token: 0x04005187 RID: 20871
	[SerializeField]
	private TappableGuardianIdol.IdolActivationSound[] _activationStageSounds;

	// Token: 0x04005188 RID: 20872
	[SerializeField]
	private TappableGuardianIdol.StageActivatedObject[] _stageActivatedObjects;

	// Token: 0x04005189 RID: 20873
	[Header("Look Around")]
	[SerializeField]
	private Transform _lookRoot;

	// Token: 0x0400518A RID: 20874
	[SerializeField]
	private float _lookInterval = 10f;

	// Token: 0x0400518B RID: 20875
	[SerializeField]
	private float _baseLookRate = 1f;

	// Token: 0x0400518C RID: 20876
	[SerializeField]
	private float _randomLookChance = 0.25f;

	// Token: 0x0400518D RID: 20877
	private Coroutine _lookRoutine;

	// Token: 0x0400518F RID: 20879
	private Vector3 transitionPos;

	// Token: 0x04005190 RID: 20880
	private Vector3 finalPos;

	// Token: 0x04005191 RID: 20881
	private int _activationState;

	// Token: 0x04005192 RID: 20882
	private Coroutine _activationRoutine;

	// Token: 0x04005193 RID: 20883
	private float _colliderBaseRadius;

	// Token: 0x04005194 RID: 20884
	private bool _zoneIsActive = true;

	// Token: 0x04005195 RID: 20885
	public bool isActivationReady;

	// Token: 0x04005196 RID: 20886
	private float requiredTapDistance = 3f;

	// Token: 0x020009E5 RID: 2533
	[Serializable]
	public struct IdolActivationSound
	{
		// Token: 0x04005197 RID: 20887
		public AudioClip activation;

		// Token: 0x04005198 RID: 20888
		public AudioClip loop;
	}

	// Token: 0x020009E6 RID: 2534
	[Serializable]
	public struct StageActivatedObject
	{
		// Token: 0x060040ED RID: 16621 RVA: 0x0015B438 File Offset: 0x00159638
		public void UpdateActiveState(int stage)
		{
			bool active = stage >= this.min && stage <= this.max;
			GameObject[] array = this.objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(active);
			}
		}

		// Token: 0x04005199 RID: 20889
		public GameObject[] objects;

		// Token: 0x0400519A RID: 20890
		public int min;

		// Token: 0x0400519B RID: 20891
		public int max;
	}
}
