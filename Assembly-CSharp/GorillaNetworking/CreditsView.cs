using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001045 RID: 4165
	public class CreditsView : MonoBehaviour
	{
		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x06006829 RID: 26665 RVA: 0x00219C9E File Offset: 0x00217E9E
		private int TotalPages
		{
			get
			{
				return this.creditsSections.Sum((CreditsSection section) => this.PagesPerSection(section));
			}
		}

		// Token: 0x0600682A RID: 26666 RVA: 0x00219CB8 File Offset: 0x00217EB8
		private void Start()
		{
			this.creditsSections = new CreditsSection[]
			{
				new CreditsSection
				{
					Title = "DEV TEAM",
					Entries = new List<string>
					{
						"Anton \"NtsFranz\" Franzluebbers",
						"Carlo Grossi Jr",
						"Cody O'Quinn",
						"David Neubelt",
						"David \"AA_DavidY\" Yee",
						"Derek \"DunkTrain\" Arabian",
						"Elie Arabian",
						"John Sleeper",
						"Haunted Army",
						"Kerestell Smith",
						"Keith \"ElectronicWall\" Taylor",
						"Laura \"Poppy\" Lorian",
						"Lilly Tothill",
						"Matt \"Crimity\" Ostgard",
						"Nick Taylor",
						"Ross Furmidge",
						"Sasha \"Kayze\" Sanders"
					}
				},
				new CreditsSection
				{
					Title = "SPECIAL THANKS",
					Entries = new List<string>
					{
						"The \"Sticks\"",
						"Alpha Squad",
						"Meta",
						"Scout House",
						"Mighty PR",
						"Caroline Arabian",
						"Clarissa & Declan",
						"Calum Haigh",
						"EZ ICE",
						"Gwen"
					}
				},
				new CreditsSection
				{
					Title = "MUSIC BY",
					Entries = new List<string>
					{
						"Stunshine",
						"David Anderson Kirk",
						"Jaguar Jen",
						"Audiopfeil",
						"Owlobe"
					}
				}
			};
			PlayFabTitleDataCache.Instance.GetTitleData("CreditsData", delegate(string result)
			{
				this.creditsSections = JsonMapper.ToObject<CreditsSection[]>(result);
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error fetching credits data: " + error.ErrorMessage);
			}, false);
		}

		// Token: 0x0600682B RID: 26667 RVA: 0x00219EC6 File Offset: 0x002180C6
		private int PagesPerSection(CreditsSection section)
		{
			return (int)Math.Ceiling((double)section.Entries.Count / (double)this.pageSize);
		}

		// Token: 0x0600682C RID: 26668 RVA: 0x00219EE2 File Offset: 0x002180E2
		private IEnumerable<string> PageOfSection(CreditsSection section, int page)
		{
			return section.Entries.Skip(this.pageSize * page).Take(this.pageSize);
		}

		// Token: 0x0600682D RID: 26669 RVA: 0x00219F04 File Offset: 0x00218104
		[return: TupleElementNames(new string[]
		{
			"creditsSection",
			"subPage"
		})]
		private ValueTuple<CreditsSection, int> GetPageEntries(int page)
		{
			int num = 0;
			foreach (CreditsSection creditsSection in this.creditsSections)
			{
				int num2 = this.PagesPerSection(creditsSection);
				if (num + num2 > page)
				{
					int item = page - num;
					return new ValueTuple<CreditsSection, int>(creditsSection, item);
				}
				num += num2;
			}
			return new ValueTuple<CreditsSection, int>(this.creditsSections.First<CreditsSection>(), 0);
		}

		// Token: 0x0600682E RID: 26670 RVA: 0x00219F60 File Offset: 0x00218160
		public void ProcessButtonPress(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.currentPage++;
				this.currentPage %= this.TotalPages;
			}
		}

		// Token: 0x0600682F RID: 26671 RVA: 0x00219F88 File Offset: 0x00218188
		public string GetScreenText()
		{
			return this.GetPage(this.currentPage);
		}

		// Token: 0x06006830 RID: 26672 RVA: 0x00219F98 File Offset: 0x00218198
		private string GetPage(int page)
		{
			ValueTuple<CreditsSection, int> pageEntries = this.GetPageEntries(page);
			CreditsSection item = pageEntries.Item1;
			int item2 = pageEntries.Item2;
			IEnumerable<string> enumerable = this.PageOfSection(item, item2);
			string defaultResult = "CREDITS";
			string str;
			LocalisationManager.TryGetKeyForCurrentLocale("CREDITS", out str, defaultResult);
			defaultResult = "(CONT)";
			string str2;
			LocalisationManager.TryGetKeyForCurrentLocale("CREDITS_CONTINUED", out str2, defaultResult);
			string value = str + " - " + ((item2 == 0) ? item.Title : (item.Title + " " + str2));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(value);
			stringBuilder.AppendLine();
			foreach (string value2 in enumerable)
			{
				stringBuilder.AppendLine(value2);
			}
			for (int i = 0; i < this.pageSize - enumerable.Count<string>(); i++)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			defaultResult = "PRESS ENTER TO CHANGE PAGES";
			string value3;
			LocalisationManager.TryGetKeyForCurrentLocale("CREDITS_PRESS_ENTER", out value3, defaultResult);
			stringBuilder.AppendLine(value3);
			return stringBuilder.ToString();
		}

		// Token: 0x0400777E RID: 30590
		private const string CREDITS_KEY = "CREDITS";

		// Token: 0x0400777F RID: 30591
		private const string CREDITS_PRESS_ENTER_KEY = "CREDITS_PRESS_ENTER";

		// Token: 0x04007780 RID: 30592
		private const string CREDITS_CONTINUED_KEY = "CREDITS_CONTINUED";

		// Token: 0x04007781 RID: 30593
		private CreditsSection[] creditsSections;

		// Token: 0x04007782 RID: 30594
		public int pageSize = 7;

		// Token: 0x04007783 RID: 30595
		private int currentPage;

		// Token: 0x04007784 RID: 30596
		private const string PlayFabKey = "CreditsData";
	}
}
