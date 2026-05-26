using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000117 RID: 279
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetWristJet : SIGadget, I_SIDisruptable, IEnergyGadget
{
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060006FA RID: 1786 RVA: 0x00027B9C File Offset: 0x00025D9C
	private bool CanRecharge
	{
		get
		{
			return (!this.rechargeRequiresFloorTouch || this._floorTouched) && this.state == SIGadgetWristJet.State.Unactive;
		}
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00027BBC File Offset: 0x00025DBC
	private void Awake()
	{
		this._maxSqrHorizontalSpeed = this.maxHorizontalSpeed * this.maxHorizontalSpeed;
		this._hasThrustLoopAudioSource = (this.m_thrustLoopAudioSource != null);
		this.m_warnFuelLowThreshold = ((this.m_warnFuelLowSound != null) ? this.m_warnFuelLowThreshold : -1f);
		this._hasInactiveStateVisual = (this.inactiveStateVisual != null);
		this._hasActiveStateVisual = (this.activeStateVisual != null);
		this._gaugeMatPropBlock = new MaterialPropertyBlock();
		this._baseFuelSpendRate = this.fuelSpendRate;
		this._baseJetForce = this.jetForce;
		this._baseMaxVerticalSpeed = this.maxVerticalSpeed;
		this._baseMaxHorizontalSpeed = this.maxHorizontalSpeed;
		if (this.m_gaugeMatSlots == null)
		{
			this.m_gaugeMatSlots = Array.Empty<GTRendererMatSlot>();
		}
		int num = 0;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			if (this.m_gaugeMatSlots[i].TryInitialize())
			{
				this.m_gaugeMatSlots[num] = this.m_gaugeMatSlots[i];
				num++;
			}
		}
		if (num != this.m_gaugeMatSlots.Length)
		{
			Array.Resize<GTRendererMatSlot>(ref this.m_gaugeMatSlots, num);
		}
		this.throttleFlapInitialRots = ((this.m_throttleFlapXforms != null) ? new Quaternion[this.m_throttleFlapXforms.Length] : Array.Empty<Quaternion>());
		for (int j = 0; j < this.throttleFlapInitialRots.Length; j++)
		{
			if (this.m_throttleFlapXforms[j] == null)
			{
				this.throttleFlapInitialRots = Array.Empty<Quaternion>();
				Debug.LogError("[SIGadgetWristJet]  ERROR!!!  Awake: Throttle indicator flaps will not animate because entry is null in " + string.Format("array at `{0}[{1}]`. Path={2}", "m_throttleFlapXforms", j, base.transform.GetPathQ()), this);
				return;
			}
			this.throttleFlapInitialRots[j] = this.m_throttleFlapXforms[j].localRotation;
		}
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00027D78 File Offset: 0x00025F78
	private void Start()
	{
		this.gtPlayer = GTPlayer.Instance;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnReleased = (Action)Delegate.Combine(gameEntity.OnReleased, new Action(this.HandleStopInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Combine(gameEntity2.OnUnsnapped, new Action(this.HandleStopInteraction));
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00027DF5 File Offset: 0x00025FF5
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.m_warnFuelLowThreshold > 0f)
		{
			this.m_warnFuelLowSound.LoadAudioData();
		}
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00027E16 File Offset: 0x00026016
	protected override void OnDisable()
	{
		if (this.m_warnFuelLowThreshold > 0f && this.m_warnFuelLowSound.loadState != AudioDataLoadState.Unloaded)
		{
			this.m_warnFuelLowSound.UnloadAudioData();
		}
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00027E40 File Offset: 0x00026040
	protected override void Update()
	{
		base.Update();
		if (this._hasThrustLoopAudioSource)
		{
			float target = (this.state == SIGadgetWristJet.State.Active) ? this.m_thrustLoopSoundVolume : 0f;
			float num = (this.state == SIGadgetWristJet.State.Active) ? this.m_thrustLoopAudioFadeInTime : this.m_thrustLoopAudioFadeOutTime;
			this.m_thrustLoopAudioSource.volume = Mathf.MoveTowards(this.m_thrustLoopAudioSource.volume, target, 1f / num * Time.unscaledDeltaTime);
		}
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x00027EB4 File Offset: 0x000260B4
	private void FixedUpdate()
	{
		if (!this.IsEquippedLocal() && !this.activatedLocally)
		{
			return;
		}
		if (this.state == SIGadgetWristJet.State.Active && this.currentFuel > 0f && this.buttonActivatable.CheckInput(0.25f) && !base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			this.gtPlayer.AddForce(-Physics.gravity * (this.gtPlayer.scale * this.gravityNegationPercent), ForceMode.Acceleration);
			this._ApplyClampedThrust();
		}
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00027F36 File Offset: 0x00026136
	private void HandleStopInteraction()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		this.SetStateAuthority(SIGadgetWristJet.State.Unactive);
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00027F50 File Offset: 0x00026150
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		bool flag = this.buttonActivatable.CheckInput(0.25f);
		if (!this._floorTouched)
		{
			this._floorTouched = (this.gtPlayer.IsGroundedButt || this.gtPlayer.IsGroundedHand);
		}
		if (this._throttleControl)
		{
			Vector2 joystickInput = base.GetJoystickInput();
			if (Mathf.Abs(joystickInput.y) > 0.75f && Mathf.Abs(joystickInput.x) < 0.5f)
			{
				this._throttle = Mathf.Clamp01(this._throttle + joystickInput.y * this.throttleChangeSpeed * Time.deltaTime);
				this._currentBurnRate = Mathf.Lerp(this.minimumBurnRate, 1f, this._throttle);
				this.UpdateThrottleIndicator();
			}
		}
		switch (this.state)
		{
		case SIGadgetWristJet.State.Unactive:
			if (flag && !base.IsBlocked(SIExclusionType.AffectsLocalMovement))
			{
				this.SetStateAuthority(SIGadgetWristJet.State.Active);
			}
			break;
		case SIGadgetWristJet.State.Active:
			this.currentFuel = Mathf.Clamp(this.currentFuel - dt * this.fuelSpendRate * this._currentBurnRate, 0f, this.fuelSize);
			this._floorTouched = false;
			this.gtPlayer.ThrusterActiveAtFrame = Time.frameCount;
			if (flag && this.m_warnFuelLowThreshold > 0f)
			{
				float num = this.currentFuel / this.fuelSize;
				if (this._warnFuelLowSoundWasPlayed && num > this.m_warnFuelLowThreshold)
				{
					this._warnFuelLowSoundWasPlayed = false;
				}
				else if (!this._warnFuelLowSoundWasPlayed && num <= this.m_warnFuelLowThreshold)
				{
					this._warnFuelLowSoundWasPlayed = true;
					this.gameEntity.audioSource.GTPlayOneShot(this.m_warnFuelLowSound, this.m_warnFuelLowSoundVolume);
				}
			}
			if (!flag || this.currentFuel <= 0f)
			{
				this.SetStateAuthority(SIGadgetWristJet.State.OutOfFuel);
			}
			break;
		case SIGadgetWristJet.State.OutOfFuel:
			if (!flag)
			{
				this.emptiedCooldownResetProgress += dt;
			}
			else if (this.currentFuel > 0f)
			{
				this.SetStateAuthority(SIGadgetWristJet.State.Active);
			}
			if (this.emptiedCooldownResetProgress > this.emptiedCooldown)
			{
				this.emptiedCooldownResetProgress = 0f;
				this.SetStateAuthority(SIGadgetWristJet.State.Unactive);
			}
			break;
		}
		float value = this.currentFuel / this.fuelSize;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			this._gaugeMatPropBlock.SetFloat(ShaderProps._EmissionDissolveProgress, value);
			this.m_gaugeMatSlots[i].renderer.SetPropertyBlock(this._gaugeMatPropBlock, this.m_gaugeMatSlots[i].slot);
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x000281CC File Offset: 0x000263CC
	private void UpdateThrottleIndicator()
	{
		for (int i = 0; i < this.throttleFlapInitialRots.Length; i++)
		{
			Quaternion b = this.throttleFlapInitialRots[i] * this.m_throttleFlapMaxRotOffset;
			this.m_throttleFlapXforms[i].localRotation = Quaternion.Lerp(this.throttleFlapInitialRots[i], b, this._throttle);
		}
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x0002822C File Offset: 0x0002642C
	private void _ApplyClampedThrust()
	{
		Vector3 rigidbodyVelocity = this.gtPlayer.RigidbodyVelocity;
		float num = this.jetForce * this._currentBurnRate;
		Vector3 vector = rigidbodyVelocity + base.transform.forward * (num * Time.fixedDeltaTime);
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		if (vector2.sqrMagnitude > this._maxSqrHorizontalSpeed)
		{
			float magnitude = new Vector3(rigidbodyVelocity.x, 0f, rigidbodyVelocity.z).magnitude;
			vector2 = Vector3.ClampMagnitude(vector2, Mathf.Max(this.maxHorizontalSpeed, magnitude));
		}
		Vector3 a = vector2;
		a.y = ((vector.y > this.maxVerticalSpeed) ? Mathf.Max(this.maxVerticalSpeed, rigidbodyVelocity.y) : vector.y);
		this.gtPlayer.AddForce(a - rigidbodyVelocity, ForceMode.VelocityChange);
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00028314 File Offset: 0x00026514
	private void OnEntityStateChanged(long oldState, long newState)
	{
		SIGadgetWristJet.State state = (SIGadgetWristJet.State)oldState;
		SIGadgetWristJet.State state2 = (SIGadgetWristJet.State)newState;
		if (state != state2)
		{
			this.SetState(state2);
		}
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x00028330 File Offset: 0x00026530
	private void SetStateAuthority(SIGadgetWristJet.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00028354 File Offset: 0x00026554
	private void SetState(SIGadgetWristJet.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetWristJet.State.Unactive:
			if (this._hasInactiveStateVisual)
			{
				this.inactiveStateVisual.SetActive(true);
			}
			if (this._hasActiveStateVisual)
			{
				this.activeStateVisual.SetActive(false);
				return;
			}
			break;
		case SIGadgetWristJet.State.Active:
			if (this._hasInactiveStateVisual)
			{
				this.inactiveStateVisual.SetActive(false);
			}
			if (this._hasActiveStateVisual)
			{
				this.activeStateVisual.SetActive(true);
				return;
			}
			break;
		case SIGadgetWristJet.State.OutOfFuel:
			if (this._hasInactiveStateVisual)
			{
				this.inactiveStateVisual.SetActive(true);
			}
			if (this._hasActiveStateVisual)
			{
				this.activeStateVisual.SetActive(false);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x00028408 File Offset: 0x00026608
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._throttleControl = withUpgrades.Contains(SIUpgradeType.Thruster_Throttle_Control);
		if (this._throttleControl)
		{
			this.UpdateThrottleIndicator();
		}
		switch (this.jetType)
		{
		case SIGadgetWristJet.WristJetType.Jet:
			this.fuelSpendRate = this._baseFuelSpendRate * (withUpgrades.Contains(SIUpgradeType.Thruster_Jet_Duration) ? 0.8f : 1f);
			this.jetForce = this._baseJetForce * (withUpgrades.Contains(SIUpgradeType.Thruster_Jet_Accel) ? 1.2f : 1f);
			break;
		case SIGadgetWristJet.WristJetType.Propellor:
			this.fuelSpendRate = this._baseFuelSpendRate * (withUpgrades.Contains(SIUpgradeType.Thruster_Prop_Duration) ? 0.8f : 1f);
			this.maxVerticalSpeed = this._baseMaxVerticalSpeed * (withUpgrades.Contains(SIUpgradeType.Thruster_Prop_Speed) ? 1.2f : 1f);
			this.maxHorizontalSpeed = this._baseMaxHorizontalSpeed * (withUpgrades.Contains(SIUpgradeType.Thruster_Prop_Speed) ? 1.2f : 1f);
			break;
		}
		AudioClip clip;
		if (this._hasThrustLoopAudioSource && this.m_thrustLoopSoundByUpgrade.TryGetActiveValue(withUpgrades, out clip))
		{
			this.m_thrustLoopAudioSource.clip = clip;
			this.m_thrustLoopAudioSource.Play();
		}
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00028530 File Offset: 0x00026730
	public void Disrupt(float disruptTime)
	{
		this.emptiedCooldownResetProgress = -disruptTime;
		this.SetState(SIGadgetWristJet.State.OutOfFuel);
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00028544 File Offset: 0x00026744
	public override void OnEntityInit()
	{
		this.emptiedCooldownResetProgress = 0f;
		if (this._hasInactiveStateVisual)
		{
			this.inactiveStateVisual.SetActive(true);
		}
		if (this._hasActiveStateVisual)
		{
			this.activeStateVisual.SetActive(false);
		}
		this.currentFuel = (this.fuelSize = 10f);
		this._throttle = (this._currentBurnRate = 1f);
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x0600070B RID: 1803 RVA: 0x00023994 File Offset: 0x00021B94
	public bool UsesEnergy
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x0600070C RID: 1804 RVA: 0x000285AC File Offset: 0x000267AC
	public bool IsFull
	{
		get
		{
			return this.currentFuel >= this.fuelSize;
		}
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x000285BF File Offset: 0x000267BF
	public void UpdateRecharge(float dt)
	{
		if (this.CanRecharge)
		{
			this.currentFuel = Mathf.Clamp(this.currentFuel + dt * this.fuelGainRate, 0f, this.fuelSize);
		}
	}

	// Token: 0x040008B6 RID: 2230
	private const string preLog = "[SIGadgetWristJet]  ";

	// Token: 0x040008B7 RID: 2231
	private const string preErr = "[SIGadgetWristJet]  ERROR!!!  ";

	// Token: 0x040008B8 RID: 2232
	private const string preErrBeta = "[SIGadgetWristJet]  ERROR!!!  (beta only log)  ";

	// Token: 0x040008B9 RID: 2233
	[SerializeField]
	private AudioSource m_thrustLoopAudioSource;

	// Token: 0x040008BA RID: 2234
	private bool _hasThrustLoopAudioSource;

	// Token: 0x040008BB RID: 2235
	[SerializeField]
	private SIUpgradeBasedGeneric<AudioClip> m_thrustLoopSoundByUpgrade;

	// Token: 0x040008BC RID: 2236
	[SerializeField]
	private float m_thrustLoopAudioFadeInTime = 0.1f;

	// Token: 0x040008BD RID: 2237
	[SerializeField]
	private float m_thrustLoopAudioFadeOutTime = 0.5f;

	// Token: 0x040008BE RID: 2238
	[SerializeField]
	private float m_thrustLoopSoundVolume = 0.33f;

	// Token: 0x040008BF RID: 2239
	[SerializeField]
	private AudioClip m_warnFuelLowSound;

	// Token: 0x040008C0 RID: 2240
	[SerializeField]
	private float m_warnFuelLowThreshold = 0.5f;

	// Token: 0x040008C1 RID: 2241
	[SerializeField]
	private float m_warnFuelLowSoundVolume = 0.05f;

	// Token: 0x040008C2 RID: 2242
	private bool _warnFuelLowSoundWasPlayed;

	// Token: 0x040008C3 RID: 2243
	[Tooltip("This renderer's material will have the `_EmissionDissolveProgress` property changed to visually communicate current fuel amount.")]
	[SerializeField]
	private GTRendererMatSlot[] m_gaugeMatSlots;

	// Token: 0x040008C4 RID: 2244
	public SIGadgetWristJet.WristJetType jetType;

	// Token: 0x040008C5 RID: 2245
	public GameButtonActivatable buttonActivatable;

	// Token: 0x040008C6 RID: 2246
	public GameObject inactiveStateVisual;

	// Token: 0x040008C7 RID: 2247
	private bool _hasInactiveStateVisual;

	// Token: 0x040008C8 RID: 2248
	[FormerlySerializedAs("jetFlame")]
	public GameObject activeStateVisual;

	// Token: 0x040008C9 RID: 2249
	private bool _hasActiveStateVisual;

	// Token: 0x040008CA RID: 2250
	public float jetForce;

	// Token: 0x040008CB RID: 2251
	public float fuelGainRate;

	// Token: 0x040008CC RID: 2252
	public float fuelSpendRate;

	// Token: 0x040008CD RID: 2253
	public float emptiedCooldown;

	// Token: 0x040008CE RID: 2254
	public float gravityNegationPercent;

	// Token: 0x040008CF RID: 2255
	public float maxVerticalSpeed;

	// Token: 0x040008D0 RID: 2256
	public float maxHorizontalSpeed;

	// Token: 0x040008D1 RID: 2257
	[SerializeField]
	private bool rechargeRequiresFloorTouch;

	// Token: 0x040008D2 RID: 2258
	[SerializeField]
	private float throttleChangeSpeed = 2f;

	// Token: 0x040008D3 RID: 2259
	[SerializeField]
	[Tooltip("Minimum proportion of thrust allowed with throttle control.")]
	[Range(0f, 1f)]
	private float minimumBurnRate = 0.33f;

	// Token: 0x040008D4 RID: 2260
	[SerializeField]
	private Transform[] m_throttleFlapXforms;

	// Token: 0x040008D5 RID: 2261
	private Quaternion[] throttleFlapInitialRots;

	// Token: 0x040008D6 RID: 2262
	[SerializeField]
	private Quaternion m_throttleFlapMaxRotOffset = Quaternion.Euler(45f, 0f, 0f);

	// Token: 0x040008D7 RID: 2263
	private float fuelSize;

	// Token: 0x040008D8 RID: 2264
	private float currentFuel;

	// Token: 0x040008D9 RID: 2265
	private SIGadgetWristJet.State state;

	// Token: 0x040008DA RID: 2266
	private GTPlayer gtPlayer;

	// Token: 0x040008DB RID: 2267
	private float emptiedCooldownResetProgress;

	// Token: 0x040008DC RID: 2268
	private bool _floorTouched;

	// Token: 0x040008DD RID: 2269
	private float _maxSqrHorizontalSpeed;

	// Token: 0x040008DE RID: 2270
	private const float kFUEL_CAPACITY = 10f;

	// Token: 0x040008DF RID: 2271
	private MaterialPropertyBlock _gaugeMatPropBlock;

	// Token: 0x040008E0 RID: 2272
	private bool _throttleControl;

	// Token: 0x040008E1 RID: 2273
	private float _throttle;

	// Token: 0x040008E2 RID: 2274
	private float _currentBurnRate;

	// Token: 0x040008E3 RID: 2275
	private float _baseFuelSpendRate;

	// Token: 0x040008E4 RID: 2276
	private float _baseJetForce;

	// Token: 0x040008E5 RID: 2277
	private float _baseMaxVerticalSpeed;

	// Token: 0x040008E6 RID: 2278
	private float _baseMaxHorizontalSpeed;

	// Token: 0x02000118 RID: 280
	private enum State
	{
		// Token: 0x040008E8 RID: 2280
		Unactive,
		// Token: 0x040008E9 RID: 2281
		Active,
		// Token: 0x040008EA RID: 2282
		OutOfFuel
	}

	// Token: 0x02000119 RID: 281
	public enum WristJetType
	{
		// Token: 0x040008EC RID: 2284
		Basic,
		// Token: 0x040008ED RID: 2285
		Jet,
		// Token: 0x040008EE RID: 2286
		Propellor
	}
}
