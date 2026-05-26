using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001340 RID: 4928
	[ExecuteInEditMode]
	public class LatexFormula : MonoBehaviour
	{
		// Token: 0x04008D50 RID: 36176
		public static readonly string BaseUrl = "http://tex.s2cms.ru/svg/f(x) ";

		// Token: 0x04008D51 RID: 36177
		private int m_hash = LatexFormula.BaseUrl.GetHashCode();

		// Token: 0x04008D52 RID: 36178
		[SerializeField]
		private string m_formula = "";

		// Token: 0x04008D53 RID: 36179
		private Texture m_texture;
	}
}
