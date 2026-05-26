using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020003FF RID: 1023
[ExecuteInEditMode]
public class LckRawImageFillCanvas : UIBehaviour
{
	// Token: 0x0600183C RID: 6204 RVA: 0x00089F42 File Offset: 0x00088142
	private new void OnEnable()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x00089F42 File Offset: 0x00088142
	private void Update()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x00089F4C File Offset: 0x0008814C
	private void UpdateSizeDelta()
	{
		if (this._rawImage == null || this._rawImage.texture == null)
		{
			return;
		}
		RectTransform rectTransform = this._rawImage.rectTransform;
		Vector2 sizeDelta = ((RectTransform)rectTransform.parent).sizeDelta;
		Vector2 vector = new Vector2((float)this._rawImage.texture.width, (float)this._rawImage.texture.height);
		float num = sizeDelta.x / sizeDelta.y;
		float num2 = vector.x / vector.y;
		float num3 = num / num2;
		Vector2 vector2 = new Vector2(sizeDelta.x, sizeDelta.x / num2);
		Vector2 vector3 = new Vector2(sizeDelta.y * num2, sizeDelta.y);
		switch (this._scaleType)
		{
		case LckRawImageFillCanvas.ScaleType.Fill:
			rectTransform.sizeDelta = ((num3 > 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Inset:
			rectTransform.sizeDelta = ((num3 < 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Stretch:
			rectTransform.sizeDelta = sizeDelta;
			return;
		default:
			return;
		}
	}

	// Token: 0x0400236D RID: 9069
	[SerializeField]
	private RawImage _rawImage;

	// Token: 0x0400236E RID: 9070
	[SerializeField]
	private LckRawImageFillCanvas.ScaleType _scaleType;

	// Token: 0x02000400 RID: 1024
	private enum ScaleType
	{
		// Token: 0x04002370 RID: 9072
		Fill,
		// Token: 0x04002371 RID: 9073
		Inset,
		// Token: 0x04002372 RID: 9074
		Stretch
	}
}
