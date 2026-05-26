using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E09 RID: 3593
[Serializable]
public class VoiceLoudnessReactorRendererColorTarget
{
	// Token: 0x0600579E RID: 22430 RVA: 0x001C6688 File Offset: 0x001C4888
	public void Inititialize()
	{
		if (this._materials == null)
		{
			this._materials = new List<Material>(this.renderer.materials);
			this._materials[this.materialIndex].EnableKeyword(this.colorProperty);
			this.renderer.SetMaterials(this._materials);
			this.UpdateMaterialColor(0f);
		}
	}

	// Token: 0x0600579F RID: 22431 RVA: 0x001C66EC File Offset: 0x001C48EC
	public void UpdateMaterialColor(float level)
	{
		Color color = this.gradient.Evaluate(level);
		if (this._lastColor == color)
		{
			return;
		}
		this._materials[this.materialIndex].SetColor(this.colorProperty, color);
		this._lastColor = color;
	}

	// Token: 0x0400684F RID: 26703
	[SerializeField]
	private string colorProperty = "_BaseColor";

	// Token: 0x04006850 RID: 26704
	public Renderer renderer;

	// Token: 0x04006851 RID: 26705
	public int materialIndex;

	// Token: 0x04006852 RID: 26706
	public Gradient gradient;

	// Token: 0x04006853 RID: 26707
	public bool useSmoothedLoudness;

	// Token: 0x04006854 RID: 26708
	public float scale = 1f;

	// Token: 0x04006855 RID: 26709
	private List<Material> _materials;

	// Token: 0x04006856 RID: 26710
	private Color _lastColor = Color.white;
}
