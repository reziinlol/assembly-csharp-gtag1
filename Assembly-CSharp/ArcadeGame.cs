using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200041D RID: 1053
public abstract class ArcadeGame : MonoBehaviour
{
	// Token: 0x060018FB RID: 6395 RVA: 0x0008D503 File Offset: 0x0008B703
	protected virtual void Awake()
	{
		this.InitializeMemoryStreams();
	}

	// Token: 0x060018FC RID: 6396 RVA: 0x0008D50B File Offset: 0x0008B70B
	public void InitializeMemoryStreams()
	{
		if (!this.memoryStreamsInitialized)
		{
			this.netStateMemStream = new MemoryStream(this.netStateBuffer, true);
			this.netStateMemStreamAlt = new MemoryStream(this.netStateBufferAlt, true);
			this.memoryStreamsInitialized = true;
		}
	}

	// Token: 0x060018FD RID: 6397 RVA: 0x0008D540 File Offset: 0x0008B740
	public void SetMachine(ArcadeMachine machine)
	{
		this.machine = machine;
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x0008D549 File Offset: 0x0008B749
	protected bool getButtonState(int player, ArcadeButtons button)
	{
		return this.playerInputs[player].HasFlag(button);
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x0008D564 File Offset: 0x0008B764
	public void OnInputStateChange(int player, ArcadeButtons buttons)
	{
		for (int i = 1; i < 256; i += i)
		{
			ArcadeButtons arcadeButtons = (ArcadeButtons)i;
			bool flag = buttons.HasFlag(arcadeButtons);
			bool flag2 = this.playerInputs[player].HasFlag(arcadeButtons);
			if (flag != flag2)
			{
				if (flag)
				{
					this.ButtonDown(player, arcadeButtons);
				}
				else
				{
					this.ButtonUp(player, arcadeButtons);
				}
			}
		}
		this.playerInputs[player] = buttons;
	}

	// Token: 0x06001900 RID: 6400
	public abstract byte[] GetNetworkState();

	// Token: 0x06001901 RID: 6401
	public abstract void SetNetworkState(byte[] obj);

	// Token: 0x06001902 RID: 6402 RVA: 0x0008D5D0 File Offset: 0x0008B7D0
	protected static void WrapNetState(object ns, MemoryStream stream)
	{
		if (stream == null)
		{
			Debug.LogWarning("Null MemoryStream passed to WrapNetState");
			return;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		stream.SetLength(0L);
		stream.Position = 0L;
		binaryFormatter.Serialize(stream, ns);
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x0008D5FC File Offset: 0x0008B7FC
	protected static object UnwrapNetState(byte[] b)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(b);
		memoryStream.Position = 0L;
		object result = binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		return result;
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x0008D634 File Offset: 0x0008B834
	protected void SwapNetStateBuffersAndStreams()
	{
		byte[] array = this.netStateBufferAlt;
		byte[] array2 = this.netStateBuffer;
		this.netStateBuffer = array;
		this.netStateBufferAlt = array2;
		MemoryStream memoryStream = this.netStateMemStreamAlt;
		MemoryStream memoryStream2 = this.netStateMemStream;
		this.netStateMemStream = memoryStream;
		this.netStateMemStreamAlt = memoryStream2;
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x0008D679 File Offset: 0x0008B879
	protected void PlaySound(int clipId, int prio = 3)
	{
		this.machine.PlaySound(clipId, prio);
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x0008D688 File Offset: 0x0008B888
	protected bool IsPlayerLocallyControlled(int player)
	{
		return this.machine.IsPlayerLocallyControlled(player);
	}

	// Token: 0x06001907 RID: 6407
	protected abstract void ButtonUp(int player, ArcadeButtons button);

	// Token: 0x06001908 RID: 6408
	protected abstract void ButtonDown(int player, ArcadeButtons button);

	// Token: 0x06001909 RID: 6409
	public abstract void OnTimeout();

	// Token: 0x0600190A RID: 6410 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x04002419 RID: 9241
	[SerializeField]
	public Vector2 Scale = new Vector2(1f, 1f);

	// Token: 0x0400241A RID: 9242
	private ArcadeButtons[] playerInputs = new ArcadeButtons[4];

	// Token: 0x0400241B RID: 9243
	public AudioClip[] audioClips;

	// Token: 0x0400241C RID: 9244
	private ArcadeMachine machine;

	// Token: 0x0400241D RID: 9245
	protected static int NetStateBufferSize = 512;

	// Token: 0x0400241E RID: 9246
	protected byte[] netStateBuffer = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x0400241F RID: 9247
	protected byte[] netStateBufferAlt = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x04002420 RID: 9248
	protected MemoryStream netStateMemStream;

	// Token: 0x04002421 RID: 9249
	protected MemoryStream netStateMemStreamAlt;

	// Token: 0x04002422 RID: 9250
	public bool memoryStreamsInitialized;
}
