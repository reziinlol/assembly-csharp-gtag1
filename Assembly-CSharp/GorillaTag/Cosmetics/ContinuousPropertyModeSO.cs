using System;
using System.Linq;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001259 RID: 4697
	public class ContinuousPropertyModeSO : ScriptableObject
	{
		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x060075B5 RID: 30133 RVA: 0x00269166 File Offset: 0x00267366
		private string GetTestDescription
		{
			get
			{
				if (this.castData.Length == 0)
				{
					return "";
				}
				return "Sample Description: " + this.GetDescriptionForCast(this.castData[0].target);
			}
		}

		// Token: 0x060075B6 RID: 30134 RVA: 0x00269198 File Offset: 0x00267398
		public bool IsCastValid(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060075B7 RID: 30135 RVA: 0x002691D4 File Offset: 0x002673D4
		public ContinuousProperty.Cast GetClosestCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast))
				{
					return this.castData[i].target;
				}
			}
			return ContinuousProperty.Cast.Null;
		}

		// Token: 0x060075B8 RID: 30136 RVA: 0x00269220 File Offset: 0x00267420
		public ContinuousProperty.DataFlags GetFlagsForCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (this.castData[i].target == cast)
				{
					return this.castData[i].additionalFlags | this.flags;
				}
			}
			return this.flags;
		}

		// Token: 0x060075B9 RID: 30137 RVA: 0x00269274 File Offset: 0x00267474
		public ContinuousProperty.DataFlags GetFlagsForClosestCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast))
				{
					return this.castData[i].additionalFlags | this.flags;
				}
			}
			return this.flags;
		}

		// Token: 0x060075BA RID: 30138 RVA: 0x002692CC File Offset: 0x002674CC
		public string GetDescriptionForCast(ContinuousProperty.Cast cast)
		{
			for (int i = 0; i < this.castData.Length; i++)
			{
				if (ContinuousProperty.CastMatches(this.castData[i].target, cast) || this.castData.Length == 1)
				{
					if (!this.replaceDescription.IsNullOrEmpty())
					{
						return this.replaceDescription;
					}
					switch (this.descriptionStyle)
					{
					case ContinuousPropertyModeSO.DescriptionStyle.Continuous:
						return string.Concat(new string[]
						{
							"sets the ",
							this.castData[i].whatItSets,
							" on the ",
							this.castData[i].target.ToString(),
							" using the height of the curve at the provided time.",
							(" " + this.afterSentence).TrimEnd()
						});
					case ContinuousPropertyModeSO.DescriptionStyle.SingleThreshold:
						return this.castData[i].whatItSets + " the " + this.type.ToString() + " when entering the 'true' part of the range.";
					case ContinuousPropertyModeSO.DescriptionStyle.DualThreshold:
					{
						string[] array = this.castData[i].whatItSets.Split('|', StringSplitOptions.None);
						if (array.Length != 2)
						{
							return string.Format("Error! '{0}'s '{1}.{2}' does not have two string separated by '|'.", base.name, this.castData[i].target, "whatItSets");
						}
						return string.Concat(new string[]
						{
							array[0],
							" the ",
							this.castData[i].target.ToString(),
							" when entering the 'true' part of the range, ",
							array[1],
							" the ",
							this.castData[i].target.ToString(),
							" when entering the 'false' part of the range."
						});
					}
					}
				}
			}
			return "Invalid target\n\n" + this.ListValidCasts();
		}

		// Token: 0x060075BB RID: 30139 RVA: 0x002694BE File Offset: 0x002676BE
		public string ListValidCasts()
		{
			return "Valid targets: " + string.Join<ContinuousProperty.Cast>(", ", from x in this.castData
			select x.target);
		}

		// Token: 0x04008779 RID: 34681
		public ContinuousProperty.Type type;

		// Token: 0x0400877A RID: 34682
		public ContinuousProperty.DataFlags flags;

		// Token: 0x0400877B RID: 34683
		public ContinuousPropertyModeSO.CastData[] castData;

		// Token: 0x0400877C RID: 34684
		[Space]
		public ContinuousPropertyModeSO.DescriptionStyle descriptionStyle;

		// Token: 0x0400877D RID: 34685
		[TextArea]
		public string afterSentence;

		// Token: 0x0400877E RID: 34686
		[TextArea]
		public string replaceDescription;

		// Token: 0x0200125A RID: 4698
		[Serializable]
		public struct CastData
		{
			// Token: 0x0400877F RID: 34687
			public ContinuousProperty.Cast target;

			// Token: 0x04008780 RID: 34688
			public ContinuousProperty.DataFlags additionalFlags;

			// Token: 0x04008781 RID: 34689
			public string whatItSets;
		}

		// Token: 0x0200125B RID: 4699
		public enum DescriptionStyle
		{
			// Token: 0x04008783 RID: 34691
			Continuous,
			// Token: 0x04008784 RID: 34692
			SingleThreshold,
			// Token: 0x04008785 RID: 34693
			DualThreshold
		}
	}
}
