using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F11 RID: 3857
	public class GorillaPlayerTimerButton : MonoBehaviour
	{
		// Token: 0x06006038 RID: 24632 RVA: 0x001F0614 File Offset: 0x001EE814
		private void Awake()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x06006039 RID: 24633 RVA: 0x001F0621 File Offset: 0x001EE821
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x0600603A RID: 24634 RVA: 0x001F0621 File Offset: 0x001EE821
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x0600603B RID: 24635 RVA: 0x001F062C File Offset: 0x001EE82C
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBothStartAndStop)
			{
				this.isStartButton = !PlayerTimerManager.instance.IsLocalTimerStarted();
			}
			this.isInitialized = true;
		}

		// Token: 0x0600603C RID: 24636 RVA: 0x001F06A8 File Offset: 0x001EE8A8
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
		}

		// Token: 0x0600603D RID: 24637 RVA: 0x001F06FF File Offset: 0x001EE8FF
		private void OnLocalTimerStarted()
		{
			if (this.isBothStartAndStop)
			{
				this.isStartButton = false;
			}
		}

		// Token: 0x0600603E RID: 24638 RVA: 0x001F0710 File Offset: 0x001EE910
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isBothStartAndStop && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStartButton = true;
			}
		}

		// Token: 0x0600603F RID: 24639 RVA: 0x001F0734 File Offset: 0x001EE934
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (Time.time < this.lastTriggeredTime + this.debounceTime)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, this.pressColor);
			this.mesh.SetPropertyBlock(this.materialProps);
			PlayerTimerManager.instance.RequestTimerToggle(this.isStartButton);
			this.lastTriggeredTime = Time.time;
		}

		// Token: 0x06006040 RID: 24640 RVA: 0x001F07F4 File Offset: 0x001EE9F4
		private void OnTriggerExit(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			if (other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
			{
				return;
			}
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, this.notPressedColor);
			this.mesh.SetPropertyBlock(this.materialProps);
		}

		// Token: 0x04006ECE RID: 28366
		private float lastTriggeredTime;

		// Token: 0x04006ECF RID: 28367
		[SerializeField]
		private bool isStartButton;

		// Token: 0x04006ED0 RID: 28368
		[SerializeField]
		private bool isBothStartAndStop;

		// Token: 0x04006ED1 RID: 28369
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x04006ED2 RID: 28370
		[SerializeField]
		private MeshRenderer mesh;

		// Token: 0x04006ED3 RID: 28371
		[SerializeField]
		private Color pressColor;

		// Token: 0x04006ED4 RID: 28372
		[SerializeField]
		private Color notPressedColor;

		// Token: 0x04006ED5 RID: 28373
		private MaterialPropertyBlock materialProps;

		// Token: 0x04006ED6 RID: 28374
		private bool isInitialized;
	}
}
