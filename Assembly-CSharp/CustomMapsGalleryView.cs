using System;
using System.Collections.Generic;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio.Mods;
using PlayFab;
using UnityEngine;

// Token: 0x02000A8D RID: 2701
public class CustomMapsGalleryView : MonoBehaviour
{
	// Token: 0x06004500 RID: 17664 RVA: 0x00174C44 File Offset: 0x00172E44
	public void ResetGallery()
	{
		for (int i = 0; i < this.modTiles.Count; i++)
		{
			this.modTiles[i].DeactivateTile();
		}
	}

	// Token: 0x06004501 RID: 17665 RVA: 0x00174C78 File Offset: 0x00172E78
	public bool DisplayGallery(List<Mod> mods, bool useMapName, out string error)
	{
		if (mods.Count > this.modTiles.Count)
		{
			GTDev.LogError<string>("Displayed Mod list is longer than the number of mod tiles in the gallery", null);
			error = "Displayed Mod list is longer than the number of mod tiles in the gallery";
			return false;
		}
		Dictionary<Mod, Action<string>> dictionary = new Dictionary<Mod, Action<string>>();
		for (int i = 0; i < mods.Count; i++)
		{
			this.modTiles[i].SetMod(mods[i], useMapName);
			int idx = i;
			dictionary[mods[idx]] = delegate(string count)
			{
				this.modTiles[idx].PlayerCountText = count;
			};
		}
		this._synchronizer.SendRequest(dictionary, null);
		error = string.Empty;
		return true;
	}

	// Token: 0x06004502 RID: 17666 RVA: 0x00174D24 File Offset: 0x00172F24
	public void ShowTileText(bool show, bool useMapName)
	{
		for (int i = 0; i < this.modTiles.Count; i++)
		{
			this.modTiles[i].ShowTileText(show, useMapName);
		}
	}

	// Token: 0x06004503 RID: 17667 RVA: 0x00174D5A File Offset: 0x00172F5A
	public void ShowDetailsForEntry(int entryIndex)
	{
		if (this.modTiles.Count > entryIndex)
		{
			this.modTiles[entryIndex].ShowDetails();
		}
	}

	// Token: 0x06004504 RID: 17668 RVA: 0x00174D7B File Offset: 0x00172F7B
	public void HighlightTileAtIndex(int tileIndex)
	{
		if (tileIndex > this.modTiles.Count)
		{
			return;
		}
		this.modTiles[tileIndex].HighlightTile();
	}

	// Token: 0x04005740 RID: 22336
	[SerializeField]
	private List<CustomMapsModTile> modTiles = new List<CustomMapsModTile>();

	// Token: 0x04005741 RID: 22337
	private readonly CustomMapsGalleryView.RequestSynchronizer _synchronizer = new CustomMapsGalleryView.RequestSynchronizer();

	// Token: 0x02000A8E RID: 2702
	private class RequestSynchronizer
	{
		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x06004506 RID: 17670 RVA: 0x00174DBB File Offset: 0x00172FBB
		// (set) Token: 0x06004507 RID: 17671 RVA: 0x00174DC3 File Offset: 0x00172FC3
		public int LatestRequest { get; private set; } = -1;

		// Token: 0x06004508 RID: 17672 RVA: 0x00174DCC File Offset: 0x00172FCC
		public void SendRequest(IDictionary<Mod, Action<string>> modsAndCallbacks, Action<PlayFabError> errorCallback = null)
		{
			int latestRequest = this.LatestRequest + 1;
			this.LatestRequest = latestRequest;
			new CustomMapsGalleryView.SynchronizedRequest(this, this.LatestRequest, modsAndCallbacks, errorCallback).Send();
		}
	}

	// Token: 0x02000A8F RID: 2703
	private class SynchronizedRequest
	{
		// Token: 0x0600450A RID: 17674 RVA: 0x00174E0B File Offset: 0x0017300B
		public SynchronizedRequest(CustomMapsGalleryView.RequestSynchronizer parent, int id, IDictionary<Mod, Action<string>> modsAndCallbacks, Action<PlayFabError> errorCallback)
		{
			this._parent = parent;
			this._id = id;
			this._modsAndCallbacks = modsAndCallbacks;
			this._errorCallback = errorCallback;
		}

		// Token: 0x0600450B RID: 17675 RVA: 0x00174E30 File Offset: 0x00173030
		public void Send()
		{
			Dictionary<Mod, Action<string>> dictionary = new Dictionary<Mod, Action<string>>();
			foreach (Mod key in this._modsAndCallbacks.Keys)
			{
				dictionary[key] = this.WrapCallback(this._modsAndCallbacks[key]);
			}
			PlayerCountHelper.GetPlayerCountBatched(dictionary, this._errorCallback);
		}

		// Token: 0x0600450C RID: 17676 RVA: 0x00174EA8 File Offset: 0x001730A8
		private Action<string> WrapCallback(Action<string> source)
		{
			return delegate(string result)
			{
				if (this._id != this._parent.LatestRequest)
				{
					return;
				}
				source(result);
			};
		}

		// Token: 0x04005743 RID: 22339
		private readonly CustomMapsGalleryView.RequestSynchronizer _parent;

		// Token: 0x04005744 RID: 22340
		private readonly int _id;

		// Token: 0x04005745 RID: 22341
		private readonly IDictionary<Mod, Action<string>> _modsAndCallbacks;

		// Token: 0x04005746 RID: 22342
		private readonly Action<PlayFabError> _errorCallback;
	}
}
