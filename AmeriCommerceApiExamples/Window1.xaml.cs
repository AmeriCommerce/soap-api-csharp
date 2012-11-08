using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AmeriCommerceApiExamples
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void btnGetSession_Click(object sender, RoutedEventArgs e)
        {
            ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
            ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();
            oWS.Url = Properties.Settings.Default.AmeriCommerceApiExamples_ACDB_AmeriCommerceDatabaseIO;

            oHeader.UserName = Properties.Settings.Default.ApiUsername;
            oHeader.Password = Properties.Settings.Default.ApiPassword;
            oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;

            oWS.AmeriCommerceHeaderInfoValue = oHeader;

            this.txtResults.Text = "";


            //Must start a user session and then keep track of this session on your own via a cookie or any accessible means on your codes side
            //this session corresponds to 1 visitor or 1 user session and will have tracking information, cart info, etc. associated with it
            //the session will be logged as a 'session visit' appropriately
            ACDB.SessionTrans oSession;
            if (this.chkNewSession.IsChecked.Value)
                oSession = oWS.Session_StartSession();
            else
                oSession = oWS.Session_GetByKey(1158);
            
            this.txtResults.Text += "SessionID: " + oSession.ID.Value + "\r\n";
            this.txtResults.Text += "SessionUID: " + oSession.UID + "\r\n";
            this.txtResults.Text += "SourceGroup: " + oSession.SourceGroup + "\r\n";
            this.txtResults.Text += "Source: " + oSession.Source + "\r\n";
            this.txtResults.Text += "UserAgent: " + oSession.userAgent + "\r\n";

            //GET A CART
            this.txtResults.Text += "\r\n";
            //will be 0 on a new session until you call getcart
            this.txtResults.Text += "CartInfoID: " + oSession.ActiveCartInfoID.Value + "\r\n";

            //Does the work of setting up a cart for the session passed in
            var oCartInfo = oWS.Cart_GetBySessionID(oSession.ID.Value);
            this.txtResults.Text += "CartInfoID: " + oCartInfo.CartInfoID.Value + "\r\n";

            //SEE WHATS IN CART - will not have any items in cart usually on a new session
            this.txtResults.Text += "Unique SKUs In Cart: " + oCartInfo.CartColTrans.Count() + "\r\n";

            //Add an item to the sessions cart by the item ID, telling it a quantity as well.
            this.txtResults.Text += "AddToCart Message: " + oWS.Cart_AddToCart_ByItem(oSession.ID.Value, 1, 1) + "\r\n";

            //NOTE: Can also perform these operations on cart
            //oWS.Cart_UpdateCartItem(oSession.ID.Value, 1234, 1); //updates an existing cart item with a new quantity
            //oWS.Cart_RemoveCartItem(oSession.ID.Value, 1234); //removes a specific item from the cart (using it's cartid)
            //oWS.Cart_ClearCart(oSession.ID.Value); //clears the cart entirely

			//NOTE: You also have the ability to add multiple items to the cart with a single call:
			//string[] sResponse =  oWS.Cart_AddToCart_ByItems(oSession.ID.Value, new int[] {1, 2}, new int[] {3, 4} //mass add to cart.
			//The expected parameters are an array of integers that contains each item ID you want to add to the cart,
			//and an additional array of integers that contains the corresponding quantity for the items you want to add
			//this example will add item IDs 1 and 2 to the cart, where item ID 1 has a quantity of 3 and item id 2 has a quantity of 4
			//the indices of the item IDs in the items array should match the indices of the corresponding quantity on the quantities array

			//The response will be an array of strings, each containing the corresponding success or failure message for each item that was added
			//For example, if the first item ID was added to the cart successfully, then element 0 of sResponse should contain "Ok"
			//if the second item ID was NOT added successfully then element 1 of sResponse should contain the error message.

			//If there is an error in the method before the code reaches the point of attempting to add the items to the cart, then sResponse will contain a 
			//single element which will be the error message.

			//You MUST specify both an items array and a quantities array, and the number of items in each array must be the same.  If not then the method
			//will fail without attempting to add any items to the cart.

            //TODO: Add Phantom Items
            //TODO: Add Variant Items
			//TODO: Add Multiple Variant Items with single call
            //TODO: Add Personalized Items
            //TODO: Add Group/Kit Items        


            //setup some tracking variables
            int iCartItemID = 0;
            int iCartItemQuantityForDisplay = 0;
            decimal dCartItemPriceForDisplay = 0m;


            //Get the updated cart now that it has an item in it, will have values and totals all calculated
            //along with any applicable discounts and rules in place
            oCartInfo = oWS.Cart_GetBySessionID(oSession.ID.Value);
            var aryCartItems = new System.Collections.ArrayList(oCartInfo.CartColTrans);
            foreach (ACDB.CartTrans oItem in aryCartItems)
            {
                if (oItem.itemID.Value == 1) //this is the ID of the product passed in that i'm looking for
                {
                    iCartItemID = oItem.ID.Value;
                    iCartItemQuantityForDisplay = oItem.quantity.Value;
                    dCartItemPriceForDisplay = oItem.price.Value;
                }
            }
            this.txtResults.Text += "Unique SKUs In Cart: " + oCartInfo.CartColTrans.Count() + "\r\n";
            this.txtResults.Text += "First Item's Quantity: " + iCartItemQuantityForDisplay + "\r\n";
            this.txtResults.Text += "First Item's Price: " + dCartItemPriceForDisplay + "\r\n";
            this.txtResults.Text += "SubTotal Total: " + oCartInfo.SubTotal.Value + "\r\n";
            this.txtResults.Text += "Discount Total: " + oCartInfo.DiscountTotal.Value + "\r\n";
            this.txtResults.Text += "Tax Total: " + oCartInfo.TaxTotal.Value + "\r\n";
            this.txtResults.Text += "Shipping Total: " + oCartInfo.ShippingTotal.Value + "\r\n";
            this.txtResults.Text += "Cart Total: " + oCartInfo.GrandTotal.Value + "\r\n";
      


            //READ:
            //At this point you can escape out of the API and go directly to the store's AmeriCommerce front end for
            //processing the order, viewing the cart, checking out, etc.
            //OPTIONALLY: You can continue via the API to cart products and process the order

            //ESCAPE POINT #1: UNCOMMENT
            //GO TO THE FRONT END OF SITE AND POPULATE THE SESSION/CART
            //setting z= will set the current session on any americommerce site to the session key passed in
            //System.Diagnostics.Process oProcess = new System.Diagnostics.Process();
            //oProcess.StartInfo.FileName = "http://shop.mybox.com/store/shopcart.aspx?z=" + oSession.UID;
            //oProcess.Start();


            //ESCAPE POINT #2: UNCOMMENT
            //DIRECT TO CHECKOUT
            //setting z= will set the current session on any americommerce site to the session key passed in
            //System.Diagnostics.Process oProcess = new System.Diagnostics.Process();
            //oProcess.StartInfo.FileName = "https://shop.mybox.com/store/OnePageCheckout.aspx?z=" + oSession.UID;
            //oProcess.Start();

            //You have passed on going to the front end so let's finish getting data for the order supplied to the cart and session.




            //Let's go ahead and update the cart, repull it and make sure it's all still there
            //you must use the CartItemID, not the items id...as an item can be in the cart more than once
            aryCartItems = new System.Collections.ArrayList(oCartInfo.CartColTrans);
            foreach (ACDB.CartTrans oItem in aryCartItems)
            {
                if (oItem.itemID.Value == 1) //this is the ID of the product passed in that i'm looking for
                {
                    iCartItemQuantityForDisplay = oItem.quantity.Value;
                    dCartItemPriceForDisplay = oItem.price.Value;
                }
            }

            //UPDATE THE QUANTITY TO 10
            this.txtResults.Text += "UpdateCartItem Message: " + oWS.Cart_UpdateCartItem(oSession.ID.Value, iCartItemID, 10);
            //CHECK RETURN VALUES
            oCartInfo = oWS.Cart_GetBySessionID(oSession.ID.Value);
            aryCartItems = new System.Collections.ArrayList(oCartInfo.CartColTrans);
            foreach (ACDB.CartTrans oItem in aryCartItems)
            {
                if (oItem.itemID.Value == 1)
                {
                    iCartItemQuantityForDisplay = oItem.quantity.Value;
                    dCartItemPriceForDisplay = oItem.price.Value;
                }
            }
            this.txtResults.Text += "Unique SKUs In Cart: " + oCartInfo.CartColTrans.Count() + "\r\n";
            this.txtResults.Text += "First Item's Quantity: " + iCartItemQuantityForDisplay + "\r\n";
            this.txtResults.Text += "First Item's Price: " + dCartItemPriceForDisplay + "\r\n";
            this.txtResults.Text += "SubTotal Total: " + oCartInfo.SubTotal.Value + "\r\n";
            this.txtResults.Text += "Discount Total: " + oCartInfo.DiscountTotal.Value + "\r\n";
            this.txtResults.Text += "Tax Total: " + oCartInfo.TaxTotal.Value + "\r\n";
            this.txtResults.Text += "Shipping Total: " + oCartInfo.ShippingTotal.Value + "\r\n";
            this.txtResults.Text += "Cart Total: " + oCartInfo.GrandTotal.Value + "\r\n";



            //UPDATE THE QUANTITY BACK TO 1
            this.txtResults.Text += "UpdateCartItem 2 Message: " + oWS.Cart_UpdateCartItem(oSession.ID.Value, iCartItemID, 1);
            //CHECK RETURN VALUES
            oCartInfo = oWS.Cart_GetBySessionID(oSession.ID.Value);
            aryCartItems = new System.Collections.ArrayList(oCartInfo.CartColTrans);
            foreach (ACDB.CartTrans oItem in aryCartItems)
            {
                if (oItem.itemID.Value == 1)
                {
                    iCartItemQuantityForDisplay = oItem.quantity.Value;
                    dCartItemPriceForDisplay = oItem.price.Value;
                }
            }
            this.txtResults.Text += "Unique SKUs In Cart: " + oCartInfo.CartColTrans.Count() + "\r\n";
            this.txtResults.Text += "First Item's Quantity: " + iCartItemQuantityForDisplay + "\r\n";
            this.txtResults.Text += "First Item's Price: " + dCartItemPriceForDisplay + "\r\n";
            this.txtResults.Text += "SubTotal Total: " + oCartInfo.SubTotal.Value + "\r\n";
            this.txtResults.Text += "Discount Total: " + oCartInfo.DiscountTotal.Value + "\r\n";
            this.txtResults.Text += "Tax Total: " + oCartInfo.TaxTotal.Value + "\r\n";
            this.txtResults.Text += "Shipping Total: " + oCartInfo.ShippingTotal.Value + "\r\n";
            this.txtResults.Text += "Cart Total: " + oCartInfo.GrandTotal.Value + "\r\n";




            //SHIPPING OPTIONS
            //You can directly apply shipping to the cart by saving the CartInfo transport back to the api with the changed value
            //or use the AmeriCommerce Shipping API and rules built into the AmeriCommerce admin console to return rates for choosing and setting on cart

            if(this.chkGetShipping.IsChecked.Value)
            {
                //RETURN SHIPPING QUOTES FOR ADDRESS
                this.txtResults.Text += "\r\n";
                this.txtResults.Text += "Get all available shipping rates:\r\n";
                var aryRates = new System.Collections.ArrayList();
                try
                {
                    //Can supply varying amounts of information, standard 2 digit country codes are required.  Can get from Country_GetAll.
                    aryRates = new System.Collections.ArrayList(oWS.Cart_GetShipping(oSession.ID.Value, "", "", "77707", "US"));
                    this.txtResults.Text += "ShippingRateCount: " + aryRates.Count + "\r\n";
                    foreach(ACDB.Rate oRate in aryRates)
                        this.txtResults.Text += "Rate: " + oRate.Description + " " + oRate.TotalCharges + "\r\n";                   
                }
                catch(System.Exception ex)
                {
                    this.txtResults.Text += ex.GetBaseException().Message + " " + ex.StackTrace;
                }

                //SHIPPING QUOTES COMPLETE, LETS TAKE ONE AND SAVE IT BACK TO CART AS A 'CHOICE'
                //Typically you will give the end customer the shipping options and they will choose one, this will be what you will do when the shipping is chosen to set it on the cart/order permanently
                if(aryRates != null)
                {
                    this.txtResults.Text += "\r\n";
                    this.txtResults.Text += "Set The Selected Shipping Rate:\r\n";
                    string sShippingMethodIdentifier = "";

                    //iterate the rates to show how to do it...just snags the last identifier for use in 'setting' the rate back onto cart from your UI 
                    foreach(ACDB.Rate oRate in aryRates)
                        sShippingMethodIdentifier = oRate.Identifier;
                    this.txtResults.Text += "Rate Set?" + oWS.Cart_SetShipping(oSession.ID.Value, sShippingMethodIdentifier) + "\r\n";
                }

                //Get the updated cart now for display, just to see the new values
                oCartInfo = oWS.Cart_GetBySessionID(oSession.ID.Value);
                this.txtResults.Text += "\r\nDISPLAY CART RESULTS\r\n";
                this.txtResults.Text += "**************************\r\n";
                this.txtResults.Text += "Unique SKUs In Cart: " + oCartInfo.CartColTrans.Count() + "\r\n";
                this.txtResults.Text += "SubTotal Total: " + oCartInfo.SubTotal.Value + "\r\n";
                this.txtResults.Text += "Discount Total: " + oCartInfo.DiscountTotal.Value + "\r\n";
                this.txtResults.Text += "Tax Total: " + oCartInfo.TaxTotal.Value + "\r\n";
                this.txtResults.Text += "Shipping Total: " + oCartInfo.ShippingTotal.Value + "\r\n";
                this.txtResults.Text += "Cart Total: " + oCartInfo.GrandTotal.Value + "\r\n";
            }





            if (chkPlaceOrder.IsChecked.Value)
            {

                //CARTING COMPLETED, PLACE ORDER OPERATIONS BEGIN
                this.txtResults.Text += "\r\n";
                this.txtResults.Text += "\r\n";
                this.txtResults.Text += "Carting Complete, Ordering Begins\r\n";

                //Get payment methods that are active on the store
                //This can be used to populate your own payment lists
                this.txtResults.Text += "\r\n";
                this.txtResults.Text += "List Active Payment Methods.\r\n";
                var aryPaymentMethods = new System.Collections.ArrayList(oWS.Cart_GetPaymentMethods());
                foreach (ACDB.ActivePaymentMethodTrans oPaymentMethod in aryPaymentMethods)
                {
                    this.txtResults.Text += oPaymentMethod.PaymentMethodName + " (" + oPaymentMethod.PaymentType + ") [" + oPaymentMethod.PaymentMethodID + "]\r\n";
                }

                //GATHER FINAL CUSTOMER INFORMATION (can be started at any time on the session, encouraged too for abandoned cart and drip series followup features of AmeriCommerce, gather and post this information as soon as you see fit)
                //We will need all of the pertinent customer information to place the order
                this.txtResults.Text += "\r\n";
                this.txtResults.Text += "\r\n";
                this.txtResults.Text += "Gather Remaining Order Information.\r\n";

                //This will be done via your user interface and the fields mapped to their sister values on our API

                var oPlaceOrderInfo = new AmeriCommerceApiExamples.ACDB.PlaceOrderTrans();
                
                oPlaceOrderInfo.CustomerFirstName = "JohnAPI";
                oPlaceOrderInfo.CustomerLastName = "DoeAPI";
                oPlaceOrderInfo.CustomerEmail = "johndoe@api.com";
                oPlaceOrderInfo.CustomerPhone1 = "123-123-1234";
                oPlaceOrderInfo.CustomerFax = "456-456-4567";
                
                //can leave this blank for guest checkout as long as the customer does not want to track their orders
                oPlaceOrderInfo.CustomerPassword = "mypasswordtest";

                //Address information
                oPlaceOrderInfo.CustomerBillingAddressLine1 = "5390 Washington Blvd.";
                oPlaceOrderInfo.CustomerBillingAddressLine2 = "";
                oPlaceOrderInfo.CustomerBillingAddressCity = "Beaumont";
                oPlaceOrderInfo.CustomerBillingAddressPostalCode = "77707";
                oPlaceOrderInfo.CustomerBillingAddressStateCode = "TX"; //can get from state list
                oPlaceOrderInfo.CustomerBillingAddressCountryCode = "US"; //can get from country list
                oPlaceOrderInfo.CustomerBillingAddressNotes = "Optional address note: leave at front door";

                //can optionally supply a separate shipping address or just set this flag if ikt is the same as the billing address
                oPlaceOrderInfo.CustomerShippingSameAsBilling = true;

                //payment information is required unless $0 orders are allowed or you have a custom payment method that allows a non-payment order
                oPlaceOrderInfo.PaymentMethodID = 0; //0 = credit card, can get all applicable types from the web service method
                oPlaceOrderInfo.CreditCardNameOnCard = "John Doe Sr.";
                oPlaceOrderInfo.CreditCardNumber = "4444444444444441"; //with or without dashes
                //oPlaceOrderInfo.CreditCardNumber = "4111111111111111"; //with or without dashes
                oPlaceOrderInfo.CreditCardCVV = "777";
                oPlaceOrderInfo.CreditCardExpirationMonth = "12";
                oPlaceOrderInfo.CreditCardExpirationYear = "2010";

                //other info
                oPlaceOrderInfo.GiftMessage = "Happy Coding!";
                oPlaceOrderInfo.CommentsPrivate = "Private Comments: Only admins see this by default.";
                oPlaceOrderInfo.CommentsPublic = "Public: Customers and Admins can see and edit this";
                oPlaceOrderInfo.CommentsInstructions = "Instructions: Viewable but not editable by customer, admins can edit";

                //PLACE THE ORDER
                //  The final step that posts the order into the appropriate status in the AmeriCommerce system
                //  This will capture/authorize payment information and sends email confirmations
                //  Will return status and validation information in the event an error occurs or more entry is needed
                this.txtResults.Text += "Calling Order Placement:\r\n";
                ACDB.PlaceOrderResponseTrans oResponse = oWS.Cart_PlaceOrder(oSession.ID.Value, oPlaceOrderInfo);
                this.txtResults.Text += "OrderID: " + oResponse.OrderID + " Msg: " + oResponse.ResponseMessage + "\r\n";

                ACDB.OrderTrans oOrder = oWS.Order_GetByKey(oResponse.OrderID);
                this.txtResults.Text += "\r\n\r\nCustomerID: " + oOrder.customerID.Value + "\r\n";
                ACDB.CustomerTrans oCustomer = oWS.Customer_GetByKey(oOrder.customerID.Value);
                oCustomer = oWS.Customer_FillCustomerPaymentMethodCollection(oCustomer);
                var aryCustomerPaymentMethods = new System.Collections.ArrayList(oCustomer.CustomerPaymentMethodColTrans);
                foreach(ACDB.CustomerPaymentMethodTrans oPayment in aryCustomerPaymentMethods)
                    this.txtResults.Text += "PaymentMethod: " + oPayment.paymentType + " MaskedValue:" + oPayment.PaymentTypeMasked + "\r\n";

            }
            //Can now grab the order itself from the API if needed by calling Order_Get methods
        }

        private void btnGetCategoryProducts_Click(object sender, RoutedEventArgs e)
        {
            ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
            ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();
            oHeader.UserName = Properties.Settings.Default.ApiUsername;
            oHeader.Password = Properties.Settings.Default.ApiPassword;
            oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;
            oWS.AmeriCommerceHeaderInfoValue = oHeader;

            this.txtResults.Text = "";
            this.txtResults.Text += "Grabbing Products from Category\r\n";
            var aryProducts = new System.Collections.ArrayList(oWS.Category_GetProducts(1));
            this.txtResults.Text += "Products Returned: " + aryProducts.Count + "\r\n";
            foreach (ACDB.ProductTrans oProduct in aryProducts)
            {
                this.txtResults.Text += "ID: " + oProduct.itemID.Value + ": " + oProduct.itemName + "\r\n";
            }
        }

        private void btnGetOrdersPreFilled_Click(object sender, RoutedEventArgs e)
        {
            ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
            ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();
            oHeader.UserName = Properties.Settings.Default.ApiUsername;
            oHeader.Password = Properties.Settings.Default.ApiPassword;
            oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;
            oWS.AmeriCommerceHeaderInfoValue = oHeader;

            this.txtResults.Text = "";
            this.txtResults.Text += "Get Orders With All Information Filled\r\n";
            var aryOrders = new System.Collections.ArrayList(oWS.Order_GetByDateRangePreFilled(DateTime.Parse("1/1/2008"), DateTime.Parse("1/1/2010")));
            this.txtResults.Text += "Orders Returned: " + aryOrders.Count + "\r\n";
            foreach (ACDB.OrderTrans oOrder in aryOrders)
            {
                var aryOrderItems = new System.Collections.ArrayList(oOrder.OrderItemColTrans);
                this.txtResults.Text += "ID: " + oOrder.orderID.Value + ": Total: " + oOrder.total.Value + " ItemCount: " + aryOrderItems.Count + "\r\n";
                foreach(ACDB.OrderItemTrans oItem in aryOrderItems)
                {
                    this.txtResults.Text += "     Qty:" + oItem.quantity.Value + " of " + oItem.itemNr + "[" + oItem.itemID.Value + "]" + " " + oItem.itemName + " at $" + oItem.price.Value + "\r\n";
                }
                var aryPayments = new System.Collections.ArrayList(oOrder.OrderPaymentColTrans);
                this.txtResults.Text += "     Payment: ";
                foreach(ACDB.OrderPaymentTrans oPayment in aryPayments)
                {
                    this.txtResults.Text += oPayment.Amount.Value + ", ";
                }
                this.txtResults.Text += "\r\n";

                if (oOrder.OrderBillingAddressTrans != null)
                    this.txtResults.Text += "     BillingAddress: " + oOrder.OrderBillingAddressTrans.City + ", " + oOrder.OrderBillingAddressTrans.StateTrans.stateCode + "[" + oOrder.OrderBillingAddressTrans.StateID.Value + "] " + oOrder.OrderBillingAddressTrans.ZipCode + "\r\n";
                if(oOrder.OrderShippingAddressTrans != null)
                    this.txtResults.Text += "     ShippingAddress: " + oOrder.OrderShippingAddressTrans.City + ", " + oOrder.OrderShippingAddressTrans.StateTrans.stateCode + "[" + oOrder.OrderShippingAddressTrans.StateID.Value + "[ " + oOrder.OrderShippingAddressTrans.ZipCode + "\r\n";
            }

        }

        private void btnPlaceOrderOnCinsaySite_Click(object sender, RoutedEventArgs e)
        {
            ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
            ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();
            oHeader.UserName = Properties.Settings.Default.ApiUsername;
            oHeader.Password = Properties.Settings.Default.ApiPassword;
            oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;
            oWS.AmeriCommerceHeaderInfoValue = oHeader;

            this.txtResults.Text = "";

        }

        private void btnSaveCustomerToAC_Click(object sender, RoutedEventArgs e)
        {
            ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
            ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();
            oHeader.UserName = Properties.Settings.Default.ApiUsername;
            oHeader.Password = Properties.Settings.Default.ApiPassword;
            oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;
            oWS.AmeriCommerceHeaderInfoValue = oHeader;
            oWS.Url = Properties.Settings.Default.AmeriCommerceApiExamples_ACDB_AmeriCommerceDatabaseIO;

            this.txtResults.Text = "";

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

            this.txtResults.Text = "Customer Returned and Saved: ID: " + oCustomer.customerID.Value;

        }

        private void btnGetProduct_Click(object sender, RoutedEventArgs e)
        {
            ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
            ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();
            oHeader.UserName = Properties.Settings.Default.ApiUsername;
            oHeader.Password = Properties.Settings.Default.ApiPassword;
            oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;
            oWS.AmeriCommerceHeaderInfoValue = oHeader;
            oWS.Url = Properties.Settings.Default.AmeriCommerceApiExamples_ACDB_AmeriCommerceDatabaseIO;

            this.txtResults.Text = "";

            ACDB.ProductTrans oProduct = new AmeriCommerceApiExamples.ACDB.ProductTrans();
            oProduct = oWS.Product_GetByKey(516);
            ACDB.ProductStatusTrans oStatus = oWS.ProductStatus_GetByKey(oProduct.productStatusID.Value);

            this.txtResults.Text = "Product Status is : " + oStatus.productStatus;
        }

        private void btnTestOrderConsumption_Click(object sender, RoutedEventArgs e)
        {
            ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
            ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();
            oHeader.UserName = Properties.Settings.Default.ApiUsername;
            oHeader.Password = Properties.Settings.Default.ApiPassword;
            oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;
            oWS.AmeriCommerceHeaderInfoValue = oHeader;
            oWS.Url = Properties.Settings.Default.AmeriCommerceApiExamples_ACDB_AmeriCommerceDatabaseIO;

            this.txtResults.Text = "";

            ACDB.OrderTrans[] colOrders = oWS.Order_GetByDateRange(DateTime.Parse("9/10/2010"), DateTime.Parse("1/1/2011"));
            ArrayList aryOrders = new ArrayList(colOrders);
            ACDB.OrderTrans oOrder = (ACDB.OrderTrans)aryOrders[0];
            this.txtResults.Text = "Order Count is : " + aryOrders.Count;
            this.txtResults.Text += "\r\n\r\nOrder ID: " + oOrder.orderID.Value;
            this.txtResults.Text += "\r\n\r\nOrder Comments: " + oOrder.comments;

            oOrder.comments = "ED: " + DateTime.Now.Ticks;
            oWS.Order_SaveAndGet(oOrder);

            this.txtResults.Text += "\r\n\r\nAFTERSAVE: Order Comments: " + oOrder.comments;


        }

        private void btnPullInventory_Click(object sender, RoutedEventArgs e)
        {
            ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
            ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();
            oHeader.UserName = Properties.Settings.Default.ApiUsername;
            oHeader.Password = Properties.Settings.Default.ApiPassword;
            oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;
            oWS.AmeriCommerceHeaderInfoValue = oHeader;
            oWS.Url = Properties.Settings.Default.AmeriCommerceApiExamples_ACDB_AmeriCommerceDatabaseIO;

            this.txtResults.Text = "";

            ACDB.ProductTrans oProduct = oWS.Product_GetByKey(2);
            oProduct = oWS.Product_FillProductVariantCollection(oProduct);
            ArrayList aryVariants = new ArrayList(oProduct.ProductVariantColTrans);
            oProduct = oWS.Product_FillVariantInventoryCollection(oProduct);
            ArrayList aryVariantInventory = new ArrayList(oProduct.VariantInventoryColTrans);
            this.txtResults.Text = oProduct.itemName + " has " + aryVariants.Count + " variants.";

            this.txtResults.Text += "\r\n\r\nAnd has " + aryVariantInventory.Count + " variant inventory records.";

            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            ACDB.AmeriCommerceDatabaseIO oWS = new AmeriCommerceApiExamples.ACDB.AmeriCommerceDatabaseIO();
            ACDB.AmeriCommerceHeaderInfo oHeader = new AmeriCommerceApiExamples.ACDB.AmeriCommerceHeaderInfo();
            oHeader.UserName = Properties.Settings.Default.ApiUsername;
            oHeader.Password = Properties.Settings.Default.ApiPassword;
            oHeader.SecurityToken = Properties.Settings.Default.ApiSecurityToken;
            oWS.AmeriCommerceHeaderInfoValue = oHeader;
            oWS.Url = Properties.Settings.Default.AmeriCommerceApiExamples_ACDB_AmeriCommerceDatabaseIO;

            this.txtResults.Text = "";

            ACDB.ProductTrans oProduct = oWS.Product_GetByItemNumber("TS2PS");
            this.txtResults.Text = oProduct.itemName + " exists with ID of " + oProduct.itemID;

        }
    }
}
