using System;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02000F8A RID: 3978
	public static class GRGameObjectExtensions
	{
		// Token: 0x0600633E RID: 25406 RVA: 0x001FECEC File Offset: 0x001FCEEC
		public static GRTool.GRToolType GetToolType(this GameObject obj)
		{
			if (obj.GetComponentInParent<GRToolClub>() != null)
			{
				return GRTool.GRToolType.Club;
			}
			if (obj.GetComponentInParent<GRToolCollector>() != null)
			{
				return GRTool.GRToolType.Collector;
			}
			if (obj.GetComponentInParent<GRToolFlash>() != null)
			{
				return GRTool.GRToolType.Flash;
			}
			if (obj.GetComponentInParent<GRToolLantern>() != null)
			{
				return GRTool.GRToolType.Lantern;
			}
			if (obj.GetComponentInParent<GRToolRevive>() != null)
			{
				return GRTool.GRToolType.Revive;
			}
			if (obj.GetComponentInParent<GRToolShieldGun>() != null)
			{
				return GRTool.GRToolType.ShieldGun;
			}
			if (obj.GetComponentInParent<GRToolDirectionalShield>() != null)
			{
				return GRTool.GRToolType.DirectionalShield;
			}
			GRTool componentInParent = obj.GetComponentInParent<GRTool>();
			if (componentInParent != null && componentInParent.toolType == GRTool.GRToolType.HockeyStick)
			{
				return GRTool.GRToolType.HockeyStick;
			}
			componentInParent = obj.GetComponentInParent<GRTool>();
			if (componentInParent != null && componentInParent.toolType == GRTool.GRToolType.DockWrist)
			{
				return GRTool.GRToolType.DockWrist;
			}
			return GRTool.GRToolType.None;
		}
	}
}
