using System;
using UnityEngine;

// Token: 0x020006DB RID: 1755
public class GameMirrorWhenEquipped : MonoBehaviour
{
	// Token: 0x06002C21 RID: 11297 RVA: 0x000EEA04 File Offset: 0x000ECC04
	private void Awake()
	{
		if (this.m_gameEntity == null)
		{
			this.m_gameEntity = base.GetComponent<GameEntity>();
		}
		if (this.m_xformsToMirror == null)
		{
			this.m_xformsToMirror = Array.Empty<Transform>();
		}
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x000EEA34 File Offset: 0x000ECC34
	protected void OnEnable()
	{
		GameEntity gameEntity = this.m_gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity2 = this.m_gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity3 = this.m_gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity4 = this.m_gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleGameEntityOnEquipChanged));
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x000EEAE0 File Offset: 0x000ECCE0
	protected void OnDisable()
	{
		GameEntity gameEntity = this.m_gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity2 = this.m_gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity3 = this.m_gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity4 = this.m_gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this._HandleGameEntityOnEquipChanged));
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x000EEB8C File Offset: 0x000ECD8C
	private void _HandleGameEntityOnEquipChanged()
	{
		if (this.m_shouldOnlyMirrorWhenSnapped && this.m_gameEntity.snappedJoint == SnapJointType.None)
		{
			return;
		}
		Vector3 localScale = (this.m_gameEntity.EquippedHandedness == this.m_handednessToMirror) ? new Vector3(-1f, 1f, 1f) : Vector3.one;
		for (int i = 0; i < this.m_xformsToMirror.Length; i++)
		{
			this.m_xformsToMirror[i].localScale = localScale;
		}
	}

	// Token: 0x04003894 RID: 14484
	[SerializeField]
	private GameEntity m_gameEntity;

	// Token: 0x04003895 RID: 14485
	[SerializeField]
	private Transform[] m_xformsToMirror;

	// Token: 0x04003896 RID: 14486
	[SerializeField]
	private bool m_shouldOnlyMirrorWhenSnapped = true;

	// Token: 0x04003897 RID: 14487
	[Tooltip("Set the X axis scale to -1 if the gadget is attached (held or snapped) to the selected side.")]
	[SerializeField]
	private EHandedness m_handednessToMirror = EHandedness.Right;
}
