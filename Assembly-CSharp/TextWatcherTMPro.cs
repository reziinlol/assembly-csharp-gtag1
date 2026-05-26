using System;
using TMPro;
using UnityEngine;

// Token: 0x020009ED RID: 2541
public class TextWatcherTMPro : MonoBehaviour
{
	// Token: 0x0600410A RID: 16650 RVA: 0x0015BB32 File Offset: 0x00159D32
	private void Start()
	{
		this.myText = base.GetComponent<TextMeshPro>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x0600410B RID: 16651 RVA: 0x0015BB58 File Offset: 0x00159D58
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x0600410C RID: 16652 RVA: 0x0015BB71 File Offset: 0x00159D71
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x040051B7 RID: 20919
	public WatchableStringSO textToCopy;

	// Token: 0x040051B8 RID: 20920
	private TextMeshPro myText;
}
