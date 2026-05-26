using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
	// Token: 0x02000EB6 RID: 3766
	public class GorillaSnapTurn : LocomotionProvider, ITickSystemTick
	{
		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x06005C92 RID: 23698 RVA: 0x001D614C File Offset: 0x001D434C
		// (set) Token: 0x06005C93 RID: 23699 RVA: 0x001D6154 File Offset: 0x001D4354
		public bool TickRunning { get; set; }

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06005C94 RID: 23700 RVA: 0x001D615D File Offset: 0x001D435D
		// (set) Token: 0x06005C95 RID: 23701 RVA: 0x001D6165 File Offset: 0x001D4365
		public GorillaSnapTurn.InputAxes turnUsage
		{
			get
			{
				return this.m_TurnUsage;
			}
			set
			{
				this.m_TurnUsage = value;
			}
		}

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06005C96 RID: 23702 RVA: 0x001D616E File Offset: 0x001D436E
		// (set) Token: 0x06005C97 RID: 23703 RVA: 0x001D6176 File Offset: 0x001D4376
		public List<XRController> controllers
		{
			get
			{
				return this.m_Controllers;
			}
			set
			{
				this.m_Controllers = value;
			}
		}

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06005C98 RID: 23704 RVA: 0x001D617F File Offset: 0x001D437F
		// (set) Token: 0x06005C99 RID: 23705 RVA: 0x001D6187 File Offset: 0x001D4387
		public float turnAmount
		{
			get
			{
				return this.m_TurnAmount;
			}
			set
			{
				this.m_TurnAmount = value;
			}
		}

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x06005C9A RID: 23706 RVA: 0x001D6190 File Offset: 0x001D4390
		// (set) Token: 0x06005C9B RID: 23707 RVA: 0x001D6198 File Offset: 0x001D4398
		public float debounceTime
		{
			get
			{
				return this.m_DebounceTime;
			}
			set
			{
				this.m_DebounceTime = value;
			}
		}

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x06005C9C RID: 23708 RVA: 0x001D61A1 File Offset: 0x001D43A1
		// (set) Token: 0x06005C9D RID: 23709 RVA: 0x001D61A9 File Offset: 0x001D43A9
		public float deadZone
		{
			get
			{
				return this.m_DeadZone;
			}
			set
			{
				this.m_DeadZone = value;
			}
		}

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x06005C9E RID: 23710 RVA: 0x001D61B2 File Offset: 0x001D43B2
		// (set) Token: 0x06005C9F RID: 23711 RVA: 0x001D61BA File Offset: 0x001D43BA
		public string turnType
		{
			get
			{
				return this.m_TurnType;
			}
			private set
			{
				this.m_TurnType = value;
			}
		}

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x06005CA0 RID: 23712 RVA: 0x001D61C3 File Offset: 0x001D43C3
		// (set) Token: 0x06005CA1 RID: 23713 RVA: 0x001D61CB File Offset: 0x001D43CB
		public int turnFactor
		{
			get
			{
				return this.m_TurnFactor;
			}
			private set
			{
				this.m_TurnFactor = value;
			}
		}

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x06005CA2 RID: 23714 RVA: 0x001D61D4 File Offset: 0x001D43D4
		public static GorillaSnapTurn CachedSnapTurnRef
		{
			get
			{
				if (GorillaSnapTurn._cachedReference == null)
				{
					Debug.LogError("[SNAP_TURN] Tried accessing static cached reference, but was still null. Trying to find component in scene");
					GorillaSnapTurn._cachedReference = Object.FindAnyObjectByType<GorillaSnapTurn>();
				}
				return GorillaSnapTurn._cachedReference;
			}
		}

		// Token: 0x06005CA3 RID: 23715 RVA: 0x001D61FC File Offset: 0x001D43FC
		protected override void Awake()
		{
			base.Awake();
			if (GorillaSnapTurn._cachedReference != null)
			{
				Debug.LogError("[SNAP_TURN] A [GorillaSnapTurn] component already exists in the scene");
				return;
			}
			GorillaSnapTurn._cachedReference = this;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06005CA4 RID: 23716 RVA: 0x001D6228 File Offset: 0x001D4428
		public void Tick()
		{
			this.ValidateTurningOverriders();
			if (this.m_Controllers.Count > 0)
			{
				this.EnsureControllerDataListSize();
				for (int i = 0; i < this.m_Controllers.Count; i++)
				{
					XRController xrcontroller = this.m_Controllers[i];
					if (!(xrcontroller == null) && xrcontroller.enableInputActions)
					{
						float num = 0f;
						if (xrcontroller.controllerNode == XRNode.RightHand)
						{
							num = ControllerInputPoller.instance.rightControllerPrimary2DAxis.x;
						}
						else if (xrcontroller.controllerNode == XRNode.LeftHand)
						{
							num = ControllerInputPoller.instance.leftControllerPrimary2DAxis.x;
						}
						if (num > this.deadZone)
						{
							this.StartTurn(this.m_TurnAmount);
						}
						else if (num < -this.deadZone)
						{
							this.StartTurn(-this.m_TurnAmount);
						}
						else
						{
							this.m_AxisReset = true;
						}
					}
				}
			}
			if (Mathf.Abs(this.m_CurrentTurnAmount) > 0f && base.TryPrepareLocomotion())
			{
				if (this.xrOrigin != null)
				{
					GTPlayer.Instance.Turn(this.m_CurrentTurnAmount);
				}
				this.m_CurrentTurnAmount = 0f;
				base.TryEndLocomotion();
			}
		}

		// Token: 0x06005CA5 RID: 23717 RVA: 0x001D634C File Offset: 0x001D454C
		private void EnsureControllerDataListSize()
		{
			if (this.m_Controllers.Count != this.m_ControllersWereActive.Count)
			{
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.Add(false);
				}
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.RemoveAt(this.m_ControllersWereActive.Count - 1);
				}
			}
		}

		// Token: 0x06005CA6 RID: 23718 RVA: 0x001D63C9 File Offset: 0x001D45C9
		internal void FakeStartTurn(bool isLeft)
		{
			this.StartTurn(isLeft ? (-this.m_TurnAmount) : this.m_TurnAmount);
		}

		// Token: 0x06005CA7 RID: 23719 RVA: 0x001D63E4 File Offset: 0x001D45E4
		private void StartTurn(float amount)
		{
			if (this.m_TimeStarted + this.m_DebounceTime > Time.time && !this.m_AxisReset)
			{
				return;
			}
			if (base.isLocomotionActive)
			{
				return;
			}
			if (this.turningOverriders.Count > 0)
			{
				return;
			}
			this.m_TimeStarted = Time.time;
			this.m_CurrentTurnAmount = amount;
			this.m_AxisReset = false;
		}

		// Token: 0x06005CA8 RID: 23720 RVA: 0x001D6440 File Offset: 0x001D4640
		public void ChangeTurnMode(string turnMode, int turnSpeedFactor)
		{
			this.turnType = turnMode;
			this.turnFactor = turnSpeedFactor;
			if (turnMode == "SNAP")
			{
				this.m_DebounceTime = 0.5f;
				this.m_TurnAmount = 60f * this.ConvertedTurnFactor((float)turnSpeedFactor);
				return;
			}
			if (!(turnMode == "SMOOTH"))
			{
				this.m_DebounceTime = 0f;
				this.m_TurnAmount = 0f;
				return;
			}
			this.m_DebounceTime = 0f;
			this.m_TurnAmount = 360f * Time.fixedDeltaTime * this.ConvertedTurnFactor((float)turnSpeedFactor);
		}

		// Token: 0x06005CA9 RID: 23721 RVA: 0x001D64D3 File Offset: 0x001D46D3
		public float ConvertedTurnFactor(float newTurnSpeed)
		{
			return Mathf.Max(0.75f, 0.5f + newTurnSpeed / 10f * 1.5f);
		}

		// Token: 0x06005CAA RID: 23722 RVA: 0x001D64F2 File Offset: 0x001D46F2
		public void SetTurningOverride(ISnapTurnOverride caller)
		{
			if (!this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Add(caller);
			}
		}

		// Token: 0x06005CAB RID: 23723 RVA: 0x001D650F File Offset: 0x001D470F
		public void UnsetTurningOverride(ISnapTurnOverride caller)
		{
			if (this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Remove(caller);
			}
		}

		// Token: 0x06005CAC RID: 23724 RVA: 0x001D652C File Offset: 0x001D472C
		public void ValidateTurningOverriders()
		{
			foreach (ISnapTurnOverride snapTurnOverride in this.turningOverriders)
			{
				if (snapTurnOverride == null || !snapTurnOverride.TurnOverrideActive())
				{
					this.turningOverriders.Remove(snapTurnOverride);
				}
			}
		}

		// Token: 0x06005CAD RID: 23725 RVA: 0x001D6590 File Offset: 0x001D4790
		public static void DisableSnapTurn()
		{
			Debug.Log("[SNAP_TURN] Disabling Snap Turn");
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			GorillaSnapTurn._cachedTurnFactor = PlayerPrefs.GetInt("turnFactor");
			GorillaSnapTurn._cachedTurnType = PlayerPrefs.GetString("stickTurning");
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode("NONE", 0);
		}

		// Token: 0x06005CAE RID: 23726 RVA: 0x001D65E3 File Offset: 0x001D47E3
		public static void UpdateAndSaveTurnType(string mode)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetString("stickTurning", mode);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(mode, GorillaSnapTurn.CachedSnapTurnRef.turnFactor);
		}

		// Token: 0x06005CAF RID: 23727 RVA: 0x001D6622 File Offset: 0x001D4822
		public static void UpdateAndSaveTurnFactor(int factor)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetInt("turnFactor", factor);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(GorillaSnapTurn.CachedSnapTurnRef.turnType, factor);
		}

		// Token: 0x06005CB0 RID: 23728 RVA: 0x001D6664 File Offset: 0x001D4864
		public static void LoadSettingsFromPlayerPrefs()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			string defaultValue = (Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP";
			string @string = PlayerPrefs.GetString("stickTurning", defaultValue);
			int @int = PlayerPrefs.GetInt("turnFactor", 4);
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(@string, @int);
		}

		// Token: 0x06005CB1 RID: 23729 RVA: 0x001D66BC File Offset: 0x001D48BC
		public static void LoadSettingsFromCache()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(GorillaSnapTurn._cachedTurnType))
			{
				GorillaSnapTurn._cachedTurnType = ((Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP");
			}
			string cachedTurnType = GorillaSnapTurn._cachedTurnType;
			int cachedTurnFactor = GorillaSnapTurn._cachedTurnFactor;
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(cachedTurnType, cachedTurnFactor);
		}

		// Token: 0x04006AF1 RID: 27377
		[Header("References")]
		[SerializeField]
		private XROrigin xrOrigin;

		// Token: 0x04006AF2 RID: 27378
		private static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[]
		{
			CommonUsages.primary2DAxis,
			CommonUsages.secondary2DAxis
		};

		// Token: 0x04006AF3 RID: 27379
		[SerializeField]
		[Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
		private GorillaSnapTurn.InputAxes m_TurnUsage;

		// Token: 0x04006AF4 RID: 27380
		[SerializeField]
		[Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
		private List<XRController> m_Controllers = new List<XRController>();

		// Token: 0x04006AF5 RID: 27381
		[SerializeField]
		[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
		private float m_TurnAmount = 45f;

		// Token: 0x04006AF6 RID: 27382
		[SerializeField]
		[Tooltip("The amount of time that the system will wait before starting another snap turn.")]
		private float m_DebounceTime = 0.5f;

		// Token: 0x04006AF7 RID: 27383
		[SerializeField]
		[Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
		private float m_DeadZone = 0.75f;

		// Token: 0x04006AF8 RID: 27384
		private float m_CurrentTurnAmount;

		// Token: 0x04006AF9 RID: 27385
		private float m_TimeStarted;

		// Token: 0x04006AFA RID: 27386
		private bool m_AxisReset;

		// Token: 0x04006AFB RID: 27387
		public float turnSpeed = 1f;

		// Token: 0x04006AFC RID: 27388
		private HashSet<ISnapTurnOverride> turningOverriders = new HashSet<ISnapTurnOverride>();

		// Token: 0x04006AFD RID: 27389
		private List<bool> m_ControllersWereActive = new List<bool>();

		// Token: 0x04006AFE RID: 27390
		private static int _cachedTurnFactor;

		// Token: 0x04006AFF RID: 27391
		private static string _cachedTurnType;

		// Token: 0x04006B00 RID: 27392
		private string m_TurnType = "";

		// Token: 0x04006B01 RID: 27393
		private int m_TurnFactor = 1;

		// Token: 0x04006B02 RID: 27394
		[OnEnterPlay_SetNull]
		private static GorillaSnapTurn _cachedReference;

		// Token: 0x02000EB7 RID: 3767
		public enum InputAxes
		{
			// Token: 0x04006B04 RID: 27396
			Primary2DAxis,
			// Token: 0x04006B05 RID: 27397
			Secondary2DAxis
		}
	}
}
