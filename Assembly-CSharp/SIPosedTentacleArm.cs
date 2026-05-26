using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200011B RID: 283
[ExecuteAlways]
public class SIPosedTentacleArm : MonoBehaviour
{
	// Token: 0x06000714 RID: 1812 RVA: 0x00028756 File Offset: 0x00026956
	public void ConfigureFrom(SIGadgetTentacleArm source, MeshRenderer rend1, MeshRenderer rend2, Transform anchor1, Transform anchor2)
	{
		this.LengthFactor = source.LengthFactor;
		this.tentacleRenderer = rend1;
		this.tentacleRenderer2 = rend2;
		this.tentacleAnchor = anchor1;
		this.tentacleAnchor2 = anchor2;
		this.tentacleSharedMaterial = rend1.sharedMaterial;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0002878E File Offset: 0x0002698E
	private void Start()
	{
		this.UpdateTentaclePose();
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00023994 File Offset: 0x00021B94
	private bool CanUpdateTentaclePose()
	{
		return true;
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00028798 File Offset: 0x00026998
	private void EnsureMaterialsInitialized()
	{
		if (this._initialized)
		{
			return;
		}
		this._tentacleMat = new Material(this.tentacleSharedMaterial);
		this.tentacleRenderer.material = this._tentacleMat;
		this._hasTentacle2 = this.tentacleRenderer2;
		if (this._hasTentacle2)
		{
			this._tentacleMat2 = new Material(this.tentacleSharedMaterial);
			this.tentacleRenderer2.material = this._tentacleMat2;
		}
		this._initialized = true;
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00028814 File Offset: 0x00026A14
	private void UpdateTentaclePose()
	{
		if (!this.CanUpdateTentaclePose())
		{
			return;
		}
		this.EnsureMaterialsInitialized();
		this.UpdateTentacle(this._tentacleMat, this.tentacleRenderer.transform, this.tentacleAnchor);
		if (this._hasTentacle2)
		{
			this.UpdateTentacle(this._tentacleMat2, this.tentacleRenderer2.transform, this.tentacleAnchor2);
		}
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00028874 File Offset: 0x00026A74
	private void UpdateTentacle(Material material, Transform tentacle, Transform anchor)
	{
		Vector3 vector = Vector3.forward * this.LengthFactor;
		material.SetVector(this.tentacleStartDir_HASH, vector);
		Vector3 vector2 = tentacle.InverseTransformPoint(anchor.position);
		material.SetVector(this.tentacleEnd_HASH, vector2);
		Vector3 vector3 = -tentacle.InverseTransformDirection(anchor.forward) * this.LengthFactor;
		material.SetVector(this.tentacleEndDir_HASH, vector3);
		Vector3 vector4 = SIGadgetTentacleArm.SplineSample(0.25f, vector, vector2, vector3);
		Vector3 a = SIGadgetTentacleArm.SplineSample(0.26f, vector, vector2, vector3);
		Vector3 vector5 = SIGadgetTentacleArm.SplineSample(0.75f, vector, vector2, vector3);
		Vector3 a2 = SIGadgetTentacleArm.SplineSample(0.76f, vector, vector2, vector3);
		Vector3 planeIntersection = SIGadgetTentacleArm.GetPlaneIntersection(vector4, (a - vector4).normalized, vector5, (a2 - vector5).normalized, Quaternion.AngleAxis(90f, Vector3.forward) * vector2.WithZ(0f).normalized);
		material.SetVector(this.tentacleRingOrigin_HASH, planeIntersection);
	}

	// Token: 0x040008F3 RID: 2291
	public float LengthFactor = 1.5f;

	// Token: 0x040008F4 RID: 2292
	public MeshRenderer tentacleRenderer;

	// Token: 0x040008F5 RID: 2293
	public MeshRenderer tentacleRenderer2;

	// Token: 0x040008F6 RID: 2294
	public Transform tentacleAnchor;

	// Token: 0x040008F7 RID: 2295
	public Transform tentacleAnchor2;

	// Token: 0x040008F8 RID: 2296
	public Material tentacleSharedMaterial;

	// Token: 0x040008F9 RID: 2297
	private bool _initialized;

	// Token: 0x040008FA RID: 2298
	private bool _hasTentacle2;

	// Token: 0x040008FB RID: 2299
	private Material _tentacleMat;

	// Token: 0x040008FC RID: 2300
	private Material _tentacleMat2;

	// Token: 0x040008FD RID: 2301
	private Vector3 _lastPos;

	// Token: 0x040008FE RID: 2302
	private Vector3 _lastAnchorPos;

	// Token: 0x040008FF RID: 2303
	private ShaderHashId tentacleStartDir_HASH = "_TentacleStartDir";

	// Token: 0x04000900 RID: 2304
	private ShaderHashId tentacleEnd_HASH = "_TentacleEndPos";

	// Token: 0x04000901 RID: 2305
	private ShaderHashId tentacleEndDir_HASH = "_TentacleEndDir";

	// Token: 0x04000902 RID: 2306
	private ShaderHashId tentacleRingOrigin_HASH = "_TentacleRingOrigin";
}
