using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000BE8 RID: 3048
public struct TextComponentLegacySupportStore
{
	// Token: 0x06004C08 RID: 19464 RVA: 0x00196600 File Offset: 0x00194800
	public TextComponentLegacySupportStore(Transform objRef)
	{
		this._objectReference = objRef;
		this._legacyTextReference = null;
		this._legacyTextMeshReference = null;
		this._tmpTextReference = objRef.GetComponent<TMP_Text>();
		if (this._tmpTextReference != null)
		{
			return;
		}
		this._legacyTextReference = objRef.GetComponent<Text>();
		if (this._legacyTextReference)
		{
			return;
		}
		this._legacyTextMeshReference = objRef.GetComponent<TextMesh>();
		if (this._legacyTextMeshReference)
		{
			return;
		}
		Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Could not find either a [TMP_Text], Legacy-[Text], or Legacy-[TextMesh] component on object [" + objRef.name + "]", this._objectReference);
	}

	// Token: 0x17000728 RID: 1832
	// (get) Token: 0x06004C09 RID: 19465 RVA: 0x00196691 File Offset: 0x00194891
	public bool IsValid
	{
		get
		{
			return this._tmpTextReference || this._legacyTextReference || this._legacyTextMeshReference;
		}
	}

	// Token: 0x17000729 RID: 1833
	// (get) Token: 0x06004C0A RID: 19466 RVA: 0x001966BA File Offset: 0x001948BA
	// (set) Token: 0x06004C0B RID: 19467 RVA: 0x001966DA File Offset: 0x001948DA
	public float characterSpacing
	{
		get
		{
			if (this._tmpTextReference)
			{
				return this._tmpTextReference.characterSpacing;
			}
			return 0f;
		}
		set
		{
			if (this._tmpTextReference)
			{
				this._tmpTextReference.characterSpacing = value;
				return;
			}
		}
	}

	// Token: 0x06004C0C RID: 19468 RVA: 0x001966F8 File Offset: 0x001948F8
	public void SetFont(TMP_FontAsset font, Font legacyFont)
	{
		if (font != null && this._tmpTextReference)
		{
			this.SetFont(font);
			return;
		}
		if (legacyFont != null && (this._legacyTextReference || this._legacyTextMeshReference))
		{
			this.SetFont(legacyFont);
			return;
		}
		if (!this.IsValid)
		{
			Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Trying to change font but both text references are NULL.");
		}
	}

	// Token: 0x06004C0D RID: 19469 RVA: 0x00196760 File Offset: 0x00194960
	public void SetFont(Font font)
	{
		if (this._legacyTextReference)
		{
			this._legacyTextReference.font = font;
			return;
		}
		if (this._legacyTextMeshReference)
		{
			this._legacyTextMeshReference.font = font;
			return;
		}
		Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Trying to change font for non-legacy reference but passed in a legacy font.", font);
	}

	// Token: 0x06004C0E RID: 19470 RVA: 0x001967AC File Offset: 0x001949AC
	public void SetFont(TMP_FontAsset font)
	{
		if (this._tmpTextReference == null)
		{
			return;
		}
		this._tmpTextReference.font = font;
	}

	// Token: 0x06004C0F RID: 19471 RVA: 0x001967CC File Offset: 0x001949CC
	public void SetFontSize(float fontSize)
	{
		if (!this._tmpTextReference)
		{
			return;
		}
		TMP_Text tmpTextReference = this._tmpTextReference;
		this._tmpTextReference.fontSizeMax = fontSize;
		tmpTextReference.fontSize = fontSize;
	}

	// Token: 0x1700072A RID: 1834
	// (get) Token: 0x06004C10 RID: 19472 RVA: 0x00196804 File Offset: 0x00194A04
	// (set) Token: 0x06004C11 RID: 19473 RVA: 0x0019686C File Offset: 0x00194A6C
	public string text
	{
		get
		{
			if (this._tmpTextReference)
			{
				return this._tmpTextReference.text;
			}
			if (this._legacyTextReference)
			{
				return this._legacyTextReference.text;
			}
			if (this._legacyTextMeshReference)
			{
				return this._legacyTextMeshReference.text;
			}
			Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Both Legacy Text ref and TMP text ref are null!");
			return "";
		}
		set
		{
			if (this._tmpTextReference != null)
			{
				this._tmpTextReference.text = value;
				return;
			}
			if (this._legacyTextReference != null)
			{
				this._legacyTextReference.text = value;
				return;
			}
			if (this._legacyTextMeshReference)
			{
				this._legacyTextMeshReference.text = value;
				return;
			}
			Debug.LogError("[LOCALIZATION::TEXT_COMPONENT_LEGACY_SUPPORT_STORE] Both Legacy Text ref and TMP text ref are null and cannot be set!", this._objectReference);
		}
	}

	// Token: 0x06004C12 RID: 19474 RVA: 0x001968D9 File Offset: 0x00194AD9
	public void SetText(string newText)
	{
		this.text = newText;
	}

	// Token: 0x06004C13 RID: 19475 RVA: 0x001968E2 File Offset: 0x00194AE2
	public void SetCharSpacing(float spacing)
	{
		this.characterSpacing = spacing;
	}

	// Token: 0x04005F4E RID: 24398
	private Transform _objectReference;

	// Token: 0x04005F4F RID: 24399
	private TMP_Text _tmpTextReference;

	// Token: 0x04005F50 RID: 24400
	private Text _legacyTextReference;

	// Token: 0x04005F51 RID: 24401
	private TextMesh _legacyTextMeshReference;
}
