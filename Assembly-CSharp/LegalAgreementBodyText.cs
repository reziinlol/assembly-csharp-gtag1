using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000BC3 RID: 3011
public class LegalAgreementBodyText : MonoBehaviour
{
	// Token: 0x06004B73 RID: 19315 RVA: 0x0019358E File Offset: 0x0019178E
	private void Awake()
	{
		this.textCollection.Add(this.textBox);
	}

	// Token: 0x06004B74 RID: 19316 RVA: 0x001935A4 File Offset: 0x001917A4
	public void SetText(string text)
	{
		text = Regex.Unescape(text);
		string[] array = text.Split(new string[]
		{
			Environment.NewLine,
			"\\r\\n",
			"\n"
		}, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			Text text2;
			if (i >= this.textCollection.Count)
			{
				text2 = Object.Instantiate<Text>(this.textBox, base.transform);
				this.textCollection.Add(text2);
			}
			else
			{
				text2 = this.textCollection[i];
			}
			text2.text = array[i];
		}
	}

	// Token: 0x06004B75 RID: 19317 RVA: 0x00193634 File Offset: 0x00191834
	public void ClearText()
	{
		foreach (Text text in this.textCollection)
		{
			text.text = string.Empty;
		}
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x06004B76 RID: 19318 RVA: 0x00193690 File Offset: 0x00191890
	public Task<bool> UpdateTextFromPlayFabTitleData(string key, string version)
	{
		LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10 <UpdateTextFromPlayFabTitleData>d__;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromPlayFabTitleData>d__.<>4__this = this;
		<UpdateTextFromPlayFabTitleData>d__.key = key;
		<UpdateTextFromPlayFabTitleData>d__.version = version;
		<UpdateTextFromPlayFabTitleData>d__.<>1__state = -1;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder.Start<LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10>(ref <UpdateTextFromPlayFabTitleData>d__);
		return <UpdateTextFromPlayFabTitleData>d__.<>t__builder.Task;
	}

	// Token: 0x06004B77 RID: 19319 RVA: 0x001936E3 File Offset: 0x001918E3
	private void OnPlayFabError(PlayFabError obj)
	{
		Debug.LogError("ERROR: " + obj.ErrorMessage);
		this.state = LegalAgreementBodyText.State.Error;
	}

	// Token: 0x06004B78 RID: 19320 RVA: 0x00193701 File Offset: 0x00191901
	private void OnTitleDataReceived(string text)
	{
		this.cachedText = text;
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x17000719 RID: 1817
	// (get) Token: 0x06004B79 RID: 19321 RVA: 0x00193714 File Offset: 0x00191914
	public float Height
	{
		get
		{
			return this.rectTransform.rect.height;
		}
	}

	// Token: 0x04005E7F RID: 24191
	[SerializeField]
	private Text textBox;

	// Token: 0x04005E80 RID: 24192
	[SerializeField]
	private TextAsset textAsset;

	// Token: 0x04005E81 RID: 24193
	[SerializeField]
	private RectTransform rectTransform;

	// Token: 0x04005E82 RID: 24194
	private List<Text> textCollection = new List<Text>();

	// Token: 0x04005E83 RID: 24195
	private string cachedText;

	// Token: 0x04005E84 RID: 24196
	private LegalAgreementBodyText.State state;

	// Token: 0x02000BC4 RID: 3012
	private enum State
	{
		// Token: 0x04005E86 RID: 24198
		Ready,
		// Token: 0x04005E87 RID: 24199
		Loading,
		// Token: 0x04005E88 RID: 24200
		Error
	}
}
