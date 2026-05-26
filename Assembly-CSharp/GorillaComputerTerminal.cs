using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A00 RID: 2560
public class GorillaComputerTerminal : MonoBehaviour, IBuildValidation
{
	// Token: 0x06004170 RID: 16752 RVA: 0x0015E4C0 File Offset: 0x0015C6C0
	public bool BuildValidationCheck()
	{
		if (this.myScreenText == null || this.myFunctionText == null || this.monitorMesh == null)
		{
			Debug.LogErrorFormat(base.gameObject, "gorilla computer terminal {0} is missing screen text, function text, or monitor mesh. this will break lots of computer stuff", new object[]
			{
				base.gameObject.name
			});
			return false;
		}
		return true;
	}

	// Token: 0x06004171 RID: 16753 RVA: 0x0015E51E File Offset: 0x0015C71E
	private void OnEnable()
	{
		if (GorillaComputer.instance == null)
		{
			base.StartCoroutine(this.<OnEnable>g__OnEnable_Local|4_0());
			return;
		}
		this.Init();
	}

	// Token: 0x06004172 RID: 16754 RVA: 0x0015E544 File Offset: 0x0015C744
	private void Init()
	{
		GameEvents.ScreenTextChangedEvent.AddListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.AddListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.AddListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
		GameEvents.LanguageEvent.AddListener(new UnityAction(this.OnLanguageChanged));
		this.myScreenText.text = GorillaComputer.instance.screenText.currentText;
		this.myFunctionText.text = GorillaComputer.instance.functionSelectText.currentText;
		if (GorillaComputer.instance.screenText.currentMaterials != null)
		{
			this.monitorMesh.sharedMaterials = GorillaComputer.instance.screenText.currentMaterials;
		}
	}

	// Token: 0x06004173 RID: 16755 RVA: 0x0015E610 File Offset: 0x0015C810
	private void OnDisable()
	{
		GameEvents.ScreenTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.RemoveListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
	}

	// Token: 0x06004174 RID: 16756 RVA: 0x0015E65F File Offset: 0x0015C85F
	public void OnScreenTextChanged(string text)
	{
		this.myScreenText.text = text;
	}

	// Token: 0x06004175 RID: 16757 RVA: 0x0015E66D File Offset: 0x0015C86D
	public void OnFunctionTextChanged(string text)
	{
		this.myFunctionText.text = text;
	}

	// Token: 0x06004176 RID: 16758 RVA: 0x0015E67B File Offset: 0x0015C87B
	private void OnMaterialsChanged(Material[] materials)
	{
		this.monitorMesh.sharedMaterials = materials;
	}

	// Token: 0x06004177 RID: 16759 RVA: 0x0015E68C File Offset: 0x0015C88C
	private void OnLanguageChanged()
	{
		LocalisationFontPair localisationFontPair;
		if (LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair))
		{
			this.myScreenText.font = localisationFontPair.fontAsset;
			this.myFunctionText.font = localisationFontPair.fontAsset;
		}
		this.myScreenText.characterSpacing = localisationFontPair.charSpacing;
	}

	// Token: 0x06004179 RID: 16761 RVA: 0x0015E6D5 File Offset: 0x0015C8D5
	[CompilerGenerated]
	private IEnumerator <OnEnable>g__OnEnable_Local|4_0()
	{
		yield return new WaitUntil(() => GorillaComputer.instance != null);
		yield return null;
		this.Init();
		yield break;
	}

	// Token: 0x04005316 RID: 21270
	public TextMeshPro myScreenText;

	// Token: 0x04005317 RID: 21271
	public TextMeshPro myFunctionText;

	// Token: 0x04005318 RID: 21272
	public MeshRenderer monitorMesh;
}
