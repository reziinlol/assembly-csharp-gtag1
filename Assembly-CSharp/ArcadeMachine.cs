using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200041E RID: 1054
[NetworkBehaviourWeaved(128)]
public class ArcadeMachine : NetworkComponent
{
	// Token: 0x0600190E RID: 6414 RVA: 0x0008D6F8 File Offset: 0x0008B8F8
	protected override void Awake()
	{
		base.Awake();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x0008D70C File Offset: 0x0008B90C
	protected override void Start()
	{
		base.Start();
		if (this.arcadeGame != null && this.arcadeGame.Scale.x > 0f && this.arcadeGame.Scale.y > 0f)
		{
			this.arcadeGameInstance = UnityEngine.Object.Instantiate<ArcadeGame>(this.arcadeGame, this.screen.transform);
			this.arcadeGameInstance.transform.localScale = new Vector3(1f / this.arcadeGameInstance.Scale.x, 1f / this.arcadeGameInstance.Scale.y, 1f);
			this.screen.forceRenderingOff = true;
			this.arcadeGameInstance.SetMachine(this);
		}
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x0008D7DC File Offset: 0x0008B9DC
	public void PlaySound(int soundId, int priority)
	{
		if (!this.audioSource.isPlaying || this.audioSourcePriority >= priority)
		{
			this.audioSource.GTStop();
			this.audioSourcePriority = priority;
			this.audioSource.clip = this.arcadeGameInstance.audioClips[soundId];
			this.audioSource.GTPlay();
			if (this.networkSynchronized && base.IsMine)
			{
				base.GetView.RPC("ArcadeGameInstance_OnPlaySound_RPC", RpcTarget.Others, new object[]
				{
					soundId
				});
			}
		}
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x0008D864 File Offset: 0x0008BA64
	public bool IsPlayerLocallyControlled(int player)
	{
		return this.sticks[player].heldByLocalPlayer;
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x0008D874 File Offset: 0x0008BA74
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		for (int i = 0; i < this.sticks.Length; i++)
		{
			this.sticks[i].Init(this, i);
		}
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x0008D8AF File Offset: 0x0008BAAF
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x0008D8C0 File Offset: 0x0008BAC0
	[PunRPC]
	private void ArcadeGameInstance_OnPlaySound_RPC(int id, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || id > this.arcadeGameInstance.audioClips.Length || id < 0 || !this.soundCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.audioSource.GTStop();
		this.audioSource.clip = this.arcadeGameInstance.audioClips[id];
		this.audioSource.GTPlay();
	}

	// Token: 0x06001915 RID: 6421 RVA: 0x0008D92F File Offset: 0x0008BB2F
	public void OnJoystickStateChange(int player, ArcadeButtons buttons)
	{
		if (this.arcadeGameInstance != null)
		{
			this.arcadeGameInstance.OnInputStateChange(player, buttons);
		}
	}

	// Token: 0x06001916 RID: 6422 RVA: 0x0008D948 File Offset: 0x0008BB48
	public bool IsControllerInUse(int player)
	{
		if (base.IsMine)
		{
			return this.playersPerJoystick[player] != null && Time.time < this.playerIdleTimeouts[player];
		}
		return (this.buttonsStateValue & 1 << player * 8) != 0;
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06001917 RID: 6423 RVA: 0x0008D980 File Offset: 0x0008BB80
	[Networked]
	[Capacity(128)]
	[NetworkedWeaved(0, 128)]
	[NetworkedWeavedArray(128, 1, typeof(ElementReaderWriterByte))]
	public unsafe NetworkArray<byte> Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ArcadeMachine.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<byte>((byte*)(this.Ptr + 0), 128, ElementReaderWriterByte.GetInstance());
		}
	}

	// Token: 0x06001918 RID: 6424 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06001919 RID: 6425 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x0600191A RID: 6426 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600191B RID: 6427 RVA: 0x0008D9C0 File Offset: 0x0008BBC0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600191C RID: 6428 RVA: 0x0008D9CD File Offset: 0x0008BBCD
	public void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.ReadPlayerDataPUN(player, stream, info);
	}

	// Token: 0x0600191D RID: 6429 RVA: 0x0008D9DD File Offset: 0x0008BBDD
	public void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.WritePlayerDataPUN(player, stream, info);
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x0008DA14 File Offset: 0x0008BC14
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		NetworkBehaviourUtils.InitializeNetworkArray<byte>(this.Data, this._Data, "Data");
	}

	// Token: 0x06001920 RID: 6432 RVA: 0x0008DA36 File Offset: 0x0008BC36
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		NetworkBehaviourUtils.CopyFromNetworkArray<byte>(this.Data, ref this._Data);
	}

	// Token: 0x04002423 RID: 9251
	[SerializeField]
	private ArcadeGame arcadeGame;

	// Token: 0x04002424 RID: 9252
	[SerializeField]
	private ArcadeMachineJoystick[] sticks;

	// Token: 0x04002425 RID: 9253
	[SerializeField]
	private Renderer screen;

	// Token: 0x04002426 RID: 9254
	[SerializeField]
	private bool networkSynchronized = true;

	// Token: 0x04002427 RID: 9255
	[SerializeField]
	private CallLimiter soundCallLimit;

	// Token: 0x04002428 RID: 9256
	private int buttonsStateValue;

	// Token: 0x04002429 RID: 9257
	private AudioSource audioSource;

	// Token: 0x0400242A RID: 9258
	private int audioSourcePriority;

	// Token: 0x0400242B RID: 9259
	private ArcadeGame arcadeGameInstance;

	// Token: 0x0400242C RID: 9260
	private Player[] playersPerJoystick = new Player[4];

	// Token: 0x0400242D RID: 9261
	private float[] playerIdleTimeouts = new float[4];

	// Token: 0x0400242E RID: 9262
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 128)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private byte[] _Data;
}
