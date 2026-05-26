using System;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class DoorSlidingOpenAudio : MonoBehaviour, IBuildValidation, ITickSystemTick
{
	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x0004129A File Offset: 0x0003F49A
	// (set) Token: 0x06000BF9 RID: 3065 RVA: 0x000412A2 File Offset: 0x0003F4A2
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06000BFA RID: 3066 RVA: 0x000412AB File Offset: 0x0003F4AB
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x000412B3 File Offset: 0x0003F4B3
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x000412BC File Offset: 0x0003F4BC
	public bool BuildValidationCheck()
	{
		if (this.button == null)
		{
			Debug.LogError("reference button missing for doorslidingopenaudio", base.gameObject);
			return false;
		}
		if (this.audioSource == null)
		{
			Debug.LogError("missing audio source on doorslidingopenaudio", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x0004130C File Offset: 0x0003F50C
	void ITickSystemTick.Tick()
	{
		if (this.button.ghostLab.IsDoorMoving(this.button.forSingleDoor, this.button.buttonIndex))
		{
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.time = 0f;
				this.audioSource.GTPlay();
				return;
			}
		}
		else if (this.audioSource.isPlaying)
		{
			this.audioSource.time = 0f;
			this.audioSource.GTStop();
		}
	}

	// Token: 0x04000E9A RID: 3738
	public GhostLabButton button;

	// Token: 0x04000E9B RID: 3739
	public AudioSource audioSource;
}
