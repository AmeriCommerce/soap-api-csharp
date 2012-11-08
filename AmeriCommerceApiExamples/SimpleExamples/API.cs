using System;
using AmeriCommerceApiExamples.ACDB;

namespace APIExamples
{
	class API
	{
		public static AmeriCommerceDatabaseIO InitializeWebservice()
		{
			AmeriCommerceDatabaseIO oWs = new AmeriCommerceDatabaseIO();

			AmeriCommerceHeaderInfo oHeader = new AmeriCommerceHeaderInfo();
			oHeader.UserName = "user";
			oHeader.Password = "pass";
			oHeader.SecurityToken = "asdfd";

			oWs.AmeriCommerceHeaderInfoValue = oHeader;

			return oWs;
		}

		public static DataInt32 MakeDataInt(int piValue)
		{
			DataInt32 iInt = new DataInt32();
			iInt.Value = piValue;

			return iInt;
		}

		public static DataMoney MakeDataMoney(decimal pdValue)
		{
			DataMoney dMoney = new DataMoney();
			dMoney.Value = pdValue;

			return dMoney;
		}

		public static DataDateTime MakeDataDateTime(DateTime pdtValue)
		{
			DataDateTime dtDate = new DataDateTime();
			dtDate.Value = pdtValue;

			return dtDate;
		}
		
		public static void CreateProduct()
		{
			AmeriCommerceDatabaseIO oWs = InitializeWebservice();

			ProductTrans oProd = new ProductTrans();
			oProd.itemName = "API Test Product 1";
			oProd.itemNr = "APITP1";
			oProd.price = MakeDataMoney(5.00m);
			oProd.catID = MakeDataInt(21);
			oProd.productStatusID = MakeDataInt(1);

			oWs.Product_Save(oProd);
		}

		public static OrderTrans CreateOrder()
		{
			AmeriCommerceDatabaseIO oWs = InitializeWebservice();

			OrderTrans oOrder = new OrderTrans();

			oOrder.subTotal = MakeDataMoney(5.00m);
			oOrder.taxAdded = MakeDataMoney(1.00m);
			oOrder.shippingAdded = MakeDataMoney(1.00m);
			oOrder.total = MakeDataMoney(7.00m);
			oOrder.orderStatusID = MakeDataInt(1);
			oOrder.customerID = MakeDataInt(1);

			// create order items collection
			// this test order will only have one item
			oOrder.OrderItemColTrans = new OrderItemTrans[1];

			OrderItemTrans oItem = new OrderItemTrans();
			oItem.itemID = MakeDataInt(209);
			oItem.itemName = "Product100";
			oItem.price = MakeDataMoney(5.00m);
			oItem.quantity = MakeDataInt(1);

			oOrder.OrderItemColTrans[0] = oItem;

			// when an object has child collections (order items in this example), as long as they are assigned
			// correctly they will be saved automatically along with the parent

			// just demoing SaveAndGet - can use this or Save
			return oWs.Order_SaveAndGet(oOrder);
		}

		public static void CreateShipment()
		{
			AmeriCommerceDatabaseIO oWs = InitializeWebservice();

			// get an existing order
			OrderTrans oOrder = oWs.Order_GetByKey(100167);

			// fill its collection of existing shipments
			oOrder = oWs.Order_FillOrderShippingCollection(oOrder);

			// create a list out of the shipments collection array so we can manipulate it
			System.Collections.Generic.List<OrderShippingTrans> oShipmentCol;
			if (oOrder.OrderShippingColTrans != null)
				oShipmentCol = new System.Collections.Generic.List<OrderShippingTrans>(oOrder.OrderShippingColTrans);
			else
				oShipmentCol = new System.Collections.Generic.List<OrderShippingTrans>();

			// add a new shipment
			OrderShippingTrans oShipment = new OrderShippingTrans();

			oShipment.OrderID = oOrder.orderID;
			oShipment.TrackingNumbers = "1234567890";
			oShipment.ShippingDate = MakeDataDateTime(DateTime.Parse("09/1/09"));
			oShipment.ShippingMethod = "test";
			oShipment.TotalWeight = MakeDataMoney(1.0m);

			// optionally add items to shipment
			// this shipment will contain all items on the order, but it can be a partial shipment as well
			oOrder = oWs.Order_FillOrderItemCollection(oOrder);

			// this part is a bit strange - this is due to OrderShippingOrderItemsTrans being an associative entity
			// between the OrderItems and OrderShipping collections

			// since the shipment will contain all the items on the order, create an array that is the same length as the order's items array
			oShipment.OrderItemColTrans = new OrderItemTrans[oOrder.OrderItemColTrans.Length];

			// loop through and create the associative entity for each order item
			for (int i = 0; i < oOrder.OrderItemColTrans.Length; i++)
			{
				OrderShippingOrderItemsTrans oItem = new OrderShippingOrderItemsTrans();

				oItem.OrderItemsID = oOrder.OrderItemColTrans[i].orderItemsID;
				oItem.QuantityShipped = oOrder.OrderItemColTrans[i].quantity;

				oShipment.OrderItemColTrans[i] = oOrder.OrderItemColTrans[i];
				oShipment.OrderItemColTrans[i].OrderShippingOrderItemsTrans = oItem;
			}

			// add this shipment to the collection
			oShipmentCol.Add(oShipment);

			// convert the shipment col back to an array and assign it to the order
			oOrder.OrderShippingColTrans = oShipmentCol.ToArray();

			// saving the parent object (the order) will handle saving all of its children (shipments, etc)
			oWs.Order_Save(oOrder);
		}
	}
}
