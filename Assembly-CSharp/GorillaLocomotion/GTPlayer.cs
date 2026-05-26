using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AA;
using BoingKit;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion
{
	// Token: 0x020010DD RID: 4317
	public class GTPlayer : MonoBehaviour
	{
		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x06006C0F RID: 27663 RVA: 0x0022F457 File Offset: 0x0022D657
		public static GTPlayer Instance
		{
			get
			{
				return GTPlayer._instance;
			}
		}

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x06006C10 RID: 27664 RVA: 0x0022F460 File Offset: 0x0022D660
		private float bodyInitialHeight
		{
			get
			{
				if (GorillaIK.playerIK == null || !GorillaIK.playerIK.usingUpdatedIK)
				{
					return this._bodyInitialHeight;
				}
				return Mathf.Max(0.2f, Vector3.Dot(GorillaIK.playerIK.bodyBone.up, GTPlayerTransform.Up)) * this._bodyInitialHeight;
			}
		}

		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x06006C11 RID: 27665 RVA: 0x0022F4B7 File Offset: 0x0022D6B7
		public GTPlayer.HandState LeftHand
		{
			get
			{
				return this.leftHand;
			}
		}

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x06006C12 RID: 27666 RVA: 0x0022F4BF File Offset: 0x0022D6BF
		public ref readonly GTPlayer.HandState LeftHandRef
		{
			get
			{
				return ref this.leftHand;
			}
		}

		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x06006C13 RID: 27667 RVA: 0x0022F4C7 File Offset: 0x0022D6C7
		public GTPlayer.HandState RightHand
		{
			get
			{
				return this.rightHand;
			}
		}

		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x06006C14 RID: 27668 RVA: 0x0022F4CF File Offset: 0x0022D6CF
		public ref readonly GTPlayer.HandState RightHandRef
		{
			get
			{
				return ref this.rightHand;
			}
		}

		// Token: 0x06006C15 RID: 27669 RVA: 0x0022F4D7 File Offset: 0x0022D6D7
		public int GetMaterialTouchIndex(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).materialTouchIndex;
		}

		// Token: 0x06006C16 RID: 27670 RVA: 0x0022F4EF File Offset: 0x0022D6EF
		public GorillaSurfaceOverride GetSurfaceOverride(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).surfaceOverride;
		}

		// Token: 0x06006C17 RID: 27671 RVA: 0x0022F507 File Offset: 0x0022D707
		public RaycastHit GetTouchHitInfo(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).hitInfo;
		}

		// Token: 0x06006C18 RID: 27672 RVA: 0x0022F51F File Offset: 0x0022D71F
		public bool IsHandTouching(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).wasColliding;
		}

		// Token: 0x06006C19 RID: 27673 RVA: 0x0022F537 File Offset: 0x0022D737
		public GorillaVelocityTracker GetHandVelocityTracker(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).velocityTracker;
		}

		// Token: 0x06006C1A RID: 27674 RVA: 0x0022F54F File Offset: 0x0022D74F
		public GorillaVelocityTracker GetInteractPointVelocityTracker(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).interactPointVelocityTracker;
		}

		// Token: 0x06006C1B RID: 27675 RVA: 0x0022F567 File Offset: 0x0022D767
		public Transform GetControllerTransform(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).controllerTransform;
		}

		// Token: 0x06006C1C RID: 27676 RVA: 0x0022F57F File Offset: 0x0022D77F
		public Transform GetHandFollower(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).handFollower;
		}

		// Token: 0x06006C1D RID: 27677 RVA: 0x0022F597 File Offset: 0x0022D797
		public Vector3 GetHandOffset(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).handOffset;
		}

		// Token: 0x06006C1E RID: 27678 RVA: 0x0022F5AF File Offset: 0x0022D7AF
		public Quaternion GetHandRotOffset(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).handRotOffset;
		}

		// Token: 0x06006C1F RID: 27679 RVA: 0x0022F5C7 File Offset: 0x0022D7C7
		public Vector3 GetHandPosition(bool isLeftHand, StiltID stiltID = StiltID.None)
		{
			return ((stiltID != StiltID.None) ? this.stiltStates[(int)stiltID] : (isLeftHand ? this.leftHand : this.rightHand)).lastPosition;
		}

		// Token: 0x06006C20 RID: 27680 RVA: 0x0022F5F4 File Offset: 0x0022D7F4
		public void GetHandTapData(bool isLeftHand, StiltID stiltID, out bool wasHandTouching, out bool wasSliding, out int handMatIndex, out GorillaSurfaceOverride surfaceOverride, out RaycastHit handHitInfo, out Vector3 handPosition, out GorillaVelocityTracker handVelocityTracker)
		{
			((stiltID != StiltID.None) ? this.stiltStates[(int)stiltID] : (isLeftHand ? this.leftHand : this.rightHand)).GetHandTapData(out wasHandTouching, out wasSliding, out handMatIndex, out surfaceOverride, out handHitInfo, out handPosition, out handVelocityTracker);
		}

		// Token: 0x06006C21 RID: 27681 RVA: 0x0022F639 File Offset: 0x0022D839
		public void SetHandOffsets(bool isLeftHand, Vector3 handOffset, Quaternion handRotOffset)
		{
			if (isLeftHand)
			{
				this.leftHand.handOffset = handOffset;
				this.leftHand.handRotOffset = handRotOffset;
				return;
			}
			this.rightHand.handOffset = handOffset;
			this.rightHand.handRotOffset = handRotOffset;
		}

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06006C22 RID: 27682 RVA: 0x0022F66F File Offset: 0x0022D86F
		// (set) Token: 0x06006C23 RID: 27683 RVA: 0x0022F677 File Offset: 0x0022D877
		public Rigidbody playerRigidBody { get; private set; }

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06006C24 RID: 27684 RVA: 0x0022F680 File Offset: 0x0022D880
		public Vector3 LastPosition
		{
			get
			{
				return this.lastPosition;
			}
		}

		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x06006C25 RID: 27685 RVA: 0x0022F688 File Offset: 0x0022D888
		public Vector3 InstantaneousVelocity
		{
			get
			{
				return this.currentVelocity;
			}
		}

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06006C26 RID: 27686 RVA: 0x0022F690 File Offset: 0x0022D890
		public Vector3 AveragedVelocity
		{
			get
			{
				return this.averagedVelocity;
			}
		}

		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06006C27 RID: 27687 RVA: 0x0022F698 File Offset: 0x0022D898
		public Transform CosmeticsHeadTarget
		{
			get
			{
				return this.cosmeticsHeadTarget;
			}
		}

		// Token: 0x17000A4E RID: 2638
		// (get) Token: 0x06006C28 RID: 27688 RVA: 0x0022F6A0 File Offset: 0x0022D8A0
		public float scale
		{
			get
			{
				return this.scaleMultiplier * this.nativeScale;
			}
		}

		// Token: 0x17000A4F RID: 2639
		// (get) Token: 0x06006C29 RID: 27689 RVA: 0x0022F6AF File Offset: 0x0022D8AF
		public float NativeScale
		{
			get
			{
				return this.nativeScale;
			}
		}

		// Token: 0x17000A50 RID: 2640
		// (get) Token: 0x06006C2A RID: 27690 RVA: 0x0022F6B7 File Offset: 0x0022D8B7
		public float ScaleMultiplier
		{
			get
			{
				return this.scaleMultiplier;
			}
		}

		// Token: 0x06006C2B RID: 27691 RVA: 0x0022F6BF File Offset: 0x0022D8BF
		public void SetScaleMultiplier(float s)
		{
			this.scaleMultiplier = s;
		}

		// Token: 0x06006C2C RID: 27692 RVA: 0x0022F6C8 File Offset: 0x0022D8C8
		public void SetNativeScale(NativeSizeChangerSettings s)
		{
			float num = this.nativeScale;
			if (s != null && s.playerSizeScale > 0f && s.playerSizeScale != 1f)
			{
				this.activeSizeChangerSettings = s;
			}
			else
			{
				this.activeSizeChangerSettings = null;
			}
			if (this.activeSizeChangerSettings == null)
			{
				this.nativeScale = 1f;
			}
			else
			{
				this.nativeScale = this.activeSizeChangerSettings.playerSizeScale;
			}
			if (num != this.nativeScale && NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig != null;
			}
		}

		// Token: 0x17000A51 RID: 2641
		// (get) Token: 0x06006C2D RID: 27693 RVA: 0x0022F753 File Offset: 0x0022D953
		public bool IsDefaultScale
		{
			get
			{
				return Mathf.Abs(1f - this.scale) < 0.001f;
			}
		}

		// Token: 0x17000A52 RID: 2642
		// (get) Token: 0x06006C2E RID: 27694 RVA: 0x0022F76D File Offset: 0x0022D96D
		public bool turnedThisFrame
		{
			get
			{
				return this.degreesTurnedThisFrame != 0f;
			}
		}

		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x06006C2F RID: 27695 RVA: 0x0022F77F File Offset: 0x0022D97F
		public List<GTPlayer.MaterialData> materialData
		{
			get
			{
				return this.materialDatasSO.datas;
			}
		}

		// Token: 0x17000A54 RID: 2644
		// (get) Token: 0x06006C30 RID: 27696 RVA: 0x0022F78C File Offset: 0x0022D98C
		// (set) Token: 0x06006C31 RID: 27697 RVA: 0x0022F794 File Offset: 0x0022D994
		protected bool IsFrozen { get; set; }

		// Token: 0x17000A55 RID: 2645
		// (get) Token: 0x06006C32 RID: 27698 RVA: 0x0022F79D File Offset: 0x0022D99D
		// (set) Token: 0x06006C33 RID: 27699 RVA: 0x0022F7A5 File Offset: 0x0022D9A5
		public bool forcedUnderwater { get; set; }

		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x06006C34 RID: 27700 RVA: 0x0022F7AE File Offset: 0x0022D9AE
		// (set) Token: 0x06006C35 RID: 27701 RVA: 0x0022F7B6 File Offset: 0x0022D9B6
		public float siJumpMultiplier { get; set; } = 1f;

		// Token: 0x17000A57 RID: 2647
		// (get) Token: 0x06006C36 RID: 27702 RVA: 0x0022F7BF File Offset: 0x0022D9BF
		public List<WaterVolume> HeadOverlappingWaterVolumes
		{
			get
			{
				return this.headOverlappingWaterVolumes;
			}
		}

		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x06006C37 RID: 27703 RVA: 0x0022F7C7 File Offset: 0x0022D9C7
		public bool InWater
		{
			get
			{
				return this.bodyInWater;
			}
		}

		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x06006C38 RID: 27704 RVA: 0x0022F7CF File Offset: 0x0022D9CF
		public bool HeadInWater
		{
			get
			{
				return this.headInWater;
			}
		}

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x06006C39 RID: 27705 RVA: 0x0022F7D7 File Offset: 0x0022D9D7
		public WaterVolume CurrentWaterVolume
		{
			get
			{
				if (this.bodyOverlappingWaterVolumes.Count <= 0)
				{
					return null;
				}
				return this.bodyOverlappingWaterVolumes[0];
			}
		}

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x06006C3A RID: 27706 RVA: 0x0022F7F5 File Offset: 0x0022D9F5
		public WaterVolume.SurfaceQuery WaterSurfaceForHead
		{
			get
			{
				return this.waterSurfaceForHead;
			}
		}

		// Token: 0x17000A5C RID: 2652
		// (get) Token: 0x06006C3B RID: 27707 RVA: 0x0022F7FD File Offset: 0x0022D9FD
		public WaterVolume LeftHandWaterVolume
		{
			get
			{
				return this.leftHandWaterVolume;
			}
		}

		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x06006C3C RID: 27708 RVA: 0x0022F805 File Offset: 0x0022DA05
		public WaterVolume RightHandWaterVolume
		{
			get
			{
				return this.rightHandWaterVolume;
			}
		}

		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x06006C3D RID: 27709 RVA: 0x0022F80D File Offset: 0x0022DA0D
		public WaterVolume.SurfaceQuery LeftHandWaterSurface
		{
			get
			{
				return this.leftHandWaterSurface;
			}
		}

		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x06006C3E RID: 27710 RVA: 0x0022F815 File Offset: 0x0022DA15
		public WaterVolume.SurfaceQuery RightHandWaterSurface
		{
			get
			{
				return this.rightHandWaterSurface;
			}
		}

		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x06006C3F RID: 27711 RVA: 0x0022F81D File Offset: 0x0022DA1D
		public Vector3 LastLeftHandPosition
		{
			get
			{
				return this.leftHand.lastPosition;
			}
		}

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x06006C40 RID: 27712 RVA: 0x0022F82A File Offset: 0x0022DA2A
		public Vector3 LastRightHandPosition
		{
			get
			{
				return this.rightHand.lastPosition;
			}
		}

		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x06006C41 RID: 27713 RVA: 0x0022F837 File Offset: 0x0022DA37
		public Vector3 RigidbodyVelocity
		{
			get
			{
				return this.playerRigidBody.linearVelocity;
			}
		}

		// Token: 0x17000A63 RID: 2659
		// (get) Token: 0x06006C42 RID: 27714 RVA: 0x0022F844 File Offset: 0x0022DA44
		public Vector3 HeadCenterPosition
		{
			get
			{
				return this.headCollider.transform.position + this.headCollider.transform.rotation * new Vector3(0f, 0f, -0.11f);
			}
		}

		// Token: 0x17000A64 RID: 2660
		// (get) Token: 0x06006C43 RID: 27715 RVA: 0x0022F884 File Offset: 0x0022DA84
		public bool HandContactingSurface
		{
			get
			{
				return this.leftHand.isColliding || this.rightHand.isColliding;
			}
		}

		// Token: 0x17000A65 RID: 2661
		// (get) Token: 0x06006C44 RID: 27716 RVA: 0x0022F8A0 File Offset: 0x0022DAA0
		public bool BodyOnGround
		{
			get
			{
				return this.bodyGroundContactTime >= Time.time - 0.05f;
			}
		}

		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x06006C45 RID: 27717 RVA: 0x0022F8B8 File Offset: 0x0022DAB8
		public bool IsGroundedHand
		{
			get
			{
				return this.HandContactingSurface || this.isClimbing || this.leftHand.isHolding || this.rightHand.isHolding;
			}
		}

		// Token: 0x17000A67 RID: 2663
		// (get) Token: 0x06006C46 RID: 27718 RVA: 0x0022F8E4 File Offset: 0x0022DAE4
		public bool IsGroundedButt
		{
			get
			{
				return this.BodyOnGround;
			}
		}

		// Token: 0x17000A68 RID: 2664
		// (get) Token: 0x06006C47 RID: 27719 RVA: 0x0022F8EC File Offset: 0x0022DAEC
		// (set) Token: 0x06006C48 RID: 27720 RVA: 0x0022F8F4 File Offset: 0x0022DAF4
		public int TentacleActiveAtFrame { get; set; }

		// Token: 0x17000A69 RID: 2665
		// (get) Token: 0x06006C49 RID: 27721 RVA: 0x0022F8FD File Offset: 0x0022DAFD
		public bool IsTentacleActive
		{
			get
			{
				return this.TentacleActiveAtFrame >= Time.frameCount;
			}
		}

		// Token: 0x17000A6A RID: 2666
		// (get) Token: 0x06006C4A RID: 27722 RVA: 0x0022F90F File Offset: 0x0022DB0F
		// (set) Token: 0x06006C4B RID: 27723 RVA: 0x0022F917 File Offset: 0x0022DB17
		public int LaserZiplineActiveAtFrame { get; set; }

		// Token: 0x17000A6B RID: 2667
		// (get) Token: 0x06006C4C RID: 27724 RVA: 0x0022F920 File Offset: 0x0022DB20
		public bool IsLaserZiplineActive
		{
			get
			{
				return this.LaserZiplineActiveAtFrame >= Time.frameCount;
			}
		}

		// Token: 0x17000A6C RID: 2668
		// (get) Token: 0x06006C4D RID: 27725 RVA: 0x0022F932 File Offset: 0x0022DB32
		// (set) Token: 0x06006C4E RID: 27726 RVA: 0x0022F93A File Offset: 0x0022DB3A
		public int ThrusterActiveAtFrame { get; set; }

		// Token: 0x17000A6D RID: 2669
		// (get) Token: 0x06006C4F RID: 27727 RVA: 0x0022F943 File Offset: 0x0022DB43
		public bool IsThrusterActive
		{
			get
			{
				return this.ThrusterActiveAtFrame >= Time.frameCount;
			}
		}

		// Token: 0x17000A6E RID: 2670
		// (set) Token: 0x06006C50 RID: 27728 RVA: 0x0022F955 File Offset: 0x0022DB55
		public Quaternion PlayerRotationOverride
		{
			set
			{
				this.playerRotationOverride = value;
				this.playerRotationOverrideFrame = Time.frameCount;
			}
		}

		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x06006C51 RID: 27729 RVA: 0x0022F969 File Offset: 0x0022DB69
		// (set) Token: 0x06006C52 RID: 27730 RVA: 0x0022F971 File Offset: 0x0022DB71
		public bool IsBodySliding { get; set; }

		// Token: 0x17000A70 RID: 2672
		// (get) Token: 0x06006C53 RID: 27731 RVA: 0x0022F97A File Offset: 0x0022DB7A
		// (set) Token: 0x06006C54 RID: 27732 RVA: 0x0022F982 File Offset: 0x0022DB82
		public bool bodyGroundIsSlippery { get; private set; }

		// Token: 0x17000A71 RID: 2673
		// (get) Token: 0x06006C55 RID: 27733 RVA: 0x0022F98B File Offset: 0x0022DB8B
		public GorillaClimbable CurrentClimbable
		{
			get
			{
				return this.currentClimbable;
			}
		}

		// Token: 0x17000A72 RID: 2674
		// (get) Token: 0x06006C56 RID: 27734 RVA: 0x0022F993 File Offset: 0x0022DB93
		public GorillaHandClimber CurrentClimber
		{
			get
			{
				return this.currentClimber;
			}
		}

		// Token: 0x17000A73 RID: 2675
		// (get) Token: 0x06006C57 RID: 27735 RVA: 0x0022F99B File Offset: 0x0022DB9B
		// (set) Token: 0x06006C58 RID: 27736 RVA: 0x0022F9A3 File Offset: 0x0022DBA3
		public float jumpMultiplier
		{
			get
			{
				return this._jumpMultiplier;
			}
			set
			{
				this._jumpMultiplier = value;
			}
		}

		// Token: 0x17000A74 RID: 2676
		// (get) Token: 0x06006C59 RID: 27737 RVA: 0x0022F9AC File Offset: 0x0022DBAC
		// (set) Token: 0x06006C5A RID: 27738 RVA: 0x0022F9B4 File Offset: 0x0022DBB4
		public float LastTouchedGroundAtNetworkTime { get; private set; }

		// Token: 0x17000A75 RID: 2677
		// (get) Token: 0x06006C5B RID: 27739 RVA: 0x0022F9BD File Offset: 0x0022DBBD
		// (set) Token: 0x06006C5C RID: 27740 RVA: 0x0022F9C5 File Offset: 0x0022DBC5
		public float LastHandTouchedGroundAtNetworkTime { get; private set; }

		// Token: 0x06006C5D RID: 27741 RVA: 0x0022F9D0 File Offset: 0x0022DBD0
		public void EnableStilt(StiltID stiltID, bool isLeftHand, Vector3 currentTipWorldPos, float maxArmLength, bool canTag, bool canStun, float customBoostFactor = 0f, GorillaVelocityTracker velocityTracker = null)
		{
			this.stiltStates[(int)stiltID] = new GTPlayer.HandState
			{
				isActive = true,
				controllerTransform = (isLeftHand ? this.leftHand : this.rightHand).controllerTransform,
				velocityTracker = ((velocityTracker != null) ? velocityTracker : (isLeftHand ? this.leftHand : this.rightHand).velocityTracker),
				handRotOffset = Quaternion.identity,
				canTag = canTag,
				canStun = canStun,
				customBoostFactor = customBoostFactor,
				hasCustomBoost = (customBoostFactor > 0f)
			};
			this.stiltStates[(int)stiltID].Init(this, isLeftHand, maxArmLength);
			this.UpdateStiltOffset(stiltID, currentTipWorldPos);
		}

		// Token: 0x06006C5E RID: 27742 RVA: 0x0022FA96 File Offset: 0x0022DC96
		public void DisableStilt(StiltID stiltID)
		{
			this.stiltStates[(int)stiltID].isActive = false;
		}

		// Token: 0x06006C5F RID: 27743 RVA: 0x0022FAAA File Offset: 0x0022DCAA
		public void UpdateStiltOffset(StiltID stiltID, Vector3 currentTipWorldPos)
		{
			this.stiltStates[(int)stiltID].handOffset = this.stiltStates[(int)stiltID].controllerTransform.InverseTransformPoint(currentTipWorldPos);
		}

		// Token: 0x06006C60 RID: 27744 RVA: 0x0022FAD4 File Offset: 0x0022DCD4
		private void Awake()
		{
			if (GTPlayer._instance != null && GTPlayer._instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				GTPlayer._instance = this;
				GTPlayer.hasInstance = true;
			}
			this.InitializeValues();
			this.playerRigidbodyInterpolationDefault = this.playerRigidBody.interpolation;
			this.playerRigidBody.maxAngularVelocity = 0f;
			this.bodyOffsetVector = new Vector3(0f, -this.bodyCollider.height / 2f, 0f);
			this._bodyInitialHeight = this.bodyCollider.height;
			this.bodyInitialRadius = this.bodyCollider.radius;
			this.rayCastNonAllocColliders = new RaycastHit[5];
			this.crazyCheckVectors = new Vector3[7];
			this.emptyHit = default(RaycastHit);
			this.crazyCheckVectors[0] = Vector3.up;
			this.crazyCheckVectors[1] = Vector3.down;
			this.crazyCheckVectors[2] = Vector3.left;
			this.crazyCheckVectors[3] = Vector3.right;
			this.crazyCheckVectors[4] = Vector3.forward;
			this.crazyCheckVectors[5] = Vector3.back;
			this.crazyCheckVectors[6] = Vector3.zero;
			if (this.controllerState == null)
			{
				this.controllerState = base.GetComponent<ConnectedControllerHandler>();
			}
			this.layerChanger = base.GetComponent<LayerChanger>();
			this.bodyTouchedSurfaces = new Dictionary<GameObject, PhysicsMaterial>();
			if (Application.isPlaying)
			{
				Application.onBeforeRender += this.OnBeforeRenderInit;
			}
		}

		// Token: 0x06006C61 RID: 27745 RVA: 0x0022FC68 File Offset: 0x0022DE68
		protected void Start()
		{
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
			}
			this.mainCamera.farClipPlane = 500f;
			this.lastScale = this.scale;
			this.layerChanger.InitializeLayers(base.transform);
			float degrees = Quaternion.Angle(Quaternion.identity, GorillaTagger.Instance.offlineVRRig.transform.rotation) * Mathf.Sign(Vector3.Dot(Vector3.up, GorillaTagger.Instance.offlineVRRig.transform.right));
			this.Turn(degrees);
		}

		// Token: 0x06006C62 RID: 27746 RVA: 0x0022FD05 File Offset: 0x0022DF05
		protected void OnDestroy()
		{
			if (GTPlayer._instance == this)
			{
				GTPlayer._instance = null;
				GTPlayer.hasInstance = false;
			}
			if (this.climbHelper)
			{
				Object.Destroy(this.climbHelper.gameObject);
			}
		}

		// Token: 0x06006C63 RID: 27747 RVA: 0x0022FD40 File Offset: 0x0022DF40
		public void InitializeValues()
		{
			Physics.SyncTransforms();
			this.playerRigidBody = base.GetComponent<Rigidbody>();
			this.velocityHistory = new Vector3[this.velocityHistorySize];
			this.slideAverageHistory = new Vector3[this.velocityHistorySize];
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Vector3.zero;
				this.slideAverageHistory[i] = Vector3.zero;
			}
			this.leftHand.Init(this, true, this.maxArmLength);
			this.rightHand.Init(this, false, this.maxArmLength);
			this.lastHeadPosition = this.headCollider.transform.position;
			this.velocityIndex = 0;
			this.averagedVelocity = Vector3.zero;
			this.slideVelocity = Vector3.zero;
			this.lastPosition = base.transform.position;
			this.lastRealTime = Time.realtimeSinceStartup;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
			this.ForceRigidBodySync();
		}

		// Token: 0x06006C64 RID: 27748 RVA: 0x0022FEAC File Offset: 0x0022E0AC
		public void SetHalloweenLevitation(float levitateStrength, float levitateDuration, float levitateBlendOutDuration, float levitateBonusStrength, float levitateBonusOffAtYSpeed, float levitateBonusFullAtYSpeed)
		{
			this.halloweenLevitationStrength = levitateStrength;
			this.halloweenLevitationFullStrengthDuration = levitateDuration;
			this.halloweenLevitationTotalDuration = levitateDuration + levitateBlendOutDuration;
			this.halloweenLevitateBonusFullAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitateBonusOffAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitationBonusStrength = levitateBonusStrength;
		}

		// Token: 0x06006C65 RID: 27749 RVA: 0x0022FEDD File Offset: 0x0022E0DD
		public void TeleportToTrain(bool enable)
		{
			this.teleportToTrain = enable;
		}

		// Token: 0x06006C66 RID: 27750 RVA: 0x0022FEE8 File Offset: 0x0022E0E8
		public void TeleportTo(Vector3 position, Quaternion rotation, bool keepVelocity = false, bool center = false)
		{
			if (center)
			{
				Vector3 position2 = base.transform.position;
				Vector3 b = this.mainCamera.transform.position - position2;
				position -= b;
			}
			this.ClearHandHolds();
			if (this.playerRigidBody != null)
			{
				this.playerRigidBody.isKinematic = true;
				this.playerRigidBody.position = position;
				this.playerRigidBody.rotation = rotation;
				this.playerRigidBody.isKinematic = false;
			}
			this.playerRigidBody.position = position;
			this.playerRigidBody.rotation = rotation;
			base.transform.position = position;
			base.transform.rotation = rotation;
			this.lastHeadPosition = this.headCollider.transform.position;
			this.lastPosition = position;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.leftHand.OnTeleport();
			this.rightHand.OnTeleport();
			for (int i = 0; i < 12; i++)
			{
				if (this.stiltStates[i].isActive)
				{
					this.stiltStates[i].OnTeleport();
				}
			}
			if (!keepVelocity)
			{
				this.playerRigidBody.linearVelocity = Vector3.zero;
			}
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
			Physics.SyncTransforms();
			GorillaTagger.Instance.offlineVRRig.transform.position = position;
			GorillaTagger.Instance.offlineVRRig.leftHandLink.BreakLink();
			GorillaTagger.Instance.offlineVRRig.rightHandLink.BreakLink();
			this.ForceRigidBodySync();
		}

		// Token: 0x06006C67 RID: 27751 RVA: 0x002300D4 File Offset: 0x0022E2D4
		public void TeleportTo(Transform destination, bool matchDestinationRotation = true, bool maintainVelocity = true)
		{
			Vector3 position = base.transform.position;
			Vector3 b = this.mainCamera.transform.position - position;
			Vector3 position2 = destination.position - b;
			float num = destination.rotation.eulerAngles.y - this.mainCamera.transform.rotation.eulerAngles.y;
			Vector3 playerVelocity = this.currentVelocity;
			if (!maintainVelocity)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			else if (matchDestinationRotation)
			{
				playerVelocity = Quaternion.AngleAxis(num, base.transform.up) * this.currentVelocity;
				this.SetPlayerVelocity(playerVelocity);
			}
			if (matchDestinationRotation)
			{
				this.Turn(num);
			}
			this.TeleportTo(position2, base.transform.rotation, false, false);
			if (maintainVelocity)
			{
				this.SetPlayerVelocity(playerVelocity);
			}
			this.ForceRigidBodySync();
		}

		// Token: 0x06006C68 RID: 27752 RVA: 0x002301B5 File Offset: 0x0022E3B5
		public void AddForce(Vector3 force, ForceMode mode)
		{
			if (mode == ForceMode.VelocityChange)
			{
				this.playerRigidBody.AddForce(force * this.playerRigidBody.mass, ForceMode.Impulse);
				return;
			}
			this.playerRigidBody.AddForce(force, mode);
		}

		// Token: 0x06006C69 RID: 27753 RVA: 0x002301E8 File Offset: 0x0022E3E8
		public void SetPlayerVelocity(Vector3 newVelocity)
		{
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = newVelocity;
			}
			this.playerRigidBody.AddForce(newVelocity - this.playerRigidBody.linearVelocity, ForceMode.VelocityChange);
		}

		// Token: 0x17000A76 RID: 2678
		// (get) Token: 0x06006C6A RID: 27754 RVA: 0x00230232 File Offset: 0x0022E432
		public int GravityOverrideCount
		{
			get
			{
				return this.gravityOverrides.Count;
			}
		}

		// Token: 0x06006C6B RID: 27755 RVA: 0x0023023F File Offset: 0x0022E43F
		public void SetGravityOverride(Object caller, Action<GTPlayer> gravityFunction)
		{
			this.gravityOverrides[caller] = gravityFunction;
		}

		// Token: 0x06006C6C RID: 27756 RVA: 0x0023024E File Offset: 0x0022E44E
		public void UnsetGravityOverride(Object caller)
		{
			this.gravityOverrides.Remove(caller);
		}

		// Token: 0x06006C6D RID: 27757 RVA: 0x00230260 File Offset: 0x0022E460
		private void ApplyGravityOverrides()
		{
			foreach (KeyValuePair<Object, Action<GTPlayer>> keyValuePair in this.gravityOverrides)
			{
				keyValuePair.Value(this);
			}
		}

		// Token: 0x06006C6E RID: 27758 RVA: 0x002302BC File Offset: 0x0022E4BC
		public void ApplyKnockback(Vector3 direction, float speed, bool forceOffTheGround = false)
		{
			if (forceOffTheGround)
			{
				if (this.leftHand.wasColliding || this.rightHand.wasColliding)
				{
					this.leftHand.wasColliding = false;
					this.rightHand.wasColliding = false;
					this.playerRigidBody.transform.position += this.minimumRaycastDistance * this.scale * Vector3.up;
				}
				this.didAJump = true;
				this.SetMaximumSlipThisFrame();
			}
			if (speed > 0.01f)
			{
				float num = Vector3.Dot(this.averagedVelocity, direction);
				float d = Mathf.InverseLerp(1.5f, 0.5f, num / speed);
				Vector3 vector = this.averagedVelocity + direction * speed * d;
				this.playerRigidBody.linearVelocity = vector;
				for (int i = 0; i < this.velocityHistory.Length; i++)
				{
					this.velocityHistory[i] = vector;
				}
			}
		}

		// Token: 0x06006C6F RID: 27759 RVA: 0x002303AC File Offset: 0x0022E5AC
		public void ApplyClampedKnockback(Vector3 direction, float speed, float boostMultiplier, bool forceOffTheGround = false)
		{
			if (forceOffTheGround)
			{
				if (this.leftHand.wasColliding || this.rightHand.wasColliding)
				{
					this.leftHand.wasColliding = false;
					this.rightHand.wasColliding = false;
					this.playerRigidBody.transform.position += this.minimumRaycastDistance * this.scale * Vector3.up;
				}
				this.didAJump = true;
				this.SetMaximumSlipThisFrame();
			}
			if (speed > 0.01f)
			{
				float num = Vector3.Dot(this.playerRigidBody.linearVelocity, direction.normalized);
				if (num >= speed)
				{
					return;
				}
				float d = Mathf.Clamp(speed - num, 0f, speed * boostMultiplier);
				Vector3 vector = this.playerRigidBody.linearVelocity + direction.normalized * d;
				this.playerRigidBody.linearVelocity = vector;
				for (int i = 0; i < this.velocityHistory.Length; i++)
				{
					this.velocityHistory[i] = vector;
				}
			}
		}

		// Token: 0x06006C70 RID: 27760 RVA: 0x002304B0 File Offset: 0x0022E6B0
		public void FixedUpdate()
		{
			this.AntiTeleportTechnology();
			this.IsFrozen = (GorillaTagger.Instance.offlineVRRig.IsFrozen || this.debugFreezeTag);
			bool isDefaultScale = this.IsDefaultScale;
			this.playerRigidBody.useGravity = false;
			if (this.gravityOverrides.Count > 0)
			{
				this.ApplyGravityOverrides();
			}
			else if (this.halloweenLevitationBonusStrength > 0f || this.halloweenLevitationStrength > 0f)
			{
				float num = Time.time - this.lastTouchedGroundTimestamp;
				if (num < this.halloweenLevitationTotalDuration)
				{
					this.playerRigidBody.AddForce(Vector3.up * (this.halloweenLevitationStrength * Mathf.InverseLerp(this.halloweenLevitationFullStrengthDuration, this.halloweenLevitationTotalDuration, num)), ForceMode.Acceleration);
				}
				float y = this.playerRigidBody.linearVelocity.y;
				if (y <= this.halloweenLevitateBonusFullAtYSpeed)
				{
					this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength, ForceMode.Acceleration);
				}
				else if (y <= this.halloweenLevitateBonusOffAtYSpeed)
				{
					float num2 = Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.linearVelocity.y);
					this.playerRigidBody.AddForce(Vector3.up * (this.halloweenLevitationBonusStrength * num2), ForceMode.Acceleration);
				}
			}
			if (this.enableHoverMode)
			{
				this.playerRigidBody.linearVelocity = this.HoverboardFixedUpdate(this.playerRigidBody.linearVelocity);
			}
			else
			{
				this.didHoverLastFrame = false;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.bodyInWater = false;
			Vector3 lhs = this.swimmingVelocity;
			this.swimmingVelocity = Vector3.MoveTowards(this.swimmingVelocity, Vector3.zero, this.swimmingParams.swimmingVelocityOutOfWaterDrainRate * fixedDeltaTime);
			this.leftHandNonDiveHapticsAmount = 0f;
			this.rightHandNonDiveHapticsAmount = 0f;
			if (this.bodyOverlappingWaterVolumes.Count > 0 || this.forcedUnderwater)
			{
				WaterVolume waterVolume = null;
				float num3 = float.MinValue;
				Vector3 vector = this.headCollider.transform.position + GTPlayerTransform.PhysicsDown * this.swimmingParams.floatingWaterLevelBelowHead * this.scale;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.bodyOverlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery;
					if (this.bodyOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector, out surfaceQuery, false))
					{
						float num4 = Vector3.Dot(surfaceQuery.surfacePoint - vector, surfaceQuery.surfaceNormal);
						if (num4 > num3)
						{
							num3 = num4;
							waterVolume = this.bodyOverlappingWaterVolumes[i];
							this.waterSurfaceForHead = surfaceQuery;
						}
						WaterCurrent waterCurrent = this.bodyOverlappingWaterVolumes[i].Current;
						if (waterCurrent != null && num4 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (this.forcedUnderwater && waterVolume == null)
				{
					this.waterSurfaceForHead = new WaterVolume.SurfaceQuery
					{
						surfacePoint = this.headCollider.transform.position + GTPlayerTransform.PhysicsUp * 1000f,
						surfaceNormal = GTPlayerTransform.PhysicsUp,
						maxDepth = 2000f
					};
					num3 = 1000f;
				}
				if (waterVolume != null || this.forcedUnderwater)
				{
					Vector3 linearVelocity = this.playerRigidBody.linearVelocity;
					float magnitude = linearVelocity.magnitude;
					bool flag = this.headInWater;
					float num5 = Vector3.Dot(this.waterSurfaceForHead.surfacePoint - this.headCollider.transform.position, this.waterSurfaceForHead.surfaceNormal);
					float num6 = Vector3.Dot(this.headCollider.transform.position - (this.waterSurfaceForHead.surfacePoint - this.waterSurfaceForHead.surfaceNormal * this.waterSurfaceForHead.maxDepth), this.waterSurfaceForHead.surfaceNormal);
					this.headInWater = ((this.forcedUnderwater || (num5 > 0f && num6 > 0f)) && waterVolume.LiquidType != GTPlayer.LiquidType.SwimInAir);
					if (this.headInWater && !flag)
					{
						this.audioSetToUnderwater = true;
						this.audioManager.SetMixerSnapshot(this.audioManager.underwaterSnapshot, 0.1f);
					}
					else if (!this.headInWater && flag)
					{
						this.audioSetToUnderwater = false;
						this.audioManager.UnsetMixerSnapshot(0.1f);
					}
					float num7 = Vector3.Dot(this.waterSurfaceForHead.surfacePoint - vector, this.waterSurfaceForHead.surfaceNormal);
					float num8 = Vector3.Dot(vector - (this.waterSurfaceForHead.surfacePoint - this.waterSurfaceForHead.surfaceNormal * this.waterSurfaceForHead.maxDepth), this.waterSurfaceForHead.surfaceNormal);
					this.bodyInWater = (this.forcedUnderwater || (num7 > 0f && num8 > 0f));
					if (this.bodyInWater)
					{
						GTPlayer.LiquidProperties liquidProperties = this.liquidPropertiesList[(int)((waterVolume != null) ? waterVolume.LiquidType : GTPlayer.LiquidType.Water)];
						float num10;
						if (this.swimmingParams.extendBouyancyFromSpeed)
						{
							float time = Mathf.Clamp(Vector3.Dot(linearVelocity / this.scale, this.waterSurfaceForHead.surfaceNormal), this.swimmingParams.speedToBouyancyExtensionMinMax.x, this.swimmingParams.speedToBouyancyExtensionMinMax.y);
							float b = this.swimmingParams.speedToBouyancyExtension.Evaluate(time);
							this.buoyancyExtension = Mathf.Max(this.buoyancyExtension, b);
							float num9 = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist + this.buoyancyExtension, num3 / this.scale + this.buoyancyExtension);
							this.buoyancyExtension = Spring.DamperDecayExact(this.buoyancyExtension, this.swimmingParams.buoyancyExtensionDecayHalflife, fixedDeltaTime, 1E-05f);
							num10 = num9;
						}
						else
						{
							num10 = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist, num3 / this.scale);
						}
						Vector3 vector2 = -(GTPlayerTransform.PhysicsDown * Physics.gravity.magnitude * this.scale) * (liquidProperties.buoyancy * num10);
						if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
						{
							vector2 *= this.frozenBodyBuoyancyFactor;
						}
						this.playerRigidBody.AddForce(vector2, ForceMode.Acceleration);
						Vector3 vector3 = Vector3.zero;
						Vector3 vector4 = Vector3.zero;
						for (int j = 0; j < this.activeWaterCurrents.Count; j++)
						{
							WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
							Vector3 startingVelocity = linearVelocity + vector3;
							Vector3 b2;
							Vector3 b3;
							if (waterCurrent2.GetCurrentAtPoint(this.bodyCollider.transform.position, startingVelocity, fixedDeltaTime, out b2, out b3))
							{
								vector4 += b2;
								vector3 += b3;
							}
						}
						if (magnitude > Mathf.Epsilon)
						{
							float num11 = 0.01f;
							Vector3 vector5 = linearVelocity / magnitude;
							Vector3 right = this.leftHand.handFollower.right;
							Vector3 dir = -this.rightHand.handFollower.right;
							Vector3 forward = this.leftHand.handFollower.forward;
							Vector3 forward2 = this.rightHand.handFollower.forward;
							Vector3 a = vector5;
							float num12 = 0f;
							float num13 = 0f;
							float num14 = 0f;
							if (this.swimmingParams.applyDiveSteering && !this.disableMovement && isDefaultScale)
							{
								float value = Vector3.Dot(linearVelocity - vector4, vector5);
								float time2 = Mathf.Clamp(value, this.swimmingParams.swimSpeedToRedirectAmountMinMax.x, this.swimmingParams.swimSpeedToRedirectAmountMinMax.y);
								float b4 = this.swimmingParams.swimSpeedToRedirectAmount.Evaluate(time2);
								time2 = Mathf.Clamp(value, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.x, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.y);
								float num15 = this.swimmingParams.swimSpeedToMaxRedirectAngle.Evaluate(time2);
								float value2 = Mathf.Acos(Vector3.Dot(vector5, forward)) / 3.1415927f * -2f + 1f;
								float value3 = Mathf.Acos(Vector3.Dot(vector5, forward2)) / 3.1415927f * -2f + 1f;
								float num16 = Mathf.Clamp(value2, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float num17 = Mathf.Clamp(value3, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float a2 = (!float.IsNaN(num16)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num16) : 0f;
								float a3 = (!float.IsNaN(num17)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num17) : 0f;
								Vector3 a4 = Vector3.ProjectOnPlane(vector5, right);
								Vector3 a5 = Vector3.ProjectOnPlane(vector5, right);
								float num18 = Mathf.Min(a4.magnitude, 1f);
								float num19 = Mathf.Min(a5.magnitude, 1f);
								float magnitude2 = this.leftHand.velocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float magnitude3 = this.rightHand.velocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float time3 = Mathf.Clamp(magnitude2, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float time4 = Mathf.Clamp(magnitude3, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float a6 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(time3);
								float a7 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(time4);
								float averageSpeedChangeMagnitudeInDirection = this.leftHand.velocityTracker.GetAverageSpeedChangeMagnitudeInDirection(right, false, this.swimmingParams.diveVelocityAveragingWindow);
								float averageSpeedChangeMagnitudeInDirection2 = this.rightHand.velocityTracker.GetAverageSpeedChangeMagnitudeInDirection(dir, false, this.swimmingParams.diveVelocityAveragingWindow);
								float time5 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float time6 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection2, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float b5 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(time5);
								float b6 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(time6);
								num12 = Mathf.Min(a2, Mathf.Min(a6, b5));
								float num20 = (Vector3.Dot(vector5, forward) > 0f) ? (Mathf.Min(num12, b4) * num18) : 0f;
								num13 = Mathf.Min(a3, Mathf.Min(a7, b6));
								float num21 = (Vector3.Dot(vector5, forward2) > 0f) ? (Mathf.Min(num13, b4) * num19) : 0f;
								if (this.swimmingParams.reduceDiveSteeringBelowVelocityPlane)
								{
									Vector3 rhs;
									if (Vector3.Dot(this.headCollider.transform.up, vector5) > 0.95f)
									{
										rhs = -this.headCollider.transform.forward;
									}
									else
									{
										rhs = Vector3.Cross(Vector3.Cross(vector5, this.headCollider.transform.up), vector5).normalized;
									}
									Vector3 position = this.headCollider.transform.position;
									Vector3 lhs2 = position - this.leftHand.handFollower.position;
									Vector3 lhs3 = position - this.rightHand.handFollower.position;
									float reduceDiveSteeringBelowPlaneFadeStartDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeStartDist;
									float reduceDiveSteeringBelowPlaneFadeEndDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeEndDist;
									float f = Vector3.Dot(lhs2, GTPlayerTransform.PhysicsUp);
									float f2 = Vector3.Dot(lhs3, GTPlayerTransform.PhysicsUp);
									float f3 = Vector3.Dot(lhs2, rhs);
									float f4 = Vector3.Dot(lhs3, rhs);
									float num22 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(f), Mathf.Abs(f3)));
									float num23 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(f2), Mathf.Abs(f4)));
									num20 *= num22;
									num21 *= num23;
								}
								float num24 = num21 + num20;
								Vector3 vector6 = Vector3.zero;
								if (this.swimmingParams.applyDiveSteering && num24 > num11)
								{
									vector6 = ((num20 * a4 + num21 * a5) / num24).normalized;
									vector6 = Vector3.Lerp(vector5, vector6, num24);
									a = Vector3.RotateTowards(vector5, vector6, 0.017453292f * num15 * fixedDeltaTime, 0f);
								}
								else
								{
									a = vector5;
								}
								num14 = Mathf.Clamp01((num12 + num13) * 0.5f);
							}
							float num25 = Mathf.Clamp(Vector3.Dot(lhs, vector5), 0f, magnitude);
							float num26 = magnitude - num25;
							if (this.swimmingParams.applyDiveSwimVelocityConversion && !this.disableMovement && num14 > num11 && num25 < this.swimmingParams.diveMaxSwimVelocityConversion)
							{
								float num27 = Mathf.Min(this.swimmingParams.diveSwimVelocityConversionRate * fixedDeltaTime, num26) * num14;
								num25 += num27;
								num26 -= num27;
							}
							float halflife = this.swimmingParams.swimUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float halflife2 = this.swimmingParams.baseUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float num28 = Spring.DamperDecayExact(num25 / this.scale, halflife, fixedDeltaTime, 1E-05f) * this.scale;
							float num29 = Spring.DamperDecayExact(num26 / this.scale, halflife2, fixedDeltaTime, 1E-05f) * this.scale;
							if (this.swimmingParams.applyDiveDampingMultiplier && !this.disableMovement)
							{
								float t = Mathf.Lerp(1f, this.swimmingParams.diveDampingMultiplier, num14);
								num28 = Mathf.Lerp(num25, num28, t);
								num29 = Mathf.Lerp(num26, num29, t);
								float time7 = Mathf.Clamp((1f - num12) * (num25 + num26), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num11, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num11);
								float time8 = Mathf.Clamp((1f - num13) * (num25 + num26), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num11, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num11);
								this.leftHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time7);
								this.rightHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time8);
							}
							this.swimmingVelocity = num28 * a + vector3 * this.scale;
							this.playerRigidBody.linearVelocity = this.swimmingVelocity + num29 * a;
						}
					}
				}
			}
			else if (this.audioSetToUnderwater)
			{
				this.audioSetToUnderwater = false;
				this.audioManager.UnsetMixerSnapshot(0.1f);
			}
			this.handleClimbing(Time.fixedDeltaTime);
			this.stuckHandsCheckFixedUpdate();
			this.FixedUpdate_HandHolds(Time.fixedDeltaTime);
		}

		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x06006C71 RID: 27761 RVA: 0x00231402 File Offset: 0x0022F602
		// (set) Token: 0x06006C72 RID: 27762 RVA: 0x0023140A File Offset: 0x0022F60A
		public bool isHoverAllowed { get; private set; }

		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x06006C73 RID: 27763 RVA: 0x00231413 File Offset: 0x0022F613
		// (set) Token: 0x06006C74 RID: 27764 RVA: 0x0023141B File Offset: 0x0022F61B
		public bool enableHoverMode { get; private set; }

		// Token: 0x06006C75 RID: 27765 RVA: 0x00231424 File Offset: 0x0022F624
		public void SetHoverboardPosRot(Vector3 worldPos, Quaternion worldRot)
		{
			this.hoverboardPlayerLocalPos = this.headCollider.transform.InverseTransformPoint(worldPos);
			this.hoverboardPlayerLocalRot = this.headCollider.transform.InverseTransformRotation(worldRot);
		}

		// Token: 0x06006C76 RID: 27766 RVA: 0x00231454 File Offset: 0x0022F654
		private void HoverboardLateUpdate()
		{
			Vector3 eulerAngles = this.headCollider.transform.eulerAngles;
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				RaycastHit raycastHit;
				hoverBoardCast.didHit = Physics.SphereCast(new Ray(this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin), this.hoverboardVisual.transform.rotation * hoverBoardCast.localDirection), hoverBoardCast.sphereRadius, out raycastHit, hoverBoardCast.distance, this.locomotionEnabledLayers);
				if (hoverBoardCast.didHit)
				{
					HoverboardCantHover hoverboardCantHover;
					if (raycastHit.collider.TryGetComponent<HoverboardCantHover>(out hoverboardCantHover))
					{
						hoverBoardCast.didHit = false;
					}
					else
					{
						hoverBoardCast.pointHit = raycastHit.point;
						hoverBoardCast.normalHit = raycastHit.normal;
					}
				}
				this.hoverboardCasts[i] = hoverBoardCast;
				if (hoverBoardCast.didHit)
				{
					flag = true;
				}
			}
			this.hasHoverPoint = flag;
			this.bodyCollider.enabled = (this.bodyCollider.transform.position - this.hoverboardVisual.transform.TransformPoint(GTPlayerTransform.Up * this.hoverBodyCollisionRadiusUpOffset)).IsLongerThan(this.hoverBodyHasCollisionsOutsideRadius);
		}

		// Token: 0x06006C77 RID: 27767 RVA: 0x0023159C File Offset: 0x0022F79C
		private Vector3 HoverboardFixedUpdate(Vector3 velocity)
		{
			this.hoverboardVisual.transform.position = this.headCollider.transform.TransformPoint(this.hoverboardPlayerLocalPos);
			this.hoverboardVisual.transform.rotation = this.headCollider.transform.TransformRotation(this.hoverboardPlayerLocalRot);
			if (this.didHoverLastFrame)
			{
				velocity += Vector3.up * this.hoverGeneralUpwardForce * Time.fixedDeltaTime;
			}
			Vector3 position = this.hoverboardVisual.transform.position;
			Vector3 a = position + velocity * Time.fixedDeltaTime;
			Vector3 vector = this.hoverboardVisual.transform.forward;
			Vector3 vector2 = this.hoverboardCasts[0].didHit ? this.hoverboardCasts[0].normalHit : Vector3.up;
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				if (hoverBoardCast.didHit)
				{
					Vector3 b = position + Vector3.Project(hoverBoardCast.pointHit - position, vector);
					Vector3 b2 = a + Vector3.Project(hoverBoardCast.pointHit - position, vector);
					bool flag2 = hoverBoardCast.isSolid || Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - b2) + this.hoverIdealHeight > 0f;
					float d = hoverBoardCast.isSolid ? (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin + hoverBoardCast.localDirection * hoverBoardCast.distance)) + hoverBoardCast.sphereRadius) : (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - b) + this.hoverIdealHeight);
					if (flag2)
					{
						flag = true;
						this.boostEnabledUntilTimestamp = Time.time + this.hoverboardBoostGracePeriod;
						if (Vector3.Dot(velocity, hoverBoardCast.normalHit) < 0f)
						{
							velocity = Vector3.ProjectOnPlane(velocity, hoverBoardCast.normalHit);
						}
						this.playerRigidBody.transform.position += hoverBoardCast.normalHit * d;
						Vector3 vector3 = this.turnParent.transform.rotation * (this.hoverboardVisual.IsLeftHanded ? this.leftHand.velocityTracker : this.rightHand.velocityTracker).GetAverageVelocity(false, 0.15f, false);
						if (Vector3.Dot(vector3, hoverBoardCast.normalHit) < 0f)
						{
							velocity -= Vector3.Project(vector3, hoverBoardCast.normalHit) * this.hoverSlamJumpStrengthFactor * Time.fixedDeltaTime;
						}
						a = position + velocity * Time.fixedDeltaTime;
					}
				}
			}
			float time = Mathf.Abs(Mathf.DeltaAngle(0f, Mathf.Acos(Vector3.Dot(this.hoverboardVisual.transform.up, Vector3.ProjectOnPlane(vector2, vector).normalized)) * 57.29578f));
			float num = this.hoverCarveAngleResponsiveness.Evaluate(time);
			vector = (vector + Vector3.ProjectOnPlane(this.hoverboardVisual.transform.up, vector2) * this.hoverTiltAdjustsForwardFactor).normalized;
			if (!flag)
			{
				this.didHoverLastFrame = false;
				num = 0f;
			}
			Vector3 b3 = velocity;
			if (this.enableHoverMode && this.hasHoverPoint)
			{
				Vector3 vector4 = Vector3.ProjectOnPlane(velocity, vector2);
				Vector3 b4 = velocity - vector4;
				Vector3 vector5 = Vector3.Project(vector4, vector);
				float num2 = vector4.magnitude;
				if (num2 <= this.hoveringSlowSpeed)
				{
					num2 *= this.hoveringSlowStoppingFactor;
				}
				Vector3 vector6 = vector4 - vector5;
				float num3 = 0f;
				bool flag3 = false;
				if (num > 0f)
				{
					if (vector6.IsLongerThan(vector5))
					{
						num3 = Mathf.Min((vector6.magnitude - vector5.magnitude) * this.hoverCarveSidewaysSpeedLossFactor * num, num2);
						if (num3 > 0f && num2 > this.hoverMinGrindSpeed)
						{
							flag3 = true;
							this.hoverboardVisual.PlayGrindHaptic();
						}
						num2 -= num3;
					}
					vector6 *= 1f - num * this.sidewaysDrag;
					if (!this.leftHand.isColliding && !this.rightHand.isColliding)
					{
						velocity = (vector5 + vector6).normalized * num2 + b4;
					}
				}
				else
				{
					velocity = vector4.normalized * num2 + b4;
				}
				float magnitude = (velocity - b3).magnitude;
				this.hoverboardAudio.UpdateAudioLoop(velocity.magnitude, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, magnitude, flag3 ? num3 : 0f);
				if (magnitude > 0f && !flag3)
				{
					this.hoverboardVisual.PlayCarveHaptic(magnitude);
				}
			}
			else
			{
				this.hoverboardAudio.UpdateAudioLoop(0f, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, 0f, 0f);
			}
			return velocity;
		}

		// Token: 0x06006C78 RID: 27768 RVA: 0x00231B0C File Offset: 0x0022FD0C
		public void GrabPersonalHoverboard(bool isLeftHand, Vector3 pos, Quaternion rot, Color col)
		{
			if (this.hoverboardVisual.IsHeld)
			{
				this.hoverboardVisual.DropFreeBoard();
			}
			this.hoverboardVisual.SetIsHeld(isLeftHand, pos, rot, col);
			this.hoverboardVisual.ProxyGrabHandle(isLeftHand);
			FreeHoverboardManager.instance.PreserveMaxHoverboardsConstraint(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}

		// Token: 0x06006C79 RID: 27769 RVA: 0x00231B68 File Offset: 0x0022FD68
		public void SetHoverAllowed(bool allowed, bool force = false)
		{
			if (allowed)
			{
				this.hoverAllowedCount++;
				this.isHoverAllowed = true;
				return;
			}
			this.hoverAllowedCount = ((force || this.hoverAllowedCount == 0) ? 0 : (this.hoverAllowedCount - 1));
			if (this.hoverAllowedCount == 0 && this.isHoverAllowed)
			{
				this.isHoverAllowed = false;
				if (this.enableHoverMode)
				{
					this.SetHoverActive(false);
					VRRig.LocalRig.hoverboardVisual.SetNotHeld();
				}
			}
		}

		// Token: 0x06006C7A RID: 27770 RVA: 0x00231BE0 File Offset: 0x0022FDE0
		public void SetHoverActive(bool enable)
		{
			if (enable && !this.isHoverAllowed)
			{
				return;
			}
			this.enableHoverMode = enable;
			if (!enable)
			{
				this.bodyCollider.enabled = true;
				this.hasHoverPoint = false;
				this.didHoverLastFrame = false;
				for (int i = 0; i < this.hoverboardCasts.Length; i++)
				{
					this.hoverboardCasts[i].didHit = false;
				}
				this.hoverboardAudio.Stop();
			}
		}

		// Token: 0x06006C7B RID: 27771 RVA: 0x00231C50 File Offset: 0x0022FE50
		private void BodyCollider()
		{
			if (this.MaxSphereSizeForNoOverlap(this.bodyInitialRadius * this.scale, this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), false, out this.bodyMaxRadius))
			{
				if (this.scale > 0f)
				{
					this.bodyCollider.radius = this.bodyMaxRadius / this.scale;
				}
				if (Physics.SphereCast(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), this.bodyMaxRadius, GTPlayerTransform.Down, out this.bodyHitInfo, this.bodyInitialHeight * this.scale - this.bodyMaxRadius, this.locomotionEnabledLayers, QueryTriggerInteraction.Ignore))
				{
					this.bodyCollider.height = (this.bodyHitInfo.distance + this.bodyMaxRadius) / this.scale;
				}
				else
				{
					this.bodyHitInfo = this.emptyHit;
					this.bodyCollider.height = this.bodyInitialHeight;
				}
				if (!this.bodyCollider.gameObject.activeSelf)
				{
					this.bodyCollider.gameObject.SetActive(true);
				}
			}
			else
			{
				this.bodyCollider.gameObject.SetActive(false);
			}
			this.bodyCollider.height = Mathf.Lerp(this.bodyCollider.height, this.bodyInitialHeight, this.bodyLerp);
			this.bodyCollider.radius = Mathf.Lerp(this.bodyCollider.radius, this.bodyInitialRadius, this.bodyLerp);
			this.bodyOffsetVector = GTPlayerTransform.Down * this.bodyCollider.height / 2f;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector * this.scale;
			this.bodyCollider.transform.rotation = Quaternion.FromToRotation(this.headCollider.transform.up, GTPlayerTransform.Up) * this.headCollider.transform.rotation;
		}

		// Token: 0x06006C7C RID: 27772 RVA: 0x00231E6F File Offset: 0x0023006F
		private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
		{
			return transformToModify.position + transformToModify.rotation * offsetVector * this.scale;
		}

		// Token: 0x06006C7D RID: 27773 RVA: 0x00231E94 File Offset: 0x00230094
		public void ScaleAwayFromPoint(float oldScale, float newScale, Vector3 scaleCenter)
		{
			if (oldScale < newScale)
			{
				this.lastHeadPosition = GTPlayer.ScalePointAwayFromCenter(this.lastHeadPosition, this.headCollider.radius, oldScale, newScale, scaleCenter);
				this.leftHand.lastPosition = GTPlayer.ScalePointAwayFromCenter(this.leftHand.lastPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
				this.rightHand.lastPosition = GTPlayer.ScalePointAwayFromCenter(this.rightHand.lastPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
			}
		}

		// Token: 0x06006C7E RID: 27774 RVA: 0x00231F0C File Offset: 0x0023010C
		private static Vector3 ScalePointAwayFromCenter(Vector3 point, float baseRadius, float oldScale, float newScale, Vector3 scaleCenter)
		{
			float magnitude = (point - scaleCenter).magnitude;
			float d = magnitude + Mathf.Epsilon + baseRadius * (newScale - oldScale);
			return scaleCenter + (point - scaleCenter) * d / magnitude;
		}

		// Token: 0x06006C7F RID: 27775 RVA: 0x00231F54 File Offset: 0x00230154
		private void OnBeforeRenderInit()
		{
			if (Application.isPlaying && !this.hasCorrectedForTracking && this.mainCamera != null && this.mainCamera.transform.localPosition != Vector3.zero)
			{
				this.ForceRigidBodySync();
				base.transform.position -= this.mainCamera.transform.localPosition;
				this.hasCorrectedForTracking = true;
			}
			Application.onBeforeRender -= this.OnBeforeRenderInit;
		}

		// Token: 0x06006C80 RID: 27776 RVA: 0x00231FE0 File Offset: 0x002301E0
		private void LateUpdate()
		{
			Vector3 value = this.antiDriftLastPosition.GetValueOrDefault();
			if (this.antiDriftLastPosition == null)
			{
				value = base.transform.position;
				this.antiDriftLastPosition = new Vector3?(value);
			}
			if ((double)(this.antiDriftLastPosition.Value - base.transform.position).sqrMagnitude < 1E-08)
			{
				base.transform.position = this.antiDriftLastPosition.Value;
			}
			else
			{
				this.antiDriftLastPosition = new Vector3?(base.transform.position);
			}
			if (!this.hasCorrectedForTracking && this.mainCamera.transform.localPosition != Vector3.zero)
			{
				base.transform.position -= this.mainCamera.transform.localPosition;
				this.hasCorrectedForTracking = true;
				Application.onBeforeRender -= this.OnBeforeRenderInit;
			}
			if (this.playerRigidBody.isKinematic)
			{
				return;
			}
			float time = Time.time;
			Vector3 position = this.headCollider.transform.position;
			this.turnParent.transform.localScale = VRRig.LocalRig.transform.localScale;
			this.playerRigidBody.MovePosition(this.playerRigidBody.position + position - this.headCollider.transform.position);
			if (Mathf.Abs(this.lastScale - this.scale) > 0.001f)
			{
				if (this.mainCamera == null)
				{
					this.mainCamera = Camera.main;
				}
				this.mainCamera.nearClipPlane = ((this.scale > 0.5f) ? 0.01f : 0.002f);
			}
			this.lastScale = this.scale;
			this.debugLastRightHandPosition = this.rightHand.lastPosition;
			this.debugPlatformDeltaPosition = this.MovingSurfaceMovement();
			if (this.debugMovement)
			{
				this.tempRealTime = Time.time;
				this.calcDeltaTime = Time.deltaTime;
				this.lastRealTime = this.tempRealTime;
			}
			else
			{
				this.tempRealTime = Time.realtimeSinceStartup;
				this.calcDeltaTime = this.tempRealTime - this.lastRealTime;
				this.lastRealTime = this.tempRealTime;
				if (this.calcDeltaTime > 0.1f)
				{
					this.calcDeltaTime = 0.05f;
				}
			}
			Vector3 a;
			if (this.lastFrameHasValidTouchPos && this.lastPlatformTouched != null && GTPlayer.ComputeWorldHitPoint(this.lastHitInfoHand, this.lastFrameTouchPosLocal, out a))
			{
				this.refMovement = a - this.lastFrameTouchPosWorld;
			}
			else
			{
				this.refMovement = Vector3.zero;
			}
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			Vector3 pivot = this.headCollider.transform.position;
			Vector3 vector2;
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeWorldHitPoint(this.lastMovingSurfaceHit, this.lastMovingSurfaceTouchLocal, out vector2))
			{
				if (this.wasMovingSurfaceMonkeBlock && (this.lastMonkeBlock == null || this.lastMonkeBlock.state != BuilderPiece.State.AttachedAndPlaced))
				{
					this.movingSurfaceOffset = Vector3.zero;
				}
				else
				{
					this.movingSurfaceOffset = vector2 - this.lastMovingSurfaceTouchWorld;
					vector = this.movingSurfaceOffset / this.calcDeltaTime;
					quaternion = this.lastMovingSurfaceHit.collider.transform.rotation * Quaternion.Inverse(this.lastMovingSurfaceRot);
					pivot = vector2;
				}
			}
			else
			{
				this.movingSurfaceOffset = Vector3.zero;
			}
			float num = 40f * this.scale;
			if (vector.sqrMagnitude >= num * num)
			{
				this.movingSurfaceOffset = Vector3.zero;
				vector = Vector3.zero;
				quaternion = Quaternion.identity;
			}
			if (!this.didAJump && (this.leftHand.wasColliding || this.rightHand.wasColliding))
			{
				base.transform.position = base.transform.position + 4.9f * GTPlayerTransform.PhysicsDown * this.calcDeltaTime * this.calcDeltaTime * this.scale;
				if (Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) <= 0f && Vector3.Dot(GTPlayerTransform.PhysicsUp, this.slideAverageNormal) > 0f)
				{
					base.transform.position = base.transform.position - Vector3.Project(Mathf.Min(this.stickDepth * this.scale, Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude * this.calcDeltaTime) * this.slideAverageNormal, GTPlayerTransform.PhysicsDown);
				}
			}
			if (!this.didAJump && this.anyHandWasSliding)
			{
				base.transform.position = base.transform.position + this.slideVelocity * this.calcDeltaTime;
				this.slideVelocity += 9.8f * GTPlayerTransform.PhysicsDown * this.calcDeltaTime * this.scale;
			}
			float paddleBoostFactor = (Time.time > this.boostEnabledUntilTimestamp) ? 0f : (Time.deltaTime * Mathf.Clamp(this.playerRigidBody.linearVelocity.magnitude * this.hoverboardPaddleBoostMultiplier, 0f, this.hoverboardPaddleBoostMax));
			int num2 = 0;
			Vector3 vector3 = Vector3.zero;
			this.anyHandIsColliding = false;
			this.anyHandIsSliding = false;
			this.anyHandIsSticking = false;
			this.leftHand.FirstIteration(ref vector3, ref num2, paddleBoostFactor);
			this.rightHand.FirstIteration(ref vector3, ref num2, paddleBoostFactor);
			for (int i = 0; i < 12; i++)
			{
				if (this.stiltStates[i].isActive)
				{
					this.stiltStates[i].FirstIteration(ref vector3, ref num2, 0f);
				}
			}
			if (num2 != 0)
			{
				vector3 /= (float)num2;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.RIGHT || this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT)
			{
				vector3 += this.movingSurfaceOffset;
			}
			else if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.BODY)
			{
				Vector3 b = this.lastHeadPosition + this.movingSurfaceOffset - this.headCollider.transform.position;
				vector3 += b;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition, true, out this.maxSphereSize1) && !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
			}
			Vector3 a2;
			float num3;
			if (this.IterativeCollisionSphereCast(this.lastHeadPosition, this.headCollider.radius * 0.9f * this.scale, this.headCollider.transform.position + vector3 - this.lastHeadPosition, Vector3.zero, out a2, false, out num3, out this.junkHit, true))
			{
				vector3 = a2 - this.headCollider.transform.position;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition + vector3, true, out this.maxSphereSize1) || !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition + vector3))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
				vector3 = this.lastHeadPosition - this.headCollider.transform.position;
			}
			else if (this.headCollider.radius * 0.9f * 0.825f * this.scale < this.maxSphereSize1)
			{
				this.lastOpenHeadPosition = this.headCollider.transform.position + vector3;
			}
			if (vector3 != Vector3.zero)
			{
				base.transform.position += vector3;
			}
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && quaternion != Quaternion.identity && !this.isClimbing && !this.rightHand.isHolding && !this.leftHand.isHolding)
			{
				this.RotateWithSurface(quaternion, pivot);
			}
			this.lastHeadPosition = this.headCollider.transform.position;
			this.areBothTouching = ((!this.leftHand.isColliding && !this.leftHand.wasColliding) || (!this.rightHand.isColliding && !this.rightHand.wasColliding));
			this.TakeMyHand_ProcessMovement();
			this.HandleTentacleMovement();
			this.anyHandIsColliding = false;
			this.anyHandIsSliding = false;
			this.anyHandIsSticking = false;
			this.leftHand.FinalizeHandPosition();
			this.rightHand.FinalizeHandPosition();
			for (int j = 0; j < 12; j++)
			{
				if (this.stiltStates[j].isActive)
				{
					this.stiltStates[j].FinalizeHandPosition();
					GTPlayer.HandState handState = this.stiltStates[j];
					GorillaTagger.Instance.SetExtraHandPosition((StiltID)j, handState.finalPositionThisFrame, handState.canTag, handState.canStun);
				}
			}
			Vector3 b2 = this.lastPosition;
			GTPlayer.MovingSurfaceContactPoint movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
			int num4 = -1;
			int num5 = -1;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = this.rightHand.isColliding && this.IsTouchingMovingSurface(this.rightHand.GetLastPosition(), this.rightHand.lastHitInfo, out num4, out flag, out flag2);
			if (flag4 && !flag)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
				this.lastMovingSurfaceHit = this.rightHand.lastHitInfo;
			}
			else
			{
				bool flag5 = false;
				BuilderPiece builderPiece = flag4 ? this.lastMonkeBlock : null;
				if (this.leftHand.isColliding && this.IsTouchingMovingSurface(this.leftHand.GetLastPosition(), this.leftHand.lastHitInfo, out num5, out flag5, out flag3))
				{
					if (flag5 && flag2 == flag3)
					{
						if (flag && num5.Equals(num4) && (double)Vector3.Dot(this.leftHand.lastHitInfo.point - this.leftHand.GetLastPosition(), this.rightHand.lastHitInfo.point - this.rightHand.GetLastPosition()) < 0.3)
						{
							movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
							this.lastMovingSurfaceHit = this.rightHand.lastHitInfo;
							this.lastMonkeBlock = builderPiece;
						}
					}
					else
					{
						movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.LEFT;
						this.lastMovingSurfaceHit = this.leftHand.lastHitInfo;
					}
				}
			}
			this.StoreVelocities();
			if (this.InWater)
			{
				PlayerGameEvents.PlayerSwam((this.lastPosition - b2).magnitude, this.currentVelocity.magnitude);
			}
			else
			{
				PlayerGameEvents.PlayerMoved((this.lastPosition - b2).magnitude, this.currentVelocity.magnitude);
			}
			this.didAJump = false;
			bool flag6 = this.exitMovingSurface;
			this.exitMovingSurface = false;
			if (this.leftHand.IsSlipOverriddenToMax() && this.rightHand.IsSlipOverriddenToMax())
			{
				this.didAJump = true;
				this.exitMovingSurface = true;
			}
			else if (this.anyHandIsSliding)
			{
				this.slideAverageNormal = Vector3.zero;
				int num6 = 0;
				this.averageSlipPercentage = 0f;
				bool flag7 = false;
				if (this.leftHand.isSliding)
				{
					this.slideAverageNormal += this.leftHand.slideNormal.normalized;
					this.averageSlipPercentage += this.leftHand.slipPercentage;
					num6++;
				}
				if (this.rightHand.isSliding)
				{
					flag7 = true;
					this.slideAverageNormal += this.rightHand.slideNormal.normalized;
					this.averageSlipPercentage += this.rightHand.slipPercentage;
					num6++;
				}
				for (int k = 0; k < this.stiltStates.Length; k++)
				{
					if (this.stiltStates[k].isActive && this.stiltStates[k].isSliding)
					{
						if (!this.stiltStates[k].isLeftHand)
						{
							flag7 = true;
						}
						this.slideAverageNormal += this.stiltStates[k].slideNormal.normalized;
						this.averageSlipPercentage += this.stiltStates[k].slipPercentage;
						num6++;
					}
				}
				this.slideAverageNormal = this.slideAverageNormal.normalized;
				this.averageSlipPercentage /= (float)num6;
				if (num6 == 1)
				{
					this.surfaceDirection = (flag7 ? Vector3.ProjectOnPlane(this.rightHand.handFollower.forward, this.rightHand.slideNormal) : Vector3.ProjectOnPlane(this.leftHand.handFollower.forward, this.leftHand.slideNormal));
					if (Vector3.Dot(this.slideVelocity, this.surfaceDirection) > 0f)
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
					else
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, -this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
				}
				if (!this.anyHandWasSliding)
				{
					this.slideVelocity = ((Vector3.Dot(this.playerRigidBody.linearVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.playerRigidBody.linearVelocity, this.slideAverageNormal) : this.playerRigidBody.linearVelocity);
				}
				else
				{
					this.slideVelocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
				}
				this.slideVelocity = this.slideVelocity.normalized * Mathf.Min(this.slideVelocity.magnitude, Mathf.Max(0.5f, this.averagedVelocity.magnitude * 2f));
				this.playerRigidBody.linearVelocity = Vector3.zero;
			}
			else if (this.anyHandIsColliding)
			{
				if (!this.turnedThisFrame)
				{
					this.playerRigidBody.linearVelocity = Vector3.zero;
				}
				else
				{
					this.playerRigidBody.linearVelocity = this.playerRigidBody.linearVelocity.normalized * Mathf.Min(2f, this.playerRigidBody.linearVelocity.magnitude);
				}
			}
			else if (this.anyHandWasSliding)
			{
				this.playerRigidBody.linearVelocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
			}
			if (this.anyHandIsColliding && !this.disableMovement && !this.turnedThisFrame && !this.didAJump)
			{
				if (this.anyHandIsSliding)
				{
					if (Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > this.slideVelocityLimit * this.scale && Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) > 0f && Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > Vector3.Project(this.slideVelocity, this.slideAverageNormal).magnitude)
					{
						this.leftHand.isSliding = false;
						this.rightHand.isSliding = false;
						for (int l = 0; l < this.stiltStates.Length; l++)
						{
							this.stiltStates[l].isSliding = false;
						}
						this.anyHandIsSliding = false;
						this.didAJump = true;
						float num7 = this.ApplyNativeScaleAdjustment(Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude));
						this.playerRigidBody.linearVelocity = num7 * this.siJumpMultiplier * this.slideAverageNormal.normalized + Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal);
						if (num7 > this.slideVelocityLimit * this.scale * this.exitMovingSurfaceThreshold)
						{
							this.exitMovingSurface = true;
						}
					}
				}
				else if (this.averagedVelocity.magnitude > this.velocityLimit * this.scale)
				{
					float num8 = (this.InWater && this.CurrentWaterVolume != null) ? this.liquidPropertiesList[(int)this.CurrentWaterVolume.LiquidType].surfaceJumpFactor : 1f;
					float num9 = this.ApplyNativeScaleAdjustment(this.enableHoverMode ? Mathf.Min(this.hoverMaxPaddleSpeed, this.averagedVelocity.magnitude) : Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * num8 * this.averagedVelocity.magnitude));
					Vector3 vector4 = num9 * this.siJumpMultiplier * this.averagedVelocity.normalized;
					this.didAJump = true;
					this.playerRigidBody.linearVelocity = vector4;
					if (this.InWater)
					{
						this.swimmingVelocity += vector4 * this.swimmingParams.underwaterJumpsAsSwimVelocityFactor;
					}
					if (num9 > this.velocityLimit * this.scale * this.exitMovingSurfaceThreshold)
					{
						this.exitMovingSurface = true;
					}
				}
			}
			this.stuckHandsCheckLateUpdate(ref this.leftHand.finalPositionThisFrame, ref this.rightHand.finalPositionThisFrame);
			if (this.lastPlatformTouched != null && this.currentPlatform == null)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.linearVelocity += this.refMovement / this.calcDeltaTime;
				}
				this.refMovement = Vector3.zero;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.linearVelocity += this.lastMovingSurfaceVelocity;
				}
				this.lastMovingSurfaceVelocity = Vector3.zero;
			}
			if (this.enableHoverMode)
			{
				this.HoverboardLateUpdate();
			}
			else
			{
				this.hasHoverPoint = false;
			}
			Vector3 vector5 = Vector3.zero;
			float a3 = 0f;
			float a4 = 0f;
			if (this.bodyInWater)
			{
				Vector3 b3;
				if (this.GetSwimmingVelocityForHand(this.leftHand.lastPosition, this.leftHand.finalPositionThisFrame, this.leftHand.controllerTransform.right, this.calcDeltaTime, ref this.leftHandWaterVolume, ref this.leftHandWaterSurface, out b3) && !this.turnedThisFrame)
				{
					a3 = Mathf.InverseLerp(0f, 0.2f, b3.magnitude) * this.swimmingParams.swimmingHapticsStrength;
					vector5 += b3;
				}
				Vector3 b4;
				if (this.GetSwimmingVelocityForHand(this.rightHand.lastPosition, this.rightHand.finalPositionThisFrame, -this.rightHand.controllerTransform.right, this.calcDeltaTime, ref this.rightHandWaterVolume, ref this.rightHandWaterSurface, out b4) && !this.turnedThisFrame)
				{
					a4 = Mathf.InverseLerp(0f, 0.15f, b4.magnitude) * this.swimmingParams.swimmingHapticsStrength;
					vector5 += b4;
				}
			}
			Vector3 vector6 = Vector3.zero;
			Vector3 b5;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.leftHand.lastPosition, this.leftHand.finalPositionThisFrame, this.leftHand.controllerTransform.right, this.leftHand.velocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.leftHandWaterVolume, this.leftHandWaterSurface, out b5))
			{
				if (time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown)
				{
					vector6 += b5;
				}
				this.lastWaterSurfaceJumpTimeLeft = Time.time;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			Vector3 b6;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.rightHand.lastPosition, this.rightHand.finalPositionThisFrame, -this.rightHand.controllerTransform.right, this.rightHand.velocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.rightHandWaterVolume, this.rightHandWaterSurface, out b6))
			{
				if (time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown)
				{
					vector6 += b6;
				}
				this.lastWaterSurfaceJumpTimeRight = Time.time;
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			vector6 = Vector3.ClampMagnitude(vector6, this.swimmingParams.waterSurfaceJumpMaxSpeed * this.scale);
			float num10 = Mathf.Max(a3, this.leftHandNonDiveHapticsAmount);
			if (num10 > 0.001f && time - this.lastWaterSurfaceJumpTimeLeft > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.LeftHand, num10, this.calcDeltaTime);
			}
			float num11 = Mathf.Max(a4, this.rightHandNonDiveHapticsAmount);
			if (num11 > 0.001f && time - this.lastWaterSurfaceJumpTimeRight > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.RightHand, num11, this.calcDeltaTime);
			}
			if (!this.disableMovement)
			{
				this.swimmingVelocity += vector5;
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.linearVelocity += vector5 + vector6;
				}
			}
			else
			{
				this.swimmingVelocity = Vector3.zero;
			}
			if (GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				if (!this.IsFrozen || !this.primaryButtonPressed)
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
					if (this.bodyTouchedSurfaces.Count > 0)
					{
						foreach (KeyValuePair<GameObject, PhysicsMaterial> keyValuePair in this.bodyTouchedSurfaces)
						{
							MeshCollider meshCollider;
							if (keyValuePair.Key.TryGetComponent<MeshCollider>(out meshCollider))
							{
								meshCollider.material = keyValuePair.Value;
							}
						}
						this.bodyTouchedSurfaces.Clear();
					}
				}
				else if (this.BodyOnGround && this.primaryButtonPressed)
				{
					float y = this.bodyInitialHeight / 2f - this.bodyInitialRadius;
					RaycastHit raycastHit;
					if (Physics.SphereCast(this.bodyCollider.transform.position - new Vector3(0f, y, 0f), this.bodyInitialRadius - 0.01f, Vector3.down, out raycastHit, 1f, ~LayerMask.GetMask(new string[]
					{
						"Gorilla Body Collider",
						"GorillaInteractable"
					}), QueryTriggerInteraction.Ignore))
					{
						this.IsBodySliding = true;
						MeshCollider meshCollider2;
						if (!this.bodyTouchedSurfaces.ContainsKey(raycastHit.transform.gameObject) && raycastHit.transform.gameObject.TryGetComponent<MeshCollider>(out meshCollider2))
						{
							this.bodyTouchedSurfaces.Add(raycastHit.transform.gameObject, meshCollider2.material);
							raycastHit.transform.gameObject.GetComponent<MeshCollider>().material = this.slipperyMaterial;
						}
					}
				}
				else
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
				}
			}
			else
			{
				this.IsBodySliding = false;
				if (this.bodyTouchedSurfaces.Count > 0)
				{
					foreach (KeyValuePair<GameObject, PhysicsMaterial> keyValuePair2 in this.bodyTouchedSurfaces)
					{
						MeshCollider meshCollider3;
						if (keyValuePair2.Key.TryGetComponent<MeshCollider>(out meshCollider3))
						{
							meshCollider3.material = keyValuePair2.Value;
						}
					}
					this.bodyTouchedSurfaces.Clear();
				}
			}
			this.leftHand.OnEndOfFrame();
			this.rightHand.OnEndOfFrame();
			for (int m = 0; m < 12; m++)
			{
				if (this.stiltStates[m].isActive)
				{
					this.stiltStates[m].OnEndOfFrame();
				}
			}
			this.leftHand.PositionHandFollower();
			this.rightHand.PositionHandFollower();
			this.anyHandWasSliding = this.anyHandIsSliding;
			this.anyHandWasColliding = this.anyHandIsColliding;
			this.anyHandWasSticking = this.anyHandIsSticking;
			if (this.anyHandIsSticking)
			{
				this.lastTouchedGroundTimestamp = Time.time;
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.IsGroundedHand || this.IsTentacleActive || this.IsThrusterActive)
				{
					this.LastHandTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
					this.LastTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
				}
				else if (this.IsGroundedButt || this.IsLaserZiplineActive)
				{
					this.LastTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
				}
			}
			else
			{
				this.LastHandTouchedGroundAtNetworkTime = 0f;
				this.LastTouchedGroundAtNetworkTime = 0f;
			}
			this.degreesTurnedThisFrame = 0f;
			this.lastPlatformTouched = this.currentPlatform;
			this.currentPlatform = null;
			this.lastMovingSurfaceVelocity = vector;
			Vector3 vector7;
			if (GTPlayer.ComputeLocalHitPoint(this.lastHitInfoHand, out vector7))
			{
				this.lastFrameHasValidTouchPos = true;
				this.lastFrameTouchPosLocal = vector7;
				this.lastFrameTouchPosWorld = this.lastHitInfoHand.point;
			}
			else
			{
				this.lastFrameHasValidTouchPos = false;
				this.lastFrameTouchPosLocal = Vector3.zero;
				this.lastFrameTouchPosWorld = Vector3.zero;
			}
			this.lastRigidbodyPosition = this.playerRigidBody.transform.position;
			RaycastHit raycastHit2 = this.emptyHit;
			this.BodyCollider();
			if (this.bodyHitInfo.collider != null)
			{
				this.wasBodyOnGround = true;
				raycastHit2 = this.bodyHitInfo;
			}
			else if (movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.bodyCollider.gameObject.activeSelf)
			{
				bool flag8 = false;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				Vector3 origin = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + (this.bodyInitialHeight * this.scale - this.bodyMaxRadius) * GTPlayerTransform.Down;
				this.bufferCount = Physics.SphereCastNonAlloc(origin, this.bodyMaxRadius, GTPlayerTransform.Down, this.rayCastNonAllocColliders, this.minimumRaycastDistance * this.scale, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int n = 0; n < this.bufferCount; n++)
					{
						if (this.tempHitInfo.distance > 0f && (!flag8 || this.rayCastNonAllocColliders[n].distance < this.tempHitInfo.distance))
						{
							flag8 = true;
							raycastHit2 = this.rayCastNonAllocColliders[n];
						}
					}
				}
				this.wasBodyOnGround = flag8;
			}
			int num12 = -1;
			bool flag9 = false;
			bool flag10;
			if (this.wasBodyOnGround && movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.IsTouchingMovingSurface(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), raycastHit2, out num12, out flag10, out flag9) && !flag10)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.BODY;
				this.lastMovingSurfaceHit = raycastHit2;
			}
			Vector3 vector8;
			if (movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeLocalHitPoint(this.lastMovingSurfaceHit, out vector8))
			{
				this.lastMovingSurfaceTouchLocal = vector8;
				this.lastMovingSurfaceTouchWorld = this.lastMovingSurfaceHit.point;
				this.lastMovingSurfaceRot = this.lastMovingSurfaceHit.collider.transform.rotation;
				this.lastAttachedToMovingSurfaceFrame = Time.frameCount;
			}
			else
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
				this.lastMovingSurfaceTouchLocal = Vector3.zero;
				this.lastMovingSurfaceTouchWorld = Vector3.zero;
				this.lastMovingSurfaceRot = Quaternion.identity;
			}
			Vector3 position2 = this.lastMovingSurfaceTouchWorld;
			int num13 = -1;
			bool flag11 = false;
			switch (movingSurfaceContactPoint)
			{
			case GTPlayer.MovingSurfaceContactPoint.NONE:
				if (flag6)
				{
					this.exitMovingSurface = true;
				}
				num13 = -1;
				break;
			case GTPlayer.MovingSurfaceContactPoint.RIGHT:
				num13 = num4;
				flag11 = flag2;
				position2 = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.LEFT:
				num13 = num5;
				flag11 = flag3;
				position2 = GorillaTagger.Instance.offlineVRRig.leftHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.BODY:
				num13 = num12;
				flag11 = flag9;
				position2 = GorillaTagger.Instance.offlineVRRig.bodyTransform.position;
				break;
			}
			if (!flag11)
			{
				this.lastMonkeBlock = null;
			}
			if (num13 != this.lastMovingSurfaceID || this.lastMovingSurfaceContact != movingSurfaceContactPoint || flag11 != this.wasMovingSurfaceMonkeBlock)
			{
				if (num13 == -1)
				{
					if (Time.frameCount - this.lastAttachedToMovingSurfaceFrame > 3)
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (flag11)
				{
					if (this.lastMonkeBlock != null)
					{
						VRRig.AttachLocalPlayerToMovingSurface(num13, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, this.lastMonkeBlock.transform.InverseTransformPoint(position2), flag11);
						this.lastMovingSurfaceID = num13;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (MovingSurfaceManager.instance != null)
				{
					MovingSurface movingSurface;
					if (MovingSurfaceManager.instance.TryGetMovingSurface(num13, out movingSurface))
					{
						VRRig.AttachLocalPlayerToMovingSurface(num13, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, movingSurface.transform.InverseTransformPoint(position2), flag11);
						this.lastMovingSurfaceID = num13;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else
				{
					VRRig.DetachLocalPlayerFromMovingSurface();
					this.lastMovingSurfaceID = -1;
				}
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE && movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			this.lastMovingSurfaceContact = movingSurfaceContactPoint;
			this.wasMovingSurfaceMonkeBlock = flag11;
			if (this.activeSizeChangerSettings != null)
			{
				if (this.activeSizeChangerSettings.ExpireOnDistance > 0f && Vector3.Distance(base.transform.position, this.activeSizeChangerSettings.WorldPosition) > this.activeSizeChangerSettings.ExpireOnDistance)
				{
					this.SetNativeScale(null);
				}
				if (this.activeSizeChangerSettings.ExpireAfterSeconds > 0f && Time.time - this.activeSizeChangerSettings.ActivationTime > this.activeSizeChangerSettings.ExpireAfterSeconds)
				{
					this.SetNativeScale(null);
				}
			}
			TakeMyHand_HandLink grabbedLink = VRRig.LocalRig.leftHandLink.grabbedLink;
			if (grabbedLink != null)
			{
				double time2 = PhotonNetwork.Time;
				float lastHandTouchedGroundAtNetworkTime = this.LastHandTouchedGroundAtNetworkTime;
				double time3 = PhotonNetwork.Time;
				float lastHandTouchedGroundAtNetworkTime2 = grabbedLink.myRig.LastHandTouchedGroundAtNetworkTime;
			}
			if (this.didAJump || this.anyHandIsColliding || this.anyHandIsSliding || this.anyHandIsSticking || this.IsGroundedHand || this.forceRBSync)
			{
				this.playerRigidBody.position = base.transform.position;
				this.playerRigidBody.rotation = base.transform.rotation;
				this.forceRBSync = false;
			}
		}

		// Token: 0x06006C81 RID: 27777 RVA: 0x00233F54 File Offset: 0x00232154
		private float ApplyNativeScaleAdjustment(float adjustedMagnitude)
		{
			if (this.nativeScale > 0f && this.nativeScale != 1f)
			{
				return adjustedMagnitude *= this.nativeScaleMagnitudeAdjustmentFactor.Evaluate(this.nativeScale);
			}
			return adjustedMagnitude;
		}

		// Token: 0x06006C82 RID: 27778 RVA: 0x00233F88 File Offset: 0x00232188
		private float RotateWithSurface(Quaternion rotationDelta, Vector3 pivot)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			QuaternionUtil.DecomposeSwingTwist(rotationDelta, GTPlayerTransform.PhysicsUp, out quaternion, out quaternion2);
			float num = quaternion2.eulerAngles.y;
			if (num > 270f)
			{
				num -= 360f;
			}
			else if (num > 90f)
			{
				num -= 180f;
			}
			if (Mathf.Abs(num) < 90f * this.calcDeltaTime)
			{
				this.turnParent.transform.RotateAround(pivot, base.transform.up, num);
				return num;
			}
			return 0f;
		}

		// Token: 0x06006C83 RID: 27779 RVA: 0x0023400C File Offset: 0x0023220C
		private void stuckHandsCheckFixedUpdate()
		{
			Vector3 currentHandPosition = this.leftHand.GetCurrentHandPosition();
			this.stuckLeft = (!this.controllerState.LeftValid || (this.leftHand.isColliding && (currentHandPosition - this.leftHand.GetLastPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (currentHandPosition - this.headCollider.transform.position).normalized, (currentHandPosition - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)));
			Vector3 currentHandPosition2 = this.rightHand.GetCurrentHandPosition();
			this.stuckRight = (!this.controllerState.RightValid || (this.rightHand.isColliding && (currentHandPosition2 - this.rightHand.GetLastPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (currentHandPosition2 - this.headCollider.transform.position).normalized, (currentHandPosition2 - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)));
		}

		// Token: 0x06006C84 RID: 27780 RVA: 0x00234198 File Offset: 0x00232398
		private void stuckHandsCheckLateUpdate(ref Vector3 finalLeftHandPosition, ref Vector3 finalRightHandPosition)
		{
			if (this.stuckLeft)
			{
				finalLeftHandPosition = this.leftHand.GetCurrentHandPosition();
				this.stuckLeft = (this.leftHand.isColliding = false);
			}
			if (this.stuckRight)
			{
				finalRightHandPosition = this.rightHand.GetCurrentHandPosition();
				this.stuckRight = (this.rightHand.isColliding = false);
			}
		}

		// Token: 0x06006C85 RID: 27781 RVA: 0x00234204 File Offset: 0x00232404
		private void handleClimbing(float deltaTime)
		{
			if (this.isClimbing && (this.inOverlay || this.climbHelper == null || this.currentClimbable == null || !this.currentClimbable.isActiveAndEnabled))
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			Vector3 vector = Vector3.zero;
			if (this.isClimbing && (this.currentClimber.transform.position - this.climbHelper.position).magnitude > 1f)
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			if (this.isClimbing)
			{
				this.playerRigidBody.linearVelocity = Vector3.zero;
				this.climbHelper.localPosition = Vector3.MoveTowards(this.climbHelper.localPosition, this.climbHelperTargetPos, deltaTime * 12f);
				vector = this.currentClimber.transform.position - this.climbHelper.position;
				vector = ((vector.sqrMagnitude > this.maxArmLength * this.maxArmLength) ? (vector.normalized * this.maxArmLength) : vector);
				if (this.isClimbableMoving)
				{
					Quaternion rotationDelta = this.currentClimbable.transform.rotation * Quaternion.Inverse(this.lastClimbableRotation);
					this.RotateWithSurface(rotationDelta, this.currentClimber.handRoot.position);
					this.lastClimbableRotation = this.currentClimbable.transform.rotation;
				}
				this.playerRigidBody.MovePosition(this.playerRigidBody.position - vector);
				if (this.currentSwing)
				{
					this.currentSwing.lastGrabTime = Time.time;
				}
			}
		}

		// Token: 0x06006C86 RID: 27782 RVA: 0x002343C3 File Offset: 0x002325C3
		public void RequestTentacleMove(bool isLeftHand, Vector3 move)
		{
			if (isLeftHand)
			{
				this.hasLeftHandTentacleMove = true;
				this.leftHandTentacleMove = move;
				return;
			}
			this.hasRightHandTentacleMove = true;
			this.rightHandTentacleMove = move;
		}

		// Token: 0x06006C87 RID: 27783 RVA: 0x002343E8 File Offset: 0x002325E8
		public void HandleTentacleMovement()
		{
			Vector3 b;
			if (this.hasLeftHandTentacleMove)
			{
				if (this.hasRightHandTentacleMove)
				{
					b = (this.leftHandTentacleMove + this.rightHandTentacleMove) * 0.5f;
					this.hasRightHandTentacleMove = (this.hasLeftHandTentacleMove = false);
				}
				else
				{
					b = this.leftHandTentacleMove;
					this.hasLeftHandTentacleMove = false;
				}
			}
			else
			{
				if (!this.hasRightHandTentacleMove)
				{
					return;
				}
				b = this.rightHandTentacleMove;
				this.hasRightHandTentacleMove = false;
			}
			this.playerRigidBody.transform.position += b;
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006C88 RID: 27784 RVA: 0x00234488 File Offset: 0x00232688
		public HandLinkAuthorityStatus TakeMyHand_GetSelfHandLinkAuthority()
		{
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			if (this.IsGroundedHand)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded);
			}
			if ((double)(this.LastHandTouchedGroundAtNetworkTime + 1f) > PhotonNetwork.Time)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, this.LastHandTouchedGroundAtNetworkTime, actorNumber);
			}
			if (this.IsGroundedButt)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded);
			}
			return new HandLinkAuthorityStatus(HandLinkAuthorityType.None, this.LastTouchedGroundAtNetworkTime, actorNumber);
		}

		// Token: 0x06006C89 RID: 27785 RVA: 0x002344F0 File Offset: 0x002326F0
		private void TakeMyHand_ProcessMovement()
		{
			TakeMyHand_HandLink leftHandLink = VRRig.LocalRig.leftHandLink;
			TakeMyHand_HandLink rightHandLink = VRRig.LocalRig.rightHandLink;
			bool flag = leftHandLink.grabbedLink != null;
			bool flag2 = rightHandLink.grabbedLink != null;
			if (!flag && !flag2)
			{
				return;
			}
			HandLinkAuthorityStatus handLinkAuthorityStatus = this.TakeMyHand_GetSelfHandLinkAuthority();
			int num = -1;
			HandLinkAuthorityStatus chainAuthority = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
			if (flag)
			{
				chainAuthority = leftHandLink.GetChainAuthority(out num);
			}
			int num2 = -1;
			HandLinkAuthorityStatus chainAuthority2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
			if (flag2)
			{
				chainAuthority2 = rightHandLink.GetChainAuthority(out num2);
			}
			if (flag && flag2)
			{
				if (leftHandLink.grabbedPlayer == rightHandLink.grabbedPlayer)
				{
					switch (handLinkAuthorityStatus.CompareTo(chainAuthority))
					{
					case -1:
						this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
						return;
					case 0:
						this.TakeMyHand_PositionBoth_BothHands(leftHandLink, rightHandLink);
						return;
					case 1:
						this.TakeMyHand_PositionChild_RemotePlayer_BothHands(leftHandLink, rightHandLink);
						return;
					default:
						return;
					}
				}
				else
				{
					int num3 = handLinkAuthorityStatus.CompareTo(chainAuthority);
					int num4 = handLinkAuthorityStatus.CompareTo(chainAuthority2);
					switch (num3 * 3 + num4)
					{
					case -3:
					case -2:
						this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
						return;
					case -1:
					case 2:
						this.TakeMyHand_PositionChild_LocalPlayer(rightHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 0:
						this.TakeMyHand_PositionTriple(leftHandLink, rightHandLink);
						return;
					case 1:
						this.TakeMyHand_PositionBoth(leftHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
						return;
					case 3:
						this.TakeMyHand_PositionBoth(rightHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 4:
						this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
						return;
					}
					switch (chainAuthority.CompareTo(chainAuthority2))
					{
					case -1:
						this.TakeMyHand_PositionChild_LocalPlayer(rightHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 0:
						if (num > num2)
						{
							this.TakeMyHand_PositionChild_LocalPlayer(rightHandLink);
							this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
							return;
						}
						if (num < num2)
						{
							this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink);
							this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
							return;
						}
						this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
						return;
					case 1:
						this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
						return;
					default:
						return;
					}
				}
			}
			else if (flag)
			{
				switch (handLinkAuthorityStatus.CompareTo(chainAuthority))
				{
				case -1:
					this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink);
					return;
				case 0:
					this.TakeMyHand_PositionBoth(leftHandLink);
					return;
				case 1:
					this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
					return;
				default:
					return;
				}
			}
			else
			{
				switch (handLinkAuthorityStatus.CompareTo(chainAuthority2))
				{
				case -1:
					this.TakeMyHand_PositionChild_LocalPlayer(rightHandLink);
					return;
				case 0:
					this.TakeMyHand_PositionBoth(rightHandLink);
					return;
				case 1:
					this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x06006C8A RID: 27786 RVA: 0x00234744 File Offset: 0x00232944
		private void TakeMyHand_PositionTriple(TakeMyHand_HandLink linkA, TakeMyHand_HandLink linkB)
		{
			Vector3 a = linkA.LinkPosition - linkA.grabbedLink.LinkPosition;
			Vector3 vector = linkB.LinkPosition - linkB.grabbedLink.LinkPosition;
			Vector3 b = (a + vector) * 0.33f;
			bool flag;
			bool flag2;
			linkA.grabbedLink.myRig.TrySweptOffsetMove(a - b, out flag, out flag2);
			bool flag3;
			bool flag4;
			linkB.grabbedLink.myRig.TrySweptOffsetMove(vector - b, out flag3, out flag4);
			this.playerRigidBody.MovePosition(this.playerRigidBody.position - b);
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006C8B RID: 27787 RVA: 0x002347F4 File Offset: 0x002329F4
		private void TakeMyHand_PositionBoth(TakeMyHand_HandLink link)
		{
			Vector3 vector = (link.grabbedLink.LinkPosition - link.LinkPosition) * 0.5f;
			bool flag;
			bool flag2;
			link.grabbedLink.myRig.TrySweptOffsetMove(-vector, out flag, out flag2);
			if (flag || flag2)
			{
				this.TakeMyHand_PositionChild_LocalPlayer(link);
			}
			else
			{
				this.playerRigidBody.transform.position += vector;
			}
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006C8C RID: 27788 RVA: 0x00234878 File Offset: 0x00232A78
		private void TakeMyHand_PositionBoth_BothHands(TakeMyHand_HandLink link1, TakeMyHand_HandLink link2)
		{
			Vector3 a = (link1.grabbedLink.LinkPosition - link1.LinkPosition) * 0.5f;
			Vector3 b = (link2.grabbedLink.LinkPosition - link2.LinkPosition) * 0.5f;
			Vector3 vector = (a + b) * 0.5f;
			bool flag;
			bool flag2;
			link1.grabbedLink.myRig.TrySweptOffsetMove(-vector, out flag, out flag2);
			if (flag || flag2)
			{
				this.TakeMyHand_PositionChild_LocalPlayer(link1, link2);
			}
			else
			{
				this.playerRigidBody.transform.position += vector;
			}
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006C8D RID: 27789 RVA: 0x0023492C File Offset: 0x00232B2C
		private void TakeMyHand_PositionChild_LocalPlayer(TakeMyHand_HandLink parentLink)
		{
			Vector3 b = parentLink.grabbedLink.LinkPosition - parentLink.LinkPosition;
			this.playerRigidBody.transform.position += b;
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006C8E RID: 27790 RVA: 0x0023497C File Offset: 0x00232B7C
		private void TakeMyHand_PositionChild_LocalPlayer(TakeMyHand_HandLink linkA, TakeMyHand_HandLink linkB)
		{
			Vector3 a = linkA.grabbedLink.LinkPosition - linkA.LinkPosition;
			Vector3 b = linkB.grabbedLink.LinkPosition - linkB.LinkPosition;
			this.playerRigidBody.transform.position += (a + b) * 0.5f;
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x06006C8F RID: 27791 RVA: 0x002349F4 File Offset: 0x00232BF4
		private void TakeMyHand_PositionChild_RemotePlayer(TakeMyHand_HandLink childLink)
		{
			Vector3 movement = childLink.LinkPosition - childLink.grabbedLink.LinkPosition;
			bool flag;
			bool flag2;
			childLink.grabbedLink.myRig.TrySweptOffsetMove(movement, out flag, out flag2);
			if (flag || flag2)
			{
				this.TakeMyHand_PositionChild_LocalPlayer(childLink);
			}
		}

		// Token: 0x06006C90 RID: 27792 RVA: 0x00234A3C File Offset: 0x00232C3C
		private void TakeMyHand_PositionChild_RemotePlayer_BothHands(TakeMyHand_HandLink childLink1, TakeMyHand_HandLink childLink2)
		{
			Vector3 a = childLink1.LinkPosition - childLink1.grabbedLink.LinkPosition;
			Vector3 b = childLink2.LinkPosition - childLink2.grabbedLink.LinkPosition;
			Vector3 movement = (a + b) * 0.5f;
			bool flag;
			bool flag2;
			childLink1.grabbedLink.myRig.TrySweptOffsetMove(movement, out flag, out flag2);
			if (flag || flag2)
			{
				this.TakeMyHand_PositionChild_LocalPlayer(childLink1, childLink2);
			}
		}

		// Token: 0x06006C91 RID: 27793 RVA: 0x00234AAC File Offset: 0x00232CAC
		private bool IterativeCollisionSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, Vector3 boostVector, out Vector3 endPosition, bool singleHand, out float slipPercentage, out RaycastHit iterativeHitInfo, bool fullSlide)
		{
			slipPercentage = this.defaultSlideFactor;
			if (!this.CollisionsSphereCast(startPosition, sphereRadius, movementVector, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				endPosition = Vector3.zero;
				return false;
			}
			this.firstPosition = endPosition;
			iterativeHitInfo = this.tempIterativeHit;
			this.slideFactor = this.GetSlidePercentage(iterativeHitInfo);
			slipPercentage = ((this.slideFactor != this.defaultSlideFactor) ? this.slideFactor : ((!singleHand) ? this.defaultSlideFactor : 0.001f));
			if (fullSlide)
			{
				slipPercentage = 1f;
			}
			this.movementToProjectedAboveCollisionPlane = Vector3.ProjectOnPlane(startPosition + movementVector - this.firstPosition, iterativeHitInfo.normal) * slipPercentage;
			Vector3 vector = Vector3.zero;
			if (boostVector.IsLongerThan(0f))
			{
				vector = Vector3.ProjectOnPlane(boostVector, iterativeHitInfo.normal);
				this.movementToProjectedAboveCollisionPlane += vector;
				this.CollisionsSphereCast(this.firstPosition, sphereRadius, vector, out endPosition, out this.tempIterativeHit);
				this.firstPosition = endPosition;
			}
			if (this.CollisionsSphereCast(this.firstPosition, sphereRadius, this.movementToProjectedAboveCollisionPlane, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			if (this.CollisionsSphereCast(this.movementToProjectedAboveCollisionPlane + this.firstPosition, sphereRadius, startPosition + movementVector + vector - (this.movementToProjectedAboveCollisionPlane + this.firstPosition), out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			endPosition = Vector3.zero;
			return false;
		}

		// Token: 0x06006C92 RID: 27794 RVA: 0x00234C68 File Offset: 0x00232E68
		private bool CollisionsSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 finalPosition, out RaycastHit collisionsHitInfo)
		{
			this.MaxSphereSizeForNoOverlap(sphereRadius, startPosition, false, out this.maxSphereSize1);
			bool flag = false;
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.SphereCastNonAlloc(startPosition, this.maxSphereSize1, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int i = 0; i < this.bufferCount; i++)
				{
					if (this.tempHitInfo.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < this.tempHitInfo.distance))
					{
						flag = true;
						this.tempHitInfo = this.rayCastNonAllocColliders[i];
					}
				}
			}
			if (flag)
			{
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = collisionsHitInfo.point + collisionsHitInfo.normal * sphereRadius;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int j = 0; j < this.bufferCount; j++)
					{
						if (this.rayCastNonAllocColliders[j].collider && this.rayCastNonAllocColliders[j].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[j];
						}
					}
					finalPosition = startPosition + movementVector.normalized * this.tempHitInfo.distance;
				}
				this.MaxSphereSizeForNoOverlap(sphereRadius, finalPosition, false, out this.maxSphereSize2);
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.SphereCastNonAlloc(startPosition, Mathf.Min(this.maxSphereSize1, this.maxSphereSize2), (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int k = 0; k < this.bufferCount; k++)
					{
						if (this.rayCastNonAllocColliders[k].collider != null && this.rayCastNonAllocColliders[k].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[k];
						}
					}
					finalPosition = startPosition + this.tempHitInfo.distance * (finalPosition - startPosition).normalized;
					collisionsHitInfo = this.tempHitInfo;
				}
				return true;
			}
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.RaycastNonAlloc(startPosition, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int l = 0; l < this.bufferCount; l++)
				{
					if (this.rayCastNonAllocColliders[l].collider != null && this.rayCastNonAllocColliders[l].distance < this.tempHitInfo.distance)
					{
						this.tempHitInfo = this.rayCastNonAllocColliders[l];
					}
				}
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = startPosition;
				return true;
			}
			finalPosition = startPosition + movementVector;
			collisionsHitInfo = default(RaycastHit);
			return false;
		}

		// Token: 0x06006C93 RID: 27795 RVA: 0x00235078 File Offset: 0x00233278
		public float GetSlidePercentage(RaycastHit raycastHit)
		{
			if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				return this.FreezeTagSlidePercentage();
			}
			this.currentOverride = raycastHit.collider.gameObject.GetComponent<GorillaSurfaceOverride>();
			BasePlatform component = raycastHit.collider.gameObject.GetComponent<BasePlatform>();
			if (component != null)
			{
				this.currentPlatform = component;
			}
			if (this.currentOverride != null)
			{
				if (this.currentOverride.slidePercentageOverride >= 0f)
				{
					return this.currentOverride.slidePercentageOverride;
				}
				this.currentMaterialIndex = this.currentOverride.overrideIndex;
				if (this.currentMaterialIndex < 0 || this.currentMaterialIndex >= this.materialData.Count)
				{
					return this.defaultSlideFactor;
				}
				if (!this.materialData[this.currentMaterialIndex].overrideSlidePercent)
				{
					return this.defaultSlideFactor;
				}
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			else
			{
				this.meshCollider = (raycastHit.collider as MeshCollider);
				if (this.meshCollider == null || this.meshCollider.sharedMesh == null || this.meshCollider.convex)
				{
					return this.defaultSlideFactor;
				}
				this.collidedMesh = this.meshCollider.sharedMesh;
				if (!this.meshTrianglesDict.TryGetValue(this.collidedMesh, out this.sharedMeshTris))
				{
					this.sharedMeshTris = this.collidedMesh.triangles;
					this.meshTrianglesDict.Add(this.collidedMesh, (int[])this.sharedMeshTris.Clone());
				}
				this.vertex1 = this.sharedMeshTris[raycastHit.triangleIndex * 3];
				this.vertex2 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 1];
				this.vertex3 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 2];
				this.slideRenderer = raycastHit.collider.GetComponent<Renderer>();
				if (this.slideRenderer != null)
				{
					this.slideRenderer.GetSharedMaterials(this.tempMaterialArray);
				}
				else
				{
					this.tempMaterialArray.Clear();
				}
				if (this.tempMaterialArray.Count > 1)
				{
					for (int i = 0; i < this.tempMaterialArray.Count; i++)
					{
						this.collidedMesh.GetTriangles(this.trianglesList, i);
						int j = 0;
						while (j < this.trianglesList.Count)
						{
							if (this.trianglesList[j] == this.vertex1 && this.trianglesList[j + 1] == this.vertex2 && this.trianglesList[j + 2] == this.vertex3)
							{
								this.findMatName = this.tempMaterialArray[i].name;
								if (this.findMatName.EndsWith("Uber"))
								{
									string text = this.findMatName;
									this.findMatName = text.Substring(0, text.Length - 4);
								}
								this.foundMatData = this.materialData.Find((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								this.currentMaterialIndex = this.materialData.FindIndex((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								if (this.currentMaterialIndex == -1)
								{
									this.currentMaterialIndex = 0;
								}
								if (!this.foundMatData.overrideSlidePercent)
								{
									return this.defaultSlideFactor;
								}
								return this.foundMatData.slidePercent;
							}
							else
							{
								j += 3;
							}
						}
					}
				}
				else if (this.tempMaterialArray.Count > 0)
				{
					return this.defaultSlideFactor;
				}
				this.currentMaterialIndex = 0;
				return this.defaultSlideFactor;
			}
		}

		// Token: 0x06006C94 RID: 27796 RVA: 0x00235410 File Offset: 0x00233610
		public bool IsTouchingMovingSurface(Vector3 rayOrigin, RaycastHit raycastHit, out int movingSurfaceId, out bool sideTouch, out bool isMonkeBlock)
		{
			movingSurfaceId = -1;
			sideTouch = false;
			isMonkeBlock = false;
			float num = Vector3.Dot(rayOrigin - raycastHit.point, Vector3.up);
			if (num < -0.3f)
			{
				return false;
			}
			if (num < 0f)
			{
				sideTouch = true;
			}
			if (raycastHit.collider == null)
			{
				return false;
			}
			MovingSurface component = raycastHit.collider.GetComponent<MovingSurface>();
			if (component != null)
			{
				isMonkeBlock = false;
				movingSurfaceId = component.GetID();
				return true;
			}
			if (!BuilderTable.IsLocalPlayerInBuilderZone())
			{
				return false;
			}
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(raycastHit.collider);
			if (builderPieceFromCollider != null && builderPieceFromCollider.IsPieceMoving())
			{
				isMonkeBlock = true;
				movingSurfaceId = builderPieceFromCollider.pieceId;
				this.lastMonkeBlock = builderPieceFromCollider;
				return true;
			}
			sideTouch = false;
			return false;
		}

		// Token: 0x06006C95 RID: 27797 RVA: 0x002354CC File Offset: 0x002336CC
		public void Turn(float degrees)
		{
			Vector3 position = this.headCollider.transform.position;
			bool flag = this.rightHand.isColliding || this.rightHand.isHolding;
			bool flag2 = this.leftHand.isColliding || this.leftHand.isHolding;
			if (flag != flag2 && flag)
			{
				position = this.rightHand.controllerTransform.position;
			}
			if (flag != flag2 && flag2)
			{
				position = this.leftHand.controllerTransform.position;
			}
			this.turnParent.transform.RotateAround(position, GTPlayerTransform.Up, degrees);
			this.degreesTurnedThisFrame = degrees;
			this.averagedVelocity = Vector3.zero;
			Quaternion rotation = Quaternion.AngleAxis(degrees, GTPlayerTransform.Up);
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = rotation * this.velocityHistory[i];
				this.averagedVelocity += this.velocityHistory[i];
			}
			this.averagedVelocity /= (float)this.velocityHistorySize;
		}

		// Token: 0x06006C96 RID: 27798 RVA: 0x002355FC File Offset: 0x002337FC
		public void BeginClimbing(GorillaClimbable climbable, GorillaHandClimber hand, GorillaClimbableRef climbableRef = null)
		{
			if (this.currentClimber != null)
			{
				this.EndClimbing(this.currentClimber, true, false);
			}
			try
			{
				Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb = climbable.onBeforeClimb;
				if (onBeforeClimb != null)
				{
					onBeforeClimb(hand, climbableRef);
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			Rigidbody rigidbody;
			climbable.TryGetComponent<Rigidbody>(out rigidbody);
			this.VerifyClimbHelper();
			this.climbHelper.SetParent(climbable.transform);
			this.climbHelper.position = hand.transform.position;
			Vector3 localPosition = this.climbHelper.localPosition;
			if (climbable.snapX)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|447_0(ref localPosition.x, climbable.maxDistanceSnap);
			}
			if (climbable.snapY)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|447_0(ref localPosition.y, climbable.maxDistanceSnap);
			}
			if (climbable.snapZ)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|447_0(ref localPosition.z, climbable.maxDistanceSnap);
			}
			this.climbHelperTargetPos = localPosition;
			climbable.isBeingClimbed = true;
			hand.isClimbing = true;
			this.currentClimbable = climbable;
			this.currentClimber = hand;
			this.isClimbing = true;
			if (climbable.climbOnlyWhileSmall)
			{
				BuilderPiece componentInParent = climbable.GetComponentInParent<BuilderPiece>();
				if (componentInParent != null && componentInParent.IsPieceMoving())
				{
					this.isClimbableMoving = true;
					this.lastClimbableRotation = climbable.transform.rotation;
				}
				else
				{
					this.isClimbableMoving = false;
				}
			}
			else
			{
				this.isClimbableMoving = false;
			}
			GorillaRopeSegment gorillaRopeSegment;
			GorillaZipline gorillaZipline;
			PhotonView view;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (climbable.TryGetComponent<GorillaRopeSegment>(out gorillaRopeSegment) && gorillaRopeSegment.swing)
			{
				this.currentSwing = gorillaRopeSegment.swing;
				this.currentSwing.AttachLocalPlayer(hand.xrNode, climbable.transform, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.transform.parent && climbable.transform.parent.TryGetComponent<GorillaZipline>(out gorillaZipline))
			{
				this.currentZipline = gorillaZipline;
			}
			else if (climbable.TryGetComponent<PhotonView>(out view))
			{
				VRRig.AttachLocalPlayerToPhotonView(view, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef))
			{
				VRRig.AttachLocalPlayerToPhotonView(photonViewXSceneRef.photonView, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			GorillaTagger.Instance.StartVibration(this.currentClimber.xrNode == XRNode.LeftHand, 0.6f, 0.06f);
			if (climbable.clip)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(climbable.clip, hand.xrNode == XRNode.LeftHand);
			}
		}

		// Token: 0x06006C97 RID: 27799 RVA: 0x00235868 File Offset: 0x00233A68
		private void VerifyClimbHelper()
		{
			if (this.climbHelper == null || this.climbHelper.gameObject == null)
			{
				this.climbHelper = new GameObject("Climb Helper").transform;
			}
		}

		// Token: 0x06006C98 RID: 27800 RVA: 0x002358A0 File Offset: 0x00233AA0
		public void EndClimbing(GorillaHandClimber hand, bool startingNewClimb, bool doDontReclimb = false)
		{
			if (hand != this.currentClimber)
			{
				return;
			}
			hand.SetCanRelease(true);
			if (!startingNewClimb)
			{
				this.enablePlayerGravity(true);
			}
			Rigidbody rigidbody = null;
			if (this.currentClimbable)
			{
				this.currentClimbable.TryGetComponent<Rigidbody>(out rigidbody);
				this.currentClimbable.isBeingClimbed = false;
			}
			Vector3 vector = Vector3.zero;
			if (this.currentClimber)
			{
				this.currentClimber.isClimbing = false;
				if (doDontReclimb)
				{
					this.currentClimber.dontReclimbLast = this.currentClimbable;
				}
				else
				{
					this.currentClimber.dontReclimbLast = null;
				}
				this.currentClimber.queuedToBecomeValidToGrabAgain = true;
				this.currentClimber.lastAutoReleasePos = this.currentClimber.handRoot.localPosition;
				if (!startingNewClimb && this.currentClimbable)
				{
					GorillaVelocityTracker interactPointVelocityTracker = this.GetInteractPointVelocityTracker(this.currentClimber.xrNode == XRNode.LeftHand);
					if (rigidbody)
					{
						this.playerRigidBody.linearVelocity = rigidbody.linearVelocity;
					}
					else if (this.currentSwing)
					{
						this.playerRigidBody.linearVelocity = this.currentSwing.velocityTracker.GetAverageVelocity(true, 0.25f, false);
					}
					else if (this.currentZipline)
					{
						this.playerRigidBody.linearVelocity = this.currentZipline.GetCurrentDirection() * this.currentZipline.currentSpeed;
					}
					else
					{
						this.playerRigidBody.linearVelocity = Vector3.zero;
					}
					vector = this.turnParent.transform.rotation * -interactPointVelocityTracker.GetAverageVelocity(false, 0.1f, true) * this.scale;
					vector = Vector3.ClampMagnitude(vector, 5.5f * this.scale);
					this.playerRigidBody.AddForce(vector, ForceMode.VelocityChange);
				}
			}
			if (this.currentSwing)
			{
				this.currentSwing.DetachLocalPlayer();
			}
			PhotonView photonView;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (this.currentClimbable.TryGetComponent<PhotonView>(out photonView) || this.currentClimbable.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef) || this.currentClimbable.IsPlayerAttached)
			{
				VRRig.DetachLocalPlayerFromPhotonView();
			}
			if (!startingNewClimb && vector.magnitude > 2f && this.currentClimbable && this.currentClimbable.clipOnFullRelease)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(this.currentClimbable.clipOnFullRelease, hand.xrNode == XRNode.LeftHand);
			}
			this.currentClimbable = null;
			this.currentClimber = null;
			this.currentSwing = null;
			this.currentZipline = null;
			this.isClimbing = false;
		}

		// Token: 0x06006C99 RID: 27801 RVA: 0x00235B30 File Offset: 0x00233D30
		public void ResetRigidbodyInterpolation()
		{
			this.playerRigidBody.interpolation = this.playerRigidbodyInterpolationDefault;
		}

		// Token: 0x17000A79 RID: 2681
		// (get) Token: 0x06006C9A RID: 27802 RVA: 0x00235B43 File Offset: 0x00233D43
		// (set) Token: 0x06006C9B RID: 27803 RVA: 0x00235B50 File Offset: 0x00233D50
		public RigidbodyInterpolation RigidbodyInterpolation
		{
			get
			{
				return this.playerRigidBody.interpolation;
			}
			set
			{
				this.playerRigidBody.interpolation = value;
			}
		}

		// Token: 0x06006C9C RID: 27804 RVA: 0x00235B5E File Offset: 0x00233D5E
		private void enablePlayerGravity(bool useGravity)
		{
			this.playerRigidBody.useGravity = useGravity;
		}

		// Token: 0x06006C9D RID: 27805 RVA: 0x00235B6C File Offset: 0x00233D6C
		public void SetVelocity(Vector3 velocity)
		{
			this.playerRigidBody.linearVelocity = velocity;
		}

		// Token: 0x06006C9E RID: 27806 RVA: 0x00235B7A File Offset: 0x00233D7A
		internal void RigidbodyMovePosition(Vector3 pos)
		{
			this.playerRigidBody.MovePosition(pos);
		}

		// Token: 0x06006C9F RID: 27807 RVA: 0x00235B88 File Offset: 0x00233D88
		public void TempFreezeHand(bool isLeft, float freezeDuration)
		{
			(isLeft ? this.leftHand : this.rightHand).TempFreezeHand(freezeDuration);
		}

		// Token: 0x06006CA0 RID: 27808 RVA: 0x00235BB0 File Offset: 0x00233DB0
		private void StoreVelocities()
		{
			this.velocityIndex = (this.velocityIndex + 1) % this.velocityHistorySize;
			this.currentVelocity = (base.transform.position - this.lastPosition - GTPlayerTransform.RotationPosOffsetChange - this.MovingSurfaceMovement()) / this.calcDeltaTime;
			this.velocityHistory[this.velocityIndex] = this.currentVelocity;
			this.averagedVelocity = this.velocityHistory.Average();
			this.lastPosition = base.transform.position;
			GTPlayerTransform.ResetRotationPositionOffset();
		}

		// Token: 0x06006CA1 RID: 27809 RVA: 0x00235C4C File Offset: 0x00233E4C
		private void AntiTeleportTechnology()
		{
			if ((this.headCollider.transform.position - this.lastHeadPosition).magnitude >= this.teleportThresholdNoVel + this.playerRigidBody.linearVelocity.magnitude * this.calcDeltaTime)
			{
				this.ForceRigidBodySync();
				base.transform.position = base.transform.position + this.lastHeadPosition - this.headCollider.transform.position;
			}
		}

		// Token: 0x06006CA2 RID: 27810 RVA: 0x00235CDC File Offset: 0x00233EDC
		private bool MaxSphereSizeForNoOverlap(float testRadius, Vector3 checkPosition, bool ignoreOneWay, out float overlapRadiusTest)
		{
			overlapRadiusTest = testRadius;
			this.overlapAttempts = 0;
			int num = 100;
			while (this.overlapAttempts < num && overlapRadiusTest > testRadius * 0.75f)
			{
				this.ClearColliderBuffer(ref this.overlapColliders);
				this.bufferCount = Physics.OverlapSphereNonAlloc(checkPosition, overlapRadiusTest, this.overlapColliders, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				if (ignoreOneWay)
				{
					int num2 = 0;
					for (int i = 0; i < this.bufferCount; i++)
					{
						if (this.overlapColliders[i].CompareTag("NoCrazyCheck"))
						{
							num2++;
						}
					}
					if (num2 == this.bufferCount)
					{
						return true;
					}
				}
				if (this.bufferCount <= 0)
				{
					overlapRadiusTest *= 0.995f;
					return true;
				}
				overlapRadiusTest = Mathf.Lerp(testRadius, 0f, (float)this.overlapAttempts / (float)num);
				this.overlapAttempts++;
			}
			return false;
		}

		// Token: 0x06006CA3 RID: 27811 RVA: 0x00235DBC File Offset: 0x00233FBC
		private bool CrazyCheck2(float sphereSize, Vector3 startPosition)
		{
			for (int i = 0; i < this.crazyCheckVectors.Length; i++)
			{
				if (this.NonAllocRaycast(startPosition, startPosition + this.crazyCheckVectors[i] * sphereSize) > 0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006CA4 RID: 27812 RVA: 0x00235E04 File Offset: 0x00234004
		private int NonAllocRaycast(Vector3 startPosition, Vector3 endPosition)
		{
			Vector3 direction = endPosition - startPosition;
			int num = Physics.RaycastNonAlloc(startPosition, direction, this.rayCastNonAllocColliders, direction.magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (!this.rayCastNonAllocColliders[i].collider.gameObject.CompareTag("NoCrazyCheck"))
				{
					num2++;
				}
			}
			return num2;
		}

		// Token: 0x06006CA5 RID: 27813 RVA: 0x00235E70 File Offset: 0x00234070
		private void ClearColliderBuffer(ref Collider[] colliders)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i] = null;
			}
		}

		// Token: 0x06006CA6 RID: 27814 RVA: 0x00235E94 File Offset: 0x00234094
		private void ClearRaycasthitBuffer(ref RaycastHit[] raycastHits)
		{
			for (int i = 0; i < raycastHits.Length; i++)
			{
				raycastHits[i] = this.emptyHit;
			}
		}

		// Token: 0x06006CA7 RID: 27815 RVA: 0x00235EBE File Offset: 0x002340BE
		private Vector3 MovingSurfaceMovement()
		{
			return this.refMovement + this.movingSurfaceOffset;
		}

		// Token: 0x06006CA8 RID: 27816 RVA: 0x00235ED4 File Offset: 0x002340D4
		private static bool ComputeLocalHitPoint(RaycastHit hit, out Vector3 localHitPoint)
		{
			if (hit.collider == null || hit.point.sqrMagnitude < 0.001f)
			{
				localHitPoint = Vector3.zero;
				return false;
			}
			localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
			return true;
		}

		// Token: 0x06006CA9 RID: 27817 RVA: 0x00235F32 File Offset: 0x00234132
		private static bool ComputeWorldHitPoint(RaycastHit hit, Vector3 localPoint, out Vector3 worldHitPoint)
		{
			if (hit.collider == null)
			{
				worldHitPoint = Vector3.zero;
				return false;
			}
			worldHitPoint = hit.collider.transform.TransformPoint(localPoint);
			return true;
		}

		// Token: 0x06006CAA RID: 27818 RVA: 0x00235F6C File Offset: 0x0023416C
		private float ExtraVelMultiplier()
		{
			float num = 1f;
			if (this.leftHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHand.surfaceOverride.extraVelMultiplier);
			}
			if (this.rightHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHand.surfaceOverride.extraVelMultiplier);
			}
			return num;
		}

		// Token: 0x06006CAB RID: 27819 RVA: 0x00235FD4 File Offset: 0x002341D4
		private float ExtraVelMaxMultiplier()
		{
			float num = 1f;
			if (this.leftHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHand.surfaceOverride.extraVelMaxMultiplier);
			}
			if (this.rightHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHand.surfaceOverride.extraVelMaxMultiplier);
			}
			return num * this.scale;
		}

		// Token: 0x06006CAC RID: 27820 RVA: 0x00236045 File Offset: 0x00234245
		public void SetMaximumSlipThisFrame()
		{
			this.leftHand.slipSetToMaxFrameIdx = Time.frameCount;
			this.rightHand.slipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06006CAD RID: 27821 RVA: 0x00236067 File Offset: 0x00234267
		public void SetLeftMaximumSlipThisFrame()
		{
			this.leftHand.slipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06006CAE RID: 27822 RVA: 0x00236079 File Offset: 0x00234279
		public void SetRightMaximumSlipThisFrame()
		{
			this.rightHand.slipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06006CAF RID: 27823 RVA: 0x0023608B File Offset: 0x0023428B
		public void ChangeLayer(string layerName)
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.ChangeLayer(base.transform.parent, layerName);
			}
		}

		// Token: 0x06006CB0 RID: 27824 RVA: 0x002360B2 File Offset: 0x002342B2
		public void RestoreLayer()
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.RestoreOriginalLayers();
			}
		}

		// Token: 0x06006CB1 RID: 27825 RVA: 0x002360D0 File Offset: 0x002342D0
		public void OnEnterWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireInWater)
			{
				this.SetNativeScale(null);
			}
			if (playerCollider == this.headCollider)
			{
				if (!this.headOverlappingWaterVolumes.Contains(volume))
				{
					this.headOverlappingWaterVolumes.Add(volume);
					return;
				}
			}
			else if (playerCollider == this.bodyCollider && !this.bodyOverlappingWaterVolumes.Contains(volume))
			{
				this.bodyOverlappingWaterVolumes.Add(volume);
			}
		}

		// Token: 0x06006CB2 RID: 27826 RVA: 0x0023614A File Offset: 0x0023434A
		public void OnExitWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (playerCollider == this.headCollider)
			{
				this.headOverlappingWaterVolumes.Remove(volume);
				return;
			}
			if (playerCollider == this.bodyCollider)
			{
				this.bodyOverlappingWaterVolumes.Remove(volume);
			}
		}

		// Token: 0x06006CB3 RID: 27827 RVA: 0x00236184 File Offset: 0x00234384
		private bool GetSwimmingVelocityForHand(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, float dt, ref WaterVolume contactingWaterVolume, ref WaterVolume.SurfaceQuery waterSurface, out Vector3 swimmingVelocityChange)
		{
			contactingWaterVolume = null;
			this.bufferCount = Physics.OverlapSphereNonAlloc(endingHandPosition, this.minimumRaycastDistance, this.overlapColliders, this.waterLayer.value, QueryTriggerInteraction.Collide);
			if (this.bufferCount > 0)
			{
				float num = float.MinValue;
				for (int i = 0; i < this.bufferCount; i++)
				{
					WaterVolume component = this.overlapColliders[i].GetComponent<WaterVolume>();
					WaterVolume.SurfaceQuery surfaceQuery;
					if (component != null && component.GetSurfaceQueryForPoint(endingHandPosition, out surfaceQuery, false))
					{
						float num2 = Vector3.Dot(surfaceQuery.surfacePoint, GTPlayerTransform.PhysicsUp);
						if (num2 > num)
						{
							num = num2;
							contactingWaterVolume = component;
							waterSurface = surfaceQuery;
						}
					}
				}
			}
			if (this.forcedUnderwater || contactingWaterVolume != null)
			{
				Vector3 a = endingHandPosition - startingHandPosition;
				Vector3 b = Vector3.zero;
				Vector3 b2 = this.playerRigidBody.transform.position - this.lastRigidbodyPosition;
				if (this.turnedThisFrame)
				{
					Vector3 vector = startingHandPosition - this.headCollider.transform.position;
					b = Quaternion.AngleAxis(this.degreesTurnedThisFrame, GTPlayerTransform.PhysicsUp) * vector - vector;
				}
				float num3 = Vector3.Dot(a - b - b2, palmForwardDirection);
				float num4 = 0f;
				if (num3 > 0f)
				{
					float num5 = -1f;
					float num6 = -1f;
					if (!this.forcedUnderwater)
					{
						Plane surfacePlane = waterSurface.surfacePlane;
						num5 = (this.forcedUnderwater ? -1f : surfacePlane.GetDistanceToPoint(startingHandPosition));
						num6 = (this.forcedUnderwater ? -1f : surfacePlane.GetDistanceToPoint(endingHandPosition));
					}
					if (num5 <= 0f && num6 <= 0f)
					{
						num4 = 1f;
					}
					else if (num5 > 0f && num6 <= 0f)
					{
						num4 = -num6 / (num5 - num6);
					}
					else if (num5 <= 0f && num6 > 0f)
					{
						num4 = -num5 / (num6 - num5);
					}
					if (num4 > Mathf.Epsilon)
					{
						float resistance = this.liquidPropertiesList[(int)(this.forcedUnderwater ? GTPlayer.LiquidType.Water : contactingWaterVolume.LiquidType)].resistance;
						swimmingVelocityChange = -palmForwardDirection * num3 * 2f * resistance * num4;
						Vector3 forward = this.mainCamera.transform.forward;
						if (Vector3.Dot(forward, GTPlayerTransform.PhysicsDown) > 0f)
						{
							Vector3 vector2 = Vector3.ProjectOnPlane(forward, GTPlayerTransform.PhysicsUp);
							float magnitude = vector2.magnitude;
							vector2 /= magnitude;
							float num7 = Vector3.Dot(swimmingVelocityChange, vector2);
							if (num7 > 0f)
							{
								Vector3 vector3 = vector2 * num7;
								float d = Vector3.Dot(forward, GTPlayerTransform.PhysicsUp);
								swimmingVelocityChange = swimmingVelocityChange - vector3 + vector3 * magnitude + GTPlayerTransform.PhysicsUp * d * num7;
							}
						}
						return true;
					}
				}
			}
			swimmingVelocityChange = Vector3.zero;
			return false;
		}

		// Token: 0x06006CB4 RID: 27828 RVA: 0x00236498 File Offset: 0x00234698
		private bool CheckWaterSurfaceJump(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, Vector3 handAvgVelocity, PlayerSwimmingParameters parameters, WaterVolume contactingWaterVolume, WaterVolume.SurfaceQuery waterSurface, out Vector3 jumpVelocity)
		{
			if (contactingWaterVolume != null)
			{
				Plane surfacePlane = waterSurface.surfacePlane;
				bool flag = handAvgVelocity.sqrMagnitude > parameters.waterSurfaceJumpHandSpeedThreshold * parameters.waterSurfaceJumpHandSpeedThreshold;
				if (surfacePlane.GetSide(startingHandPosition) && !surfacePlane.GetSide(endingHandPosition) && flag)
				{
					float value = Vector3.Dot(palmForwardDirection, -waterSurface.surfaceNormal);
					float value2 = Vector3.Dot(handAvgVelocity.normalized, -waterSurface.surfaceNormal);
					float d = parameters.waterSurfaceJumpPalmFacingCurve.Evaluate(Mathf.Clamp(value, 0.01f, 0.99f));
					float d2 = parameters.waterSurfaceJumpHandVelocityFacingCurve.Evaluate(Mathf.Clamp(value2, 0.01f, 0.99f));
					jumpVelocity = -handAvgVelocity * parameters.waterSurfaceJumpAmount * d * d2;
					return true;
				}
			}
			jumpVelocity = Vector3.zero;
			return false;
		}

		// Token: 0x06006CB5 RID: 27829 RVA: 0x00236591 File Offset: 0x00234791
		private bool TryNormalize(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > eps)
			{
				normalized = input / magnitude;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x06006CB6 RID: 27830 RVA: 0x002365BE File Offset: 0x002347BE
		private bool TryNormalizeDown(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > 1f)
			{
				normalized = input / magnitude;
				return true;
			}
			if (magnitude >= eps)
			{
				normalized = input;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x06006CB7 RID: 27831 RVA: 0x00236600 File Offset: 0x00234800
		private float FreezeTagSlidePercentage()
		{
			if (this.materialData[this.currentMaterialIndex].overrideSlidePercent && this.materialData[this.currentMaterialIndex].slidePercent > this.freezeTagHandSlidePercent)
			{
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			return this.freezeTagHandSlidePercent;
		}

		// Token: 0x06006CB8 RID: 27832 RVA: 0x00236660 File Offset: 0x00234860
		private void OnCollisionStay(UnityEngine.Collision collision)
		{
			this.bodyCollisionContactsCount = collision.GetContacts(this.bodyCollisionContacts);
			float num = -1f;
			for (int i = 0; i < this.bodyCollisionContactsCount; i++)
			{
				float num2 = Vector3.Dot(this.bodyCollisionContacts[i].normal, Vector3.up);
				if (num2 > num)
				{
					this.bodyGroundContact = this.bodyCollisionContacts[i];
					num = num2;
				}
			}
			float num3 = 0.5f;
			if (num > num3)
			{
				this.bodyGroundContactTime = Time.time;
				Collider otherCollider = this.bodyGroundContact.otherCollider;
				this.bodyGroundIsSlippery = (otherCollider != null && otherCollider.sharedMaterial != null && otherCollider.sharedMaterial.staticFriction <= 0.0001f && otherCollider.sharedMaterial.dynamicFriction <= 0.0001f);
			}
		}

		// Token: 0x06006CB9 RID: 27833 RVA: 0x00236740 File Offset: 0x00234940
		public void DoLaunch(Vector3 velocity)
		{
			GTPlayer.<DoLaunch>d__483 <DoLaunch>d__;
			<DoLaunch>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DoLaunch>d__.<>4__this = this;
			<DoLaunch>d__.velocity = velocity;
			<DoLaunch>d__.<>1__state = -1;
			<DoLaunch>d__.<>t__builder.Start<GTPlayer.<DoLaunch>d__483>(ref <DoLaunch>d__);
		}

		// Token: 0x06006CBA RID: 27834 RVA: 0x0023677F File Offset: 0x0023497F
		private void OnEnable()
		{
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		}

		// Token: 0x06006CBB RID: 27835 RVA: 0x0023679C File Offset: 0x0023499C
		private void OnJoinedRoom()
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireOnRoomJoin)
			{
				this.SetNativeScale(null);
			}
		}

		// Token: 0x06006CBC RID: 27836 RVA: 0x002367BA File Offset: 0x002349BA
		private void OnDisable()
		{
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
		}

		// Token: 0x06006CBD RID: 27837 RVA: 0x002367D7 File Offset: 0x002349D7
		public void ForceRigidBodySync()
		{
			this.forceRBSync = true;
		}

		// Token: 0x06006CBE RID: 27838 RVA: 0x002367E0 File Offset: 0x002349E0
		internal void ClearHandHolds()
		{
			this.leftHand.isHolding = false;
			this.rightHand.isHolding = false;
			this.wasHoldingHandhold = false;
			this.activeHandHold = default(GTPlayer.HandHoldState);
			this.secondaryHandHold = default(GTPlayer.HandHoldState);
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06006CBF RID: 27839 RVA: 0x00236820 File Offset: 0x00234A20
		internal void AddHandHold(Transform objectHeld, Vector3 localPositionHeld, GorillaGrabber grabber, bool forLeftHand, bool rotatePlayerWhenHeld, out Vector3 grabbedVelocity)
		{
			if (!this.leftHand.isHolding && !this.rightHand.isHolding)
			{
				grabbedVelocity = -this.bodyCollider.attachedRigidbody.linearVelocity;
				this.playerRigidBody.AddForce(grabbedVelocity, ForceMode.VelocityChange);
			}
			else
			{
				grabbedVelocity = Vector3.zero;
			}
			this.secondaryHandHold = this.activeHandHold;
			Vector3 position = grabber.transform.position;
			this.activeHandHold = new GTPlayer.HandHoldState
			{
				grabber = grabber,
				objectHeld = objectHeld,
				localPositionHeld = localPositionHeld,
				localRotationalOffset = grabber.transform.rotation.eulerAngles.y - objectHeld.rotation.eulerAngles.y,
				applyRotation = rotatePlayerWhenHeld
			};
			if (forLeftHand)
			{
				this.leftHand.isHolding = true;
			}
			else
			{
				this.rightHand.isHolding = true;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06006CC0 RID: 27840 RVA: 0x00236924 File Offset: 0x00234B24
		internal void RemoveHandHold(GorillaGrabber grabber, bool forLeftHand)
		{
			this.activeHandHold.objectHeld == grabber;
			if (this.activeHandHold.grabber == grabber)
			{
				this.activeHandHold = this.secondaryHandHold;
			}
			this.secondaryHandHold = default(GTPlayer.HandHoldState);
			if (forLeftHand)
			{
				this.leftHand.isHolding = false;
			}
			else
			{
				this.rightHand.isHolding = false;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06006CC1 RID: 27841 RVA: 0x00236994 File Offset: 0x00234B94
		private void OnChangeActiveHandhold()
		{
			if (this.activeHandHold.objectHeld != null)
			{
				PhotonView view;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonView>(out view))
				{
					VRRig.AttachLocalPlayerToPhotonView(view, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
					return;
				}
				PhotonViewXSceneRef photonViewXSceneRef;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef))
				{
					PhotonView photonView = photonViewXSceneRef.photonView;
					if (photonView != null)
					{
						VRRig.AttachLocalPlayerToPhotonView(photonView, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
						return;
					}
				}
				BuilderPieceHandHold builderPieceHandHold;
				if (this.activeHandHold.objectHeld.TryGetComponent<BuilderPieceHandHold>(out builderPieceHandHold) && builderPieceHandHold.IsHandHoldMoving())
				{
					this.isHandHoldMoving = true;
					this.lastHandHoldRotation = builderPieceHandHold.transform.rotation;
					this.movingHandHoldReleaseVelocity = this.playerRigidBody.linearVelocity;
				}
				else
				{
					this.isHandHoldMoving = false;
					this.lastHandHoldRotation = Quaternion.identity;
					this.movingHandHoldReleaseVelocity = Vector3.zero;
				}
			}
			VRRig.DetachLocalPlayerFromPhotonView();
		}

		// Token: 0x06006CC2 RID: 27842 RVA: 0x00236AA4 File Offset: 0x00234CA4
		private void FixedUpdate_HandHolds(float timeDelta)
		{
			if (this.activeHandHold.objectHeld == null)
			{
				if (this.wasHoldingHandhold)
				{
					this.playerRigidBody.linearVelocity = Vector3.ClampMagnitude(this.secondLastPreHandholdVelocity, 5.5f * this.scale);
				}
				this.wasHoldingHandhold = false;
				return;
			}
			Vector3 vector = this.activeHandHold.objectHeld.TransformPoint(this.activeHandHold.localPositionHeld);
			Vector3 position = this.activeHandHold.grabber.transform.position;
			this.secondLastPreHandholdVelocity = this.lastPreHandholdVelocity;
			this.lastPreHandholdVelocity = this.playerRigidBody.linearVelocity;
			this.wasHoldingHandhold = true;
			if (this.isHandHoldMoving)
			{
				this.lastPreHandholdVelocity = this.movingHandHoldReleaseVelocity;
				this.playerRigidBody.linearVelocity = Vector3.zero;
				Vector3 vector2 = vector - position;
				this.playerRigidBody.transform.position += vector2;
				this.movingHandHoldReleaseVelocity = vector2 / timeDelta;
				Quaternion rotationDelta = this.activeHandHold.objectHeld.rotation * Quaternion.Inverse(this.lastHandHoldRotation);
				this.RotateWithSurface(rotationDelta, vector);
				this.lastHandHoldRotation = this.activeHandHold.objectHeld.rotation;
				return;
			}
			this.playerRigidBody.linearVelocity = (vector - position) / timeDelta;
			if (this.activeHandHold.applyRotation)
			{
				this.turnParent.transform.RotateAround(vector, base.transform.up, this.activeHandHold.localRotationalOffset - (this.activeHandHold.grabber.transform.rotation.eulerAngles.y - this.activeHandHold.objectHeld.rotation.eulerAngles.y));
			}
		}

		// Token: 0x06006CC7 RID: 27847 RVA: 0x00237073 File Offset: 0x00235273
		[CompilerGenerated]
		internal static void <BeginClimbing>g__SnapAxis|447_0(ref float val, float maxDist)
		{
			if (val > maxDist)
			{
				val = maxDist;
				return;
			}
			if (val < -maxDist)
			{
				val = -maxDist;
			}
		}

		// Token: 0x04007C46 RID: 31814
		public static LayerMask LocomotionEnabledLayers = 201327105;

		// Token: 0x04007C47 RID: 31815
		private static GTPlayer _instance;

		// Token: 0x04007C48 RID: 31816
		public static bool hasInstance = false;

		// Token: 0x04007C49 RID: 31817
		public Camera mainCamera;

		// Token: 0x04007C4A RID: 31818
		public SphereCollider headCollider;

		// Token: 0x04007C4B RID: 31819
		public CapsuleCollider bodyCollider;

		// Token: 0x04007C4C RID: 31820
		private float bodyInitialRadius;

		// Token: 0x04007C4D RID: 31821
		private float _bodyInitialHeight;

		// Token: 0x04007C4E RID: 31822
		private float currentBodyHeight;

		// Token: 0x04007C4F RID: 31823
		private double frameCount;

		// Token: 0x04007C50 RID: 31824
		private RaycastHit bodyHitInfo;

		// Token: 0x04007C51 RID: 31825
		private RaycastHit lastHitInfoHand;

		// Token: 0x04007C52 RID: 31826
		public GorillaVelocityTracker bodyVelocityTracker;

		// Token: 0x04007C53 RID: 31827
		public PlayerAudioManager audioManager;

		// Token: 0x04007C54 RID: 31828
		[SerializeField]
		private GTPlayer.HandState leftHand;

		// Token: 0x04007C55 RID: 31829
		[SerializeField]
		private GTPlayer.HandState rightHand;

		// Token: 0x04007C56 RID: 31830
		private GTPlayer.HandState[] stiltStates = new GTPlayer.HandState[12];

		// Token: 0x04007C57 RID: 31831
		private bool anyHandIsColliding;

		// Token: 0x04007C58 RID: 31832
		private bool anyHandWasColliding;

		// Token: 0x04007C59 RID: 31833
		private bool anyHandIsSliding;

		// Token: 0x04007C5A RID: 31834
		private bool anyHandWasSliding;

		// Token: 0x04007C5B RID: 31835
		private bool anyHandIsSticking;

		// Token: 0x04007C5C RID: 31836
		private bool anyHandWasSticking;

		// Token: 0x04007C5D RID: 31837
		private bool forceRBSync;

		// Token: 0x04007C5E RID: 31838
		public Vector3 lastHeadPosition;

		// Token: 0x04007C5F RID: 31839
		private Vector3 lastRigidbodyPosition;

		// Token: 0x04007C61 RID: 31841
		private RigidbodyInterpolation playerRigidbodyInterpolationDefault;

		// Token: 0x04007C62 RID: 31842
		public int velocityHistorySize;

		// Token: 0x04007C63 RID: 31843
		public float maxArmLength = 1f;

		// Token: 0x04007C64 RID: 31844
		public float unStickDistance = 1f;

		// Token: 0x04007C65 RID: 31845
		public float velocityLimit;

		// Token: 0x04007C66 RID: 31846
		public float slideVelocityLimit;

		// Token: 0x04007C67 RID: 31847
		public float maxJumpSpeed;

		// Token: 0x04007C68 RID: 31848
		private float _jumpMultiplier;

		// Token: 0x04007C69 RID: 31849
		public float minimumRaycastDistance = 0.05f;

		// Token: 0x04007C6A RID: 31850
		public float defaultSlideFactor = 0.03f;

		// Token: 0x04007C6B RID: 31851
		public float slidingMinimum = 0.9f;

		// Token: 0x04007C6C RID: 31852
		public float defaultPrecision = 0.995f;

		// Token: 0x04007C6D RID: 31853
		public float teleportThresholdNoVel = 1f;

		// Token: 0x04007C6E RID: 31854
		public float frictionConstant = 1f;

		// Token: 0x04007C6F RID: 31855
		public float slideControl = 0.00425f;

		// Token: 0x04007C70 RID: 31856
		public float stickDepth = 0.01f;

		// Token: 0x04007C71 RID: 31857
		private Vector3[] velocityHistory;

		// Token: 0x04007C72 RID: 31858
		private Vector3[] slideAverageHistory;

		// Token: 0x04007C73 RID: 31859
		private int velocityIndex;

		// Token: 0x04007C74 RID: 31860
		private Vector3 currentVelocity;

		// Token: 0x04007C75 RID: 31861
		private Vector3 averagedVelocity;

		// Token: 0x04007C76 RID: 31862
		private Vector3 lastPosition;

		// Token: 0x04007C77 RID: 31863
		public Vector3 bodyOffset;

		// Token: 0x04007C78 RID: 31864
		public LayerMask locomotionEnabledLayers;

		// Token: 0x04007C79 RID: 31865
		public LayerMask waterLayer;

		// Token: 0x04007C7A RID: 31866
		public bool wasHeadTouching;

		// Token: 0x04007C7B RID: 31867
		public int currentMaterialIndex;

		// Token: 0x04007C7C RID: 31868
		public Vector3 headSlideNormal;

		// Token: 0x04007C7D RID: 31869
		public float headSlipPercentage;

		// Token: 0x04007C7E RID: 31870
		[SerializeField]
		private Transform cosmeticsHeadTarget;

		// Token: 0x04007C7F RID: 31871
		[SerializeField]
		private float nativeScale = 1f;

		// Token: 0x04007C80 RID: 31872
		[SerializeField]
		private float scaleMultiplier = 1f;

		// Token: 0x04007C81 RID: 31873
		private NativeSizeChangerSettings activeSizeChangerSettings;

		// Token: 0x04007C82 RID: 31874
		public bool debugMovement;

		// Token: 0x04007C83 RID: 31875
		public bool disableMovement;

		// Token: 0x04007C84 RID: 31876
		[NonSerialized]
		public bool inOverlay;

		// Token: 0x04007C85 RID: 31877
		[NonSerialized]
		public bool isUserPresent;

		// Token: 0x04007C86 RID: 31878
		public GameObject turnParent;

		// Token: 0x04007C87 RID: 31879
		[SerializeField]
		public GameObject RecordingRig;

		// Token: 0x04007C88 RID: 31880
		public GorillaSurfaceOverride currentOverride;

		// Token: 0x04007C89 RID: 31881
		public MaterialDatasSO materialDatasSO;

		// Token: 0x04007C8A RID: 31882
		private float degreesTurnedThisFrame;

		// Token: 0x04007C8B RID: 31883
		private Vector3 bodyOffsetVector;

		// Token: 0x04007C8C RID: 31884
		private Vector3 movementToProjectedAboveCollisionPlane;

		// Token: 0x04007C8D RID: 31885
		private MeshCollider meshCollider;

		// Token: 0x04007C8E RID: 31886
		private Mesh collidedMesh;

		// Token: 0x04007C8F RID: 31887
		private GTPlayer.MaterialData foundMatData;

		// Token: 0x04007C90 RID: 31888
		private string findMatName;

		// Token: 0x04007C91 RID: 31889
		private int vertex1;

		// Token: 0x04007C92 RID: 31890
		private int vertex2;

		// Token: 0x04007C93 RID: 31891
		private int vertex3;

		// Token: 0x04007C94 RID: 31892
		private List<int> trianglesList = new List<int>(1000000);

		// Token: 0x04007C95 RID: 31893
		private Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(128);

		// Token: 0x04007C96 RID: 31894
		private int[] sharedMeshTris;

		// Token: 0x04007C97 RID: 31895
		private float lastRealTime;

		// Token: 0x04007C98 RID: 31896
		private float calcDeltaTime;

		// Token: 0x04007C99 RID: 31897
		private float tempRealTime;

		// Token: 0x04007C9A RID: 31898
		private Vector3 slideVelocity;

		// Token: 0x04007C9B RID: 31899
		private Vector3 slideAverageNormal;

		// Token: 0x04007C9C RID: 31900
		private RaycastHit tempHitInfo;

		// Token: 0x04007C9D RID: 31901
		private RaycastHit junkHit;

		// Token: 0x04007C9E RID: 31902
		private Vector3 firstPosition;

		// Token: 0x04007C9F RID: 31903
		private RaycastHit tempIterativeHit;

		// Token: 0x04007CA0 RID: 31904
		private float maxSphereSize1;

		// Token: 0x04007CA1 RID: 31905
		private float maxSphereSize2;

		// Token: 0x04007CA2 RID: 31906
		private Collider[] overlapColliders = new Collider[10];

		// Token: 0x04007CA3 RID: 31907
		private int overlapAttempts;

		// Token: 0x04007CA4 RID: 31908
		private float averageSlipPercentage;

		// Token: 0x04007CA5 RID: 31909
		private Vector3 surfaceDirection;

		// Token: 0x04007CA6 RID: 31910
		public float iceThreshold = 0.9f;

		// Token: 0x04007CA7 RID: 31911
		private float bodyMaxRadius;

		// Token: 0x04007CA8 RID: 31912
		public float bodyLerp = 0.17f;

		// Token: 0x04007CA9 RID: 31913
		private bool areBothTouching;

		// Token: 0x04007CAA RID: 31914
		private float slideFactor;

		// Token: 0x04007CAB RID: 31915
		[DebugOption]
		public bool didAJump;

		// Token: 0x04007CAC RID: 31916
		private bool updateRB;

		// Token: 0x04007CAD RID: 31917
		private Renderer slideRenderer;

		// Token: 0x04007CAE RID: 31918
		private RaycastHit[] rayCastNonAllocColliders;

		// Token: 0x04007CAF RID: 31919
		private Vector3[] crazyCheckVectors;

		// Token: 0x04007CB0 RID: 31920
		private RaycastHit emptyHit;

		// Token: 0x04007CB1 RID: 31921
		private int bufferCount;

		// Token: 0x04007CB2 RID: 31922
		private Vector3 lastOpenHeadPosition;

		// Token: 0x04007CB3 RID: 31923
		private List<Material> tempMaterialArray = new List<Material>(16);

		// Token: 0x04007CB4 RID: 31924
		private Vector3? antiDriftLastPosition;

		// Token: 0x04007CB5 RID: 31925
		private const float CameraFarClipDefault = 500f;

		// Token: 0x04007CB6 RID: 31926
		private const float CameraNearClipDefault = 0.01f;

		// Token: 0x04007CB7 RID: 31927
		private const float CameraNearClipTiny = 0.002f;

		// Token: 0x04007CB8 RID: 31928
		private Dictionary<GameObject, PhysicsMaterial> bodyTouchedSurfaces;

		// Token: 0x04007CB9 RID: 31929
		private bool primaryButtonPressed = true;

		// Token: 0x04007CBA RID: 31930
		[Header("Swimming")]
		public PlayerSwimmingParameters swimmingParams;

		// Token: 0x04007CBB RID: 31931
		public WaterParameters waterParams;

		// Token: 0x04007CBC RID: 31932
		public List<GTPlayer.LiquidProperties> liquidPropertiesList = new List<GTPlayer.LiquidProperties>(16);

		// Token: 0x04007CBD RID: 31933
		public bool debugDrawSwimming;

		// Token: 0x04007CBE RID: 31934
		[Header("Slam/Hit effects")]
		public GameObject wizardStaffSlamEffects;

		// Token: 0x04007CBF RID: 31935
		public GameObject geodeHitEffects;

		// Token: 0x04007CC0 RID: 31936
		[Header("Freeze Tag")]
		public float freezeTagHandSlidePercent = 0.88f;

		// Token: 0x04007CC1 RID: 31937
		public bool debugFreezeTag;

		// Token: 0x04007CC2 RID: 31938
		public float frozenBodyBuoyancyFactor = 1.5f;

		// Token: 0x04007CC4 RID: 31940
		[Space]
		private WaterVolume leftHandWaterVolume;

		// Token: 0x04007CC5 RID: 31941
		private WaterVolume rightHandWaterVolume;

		// Token: 0x04007CC6 RID: 31942
		private WaterVolume.SurfaceQuery leftHandWaterSurface;

		// Token: 0x04007CC7 RID: 31943
		private WaterVolume.SurfaceQuery rightHandWaterSurface;

		// Token: 0x04007CC8 RID: 31944
		private Vector3 swimmingVelocity = Vector3.zero;

		// Token: 0x04007CC9 RID: 31945
		private WaterVolume.SurfaceQuery waterSurfaceForHead;

		// Token: 0x04007CCA RID: 31946
		private bool bodyInWater;

		// Token: 0x04007CCB RID: 31947
		private bool headInWater;

		// Token: 0x04007CCC RID: 31948
		private bool audioSetToUnderwater;

		// Token: 0x04007CCD RID: 31949
		private float buoyancyExtension;

		// Token: 0x04007CD0 RID: 31952
		private float lastWaterSurfaceJumpTimeLeft = -1f;

		// Token: 0x04007CD1 RID: 31953
		private float lastWaterSurfaceJumpTimeRight = -1f;

		// Token: 0x04007CD2 RID: 31954
		private float waterSurfaceJumpCooldown = 0.1f;

		// Token: 0x04007CD3 RID: 31955
		private float leftHandNonDiveHapticsAmount;

		// Token: 0x04007CD4 RID: 31956
		private float rightHandNonDiveHapticsAmount;

		// Token: 0x04007CD5 RID: 31957
		private List<WaterVolume> headOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x04007CD6 RID: 31958
		private List<WaterVolume> bodyOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x04007CD7 RID: 31959
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x04007CDB RID: 31963
		private Quaternion playerRotationOverride = Quaternion.identity;

		// Token: 0x04007CDC RID: 31964
		private int playerRotationOverrideFrame = -1;

		// Token: 0x04007CDD RID: 31965
		private float playerRotationOverrideDecayRate = Mathf.Exp(1.5f);

		// Token: 0x04007CDF RID: 31967
		private ContactPoint[] bodyCollisionContacts = new ContactPoint[8];

		// Token: 0x04007CE0 RID: 31968
		private int bodyCollisionContactsCount;

		// Token: 0x04007CE1 RID: 31969
		private ContactPoint bodyGroundContact;

		// Token: 0x04007CE2 RID: 31970
		private float bodyGroundContactTime;

		// Token: 0x04007CE4 RID: 31972
		private const float movingSurfaceVelocityLimit = 40f;

		// Token: 0x04007CE5 RID: 31973
		private bool exitMovingSurface;

		// Token: 0x04007CE6 RID: 31974
		private float exitMovingSurfaceThreshold = 6f;

		// Token: 0x04007CE7 RID: 31975
		private bool isClimbableMoving;

		// Token: 0x04007CE8 RID: 31976
		private Quaternion lastClimbableRotation;

		// Token: 0x04007CE9 RID: 31977
		private int lastAttachedToMovingSurfaceFrame;

		// Token: 0x04007CEA RID: 31978
		private const int MIN_FRAMES_OFF_SURFACE_TO_DETACH = 3;

		// Token: 0x04007CEB RID: 31979
		private bool isHandHoldMoving;

		// Token: 0x04007CEC RID: 31980
		private Quaternion lastHandHoldRotation;

		// Token: 0x04007CED RID: 31981
		private Vector3 movingHandHoldReleaseVelocity;

		// Token: 0x04007CEE RID: 31982
		private GTPlayer.MovingSurfaceContactPoint lastMovingSurfaceContact;

		// Token: 0x04007CEF RID: 31983
		private int lastMovingSurfaceID = -1;

		// Token: 0x04007CF0 RID: 31984
		private BuilderPiece lastMonkeBlock;

		// Token: 0x04007CF1 RID: 31985
		private Quaternion lastMovingSurfaceRot;

		// Token: 0x04007CF2 RID: 31986
		private RaycastHit lastMovingSurfaceHit;

		// Token: 0x04007CF3 RID: 31987
		private Vector3 lastMovingSurfaceTouchLocal;

		// Token: 0x04007CF4 RID: 31988
		private Vector3 lastMovingSurfaceTouchWorld;

		// Token: 0x04007CF5 RID: 31989
		private Vector3 movingSurfaceOffset;

		// Token: 0x04007CF6 RID: 31990
		private bool wasMovingSurfaceMonkeBlock;

		// Token: 0x04007CF7 RID: 31991
		private Vector3 lastMovingSurfaceVelocity;

		// Token: 0x04007CF8 RID: 31992
		private bool wasBodyOnGround;

		// Token: 0x04007CF9 RID: 31993
		private BasePlatform currentPlatform;

		// Token: 0x04007CFA RID: 31994
		private BasePlatform lastPlatformTouched;

		// Token: 0x04007CFB RID: 31995
		private Vector3 lastFrameTouchPosLocal;

		// Token: 0x04007CFC RID: 31996
		private Vector3 lastFrameTouchPosWorld;

		// Token: 0x04007CFD RID: 31997
		private bool lastFrameHasValidTouchPos;

		// Token: 0x04007CFE RID: 31998
		private Vector3 refMovement = Vector3.zero;

		// Token: 0x04007CFF RID: 31999
		private Vector3 platformTouchOffset;

		// Token: 0x04007D00 RID: 32000
		private Vector3 debugLastRightHandPosition;

		// Token: 0x04007D01 RID: 32001
		private Vector3 debugPlatformDeltaPosition;

		// Token: 0x04007D02 RID: 32002
		public double tempFreezeRightHandEnableTime;

		// Token: 0x04007D03 RID: 32003
		public double tempFreezeLeftHandEnableTime;

		// Token: 0x04007D04 RID: 32004
		private const float climbingMaxThrowSpeed = 5.5f;

		// Token: 0x04007D05 RID: 32005
		private const float climbHelperSmoothSnapSpeed = 12f;

		// Token: 0x04007D06 RID: 32006
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x04007D07 RID: 32007
		private GorillaClimbable currentClimbable;

		// Token: 0x04007D08 RID: 32008
		private GorillaHandClimber currentClimber;

		// Token: 0x04007D09 RID: 32009
		private Vector3 climbHelperTargetPos = Vector3.zero;

		// Token: 0x04007D0A RID: 32010
		private Transform climbHelper;

		// Token: 0x04007D0B RID: 32011
		private GorillaRopeSwing currentSwing;

		// Token: 0x04007D0C RID: 32012
		private GorillaZipline currentZipline;

		// Token: 0x04007D0D RID: 32013
		[SerializeField]
		private ConnectedControllerHandler controllerState;

		// Token: 0x04007D0E RID: 32014
		public int sizeLayerMask;

		// Token: 0x04007D0F RID: 32015
		public bool InReportMenu;

		// Token: 0x04007D10 RID: 32016
		private LayerChanger layerChanger;

		// Token: 0x04007D13 RID: 32019
		private bool hasCorrectedForTracking;

		// Token: 0x04007D14 RID: 32020
		private float halloweenLevitationStrength;

		// Token: 0x04007D15 RID: 32021
		private float halloweenLevitationFullStrengthDuration;

		// Token: 0x04007D16 RID: 32022
		private float halloweenLevitationTotalDuration = 1f;

		// Token: 0x04007D17 RID: 32023
		private float halloweenLevitationBonusStrength;

		// Token: 0x04007D18 RID: 32024
		private float halloweenLevitateBonusOffAtYSpeed;

		// Token: 0x04007D19 RID: 32025
		private float halloweenLevitateBonusFullAtYSpeed = 1f;

		// Token: 0x04007D1A RID: 32026
		private float lastTouchedGroundTimestamp;

		// Token: 0x04007D1B RID: 32027
		private bool teleportToTrain;

		// Token: 0x04007D1C RID: 32028
		public bool isAttachedToTrain;

		// Token: 0x04007D1D RID: 32029
		private bool stuckLeft;

		// Token: 0x04007D1E RID: 32030
		private bool stuckRight;

		// Token: 0x04007D1F RID: 32031
		private float lastScale;

		// Token: 0x04007D20 RID: 32032
		private Vector3 currentSlopDirection;

		// Token: 0x04007D21 RID: 32033
		private Vector3 lastSlopeDirection = Vector3.zero;

		// Token: 0x04007D22 RID: 32034
		private readonly Dictionary<Object, Action<GTPlayer>> gravityOverrides = new Dictionary<Object, Action<GTPlayer>>();

		// Token: 0x04007D25 RID: 32037
		private int hoverAllowedCount;

		// Token: 0x04007D26 RID: 32038
		[Header("Hoverboard")]
		[SerializeField]
		private float hoverIdealHeight = 0.5f;

		// Token: 0x04007D27 RID: 32039
		[SerializeField]
		private float hoverCarveSidewaysSpeedLossFactor = 1f;

		// Token: 0x04007D28 RID: 32040
		[SerializeField]
		private AnimationCurve hoverCarveAngleResponsiveness;

		// Token: 0x04007D29 RID: 32041
		[SerializeField]
		private HoverboardVisual hoverboardVisual;

		// Token: 0x04007D2A RID: 32042
		[SerializeField]
		private float sidewaysDrag = 0.1f;

		// Token: 0x04007D2B RID: 32043
		[SerializeField]
		private float hoveringSlowSpeed = 0.1f;

		// Token: 0x04007D2C RID: 32044
		[SerializeField]
		private float hoveringSlowStoppingFactor = 0.95f;

		// Token: 0x04007D2D RID: 32045
		[SerializeField]
		private float hoverboardPaddleBoostMultiplier = 0.1f;

		// Token: 0x04007D2E RID: 32046
		[SerializeField]
		private float hoverboardPaddleBoostMax = 10f;

		// Token: 0x04007D2F RID: 32047
		[SerializeField]
		private float hoverboardBoostGracePeriod = 1f;

		// Token: 0x04007D30 RID: 32048
		[SerializeField]
		private float hoverBodyHasCollisionsOutsideRadius = 0.5f;

		// Token: 0x04007D31 RID: 32049
		[SerializeField]
		private float hoverBodyCollisionRadiusUpOffset = 0.2f;

		// Token: 0x04007D32 RID: 32050
		[SerializeField]
		private float hoverGeneralUpwardForce = 8f;

		// Token: 0x04007D33 RID: 32051
		[SerializeField]
		private float hoverTiltAdjustsForwardFactor = 0.2f;

		// Token: 0x04007D34 RID: 32052
		[SerializeField]
		private float hoverMinGrindSpeed = 1f;

		// Token: 0x04007D35 RID: 32053
		[SerializeField]
		private float hoverSlamJumpStrengthFactor = 25f;

		// Token: 0x04007D36 RID: 32054
		[SerializeField]
		private float hoverMaxPaddleSpeed = 35f;

		// Token: 0x04007D37 RID: 32055
		[SerializeField]
		private HoverboardAudio hoverboardAudio;

		// Token: 0x04007D38 RID: 32056
		private bool hasHoverPoint;

		// Token: 0x04007D39 RID: 32057
		private float boostEnabledUntilTimestamp;

		// Token: 0x04007D3A RID: 32058
		private GTPlayer.HoverBoardCast[] hoverboardCasts = new GTPlayer.HoverBoardCast[]
		{
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 1f, 0.36f),
				localDirection = Vector3.down,
				distance = 1f,
				sphereRadius = 0.2f,
				intersectToVelocityCap = 0.1f
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, 0.36f),
				localDirection = Vector3.forward,
				distance = 0.25f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, -0.1f),
				localDirection = -Vector3.forward,
				distance = 0.24f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			}
		};

		// Token: 0x04007D3B RID: 32059
		private Vector3 hoverboardPlayerLocalPos;

		// Token: 0x04007D3C RID: 32060
		private Quaternion hoverboardPlayerLocalRot;

		// Token: 0x04007D3D RID: 32061
		private bool didHoverLastFrame;

		// Token: 0x04007D3E RID: 32062
		private bool hasLeftHandTentacleMove;

		// Token: 0x04007D3F RID: 32063
		private bool hasRightHandTentacleMove;

		// Token: 0x04007D40 RID: 32064
		private Vector3 leftHandTentacleMove;

		// Token: 0x04007D41 RID: 32065
		private Vector3 rightHandTentacleMove;

		// Token: 0x04007D42 RID: 32066
		private GTPlayer.HandHoldState activeHandHold;

		// Token: 0x04007D43 RID: 32067
		private GTPlayer.HandHoldState secondaryHandHold;

		// Token: 0x04007D44 RID: 32068
		public PhysicsMaterial slipperyMaterial;

		// Token: 0x04007D45 RID: 32069
		private bool wasHoldingHandhold;

		// Token: 0x04007D46 RID: 32070
		private Vector3 secondLastPreHandholdVelocity;

		// Token: 0x04007D47 RID: 32071
		private Vector3 lastPreHandholdVelocity;

		// Token: 0x04007D48 RID: 32072
		[Header("Native Scale Adjustment")]
		[SerializeField]
		private AnimationCurve nativeScaleMagnitudeAdjustmentFactor;

		// Token: 0x020010DE RID: 4318
		[Serializable]
		public struct HandState
		{
			// Token: 0x06006CC8 RID: 27848 RVA: 0x00237088 File Offset: 0x00235288
			public void Init(GTPlayer gtPlayer, bool isLeftHand, float maxArmLength)
			{
				this.gtPlayer = gtPlayer;
				this.isLeftHand = isLeftHand;
				this.maxArmLength = maxArmLength;
				this.lastPosition = this.controllerTransform.position;
				this.lastRotation = this.controllerTransform.rotation;
				if (this.handFollower != null)
				{
					this.handFollower.transform.position = this.lastPosition;
					this.handFollower.transform.rotation = this.lastRotation;
				}
				this.wasColliding = false;
				this.slipSetToMaxFrameIdx = -1;
			}

			// Token: 0x06006CC9 RID: 27849 RVA: 0x00237114 File Offset: 0x00235314
			public void OnTeleport()
			{
				this.wasColliding = false;
				this.isColliding = false;
				this.isSliding = false;
				this.wasSliding = false;
				if (this.handFollower != null)
				{
					this.handFollower.position = this.controllerTransform.position;
					this.handFollower.rotation = this.controllerTransform.rotation;
				}
				this.lastPosition = this.controllerTransform.position;
				this.lastRotation = this.controllerTransform.rotation;
			}

			// Token: 0x06006CCA RID: 27850 RVA: 0x00237199 File Offset: 0x00235399
			public Vector3 GetLastPosition()
			{
				return this.lastPosition + this.gtPlayer.MovingSurfaceMovement();
			}

			// Token: 0x06006CCB RID: 27851 RVA: 0x002371B1 File Offset: 0x002353B1
			public bool SlipOverriddenToMax()
			{
				return this.slipSetToMaxFrameIdx == Time.frameCount;
			}

			// Token: 0x06006CCC RID: 27852 RVA: 0x002371C0 File Offset: 0x002353C0
			public void FirstIteration(ref Vector3 totalMove, ref int divisor, float paddleBoostFactor)
			{
				if (this.hasCustomBoost)
				{
					this.boostVectorThisFrame = this.gtPlayer.turnParent.transform.rotation * -this.velocityTracker.GetAverageVelocity(false, 0.15f, false) * this.customBoostFactor;
				}
				else
				{
					this.boostVectorThisFrame = (this.gtPlayer.enableHoverMode ? (this.gtPlayer.turnParent.transform.rotation * -this.velocityTracker.GetAverageVelocity(false, 0.15f, false) * paddleBoostFactor) : Vector3.zero);
				}
				Vector3 vector = this.GetCurrentHandPosition() + this.gtPlayer.movingSurfaceOffset;
				Vector3 vector2 = this.GetLastPosition();
				Vector3 a = vector - vector2;
				bool flag = this.gtPlayer.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT;
				if (!this.gtPlayer.didAJump && this.wasSliding && Vector3.Dot(this.gtPlayer.slideAverageNormal, GTPlayerTransform.PhysicsUp) > 0f)
				{
					a += Vector3.Project(-this.gtPlayer.slideAverageNormal * this.gtPlayer.stickDepth * this.gtPlayer.scale, GTPlayerTransform.PhysicsDown);
				}
				float num = this.gtPlayer.minimumRaycastDistance * this.gtPlayer.scale;
				if (this.gtPlayer.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
				{
					num = (this.gtPlayer.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.gtPlayer.scale;
				}
				Vector3 vector3 = Vector3.zero;
				if (flag && !this.gtPlayer.exitMovingSurface)
				{
					vector3 = Vector3.Project(-this.gtPlayer.lastMovingSurfaceHit.normal * (this.gtPlayer.stickDepth * this.gtPlayer.scale), GTPlayerTransform.PhysicsDown);
					if (this.gtPlayer.scale < 0.5f)
					{
						Vector3 normalized = this.gtPlayer.MovingSurfaceMovement().normalized;
						if (normalized != Vector3.zero)
						{
							float num2 = Vector3.Dot(GTPlayerTransform.PhysicsUp, normalized);
							if ((double)num2 > 0.9 || (double)num2 < -0.9)
							{
								vector3 *= 6f;
								num *= 1.1f;
							}
						}
					}
				}
				Vector3 a2;
				RaycastHit lastHitInfoHand;
				Vector3 b;
				if (this.gtPlayer.IterativeCollisionSphereCast(vector2, num, a + vector3, this.boostVectorThisFrame, out a2, true, out this.slipPercentage, out lastHitInfoHand, this.SlipOverriddenToMax()) && !this.isHolding && !this.gtPlayer.InReportMenu)
				{
					if (this.wasColliding && this.slipPercentage <= this.gtPlayer.defaultSlideFactor && !this.boostVectorThisFrame.IsLongerThan(0f))
					{
						b = vector2 - vector;
					}
					else
					{
						b = a2 - vector;
					}
					this.isSliding = (this.slipPercentage > this.gtPlayer.iceThreshold);
					this.slideNormal = this.gtPlayer.tempHitInfo.normal;
					this.isColliding = true;
					this.materialTouchIndex = this.gtPlayer.currentMaterialIndex;
					this.surfaceOverride = this.gtPlayer.currentOverride;
					this.gtPlayer.lastHitInfoHand = lastHitInfoHand;
					this.lastHitInfo = lastHitInfoHand;
				}
				else
				{
					b = Vector3.zero;
					this.slipPercentage = 0f;
					this.isSliding = false;
					this.slideNormal = GTPlayerTransform.PhysicsUp;
					this.isColliding = false;
					this.materialTouchIndex = 0;
					this.surfaceOverride = null;
				}
				bool flag2 = this.isLeftHand ? this.gtPlayer.controllerState.LeftValid : this.gtPlayer.controllerState.RightValid;
				this.isColliding = (this.isColliding && flag2);
				this.isSliding = (this.isSliding && flag2);
				if (this.isColliding)
				{
					this.gtPlayer.anyHandIsColliding = true;
					if (this.isSliding)
					{
						this.gtPlayer.anyHandIsSliding = true;
					}
					else
					{
						this.gtPlayer.anyHandIsSticking = true;
					}
				}
				if (this.isColliding || this.wasColliding)
				{
					if (!this.surfaceOverride || !this.surfaceOverride.disablePushBackEffect)
					{
						totalMove += b;
					}
					divisor++;
				}
			}

			// Token: 0x06006CCD RID: 27853 RVA: 0x00237644 File Offset: 0x00235844
			public void FinalizeHandPosition()
			{
				Vector3 vector = this.GetLastPosition();
				if (Time.time < this.tempFreezeUntilTimestamp)
				{
					this.finalPositionThisFrame = vector;
				}
				else
				{
					Vector3 movementVector = this.GetCurrentHandPosition() - vector;
					float sphereRadius = this.gtPlayer.minimumRaycastDistance * this.gtPlayer.scale;
					if (this.gtPlayer.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
					{
						sphereRadius = (this.gtPlayer.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.gtPlayer.scale;
					}
					Vector3 vector2;
					float num;
					RaycastHit lastHitInfoHand;
					if (this.gtPlayer.IterativeCollisionSphereCast(vector, sphereRadius, movementVector, this.boostVectorThisFrame, out vector2, this.gtPlayer.areBothTouching, out num, out lastHitInfoHand, false) && !this.isHolding)
					{
						this.isColliding = true;
						this.isSliding = (num > this.gtPlayer.iceThreshold);
						this.materialTouchIndex = this.gtPlayer.currentMaterialIndex;
						this.surfaceOverride = this.gtPlayer.currentOverride;
						this.gtPlayer.lastHitInfoHand = lastHitInfoHand;
						this.lastHitInfo = lastHitInfoHand;
						this.finalPositionThisFrame = vector2;
					}
					else
					{
						this.finalPositionThisFrame = this.GetCurrentHandPosition();
					}
				}
				bool flag = this.isLeftHand ? this.gtPlayer.controllerState.LeftValid : this.gtPlayer.controllerState.RightValid;
				this.isColliding = (this.isColliding && flag);
				this.isSliding = (this.isSliding && flag);
				if (this.isColliding)
				{
					this.gtPlayer.anyHandIsColliding = true;
					if (this.isSliding)
					{
						this.gtPlayer.anyHandIsSliding = true;
						return;
					}
					this.gtPlayer.anyHandIsSticking = true;
				}
			}

			// Token: 0x06006CCE RID: 27854 RVA: 0x002371B1 File Offset: 0x002353B1
			public bool IsSlipOverriddenToMax()
			{
				return this.slipSetToMaxFrameIdx == Time.frameCount;
			}

			// Token: 0x06006CCF RID: 27855 RVA: 0x002377FC File Offset: 0x002359FC
			public Vector3 GetCurrentHandPosition()
			{
				Vector3 position = this.gtPlayer.headCollider.transform.position;
				if (this.gtPlayer.inOverlay)
				{
					return position + this.gtPlayer.headCollider.transform.up * -0.5f * this.gtPlayer.scale;
				}
				Vector3 vector = this.gtPlayer.PositionWithOffset(this.controllerTransform, this.handOffset);
				if ((vector - position).IsShorterThan(this.maxArmLength * this.gtPlayer.scale))
				{
					return vector;
				}
				return position + (vector - position).normalized * this.maxArmLength * this.gtPlayer.scale;
			}

			// Token: 0x06006CD0 RID: 27856 RVA: 0x002378CC File Offset: 0x00235ACC
			public void PositionHandFollower()
			{
				this.handFollower.position = this.finalPositionThisFrame;
				this.handFollower.rotation = this.lastRotation;
			}

			// Token: 0x06006CD1 RID: 27857 RVA: 0x002378F0 File Offset: 0x00235AF0
			public void OnEndOfFrame()
			{
				this.wasColliding = this.isColliding;
				this.wasSliding = this.isSliding;
				this.lastPosition = this.finalPositionThisFrame;
				if (Time.time > this.tempFreezeUntilTimestamp)
				{
					this.lastRotation = this.controllerTransform.rotation * this.handRotOffset;
				}
			}

			// Token: 0x06006CD2 RID: 27858 RVA: 0x0023794A File Offset: 0x00235B4A
			public void TempFreezeHand(float freezeDuration)
			{
				this.tempFreezeUntilTimestamp = Math.Max(this.tempFreezeUntilTimestamp, Time.time + freezeDuration);
			}

			// Token: 0x06006CD3 RID: 27859 RVA: 0x00237964 File Offset: 0x00235B64
			public void GetHandTapData(out bool wasHandTouching, out bool wasSliding, out int handMatIndex, out GorillaSurfaceOverride surfaceOverride, out RaycastHit handHitInfo, out Vector3 handPosition, out GorillaVelocityTracker handVelocityTracker)
			{
				wasHandTouching = this.wasColliding;
				wasSliding = this.wasSliding;
				handMatIndex = this.materialTouchIndex;
				surfaceOverride = this.surfaceOverride;
				handHitInfo = this.lastHitInfo;
				handPosition = this.finalPositionThisFrame;
				handVelocityTracker = this.velocityTracker;
			}

			// Token: 0x04007D49 RID: 32073
			[NonSerialized]
			public Vector3 lastPosition;

			// Token: 0x04007D4A RID: 32074
			[NonSerialized]
			public Quaternion lastRotation;

			// Token: 0x04007D4B RID: 32075
			[NonSerialized]
			public bool isLeftHand;

			// Token: 0x04007D4C RID: 32076
			[NonSerialized]
			public bool wasColliding;

			// Token: 0x04007D4D RID: 32077
			[NonSerialized]
			public bool isColliding;

			// Token: 0x04007D4E RID: 32078
			[NonSerialized]
			public bool wasSliding;

			// Token: 0x04007D4F RID: 32079
			[NonSerialized]
			public bool isSliding;

			// Token: 0x04007D50 RID: 32080
			[NonSerialized]
			public bool isHolding;

			// Token: 0x04007D51 RID: 32081
			[NonSerialized]
			public Vector3 slideNormal;

			// Token: 0x04007D52 RID: 32082
			[NonSerialized]
			public float slipPercentage;

			// Token: 0x04007D53 RID: 32083
			[NonSerialized]
			public Vector3 hitPoint;

			// Token: 0x04007D54 RID: 32084
			[NonSerialized]
			private Vector3 boostVectorThisFrame;

			// Token: 0x04007D55 RID: 32085
			[NonSerialized]
			public Vector3 finalPositionThisFrame;

			// Token: 0x04007D56 RID: 32086
			[NonSerialized]
			public int slipSetToMaxFrameIdx;

			// Token: 0x04007D57 RID: 32087
			[NonSerialized]
			public int materialTouchIndex;

			// Token: 0x04007D58 RID: 32088
			[NonSerialized]
			public GorillaSurfaceOverride surfaceOverride;

			// Token: 0x04007D59 RID: 32089
			[NonSerialized]
			public RaycastHit hitInfo;

			// Token: 0x04007D5A RID: 32090
			[NonSerialized]
			public RaycastHit lastHitInfo;

			// Token: 0x04007D5B RID: 32091
			[NonSerialized]
			private GTPlayer gtPlayer;

			// Token: 0x04007D5C RID: 32092
			[SerializeField]
			public Transform handFollower;

			// Token: 0x04007D5D RID: 32093
			[SerializeField]
			public Transform controllerTransform;

			// Token: 0x04007D5E RID: 32094
			[SerializeField]
			public GorillaVelocityTracker velocityTracker;

			// Token: 0x04007D5F RID: 32095
			[SerializeField]
			public GorillaVelocityTracker interactPointVelocityTracker;

			// Token: 0x04007D60 RID: 32096
			[SerializeField]
			public Vector3 handOffset;

			// Token: 0x04007D61 RID: 32097
			[SerializeField]
			public Quaternion handRotOffset;

			// Token: 0x04007D62 RID: 32098
			[NonSerialized]
			public float tempFreezeUntilTimestamp;

			// Token: 0x04007D63 RID: 32099
			[NonSerialized]
			public bool canTag;

			// Token: 0x04007D64 RID: 32100
			[NonSerialized]
			public bool canStun;

			// Token: 0x04007D65 RID: 32101
			private float maxArmLength;

			// Token: 0x04007D66 RID: 32102
			[NonSerialized]
			public bool isActive;

			// Token: 0x04007D67 RID: 32103
			[NonSerialized]
			public float customBoostFactor;

			// Token: 0x04007D68 RID: 32104
			[NonSerialized]
			public bool hasCustomBoost;
		}

		// Token: 0x020010DF RID: 4319
		private enum MovingSurfaceContactPoint
		{
			// Token: 0x04007D6A RID: 32106
			NONE,
			// Token: 0x04007D6B RID: 32107
			RIGHT,
			// Token: 0x04007D6C RID: 32108
			LEFT,
			// Token: 0x04007D6D RID: 32109
			BODY
		}

		// Token: 0x020010E0 RID: 4320
		[Serializable]
		public struct MaterialData
		{
			// Token: 0x04007D6E RID: 32110
			public string matName;

			// Token: 0x04007D6F RID: 32111
			public bool overrideAudio;

			// Token: 0x04007D70 RID: 32112
			public AudioClip audio;

			// Token: 0x04007D71 RID: 32113
			public bool overrideSlidePercent;

			// Token: 0x04007D72 RID: 32114
			public float slidePercent;

			// Token: 0x04007D73 RID: 32115
			public int surfaceEffectIndex;
		}

		// Token: 0x020010E1 RID: 4321
		[Serializable]
		public struct LiquidProperties
		{
			// Token: 0x04007D74 RID: 32116
			[Range(0f, 2f)]
			[Tooltip("0: no resistance just like air, 1: full resistance like solid geometry")]
			public float resistance;

			// Token: 0x04007D75 RID: 32117
			[Range(0f, 3f)]
			[Tooltip("0: no buoyancy. 1: Fully compensates gravity. 2: net force is upwards equal to gravity")]
			public float buoyancy;

			// Token: 0x04007D76 RID: 32118
			[Range(0f, 3f)]
			[Tooltip("Damping Half-life Multiplier")]
			public float dampingFactor;

			// Token: 0x04007D77 RID: 32119
			[Range(0f, 1f)]
			public float surfaceJumpFactor;
		}

		// Token: 0x020010E2 RID: 4322
		public enum LiquidType
		{
			// Token: 0x04007D79 RID: 32121
			Water,
			// Token: 0x04007D7A RID: 32122
			Lava,
			// Token: 0x04007D7B RID: 32123
			SwimInAir
		}

		// Token: 0x020010E3 RID: 4323
		private struct HoverBoardCast
		{
			// Token: 0x04007D7C RID: 32124
			public Vector3 localOrigin;

			// Token: 0x04007D7D RID: 32125
			public Vector3 localDirection;

			// Token: 0x04007D7E RID: 32126
			public float sphereRadius;

			// Token: 0x04007D7F RID: 32127
			public float distance;

			// Token: 0x04007D80 RID: 32128
			public float intersectToVelocityCap;

			// Token: 0x04007D81 RID: 32129
			public bool isSolid;

			// Token: 0x04007D82 RID: 32130
			public bool didHit;

			// Token: 0x04007D83 RID: 32131
			public Vector3 pointHit;

			// Token: 0x04007D84 RID: 32132
			public Vector3 normalHit;
		}

		// Token: 0x020010E4 RID: 4324
		private struct HandHoldState
		{
			// Token: 0x04007D85 RID: 32133
			public GorillaGrabber grabber;

			// Token: 0x04007D86 RID: 32134
			public Transform objectHeld;

			// Token: 0x04007D87 RID: 32135
			public Vector3 localPositionHeld;

			// Token: 0x04007D88 RID: 32136
			public float localRotationalOffset;

			// Token: 0x04007D89 RID: 32137
			public bool applyRotation;
		}
	}
}
