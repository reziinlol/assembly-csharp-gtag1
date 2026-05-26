using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000ACA RID: 2762
public class GizmoRenderer : MonoBehaviour
{
	// Token: 0x06004695 RID: 18069 RVA: 0x0017E154 File Offset: 0x0017C354
	private void Update()
	{
		this.RenderGizmos();
	}

	// Token: 0x06004696 RID: 18070 RVA: 0x0017E15C File Offset: 0x0017C35C
	private unsafe void RenderGizmos()
	{
		if (this.renderMode == GizmoRenderer.RenderMode.Never)
		{
			return;
		}
		if (this.gizmos == null)
		{
			return;
		}
		int num = this.gizmos.Length;
		if (num == 0)
		{
			return;
		}
		CommandBuilder arg = *Draw.ingame;
		Transform transform = base.transform;
		for (int i = 0; i < num; i++)
		{
			GizmoRenderer.GizmoInfo gizmoInfo = this.gizmos[i];
			if (gizmoInfo.render)
			{
				Transform transform2 = gizmoInfo.target ? gizmoInfo.target : transform;
				using (arg.InLocalSpace(transform2))
				{
					using (arg.WithLineWidth(gizmoInfo.lineWidth, false))
					{
						GizmoRenderer.gRenderFuncs[(int)gizmoInfo.type](arg, gizmoInfo);
					}
				}
			}
		}
	}

	// Token: 0x06004697 RID: 18071 RVA: 0x0017E248 File Offset: 0x0017C448
	private static void RenderPlaneWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WirePlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x06004698 RID: 18072 RVA: 0x0017E26E File Offset: 0x0017C46E
	private static void RenderPlaneSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidPlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x06004699 RID: 18073 RVA: 0x0017E294 File Offset: 0x0017C494
	private static void RenderGridWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireGrid(gizmo.center, gizmo.rotation, gizmo.gridCells, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x0600469A RID: 18074 RVA: 0x0017E2C0 File Offset: 0x0017C4C0
	private static void RenderBoxWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x0600469B RID: 18075 RVA: 0x0017E2E1 File Offset: 0x0017C4E1
	private static void RenderBoxSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x0600469C RID: 18076 RVA: 0x0017E302 File Offset: 0x0017C502
	private static void RenderSphereWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireSphere(gizmo.center, gizmo.radius * 0.5f, gizmo.color);
	}

	// Token: 0x0600469D RID: 18077 RVA: 0x0017E324 File Offset: 0x0017C524
	private static void RenderSphereSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		Matrix4x4 matrix = Matrix4x4.TRS(gizmo.center, quaternion.identity, new float3(gizmo.radius));
		using (draw.WithMatrix(matrix))
		{
			draw.SolidMesh(GizmoRenderer.gSphereMesh, gizmo.color);
		}
	}

	// Token: 0x0600469E RID: 18078 RVA: 0x0017E398 File Offset: 0x0017C598
	private static void RenderLabel3D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label3D(gizmo.center, gizmo.rotation, gizmo.text, gizmo.textSize * 0.1f, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x0600469F RID: 18079 RVA: 0x0017E3D5 File Offset: 0x0017C5D5
	private static void RenderLabel2D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label2D(gizmo.center, gizmo.text, gizmo.textSize * gizmo.textPPU, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x060046A0 RID: 18080 RVA: 0x0017E40F File Offset: 0x0017C60F
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		GizmoRenderer.gSphereMesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
	}

	// Token: 0x060046A1 RID: 18081 RVA: 0x0017E420 File Offset: 0x0017C620
	private static Color GetRandomColor()
	{
		Color result = Color.HSVToRGB((float)(DateTime.UtcNow.Ticks % 65536L) / 65535f, 1f, 1f, true);
		result.a = 1f;
		return result;
	}

	// Token: 0x040058F4 RID: 22772
	public GizmoRenderer.RenderMode renderMode = GizmoRenderer.RenderMode.Always;

	// Token: 0x040058F5 RID: 22773
	public bool includeInBuild;

	// Token: 0x040058F6 RID: 22774
	public GizmoRenderer.GizmoInfo[] gizmos = new GizmoRenderer.GizmoInfo[0];

	// Token: 0x040058F7 RID: 22775
	private static readonly Action<CommandBuilder, GizmoRenderer.GizmoInfo>[] gRenderFuncs = new Action<CommandBuilder, GizmoRenderer.GizmoInfo>[]
	{
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel3D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel2D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderGridWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneWire)
	};

	// Token: 0x040058F8 RID: 22776
	private static readonly LabelAlignment[] gLabelAligns = new LabelAlignment[]
	{
		LabelAlignment.Center,
		LabelAlignment.MiddleRight,
		LabelAlignment.MiddleLeft,
		LabelAlignment.BottomCenter,
		LabelAlignment.BottomRight,
		LabelAlignment.BottomLeft,
		LabelAlignment.TopRight,
		LabelAlignment.TopLeft,
		LabelAlignment.TopCenter
	};

	// Token: 0x040058F9 RID: 22777
	private static Mesh gSphereMesh;

	// Token: 0x02000ACB RID: 2763
	[Serializable]
	public class GizmoInfo
	{
		// Token: 0x040058FA RID: 22778
		public bool render = true;

		// Token: 0x040058FB RID: 22779
		public GizmoRenderer.GizmoType type;

		// Token: 0x040058FC RID: 22780
		public Color color = GizmoRenderer.GetRandomColor();

		// Token: 0x040058FD RID: 22781
		public uint lineWidth = 1U;

		// Token: 0x040058FE RID: 22782
		[Space]
		public Transform target;

		// Token: 0x040058FF RID: 22783
		[Space]
		public float3 center = float3.zero;

		// Token: 0x04005900 RID: 22784
		public float3 size = Vector3.one;

		// Token: 0x04005901 RID: 22785
		public float radius = 1f;

		// Token: 0x04005902 RID: 22786
		public quaternion rotation = quaternion.identity;

		// Token: 0x04005903 RID: 22787
		[Space]
		public string text = string.Empty;

		// Token: 0x04005904 RID: 22788
		public float textSize = 4f;

		// Token: 0x04005905 RID: 22789
		public GizmoRenderer.TextAlign textAlign;

		// Token: 0x04005906 RID: 22790
		public uint textPPU = 24U;

		// Token: 0x04005907 RID: 22791
		[Space]
		public int2 gridCells = new int2(4);
	}

	// Token: 0x02000ACC RID: 2764
	[Flags]
	public enum RenderMode : uint
	{
		// Token: 0x04005909 RID: 22793
		Never = 0U,
		// Token: 0x0400590A RID: 22794
		InEditor = 1U,
		// Token: 0x0400590B RID: 22795
		InBuild = 2U,
		// Token: 0x0400590C RID: 22796
		Always = 3U
	}

	// Token: 0x02000ACD RID: 2765
	public enum GizmoType : uint
	{
		// Token: 0x0400590E RID: 22798
		BoxWire,
		// Token: 0x0400590F RID: 22799
		BoxSolid,
		// Token: 0x04005910 RID: 22800
		SphereWire,
		// Token: 0x04005911 RID: 22801
		SphereSolid,
		// Token: 0x04005912 RID: 22802
		Label3D,
		// Token: 0x04005913 RID: 22803
		Label2D,
		// Token: 0x04005914 RID: 22804
		GridWire,
		// Token: 0x04005915 RID: 22805
		PlaneSolid,
		// Token: 0x04005916 RID: 22806
		PlaneWire
	}

	// Token: 0x02000ACE RID: 2766
	public enum TextAlign : uint
	{
		// Token: 0x04005918 RID: 22808
		Center,
		// Token: 0x04005919 RID: 22809
		MiddleRight,
		// Token: 0x0400591A RID: 22810
		MiddleLeft,
		// Token: 0x0400591B RID: 22811
		BottomCenter,
		// Token: 0x0400591C RID: 22812
		BottomRight,
		// Token: 0x0400591D RID: 22813
		BottomLeft,
		// Token: 0x0400591E RID: 22814
		TopRight,
		// Token: 0x0400591F RID: 22815
		TopLeft,
		// Token: 0x04005920 RID: 22816
		TopCenter
	}
}
