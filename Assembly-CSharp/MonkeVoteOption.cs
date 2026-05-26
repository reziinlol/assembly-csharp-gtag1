using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000242 RID: 578
public class MonkeVoteOption : MonoBehaviour
{
	// Token: 0x14000020 RID: 32
	// (add) Token: 0x06000F71 RID: 3953 RVA: 0x000543B8 File Offset: 0x000525B8
	// (remove) Token: 0x06000F72 RID: 3954 RVA: 0x000543F0 File Offset: 0x000525F0
	public event Action<MonkeVoteOption, Collider> OnVote;

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06000F73 RID: 3955 RVA: 0x00054425 File Offset: 0x00052625
	// (set) Token: 0x06000F74 RID: 3956 RVA: 0x00054430 File Offset: 0x00052630
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

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06000F75 RID: 3957 RVA: 0x00054452 File Offset: 0x00052652
	// (set) Token: 0x06000F76 RID: 3958 RVA: 0x0005445C File Offset: 0x0005265C
	public bool CanVote
	{
		get
		{
			return this._canVote;
		}
		set
		{
			Collider trigger = this._trigger;
			this._canVote = value;
			trigger.enabled = value;
		}
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x0005447E File Offset: 0x0005267E
	private void Reset()
	{
		this.Configure();
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x00054488 File Offset: 0x00052688
	private void Configure()
	{
		foreach (Collider collider in base.GetComponentsInChildren<Collider>())
		{
			if (collider.isTrigger)
			{
				this._trigger = collider;
				break;
			}
		}
		if (!this._optionText)
		{
			this._optionText = base.GetComponentInChildren<TMP_Text>();
		}
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x000544D8 File Offset: 0x000526D8
	private void OnTriggerEnter(Collider other)
	{
		if (!this.IsValidVotingRock(other))
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote(this, other);
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x000544F8 File Offset: 0x000526F8
	private bool IsValidVotingRock(Collider other)
	{
		SlingshotProjectile component = other.GetComponent<SlingshotProjectile>();
		return component && component.projectileOwner.IsLocal;
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x00054521 File Offset: 0x00052721
	public void ResetState()
	{
		this.OnVote = null;
		this.ShowIndicators(false, false, true);
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x00054533 File Offset: 0x00052733
	public void ShowIndicators(bool showVote, bool showPrediction, bool instant = true)
	{
		this._voteIndicator.SetVisible(showVote, instant);
		this._guessIndicator.SetVisible(showPrediction, instant);
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x0005454F File Offset: 0x0005274F
	private void Vote()
	{
		this.SendVote(null);
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x00054558 File Offset: 0x00052758
	private void SendVote(Collider other)
	{
		if (!this._canVote)
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote(this, other);
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x00054575 File Offset: 0x00052775
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._voteIndicator.SetVisible(visible, true);
		this._guessIndicator.SetVisible(visible, true);
	}

	// Token: 0x040012AA RID: 4778
	[SerializeField]
	private Collider _trigger;

	// Token: 0x040012AB RID: 4779
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x040012AC RID: 4780
	[SerializeField]
	private VotingCard _voteIndicator;

	// Token: 0x040012AD RID: 4781
	[FormerlySerializedAs("_predictionIndicator")]
	[SerializeField]
	private VotingCard _guessIndicator;

	// Token: 0x040012AF RID: 4783
	private string _text = string.Empty;

	// Token: 0x040012B0 RID: 4784
	private bool _canVote;
}
