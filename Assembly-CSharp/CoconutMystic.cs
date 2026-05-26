using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x0200051A RID: 1306
public class CoconutMystic : MonoBehaviour
{
	// Token: 0x060020B2 RID: 8370 RVA: 0x000AF378 File Offset: 0x000AD578
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x000AF386 File Offset: 0x000AD586
	private void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += this.OnPhotonEvent;
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x000AF39E File Offset: 0x000AD59E
	private void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= this.OnPhotonEvent;
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x000AF3B8 File Offset: 0x000AD5B8
	private void OnPhotonEvent(EventData evData)
	{
		if (evData.Code != 176)
		{
			return;
		}
		object[] array = (object[])evData.CustomData;
		object obj = array[0];
		if (!(obj is int))
		{
			return;
		}
		int num = (int)obj;
		if (num != CoconutMystic.kUpdateLabelEvent)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(evData.Sender);
		NetPlayer owningNetPlayer = this.rig.OwningNetPlayer;
		if (player != owningNetPlayer)
		{
			return;
		}
		int index = (int)array[1];
		this.label.text = this.answers.GetItem(index).GetLocalizedString();
		this.soundPlayer.Play();
		this.breakEffect.Play();
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x000AF45C File Offset: 0x000AD65C
	public void UpdateLabel()
	{
		bool flag = this.geodeItem.currentState == TransferrableObject.PositionState.InLeftHand;
		this.label.rectTransform.localRotation = Quaternion.Euler(0f, flag ? 270f : 90f, 0f);
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000AF4A8 File Offset: 0x000AD6A8
	public void ShowAnswer()
	{
		this.answers.distinct = this.distinct;
		this.label.text = this.answers.NextItem().GetLocalizedString();
		this.soundPlayer.Play();
		this.breakEffect.Play();
		object eventContent = new object[]
		{
			CoconutMystic.kUpdateLabelEvent,
			this.answers.lastItemIndex
		};
		PhotonNetwork.RaiseEvent(176, eventContent, RaiseEventOptions.Default, SendOptions.SendReliable);
	}

	// Token: 0x04002B62 RID: 11106
	public VRRig rig;

	// Token: 0x04002B63 RID: 11107
	public GeodeItem geodeItem;

	// Token: 0x04002B64 RID: 11108
	public SoundBankPlayer soundPlayer;

	// Token: 0x04002B65 RID: 11109
	public ParticleSystem breakEffect;

	// Token: 0x04002B66 RID: 11110
	public RandomLocalizedStrings answers;

	// Token: 0x04002B67 RID: 11111
	public TMP_Text label;

	// Token: 0x04002B68 RID: 11112
	public bool distinct;

	// Token: 0x04002B69 RID: 11113
	private static readonly int kUpdateLabelEvent = "CoconutMystic.UpdateLabel".GetStaticHash();
}
