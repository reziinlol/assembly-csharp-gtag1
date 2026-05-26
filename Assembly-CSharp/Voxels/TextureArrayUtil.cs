using System;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020012EC RID: 4844
	public class TextureArrayUtil : MonoBehaviour
	{
		// Token: 0x17000B9D RID: 2973
		// (get) Token: 0x060078B3 RID: 30899 RVA: 0x0027B643 File Offset: 0x00279843
		private bool UnreadableTextureFound
		{
			get
			{
				return !this.TexturesReadable;
			}
		}

		// Token: 0x17000B9E RID: 2974
		// (get) Token: 0x060078B4 RID: 30900 RVA: 0x0027B650 File Offset: 0x00279850
		private bool TexturesReadable
		{
			get
			{
				foreach (TextureEntry textureEntry in this.textureEntries)
				{
					if (textureEntry.Diffuse == null || textureEntry.Normal == null)
					{
						return false;
					}
					if (!textureEntry.Diffuse.isReadable || !textureEntry.Normal.isReadable)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x04008BD5 RID: 35797
		public TextureEntry[] textureEntries;

		// Token: 0x04008BD6 RID: 35798
		public Texture2DArray diffuseArray;

		// Token: 0x04008BD7 RID: 35799
		public Texture2DArray normalArray;

		// Token: 0x04008BD8 RID: 35800
		public Material material;

		// Token: 0x04008BD9 RID: 35801
		public bool linearNormalMaps = true;

		// Token: 0x04008BDA RID: 35802
		public string diffuseName = "_Diffuse";

		// Token: 0x04008BDB RID: 35803
		public string normalName = "_Normal";
	}
}
