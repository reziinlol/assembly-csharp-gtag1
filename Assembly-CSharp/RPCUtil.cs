using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200083F RID: 2111
internal class RPCUtil
{
	// Token: 0x0600366E RID: 13934 RVA: 0x0012C030 File Offset: 0x0012A230
	public static bool NotSpam(string id, PhotonMessageInfoWrapped info, float delay)
	{
		RPCUtil.RPCCallID key = new RPCUtil.RPCCallID(id, info.senderID);
		if (!RPCUtil.RPCCallLog.ContainsKey(key))
		{
			RPCUtil.RPCCallLog.Add(key, Time.time);
			return true;
		}
		if (Time.time - RPCUtil.RPCCallLog[key] > delay)
		{
			RPCUtil.RPCCallLog[key] = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x0600366F RID: 13935 RVA: 0x0012C091 File Offset: 0x0012A291
	public static bool SafeValue(float v)
	{
		return !float.IsNaN(v) && float.IsFinite(v);
	}

	// Token: 0x06003670 RID: 13936 RVA: 0x0012C0A3 File Offset: 0x0012A2A3
	public static bool SafeValue(float v, float min, float max)
	{
		return RPCUtil.SafeValue(v) && v <= max && v >= min;
	}

	// Token: 0x040046D2 RID: 18130
	private static Dictionary<RPCUtil.RPCCallID, float> RPCCallLog = new Dictionary<RPCUtil.RPCCallID, float>();

	// Token: 0x02000840 RID: 2112
	private struct RPCCallID : IEquatable<RPCUtil.RPCCallID>
	{
		// Token: 0x06003673 RID: 13939 RVA: 0x0012C0C8 File Offset: 0x0012A2C8
		public RPCCallID(string nameOfFunction, int senderId)
		{
			this._senderID = senderId;
			this._nameOfFunction = nameOfFunction;
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06003674 RID: 13940 RVA: 0x0012C0D8 File Offset: 0x0012A2D8
		public readonly int SenderID
		{
			get
			{
				return this._senderID;
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06003675 RID: 13941 RVA: 0x0012C0E0 File Offset: 0x0012A2E0
		public readonly string NameOfFunction
		{
			get
			{
				return this._nameOfFunction;
			}
		}

		// Token: 0x06003676 RID: 13942 RVA: 0x0012C0E8 File Offset: 0x0012A2E8
		bool IEquatable<RPCUtil.RPCCallID>.Equals(RPCUtil.RPCCallID other)
		{
			return other.NameOfFunction.Equals(this.NameOfFunction) && other.SenderID.Equals(this.SenderID);
		}

		// Token: 0x040046D3 RID: 18131
		private int _senderID;

		// Token: 0x040046D4 RID: 18132
		private string _nameOfFunction;
	}
}
