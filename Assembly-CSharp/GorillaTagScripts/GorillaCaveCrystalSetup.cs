using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F0B RID: 3851
	[CreateAssetMenu(fileName = "GorillaCaveCrystalSetup", menuName = "ScriptableObjects/GorillaCaveCrystalSetup", order = 0)]
	public class GorillaCaveCrystalSetup : ScriptableObject
	{
		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x06006006 RID: 24582 RVA: 0x001EF6BF File Offset: 0x001ED8BF
		public static GorillaCaveCrystalSetup Instance
		{
			get
			{
				return GorillaCaveCrystalSetup.gInstance;
			}
		}

		// Token: 0x06006007 RID: 24583 RVA: 0x001EF6C6 File Offset: 0x001ED8C6
		private void OnEnable()
		{
			if (GorillaCaveCrystalSetup.gInstance == null)
			{
				GorillaCaveCrystalSetup.gInstance = this;
			}
		}

		// Token: 0x06006008 RID: 24584 RVA: 0x001EF6DC File Offset: 0x001ED8DC
		public GorillaCaveCrystalSetup.CrystalDef[] GetCrystalDefs()
		{
			return (from f in typeof(GorillaCaveCrystalSetup).GetRuntimeFields()
			where f != null && f.FieldType == typeof(GorillaCaveCrystalSetup.CrystalDef)
			select (GorillaCaveCrystalSetup.CrystalDef)f.GetValue(this)).ToArray<GorillaCaveCrystalSetup.CrystalDef>();
		}

		// Token: 0x04006E9F RID: 28319
		public Material SharedBase;

		// Token: 0x04006EA0 RID: 28320
		public Texture2D CrystalAlbedo;

		// Token: 0x04006EA1 RID: 28321
		public Texture2D CrystalDarkAlbedo;

		// Token: 0x04006EA2 RID: 28322
		public GorillaCaveCrystalSetup.CrystalDef Red;

		// Token: 0x04006EA3 RID: 28323
		public GorillaCaveCrystalSetup.CrystalDef Orange;

		// Token: 0x04006EA4 RID: 28324
		public GorillaCaveCrystalSetup.CrystalDef Yellow;

		// Token: 0x04006EA5 RID: 28325
		public GorillaCaveCrystalSetup.CrystalDef Green;

		// Token: 0x04006EA6 RID: 28326
		public GorillaCaveCrystalSetup.CrystalDef Teal;

		// Token: 0x04006EA7 RID: 28327
		public GorillaCaveCrystalSetup.CrystalDef DarkBlue;

		// Token: 0x04006EA8 RID: 28328
		public GorillaCaveCrystalSetup.CrystalDef Pink;

		// Token: 0x04006EA9 RID: 28329
		public GorillaCaveCrystalSetup.CrystalDef Dark;

		// Token: 0x04006EAA RID: 28330
		public GorillaCaveCrystalSetup.CrystalDef DarkLight;

		// Token: 0x04006EAB RID: 28331
		public GorillaCaveCrystalSetup.CrystalDef DarkLightUnderWater;

		// Token: 0x04006EAC RID: 28332
		[SerializeField]
		[TextArea(4, 10)]
		private string _notes;

		// Token: 0x04006EAD RID: 28333
		[Space]
		[SerializeField]
		private GameObject _target;

		// Token: 0x04006EAE RID: 28334
		private static GorillaCaveCrystalSetup gInstance;

		// Token: 0x04006EAF RID: 28335
		private static GorillaCaveCrystalSetup.CrystalDef[] gCrystalDefs;

		// Token: 0x02000F0C RID: 3852
		[Serializable]
		public class CrystalDef
		{
			// Token: 0x04006EB0 RID: 28336
			public Material keyMaterial;

			// Token: 0x04006EB1 RID: 28337
			public CrystalVisualsPreset visualPreset;

			// Token: 0x04006EB2 RID: 28338
			[Space]
			public int low;

			// Token: 0x04006EB3 RID: 28339
			public int mid;

			// Token: 0x04006EB4 RID: 28340
			public int high;
		}
	}
}
