using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006A9 RID: 1705
public class GameDock : MonoBehaviour
{
	// Token: 0x06002A87 RID: 10887 RVA: 0x000E4693 File Offset: 0x000E2893
	private void Awake()
	{
		this.docked = new List<GameEntity>(1);
		if (this.dockMarker == null)
		{
			this.dockMarker = base.transform;
		}
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnEnable()
	{
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x000E46BB File Offset: 0x000E28BB
	public bool CanDock(GameDockable dockable)
	{
		return !(dockable == null) && (this.dockType != GameDockType.GRToolDock || this.GetDockedCount() <= 0);
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x000E46DE File Offset: 0x000E28DE
	public int GetDockedCount()
	{
		return this.docked.Count;
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x000E46EB File Offset: 0x000E28EB
	public void OnDock(GameEntity attachedGameEntity, GameEntity attachedToGameEntity)
	{
		this.dockSound.Play(null);
		this.docked.Add(attachedGameEntity);
		this.dockHaptic.PlayIfSnappedLocal(attachedToGameEntity);
	}

	// Token: 0x06002A8C RID: 10892 RVA: 0x000E4711 File Offset: 0x000E2911
	public void OnUndock(GameEntity gameEntity, GameEntity attachedToGameEntity)
	{
		this.undockSound.Play(null);
		this.docked.Remove(gameEntity);
	}

	// Token: 0x0400373C RID: 14140
	public GameEntity gameEntity;

	// Token: 0x0400373D RID: 14141
	public GameDockType dockType;

	// Token: 0x0400373E RID: 14142
	public float dockRadius = 0.15f;

	// Token: 0x0400373F RID: 14143
	public AbilitySound dockSound;

	// Token: 0x04003740 RID: 14144
	public AbilitySound undockSound;

	// Token: 0x04003741 RID: 14145
	public AbilityHaptic dockHaptic;

	// Token: 0x04003742 RID: 14146
	public Transform dockMarker;

	// Token: 0x04003743 RID: 14147
	private List<GameEntity> docked;
}
