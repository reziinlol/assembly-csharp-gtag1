using System;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x020011CB RID: 4555
	[Serializable]
	public struct CosmeticInfoV2 : ISerializationCallbackReceiver
	{
		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x060072B8 RID: 29368 RVA: 0x00255394 File Offset: 0x00253594
		public bool hasHoldableParts
		{
			get
			{
				CosmeticPart[] array = this.holdableParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000B03 RID: 2819
		// (get) Token: 0x060072B9 RID: 29369 RVA: 0x002553B4 File Offset: 0x002535B4
		public bool hasWardrobeParts
		{
			get
			{
				CosmeticPart[] array = this.wardrobeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x060072BA RID: 29370 RVA: 0x002553D4 File Offset: 0x002535D4
		public bool hasStoreParts
		{
			get
			{
				CosmeticPart[] array = this.storeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x060072BB RID: 29371 RVA: 0x002553F4 File Offset: 0x002535F4
		public bool hasFunctionalParts
		{
			get
			{
				CosmeticPart[] array = this.functionalParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x060072BC RID: 29372 RVA: 0x00255414 File Offset: 0x00253614
		public bool hasFirstPersonViewParts
		{
			get
			{
				CosmeticPart[] array = this.firstPersonViewParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x060072BD RID: 29373 RVA: 0x00255434 File Offset: 0x00253634
		public bool hasLocalRigParts
		{
			get
			{
				CosmeticPart[] array = this.localRigParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x060072BE RID: 29374 RVA: 0x00255454 File Offset: 0x00253654
		public CosmeticInfoV2(string displayName)
		{
			this.enabled = true;
			this.season = null;
			this.displayName = displayName;
			this.playFabID = "";
			this.category = CosmeticsController.CosmeticCategory.None;
			this.icon = null;
			this.isHoldable = false;
			this.isThrowable = false;
			this.usesBothHandSlots = false;
			this.hideWardrobeMannequin = false;
			this.holdableParts = new CosmeticPart[0];
			this.functionalParts = new CosmeticPart[0];
			this.wardrobeParts = new CosmeticPart[0];
			this.storeParts = new CosmeticPart[0];
			this.firstPersonViewParts = new CosmeticPart[0];
			this.localRigParts = new CosmeticPart[0];
			this.setCosmetics = new CosmeticSO[0];
			this.collectionSlots = Array.Empty<CosmeticCollectionSlotDefinition>();
			this.collectionIsCycling = false;
			this.collectionParentPlayFabID = string.Empty;
			this.collectionTargetSlotIndex = -1;
			this.anchorAntiIntersectOffsets = default(CosmeticAnchorAntiIntersectOffsets);
			this.debugCosmeticSOName = "__UNINITIALIZED__";
			this.throwableMaterialGrabIndices = new int[0];
			this.throwableIndex = -1;
			this.collectionUsesIndexTargeting = false;
			this.appliedCosmeticPlayFabID = null;
		}

		// Token: 0x060072BF RID: 29375 RVA: 0x000028C5 File Offset: 0x00000AC5
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		// Token: 0x060072C0 RID: 29376 RVA: 0x00255560 File Offset: 0x00253760
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this._OnAfterDeserialize_InitializePartsArray(ref this.holdableParts, ECosmeticPartType.Holdable);
			this._OnAfterDeserialize_InitializePartsArray(ref this.functionalParts, ECosmeticPartType.Functional);
			this._OnAfterDeserialize_InitializePartsArray(ref this.wardrobeParts, ECosmeticPartType.Wardrobe);
			this._OnAfterDeserialize_InitializePartsArray(ref this.storeParts, ECosmeticPartType.Store);
			this._OnAfterDeserialize_InitializePartsArray(ref this.firstPersonViewParts, ECosmeticPartType.FirstPerson);
			this._OnAfterDeserialize_InitializePartsArray(ref this.localRigParts, ECosmeticPartType.LocalRig);
			if (this.setCosmetics == null)
			{
				this.setCosmetics = Array.Empty<CosmeticSO>();
			}
		}

		// Token: 0x060072C1 RID: 29377 RVA: 0x002555D0 File Offset: 0x002537D0
		private void _OnAfterDeserialize_InitializePartsArray(ref CosmeticPart[] parts, ECosmeticPartType partType)
		{
			for (int i = 0; i < parts.Length; i++)
			{
				parts[i].partType = partType;
				ref CosmeticAttachInfo[] ptr = ref parts[i].attachAnchors;
				if (ptr == null)
				{
					ptr = Array.Empty<CosmeticAttachInfo>();
				}
			}
		}

		// Token: 0x04008291 RID: 33425
		public bool enabled;

		// Token: 0x04008292 RID: 33426
		[Tooltip("// TODO: (2024-09-27 MattO) season will determine what addressables bundle it will be in and wheter it should be active based on release time of season.\n\nThe assigned season will determine what folder the Cosmetic will go in and how it will be listed in the Cosmetic Browser.")]
		[Delayed]
		public SeasonSO season;

		// Token: 0x04008293 RID: 33427
		[Tooltip("Name that is displayed in the store during purchasing.")]
		[Delayed]
		public string displayName;

		// Token: 0x04008294 RID: 33428
		[Tooltip("ID used on the PlayFab servers that must be unique. If this does not exist on the playfab servers then an error will be thrown. In notion search for \"Cosmetics - Adding a PlayFab ID\".")]
		[Delayed]
		public string playFabID;

		// Token: 0x04008295 RID: 33429
		public Sprite icon;

		// Token: 0x04008296 RID: 33430
		[Tooltip("Category determines which category button in the user's wardrobe (which are the two rows of buttons with equivalent names) have to be pressed to access the cosmetic along with others in the same category.")]
		public StringEnum<CosmeticsController.CosmeticCategory> category;

		// Token: 0x04008297 RID: 33431
		[Obsolete("(2024-08-13 MattO) Will be removed after holdables array is fully implemented. Check length of `holdableParts` instead.")]
		[HideInInspector]
		public bool isHoldable;

		// Token: 0x04008298 RID: 33432
		public bool isThrowable;

		// Token: 0x04008299 RID: 33433
		[HideInInspector]
		public int[] throwableMaterialGrabIndices;

		// Token: 0x0400829A RID: 33434
		[HideInInspector]
		public int throwableIndex;

		// Token: 0x0400829B RID: 33435
		public bool usesBothHandSlots;

		// Token: 0x0400829C RID: 33436
		public bool hideWardrobeMannequin;

		// Token: 0x0400829D RID: 33437
		public const string holdableParts_infoBoxShortMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).";

		// Token: 0x0400829E RID: 33438
		public const string holdableParts_infoBoxDetailedMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components. The prefab asset's transform will be moved between the listed \n attach points on \"Gorilla Player Networked.prefab\" when grabbed by the player \n";

		// Token: 0x0400829F RID: 33439
		[Space]
		[Tooltip("\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components. The prefab asset's transform will be moved between the listed \n attach points on \"Gorilla Player Networked.prefab\" when grabbed by the player \n")]
		public CosmeticPart[] holdableParts;

		// Token: 0x040082A0 RID: 33440
		public const string functionalParts_infoBoxShortMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.";

		// Token: 0x040082A1 RID: 33441
		public const string functionalParts_infoBoxDetailedMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nIf a \"First Person View\" part or \"Local Rig Part\" is set it will be enabled instead of the wearable parts for the local player";

		// Token: 0x040082A2 RID: 33442
		[Space]
		[Tooltip("\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nIf a \"First Person View\" part or \"Local Rig Part\" is set it will be enabled instead of the wearable parts for the local player")]
		public CosmeticPart[] functionalParts;

		// Token: 0x040082A3 RID: 33443
		public const string wardrobeParts_infoBoxShortMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.";

		// Token: 0x040082A4 RID: 33444
		public const string wardrobeParts_infoBoxDetailedMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\nThese prefabs are shown on the satellite wardrobe, and in the store (if \"Store Parts\" is left empty)";

		// Token: 0x040082A5 RID: 33445
		[Space]
		[Tooltip("\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\nThese prefabs are shown on the satellite wardrobe, and in the store (if \"Store Parts\" is left empty)")]
		public CosmeticPart[] wardrobeParts;

		// Token: 0x040082A6 RID: 33446
		public const string storeParts_infoBoxShortMsg = "\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.";

		// Token: 0x040082A7 RID: 33447
		public const string storeParts_infoBoxDetailedMsg = "\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.\nStore parts only need to be specified if the store display should be different than the wardrobe display";

		// Token: 0x040082A8 RID: 33448
		[Space]
		[Tooltip("\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.\nStore parts only need to be specified if the store display should be different than the wardrobe display")]
		public CosmeticPart[] storeParts;

		// Token: 0x040082A9 RID: 33449
		public const string firstPersonViewParts_infoBoxShortMsg = "\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player";

		// Token: 0x040082AA RID: 33450
		public const string firstPersonViewParts_infoBoxDetailedMsg = "\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player\nThese are used for any peripheral view meshes on the No Mirror layer, usually on HAT or FACE items";

		// Token: 0x040082AB RID: 33451
		[Space]
		[Tooltip("\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player\nThese are used for any peripheral view meshes on the No Mirror layer, usually on HAT or FACE items")]
		public CosmeticPart[] firstPersonViewParts;

		// Token: 0x040082AC RID: 33452
		public const string localRigParts_infoBoxShortMsg = "\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".";

		// Token: 0x040082AD RID: 33453
		public const string localRigParts_infoBoxDetailedMsg = "\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".\nThese objects can be used in addition to first person view parts.\nThese can be used for mirror view meshes (usually HAT or FACE items)\nAny item with GTPosRotConstraints should be parented to the rig and not the camera";

		// Token: 0x040082AE RID: 33454
		[Space]
		[Tooltip("\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".\nThese objects can be used in addition to first person view parts.\nThese can be used for mirror view meshes (usually HAT or FACE items)\nAny item with GTPosRotConstraints should be parented to the rig and not the camera")]
		public CosmeticPart[] localRigParts;

		// Token: 0x040082AF RID: 33455
		[Space]
		[Tooltip("When this cosmetic is equipped, these offsets will be applied to the other objects on the player that are likely to clip\nSHIRT items ususally offset the badge, nametag, and chest items\n PAW items usually offset the hunt computer and builder watch")]
		public CosmeticAnchorAntiIntersectOffsets anchorAntiIntersectOffsets;

		// Token: 0x040082B0 RID: 33456
		[Space]
		[Tooltip("TODO COMMENT")]
		public CosmeticSO[] setCosmetics;

		// Token: 0x040082B1 RID: 33457
		[Space]
		[Tooltip("For parent (collection) cosmetics: the slots that collectables snap into. Each entry defines the slot type and its local space offset from the cosmetic's root. Edit slot positions visually via the Cosmetic Editor Stage. The slot count is implicit from this array's length.")]
		public CosmeticCollectionSlotDefinition[] collectionSlots;

		// Token: 0x040082B2 RID: 33458
		[Tooltip("For parent (collection) cosmetics: when true only one collectable is visible at a time and the player can cycle through them. When false all slots are shown simultaneously")]
		public bool collectionIsCycling;

		// Token: 0x040082B3 RID: 33459
		[Tooltip("For parent (collection) cosmetics: when true each sub-item's Target Slot Index is respected and items snap to their declared slot. When false index values are ignored and sub-items fill slots in acquisition order. Uncheck this to quickly compare both layouts without touching sub-item SOs.")]
		public bool collectionUsesIndexTargeting;

		// Token: 0x040082B4 RID: 33460
		[Space]
		[Tooltip("For sub-item (collectable) cosmetics: the PlayFab ID of the parent cosmetic that must be owned before this sub-item can be purchased. Leave empty if this is not a sub-item.")]
		public string collectionParentPlayFabID;

		// Token: 0x040082B5 RID: 33461
		[Tooltip("For sub-item (collectable) cosmetics: the slot index (0-based) on the parent cosmeticSO that this sub-item occupies. Set to Any for interchangeable items (e.g. badges) that fill any available slot in acquisition order. Set to a specific index when this item must always go to a particular socket on the parent.")]
		public int collectionTargetSlotIndex;

		// Token: 0x040082B6 RID: 33462
		[Tooltip("PlayFab ID of the cosmetic to apply to a hit player via Cosmetic Swapper (e.g. chicken sword) tech. Distinct from this sub-item's own visual.")]
		public string appliedCosmeticPlayFabID;

		// Token: 0x040082B7 RID: 33463
		[NonSerialized]
		public string debugCosmeticSOName;
	}
}
