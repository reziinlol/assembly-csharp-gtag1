using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000595 RID: 1429
public class Bubbler : TransferrableObject
{
	// Token: 0x06002432 RID: 9266 RVA: 0x000C2604 File Offset: 0x000C0804
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasParticleSystem = (this.bubbleParticleSystem != null);
		if (this.hasParticleSystem)
		{
			this.bubbleParticleArray = new ParticleSystem.Particle[this.bubbleParticleSystem.main.maxParticles];
			this.bubbleParticleSystem.trigger.SetCollider(0, GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<SphereCollider>());
			this.bubbleParticleSystem.trigger.SetCollider(1, GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<SphereCollider>());
		}
		this.initialTriggerDuration = 0.05f;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06002433 RID: 9267 RVA: 0x000C26A8 File Offset: 0x000C08A8
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasBubblerAudio = (this.bubblerAudio != null && this.bubblerAudio.clip != null);
		this.hasPopBubbleAudio = (this.popBubbleAudio != null && this.popBubbleAudio.clip != null);
		this.hasFan = (this.fan != null);
		this.hasActiveOnlyComponent = (this.gameObjectActiveOnlyWhileTriggerDown != null);
	}

	// Token: 0x06002434 RID: 9268 RVA: 0x000C2738 File Offset: 0x000C0938
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
	}

	// Token: 0x06002435 RID: 9269 RVA: 0x000C278C File Offset: 0x000C098C
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.GTStop();
		}
		this.currentParticles.Clear();
		this.particleInfoDict.Clear();
	}

	// Token: 0x06002436 RID: 9270 RVA: 0x000C27FC File Offset: 0x000C09FC
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x000C280A File Offset: 0x000C0A0A
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this._worksInWater && GTPlayer.Instance.InWater)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x000C2830 File Offset: 0x000C0A30
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		bool forLeftController = this.currentState == TransferrableObject.PositionState.InLeftHand;
		bool enabled = this.itemState != TransferrableObject.ItemStates.State0;
		Behaviour[] array = this.behavioursToEnableWhenTriggerPressed;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = enabled;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
			{
				this.bubbleParticleSystem.Stop();
			}
			if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTStop();
			}
			if (this.hasActiveOnlyComponent)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(false);
			}
		}
		else
		{
			if (this.hasParticleSystem && !this.bubbleParticleSystem.isEmitting)
			{
				this.bubbleParticleSystem.Play();
			}
			if (this.hasBubblerAudio && !this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.GTPlay();
			}
			if (this.hasActiveOnlyComponent && !this.gameObjectActiveOnlyWhileTriggerDown.activeSelf)
			{
				this.gameObjectActiveOnlyWhileTriggerDown.SetActive(true);
			}
			if (this.IsMyItem())
			{
				this.initialTriggerPull = Time.time;
				GorillaTagger.Instance.StartVibration(forLeftController, this.triggerStrength, this.initialTriggerDuration);
				if (Time.time > this.initialTriggerPull + this.initialTriggerDuration)
				{
					GorillaTagger.Instance.StartVibration(forLeftController, this.ongoingStrength, Time.deltaTime);
				}
			}
			if (this.hasFan)
			{
				if (!this.fanYaxisinstead)
				{
					float z = this.fan.transform.localEulerAngles.z + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, 0f, z);
				}
				else
				{
					float y = this.fan.transform.localEulerAngles.y + this.rotationSpeed * Time.fixedDeltaTime;
					this.fan.transform.localEulerAngles = new Vector3(0f, y, 0f);
				}
			}
		}
		if (this.hasParticleSystem && (!this.allBubblesPopped || this.itemState == TransferrableObject.ItemStates.State1))
		{
			int particles = this.bubbleParticleSystem.GetParticles(this.bubbleParticleArray);
			this.allBubblesPopped = (particles <= 0);
			if (!this.allBubblesPopped)
			{
				for (int j = 0; j < particles; j++)
				{
					if (this.currentParticles.Contains(this.bubbleParticleArray[j].randomSeed))
					{
						this.currentParticles.Remove(this.bubbleParticleArray[j].randomSeed);
					}
				}
				foreach (uint key in this.currentParticles)
				{
					if (this.particleInfoDict.TryGetValue(key, out this.outPosition))
					{
						if (this.hasPopBubbleAudio)
						{
							GTAudioSourceExtensions.GTPlayClipAtPoint(this.popBubbleAudio.clip, this.outPosition);
						}
						this.particleInfoDict.Remove(key);
					}
				}
				this.currentParticles.Clear();
				for (int k = 0; k < particles; k++)
				{
					if (this.particleInfoDict.TryGetValue(this.bubbleParticleArray[k].randomSeed, out this.outPosition))
					{
						this.particleInfoDict[this.bubbleParticleArray[k].randomSeed] = this.bubbleParticleArray[k].position;
					}
					else
					{
						this.particleInfoDict.Add(this.bubbleParticleArray[k].randomSeed, this.bubbleParticleArray[k].position);
					}
					this.currentParticles.Add(this.bubbleParticleArray[k].randomSeed);
				}
			}
		}
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x000C2C3C File Offset: 0x000C0E3C
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x0600243A RID: 9274 RVA: 0x0004C4BC File Offset: 0x0004A6BC
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x000C2C4B File Offset: 0x000C0E4B
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x000C2C56 File Offset: 0x000C0E56
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04002F85 RID: 12165
	[SerializeField]
	private bool _worksInWater = true;

	// Token: 0x04002F86 RID: 12166
	public ParticleSystem bubbleParticleSystem;

	// Token: 0x04002F87 RID: 12167
	private ParticleSystem.Particle[] bubbleParticleArray;

	// Token: 0x04002F88 RID: 12168
	public AudioSource bubblerAudio;

	// Token: 0x04002F89 RID: 12169
	public AudioSource popBubbleAudio;

	// Token: 0x04002F8A RID: 12170
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x04002F8B RID: 12171
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x04002F8C RID: 12172
	private Vector3 outPosition;

	// Token: 0x04002F8D RID: 12173
	private bool allBubblesPopped;

	// Token: 0x04002F8E RID: 12174
	public bool disableActivation;

	// Token: 0x04002F8F RID: 12175
	public bool disableDeactivation;

	// Token: 0x04002F90 RID: 12176
	public float rotationSpeed = 5f;

	// Token: 0x04002F91 RID: 12177
	public GameObject fan;

	// Token: 0x04002F92 RID: 12178
	public bool fanYaxisinstead;

	// Token: 0x04002F93 RID: 12179
	public float ongoingStrength = 0.005f;

	// Token: 0x04002F94 RID: 12180
	public float triggerStrength = 0.2f;

	// Token: 0x04002F95 RID: 12181
	private float initialTriggerPull;

	// Token: 0x04002F96 RID: 12182
	private float initialTriggerDuration;

	// Token: 0x04002F97 RID: 12183
	private bool hasBubblerAudio;

	// Token: 0x04002F98 RID: 12184
	private bool hasPopBubbleAudio;

	// Token: 0x04002F99 RID: 12185
	public GameObject gameObjectActiveOnlyWhileTriggerDown;

	// Token: 0x04002F9A RID: 12186
	public Behaviour[] behavioursToEnableWhenTriggerPressed;

	// Token: 0x04002F9B RID: 12187
	private bool hasParticleSystem;

	// Token: 0x04002F9C RID: 12188
	private bool hasFan;

	// Token: 0x04002F9D RID: 12189
	private bool hasActiveOnlyComponent;

	// Token: 0x02000596 RID: 1430
	private enum BubblerState
	{
		// Token: 0x04002F9F RID: 12191
		None = 1,
		// Token: 0x04002FA0 RID: 12192
		Bubbling
	}
}
