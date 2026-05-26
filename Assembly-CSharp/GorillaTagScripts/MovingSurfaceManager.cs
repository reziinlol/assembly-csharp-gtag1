using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ECD RID: 3789
	public class MovingSurfaceManager : MonoBehaviour
	{
		// Token: 0x06005D4E RID: 23886 RVA: 0x001D917C File Offset: 0x001D737C
		private void Awake()
		{
			if (MovingSurfaceManager.instance != null && MovingSurfaceManager.instance != this)
			{
				GTDev.LogWarning<string>("Instance of MovingSurfaceManager already exists. Destroying.", null);
				Object.Destroy(this);
				return;
			}
			if (MovingSurfaceManager.instance == null)
			{
				MovingSurfaceManager.instance = this;
			}
		}

		// Token: 0x06005D4F RID: 23887 RVA: 0x001D91C8 File Offset: 0x001D73C8
		public void RegisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.TryAdd(ms.GetID(), ms);
		}

		// Token: 0x06005D50 RID: 23888 RVA: 0x001D91DD File Offset: 0x001D73DD
		public void UnregisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.Remove(ms.GetID());
		}

		// Token: 0x06005D51 RID: 23889 RVA: 0x001D91F1 File Offset: 0x001D73F1
		public void RegisterSurfaceMover(SurfaceMover sm)
		{
			if (!this.surfaceMovers.Contains(sm))
			{
				this.surfaceMovers.Add(sm);
				sm.InitMovingSurface();
			}
		}

		// Token: 0x06005D52 RID: 23890 RVA: 0x001D9213 File Offset: 0x001D7413
		public void UnregisterSurfaceMover(SurfaceMover sm)
		{
			this.surfaceMovers.Remove(sm);
		}

		// Token: 0x06005D53 RID: 23891 RVA: 0x001D9222 File Offset: 0x001D7422
		public bool TryGetMovingSurface(int id, out MovingSurface result)
		{
			return this.movingSurfaces.TryGetValue(id, out result) && result != null;
		}

		// Token: 0x06005D54 RID: 23892 RVA: 0x001D9240 File Offset: 0x001D7440
		private void FixedUpdate()
		{
			foreach (SurfaceMover surfaceMover in this.surfaceMovers)
			{
				if (surfaceMover.isActiveAndEnabled)
				{
					surfaceMover.Move();
				}
			}
		}

		// Token: 0x04006BD4 RID: 27604
		private List<SurfaceMover> surfaceMovers = new List<SurfaceMover>(5);

		// Token: 0x04006BD5 RID: 27605
		private Dictionary<int, MovingSurface> movingSurfaces = new Dictionary<int, MovingSurface>(10);

		// Token: 0x04006BD6 RID: 27606
		public static MovingSurfaceManager instance;
	}
}
