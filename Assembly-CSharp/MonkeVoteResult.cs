using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000244 RID: 580
public class MonkeVoteResult : MonoBehaviour
{
	// Token: 0x17000187 RID: 391
	// (get) Token: 0x06000F88 RID: 3976 RVA: 0x00054681 File Offset: 0x00052881
	// (set) Token: 0x06000F89 RID: 3977 RVA: 0x0005468C File Offset: 0x0005288C
	public string Text
	{
		get
		{
			return this._text;
		}
		set
		{
			TMP_Text optionText = this._optionText;
			this._text = value;
			optionText.text = value;
		}
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x000546B0 File Offset: 0x000528B0
	public void ShowResult(string questionOption, int percentage, bool showVote, bool showPrediction, bool isWinner)
	{
		this._optionText.text = questionOption;
		this._optionIndicator.SetActive(true);
		this._scoreText.text = ((percentage >= 0) ? string.Format("{0}%", percentage) : "--");
		this._voteIndicator.SetActive(showVote);
		this._guessWinIndicator.SetActive(showPrediction && isWinner);
		this._guessLoseIndicator.SetActive(showPrediction && !isWinner);
		this._youWinIndicator.SetActive(isWinner && showPrediction);
		this._mostPopularIndicator.SetActive(isWinner);
		this.ShowRockPile(percentage);
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x00054754 File Offset: 0x00052954
	public void HideResult()
	{
		this._optionIndicator.SetActive(false);
		this._voteIndicator.SetActive(false);
		this._guessWinIndicator.SetActive(false);
		this._guessLoseIndicator.SetActive(false);
		this._youWinIndicator.SetActive(false);
		this._mostPopularIndicator.SetActive(false);
		this.ShowRockPile(0);
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x000547B0 File Offset: 0x000529B0
	private void ShowRockPile(int percentage)
	{
		this._rockPiles.Show(percentage);
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x000547C0 File Offset: 0x000529C0
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._mostPopularIndicator.SetActive(visible);
		this._voteIndicator.SetActive(visible);
		this._guessWinIndicator.SetActive(visible);
		this._guessLoseIndicator.SetActive(visible);
		this._rockPiles.Show(visible ? 100 : -1);
	}

	// Token: 0x040012B5 RID: 4789
	[SerializeField]
	private GameObject _optionIndicator;

	// Token: 0x040012B6 RID: 4790
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x040012B7 RID: 4791
	[FormerlySerializedAs("_scoreLabelPost")]
	[SerializeField]
	private GameObject _scoreIndicator;

	// Token: 0x040012B8 RID: 4792
	[SerializeField]
	private TMP_Text _scoreText;

	// Token: 0x040012B9 RID: 4793
	[SerializeField]
	private GameObject _voteIndicator;

	// Token: 0x040012BA RID: 4794
	[SerializeField]
	private GameObject _guessWinIndicator;

	// Token: 0x040012BB RID: 4795
	[SerializeField]
	private GameObject _guessLoseIndicator;

	// Token: 0x040012BC RID: 4796
	[SerializeField]
	private GameObject _mostPopularIndicator;

	// Token: 0x040012BD RID: 4797
	[SerializeField]
	private GameObject _youWinIndicator;

	// Token: 0x040012BE RID: 4798
	[SerializeField]
	private RockPiles _rockPiles;

	// Token: 0x040012BF RID: 4799
	private MonkeVoteMachine _machine;

	// Token: 0x040012C0 RID: 4800
	private string _text = string.Empty;

	// Token: 0x040012C1 RID: 4801
	private bool _canVote;

	// Token: 0x040012C2 RID: 4802
	private float _rockPileHeight;
}
