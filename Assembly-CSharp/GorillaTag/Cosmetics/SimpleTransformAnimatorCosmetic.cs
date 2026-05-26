using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012BA RID: 4794
	public class SimpleTransformAnimatorCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x060077E8 RID: 30696 RVA: 0x0027557C File Offset: 0x0027377C
		private void DebugToggle()
		{
			this.Toggle();
		}

		// Token: 0x060077E9 RID: 30697 RVA: 0x00275584 File Offset: 0x00273784
		private void DebugA()
		{
			this.TogglePoseA();
		}

		// Token: 0x060077EA RID: 30698 RVA: 0x0027558C File Offset: 0x0027378C
		private void DebugB()
		{
			this.TogglePoseB();
		}

		// Token: 0x17000B8C RID: 2956
		// (get) Token: 0x060077EB RID: 30699 RVA: 0x00275594 File Offset: 0x00273794
		// (set) Token: 0x060077EC RID: 30700 RVA: 0x0027559C File Offset: 0x0027379C
		public bool TickRunning { get; set; }

		// Token: 0x060077ED RID: 30701 RVA: 0x002755A5 File Offset: 0x002737A5
		private void OnEnable()
		{
			this.posBlendCurrent = this.posBlendTarget;
			this.UpdateTransform();
		}

		// Token: 0x060077EE RID: 30702 RVA: 0x002755B9 File Offset: 0x002737B9
		private void OnDisable()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
				this.TickRunning = false;
			}
		}

		// Token: 0x060077EF RID: 30703 RVA: 0x002755D0 File Offset: 0x002737D0
		private void CheckAnimationNeeded()
		{
			bool flag = false;
			bool flag2 = Mathf.Approximately(this.posBlendCurrent, this.posBlendTarget);
			switch (this.animMode)
			{
			case SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos:
				flag = !flag2;
				break;
			case SimpleTransformAnimatorCosmetic.animModes.animateOneshot:
				flag = (this.loopAnim || !flag2);
				break;
			}
			if (flag && !this.TickRunning)
			{
				TickSystem<object>.AddCallbackTarget(this);
				this.TickRunning = true;
				this.isAnimating = true;
				return;
			}
			if (!flag && this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
				this.TickRunning = false;
				this.isAnimating = false;
			}
		}

		// Token: 0x060077F0 RID: 30704 RVA: 0x00275664 File Offset: 0x00273864
		public void Tick()
		{
			float num = 1f / this.animationDuration;
			this.posBlendCurrent = Mathf.MoveTowards(this.posBlendCurrent, this.posBlendTarget, Time.deltaTime * num);
			switch (this.animMode)
			{
			default:
				this.UpdateTransform();
				this.CheckAnimationNeeded();
				return;
			}
		}

		// Token: 0x060077F1 RID: 30705 RVA: 0x002756C4 File Offset: 0x002738C4
		private void UpdateTransform()
		{
			Vector3 position = this.targetTransform.position;
			Quaternion rotation = this.targetTransform.rotation;
			float t = this.InterpolationCurve.Evaluate(this.posBlendCurrent);
			if (this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.Position || this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation)
			{
				position = Vector3.Lerp(this.poseA.position, this.poseB.position, t);
			}
			if (this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.Rotation || this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation)
			{
				rotation = Quaternion.Slerp(this.poseA.rotation, this.poseB.rotation, t);
			}
			this.targetTransform.SetPositionAndRotation(position, rotation);
		}

		// Token: 0x060077F2 RID: 30706 RVA: 0x00275765 File Offset: 0x00273965
		public void Toggle()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
			this.posBlendTarget = ((this.posBlendTarget < 0.5f) ? 1f : 0f);
			this.CheckAnimationNeeded();
		}

		// Token: 0x060077F3 RID: 30707 RVA: 0x00275793 File Offset: 0x00273993
		public void TogglePoseA()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
			this.posBlendTarget = 0f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x060077F4 RID: 30708 RVA: 0x002757AD File Offset: 0x002739AD
		public void TogglePoseB()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
			this.posBlendTarget = 1f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x060077F5 RID: 30709 RVA: 0x002757C7 File Offset: 0x002739C7
		public void playAnimationOneshot()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.animateOneshot;
			this.posBlendCurrent = 0f;
			this.posBlendTarget = 1f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x060077F6 RID: 30710 RVA: 0x002757EC File Offset: 0x002739EC
		private void DebugPlayAnimationOneShot()
		{
			this.playAnimationOneshot();
		}

		// Token: 0x04008AC8 RID: 35528
		private SimpleTransformAnimatorCosmetic.animModes animMode;

		// Token: 0x04008AC9 RID: 35529
		[Tooltip("Shapes how the transform will interpolate over the course of the animation.")]
		public AnimationCurve InterpolationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008ACA RID: 35530
		[SerializeField]
		[Tooltip("The object that will animate (blend) between the poses.")]
		private Transform targetTransform;

		// Token: 0x04008ACB RID: 35531
		[SerializeField]
		[Tooltip("Start pose (blend value 0).")]
		private Transform poseA;

		// Token: 0x04008ACC RID: 35532
		[SerializeField]
		[Tooltip("End pose (blend value 1).")]
		private Transform poseB;

		// Token: 0x04008ACD RID: 35533
		[FormerlySerializedAs("transitionTime")]
		[SerializeField]
		[Tooltip("Total time (in seconds) to animate fully between poses.")]
		private float animationDuration = 1f;

		// Token: 0x04008ACE RID: 35534
		[SerializeField]
		[Tooltip("Controls what aspect of the transform is affected by the blend.")]
		private SimpleTransformAnimatorCosmetic.animatedPropertyChoices animatedProperties = SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation;

		// Token: 0x04008ACF RID: 35535
		private bool loopAnim;

		// Token: 0x04008AD0 RID: 35536
		private float posBlendCurrent;

		// Token: 0x04008AD1 RID: 35537
		private float posBlendTarget;

		// Token: 0x04008AD2 RID: 35538
		private bool isAnimating;

		// Token: 0x020012BB RID: 4795
		public enum animatedPropertyChoices
		{
			// Token: 0x04008AD5 RID: 35541
			Position,
			// Token: 0x04008AD6 RID: 35542
			Rotation,
			// Token: 0x04008AD7 RID: 35543
			PositionAndRotation
		}

		// Token: 0x020012BC RID: 4796
		public enum animModes
		{
			// Token: 0x04008AD9 RID: 35545
			stepToTargetPos,
			// Token: 0x04008ADA RID: 35546
			animateBounce,
			// Token: 0x04008ADB RID: 35547
			animateOneshot
		}
	}
}
