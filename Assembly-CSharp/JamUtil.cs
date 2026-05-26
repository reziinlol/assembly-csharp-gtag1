using System;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public static class JamUtil
{
	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000CB0 RID: 3248 RVA: 0x00045E6B File Offset: 0x0004406B
	public static bool IsPlaying
	{
		get
		{
			return Application.isPlaying;
		}
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x00045E72 File Offset: 0x00044072
	public static void Destroy(Object obj)
	{
		Object.Destroy(obj);
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x00045E7C File Offset: 0x0004407C
	public static RaycastHit ToRaycastHit(this Collision collision)
	{
		RaycastHit result;
		if (!collision.ConvertToRaycast(out result))
		{
			GTDev.LogError<string>(string.Format("No hit! ({0})", collision), null);
		}
		return result;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x00045EA8 File Offset: 0x000440A8
	public static bool ConvertToRaycast(this Collision collision, out RaycastHit hit)
	{
		ContactPoint contact = collision.GetContact(0);
		Vector3 point = contact.point;
		Vector3 normal = contact.normal;
		LayerMask mask = 1 << collision.gameObject.layer;
		return Physics.Raycast(new Ray(point + normal * 0.1f, -normal), out hit, 0.2f, mask, QueryTriggerInteraction.Ignore);
	}
}
