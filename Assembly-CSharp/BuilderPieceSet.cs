using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

// Token: 0x02000637 RID: 1591
[CreateAssetMenu(fileName = "BuilderPieceSet01", menuName = "Gorilla Tag/Builder/PieceSet", order = 0)]
public class BuilderPieceSet : ScriptableObject
{
	// Token: 0x170003F9 RID: 1017
	// (get) Token: 0x060027D8 RID: 10200 RVA: 0x000D5F09 File Offset: 0x000D4109
	public string SetName
	{
		get
		{
			return this.setName;
		}
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x000D5F11 File Offset: 0x000D4111
	public int GetIntIdentifier()
	{
		return this.playfabID.GetStaticHash();
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x000D5F20 File Offset: 0x000D4120
	public DateTime GetScheduleDateTime()
	{
		if (this.isScheduled)
		{
			try
			{
				return DateTime.Parse(this.scheduledDate, CultureInfo.InvariantCulture);
			}
			catch
			{
				return DateTime.MinValue;
			}
		}
		return DateTime.MinValue;
	}

	// Token: 0x0400339B RID: 13211
	[Tooltip("Display Name - Fallback for Localization")]
	public string setName;

	// Token: 0x0400339C RID: 13212
	public GameObject displayModel;

	// Token: 0x0400339D RID: 13213
	[Tooltip("If this should error if no localization is found")]
	public bool isLocalized;

	// Token: 0x0400339E RID: 13214
	[Tooltip("Localized Display Name")]
	public LocalizedString setLocName;

	// Token: 0x0400339F RID: 13215
	[FormerlySerializedAs("uniqueId")]
	[Tooltip("If purchaseable, this should be a valid playfabID starting with LD\nIf a starter set, this just needs to be a unique string from the other set IDs")]
	public string playfabID;

	// Token: 0x040033A0 RID: 13216
	[Tooltip("(Optional) Default Material ID applied to all prefabs with BuilderMaterialOptions")]
	public string materialId;

	// Token: 0x040033A1 RID: 13217
	[Tooltip("(Optional) If this set is not available on launch day use scheduling")]
	public bool isScheduled;

	// Token: 0x040033A2 RID: 13218
	public string scheduledDate = "1/1/0001 00:00:00";

	// Token: 0x040033A3 RID: 13219
	[Tooltip("A group of pieces on the same shelf")]
	public List<BuilderPieceSet.BuilderPieceSubset> subsets;

	// Token: 0x02000638 RID: 1592
	public enum BuilderPieceCategory
	{
		// Token: 0x040033A5 RID: 13221
		FLAT,
		// Token: 0x040033A6 RID: 13222
		TALL,
		// Token: 0x040033A7 RID: 13223
		HALF_HEIGHT,
		// Token: 0x040033A8 RID: 13224
		BEAM,
		// Token: 0x040033A9 RID: 13225
		SLOPE,
		// Token: 0x040033AA RID: 13226
		OVERSIZED,
		// Token: 0x040033AB RID: 13227
		SPECIAL_DISPLAY,
		// Token: 0x040033AC RID: 13228
		FUNCTIONAL = 18,
		// Token: 0x040033AD RID: 13229
		DECORATIVE,
		// Token: 0x040033AE RID: 13230
		MISC
	}

	// Token: 0x02000639 RID: 1593
	[Serializable]
	public class BuilderPieceSubset
	{
		// Token: 0x060027DC RID: 10204 RVA: 0x000D5F7B File Offset: 0x000D417B
		public string GetShelfButtonName()
		{
			return this.shelfButtonName;
		}

		// Token: 0x040033AF RID: 13231
		[Tooltip("(Optional) Text to put on the shelf button if not the set name")]
		public string shelfButtonName;

		// Token: 0x040033B0 RID: 13232
		public LocalizedString localizedShelfButtonName;

		// Token: 0x040033B1 RID: 13233
		public BuilderPieceSet.BuilderPieceCategory pieceCategory;

		// Token: 0x040033B2 RID: 13234
		public List<BuilderPieceSet.PieceInfo> pieceInfos;
	}

	// Token: 0x0200063A RID: 1594
	[Serializable]
	public struct PieceInfo
	{
		// Token: 0x040033B3 RID: 13235
		public BuilderPiece piecePrefab;

		// Token: 0x040033B4 RID: 13236
		[Tooltip("(Optional) should this piece use a materialID other than the set's materialID")]
		public bool overrideSetMaterial;

		// Token: 0x040033B5 RID: 13237
		[Tooltip("material type string should match an entry in this prefab's BuilderMaterialOptions\nIf multiple are in the list the piece will cycle through materials when spawned\nTo have each variant on the shelf create a new pieceInfo for each color")]
		public string[] pieceMaterialTypes;
	}

	// Token: 0x0200063B RID: 1595
	public class BuilderDisplayGroup
	{
		// Token: 0x060027DE RID: 10206 RVA: 0x000D5F83 File Offset: 0x000D4183
		public BuilderDisplayGroup()
		{
			this.displayName = string.Empty;
			this.pieceSubsets = new List<BuilderPieceSet.BuilderPieceSubset>();
			this.defaultMaterial = string.Empty;
			this.setID = -1;
			this.uniqueGroupID = string.Empty;
		}

		// Token: 0x060027DF RID: 10207 RVA: 0x000D5FBE File Offset: 0x000D41BE
		public BuilderDisplayGroup(string groupName, string material, int inSetID, string groupID)
		{
			this.displayName = groupName;
			this.pieceSubsets = new List<BuilderPieceSet.BuilderPieceSubset>();
			this.defaultMaterial = material;
			this.setID = inSetID;
			this.uniqueGroupID = groupID;
		}

		// Token: 0x060027E0 RID: 10208 RVA: 0x000D5FEE File Offset: 0x000D41EE
		public int GetDisplayGroupIdentifier()
		{
			return this.uniqueGroupID.GetStaticHash();
		}

		// Token: 0x040033B6 RID: 13238
		public string displayName;

		// Token: 0x040033B7 RID: 13239
		public List<BuilderPieceSet.BuilderPieceSubset> pieceSubsets;

		// Token: 0x040033B8 RID: 13240
		public string defaultMaterial;

		// Token: 0x040033B9 RID: 13241
		public int setID;

		// Token: 0x040033BA RID: 13242
		public string uniqueGroupID;
	}
}
