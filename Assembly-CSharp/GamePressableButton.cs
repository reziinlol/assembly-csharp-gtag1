using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006E7 RID: 1767
public class GamePressableButton : MonoBehaviour, IClickable
{
	// Token: 0x06002C99 RID: 11417 RVA: 0x000F13A8 File Offset: 0x000EF5A8
	public void Click(bool leftHand = false)
	{
		this.PressButton(leftHand);
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x000F13B4 File Offset: 0x000EF5B4
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = collider.gameObject.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (!component)
		{
			return;
		}
		if (!this.CheckValidEquippedState(component.isLeftHand))
		{
			return;
		}
		this.PressButton(component.isLeftHand);
	}

	// Token: 0x06002C9B RID: 11419 RVA: 0x000F1410 File Offset: 0x000EF610
	private bool CheckValidEquippedState(bool pressedHandLeft)
	{
		if (!this.requireEquipped)
		{
			return true;
		}
		int num = -1;
		GamePlayer gamePlayer;
		if (this.gameEntity.IsHeldByLocalPlayer() && this.activeWhileGrabbed && GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			num = gamePlayer.FindHandIndex(this.gameEntity.id);
		}
		GamePlayer gamePlayer2;
		if (num == -1 && this.gameEntity.IsSnappedByLocalPlayer() && this.activeWhileSnapped && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer2))
		{
			num = gamePlayer2.FindSnapIndex(this.gameEntity.id);
		}
		if (num == -1)
		{
			return false;
		}
		bool flag = GamePlayer.IsLeftHand(num);
		return pressedHandLeft != flag;
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x000F14B8 File Offset: 0x000EF6B8
	private void PressButton(bool isLeftHand)
	{
		this.touchTime = Time.time;
		UnityEvent unityEvent = this.onPressButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
			{
				67,
				isLeftHand,
				0.05f
			});
		}
	}

	// Token: 0x040038F3 RID: 14579
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x040038F4 RID: 14580
	[SerializeField]
	private bool requireEquipped;

	// Token: 0x040038F5 RID: 14581
	[SerializeField]
	private bool activeWhileGrabbed;

	// Token: 0x040038F6 RID: 14582
	[SerializeField]
	private bool activeWhileSnapped;

	// Token: 0x040038F7 RID: 14583
	public UnityEvent onPressButton;

	// Token: 0x040038F8 RID: 14584
	[Header("Button Press")]
	public float debounceTime = 0.25f;

	// Token: 0x040038F9 RID: 14585
	public int pressButtonSoundIndex = 67;

	// Token: 0x040038FA RID: 14586
	private float touchTime;
}
