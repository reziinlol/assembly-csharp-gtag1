using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020009BB RID: 2491
public class RadialMaterialFade : MonoBehaviour
{
	// Token: 0x06003FC1 RID: 16321 RVA: 0x00154DD8 File Offset: 0x00152FD8
	private void Update()
	{
		if (this.material == null || this.target == null)
		{
			return;
		}
		Camera mainCamera = GTPlayer.Instance.mainCamera;
		if (mainCamera == null)
		{
			return;
		}
		float value = Vector3.Distance(mainCamera.transform.position, this.target.position);
		float t = Mathf.InverseLerp(this.minDistance, this.maxDistance, value);
		float a = Mathf.Lerp(this.alphaAtMinDistance, this.alphaAtMaxDistance, t);
		Color color = this.material.GetColor(RadialMaterialFade.colorID);
		color.a = a;
		this.material.SetColor(RadialMaterialFade.colorID, color);
	}

	// Token: 0x04005033 RID: 20531
	[SerializeField]
	private Material material;

	// Token: 0x04005034 RID: 20532
	[SerializeField]
	private Transform target;

	// Token: 0x04005035 RID: 20533
	[Header("Distance")]
	[SerializeField]
	private float minDistance = 1f;

	// Token: 0x04005036 RID: 20534
	[SerializeField]
	private float maxDistance = 10f;

	// Token: 0x04005037 RID: 20535
	[Header("Alpha")]
	[SerializeField]
	[Range(0f, 1f)]
	private float alphaAtMinDistance;

	// Token: 0x04005038 RID: 20536
	[SerializeField]
	[Range(0f, 1f)]
	private float alphaAtMaxDistance = 1f;

	// Token: 0x04005039 RID: 20537
	private static readonly int colorID = Shader.PropertyToID("_Color");
}
