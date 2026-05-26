using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000E41 RID: 3649
	public class IAPurchase
	{
		// Token: 0x060058B4 RID: 22708 RVA: 0x001CB65B File Offset: 0x001C985B
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x060058B5 RID: 22709 RVA: 0x001CB669 File Offset: 0x001C9869
		public static void IsReady(IAPurchase.IAPurchaseListener listener, string pchAppKey)
		{
			IAPurchase.isReadyIl2cppCallback = new IAPurchase.IAPHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.IsReady_64(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
				return;
			}
			IAPurchase.IsReady(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
		}

		// Token: 0x060058B6 RID: 22710 RVA: 0x001CB6A8 File Offset: 0x001C98A8
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request01Il2cppCallback(errorCode, message);
		}

		// Token: 0x060058B7 RID: 22711 RVA: 0x001CB6B6 File Offset: 0x001C98B6
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice)
		{
			IAPurchase.request01Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
		}

		// Token: 0x060058B8 RID: 22712 RVA: 0x001CB6F5 File Offset: 0x001C98F5
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request02Il2cppCallback(errorCode, message);
		}

		// Token: 0x060058B9 RID: 22713 RVA: 0x001CB704 File Offset: 0x001C9904
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchUserData)
		{
			IAPurchase.request02Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
		}

		// Token: 0x060058BA RID: 22714 RVA: 0x001CB750 File Offset: 0x001C9950
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void PurchaseIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.purchaseIl2cppCallback(errorCode, message);
		}

		// Token: 0x060058BB RID: 22715 RVA: 0x001CB75E File Offset: 0x001C995E
		public static void Purchase(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.purchaseIl2cppCallback = new IAPurchase.IAPHandler(listener).getPurchaseHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Purchase_64(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Purchase(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
		}

		// Token: 0x060058BC RID: 22716 RVA: 0x001CB79D File Offset: 0x001C999D
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query01Il2cppCallback(errorCode, message);
		}

		// Token: 0x060058BD RID: 22717 RVA: 0x001CB7AB File Offset: 0x001C99AB
		public static void Query(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.query01Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
		}

		// Token: 0x060058BE RID: 22718 RVA: 0x001CB7EA File Offset: 0x001C99EA
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query02Il2cppCallback(errorCode, message);
		}

		// Token: 0x060058BF RID: 22719 RVA: 0x001CB7F8 File Offset: 0x001C99F8
		public static void Query(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.query02Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
		}

		// Token: 0x060058C0 RID: 22720 RVA: 0x001CB835 File Offset: 0x001C9A35
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void GetBalanceIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.getBalanceIl2cppCallback(errorCode, message);
		}

		// Token: 0x060058C1 RID: 22721 RVA: 0x001CB843 File Offset: 0x001C9A43
		public static void GetBalance(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.getBalanceIl2cppCallback = new IAPurchase.IAPHandler(listener).getBalanceHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.GetBalance_64(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
				return;
			}
			IAPurchase.GetBalance(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
		}

		// Token: 0x060058C2 RID: 22722 RVA: 0x001CB880 File Offset: 0x001C9A80
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x060058C3 RID: 22723 RVA: 0x001CB890 File Offset: 0x001C9A90
		public static void RequestSubscription(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId)
		{
			IAPurchase.requestSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscription_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
				return;
			}
			IAPurchase.RequestSubscription(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
		}

		// Token: 0x060058C4 RID: 22724 RVA: 0x001CB8EE File Offset: 0x001C9AEE
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionWithPlanIDIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback(errorCode, message);
		}

		// Token: 0x060058C5 RID: 22725 RVA: 0x001CB8FC File Offset: 0x001C9AFC
		public static void RequestSubscriptionWithPlanID(IAPurchase.IAPurchaseListener listener, string pchPlanId)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionWithPlanIDHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscriptionWithPlanID_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
				return;
			}
			IAPurchase.RequestSubscriptionWithPlanID(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
		}

		// Token: 0x060058C6 RID: 22726 RVA: 0x001CB93B File Offset: 0x001C9B3B
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void SubscribeIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.subscribeIl2cppCallback(errorCode, message);
		}

		// Token: 0x060058C7 RID: 22727 RVA: 0x001CB949 File Offset: 0x001C9B49
		public static void Subscribe(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.subscribeIl2cppCallback = new IAPurchase.IAPHandler(listener).getSubscribeHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Subscribe_64(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.Subscribe(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x060058C8 RID: 22728 RVA: 0x001CB988 File Offset: 0x001C9B88
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x060058C9 RID: 22729 RVA: 0x001CB996 File Offset: 0x001C9B96
		public static void QuerySubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.querySubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscription_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.QuerySubscription(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x060058CA RID: 22730 RVA: 0x001CB9D5 File Offset: 0x001C9BD5
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionListIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionListIl2cppCallback(errorCode, message);
		}

		// Token: 0x060058CB RID: 22731 RVA: 0x001CB9E3 File Offset: 0x001C9BE3
		public static void QuerySubscriptionList(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.querySubscriptionListIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscriptionList_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
				return;
			}
			IAPurchase.QuerySubscriptionList(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
		}

		// Token: 0x060058CC RID: 22732 RVA: 0x001CBA20 File Offset: 0x001C9C20
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void CancelSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x060058CD RID: 22733 RVA: 0x001CBA2E File Offset: 0x001C9C2E
		public static void CancelSubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getCancelSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.CancelSubscription_64(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.CancelSubscription(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x04006933 RID: 26931
		private static IAPurchaseCallback isReadyIl2cppCallback;

		// Token: 0x04006934 RID: 26932
		private static IAPurchaseCallback request01Il2cppCallback;

		// Token: 0x04006935 RID: 26933
		private static IAPurchaseCallback request02Il2cppCallback;

		// Token: 0x04006936 RID: 26934
		private static IAPurchaseCallback purchaseIl2cppCallback;

		// Token: 0x04006937 RID: 26935
		private static IAPurchaseCallback query01Il2cppCallback;

		// Token: 0x04006938 RID: 26936
		private static IAPurchaseCallback query02Il2cppCallback;

		// Token: 0x04006939 RID: 26937
		private static IAPurchaseCallback getBalanceIl2cppCallback;

		// Token: 0x0400693A RID: 26938
		private static IAPurchaseCallback requestSubscriptionIl2cppCallback;

		// Token: 0x0400693B RID: 26939
		private static IAPurchaseCallback requestSubscriptionWithPlanIDIl2cppCallback;

		// Token: 0x0400693C RID: 26940
		private static IAPurchaseCallback subscribeIl2cppCallback;

		// Token: 0x0400693D RID: 26941
		private static IAPurchaseCallback querySubscriptionIl2cppCallback;

		// Token: 0x0400693E RID: 26942
		private static IAPurchaseCallback querySubscriptionListIl2cppCallback;

		// Token: 0x0400693F RID: 26943
		private static IAPurchaseCallback cancelSubscriptionIl2cppCallback;

		// Token: 0x02000E42 RID: 3650
		private class IAPHandler : IAPurchase.BaseHandler
		{
			// Token: 0x060058CF RID: 22735 RVA: 0x001CBA6D File Offset: 0x001C9C6D
			public IAPHandler(IAPurchase.IAPurchaseListener cb)
			{
				IAPurchase.IAPHandler.listener = cb;
			}

			// Token: 0x060058D0 RID: 22736 RVA: 0x001CBA7B File Offset: 0x001C9C7B
			public IAPurchaseCallback getIsReadyHandler()
			{
				return new IAPurchaseCallback(this.IsReadyHandler);
			}

			// Token: 0x060058D1 RID: 22737 RVA: 0x001CBA8C File Offset: 0x001C9C8C
			protected override void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[IsReadyHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[IsReadyHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["currencyName"];
						}
						catch (Exception ex3)
						{
							string str2 = "[IsReadyHandler] currencyName ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] currencyName=" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058D2 RID: 22738 RVA: 0x001CBBBC File Offset: 0x001C9DBC
			public IAPurchaseCallback getRequestHandler()
			{
				return new IAPurchaseCallback(this.RequestHandler);
			}

			// Token: 0x060058D3 RID: 22739 RVA: 0x001CBBCC File Offset: 0x001C9DCC
			protected override void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[RequestHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[RequestHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
						}
						catch (Exception ex3)
						{
							string str2 = "[RequestHandler] purchase_id ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[RequestHandler] purchaseId =" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058D4 RID: 22740 RVA: 0x001CBCFC File Offset: 0x001C9EFC
			public IAPurchaseCallback getPurchaseHandler()
			{
				return new IAPurchaseCallback(this.PurchaseHandler);
			}

			// Token: 0x060058D5 RID: 22741 RVA: 0x001CBD0C File Offset: 0x001C9F0C
			protected override void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[PurchaseHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				long num2 = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[PurchaseHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[PurchaseHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							num2 = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string str2 = "[PurchaseHandler] purchase_id,paid_timestamp ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[PurchaseHandler] purchaseId =" + text + ",paid_timestamp=" + num2.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnPurchaseSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058D6 RID: 22742 RVA: 0x001CBE5C File Offset: 0x001CA05C
			public IAPurchaseCallback getQueryHandler()
			{
				return new IAPurchaseCallback(this.QueryHandler);
			}

			// Token: 0x060058D7 RID: 22743 RVA: 0x001CBE6C File Offset: 0x001CA06C
			protected override void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				string text6 = "";
				long paid_timestamp = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QueryHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							text3 = (string)jsonData["order_id"];
							text4 = (string)jsonData["status"];
							text5 = (string)jsonData["price"];
							text6 = (string)jsonData["currency"];
							paid_timestamp = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string str2 = "[QueryHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] status =",
							text4,
							",price=",
							text5,
							",currency=",
							text6
						}));
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] purchaseId =",
							text,
							",order_id=",
							text3,
							",paid_timestamp=",
							paid_timestamp.ToString()
						}));
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryResponse queryResponse = new IAPurchase.QueryResponse();
							queryResponse.purchase_id = text;
							queryResponse.order_id = text3;
							queryResponse.price = text5;
							queryResponse.currency = text6;
							queryResponse.paid_timestamp = paid_timestamp;
							queryResponse.status = text4;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058D8 RID: 22744 RVA: 0x001CC0B8 File Offset: 0x001CA2B8
			public IAPurchaseCallback getQueryListHandler()
			{
				return new IAPurchaseCallback(this.QueryListHandler);
			}

			// Token: 0x060058D9 RID: 22745 RVA: 0x001CC0C8 File Offset: 0x001CA2C8
			protected override void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				int total = 0;
				int from = 0;
				int to = 0;
				List<IAPurchase.QueryResponse2> list = new List<IAPurchase.QueryResponse2>();
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QueryListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							JsonData jsonData2 = JsonMapper.ToObject(text);
							total = (int)jsonData2["total"];
							from = (int)jsonData2["from"];
							to = (int)jsonData2["to"];
							JsonData jsonData3 = jsonData2["purchases"];
							bool isArray = jsonData3.IsArray;
							foreach (object obj in ((IEnumerable)jsonData3))
							{
								JsonData jsonData4 = (JsonData)obj;
								IAPurchase.QueryResponse2 queryResponse = new IAPurchase.QueryResponse2();
								IDictionary dictionary = jsonData4;
								queryResponse.app_id = (dictionary.Contains("app_id") ? ((string)jsonData4["app_id"]) : "");
								queryResponse.currency = (dictionary.Contains("currency") ? ((string)jsonData4["currency"]) : "");
								queryResponse.purchase_id = (dictionary.Contains("purchase_id") ? ((string)jsonData4["purchase_id"]) : "");
								queryResponse.order_id = (dictionary.Contains("order_id") ? ((string)jsonData4["order_id"]) : "");
								queryResponse.price = (dictionary.Contains("price") ? ((string)jsonData4["price"]) : "");
								queryResponse.user_data = (dictionary.Contains("user_data") ? ((string)jsonData4["user_data"]) : "");
								if (dictionary.Contains("paid_timestamp"))
								{
									if (jsonData4["paid_timestamp"].IsLong)
									{
										queryResponse.paid_timestamp = (long)jsonData4["paid_timestamp"];
									}
									else if (jsonData4["paid_timestamp"].IsInt)
									{
										queryResponse.paid_timestamp = (long)((int)jsonData4["paid_timestamp"]);
									}
								}
								list.Add(queryResponse);
							}
						}
						catch (Exception ex3)
						{
							string str2 = "[QueryListHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryListResponse queryListResponse = new IAPurchase.QueryListResponse();
							queryListResponse.total = total;
							queryListResponse.from = from;
							queryListResponse.to = to;
							queryListResponse.purchaseList = list;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryListResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058DA RID: 22746 RVA: 0x001CC450 File Offset: 0x001CA650
			public IAPurchaseCallback getBalanceHandler()
			{
				return new IAPurchaseCallback(this.BalanceHandler);
			}

			// Token: 0x060058DB RID: 22747 RVA: 0x001CC460 File Offset: 0x001CA660
			protected override void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[BalanceHandler] code=" + code.ToString() + ",message= " + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string str = "";
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str2 = "[BalanceHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[BalanceHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							str = (string)jsonData["currencyName"];
							text = (string)jsonData["balance"];
						}
						catch (Exception ex3)
						{
							string str3 = "[BalanceHandler] currencyName, balance ex=";
							Exception ex4 = ex3;
							Logger.Log(str3 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[BalanceHandler] currencyName=" + str + ",balance=" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnBalanceSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058DC RID: 22748 RVA: 0x001CC5BC File Offset: 0x001CA7BC
			public IAPurchaseCallback getRequestSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionHandler);
			}

			// Token: 0x060058DD RID: 22749 RVA: 0x001CC5CC File Offset: 0x001CA7CC
			protected override void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string str = "[RequestSubscriptionHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string str2 = "[RequestSubscriptionHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058DE RID: 22750 RVA: 0x001CC6F4 File Offset: 0x001CA8F4
			public IAPurchaseCallback getRequestSubscriptionWithPlanIDHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionWithPlanIDHandler);
			}

			// Token: 0x060058DF RID: 22751 RVA: 0x001CC704 File Offset: 0x001CA904
			protected override void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string str = "[RequestSubscriptionWithPlanIDHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string str2 = "[RequestSubscriptionWithPlanIDHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionWithPlanIDHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionWithPlanIDSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058E0 RID: 22752 RVA: 0x001CC82C File Offset: 0x001CAA2C
			public IAPurchaseCallback getSubscribeHandler()
			{
				return new IAPurchaseCallback(this.SubscribeHandler);
			}

			// Token: 0x060058E1 RID: 22753 RVA: 0x001CC83C File Offset: 0x001CAA3C
			protected override void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[SubscribeHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				long num2 = 0L;
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string str = "[SubscribeHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[SubscribeHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
						text3 = (string)jsonData["plan_id"];
						num2 = (long)jsonData["subscribed_timestamp"];
					}
					catch (Exception ex3)
					{
						string str2 = "[SubscribeHandler] subscription_id, plan_id ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log(string.Concat(new string[]
					{
						"[SubscribeHandler] subscription_id =",
						text,
						", plan_id=",
						text3,
						", timestamp=",
						num2.ToString()
					}));
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSubscribeSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058E2 RID: 22754 RVA: 0x001CC9C4 File Offset: 0x001CABC4
			public IAPurchaseCallback getQuerySubscriptionHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionHandler);
			}

			// Token: 0x060058E3 RID: 22755 RVA: 0x001CC9D4 File Offset: 0x001CABD4
			protected override void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QuerySubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string str2 = "[QuerySubscriptionHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058E4 RID: 22756 RVA: 0x001CCAFC File Offset: 0x001CACFC
			public IAPurchaseCallback getQuerySubscriptionListHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionListHandler);
			}

			// Token: 0x060058E5 RID: 22757 RVA: 0x001CCB0C File Offset: 0x001CAD0C
			protected override void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QuerySubscriptionListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string str2 = "[QuerySubscriptionListHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionListSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060058E6 RID: 22758 RVA: 0x001CCC34 File Offset: 0x001CAE34
			public IAPurchaseCallback getCancelSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.CancelSubscriptionHandler);
			}

			// Token: 0x060058E7 RID: 22759 RVA: 0x001CCC44 File Offset: 0x001CAE44
			protected override void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[CancelSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				bool bCanceled = false;
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[CancelSubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[CancelSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						bCanceled = true;
						Logger.Log("[CancelSubscriptionHandler] isCanceled = " + bCanceled.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnCancelSubscriptionSuccess(bCanceled);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x04006940 RID: 26944
			private static IAPurchase.IAPurchaseListener listener;
		}

		// Token: 0x02000E43 RID: 3651
		private abstract class BaseHandler
		{
			// Token: 0x060058E8 RID: 22760
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058E9 RID: 22761
			protected abstract void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058EA RID: 22762
			protected abstract void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058EB RID: 22763
			protected abstract void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058EC RID: 22764
			protected abstract void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058ED RID: 22765
			protected abstract void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058EE RID: 22766
			protected abstract void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058EF RID: 22767
			protected abstract void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058F0 RID: 22768
			protected abstract void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058F1 RID: 22769
			protected abstract void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058F2 RID: 22770
			protected abstract void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060058F3 RID: 22771
			protected abstract void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x02000E44 RID: 3652
		public class IAPurchaseListener
		{
			// Token: 0x060058F5 RID: 22773 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnSuccess(string pchCurrencyName)
			{
			}

			// Token: 0x060058F6 RID: 22774 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnRequestSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x060058F7 RID: 22775 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnPurchaseSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x060058F8 RID: 22776 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnQuerySuccess(IAPurchase.QueryResponse response)
			{
			}

			// Token: 0x060058F9 RID: 22777 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnQuerySuccess(IAPurchase.QueryListResponse response)
			{
			}

			// Token: 0x060058FA RID: 22778 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnBalanceSuccess(string pchBalance)
			{
			}

			// Token: 0x060058FB RID: 22779 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}

			// Token: 0x060058FC RID: 22780 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnRequestSubscriptionSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x060058FD RID: 22781 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnRequestSubscriptionWithPlanIDSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x060058FE RID: 22782 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnSubscribeSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x060058FF RID: 22783 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnQuerySubscriptionSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06005900 RID: 22784 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnQuerySubscriptionListSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06005901 RID: 22785 RVA: 0x000028C5 File Offset: 0x00000AC5
			public virtual void OnCancelSubscriptionSuccess(bool bCanceled)
			{
			}
		}

		// Token: 0x02000E45 RID: 3653
		public class QueryResponse
		{
			// Token: 0x17000863 RID: 2147
			// (get) Token: 0x06005903 RID: 22787 RVA: 0x001CCD34 File Offset: 0x001CAF34
			// (set) Token: 0x06005904 RID: 22788 RVA: 0x001CCD3C File Offset: 0x001CAF3C
			public string order_id { get; set; }

			// Token: 0x17000864 RID: 2148
			// (get) Token: 0x06005905 RID: 22789 RVA: 0x001CCD45 File Offset: 0x001CAF45
			// (set) Token: 0x06005906 RID: 22790 RVA: 0x001CCD4D File Offset: 0x001CAF4D
			public string purchase_id { get; set; }

			// Token: 0x17000865 RID: 2149
			// (get) Token: 0x06005907 RID: 22791 RVA: 0x001CCD56 File Offset: 0x001CAF56
			// (set) Token: 0x06005908 RID: 22792 RVA: 0x001CCD5E File Offset: 0x001CAF5E
			public string status { get; set; }

			// Token: 0x17000866 RID: 2150
			// (get) Token: 0x06005909 RID: 22793 RVA: 0x001CCD67 File Offset: 0x001CAF67
			// (set) Token: 0x0600590A RID: 22794 RVA: 0x001CCD6F File Offset: 0x001CAF6F
			public string price { get; set; }

			// Token: 0x17000867 RID: 2151
			// (get) Token: 0x0600590B RID: 22795 RVA: 0x001CCD78 File Offset: 0x001CAF78
			// (set) Token: 0x0600590C RID: 22796 RVA: 0x001CCD80 File Offset: 0x001CAF80
			public string currency { get; set; }

			// Token: 0x17000868 RID: 2152
			// (get) Token: 0x0600590D RID: 22797 RVA: 0x001CCD89 File Offset: 0x001CAF89
			// (set) Token: 0x0600590E RID: 22798 RVA: 0x001CCD91 File Offset: 0x001CAF91
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000E46 RID: 3654
		public class QueryResponse2
		{
			// Token: 0x17000869 RID: 2153
			// (get) Token: 0x06005910 RID: 22800 RVA: 0x001CCD9A File Offset: 0x001CAF9A
			// (set) Token: 0x06005911 RID: 22801 RVA: 0x001CCDA2 File Offset: 0x001CAFA2
			public string order_id { get; set; }

			// Token: 0x1700086A RID: 2154
			// (get) Token: 0x06005912 RID: 22802 RVA: 0x001CCDAB File Offset: 0x001CAFAB
			// (set) Token: 0x06005913 RID: 22803 RVA: 0x001CCDB3 File Offset: 0x001CAFB3
			public string app_id { get; set; }

			// Token: 0x1700086B RID: 2155
			// (get) Token: 0x06005914 RID: 22804 RVA: 0x001CCDBC File Offset: 0x001CAFBC
			// (set) Token: 0x06005915 RID: 22805 RVA: 0x001CCDC4 File Offset: 0x001CAFC4
			public string purchase_id { get; set; }

			// Token: 0x1700086C RID: 2156
			// (get) Token: 0x06005916 RID: 22806 RVA: 0x001CCDCD File Offset: 0x001CAFCD
			// (set) Token: 0x06005917 RID: 22807 RVA: 0x001CCDD5 File Offset: 0x001CAFD5
			public string user_data { get; set; }

			// Token: 0x1700086D RID: 2157
			// (get) Token: 0x06005918 RID: 22808 RVA: 0x001CCDDE File Offset: 0x001CAFDE
			// (set) Token: 0x06005919 RID: 22809 RVA: 0x001CCDE6 File Offset: 0x001CAFE6
			public string price { get; set; }

			// Token: 0x1700086E RID: 2158
			// (get) Token: 0x0600591A RID: 22810 RVA: 0x001CCDEF File Offset: 0x001CAFEF
			// (set) Token: 0x0600591B RID: 22811 RVA: 0x001CCDF7 File Offset: 0x001CAFF7
			public string currency { get; set; }

			// Token: 0x1700086F RID: 2159
			// (get) Token: 0x0600591C RID: 22812 RVA: 0x001CCE00 File Offset: 0x001CB000
			// (set) Token: 0x0600591D RID: 22813 RVA: 0x001CCE08 File Offset: 0x001CB008
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000E47 RID: 3655
		public class QueryListResponse
		{
			// Token: 0x17000870 RID: 2160
			// (get) Token: 0x0600591F RID: 22815 RVA: 0x001CCE11 File Offset: 0x001CB011
			// (set) Token: 0x06005920 RID: 22816 RVA: 0x001CCE19 File Offset: 0x001CB019
			public int total { get; set; }

			// Token: 0x17000871 RID: 2161
			// (get) Token: 0x06005921 RID: 22817 RVA: 0x001CCE22 File Offset: 0x001CB022
			// (set) Token: 0x06005922 RID: 22818 RVA: 0x001CCE2A File Offset: 0x001CB02A
			public int from { get; set; }

			// Token: 0x17000872 RID: 2162
			// (get) Token: 0x06005923 RID: 22819 RVA: 0x001CCE33 File Offset: 0x001CB033
			// (set) Token: 0x06005924 RID: 22820 RVA: 0x001CCE3B File Offset: 0x001CB03B
			public int to { get; set; }

			// Token: 0x04006951 RID: 26961
			public List<IAPurchase.QueryResponse2> purchaseList;
		}

		// Token: 0x02000E48 RID: 3656
		public class StatusDetailTransaction
		{
			// Token: 0x17000873 RID: 2163
			// (get) Token: 0x06005926 RID: 22822 RVA: 0x001CCE44 File Offset: 0x001CB044
			// (set) Token: 0x06005927 RID: 22823 RVA: 0x001CCE4C File Offset: 0x001CB04C
			public long create_time { get; set; }

			// Token: 0x17000874 RID: 2164
			// (get) Token: 0x06005928 RID: 22824 RVA: 0x001CCE55 File Offset: 0x001CB055
			// (set) Token: 0x06005929 RID: 22825 RVA: 0x001CCE5D File Offset: 0x001CB05D
			public string payment_method { get; set; }

			// Token: 0x17000875 RID: 2165
			// (get) Token: 0x0600592A RID: 22826 RVA: 0x001CCE66 File Offset: 0x001CB066
			// (set) Token: 0x0600592B RID: 22827 RVA: 0x001CCE6E File Offset: 0x001CB06E
			public string status { get; set; }
		}

		// Token: 0x02000E49 RID: 3657
		public class StatusDetail
		{
			// Token: 0x17000876 RID: 2166
			// (get) Token: 0x0600592D RID: 22829 RVA: 0x001CCE77 File Offset: 0x001CB077
			// (set) Token: 0x0600592E RID: 22830 RVA: 0x001CCE7F File Offset: 0x001CB07F
			public long date_next_charge { get; set; }

			// Token: 0x17000877 RID: 2167
			// (get) Token: 0x0600592F RID: 22831 RVA: 0x001CCE88 File Offset: 0x001CB088
			// (set) Token: 0x06005930 RID: 22832 RVA: 0x001CCE90 File Offset: 0x001CB090
			public IAPurchase.StatusDetailTransaction[] transactions { get; set; }

			// Token: 0x17000878 RID: 2168
			// (get) Token: 0x06005931 RID: 22833 RVA: 0x001CCE99 File Offset: 0x001CB099
			// (set) Token: 0x06005932 RID: 22834 RVA: 0x001CCEA1 File Offset: 0x001CB0A1
			public string cancel_reason { get; set; }
		}

		// Token: 0x02000E4A RID: 3658
		public class TimePeriod
		{
			// Token: 0x17000879 RID: 2169
			// (get) Token: 0x06005934 RID: 22836 RVA: 0x001CCEAA File Offset: 0x001CB0AA
			// (set) Token: 0x06005935 RID: 22837 RVA: 0x001CCEB2 File Offset: 0x001CB0B2
			public string time_type { get; set; }

			// Token: 0x1700087A RID: 2170
			// (get) Token: 0x06005936 RID: 22838 RVA: 0x001CCEBB File Offset: 0x001CB0BB
			// (set) Token: 0x06005937 RID: 22839 RVA: 0x001CCEC3 File Offset: 0x001CB0C3
			public int value { get; set; }
		}

		// Token: 0x02000E4B RID: 3659
		public class Subscription
		{
			// Token: 0x1700087B RID: 2171
			// (get) Token: 0x06005939 RID: 22841 RVA: 0x001CCECC File Offset: 0x001CB0CC
			// (set) Token: 0x0600593A RID: 22842 RVA: 0x001CCED4 File Offset: 0x001CB0D4
			public string app_id { get; set; }

			// Token: 0x1700087C RID: 2172
			// (get) Token: 0x0600593B RID: 22843 RVA: 0x001CCEDD File Offset: 0x001CB0DD
			// (set) Token: 0x0600593C RID: 22844 RVA: 0x001CCEE5 File Offset: 0x001CB0E5
			public string order_id { get; set; }

			// Token: 0x1700087D RID: 2173
			// (get) Token: 0x0600593D RID: 22845 RVA: 0x001CCEEE File Offset: 0x001CB0EE
			// (set) Token: 0x0600593E RID: 22846 RVA: 0x001CCEF6 File Offset: 0x001CB0F6
			public string subscription_id { get; set; }

			// Token: 0x1700087E RID: 2174
			// (get) Token: 0x0600593F RID: 22847 RVA: 0x001CCEFF File Offset: 0x001CB0FF
			// (set) Token: 0x06005940 RID: 22848 RVA: 0x001CCF07 File Offset: 0x001CB107
			public string price { get; set; }

			// Token: 0x1700087F RID: 2175
			// (get) Token: 0x06005941 RID: 22849 RVA: 0x001CCF10 File Offset: 0x001CB110
			// (set) Token: 0x06005942 RID: 22850 RVA: 0x001CCF18 File Offset: 0x001CB118
			public string currency { get; set; }

			// Token: 0x17000880 RID: 2176
			// (get) Token: 0x06005943 RID: 22851 RVA: 0x001CCF21 File Offset: 0x001CB121
			// (set) Token: 0x06005944 RID: 22852 RVA: 0x001CCF29 File Offset: 0x001CB129
			public long subscribed_timestamp { get; set; }

			// Token: 0x17000881 RID: 2177
			// (get) Token: 0x06005945 RID: 22853 RVA: 0x001CCF32 File Offset: 0x001CB132
			// (set) Token: 0x06005946 RID: 22854 RVA: 0x001CCF3A File Offset: 0x001CB13A
			public IAPurchase.TimePeriod free_trial_period { get; set; }

			// Token: 0x17000882 RID: 2178
			// (get) Token: 0x06005947 RID: 22855 RVA: 0x001CCF43 File Offset: 0x001CB143
			// (set) Token: 0x06005948 RID: 22856 RVA: 0x001CCF4B File Offset: 0x001CB14B
			public IAPurchase.TimePeriod charge_period { get; set; }

			// Token: 0x17000883 RID: 2179
			// (get) Token: 0x06005949 RID: 22857 RVA: 0x001CCF54 File Offset: 0x001CB154
			// (set) Token: 0x0600594A RID: 22858 RVA: 0x001CCF5C File Offset: 0x001CB15C
			public int number_of_charge_period { get; set; }

			// Token: 0x17000884 RID: 2180
			// (get) Token: 0x0600594B RID: 22859 RVA: 0x001CCF65 File Offset: 0x001CB165
			// (set) Token: 0x0600594C RID: 22860 RVA: 0x001CCF6D File Offset: 0x001CB16D
			public string plan_id { get; set; }

			// Token: 0x17000885 RID: 2181
			// (get) Token: 0x0600594D RID: 22861 RVA: 0x001CCF76 File Offset: 0x001CB176
			// (set) Token: 0x0600594E RID: 22862 RVA: 0x001CCF7E File Offset: 0x001CB17E
			public string plan_name { get; set; }

			// Token: 0x17000886 RID: 2182
			// (get) Token: 0x0600594F RID: 22863 RVA: 0x001CCF87 File Offset: 0x001CB187
			// (set) Token: 0x06005950 RID: 22864 RVA: 0x001CCF8F File Offset: 0x001CB18F
			public string status { get; set; }

			// Token: 0x17000887 RID: 2183
			// (get) Token: 0x06005951 RID: 22865 RVA: 0x001CCF98 File Offset: 0x001CB198
			// (set) Token: 0x06005952 RID: 22866 RVA: 0x001CCFA0 File Offset: 0x001CB1A0
			public IAPurchase.StatusDetail status_detail { get; set; }
		}

		// Token: 0x02000E4C RID: 3660
		public class QuerySubscritionResponse
		{
			// Token: 0x17000888 RID: 2184
			// (get) Token: 0x06005954 RID: 22868 RVA: 0x001CCFA9 File Offset: 0x001CB1A9
			// (set) Token: 0x06005955 RID: 22869 RVA: 0x001CCFB1 File Offset: 0x001CB1B1
			public int statusCode { get; set; }

			// Token: 0x17000889 RID: 2185
			// (get) Token: 0x06005956 RID: 22870 RVA: 0x001CCFBA File Offset: 0x001CB1BA
			// (set) Token: 0x06005957 RID: 22871 RVA: 0x001CCFC2 File Offset: 0x001CB1C2
			public string message { get; set; }

			// Token: 0x1700088A RID: 2186
			// (get) Token: 0x06005958 RID: 22872 RVA: 0x001CCFCB File Offset: 0x001CB1CB
			// (set) Token: 0x06005959 RID: 22873 RVA: 0x001CCFD3 File Offset: 0x001CB1D3
			public List<IAPurchase.Subscription> subscriptions { get; set; }
		}
	}
}
