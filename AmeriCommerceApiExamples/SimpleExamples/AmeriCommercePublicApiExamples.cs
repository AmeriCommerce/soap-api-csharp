using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmeriCommerceApiExamples
{
	public class AmeriCommercePublicApiExamples
	{
		public static void Examples()
		{
			string sResults = "";

			ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
			ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();

			oHeader.UserName = Properties.Settings.Default.ApiUsername;
			oHeader.Password = Properties.Settings.Default.ApiPassword;
			oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;

			oWS.AmeriCommerceHeaderInfoValue = oHeader;
			oWS.Url = Properties.Settings.Default.AmeriCommerceApiExamples_ACDB_AmeriCommerceDatabaseIO;

			#region Getting a product by item number
			sResults = "";

			ACDB.ProductTrans oProduct = oWS.Product_GetByItemNumber("TS2PS");
			sResults = oProduct.itemName + " exists with ID of " + oProduct.itemID;
			#endregion

			#region Getting the variant inventory information for a product
			sResults = "";

			ACDB.ProductTrans oProduct2 = oWS.Product_GetByKey(2);	//2 is the itemId of a known product in this example
			oProduct2 = oWS.Product_FillProductVariantCollection(oProduct2);
			ArrayList aryVariants = new ArrayList(oProduct2.ProductVariantColTrans);
			oProduct2 = oWS.Product_FillVariantInventoryCollection(oProduct2);
			ArrayList aryVariantInventory = new ArrayList(oProduct2.VariantInventoryColTrans);
			sResults = oProduct2.itemName + " has " + aryVariants.Count + " variants.";

			sResults += "\r\n\r\nAnd has " + aryVariantInventory.Count + " variant inventory records.";
			#endregion

			#region Get orders for a particular date range and make a change to one of them
			sResults = "";

			ACDB.OrderTrans[] colOrders = oWS.Order_GetByDateRange(DateTime.Parse("9/10/2010"), DateTime.Parse("1/1/2011"));
			ArrayList aryOrders = new ArrayList(colOrders);
			ACDB.OrderTrans oOrder = (ACDB.OrderTrans)aryOrders[0];
			sResults = "Order Count is : " + aryOrders.Count;
			sResults += "\r\n\r\nOrder ID: " + oOrder.orderID.Value;
			sResults += "\r\n\r\nOrder Comments: " + oOrder.comments;

			oOrder.comments = "Test: " + DateTime.Now.Ticks;
			oWS.Order_SaveAndGet(oOrder);

			sResults += "\r\n\r\nAFTERSAVE: Order Comments: " + oOrder.comments;
			#endregion

			#region Get the status for a particular product
			sResults = "";

			ACDB.ProductTrans oProduct3 = new AmeriCommerceApiExamples.ACDB.ProductTrans();
			oProduct3 = oWS.Product_GetByKey(516);		//516 is the itemId of a known product in this sample
			ACDB.ProductStatusTrans oStatus = oWS.ProductStatus_GetByKey(oProduct3.productStatusID.Value);

			sResults = "Product Status is : " + oStatus.productStatus;
			#endregion

			#region Add a new customer
			sResults = "";

			ACDB.CustomerTrans oCustomer = new ACDB.CustomerTrans();
			oCustomer.IsNew = true;
			oCustomer.email = "test@test.com";
			oCustomer.firstName = "TestFirst";
			oCustomer.lastName = "TestLast";
			var oStoreID = new ACDB.DataInt32();
			oStoreID.Value = 1;
			oCustomer.storeID = oStoreID;
			oCustomer.Company = "Test Company";
			var dtLastVisitDate = new ACDB.DataDateTime();
			dtLastVisitDate.Value = DateTime.Now;
			oCustomer.lastVisitDate = dtLastVisitDate;
			var dtRegisteredDate = new ACDB.DataDateTime();
			dtRegisteredDate.Value = DateTime.Now;
			oCustomer.registeredDate = dtRegisteredDate;

			oCustomer = oWS.Customer_SaveAndGet(oCustomer);

			sResults = "Customer Returned and Saved: ID: " + oCustomer.customerID.Value;
			#endregion

			#region Get all orders for a particular date range pre-filled with all child information
			sResults = "";
			sResults += "Get Orders With All Information Filled\r\n";

			//this will populate child information on the order, such as items
			var oOrders = new System.Collections.ArrayList(oWS.Order_GetByDateRangePreFilled(DateTime.Parse("1/1/2008"), DateTime.Parse("1/1/2010")));

			sResults += "Orders Returned: " + oOrders.Count + "\r\n";
			foreach (ACDB.OrderTrans oOrderTemp in oOrders)
			{
				var aryOrderItems = new System.Collections.ArrayList(oOrderTemp.OrderItemColTrans);
				sResults += "ID: " + oOrder.orderID.Value + ": Total: " + oOrderTemp.total.Value + " ItemCount: " + aryOrderItems.Count + "\r\n";
				foreach (ACDB.OrderItemTrans oItem in aryOrderItems)
				{
					sResults += "     Qty:" + oItem.quantity.Value + " of " + oItem.itemNr + "[" + oItem.itemID.Value + "]" + " " + oItem.itemName + " at $" + oItem.price.Value + "\r\n";
				}
				var aryPayments = new System.Collections.ArrayList(oOrderTemp.OrderPaymentColTrans);
				sResults += "     Payment: ";
				foreach (ACDB.OrderPaymentTrans oPayment in aryPayments)
				{
					sResults += oPayment.Amount.Value + ", ";
				}
				sResults += "\r\n";

				if (oOrderTemp.OrderBillingAddressTrans != null)
					sResults += "     BillingAddress: " + oOrderTemp.OrderBillingAddressTrans.City + ", " + oOrderTemp.OrderBillingAddressTrans.StateTrans.stateCode + "[" + oOrderTemp.OrderBillingAddressTrans.StateID.Value + "] " + oOrderTemp.OrderBillingAddressTrans.ZipCode + "\r\n";
				if (oOrder.OrderShippingAddressTrans != null)
					sResults += "     ShippingAddress: " + oOrderTemp.OrderShippingAddressTrans.City + ", " + oOrderTemp.OrderShippingAddressTrans.StateTrans.stateCode + "[" + oOrderTemp.OrderShippingAddressTrans.StateID.Value + "[ " + oOrderTemp.OrderShippingAddressTrans.ZipCode + "\r\n";
			}
			#endregion

			#region Customer Single Signon Key
			//you will need to know the customer’s plaintext password to pass to GetCustomerSingleSignonKey
			//You will also need to know the customer's email address, and the store for which they are associated.
			string sCustomerPassword = "";
			string sCustomerEmail = "";
			int iStoreID = 0;

			//string sKey  = oWS.GetCustomerSingleSignonKey(sCustomerEmail, sCustomerPassword, iStoreID);
            string sKey = "";

			string sUrl = "http://yoururl.com/store/login.aspx?key=" + sKey;
			#endregion

		}
	}
}
