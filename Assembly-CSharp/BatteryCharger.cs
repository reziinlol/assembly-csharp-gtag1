using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000192 RID: 402
public class BatteryCharger : MonoBehaviour
{
	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000ACE RID: 2766 RVA: 0x00039F23 File Offset: 0x00038123
	public int CurrentEventPhase
	{
		get
		{
			if (!(this.state != null))
			{
				return -1;
			}
			return this.state.EventPhase;
		}
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x00039F40 File Offset: 0x00038140
	internal int RegisterCrank(BatteryChargerCrank crank)
	{
		if (this.crankCount >= 20)
		{
			Debug.LogError(string.Format("BatteryCharger: too many cranks (max {0})", 20), this);
			return -1;
		}
		int num = this.crankCount;
		this.cranks[num] = crank;
		this.crankCount++;
		return num;
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x00038BCD File Offset: 0x00036DCD
	private int LocalActorNr
	{
		get
		{
			if (PhotonNetwork.LocalPlayer == null)
			{
				return -1;
			}
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00039F90 File Offset: 0x00038190
	private void OnEnable()
	{
		BatteryChargerState newState;
		if (this.stateRef.TryResolve<BatteryChargerState>(out newState))
		{
			this.Bind(newState);
			return;
		}
		this.stateRef.AddCallbackOnLoad(new Action(this.OnStateSceneLoaded));
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00039FCB File Offset: 0x000381CB
	private void OnDisable()
	{
		this.stateRef.RemoveCallbackOnLoad(new Action(this.OnStateSceneLoaded));
		this.Unbind();
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x00039FEC File Offset: 0x000381EC
	private void OnStateSceneLoaded()
	{
		BatteryChargerState newState;
		if (this.stateRef.TryResolve<BatteryChargerState>(out newState))
		{
			this.Bind(newState);
		}
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x0003A010 File Offset: 0x00038210
	private void Bind(BatteryChargerState newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.Unbind();
		this.state = newState;
		if (this.state == null)
		{
			return;
		}
		this.state.onChargeChanged += this.OnChargeChanged;
		this.state.onFullyCharged += this.OnFullyCharged;
		this.state.onEventPhaseChanged += this.OnEventPhaseChanged;
		this.previousCharge = this.state.CurrentCharge;
		this.ApplyChargeVisuals();
		this.OnEventPhaseChanged(this.state.EventPhase);
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0003A0B8 File Offset: 0x000382B8
	private void Unbind()
	{
		if (this.state == null)
		{
			return;
		}
		this.state.onChargeChanged -= this.OnChargeChanged;
		this.state.onFullyCharged -= this.OnFullyCharged;
		this.state.onEventPhaseChanged -= this.OnEventPhaseChanged;
		this.state = null;
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0003A120 File Offset: 0x00038320
	private void LateUpdate()
	{
		if (this.state == null)
		{
			return;
		}
		int localActorNr = this.LocalActorNr;
		for (int i = 0; i < this.crankCount; i++)
		{
			if (!(this.cranks[i] == null))
			{
				if (this.state.crankSyncs[i].holderActorNr == localActorNr)
				{
					this.state.UpdateLocalCrankState(i, this.cranks[i].IsHeldLeftHand, this.cranks[i].CurrentAngle);
				}
				this.UpdateRemoteCrankVisual(this.cranks[i], this.state.crankSyncs[i], localActorNr);
			}
		}
		if (this.chargingLoopSound != null)
		{
			bool flag = false;
			for (int j = 0; j < 20; j++)
			{
				if (this.state.crankSyncs[j].holderActorNr != -1)
				{
					flag = true;
					break;
				}
			}
			if (flag && !this.chargingLoopSound.isPlaying)
			{
				this.chargingLoopSound.Play();
				return;
			}
			if (!flag && this.chargingLoopSound.isPlaying)
			{
				this.chargingLoopSound.Stop();
			}
		}
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0003A234 File Offset: 0x00038434
	private void UpdateRemoteCrankVisual(BatteryChargerCrank crank, BatteryChargerState.CrankSyncState syncState, int localActor)
	{
		if (crank == null || syncState.holderActorNr == localActor)
		{
			return;
		}
		if (syncState.holderActorNr != -1)
		{
			VRRig vrrig = BatteryChargerState.FindRigForActor(syncState.holderActorNr);
			if (vrrig != null)
			{
				crank.UpdateFromRemoteHand(vrrig, syncState.isLeftHand);
				return;
			}
		}
		crank.SetVisualAngle(syncState.angle);
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x0003A28C File Offset: 0x0003848C
	internal bool IsCrankHeldLocally(int crankIndex)
	{
		return !(this.state == null) && crankIndex >= 0 && crankIndex < 20 && this.state.crankSyncs[crankIndex].holderActorNr == this.LocalActorNr;
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0003A2C5 File Offset: 0x000384C5
	public void SetEventPhase(int phase)
	{
		this.state.SetEventPhase(phase);
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0003A2D3 File Offset: 0x000384D3
	public void SetChargePerCrankDegree(float chargeRate)
	{
		this.state.SetChargePerCrankDegree(chargeRate);
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0003A2E1 File Offset: 0x000384E1
	internal bool OnCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		return this.state.NotifyCrankGrabbed(crankIndex, isLeftHand);
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0003A2F0 File Offset: 0x000384F0
	internal void OnCrankReleased(int crankIndex, float finalAngle)
	{
		this.state.NotifyCrankReleased(crankIndex, finalAngle);
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0003A2FF File Offset: 0x000384FF
	internal void OnCrankInput(int crankIndex, float degrees)
	{
		this.state.NotifyCrankInput(crankIndex, degrees);
		this.ApplyChargeVisuals();
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0003A314 File Offset: 0x00038514
	private void OnChargeChanged()
	{
		for (int i = 0; i < this.actions.Length; i++)
		{
			if ((this.actions[i].Direction == BatteryCharger.BatteryChargerEvent.VDirection.Up && this.previousCharge < this.state.CurrentCharge && this.previousCharge < this.actions[i].Value && this.state.CurrentCharge >= this.actions[i].Value) || (this.actions[i].Direction == BatteryCharger.BatteryChargerEvent.VDirection.Down && this.previousCharge > this.state.CurrentCharge && this.previousCharge > this.actions[i].Value && this.state.CurrentCharge <= this.actions[i].Value))
			{
				UnityEvent action = this.actions[i].Action;
				if (action != null)
				{
					action.Invoke();
				}
			}
		}
		this.previousCharge = this.state.CurrentCharge;
		this.ApplyChargeVisuals();
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0003A40C File Offset: 0x0003860C
	private void OnFullyCharged()
	{
		if (this.fullyChargedSound != null)
		{
			this.fullyChargedSound.GTPlay();
		}
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0003A428 File Offset: 0x00038628
	private void OnEventPhaseChanged(int phase)
	{
		for (int i = 0; i < this.eventPhases.Length; i++)
		{
			BatteryCharger.EventPhaseObjects eventPhaseObjects = this.eventPhases[i];
			if (((eventPhaseObjects != null) ? eventPhaseObjects.objects : null) != null)
			{
				bool active = i == phase;
				for (int j = 0; j < this.eventPhases[i].objects.Length; j++)
				{
					if (this.eventPhases[i].objects[j] != null)
					{
						this.eventPhases[i].objects[j].SetActive(active);
					}
				}
			}
		}
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0003A4AC File Offset: 0x000386AC
	private void ApplyChargeVisuals()
	{
		if (this.state == null)
		{
			return;
		}
		float chargePercent = this.state.ChargePercent;
		if (this.chargeFillTransform != null)
		{
			this.chargeFillTransform.localRotation = Quaternion.Euler(0f, 0f, chargePercent * this.chargeFullRollAngle);
		}
		if (this.chargeFillRenderer != null)
		{
			this.chargeFillRenderer.material.color = Color.Lerp(this.emptyColor, this.fullColor, chargePercent);
		}
	}

	// Token: 0x04000D12 RID: 3346
	[Header("Network State")]
	[SerializeField]
	private XSceneRef stateRef;

	// Token: 0x04000D13 RID: 3347
	[Header("Charge Visuals")]
	[Tooltip("Transform rotated on its local Z axis to show charge level")]
	[SerializeField]
	private Transform chargeFillTransform;

	// Token: 0x04000D14 RID: 3348
	[Tooltip("Local Z rotation in degrees when fully charged")]
	[SerializeField]
	private float chargeFullRollAngle = -180f;

	// Token: 0x04000D15 RID: 3349
	[Tooltip("Renderer whose material color lerps with charge")]
	[SerializeField]
	private Renderer chargeFillRenderer;

	// Token: 0x04000D16 RID: 3350
	[SerializeField]
	private Color emptyColor = Color.red;

	// Token: 0x04000D17 RID: 3351
	[SerializeField]
	private Color fullColor = Color.green;

	// Token: 0x04000D18 RID: 3352
	[Header("Audio")]
	[SerializeField]
	private AudioSource chargingLoopSound;

	// Token: 0x04000D19 RID: 3353
	[SerializeField]
	private AudioSource fullyChargedSound;

	// Token: 0x04000D1A RID: 3354
	[Header("Event Phases")]
	[SerializeField]
	private BatteryCharger.EventPhaseObjects[] eventPhases;

	// Token: 0x04000D1B RID: 3355
	private BatteryChargerState state;

	// Token: 0x04000D1C RID: 3356
	private BatteryChargerCrank[] cranks = new BatteryChargerCrank[20];

	// Token: 0x04000D1D RID: 3357
	private int crankCount;

	// Token: 0x04000D1E RID: 3358
	[SerializeField]
	private BatteryCharger.BatteryChargerEvent[] actions;

	// Token: 0x04000D1F RID: 3359
	private float previousCharge;

	// Token: 0x02000193 RID: 403
	[Serializable]
	private class EventPhaseObjects
	{
		// Token: 0x04000D20 RID: 3360
		public string friendlyName;

		// Token: 0x04000D21 RID: 3361
		public GameObject[] objects;
	}

	// Token: 0x02000194 RID: 404
	[Serializable]
	private class BatteryChargerEvent
	{
		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000AE4 RID: 2788 RVA: 0x0003A56A File Offset: 0x0003876A
		public BatteryCharger.BatteryChargerEvent.VDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x0003A572 File Offset: 0x00038772
		public float Value
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000AE6 RID: 2790 RVA: 0x0003A57A File Offset: 0x0003877A
		public UnityEvent Action
		{
			get
			{
				return this.action;
			}
		}

		// Token: 0x04000D22 RID: 3362
		[SerializeField]
		private BatteryCharger.BatteryChargerEvent.VDirection direction;

		// Token: 0x04000D23 RID: 3363
		[SerializeField]
		private float value;

		// Token: 0x04000D24 RID: 3364
		[SerializeField]
		private UnityEvent action;

		// Token: 0x02000195 RID: 405
		public enum VDirection
		{
			// Token: 0x04000D26 RID: 3366
			Up,
			// Token: 0x04000D27 RID: 3367
			Down
		}
	}
}
