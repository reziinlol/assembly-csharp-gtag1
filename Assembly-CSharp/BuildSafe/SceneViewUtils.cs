using System;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02001011 RID: 4113
	public static class SceneViewUtils
	{
		// Token: 0x060066A8 RID: 26280 RVA: 0x00210549 File Offset: 0x0020E749
		private static bool RaycastWorldSafe(Vector2 screenPos, out RaycastHit hit)
		{
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x04007608 RID: 30216
		public static readonly SceneViewUtils.FuncRaycastWorld RaycastWorld = new SceneViewUtils.FuncRaycastWorld(SceneViewUtils.RaycastWorldSafe);

		// Token: 0x02001012 RID: 4114
		// (Invoke) Token: 0x060066AB RID: 26283
		public delegate bool FuncRaycastWorld(Vector2 screenPos, out RaycastHit hit);

		// Token: 0x02001013 RID: 4115
		// (Invoke) Token: 0x060066AF RID: 26287
		public delegate GameObject FuncPickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);
	}
}
