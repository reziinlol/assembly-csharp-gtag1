using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001283 RID: 4739
	public class FingerFlexEvent2 : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x060076C8 RID: 30408 RVA: 0x0026F61C File Offset: 0x0026D81C
		private bool TryLinkToNextEvent(int index)
		{
			if (index < this.list.Length - 1)
			{
				if (this.list[index].IsFlexTrigger && this.list[index + 1].IsReleaseTrigger)
				{
					this.list[index].linkIndex = index + 1;
					this.list[index + 1].linkIndex = index;
					return true;
				}
				this.list[index + 1].linkIndex = -1;
			}
			this.list[index].linkIndex = -1;
			return false;
		}

		// Token: 0x060076C9 RID: 30409 RVA: 0x0026F698 File Offset: 0x0026D898
		private void Awake()
		{
			this.myRig = base.GetComponentInParent<VRRig>();
			this.myTransferrable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
			for (int i = 0; i < this.list.Length; i++)
			{
				FingerFlexEvent2.FlexEvent flexEvent = this.list[i];
				if (this.myTransferrable.IsNull() && flexEvent.RequiresHeldItem)
				{
					this.myTransferrable = base.GetComponentInParent<TransferrableObject>();
				}
				if (flexEvent.tryLink && this.TryLinkToNextEvent(i))
				{
					FingerFlexEvent2.FlexEvent flexEvent2 = this.list[i + 1];
					flexEvent.releaseThreshold = flexEvent2.releaseThreshold;
					flexEvent2.flexThreshold = flexEvent.flexThreshold;
					flexEvent2.fingerType = flexEvent.fingerType;
					flexEvent2.handType = flexEvent.handType;
					flexEvent2.networked = flexEvent.networked;
					i++;
				}
			}
		}

		// Token: 0x060076CA RID: 30410 RVA: 0x0026F768 File Offset: 0x0026D968
		private void CalcFlex(bool disable)
		{
			for (int i = 0; i < this.list.Length; i++)
			{
				FingerFlexEvent2.FlexEvent flexEvent = this.list[i];
				if ((flexEvent.networked || this.myRig.isOfflineVRRig) && (!flexEvent.RequiresHeldItem || !this.myTransferrable.IsNull() || this.myHeldItem != null) && (flexEvent.handType != FingerFlexEvent2.FlexEvent.HandType.EquippedSide || !this.myTransferrable.IsNull()))
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					switch (flexEvent.handType)
					{
					case FingerFlexEvent2.FlexEvent.HandType.HeldItemHand:
						if (!this.myTransferrable.IsNull())
						{
							flag = (this.myTransferrable.currentState == TransferrableObject.PositionState.InLeftHand);
							flag2 = (this.myTransferrable.currentState == TransferrableObject.PositionState.InRightHand);
							flag3 = (flag || flag2);
						}
						else if (this.myHeldItem != null)
						{
							flag = this.myHeldItem.InLeftHand();
							flag2 = (!flag && this.myHeldItem.InHand());
							flag3 = (flag || flag2);
						}
						break;
					case FingerFlexEvent2.FlexEvent.HandType.EquippedSide:
						flag = ((this.myTransferrable.storedZone & (BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.LeftBack)) > BodyDockPositions.DropPositions.None);
						flag2 = ((this.myTransferrable.storedZone & (BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.RightBack)) > BodyDockPositions.DropPositions.None);
						break;
					case FingerFlexEvent2.FlexEvent.HandType.LeftHand:
						flag = true;
						break;
					case FingerFlexEvent2.FlexEvent.HandType.RightHand:
						flag2 = true;
						break;
					}
					if ((!flag || !flag2) && (flag || flag2 || flexEvent.wasHeld))
					{
						float num;
						if (disable || (flexEvent.wasHeld && !flag3))
						{
							num = 0f;
						}
						else
						{
							FingerFlexEvent2.FlexEvent.FingerType fingerType = flexEvent.fingerType;
							float num2;
							switch (fingerType)
							{
							case FingerFlexEvent2.FlexEvent.FingerType.Thumb:
								num2 = (flag ? this.myRig.leftThumb.calcT : this.myRig.rightThumb.calcT);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.Index:
								num2 = (flag ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.Middle:
								num2 = (flag ? this.myRig.leftMiddle.calcT : this.myRig.rightMiddle.calcT);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.IndexAndMiddle:
								num2 = (flag ? Mathf.Min(this.myRig.leftIndex.calcT, this.myRig.leftMiddle.calcT) : Mathf.Min(this.myRig.rightIndex.calcT, this.myRig.rightMiddle.calcT));
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.IndexOrMiddle:
								num2 = (flag ? Mathf.Max(this.myRig.leftIndex.calcT, this.myRig.leftMiddle.calcT) : Mathf.Max(this.myRig.rightIndex.calcT, this.myRig.rightMiddle.calcT));
								break;
							default:
								<PrivateImplementationDetails>.ThrowSwitchExpressionException(fingerType);
								break;
							}
							num = num2;
						}
						float flexValue = num;
						flexEvent.ProcessState(flag, flexValue);
						flexEvent.wasHeld = (flag3 && !disable);
						if (flexEvent.IsLinked)
						{
							FingerFlexEvent2.FlexEvent flexEvent2 = this.list[i + 1];
							flexEvent2.ProcessState(flag, flexValue);
							flexEvent2.wasHeld = flag3;
							i++;
						}
					}
				}
			}
		}

		// Token: 0x060076CB RID: 30411 RVA: 0x00019E3F File Offset: 0x0001803F
		public void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x060076CC RID: 30412 RVA: 0x0026FA7E File Offset: 0x0026DC7E
		public void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
			this.CalcFlex(true);
		}

		// Token: 0x17000B76 RID: 2934
		// (get) Token: 0x060076CD RID: 30413 RVA: 0x0026FA8D File Offset: 0x0026DC8D
		// (set) Token: 0x060076CE RID: 30414 RVA: 0x0026FA95 File Offset: 0x0026DC95
		public bool TickRunning { get; set; }

		// Token: 0x060076CF RID: 30415 RVA: 0x0026FA9E File Offset: 0x0026DC9E
		public void Tick()
		{
			this.CalcFlex(false);
		}

		// Token: 0x040088E6 RID: 35046
		public FingerFlexEvent2.FlexEvent[] list;

		// Token: 0x040088E7 RID: 35047
		private VRRig myRig;

		// Token: 0x040088E8 RID: 35048
		private TransferrableObject myTransferrable;

		// Token: 0x040088E9 RID: 35049
		private IHeldItem myHeldItem;

		// Token: 0x02001284 RID: 4740
		[Serializable]
		public class FlexEvent
		{
			// Token: 0x17000B77 RID: 2935
			// (get) Token: 0x060076D1 RID: 30417 RVA: 0x0026FAA7 File Offset: 0x0026DCA7
			public bool IsFlexTrigger
			{
				get
				{
					return this.triggerType == FingerFlexEvent2.FlexEvent.TriggerType.OnFlex;
				}
			}

			// Token: 0x17000B78 RID: 2936
			// (get) Token: 0x060076D2 RID: 30418 RVA: 0x0026FAB2 File Offset: 0x0026DCB2
			public bool IsReleaseTrigger
			{
				get
				{
					return this.triggerType == FingerFlexEvent2.FlexEvent.TriggerType.OnRelease;
				}
			}

			// Token: 0x17000B79 RID: 2937
			// (get) Token: 0x060076D3 RID: 30419 RVA: 0x0026FAC0 File Offset: 0x0026DCC0
			public bool RequiresHeldItem
			{
				get
				{
					FingerFlexEvent2.FlexEvent.HandType handType = this.handType;
					return handType == FingerFlexEvent2.FlexEvent.HandType.HeldItemHand || handType == FingerFlexEvent2.FlexEvent.HandType.EquippedSide;
				}
			}

			// Token: 0x17000B7A RID: 2938
			// (get) Token: 0x060076D4 RID: 30420 RVA: 0x0026FAE4 File Offset: 0x0026DCE4
			public bool HasValidLink
			{
				get
				{
					return this.linkIndex >= 0;
				}
			}

			// Token: 0x17000B7B RID: 2939
			// (get) Token: 0x060076D5 RID: 30421 RVA: 0x0026FAF2 File Offset: 0x0026DCF2
			public bool IsLinked
			{
				get
				{
					return this.tryLink && this.linkIndex >= 0;
				}
			}

			// Token: 0x17000B7C RID: 2940
			// (get) Token: 0x060076D6 RID: 30422 RVA: 0x0026FB0A File Offset: 0x0026DD0A
			private bool ShowMainProperties
			{
				get
				{
					return !this.IsLinked || this.IsFlexTrigger;
				}
			}

			// Token: 0x17000B7D RID: 2941
			// (get) Token: 0x060076D7 RID: 30423 RVA: 0x0026FB1C File Offset: 0x0026DD1C
			private bool ShowFlexThreshold
			{
				get
				{
					return this.ShowMainProperties;
				}
			}

			// Token: 0x17000B7E RID: 2942
			// (get) Token: 0x060076D8 RID: 30424 RVA: 0x0026FB24 File Offset: 0x0026DD24
			private bool ShowReleaseThreshold
			{
				get
				{
					return (!this.IsLinked || this.IsReleaseTrigger) && !this.IsFlexTrigger;
				}
			}

			// Token: 0x060076D9 RID: 30425 RVA: 0x0026FB44 File Offset: 0x0026DD44
			public void ProcessState(bool leftHand, float flexValue)
			{
				this.currentState = ((flexValue < this.releaseThreshold) ? FingerFlexEvent2.FlexEvent.RangeState.Below : ((flexValue >= this.flexThreshold) ? FingerFlexEvent2.FlexEvent.RangeState.Above : FingerFlexEvent2.FlexEvent.RangeState.Within));
				if (this.ShowMainProperties && this.currentState != this.lastState && this.continuousProperties != null && this.continuousProperties.Count > 0)
				{
					float f = Mathf.InverseLerp(this.releaseThreshold, this.flexThreshold, flexValue);
					this.continuousProperties.ApplyAll(f);
				}
				if (this.currentState == FingerFlexEvent2.FlexEvent.RangeState.Above && this.lastState == FingerFlexEvent2.FlexEvent.RangeState.Below)
				{
					this.lastThresholdTime = Time.time;
					this.lastState = FingerFlexEvent2.FlexEvent.RangeState.Above;
					if (this.IsFlexTrigger)
					{
						UnityEvent<bool, float> unityEvent = this.unityEvent;
						if (unityEvent == null)
						{
							return;
						}
						unityEvent.Invoke(leftHand, flexValue);
						return;
					}
				}
				else if (this.currentState == FingerFlexEvent2.FlexEvent.RangeState.Below && this.lastState == FingerFlexEvent2.FlexEvent.RangeState.Above)
				{
					this.lastThresholdTime = Time.time;
					this.lastState = FingerFlexEvent2.FlexEvent.RangeState.Below;
					if (this.IsReleaseTrigger)
					{
						UnityEvent<bool, float> unityEvent2 = this.unityEvent;
						if (unityEvent2 == null)
						{
							return;
						}
						unityEvent2.Invoke(leftHand, flexValue);
					}
				}
			}

			// Token: 0x040088EB RID: 35051
			public FingerFlexEvent2.FlexEvent.TriggerType triggerType;

			// Token: 0x040088EC RID: 35052
			public bool tryLink = true;

			// Token: 0x040088ED RID: 35053
			[HideInInspector]
			public int linkIndex = -1;

			// Token: 0x040088EE RID: 35054
			[Space]
			public FingerFlexEvent2.FlexEvent.FingerType fingerType = FingerFlexEvent2.FlexEvent.FingerType.Index;

			// Token: 0x040088EF RID: 35055
			[Space]
			public FingerFlexEvent2.FlexEvent.HandType handType;

			// Token: 0x040088F0 RID: 35056
			private const string ADVANCED = "Advanced Properties";

			// Token: 0x040088F1 RID: 35057
			[Tooltip("When this is checked, all players in the room will fire the event. Otherwise, only the local player will fire it. You should usually leave this on, unless you're using it for something local like controller haptics.")]
			public bool networked = true;

			// Token: 0x040088F2 RID: 35058
			[Range(0.01f, 0.75f)]
			public float flexThreshold = 0.75f;

			// Token: 0x040088F3 RID: 35059
			[Range(0.01f, 1f)]
			public float releaseThreshold = 0.01f;

			// Token: 0x040088F4 RID: 35060
			public ContinuousPropertyArray continuousProperties;

			// Token: 0x040088F5 RID: 35061
			public UnityEvent<bool, float> unityEvent;

			// Token: 0x040088F6 RID: 35062
			[NonSerialized]
			public bool wasHeld;

			// Token: 0x040088F7 RID: 35063
			[NonSerialized]
			public bool marginError;

			// Token: 0x040088F8 RID: 35064
			private FingerFlexEvent2.FlexEvent.RangeState currentState;

			// Token: 0x040088F9 RID: 35065
			private FingerFlexEvent2.FlexEvent.RangeState lastState;

			// Token: 0x040088FA RID: 35066
			private float lastThresholdTime = -100000f;

			// Token: 0x02001285 RID: 4741
			public enum TriggerType
			{
				// Token: 0x040088FC RID: 35068
				OnFlex,
				// Token: 0x040088FD RID: 35069
				OnRelease = 2
			}

			// Token: 0x02001286 RID: 4742
			public enum FingerType
			{
				// Token: 0x040088FF RID: 35071
				Thumb,
				// Token: 0x04008900 RID: 35072
				Index,
				// Token: 0x04008901 RID: 35073
				Middle,
				// Token: 0x04008902 RID: 35074
				IndexAndMiddle,
				// Token: 0x04008903 RID: 35075
				IndexOrMiddle
			}

			// Token: 0x02001287 RID: 4743
			public enum HandType
			{
				// Token: 0x04008905 RID: 35077
				HeldItemHand,
				// Token: 0x04008906 RID: 35078
				EquippedSide,
				// Token: 0x04008907 RID: 35079
				LeftHand,
				// Token: 0x04008908 RID: 35080
				RightHand
			}

			// Token: 0x02001288 RID: 4744
			private enum RangeState
			{
				// Token: 0x0400890A RID: 35082
				Below,
				// Token: 0x0400890B RID: 35083
				Within,
				// Token: 0x0400890C RID: 35084
				Above
			}
		}
	}
}
