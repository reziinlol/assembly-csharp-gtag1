using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200138A RID: 5002
	public struct Aabb
	{
		// Token: 0x17000BEA RID: 3050
		// (get) Token: 0x06007DD6 RID: 32214 RVA: 0x002965B7 File Offset: 0x002947B7
		// (set) Token: 0x06007DD7 RID: 32215 RVA: 0x002965C4 File Offset: 0x002947C4
		public float MinX
		{
			get
			{
				return this.Min.x;
			}
			set
			{
				this.Min.x = value;
			}
		}

		// Token: 0x17000BEB RID: 3051
		// (get) Token: 0x06007DD8 RID: 32216 RVA: 0x002965D2 File Offset: 0x002947D2
		// (set) Token: 0x06007DD9 RID: 32217 RVA: 0x002965DF File Offset: 0x002947DF
		public float MinY
		{
			get
			{
				return this.Min.y;
			}
			set
			{
				this.Min.y = value;
			}
		}

		// Token: 0x17000BEC RID: 3052
		// (get) Token: 0x06007DDA RID: 32218 RVA: 0x002965ED File Offset: 0x002947ED
		// (set) Token: 0x06007DDB RID: 32219 RVA: 0x002965FA File Offset: 0x002947FA
		public float MinZ
		{
			get
			{
				return this.Min.z;
			}
			set
			{
				this.Min.z = value;
			}
		}

		// Token: 0x17000BED RID: 3053
		// (get) Token: 0x06007DDC RID: 32220 RVA: 0x00296608 File Offset: 0x00294808
		// (set) Token: 0x06007DDD RID: 32221 RVA: 0x00296615 File Offset: 0x00294815
		public float MaxX
		{
			get
			{
				return this.Max.x;
			}
			set
			{
				this.Max.x = value;
			}
		}

		// Token: 0x17000BEE RID: 3054
		// (get) Token: 0x06007DDE RID: 32222 RVA: 0x00296623 File Offset: 0x00294823
		// (set) Token: 0x06007DDF RID: 32223 RVA: 0x00296630 File Offset: 0x00294830
		public float MaxY
		{
			get
			{
				return this.Max.y;
			}
			set
			{
				this.Max.y = value;
			}
		}

		// Token: 0x17000BEF RID: 3055
		// (get) Token: 0x06007DE0 RID: 32224 RVA: 0x0029663E File Offset: 0x0029483E
		// (set) Token: 0x06007DE1 RID: 32225 RVA: 0x0029664B File Offset: 0x0029484B
		public float MaxZ
		{
			get
			{
				return this.Max.z;
			}
			set
			{
				this.Max.z = value;
			}
		}

		// Token: 0x17000BF0 RID: 3056
		// (get) Token: 0x06007DE2 RID: 32226 RVA: 0x00296659 File Offset: 0x00294859
		public Vector3 Center
		{
			get
			{
				return 0.5f * (this.Min + this.Max);
			}
		}

		// Token: 0x17000BF1 RID: 3057
		// (get) Token: 0x06007DE3 RID: 32227 RVA: 0x00296678 File Offset: 0x00294878
		public Vector3 Size
		{
			get
			{
				Vector3 vector = this.Max - this.Min;
				vector.x = Mathf.Max(0f, vector.x);
				vector.y = Mathf.Max(0f, vector.y);
				vector.z = Mathf.Max(0f, vector.z);
				return vector;
			}
		}

		// Token: 0x17000BF2 RID: 3058
		// (get) Token: 0x06007DE4 RID: 32228 RVA: 0x002966DD File Offset: 0x002948DD
		public static Aabb Empty
		{
			get
			{
				return new Aabb(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), new Vector3(float.MinValue, float.MinValue, float.MinValue));
			}
		}

		// Token: 0x06007DE5 RID: 32229 RVA: 0x0029670C File Offset: 0x0029490C
		public static Aabb FromPoint(Vector3 p)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(p);
			return empty;
		}

		// Token: 0x06007DE6 RID: 32230 RVA: 0x00296728 File Offset: 0x00294928
		public static Aabb FromPoints(Vector3 a, Vector3 b)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(a);
			empty.Include(b);
			return empty;
		}

		// Token: 0x06007DE7 RID: 32231 RVA: 0x0029674C File Offset: 0x0029494C
		public Aabb(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x06007DE8 RID: 32232 RVA: 0x0029675C File Offset: 0x0029495C
		public void Include(Vector3 p)
		{
			this.MinX = Mathf.Min(this.MinX, p.x);
			this.MinY = Mathf.Min(this.MinY, p.y);
			this.MinZ = Mathf.Min(this.MinZ, p.z);
			this.MaxX = Mathf.Max(this.MaxX, p.x);
			this.MaxY = Mathf.Max(this.MaxY, p.y);
			this.MaxZ = Mathf.Max(this.MaxZ, p.z);
		}

		// Token: 0x06007DE9 RID: 32233 RVA: 0x002967F4 File Offset: 0x002949F4
		public bool Contains(Vector3 p)
		{
			return this.MinX <= p.x && this.MinY <= p.y && this.MinZ <= p.z && this.MaxX >= p.x && this.MaxY >= p.y && this.MaxZ >= p.z;
		}

		// Token: 0x06007DEA RID: 32234 RVA: 0x0029685A File Offset: 0x00294A5A
		public bool ContainsX(Vector3 p)
		{
			return this.MinX <= p.x && this.MaxX >= p.x;
		}

		// Token: 0x06007DEB RID: 32235 RVA: 0x0029687D File Offset: 0x00294A7D
		public bool ContainsY(Vector3 p)
		{
			return this.MinY <= p.y && this.MaxY >= p.y;
		}

		// Token: 0x06007DEC RID: 32236 RVA: 0x002968A0 File Offset: 0x00294AA0
		public bool ContainsZ(Vector3 p)
		{
			return this.MinZ <= p.z && this.MaxZ >= p.z;
		}

		// Token: 0x06007DED RID: 32237 RVA: 0x002968C4 File Offset: 0x00294AC4
		public bool Intersects(Aabb rhs)
		{
			return this.MinX <= rhs.MaxX && this.MinY <= rhs.MaxY && this.MinZ <= rhs.MaxZ && this.MaxX >= rhs.MinX && this.MaxY >= rhs.MinY && this.MaxZ >= rhs.MinZ;
		}

		// Token: 0x06007DEE RID: 32238 RVA: 0x00296930 File Offset: 0x00294B30
		public bool Intersects(ref BoingEffector.Params effector)
		{
			if (!effector.Bits.IsBitSet(0))
			{
				return this.Intersects(Aabb.FromPoint(effector.CurrPosition).Expand(effector.Radius));
			}
			return this.Intersects(Aabb.FromPoints(effector.PrevPosition, effector.CurrPosition).Expand(effector.Radius));
		}

		// Token: 0x06007DEF RID: 32239 RVA: 0x00296990 File Offset: 0x00294B90
		public Aabb Expand(float amount)
		{
			this.MinX -= amount;
			this.MinY -= amount;
			this.MinZ -= amount;
			this.MaxX += amount;
			this.MaxY += amount;
			this.MaxZ += amount;
			return this;
		}

		// Token: 0x04008F57 RID: 36695
		public Vector3 Min;

		// Token: 0x04008F58 RID: 36696
		public Vector3 Max;
	}
}
