using System;
using System.Text;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001292 RID: 4754
	public class NetworkedRandomProvider : MonoBehaviour
	{
		// Token: 0x060076FF RID: 30463 RVA: 0x00270776 File Offset: 0x0026E976
		private void Awake()
		{
			if (this.parentTransferable == null)
			{
				this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			}
		}

		// Token: 0x06007700 RID: 30464 RVA: 0x00270792 File Offset: 0x0026E992
		private void OnEnable()
		{
			this.EnsureOwner();
		}

		// Token: 0x06007701 RID: 30465 RVA: 0x0027079C File Offset: 0x0026E99C
		private void OnValidate()
		{
			if (this.windowSeconds < 0.01f)
			{
				this.windowSeconds = 0.01f;
			}
			if (this.floatRange.y < this.floatRange.x)
			{
				ref float ptr = ref this.floatRange.x;
				float y = this.floatRange.y;
				float x = this.floatRange.x;
				ptr = y;
				this.floatRange.y = x;
			}
			if (this.doubleMax < this.doubleMin)
			{
				double num = this.doubleMax;
				double num2 = this.doubleMin;
				this.doubleMin = num;
				this.doubleMax = num2;
			}
		}

		// Token: 0x06007702 RID: 30466 RVA: 0x0027083C File Offset: 0x0026EA3C
		private void Update()
		{
			long num = (long)Math.Floor(this.GetSharedTime() / (double)this.windowSeconds);
			this.debugWindow = num;
		}

		// Token: 0x06007703 RID: 30467 RVA: 0x00270865 File Offset: 0x0026EA65
		private bool ShowFloatRange()
		{
			return this.outputMode == NetworkedRandomProvider.OutputMode.FloatRange;
		}

		// Token: 0x06007704 RID: 30468 RVA: 0x00270870 File Offset: 0x0026EA70
		private bool ShowDoubleRange()
		{
			return this.outputMode == NetworkedRandomProvider.OutputMode.DoubleRange;
		}

		// Token: 0x06007705 RID: 30469 RVA: 0x0027087B File Offset: 0x0026EA7B
		private long GetWindowIndex()
		{
			return (long)Math.Floor(this.GetSharedTime() / (double)this.windowSeconds);
		}

		// Token: 0x06007706 RID: 30470 RVA: 0x00270891 File Offset: 0x0026EA91
		private double GetSharedTime()
		{
			if (PhotonNetwork.InRoom)
			{
				return PhotonNetwork.Time;
			}
			return (double)Time.realtimeSinceStartup;
		}

		// Token: 0x06007707 RID: 30471 RVA: 0x002708A6 File Offset: 0x0026EAA6
		private static ulong Mix64(ulong x)
		{
			x += 11400714819323198485UL;
			x = (x ^ x >> 30) * 13787848793156543929UL;
			x = (x ^ x >> 27) * 10723151780598845931UL;
			x ^= x >> 31;
			return x;
		}

		// Token: 0x06007708 RID: 30472 RVA: 0x002708E2 File Offset: 0x0026EAE2
		private static ulong BuildSeed(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
		{
			return (ulong)(windowIndex ^ (long)((long)((ulong)ownerId) << 32) ^ (long)((ulong)objectSalt * 11400714819323198485UL) ^ (long)((ulong)roomSalt * 15183679468541472403UL));
		}

		// Token: 0x06007709 RID: 30473 RVA: 0x00270905 File Offset: 0x0026EB05
		private static float UnitFloat01(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
		{
			return (uint)(NetworkedRandomProvider.Mix64(NetworkedRandomProvider.BuildSeed(windowIndex, ownerId, objectSalt, roomSalt)) >> 40) * 5.9604645E-08f;
		}

		// Token: 0x0600770A RID: 30474 RVA: 0x00270921 File Offset: 0x0026EB21
		private static double UnitDouble01(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
		{
			return (NetworkedRandomProvider.Mix64(NetworkedRandomProvider.BuildSeed(windowIndex, ownerId, objectSalt, roomSalt)) >> 11) * 1.1102230246251565E-16;
		}

		// Token: 0x0600770B RID: 30475 RVA: 0x00270940 File Offset: 0x0026EB40
		public float NextFloat01()
		{
			this.EnsureOwner();
			long windowIndex = this.GetWindowIndex();
			uint num;
			if (!this.includeRoomNameInSeed)
			{
				num = 0U;
			}
			else
			{
				string s;
				if (!PhotonNetwork.InRoom)
				{
					s = "no_room";
				}
				else
				{
					Room currentRoom = PhotonNetwork.CurrentRoom;
					s = (((currentRoom != null) ? currentRoom.Name : null) ?? "no_room");
				}
				num = NetworkedRandomProvider.StableHash(s);
			}
			uint roomSalt = num;
			float result = NetworkedRandomProvider.UnitFloat01(windowIndex, this.OwnerID, this.objectSalt, roomSalt);
			this.debugResult = result;
			return result;
		}

		// Token: 0x0600770C RID: 30476 RVA: 0x002709B0 File Offset: 0x0026EBB0
		public float NextFloat(float min, float max)
		{
			float t = this.NextFloat01();
			if (max < min)
			{
				float num = max;
				float num2 = min;
				min = num;
				max = num2;
			}
			return Mathf.Lerp(min, max, t);
		}

		// Token: 0x0600770D RID: 30477 RVA: 0x002709D8 File Offset: 0x0026EBD8
		public double NextDouble(double min, double max)
		{
			this.EnsureOwner();
			long windowIndex = this.GetWindowIndex();
			uint num;
			if (!this.includeRoomNameInSeed)
			{
				num = 0U;
			}
			else
			{
				string s;
				if (!PhotonNetwork.InRoom)
				{
					s = "no_room";
				}
				else
				{
					Room currentRoom = PhotonNetwork.CurrentRoom;
					s = (((currentRoom != null) ? currentRoom.Name : null) ?? "no_room");
				}
				num = NetworkedRandomProvider.StableHash(s);
			}
			uint roomSalt = num;
			double num2 = NetworkedRandomProvider.UnitDouble01(windowIndex, this.OwnerID, this.objectSalt, roomSalt);
			if (max < min)
			{
				double num3 = max;
				double num4 = min;
				min = num3;
				max = num4;
			}
			double num5 = min + (max - min) * num2;
			this.debugResult = (float)num5;
			return num5;
		}

		// Token: 0x0600770E RID: 30478 RVA: 0x00270A5C File Offset: 0x0026EC5C
		public float GetSelectedAsFloat()
		{
			switch (this.outputMode)
			{
			default:
				return this.NextFloat01();
			case NetworkedRandomProvider.OutputMode.Double01:
				return (float)this.NextDouble(0.0, 1.0);
			case NetworkedRandomProvider.OutputMode.FloatRange:
				return this.NextFloat(this.floatRange.x, this.floatRange.y);
			case NetworkedRandomProvider.OutputMode.DoubleRange:
				return (float)this.NextDouble(this.doubleMin, this.doubleMax);
			}
		}

		// Token: 0x0600770F RID: 30479 RVA: 0x00270AD8 File Offset: 0x0026ECD8
		public double GetSelectedAsDouble()
		{
			switch (this.outputMode)
			{
			default:
				return (double)this.NextFloat01();
			case NetworkedRandomProvider.OutputMode.Double01:
				return this.NextDouble(0.0, 1.0);
			case NetworkedRandomProvider.OutputMode.FloatRange:
				return (double)this.NextFloat(this.floatRange.x, this.floatRange.y);
			case NetworkedRandomProvider.OutputMode.DoubleRange:
				return this.NextDouble(this.doubleMin, this.doubleMax);
			}
		}

		// Token: 0x06007710 RID: 30480 RVA: 0x00270B54 File Offset: 0x0026ED54
		private static uint StableHash(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0U;
			}
			uint num = 2166136261U;
			for (int i = 0; i < s.Length; i++)
			{
				num ^= (uint)s[i];
				num *= 16777619U;
			}
			return num;
		}

		// Token: 0x06007711 RID: 30481 RVA: 0x00270B95 File Offset: 0x0026ED95
		private void EnsureOwner()
		{
			if (this.OwnerID == 0)
			{
				this.TrySetID();
			}
		}

		// Token: 0x06007712 RID: 30482 RVA: 0x00270BA8 File Offset: 0x0026EDA8
		private void TrySetID()
		{
			if (this.parentTransferable == null)
			{
				string name = base.gameObject.scene.name;
				string str = "/";
				string hierarchyPath = NetworkedRandomProvider.GetHierarchyPath(base.transform);
				Type type = base.GetType();
				string s = name + str + hierarchyPath + ((type != null) ? type.ToString() : null);
				this.OwnerID = s.GetStaticHash();
				return;
			}
			if (this.parentTransferable.IsLocalObject())
			{
				PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
				if (instance != null)
				{
					string playFabPlayerId = instance.GetPlayFabPlayerId();
					Type type2 = base.GetType();
					this.OwnerID = (playFabPlayerId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
					return;
				}
			}
			else if (this.parentTransferable.targetRig != null && this.parentTransferable.targetRig.creator != null)
			{
				string userId = this.parentTransferable.targetRig.creator.UserId;
				Type type3 = base.GetType();
				this.OwnerID = (userId + ((type3 != null) ? type3.ToString() : null)).GetStaticHash();
			}
		}

		// Token: 0x06007713 RID: 30483 RVA: 0x00270CB4 File Offset: 0x0026EEB4
		private static string GetHierarchyPath(Transform t)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (t != null)
			{
				stringBuilder.Insert(0, "/" + t.name + "#" + t.GetSiblingIndex().ToString());
				t = t.parent;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0400893E RID: 35134
		[Header("Time Granularity")]
		[Min(0.01f)]
		[Tooltip("Length of the time bucket (seconds). Within a bucket the pick is fixed; re-rolls next bucket.")]
		[SerializeField]
		private float windowSeconds = 1f;

		// Token: 0x0400893F RID: 35135
		[Tooltip("Mix room name into seed so different rooms never collide.")]
		[SerializeField]
		private bool includeRoomNameInSeed = true;

		// Token: 0x04008940 RID: 35136
		[Tooltip("Optional - If multiple component live on the same cosmetic, use different salts.")]
		[SerializeField]
		private int objectSalt;

		// Token: 0x04008941 RID: 35137
		[Header("Output")]
		[SerializeField]
		private NetworkedRandomProvider.OutputMode outputMode;

		// Token: 0x04008942 RID: 35138
		[SerializeField]
		private Vector2 floatRange = new Vector2(0f, 1f);

		// Token: 0x04008943 RID: 35139
		[SerializeField]
		private double doubleMin;

		// Token: 0x04008944 RID: 35140
		[SerializeField]
		private double doubleMax = 1.0;

		// Token: 0x04008945 RID: 35141
		private TransferrableObject parentTransferable;

		// Token: 0x04008946 RID: 35142
		private int OwnerID;

		// Token: 0x04008947 RID: 35143
		[Header("Debug")]
		[SerializeField]
		private long debugWindow;

		// Token: 0x04008948 RID: 35144
		[SerializeField]
		private float debugResult;

		// Token: 0x02001293 RID: 4755
		public enum OutputMode
		{
			// Token: 0x0400894A RID: 35146
			Float01,
			// Token: 0x0400894B RID: 35147
			Double01,
			// Token: 0x0400894C RID: 35148
			FloatRange,
			// Token: 0x0400894D RID: 35149
			DoubleRange
		}
	}
}
