using System;

// Token: 0x0200069C RID: 1692
[Serializable]
public struct GroupJoinZoneAB
{
	// Token: 0x06002A16 RID: 10774 RVA: 0x000E2ECC File Offset: 0x000E10CC
	public static GroupJoinZoneAB operator &(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a & two.a),
			b = (one.b & two.b)
		};
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x000E2F0C File Offset: 0x000E110C
	public static GroupJoinZoneAB operator |(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a | two.a),
			b = (one.b | two.b)
		};
	}

	// Token: 0x06002A18 RID: 10776 RVA: 0x000E2F4C File Offset: 0x000E114C
	public static GroupJoinZoneAB operator ~(GroupJoinZoneAB z)
	{
		return new GroupJoinZoneAB
		{
			a = ~z.a,
			b = ~z.b
		};
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x000E2F7E File Offset: 0x000E117E
	public static bool operator ==(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a == two.a && one.b == two.b;
	}

	// Token: 0x06002A1A RID: 10778 RVA: 0x000E2F9E File Offset: 0x000E119E
	public static bool operator !=(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a != two.a || one.b != two.b;
	}

	// Token: 0x06002A1B RID: 10779 RVA: 0x000E2FC1 File Offset: 0x000E11C1
	public bool HasAnyFlag(GroupJoinZoneAB other)
	{
		return (this.a & other.a) != ~(GroupJoinZoneA.Basement | GroupJoinZoneA.Beach | GroupJoinZoneA.Cave | GroupJoinZoneA.Canyon | GroupJoinZoneA.City | GroupJoinZoneA.Clouds | GroupJoinZoneA.Forest | GroupJoinZoneA.Mountain | GroupJoinZoneA.Rotating | GroupJoinZoneA.Mines | GroupJoinZoneA.Arena | GroupJoinZoneA.ArenaTunnel | GroupJoinZoneA.Hoverboard | GroupJoinZoneA.TreeRoom | GroupJoinZoneA.MountainTunnel | GroupJoinZoneA.BasementTunnel | GroupJoinZoneA.RotatingTunnel | GroupJoinZoneA.BeachTunnel | GroupJoinZoneA.CloudsElevator | GroupJoinZoneA.MinesTunnel | GroupJoinZoneA.CavesComputer | GroupJoinZoneA.Metropolis | GroupJoinZoneA.MetropolisTunnel | GroupJoinZoneA.Attic | GroupJoinZoneA.Arcade | GroupJoinZoneA.ArcadeTunnel | GroupJoinZoneA.Bayou | GroupJoinZoneA.BayouTunnel | GroupJoinZoneA.CustomMaps | GroupJoinZoneA.MallConnector | GroupJoinZoneA.MonkeBlocks | GroupJoinZoneA.GTFC) || (this.b & other.b) > (GroupJoinZoneB)0;
	}

	// Token: 0x06002A1C RID: 10780 RVA: 0x000E2FE4 File Offset: 0x000E11E4
	public override bool Equals(object other)
	{
		return this == (GroupJoinZoneAB)other;
	}

	// Token: 0x06002A1D RID: 10781 RVA: 0x000E2FF7 File Offset: 0x000E11F7
	public override int GetHashCode()
	{
		return this.a.GetHashCode() ^ this.b.GetHashCode();
	}

	// Token: 0x06002A1E RID: 10782 RVA: 0x000E301C File Offset: 0x000E121C
	public static implicit operator GroupJoinZoneAB(int d)
	{
		return new GroupJoinZoneAB
		{
			a = (GroupJoinZoneA)d
		};
	}

	// Token: 0x06002A1F RID: 10783 RVA: 0x000E303C File Offset: 0x000E123C
	public override string ToString()
	{
		if (this.b == (GroupJoinZoneB)0)
		{
			return this.a.ToString();
		}
		if (this.a != ~(GroupJoinZoneA.Basement | GroupJoinZoneA.Beach | GroupJoinZoneA.Cave | GroupJoinZoneA.Canyon | GroupJoinZoneA.City | GroupJoinZoneA.Clouds | GroupJoinZoneA.Forest | GroupJoinZoneA.Mountain | GroupJoinZoneA.Rotating | GroupJoinZoneA.Mines | GroupJoinZoneA.Arena | GroupJoinZoneA.ArenaTunnel | GroupJoinZoneA.Hoverboard | GroupJoinZoneA.TreeRoom | GroupJoinZoneA.MountainTunnel | GroupJoinZoneA.BasementTunnel | GroupJoinZoneA.RotatingTunnel | GroupJoinZoneA.BeachTunnel | GroupJoinZoneA.CloudsElevator | GroupJoinZoneA.MinesTunnel | GroupJoinZoneA.CavesComputer | GroupJoinZoneA.Metropolis | GroupJoinZoneA.MetropolisTunnel | GroupJoinZoneA.Attic | GroupJoinZoneA.Arcade | GroupJoinZoneA.ArcadeTunnel | GroupJoinZoneA.Bayou | GroupJoinZoneA.BayouTunnel | GroupJoinZoneA.CustomMaps | GroupJoinZoneA.MallConnector | GroupJoinZoneA.MonkeBlocks | GroupJoinZoneA.GTFC))
		{
			return this.a.ToString() + "," + this.b.ToString();
		}
		return this.b.ToString();
	}

	// Token: 0x04003701 RID: 14081
	public GroupJoinZoneA a;

	// Token: 0x04003702 RID: 14082
	public GroupJoinZoneB b;
}
