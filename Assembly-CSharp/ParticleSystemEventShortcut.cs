using System;
using UnityEngine;

// Token: 0x0200058B RID: 1419
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemEventShortcut : MonoBehaviour
{
	// Token: 0x060023FB RID: 9211 RVA: 0x000C15D4 File Offset: 0x000BF7D4
	private void InitIfNeeded()
	{
		if (!this.initialized)
		{
			this.initialized = true;
			this.ps = base.GetComponent<ParticleSystem>();
			this.shape = this.ps.shape;
			this.poolExists = ObjectPools.instance.DoesPoolExist(base.gameObject);
		}
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x000C1623 File Offset: 0x000BF823
	public void StopAndClear()
	{
		this.InitIfNeeded();
		this.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000C1638 File Offset: 0x000BF838
	public void ClearAndPlay()
	{
		this.InitIfNeeded();
		this.ps.Clear();
		this.ps.Play();
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x000C1656 File Offset: 0x000BF856
	public void PlayFromMesh(MeshRenderer mesh)
	{
		this.InitIfNeeded();
		this.shape.shapeType = ParticleSystemShapeType.MeshRenderer;
		this.shape.meshRenderer = mesh;
		this.ps.Play();
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x000C1682 File Offset: 0x000BF882
	public void PlayFromSkin(SkinnedMeshRenderer skin)
	{
		this.InitIfNeeded();
		this.shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
		this.shape.skinnedMeshRenderer = skin;
		this.ps.Play();
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000C16AE File Offset: 0x000BF8AE
	public void ReturnToPool()
	{
		this.InitIfNeeded();
		if (this.poolExists)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000C16CE File Offset: 0x000BF8CE
	private void OnParticleSystemStopped()
	{
		this.ReturnToPool();
	}

	// Token: 0x04002F36 RID: 12086
	private bool initialized;

	// Token: 0x04002F37 RID: 12087
	private ParticleSystem ps;

	// Token: 0x04002F38 RID: 12088
	private ParticleSystem.ShapeModule shape;

	// Token: 0x04002F39 RID: 12089
	private bool poolExists;
}
