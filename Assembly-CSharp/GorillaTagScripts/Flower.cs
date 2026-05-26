using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F00 RID: 3840
	public class Flower : MonoBehaviour
	{
		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x06005F77 RID: 24439 RVA: 0x001EB9D7 File Offset: 0x001E9BD7
		// (set) Token: 0x06005F78 RID: 24440 RVA: 0x001EB9DF File Offset: 0x001E9BDF
		public bool IsWatered { get; private set; }

		// Token: 0x06005F79 RID: 24441 RVA: 0x001EB9E8 File Offset: 0x001E9BE8
		private void Awake()
		{
			this.shouldUpdateVisuals = true;
			this.anim = base.GetComponent<Animator>();
			this.timer = base.GetComponent<GorillaTimer>();
			this.perchPoint = base.GetComponent<BeePerchPoint>();
			this.timer.onTimerStopped.AddListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
			this.currentState = Flower.FlowerState.None;
			this.wateredFx = this.wateredFx.GetComponent<ParticleSystem>();
			this.IsWatered = false;
			this.meshRenderer = base.GetComponent<SkinnedMeshRenderer>();
			this.meshRenderer.enabled = false;
			this.anim.enabled = false;
		}

		// Token: 0x06005F7A RID: 24442 RVA: 0x001EBA7F File Offset: 0x001E9C7F
		private void OnDestroy()
		{
			this.timer.onTimerStopped.RemoveListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
		}

		// Token: 0x06005F7B RID: 24443 RVA: 0x001EBAA0 File Offset: 0x001E9CA0
		public void WaterFlower(bool isWatered = false)
		{
			this.IsWatered = isWatered;
			switch (this.currentState)
			{
			case Flower.FlowerState.None:
				this.UpdateFlowerState(Flower.FlowerState.Healthy, false, true);
				return;
			case Flower.FlowerState.Healthy:
				if (!isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, false, true);
					return;
				}
				break;
			case Flower.FlowerState.Middle:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Healthy, true, true);
					return;
				}
				this.UpdateFlowerState(Flower.FlowerState.Wilted, false, true);
				return;
			case Flower.FlowerState.Wilted:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, true, true);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06005F7C RID: 24444 RVA: 0x001EBB10 File Offset: 0x001E9D10
		public void UpdateFlowerState(Flower.FlowerState newState, bool isWatered = false, bool updateVisual = true)
		{
			if (FlowersManager.Instance.IsMine)
			{
				this.timer.RestartTimer();
			}
			this.ChangeState(newState);
			if (this.perchPoint)
			{
				this.perchPoint.enabled = (this.currentState == Flower.FlowerState.Healthy);
			}
			if (updateVisual)
			{
				this.LocalUpdateFlowers(newState, isWatered);
			}
		}

		// Token: 0x06005F7D RID: 24445 RVA: 0x001EBB68 File Offset: 0x001E9D68
		private void LocalUpdateFlowers(Flower.FlowerState state, bool isWatered = false)
		{
			GameObject[] array = this.meshStates;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			if (!this.shouldUpdateVisuals)
			{
				this.meshStates[(int)this.currentState].SetActive(true);
				return;
			}
			if (isWatered && this.wateredFx)
			{
				this.wateredFx.Play();
			}
			this.meshRenderer.enabled = true;
			this.anim.enabled = true;
			switch (state)
			{
			case Flower.FlowerState.Healthy:
				this.anim.SetTrigger(Flower.middle_to_healthy);
				return;
			case Flower.FlowerState.Middle:
				if (this.lastState == Flower.FlowerState.Wilted)
				{
					this.anim.SetTrigger(Flower.wilted_to_middle);
					return;
				}
				this.anim.SetTrigger(Flower.healthy_to_middle);
				return;
			case Flower.FlowerState.Wilted:
				this.anim.SetTrigger(Flower.middle_to_wilted);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005F7E RID: 24446 RVA: 0x001EBC41 File Offset: 0x001E9E41
		private void HandleOnFlowerTimerEnded(GorillaTimer _timer)
		{
			if (!FlowersManager.Instance.IsMine)
			{
				return;
			}
			if (this.timer == _timer)
			{
				this.WaterFlower(false);
			}
		}

		// Token: 0x06005F7F RID: 24447 RVA: 0x001EBC65 File Offset: 0x001E9E65
		private void ChangeState(Flower.FlowerState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
		}

		// Token: 0x06005F80 RID: 24448 RVA: 0x001EBC7A File Offset: 0x001E9E7A
		public Flower.FlowerState GetCurrentState()
		{
			return this.currentState;
		}

		// Token: 0x06005F81 RID: 24449 RVA: 0x001EBC84 File Offset: 0x001E9E84
		public void OnAnimationIsDone(int state)
		{
			if (this.meshRenderer.enabled)
			{
				for (int i = 0; i < this.meshStates.Length; i++)
				{
					bool active = i == (int)this.currentState;
					this.meshStates[i].SetActive(active);
				}
				this.anim.enabled = false;
				this.meshRenderer.enabled = false;
			}
		}

		// Token: 0x06005F82 RID: 24450 RVA: 0x001EBCE1 File Offset: 0x001E9EE1
		public void UpdateVisuals(bool enable)
		{
			this.shouldUpdateVisuals = enable;
			this.meshStatesGameObject.SetActive(enable);
		}

		// Token: 0x06005F83 RID: 24451 RVA: 0x001EBCF8 File Offset: 0x001E9EF8
		public void AnimCatch()
		{
			if (this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				this.OnAnimationIsDone(0);
			}
		}

		// Token: 0x04006E2D RID: 28205
		private Animator anim;

		// Token: 0x04006E2E RID: 28206
		private SkinnedMeshRenderer meshRenderer;

		// Token: 0x04006E2F RID: 28207
		[HideInInspector]
		public GorillaTimer timer;

		// Token: 0x04006E30 RID: 28208
		private BeePerchPoint perchPoint;

		// Token: 0x04006E31 RID: 28209
		public ParticleSystem wateredFx;

		// Token: 0x04006E32 RID: 28210
		public ParticleSystem sparkleFx;

		// Token: 0x04006E33 RID: 28211
		public GameObject meshStatesGameObject;

		// Token: 0x04006E34 RID: 28212
		public GameObject[] meshStates;

		// Token: 0x04006E35 RID: 28213
		private static readonly int healthy_to_middle = Animator.StringToHash("healthy_to_middle");

		// Token: 0x04006E36 RID: 28214
		private static readonly int middle_to_healthy = Animator.StringToHash("middle_to_healthy");

		// Token: 0x04006E37 RID: 28215
		private static readonly int wilted_to_middle = Animator.StringToHash("wilted_to_middle");

		// Token: 0x04006E38 RID: 28216
		private static readonly int middle_to_wilted = Animator.StringToHash("middle_to_wilted");

		// Token: 0x04006E39 RID: 28217
		private Flower.FlowerState currentState;

		// Token: 0x04006E3A RID: 28218
		private string id;

		// Token: 0x04006E3B RID: 28219
		private bool shouldUpdateVisuals;

		// Token: 0x04006E3C RID: 28220
		private Flower.FlowerState lastState;

		// Token: 0x02000F01 RID: 3841
		public enum FlowerState
		{
			// Token: 0x04006E3F RID: 28223
			None = -1,
			// Token: 0x04006E40 RID: 28224
			Healthy,
			// Token: 0x04006E41 RID: 28225
			Middle,
			// Token: 0x04006E42 RID: 28226
			Wilted
		}
	}
}
