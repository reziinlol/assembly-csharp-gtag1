using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000247 RID: 583
public class VotingCard : MonoBehaviour
{
	// Token: 0x06000F92 RID: 3986 RVA: 0x000548BE File Offset: 0x00052ABE
	private void MoveToOffPosition()
	{
		this._card.transform.position = this._offPosition.position;
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x000548DB File Offset: 0x00052ADB
	private void MoveToOnPosition()
	{
		this._card.transform.position = this._onPosition.position;
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x000548F8 File Offset: 0x00052AF8
	public void SetVisible(bool showVote, bool instant)
	{
		if (this._isVisible != showVote)
		{
			base.StopAllCoroutines();
		}
		if (instant)
		{
			this._card.transform.position = (showVote ? this._onPosition.position : this._offPosition.position);
			this._card.SetActive(showVote);
		}
		else if (showVote)
		{
			if (this._isVisible != showVote)
			{
				base.StartCoroutine(this.DoActivate());
			}
		}
		else
		{
			this._card.SetActive(false);
			this._card.transform.position = this._offPosition.position;
		}
		this._isVisible = showVote;
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x00054999 File Offset: 0x00052B99
	private IEnumerator DoActivate()
	{
		Vector3 from = this._offPosition.position;
		Vector3 to = this._onPosition.position;
		this._card.transform.position = from;
		this._card.SetActive(true);
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / this.activationTime;
			this._card.transform.position = Vector3.Lerp(from, to, lerpVal);
			yield return null;
		}
		yield break;
	}

	// Token: 0x040012C6 RID: 4806
	[SerializeField]
	private GameObject _card;

	// Token: 0x040012C7 RID: 4807
	[SerializeField]
	private Transform _offPosition;

	// Token: 0x040012C8 RID: 4808
	[SerializeField]
	private Transform _onPosition;

	// Token: 0x040012C9 RID: 4809
	[SerializeField]
	private float activationTime = 0.5f;

	// Token: 0x040012CA RID: 4810
	private bool _isVisible;
}
