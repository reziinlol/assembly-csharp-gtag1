using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000DA1 RID: 3489
public static class Utils
{
	// Token: 0x06005584 RID: 21892 RVA: 0x001BDE88 File Offset: 0x001BC088
	public static void Disable(this GameObject target)
	{
		if (!target.activeSelf)
		{
			return;
		}
		PooledList<IPreDisable> pooledList = Utils.g_listPool.Take();
		List<IPreDisable> list = pooledList.List;
		target.GetComponents<IPreDisable>(list);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			try
			{
				list[i].PreDisable();
			}
			catch (Exception)
			{
			}
		}
		target.SetActive(false);
		Utils.g_listPool.Return(pooledList);
	}

	// Token: 0x06005585 RID: 21893 RVA: 0x001BDF00 File Offset: 0x001BC100
	public static void AddIfNew<T>(this List<T> list, T item)
	{
		if (!list.Contains(item))
		{
			list.Add(item);
		}
	}

	// Token: 0x06005586 RID: 21894 RVA: 0x001BDF12 File Offset: 0x001BC112
	public static void RemoveIfContains<T>(this List<T> list, T item)
	{
		if (list.Contains(item))
		{
			list.Remove(item);
		}
	}

	// Token: 0x06005587 RID: 21895 RVA: 0x001BDF25 File Offset: 0x001BC125
	public static bool InRoom(this NetPlayer player)
	{
		return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.AllNetPlayers.Contains(player);
	}

	// Token: 0x06005588 RID: 21896 RVA: 0x001BDF48 File Offset: 0x001BC148
	public static bool PlayerInRoom(int actorNumber)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
			for (int i = 0; i < allNetPlayers.Length; i++)
			{
				if (allNetPlayers[i].ActorNumber == actorNumber)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06005589 RID: 21897 RVA: 0x001BDF88 File Offset: 0x001BC188
	public static bool PlayerInRoom(int actorNumer, out Player photonPlayer)
	{
		photonPlayer = null;
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumer, out photonPlayer);
	}

	// Token: 0x0600558A RID: 21898 RVA: 0x001BDFA7 File Offset: 0x001BC1A7
	public static bool PlayerInRoom(int actorNumber, out NetPlayer player)
	{
		if (NetworkSystem.Instance == null)
		{
			player = null;
			return false;
		}
		player = NetworkSystem.Instance.GetPlayer(actorNumber);
		return NetworkSystem.Instance.InRoom && player != null;
	}

	// Token: 0x0600558B RID: 21899 RVA: 0x001BDFDC File Offset: 0x001BC1DC
	public static long PackVector3ToLong(Vector3 vector)
	{
		long num = (long)Mathf.Clamp(Mathf.RoundToInt(vector.x * 1024f) + 1048576, 0, 2097151);
		long num2 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.y * 1024f) + 1048576, 0, 2097151);
		long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.z * 1024f) + 1048576, 0, 2097151);
		return num + (num2 << 21) + (num3 << 42);
	}

	// Token: 0x0600558C RID: 21900 RVA: 0x001BE060 File Offset: 0x001BC260
	public static Vector3 UnpackVector3FromLong(long data)
	{
		float num = (float)(data & 2097151L);
		long num2 = data >> 21 & 2097151L;
		long num3 = data >> 42 & 2097151L;
		return new Vector3((float)((long)num - 1048576L) * 0.0009765625f, (float)(num2 - 1048576L) * 0.0009765625f, (float)(num3 - 1048576L) * 0.0009765625f);
	}

	// Token: 0x0600558D RID: 21901 RVA: 0x001BE0BE File Offset: 0x001BC2BE
	public static bool IsASCIILetterOrDigit(char c)
	{
		return (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z');
	}

	// Token: 0x0600558E RID: 21902 RVA: 0x000028C5 File Offset: 0x00000AC5
	public static void Log(object message)
	{
	}

	// Token: 0x0600558F RID: 21903 RVA: 0x000028C5 File Offset: 0x00000AC5
	public static void Log(object message, Object context)
	{
	}

	// Token: 0x06005590 RID: 21904 RVA: 0x001BE0E8 File Offset: 0x001BC2E8
	public static bool ValidateServerTime(double time, double maximumLatency)
	{
		double currentTime = PhotonNetwork.CurrentTime;
		double num = 4294967.295 - maximumLatency;
		double num2;
		if (currentTime > maximumLatency || time < maximumLatency)
		{
			if (time > currentTime + 0.5)
			{
				return false;
			}
			num2 = currentTime - time;
		}
		else
		{
			double num3 = num + currentTime;
			if (time > currentTime + 0.5 && time < num3)
			{
				return false;
			}
			num2 = currentTime + (4294967.295 - time);
		}
		return num2 <= maximumLatency;
	}

	// Token: 0x06005591 RID: 21905 RVA: 0x001BE158 File Offset: 0x001BC358
	public static double CalculateNetworkDeltaTime(double prevTime, double newTime)
	{
		if (newTime >= prevTime)
		{
			return newTime - prevTime;
		}
		double num = 4294967.295 - prevTime;
		return newTime + num;
	}

	// Token: 0x040065BD RID: 26045
	private static ObjectPool<PooledList<IPreDisable>> g_listPool = new ObjectPool<PooledList<IPreDisable>>(2, 10);

	// Token: 0x040065BE RID: 26046
	private static StringBuilder reusableSB = new StringBuilder();
}
