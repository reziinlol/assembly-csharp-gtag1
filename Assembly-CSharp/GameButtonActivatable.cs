using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020006A6 RID: 1702
public class GameButtonActivatable : MonoBehaviour, IGameActivatable
{
	// Token: 0x06002A84 RID: 10884 RVA: 0x000E4554 File Offset: 0x000E2754
	public bool CheckInput(XRNode xrNode, float sensitivity = 0.25f)
	{
		switch (this.inputButton)
		{
		case GameButtonActivatable.InputButton.Trigger:
			return ControllerInputPoller.TriggerFloat(xrNode) > sensitivity;
		case GameButtonActivatable.InputButton.ButtonA:
			return ControllerInputPoller.PrimaryButtonPress(xrNode);
		case GameButtonActivatable.InputButton.ButtonB:
			return ControllerInputPoller.SecondaryButtonPress(xrNode);
		case GameButtonActivatable.InputButton.Grip:
			return ControllerInputPoller.GripFloat(xrNode) > sensitivity;
		case GameButtonActivatable.InputButton.Joystick:
			return ControllerInputPoller.TriggerFloat(xrNode) > sensitivity;
		default:
			return false;
		}
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x000E45B4 File Offset: 0x000E27B4
	public bool CheckInput(float sensitivity = 0.25f)
	{
		int equippedSlotIndex = this.gameEntity.EquippedSlotIndex;
		if (equippedSlotIndex == -1 || !this.gameEntity.IsHeldOrSnappedByLocalPlayer)
		{
			return false;
		}
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		if (this.gameEntity.IsSnappedToHand)
		{
			int num;
			if (equippedSlotIndex != 2)
			{
				if (equippedSlotIndex != 3)
				{
					num = -1;
				}
				else
				{
					num = 1;
				}
			}
			else
			{
				num = 0;
			}
			int num2 = num;
			GameEntity gameEntity;
			IGameActivatable gameActivatable;
			if (gamePlayer.TryGetSlotEntity(num2, out gameEntity) && gameEntity.TryGetComponent<IGameActivatable>(out gameActivatable))
			{
				return false;
			}
			if (this.inputButton == GameButtonActivatable.InputButton.Trigger && GameTriggerInteractable.LocalInteractableTriggers.Count > 0)
			{
				Vector3 position = gamePlayer.GetHandTransform(num2).position;
				for (int i = 0; i < GameTriggerInteractable.LocalInteractableTriggers.Count; i++)
				{
					if (GameTriggerInteractable.LocalInteractableTriggers[i].PointWithinInteractableArea(position))
					{
						return false;
					}
				}
			}
		}
		return this.CheckInput(this.gameEntity.EquippedHandXRNode, sensitivity);
	}

	// Token: 0x04003732 RID: 14130
	[SerializeField]
	public GameButtonActivatable.InputButton inputButton;

	// Token: 0x04003733 RID: 14131
	public GameEntity gameEntity;

	// Token: 0x020006A7 RID: 1703
	public enum InputButton
	{
		// Token: 0x04003735 RID: 14133
		Trigger,
		// Token: 0x04003736 RID: 14134
		ButtonA,
		// Token: 0x04003737 RID: 14135
		ButtonB,
		// Token: 0x04003738 RID: 14136
		Grip,
		// Token: 0x04003739 RID: 14137
		Joystick
	}
}
