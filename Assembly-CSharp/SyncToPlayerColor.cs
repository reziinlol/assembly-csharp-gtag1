using System;
using UnityEngine;

// Token: 0x0200059C RID: 1436
public class SyncToPlayerColor : MonoBehaviour
{
	// Token: 0x06002462 RID: 9314 RVA: 0x000C3AA5 File Offset: 0x000C1CA5
	protected virtual void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this._colorFunc = new Action<Color>(this.UpdateColor);
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x000C3AC6 File Offset: 0x000C1CC6
	protected virtual void Start()
	{
		this.UpdateColor(this.rig.playerColor);
		this.rig.OnColorInitialized(this._colorFunc);
	}

	// Token: 0x06002464 RID: 9316 RVA: 0x000C3AEA File Offset: 0x000C1CEA
	protected virtual void OnEnable()
	{
		this.rig.OnColorChanged += this._colorFunc;
	}

	// Token: 0x06002465 RID: 9317 RVA: 0x000C3AFD File Offset: 0x000C1CFD
	protected virtual void OnDisable()
	{
		this.rig.OnColorChanged -= this._colorFunc;
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x000C3B10 File Offset: 0x000C1D10
	public virtual void UpdateColor(Color color)
	{
		if (!this.target)
		{
			return;
		}
		if (this.colorPropertiesToSync == null)
		{
			return;
		}
		for (int i = 0; i < this.colorPropertiesToSync.Length; i++)
		{
			ShaderHashId h = this.colorPropertiesToSync[i];
			this.target.SetColor(h, color);
		}
	}

	// Token: 0x04002FCE RID: 12238
	public VRRig rig;

	// Token: 0x04002FCF RID: 12239
	public Material target;

	// Token: 0x04002FD0 RID: 12240
	public ShaderHashId[] colorPropertiesToSync = new ShaderHashId[]
	{
		"_BaseColor"
	};

	// Token: 0x04002FD1 RID: 12241
	private Action<Color> _colorFunc;
}
