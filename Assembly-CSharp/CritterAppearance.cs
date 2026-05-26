using System;
using GorillaExtensions;
using Photon.Pun;

// Token: 0x0200007F RID: 127
public struct CritterAppearance
{
	// Token: 0x0600031F RID: 799 RVA: 0x00013037 File Offset: 0x00011237
	public CritterAppearance(string hatName, float size = 1f)
	{
		this.hatName = hatName;
		this.size = size;
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00013048 File Offset: 0x00011248
	public object[] WriteToRPCData()
	{
		object[] array = new object[]
		{
			this.hatName,
			this.size
		};
		if (this.hatName == null)
		{
			array[0] = string.Empty;
		}
		if (this.size != 0f)
		{
			array[1] = this.size;
		}
		return array;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x0001309F File Offset: 0x0001129F
	public static int DataLength()
	{
		return 2;
	}

	// Token: 0x06000322 RID: 802 RVA: 0x000130A4 File Offset: 0x000112A4
	public static bool ValidateData(object[] data)
	{
		float num;
		return data != null && data.Length == CritterAppearance.DataLength() && CrittersManager.ValidateDataType<float>(data[1], out num) && num >= 0f && !float.IsNaN(num) && !float.IsInfinity(num);
	}

	// Token: 0x06000323 RID: 803 RVA: 0x000130EC File Offset: 0x000112EC
	public static CritterAppearance ReadFromRPCData(object[] data)
	{
		string text;
		if (!CrittersManager.ValidateDataType<string>(data[0], out text))
		{
			return new CritterAppearance(string.Empty, 1f);
		}
		float value;
		if (!CrittersManager.ValidateDataType<float>(data[1], out value))
		{
			return new CritterAppearance(string.Empty, 1f);
		}
		return new CritterAppearance((string)data[0], value.GetFinite());
	}

	// Token: 0x06000324 RID: 804 RVA: 0x00013144 File Offset: 0x00011344
	public static CritterAppearance ReadFromPhotonStream(PhotonStream data)
	{
		string text = (string)data.ReceiveNext();
		float num = (float)data.ReceiveNext();
		return new CritterAppearance(text, num);
	}

	// Token: 0x06000325 RID: 805 RVA: 0x0001316E File Offset: 0x0001136E
	public override string ToString()
	{
		return string.Format("Size: {0} Hat: {1}", this.size, this.hatName);
	}

	// Token: 0x040003B9 RID: 953
	public float size;

	// Token: 0x040003BA RID: 954
	public string hatName;
}
