using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KID.Model;
using UnityEngine;

// Token: 0x02000B21 RID: 2849
public class TMPSession
{
	// Token: 0x170006D1 RID: 1745
	// (get) Token: 0x0600485B RID: 18523 RVA: 0x001829D6 File Offset: 0x00180BD6
	public bool IsValidSession
	{
		get
		{
			return (this.IsDefault && this.Permissions != null && this.Permissions.Count > 0) || (!this.IsDefault && this.SessionId != Guid.Empty);
		}
	}

	// Token: 0x0600485C RID: 18524 RVA: 0x00182A14 File Offset: 0x00180C14
	public TMPSession(Session session, KIDDefaultSession defaultSession, SessionStatus status)
	{
		this.Permissions = new Dictionary<EKIDFeatures, Permission>();
		this.OptedInPermissions = new HashSet<EKIDFeatures>();
		this.SessionStatus = status;
		if (session == null && defaultSession == null)
		{
			return;
		}
		if (session == null)
		{
			this.IsDefault = true;
			this.AgeStatus = defaultSession.AgeStatus;
			this.Age = defaultSession.Age;
			this.InitialiseDefaultPermissionSet(defaultSession);
			return;
		}
		this.SessionId = session.SessionId;
		this.Etag = session.Etag;
		this.AgeStatus = session.AgeStatus;
		this.KidStatus = session.Status;
		this.DateOfBirth = session.DateOfBirth;
		this.KUID = session.Kuid;
		this.Jurisdiction = session.Jurisdiction;
		this.ManagedBy = session.ManagedBy;
		this.Age = this.GetAgeFromDateOfBirth();
		for (int i = 0; i < session.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(session.Permissions[i].Name);
			if (ekidfeatures != null && !this.Permissions.TryAdd(ekidfeatures.Value, session.Permissions[i]))
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
			}
		}
	}

	// Token: 0x0600485D RID: 18525 RVA: 0x00182B58 File Offset: 0x00180D58
	public void SetOptInPermissions(string[] optedInPermissions)
	{
		if (optedInPermissions == null || optedInPermissions.Length == 0)
		{
			Debug.LogWarning("[KID::SESSION] OptedInPermissions is null or empty. Returning without setting.");
			return;
		}
		int num = 0;
		for (;;)
		{
			int num2 = num;
			int? num3 = (optedInPermissions != null) ? new int?(optedInPermissions.Length) : null;
			if (!(num2 < num3.GetValueOrDefault() & num3 != null))
			{
				break;
			}
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(optedInPermissions[num]);
			if (ekidfeatures != null)
			{
				this.OptInToPermission(ekidfeatures.Value, true);
			}
			num++;
		}
		Debug.Log(string.Format("[KID::SESSION::OptInRefactor] Constructor OptedInPermissions: {0}", this.GetOptedInPermissions()));
	}

	// Token: 0x0600485E RID: 18526 RVA: 0x00182BDF File Offset: 0x00180DDF
	public bool TryGetPermission(EKIDFeatures feature, out Permission permission)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.LogError("[KID::SESSION] Tried retreiving permission for [" + feature.ToStandardisedString() + "], but does not exist");
			permission = null;
			return false;
		}
		permission = this.Permissions[feature];
		return true;
	}

	// Token: 0x0600485F RID: 18527 RVA: 0x00182C1D File Offset: 0x00180E1D
	public List<Permission> GetAllPermissions()
	{
		return this.Permissions.Values.ToList<Permission>();
	}

	// Token: 0x06004860 RID: 18528 RVA: 0x00182C30 File Offset: 0x00180E30
	public bool HasPermissionForFeature(EKIDFeatures feature)
	{
		Permission permission;
		if (!this.TryGetPermission(feature, out permission))
		{
			Debug.LogError("[KID::SESSION] Tried checking for permission but couldn't find [" + feature.ToStandardisedString() + "]. Assuming disabled");
			return false;
		}
		return permission.Enabled;
	}

	// Token: 0x06004861 RID: 18529 RVA: 0x00182C6C File Offset: 0x00180E6C
	public void OptInToPermission(EKIDFeatures feature, bool optIn)
	{
		Debug.Log(string.Format("[KID::SESSION::OptInRefactor] Opting in to permission for [{0}] with optIn: {1}", feature.ToStandardisedString(), optIn));
		if (optIn && !this.OptedInPermissions.Contains(feature))
		{
			this.OptedInPermissions.Add(feature);
			return;
		}
		if (!optIn && this.OptedInPermissions.Contains(feature))
		{
			this.OptedInPermissions.Remove(feature);
			return;
		}
	}

	// Token: 0x06004862 RID: 18530 RVA: 0x00182CD2 File Offset: 0x00180ED2
	public bool HasOptedInToPermission(EKIDFeatures feature)
	{
		return this.OptedInPermissions.Contains(feature);
	}

	// Token: 0x06004863 RID: 18531 RVA: 0x00182CE0 File Offset: 0x00180EE0
	public string[] GetOptedInPermissions()
	{
		if (this.OptedInPermissions == null || this.OptedInPermissions.Count == 0)
		{
			Debug.LogWarning("[KID::SESSION] OptedInPermissions is null or empty. Returning empty array.");
			return Array.Empty<string>();
		}
		return (from f in this.OptedInPermissions
		select f.ToStandardisedString()).ToArray<string>();
	}

	// Token: 0x06004864 RID: 18532 RVA: 0x00182D44 File Offset: 0x00180F44
	public void UpdatePermission(EKIDFeatures feature, Permission newData)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.Log("[KID::SESSION] Trying to update permission, but could not find [" + feature.ToStandardisedString() + "] in dictionary. Will add new one");
			this.Permissions.Add(feature, null);
		}
		this.Permissions[feature] = newData;
	}

	// Token: 0x06004865 RID: 18533 RVA: 0x00182D94 File Offset: 0x00180F94
	private void InitialiseDefaultPermissionSet(KIDDefaultSession defaultSession)
	{
		for (int i = 0; i < defaultSession.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(defaultSession.Permissions[i].Name);
			if (ekidfeatures != null && !this.Permissions.TryAdd(ekidfeatures.Value, defaultSession.Permissions[i]))
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
			}
		}
	}

	// Token: 0x06004866 RID: 18534 RVA: 0x00182E18 File Offset: 0x00181018
	private int GetAgeFromDateOfBirth()
	{
		DateTime today = DateTime.Today;
		int num = today.Year - this.DateOfBirth.Year;
		int num2 = today.Month - this.DateOfBirth.Month;
		if (num2 < 0)
		{
			num--;
		}
		else if (num2 == 0 && today.Day - this.DateOfBirth.Day < 0)
		{
			num--;
		}
		return num;
	}

	// Token: 0x06004867 RID: 18535 RVA: 0x00182E7C File Offset: 0x0018107C
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("New TMPSession]:");
		stringBuilder.AppendLine(string.Format("    - Is Default    :   {0}", this.IsDefault));
		stringBuilder.AppendLine(string.Format("    - Is Valid      :   {0}", this.IsValidSession));
		stringBuilder.AppendLine(string.Format("    - SessionID     :   {0}", this.SessionId));
		stringBuilder.AppendLine(string.Format("    - Age           :   {0}", this.Age));
		stringBuilder.AppendLine(string.Format("    - AgeStatus     :   {0}", this.AgeStatus));
		stringBuilder.AppendLine(string.Format("    - SessionStatus :   {0}", this.KidStatus));
		stringBuilder.AppendLine("    - DoB           :   " + this.DateOfBirth.ToString());
		stringBuilder.AppendLine("    - KUID          :   " + this.KUID);
		stringBuilder.AppendLine("    - Jurisdiction  :   " + this.Jurisdiction);
		stringBuilder.AppendLine("    - PERMISSIONS   :");
		if (this.Permissions != null)
		{
			foreach (Permission permission in this.Permissions.Values)
			{
				stringBuilder.AppendLine(string.Format("        - {0} - Enabled: {1} - ManagedBy: {2}", permission.Name, permission.Enabled, permission.ManagedBy));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04005AA1 RID: 23201
	public readonly Guid SessionId;

	// Token: 0x04005AA2 RID: 23202
	public readonly string Etag;

	// Token: 0x04005AA3 RID: 23203
	public readonly AgeStatusType AgeStatus;

	// Token: 0x04005AA4 RID: 23204
	public readonly Session.StatusEnum KidStatus;

	// Token: 0x04005AA5 RID: 23205
	public readonly Session.ManagedByEnum ManagedBy;

	// Token: 0x04005AA6 RID: 23206
	public readonly DateTime DateOfBirth;

	// Token: 0x04005AA7 RID: 23207
	public readonly string Jurisdiction;

	// Token: 0x04005AA8 RID: 23208
	public readonly string KUID;

	// Token: 0x04005AA9 RID: 23209
	public readonly int Age;

	// Token: 0x04005AAA RID: 23210
	public readonly bool IsDefault;

	// Token: 0x04005AAB RID: 23211
	public readonly SessionStatus SessionStatus;

	// Token: 0x04005AAC RID: 23212
	private Dictionary<EKIDFeatures, Permission> Permissions;

	// Token: 0x04005AAD RID: 23213
	private HashSet<EKIDFeatures> OptedInPermissions;
}
