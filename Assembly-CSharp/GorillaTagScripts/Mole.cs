using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EBD RID: 3773
	public class Mole : Tappable
	{
		// Token: 0x140000A3 RID: 163
		// (add) Token: 0x06005CC6 RID: 23750 RVA: 0x001D6E34 File Offset: 0x001D5034
		// (remove) Token: 0x06005CC7 RID: 23751 RVA: 0x001D6E6C File Offset: 0x001D506C
		public event Mole.MoleTapEvent OnTapped;

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x06005CC8 RID: 23752 RVA: 0x001D6EA1 File Offset: 0x001D50A1
		// (set) Token: 0x06005CC9 RID: 23753 RVA: 0x001D6EA9 File Offset: 0x001D50A9
		public bool IsLeftSideMole { get; set; }

		// Token: 0x06005CCA RID: 23754 RVA: 0x001D6EB4 File Offset: 0x001D50B4
		private void Awake()
		{
			this.currentState = Mole.MoleState.Hidden;
			Vector3 position = base.transform.position;
			this.origin = (this.target = position);
			this.visiblePosition = new Vector3(position.x, position.y + this.positionOffset, position.z);
			this.hiddenPosition = new Vector3(position.x, position.y - this.positionOffset, position.z);
			this.travelTime = this.normalTravelTime;
			this.animCurve = (this.normalAnimCurve = AnimationCurves.EaseInOutQuad);
			this.hitAnimCurve = AnimationCurves.EaseOutBack;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				if (this.moleTypes[i].isHazard)
				{
					this.hazardMoles.Add(i);
				}
				else
				{
					this.safeMoles.Add(i);
				}
			}
			this.randomMolePickedIndex = -1;
		}

		// Token: 0x06005CCB RID: 23755 RVA: 0x001D6F9C File Offset: 0x001D519C
		public void InvokeUpdate()
		{
			if (this.currentState == Mole.MoleState.Ready)
			{
				return;
			}
			switch (this.currentState)
			{
			case Mole.MoleState.Reset:
			case Mole.MoleState.Hidden:
				this.currentState = Mole.MoleState.Ready;
				break;
			case Mole.MoleState.TransitionToVisible:
			case Mole.MoleState.TransitionToHidden:
			{
				float num = this.animCurve.Evaluate(Mathf.Clamp01((Time.time - this.animStartTime) / this.travelTime));
				base.transform.position = Vector3.Lerp(this.origin, this.target, num);
				if (num >= 1f)
				{
					this.currentState++;
				}
				break;
			}
			}
			if (Time.time - this.currentTime >= this.showMoleDuration && this.currentState > Mole.MoleState.Ready && this.currentState < Mole.MoleState.TransitionToHidden)
			{
				this.HideMole(false);
			}
		}

		// Token: 0x06005CCC RID: 23756 RVA: 0x001D7067 File Offset: 0x001D5267
		public bool CanPickMole()
		{
			return this.currentState == Mole.MoleState.Ready;
		}

		// Token: 0x06005CCD RID: 23757 RVA: 0x001D7074 File Offset: 0x001D5274
		public void ShowMole(float _showMoleDuration, int randomMoleTypeIndex)
		{
			if (randomMoleTypeIndex >= this.moleTypes.Length || randomMoleTypeIndex < 0)
			{
				return;
			}
			this.randomMolePickedIndex = randomMoleTypeIndex;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				this.moleTypes[i].gameObject.SetActive(i == randomMoleTypeIndex);
				if (this.moleTypes[i].monkeMoleDefaultMaterial != null)
				{
					this.moleTypes[i].MeshRenderer.material = this.moleTypes[i].monkeMoleDefaultMaterial;
				}
			}
			this.showMoleDuration = _showMoleDuration;
			this.origin = base.transform.position;
			this.target = this.visiblePosition;
			this.animCurve = this.normalAnimCurve;
			this.currentState = Mole.MoleState.TransitionToVisible;
			this.animStartTime = (this.currentTime = Time.time);
			this.travelTime = this.normalTravelTime;
		}

		// Token: 0x06005CCE RID: 23758 RVA: 0x001D714C File Offset: 0x001D534C
		public void HideMole(bool isHit = false)
		{
			if (this.currentState < Mole.MoleState.TransitionToVisible || this.currentState > Mole.MoleState.Visible)
			{
				return;
			}
			this.origin = base.transform.position;
			this.target = this.hiddenPosition;
			this.animCurve = (isHit ? this.hitAnimCurve : this.normalAnimCurve);
			this.animStartTime = Time.time;
			this.travelTime = (isHit ? this.hitTravelTime : this.normalTravelTime);
			this.currentState = Mole.MoleState.TransitionToHidden;
		}

		// Token: 0x06005CCF RID: 23759 RVA: 0x001D71CC File Offset: 0x001D53CC
		public bool CanTap()
		{
			Mole.MoleState moleState = this.currentState;
			return moleState == Mole.MoleState.TransitionToVisible || moleState == Mole.MoleState.Visible;
		}

		// Token: 0x06005CD0 RID: 23760 RVA: 0x001D71F1 File Offset: 0x001D53F1
		public override bool CanTap(bool isLeftHand)
		{
			return this.CanTap();
		}

		// Token: 0x06005CD1 RID: 23761 RVA: 0x001D71FC File Offset: 0x001D53FC
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!this.CanTap())
			{
				return;
			}
			bool flag = info.Sender.ActorNumber == NetworkSystem.Instance.LocalPlayerID;
			bool isLeft = flag && GorillaTagger.Instance.lastLeftTap >= GorillaTagger.Instance.lastRightTap;
			MoleTypes moleTypes = null;
			if (this.randomMolePickedIndex >= 0 && this.randomMolePickedIndex < this.moleTypes.Length)
			{
				moleTypes = this.moleTypes[this.randomMolePickedIndex];
			}
			if (moleTypes != null)
			{
				Mole.MoleTapEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(moleTypes, base.transform.position, flag, isLeft);
			}
		}

		// Token: 0x06005CD2 RID: 23762 RVA: 0x001D729A File Offset: 0x001D549A
		public void ResetPosition()
		{
			base.transform.position = this.hiddenPosition;
			this.currentState = Mole.MoleState.Reset;
		}

		// Token: 0x06005CD3 RID: 23763 RVA: 0x001D72B4 File Offset: 0x001D54B4
		public int GetMoleTypeIndex(bool useHazardMole)
		{
			if (!useHazardMole)
			{
				return this.safeMoles[Random.Range(0, this.safeMoles.Count)];
			}
			return this.hazardMoles[Random.Range(0, this.hazardMoles.Count)];
		}

		// Token: 0x04006B2F RID: 27439
		public float positionOffset = 0.2f;

		// Token: 0x04006B30 RID: 27440
		public MoleTypes[] moleTypes;

		// Token: 0x04006B31 RID: 27441
		private float showMoleDuration;

		// Token: 0x04006B32 RID: 27442
		private Vector3 visiblePosition;

		// Token: 0x04006B33 RID: 27443
		private Vector3 hiddenPosition;

		// Token: 0x04006B34 RID: 27444
		private float currentTime;

		// Token: 0x04006B35 RID: 27445
		private float animStartTime;

		// Token: 0x04006B36 RID: 27446
		private float travelTime;

		// Token: 0x04006B37 RID: 27447
		private float normalTravelTime = 0.3f;

		// Token: 0x04006B38 RID: 27448
		private float hitTravelTime = 0.2f;

		// Token: 0x04006B39 RID: 27449
		private AnimationCurve animCurve;

		// Token: 0x04006B3A RID: 27450
		private AnimationCurve normalAnimCurve;

		// Token: 0x04006B3B RID: 27451
		private AnimationCurve hitAnimCurve;

		// Token: 0x04006B3C RID: 27452
		private Mole.MoleState currentState;

		// Token: 0x04006B3D RID: 27453
		private Vector3 origin;

		// Token: 0x04006B3E RID: 27454
		private Vector3 target;

		// Token: 0x04006B3F RID: 27455
		private int randomMolePickedIndex;

		// Token: 0x04006B41 RID: 27457
		public CallLimiter rpcCooldown;

		// Token: 0x04006B42 RID: 27458
		private int moleScore;

		// Token: 0x04006B43 RID: 27459
		private List<int> safeMoles = new List<int>();

		// Token: 0x04006B44 RID: 27460
		private List<int> hazardMoles = new List<int>();

		// Token: 0x02000EBE RID: 3774
		// (Invoke) Token: 0x06005CD6 RID: 23766
		public delegate void MoleTapEvent(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeft);

		// Token: 0x02000EBF RID: 3775
		public enum MoleState
		{
			// Token: 0x04006B47 RID: 27463
			Reset,
			// Token: 0x04006B48 RID: 27464
			Ready,
			// Token: 0x04006B49 RID: 27465
			TransitionToVisible,
			// Token: 0x04006B4A RID: 27466
			Visible,
			// Token: 0x04006B4B RID: 27467
			TransitionToHidden,
			// Token: 0x04006B4C RID: 27468
			Hidden
		}
	}
}
