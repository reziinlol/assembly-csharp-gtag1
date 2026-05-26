using System;
using UnityEngine;

// Token: 0x02000B64 RID: 2916
public static class KIDTelemetry
{
	// Token: 0x170006EC RID: 1772
	// (get) Token: 0x06004978 RID: 18808 RVA: 0x00038A27 File Offset: 0x00036C27
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x170006ED RID: 1773
	// (get) Token: 0x06004979 RID: 18809 RVA: 0x001899CE File Offset: 0x00187BCE
	public static string Open_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Open";
		}
	}

	// Token: 0x170006EE RID: 1774
	// (get) Token: 0x0600497A RID: 18810 RVA: 0x001899D5 File Offset: 0x00187BD5
	public static string Updated_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Updated";
		}
	}

	// Token: 0x170006EF RID: 1775
	// (get) Token: 0x0600497B RID: 18811 RVA: 0x001899DC File Offset: 0x00187BDC
	public static string Closed_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Closed";
		}
	}

	// Token: 0x170006F0 RID: 1776
	// (get) Token: 0x0600497C RID: 18812 RVA: 0x00038A38 File Offset: 0x00036C38
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x0600497D RID: 18813 RVA: 0x001899E3 File Offset: 0x00187BE3
	public static string GetPermissionManagedByBodyData(string permission)
	{
		return "permission_managedby_" + permission.Replace('-', '_');
	}

	// Token: 0x0600497E RID: 18814 RVA: 0x001899F9 File Offset: 0x00187BF9
	public static string GetPermissionEnabledBodyData(string permission)
	{
		return "permission_eneabled_" + permission.Replace('-', '_');
	}

	// Token: 0x04005C20 RID: 23584
	public const string SCREEN_SHOWN_EVENT_NAME = "kid_screen_shown";

	// Token: 0x04005C21 RID: 23585
	public const string PHASE_TWO_IN_COHORT_EVENT_NAME = "kid_phase2_incohort";

	// Token: 0x04005C22 RID: 23586
	public const string PHASE_THREE_OPTIONAL_EVENT_NAME = "kid_phase3_optional";

	// Token: 0x04005C23 RID: 23587
	public const string AGE_GATE_EVENT_NAME = "kid_age_gate";

	// Token: 0x04005C24 RID: 23588
	public const string AGE_GATE_CONFIRM_EVENT_NAME = "kid_age_gate_confirm";

	// Token: 0x04005C25 RID: 23589
	public const string AGE_DISCREPENCY_EVENT_NAME = "kid_age_gate_discrepency";

	// Token: 0x04005C26 RID: 23590
	public const string GAME_SETTINGS_EVENT_NAME = "kid_game_settings";

	// Token: 0x04005C27 RID: 23591
	public const string EMAIL_CONFIRM_EVENT_NAME = "kid_email_confirm";

	// Token: 0x04005C28 RID: 23592
	public const string AGE_APPEAL_EVENT_NAME = "kid_age_appeal";

	// Token: 0x04005C29 RID: 23593
	public const string APPEAL_AGE_GATE_EVENT_NAME = "kid_age_appeal_age_gate";

	// Token: 0x04005C2A RID: 23594
	public const string APPEAL_ENTER_EMAIL_EVENT_NAME = "kid_age_appeal_enter_email";

	// Token: 0x04005C2B RID: 23595
	public const string APPEAL_CONFIRM_EMAIL_EVENT_NAME = "kid_age_appeal_confirm_email";

	// Token: 0x04005C2C RID: 23596
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x04005C2D RID: 23597
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x04005C2E RID: 23598
	public const string WARNING_SCREEN_CUSTOM_TAG = "kid_warning_screen";

	// Token: 0x04005C2F RID: 23599
	public const string PHASE_TWO = "kid_phase_2";

	// Token: 0x04005C30 RID: 23600
	public const string PHASE_THREE = "kid_phase_3";

	// Token: 0x04005C31 RID: 23601
	public const string PHASE_FOUR = "kid_phase_4";

	// Token: 0x04005C32 RID: 23602
	public const string AGE_GATE_CUSTOM_TAG = "kid_age_gate";

	// Token: 0x04005C33 RID: 23603
	public const string SETTINGS_CUSTOM_TAG = "kid_settings";

	// Token: 0x04005C34 RID: 23604
	public const string SETUP_CUSTOM_TAG = "kid_setup";

	// Token: 0x04005C35 RID: 23605
	public const string APPEAL_CUSTOM_TAG = "kid_age_appeal";

	// Token: 0x04005C36 RID: 23606
	public const string SCREEN_TYPE_BODY_DATA = "screen";

	// Token: 0x04005C37 RID: 23607
	public const string OPT_IN_CHOICE_BODY_DATA = "opt_in_choice";

	// Token: 0x04005C38 RID: 23608
	public const string BUTTON_PRESSED_BODY_DATA = "button_pressed";

	// Token: 0x04005C39 RID: 23609
	public const string MISMATCH_EXPECTED_BODY_DATA = "mismatch_expected";

	// Token: 0x04005C3A RID: 23610
	public const string MISMATCH_ACTUAL_BODY_DATA = "mismatch_actual";

	// Token: 0x04005C3B RID: 23611
	public const string AGE_DECLARED_BODY_DATA = "age_declared";

	// Token: 0x04005C3C RID: 23612
	public const string LEARN_MORE_URL_PRESSED_BODY_DATA = "learn_more_url_pressed";

	// Token: 0x04005C3D RID: 23613
	public const string SCREEN_SHOWN_REASON_BODY_DATA = "screen_shown_reason";

	// Token: 0x04005C3E RID: 23614
	public const string SUBMITTED_AGE_BODY_DATA = "submitted_age";

	// Token: 0x04005C3F RID: 23615
	public const string CORRECT_AGE_BODY_DATA = "correct_age";

	// Token: 0x04005C40 RID: 23616
	public const string APPEAL_EMAIL_TYPE_BODY_DATA = "email_type";

	// Token: 0x04005C41 RID: 23617
	public const string SHOWN_SETTINGS_SCREEN = "saw_game_settings";

	// Token: 0x04005C42 RID: 23618
	public const string KID_STATUS_BODY_DATA = "kid_status";

	// Token: 0x04005C43 RID: 23619
	private const string PERMISSION_MANAGED_BY_BODY_DATA = "permission_managedby_";

	// Token: 0x04005C44 RID: 23620
	private const string PERMISSION_ENABLED_BODY_DATA = "permission_eneabled_";
}
