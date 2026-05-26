using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000CCB RID: 3275
[Serializable]
public class CallLimiter
{
	// Token: 0x06005161 RID: 20833 RVA: 0x00002050 File Offset: 0x00000250
	public CallLimiter()
	{
	}

	// Token: 0x06005162 RID: 20834 RVA: 0x001AD2EC File Offset: 0x001AB4EC
	public CallLimiter(int historyLength, float coolDown, float latencyMax = 0.5f)
	{
		this.callTimeHistory = new float[historyLength];
		this.callHistoryLength = historyLength;
		for (int i = 0; i < historyLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.timeCooldown = coolDown;
		this.maxLatency = (double)latencyMax;
	}

	// Token: 0x06005163 RID: 20835 RVA: 0x001AD33C File Offset: 0x001AB53C
	public bool CheckCallServerTime(double time)
	{
		double currentTime = PhotonNetwork.CurrentTime;
		double num = this.maxLatency;
		double num2 = 4294967.295 - this.maxLatency;
		double num3;
		if (currentTime > num || time < num)
		{
			if (time > currentTime + 0.05)
			{
				return false;
			}
			num3 = currentTime - time;
		}
		else
		{
			double num4 = num2 + currentTime;
			if (time > currentTime + 0.5 && time < num4)
			{
				return false;
			}
			num3 = currentTime + (4294967.295 - time);
		}
		if (num3 > this.maxLatency)
		{
			return false;
		}
		int num5 = (this.oldTimeIndex > 0) ? (this.oldTimeIndex - 1) : (this.callHistoryLength - 1);
		double num6 = (double)this.callTimeHistory[num5];
		if (num6 > num2 && time < num6)
		{
			this.Reset();
		}
		else if (time < num6)
		{
			return false;
		}
		return this.CheckCallTime((float)time);
	}

	// Token: 0x06005164 RID: 20836 RVA: 0x001AD404 File Offset: 0x001AB604
	public virtual bool CheckCallTime(float time)
	{
		if (this.callTimeHistory[this.oldTimeIndex] > time)
		{
			this.blockCall = true;
			this.blockStartTime = time;
			return false;
		}
		this.callTimeHistory[this.oldTimeIndex] = time + this.timeCooldown;
		int num = this.oldTimeIndex + 1;
		this.oldTimeIndex = num;
		this.oldTimeIndex = num % this.callHistoryLength;
		this.blockCall = false;
		return true;
	}

	// Token: 0x06005165 RID: 20837 RVA: 0x001AD46C File Offset: 0x001AB66C
	public virtual void Reset()
	{
		if (this.callTimeHistory == null)
		{
			return;
		}
		for (int i = 0; i < this.callHistoryLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.oldTimeIndex = 0;
		this.blockStartTime = 0f;
		this.blockCall = false;
	}

	// Token: 0x040062B7 RID: 25271
	protected const double k_serverMaxTime = 4294967.295;

	// Token: 0x040062B8 RID: 25272
	[SerializeField]
	protected float[] callTimeHistory;

	// Token: 0x040062B9 RID: 25273
	[Space]
	[SerializeField]
	protected int callHistoryLength;

	// Token: 0x040062BA RID: 25274
	[SerializeField]
	protected float timeCooldown;

	// Token: 0x040062BB RID: 25275
	[SerializeField]
	protected double maxLatency;

	// Token: 0x040062BC RID: 25276
	private int oldTimeIndex;

	// Token: 0x040062BD RID: 25277
	protected bool blockCall;

	// Token: 0x040062BE RID: 25278
	protected float blockStartTime;
}
