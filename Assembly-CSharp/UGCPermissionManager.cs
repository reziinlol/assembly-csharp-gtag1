using System;
using GorillaNetworking;
using KID.Model;
using UnityEngine;

// Token: 0x02000B6F RID: 2927
internal class UGCPermissionManager : MonoBehaviour
{
	// Token: 0x060049B7 RID: 18871 RVA: 0x0018B2AA File Offset: 0x001894AA
	public static void UsePlayFabSafety()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.PlayFabPermissions(new Action<bool>(UGCPermissionManager.SetUGCEnabled));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x060049B8 RID: 18872 RVA: 0x0018B2CC File Offset: 0x001894CC
	public static void UseKID()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.KIDPermissions(new Action<bool>(UGCPermissionManager.SetUGCEnabled));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x170006F9 RID: 1785
	// (get) Token: 0x060049B9 RID: 18873 RVA: 0x0018B2EE File Offset: 0x001894EE
	public static bool IsUGCDisabled
	{
		get
		{
			return !UGCPermissionManager.isUGCEnabled.GetValueOrDefault();
		}
	}

	// Token: 0x060049BA RID: 18874 RVA: 0x0018B2FD File Offset: 0x001894FD
	public static void CheckPermissions()
	{
		UGCPermissionManager.IUGCPermissions iugcpermissions = UGCPermissionManager.permissions;
		if (iugcpermissions == null)
		{
			return;
		}
		iugcpermissions.CheckPermissions();
	}

	// Token: 0x060049BB RID: 18875 RVA: 0x0018B30E File Offset: 0x0018950E
	public static void SubscribeToUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x060049BC RID: 18876 RVA: 0x0018B325 File Offset: 0x00189525
	public static void UnsubscribeFromUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x060049BD RID: 18877 RVA: 0x0018B33C File Offset: 0x0018953C
	public static void SubscribeToUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x060049BE RID: 18878 RVA: 0x0018B353 File Offset: 0x00189553
	public static void UnsubscribeFromUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x060049BF RID: 18879 RVA: 0x0018B36C File Offset: 0x0018956C
	private static void SetUGCEnabled(bool enabled)
	{
		bool? flag = UGCPermissionManager.isUGCEnabled;
		if (!(enabled == flag.GetValueOrDefault() & flag != null))
		{
			UGCPermissionManager.isUGCEnabled = new bool?(enabled);
			if (enabled)
			{
				Action action = UGCPermissionManager.onUGCEnabled;
				if (action == null)
				{
					return;
				}
				action();
				return;
			}
			else
			{
				Action action2 = UGCPermissionManager.onUGCDisabled;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}
	}

	// Token: 0x04005C89 RID: 23689
	[OnEnterPlay_SetNull]
	private static UGCPermissionManager.IUGCPermissions permissions;

	// Token: 0x04005C8A RID: 23690
	[OnEnterPlay_SetNull]
	private static Action onUGCEnabled;

	// Token: 0x04005C8B RID: 23691
	[OnEnterPlay_SetNull]
	private static Action onUGCDisabled;

	// Token: 0x04005C8C RID: 23692
	private static bool? isUGCEnabled;

	// Token: 0x02000B70 RID: 2928
	private interface IUGCPermissions
	{
		// Token: 0x060049C1 RID: 18881
		void Initialize();

		// Token: 0x060049C2 RID: 18882
		void CheckPermissions();
	}

	// Token: 0x02000B71 RID: 2929
	private class PlayFabPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x060049C3 RID: 18883 RVA: 0x0018B3C0 File Offset: 0x001895C0
		public PlayFabPermissions(Action<bool> setUGCEnabled)
		{
			this.setUGCEnabled = setUGCEnabled;
		}

		// Token: 0x060049C4 RID: 18884 RVA: 0x0018B3D0 File Offset: 0x001895D0
		public void Initialize()
		{
			bool safety = PlayFabAuthenticator.instance.GetSafety();
			Action<bool> action = this.setUGCEnabled;
			if (action == null)
			{
				return;
			}
			action(!safety);
		}

		// Token: 0x060049C5 RID: 18885 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void CheckPermissions()
		{
		}

		// Token: 0x04005C8D RID: 23693
		private Action<bool> setUGCEnabled;
	}

	// Token: 0x02000B72 RID: 2930
	private class KIDPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x060049C6 RID: 18886 RVA: 0x0018B3FE File Offset: 0x001895FE
		public KIDPermissions(Action<bool> setUGCEnabled)
		{
			this.setUGCEnabled = setUGCEnabled;
		}

		// Token: 0x060049C7 RID: 18887 RVA: 0x0018B40D File Offset: 0x0018960D
		private void SetUGCEnabled(bool enabled)
		{
			Action<bool> action = this.setUGCEnabled;
			if (action == null)
			{
				return;
			}
			action(enabled);
		}

		// Token: 0x060049C8 RID: 18888 RVA: 0x0018B420 File Offset: 0x00189620
		public void Initialize()
		{
			Debug.Log("[UGCPermissionManager][KID] Initializing with KID");
			this.CheckPermissions();
			KIDManager.RegisterSessionUpdatedCallback_UGC(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdate));
		}

		// Token: 0x060049C9 RID: 18889 RVA: 0x0018B444 File Offset: 0x00189644
		public void CheckPermissions()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Mods);
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, permissionDataByFeature.Enabled, permissionDataByFeature.ManagedBy);
		}

		// Token: 0x060049CA RID: 18890 RVA: 0x0018B478 File Offset: 0x00189678
		private void OnKIDSessionUpdate(bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.Log("[UGCPermissionManager][KID] KID session update.");
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, isEnabled, managedBy);
		}

		// Token: 0x060049CB RID: 18891 RVA: 0x0018B4A8 File Offset: 0x001896A8
		private void ProcessPermissionKID(bool hasOptedIn, bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.LogFormat("[UGCPermissionManager][KID] Process KID permissions - opted in: [{0}], enabled: [{1}], managedBy: [{2}].", new object[]
			{
				hasOptedIn,
				isEnabled,
				managedBy
			});
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC prohibited.");
				this.SetUGCEnabled(false);
				return;
			}
			if (managedBy != Permission.ManagedByEnum.PLAYER)
			{
				if (managedBy == Permission.ManagedByEnum.GUARDIAN)
				{
					Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by guardian. (opted in: [{0}], enabled: [{1}])", new object[]
					{
						hasOptedIn,
						isEnabled
					});
					this.SetUGCEnabled(isEnabled);
				}
				return;
			}
			if (isEnabled)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC managed by player and enabled - opting in and enabling UGC.");
				if (!hasOptedIn)
				{
					KIDManager.SetFeatureOptIn(EKIDFeatures.Mods, true);
				}
				this.SetUGCEnabled(true);
				return;
			}
			Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by player and disabled by default - using opt in status. (opted in: [{0}])", new object[]
			{
				hasOptedIn
			});
			this.SetUGCEnabled(hasOptedIn);
		}

		// Token: 0x04005C8E RID: 23694
		private Action<bool> setUGCEnabled;
	}
}
