using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GimjaBL
{
    public class SyncTransactionBL
    {
        public static XElement GetReceiptElement(string receiptId)
        {
            if (string.IsNullOrEmpty(receiptId))
                throw new ArgumentNullException("The receipt ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _receipt = (from r in db.Receipts.Include("ReceivedItems")
                                where r.id.Equals(receiptId) //&& !(r.isDeleted ?? false)
                                select r).SingleOrDefault();
                return GetReceiptElement(_receipt);
            }
        }

        public static XElement GetReceiptElement(Receipt _receipt)
        {
            if (_receipt == null)
            {
                XElement e = new XElement("receipts", null);
                return e;
            }
            else
            {
                //XElement elem = new XElement("receipts",
                XElement elem = new XElement("receipt", new XAttribute("ID", _receipt.id),
                    new XAttribute("SupplierID", _receipt.rlkSupplierID),
                    new XAttribute("Date", _receipt.date),
                    new XAttribute("ReceivedBy", _receipt.receivedBy),
                    new XAttribute("ApprovedBy", string.IsNullOrEmpty(_receipt.approvedBy) ? string.Empty : _receipt.approvedBy),
                    new XAttribute("ApprovedDate", _receipt.approvedDate.HasValue ? Convert.ToString(_receipt.approvedDate) : string.Empty),
                    new XAttribute("StoreID", _receipt.storeID),
                    new XAttribute("ProcessedBy", _receipt.processedBy),
                    new XAttribute("IsStoreWarehouse", _receipt.isStoreWarehouse),
                    new XAttribute("ReceivedFrom", _receipt.receivedFrom),
                    new XAttribute("IsApproved", _receipt.isApproved ?? false),
                    new XAttribute("CreatedBy", _receipt.createdBy),
                    new XAttribute("CreatedDate", _receipt.createdDate),
                    new XAttribute("LastUpdatedBy", _receipt.lastUpdatedBy ?? string.Empty),
                    new XAttribute("LastUpdatedDate", _receipt.lastUpdatedDate.HasValue ? Convert.ToString(_receipt.lastUpdatedDate) : string.Empty),
                    new XAttribute("IsDeleted", _receipt.isDeleted ?? false),
                    new XElement("receivedItems", _receipt.ReceivedItems.Select(x =>
                        new XElement("receivedItem", new XAttribute("ReceiptID", x.receiptID),
                        new XAttribute("ReceiptDetailID", x.receiptDetailID),
                        new XAttribute("ItemID", x.itemID),
                        new XAttribute("ManufacturerID", x.rlkManufacturerID),
                        new XAttribute("NoPack", x.noPack.HasValue ? Convert.ToString(x.noPack) : string.Empty),
                        new XAttribute("QtyPerPack", x.qtyPerPack.HasValue ? Convert.ToString(x.qtyPerPack) : string.Empty),
                        new XAttribute("Quantity", x.quantity),
                        new XAttribute("Price", x.price),
                        new XAttribute("UnitSellingPrice", x.unitSellingPrice),
                        new XAttribute("CreatedBy", x.createdBy),
                        new XAttribute("CreatedDate", x.createdDate),
                        new XAttribute("LastUpdatedBy", x.lastUpdatedBy ?? string.Empty),
                        new XAttribute("LastUpdatedDate", x.lastUpdatedDate.HasValue ? Convert.ToString(x.lastUpdatedDate) : string.Empty),
                        new XAttribute("IsDeleted", x.isDeleted ?? false)
                        ))));

                return elem;
            }
        }

        public static ReceiptData GetReceiptData(Guid syncId, out List<ReceivedItemData> receivedItems)
        {
            if (syncId == Guid.Empty)
            {
                receivedItems = null;
                return null;
            }
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var syncValue = (from s in db.SyncTransactions
                                 where s.id == syncId && s.tableName.Equals("tblReceipt")
                                 select s.value).SingleOrDefault();

                return GetReceiptData(syncValue, out receivedItems);
            }
        }

        public static ReceiptData GetReceiptData(string syncValue, out List<ReceivedItemData> receivedItems)
        {
            XElement elem;
            if (!string.IsNullOrEmpty(syncValue))
            {
                elem = XElement.Parse(syncValue);
                //var receiptsRoot = elem.Element("receipts");
                var receipt = elem.Element("receipt");
                if (receipt != null)//.Count() == 1)
                {
                    receivedItems = new List<ReceivedItemData>();
                    //create a receipt object
                    ReceiptData retValue = new ReceiptData();
                    //var receipt = receipts.First();
                    retValue.ID = receipt.Attribute("ID").Value;
                    retValue.SupplierID = receipt.Attribute("SupplierID").Value;
                    retValue.Date = Convert.ToDateTime(receipt.Attribute("Date").Value);
                    retValue.ReceivedBy = receipt.Attribute("ReceivedBy").Value;
                    retValue.ApprovedBy = string.IsNullOrEmpty(receipt.Attribute("ApprovedBy").Value) ? null : receipt.Attribute("ApprovedBy").Value;
                    retValue.ApprovedDate = string.IsNullOrEmpty(receipt.Attribute("ApprovedDate").Value) ? (DateTime?)null : Convert.ToDateTime(receipt.Attribute("ApprovedDate").Value);
                    retValue.StoreID = receipt.Attribute("StoreID").Value;
                    retValue.ProcessedBy = receipt.Attribute("ProcessedBy").Value;
                    retValue.IsStoreWarehouse = string.IsNullOrEmpty(receipt.Attribute("IsStoreWarehouse").Value) ? (bool?)null : Convert.ToBoolean(receipt.Attribute("IsStoreWarehouse").Value);
                    retValue.ReceivedFrom = receipt.Attribute("ReceivedFrom").Value;
                    retValue.IsApproved = string.IsNullOrEmpty(receipt.Attribute("IsApproved").Value) ? (bool?)null : Convert.ToBoolean(receipt.Attribute("IsApproved").Value);
                    retValue.CreatedBy = receipt.Attribute("CreatedBy").Value;
                    retValue.CreatedDate = Convert.ToDateTime(receipt.Attribute("CreatedDate").Value);
                    retValue.LastUpdatedBy = string.IsNullOrEmpty(receipt.Attribute("LastUpdatedBy").Value) ? null : receipt.Attribute("LastUpdatedBy").Value;
                    retValue.LastUpdatedDate = string.IsNullOrEmpty(receipt.Attribute("LastUpdatedDate").Value) ? (DateTime?)null : Convert.ToDateTime(receipt.Attribute("LastUpdatedDate").Value); ;
                    retValue.IsDeleted = string.IsNullOrEmpty(receipt.Attribute("IsDeleted").Value) ? (bool?)null : Convert.ToBoolean(receipt.Attribute("IsDeleted").Value); ;
                    var receivedItemElems = receipt.Element("receivedItems").Elements("receivedItem");
                    if (receivedItemElems.Count() > 0)
                    {
                        foreach (var item in receivedItemElems)
                        {
                            var _receiptDetail = new ReceivedItemData();
                            _receiptDetail.ReceiptID = item.Attribute("ReceiptID").Value;
                            _receiptDetail.ReceiptDetailsID = new Guid(item.Attribute("ReceiptDetailID").Value);
                            _receiptDetail.ItemID = item.Attribute("ItemID").Value;
                            _receiptDetail.ManufacturerID = item.Attribute("ManufacturerID").Value;
                            _receiptDetail.NoPack = string.IsNullOrEmpty(item.Attribute("NoPack").Value) ? (int?)null : Convert.ToInt32(item.Attribute("NoPack").Value);
                            _receiptDetail.QtyPerPack = string.IsNullOrEmpty(item.Attribute("QtyPerPack").Value) ? (int?)null : Convert.ToInt32(item.Attribute("QtyPerPack").Value);
                            _receiptDetail.Quantity = Convert.ToInt32(item.Attribute("Quantity").Value);
                            _receiptDetail.Price = Convert.ToDouble(item.Attribute("Price").Value);
                            _receiptDetail.UnitSellingPrice = Convert.ToDouble(item.Attribute("UnitSellingPrice").Value);
                            _receiptDetail.CreatedBy = item.Attribute("CreatedBy").Value;
                            _receiptDetail.CreatedDate = Convert.ToDateTime(item.Attribute("CreatedDate").Value);
                            _receiptDetail.LastUpdatedBy = string.IsNullOrEmpty(item.Attribute("LastUpdatedBy").Value) ? null : item.Attribute("LastUpdatedBy").Value;
                            _receiptDetail.LastUpdatedDate = string.IsNullOrEmpty(item.Attribute("LastUpdatedDate").Value) ? (DateTime?)null : Convert.ToDateTime(item.Attribute("LastUpdatedDate").Value);
                            _receiptDetail.IsDeleted = string.IsNullOrEmpty(item.Attribute("IsDeleted").Value) ? (bool?)null : Convert.ToBoolean(item.Attribute("IsDeleted").Value);

                            receivedItems.Add(_receiptDetail);
                        }
                    }
                    return retValue;
                }
                else
                {//the specified syncvalue doesn't contain a receipt element
                    receivedItems = null;
                    return null;
                }
            }
            else
            {//the given syncvalue is empty
                receivedItems = null;
                return null;
            }
        }

        public static XElement GetSaleElement(Guid salesId)
        {
            if (salesId == Guid.Empty)
                return null;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _sale = (from s in db.Sales.Include("SalesDetails")
                             where s.id == salesId
                             select s).SingleOrDefault();

                return GetSaleElement(_sale);
            }
        }

        public static XElement GetSaleElement(Sale _sale)
        {
            if (_sale == null)
                return null;
            //XElement elem = new XElement("sales",
            XElement elem = new XElement("sale", new XAttribute("ID", _sale.id),
                new XAttribute("BranchID", _sale.branchID),
                new XAttribute("Date", _sale.date),
                new XAttribute("ReferenceNo", _sale.referenceNo),
                new XAttribute("ReceiptID", string.IsNullOrEmpty(_sale.receiptID) ? string.Empty : _sale.receiptID),
                new XAttribute("CustomerID", _sale.customerID),
                new XAttribute("ProcessedBy", _sale.processedBy),
                new XAttribute("AuthorizedBy", string.IsNullOrEmpty(_sale.authorizedBy) ? string.Empty : _sale.authorizedBy),
                new XAttribute("CreatedBy", _sale.createdBy),
                new XAttribute("CreatedDate", _sale.createdDate),
                new XAttribute("LastUpdatedBy", _sale.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", _sale.lastUpdatedDate.HasValue ? _sale.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsVoid", _sale.isVoid ?? false),
                new XAttribute("IsSalesCredit", _sale.isSalesCredit),
                new XElement("salesDetails", _sale.SalesDetails.Select(sd =>
                    new XElement("salesDetail", new XAttribute("SalesDetailID", sd.salesDetailID),
                        new XAttribute("ItemID", sd.itemID),
                        new XAttribute("Quantity", sd.quantity),
                        new XAttribute("SalesFrom", sd.salesFrom),
                        new XAttribute("UnitPrice", sd.unitPrice),
                        new XAttribute("Discount", sd.discount ?? 0),
                        new XAttribute("CreatedBy", sd.createdBy),
                        new XAttribute("CreatedDate", sd.createdDate),
                        new XAttribute("LastUpdatedBy", sd.lastUpdatedBy ?? string.Empty),
                        new XAttribute("LastUpdatedDate", sd.lastUpdatedDate.HasValue ? sd.lastUpdatedDate.Value.ToString() : string.Empty),
                        new XAttribute("IsDeleted", sd.isDeleted ?? false)
                        ))));
            return elem;
        }

        public static SalesData GetSalesData(string syncValue, out List<SalesDetailData> saleDetailes)
        {
            if (string.IsNullOrEmpty(syncValue))
            {
                saleDetailes = null;
                return null;
            }
            var _elems = XElement.Parse(syncValue);
            //var salesRoot = _elems.Element("sales");
            //var sales = salesRoot.Elements("sale");
            var sale = _elems.Element("sale");
            if (sale != null)//sales.Count() == 1)
            {
                saleDetailes = new List<SalesDetailData>();
                SalesData retValue = new SalesData();
                //var sale = sales.First();
                retValue.ID = new Guid(sale.Attribute("ID").Value);
                retValue.BranchID = sale.Attribute("BranchID").Value;
                retValue.SalesDate = Convert.ToDateTime(sale.Attribute("Date").Value);
                retValue.ReferenceNo = sale.Attribute("ReferenceNo").Value;
                retValue.ReceiptID = sale.Attribute("ReceiptID").Value;
                retValue.CustomerID = sale.Attribute("CustomerID").Value;
                retValue.ProcessedBy = sale.Attribute("ProcessedBy").Value;
                retValue.AuthorizedBy = string.IsNullOrEmpty(sale.Attribute("AuthorizedBy").Value) ? null : sale.Attribute("AuthorizedBy").Value;
                retValue.CreatedBy = sale.Attribute("CreatedBy").Value;
                retValue.CreatedDate = Convert.ToDateTime(sale.Attribute("CreatedDate").Value);
                retValue.LastUpdatedBy = string.IsNullOrEmpty(sale.Attribute("LastUpdatedBy").Value) ? null : sale.Attribute("LastUpdatedBy").Value;
                retValue.LastUpdatedDate = string.IsNullOrEmpty(sale.Attribute("LastUpdatedDate").Value) ? (DateTime?)null : Convert.ToDateTime(sale.Attribute("LastUpdatedDate").Value);
                retValue.IsVoid = string.IsNullOrEmpty(sale.Attribute("IsVoid").Value) ? (bool?)null : Convert.ToBoolean(sale.Attribute("IsVoid").Value);
                retValue.IsSalesCredit = Convert.ToBoolean(sale.Attribute("IsSalesCredit").Value);

                var salesDetailElems = sale.Element("salesDetails").Elements("salesDetail");
                if (salesDetailElems.Count() > 0)
                {
                    foreach (var sd in salesDetailElems)
                    {
                        SalesDetailData detail = new SalesDetailData();
                        detail.SalesID = retValue.ID;
                        detail.SalesDetailID = new Guid(sd.Attribute("SalesDetailID").Value);
                        detail.ItemID = sd.Attribute("ItemID").Value;
                        detail.Quantity = Convert.ToInt64(sd.Attribute("Quantity").Value);
                        detail.SalesFrom = Convert.ToInt16(sd.Attribute("SalesFrom").Value);
                        detail.UnitPrice = Convert.ToDouble(sd.Attribute("UnitPrice").Value);
                        detail.Discount = string.IsNullOrEmpty(sd.Attribute("Discount").Value) ? (double?)null : Convert.ToDouble(sd.Attribute("Discount").Value);
                        detail.CreatedBy = Convert.ToString(sd.Attribute("CreatedBy").Value);
                        detail.CreatedDate = Convert.ToDateTime(sd.Attribute("CreatedDate").Value);
                        detail.LastUpdatedBy = string.IsNullOrEmpty(sd.Attribute("LastUpdatedBy").Value) ? null : Convert.ToString(sd.Attribute("LastUpdatedBy").Value);
                        detail.LastUpdatedDate = string.IsNullOrEmpty(sd.Attribute("LastUpdatedDate").Value) ? (DateTime?)null : Convert.ToDateTime(sd.Attribute("LastUpdatedDate").Value);
                        detail.IsDeleted = string.IsNullOrEmpty(sd.Attribute("IsDeleted").Value) ? (bool?)null : Convert.ToBoolean(sd.Attribute("IsDeleted").Value);

                        saleDetailes.Add(detail);
                    }
                }
                return retValue;
            }
            else
            {
                saleDetailes = null;
                return null;
            }
        }

        public static XElement GetSaleReturnElement(Guid saleReturnId)
        {
            if (saleReturnId == Guid.Empty)
                throw new ArgumentException("The given sale return ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _saleReturn = (from r in db.SalesReturns.Include("ReturnedItems")
                                   where r.id == saleReturnId
                                   select r).FirstOrDefault();

                return GetSaleReturnElement(_saleReturn);
            }
        }

        public static XElement GetSaleReturnElement(SalesReturn _saleReturn)
        {
            if (_saleReturn == null)
                return null;
            //XElement elem = new XElement("saleReturns",
            XElement elem = new XElement("saleReturn", new XAttribute("ID", _saleReturn.id),
                new XAttribute("SalesID", _saleReturn.salesID),
                new XAttribute("Date", _saleReturn.date),
                new XAttribute("ProcessedBy", _saleReturn.processedBy),
                new XAttribute("Reason", _saleReturn.reason),
                new XAttribute("IsDeleted", _saleReturn.isDeleted ?? false),
                new XAttribute("CreatedBy", _saleReturn.createdBy),
                new XAttribute("CreatedDate", _saleReturn.createdDate),
                new XAttribute("LastUpdatedBy", _saleReturn.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", _saleReturn.lastUpdatedDate.HasValue ? _saleReturn.lastUpdatedDate.Value.ToString() : string.Empty),
                new XElement("returnedItems", _saleReturn.ReturnedItems.Select(x =>
                    new XElement("returnedItem", new XAttribute("ReturnedDetailID", x.returnedDetailID),
                        new XAttribute("ItemID", x.itemID),
                        new XAttribute("Quantity", x.quantity),
                        new XAttribute("RefundedAmount", x.refundedAmount),
                        new XAttribute("CreatedBy", x.createdBy),
                        new XAttribute("CreatedDate", x.createdDate),
                        new XAttribute("LastUpdatedBy", x.lastUpdatedBy ?? string.Empty),
                        new XAttribute("LastUpdatedDate", x.lastUpdatedDate.HasValue ? x.lastUpdatedDate.Value.ToString() : string.Empty),
                        new XAttribute("IsDeleted", x.isDeleted ?? false)
                        ))));
            return elem;
        }

        public static SaleReturnData GetSaleReturnData(string syncValue, out List<ReturnItemData> saleReturnDetails)
        {
            if (string.IsNullOrEmpty(syncValue))
            {
                saleReturnDetails = null;
                return null;
            }
            var _elems = XElement.Parse(syncValue);
            //var salesRoot = _elems.Element("saleReturns");
            var saleReturn = _elems.Element("saleReturn");
            if (saleReturn != null)// sales.Count() == 1)
            {//the single sale return item contains multiple return items
                saleReturnDetails = new List<ReturnItemData>();
                SaleReturnData retValue = new SaleReturnData();
                //var saleReturn = sales.First();
                retValue.ID = new Guid(saleReturn.Attribute("ID").Value);
                retValue.SalesID = new Guid(saleReturn.Attribute("SalesID").Value);
                retValue.Date = Convert.ToDateTime(saleReturn.Attribute("Date").Value);
                retValue.ProcessedBy = saleReturn.Attribute("ProcessedBy").Value;
                retValue.Reason = string.IsNullOrEmpty(saleReturn.Attribute("Reason").Value) ? null : saleReturn.Attribute("Reason").Value;
                retValue.IsDeleted = string.IsNullOrEmpty(saleReturn.Attribute("IsDeleted").Value) ? (bool?)null : Convert.ToBoolean(saleReturn.Attribute("IsDeleted").Value);
                retValue.CreatedBy = saleReturn.Attribute("CreatedBy").Value;
                retValue.CreatedDate = Convert.ToDateTime(saleReturn.Attribute("CreatedDate").Value);
                retValue.LastUpdatedBy = string.IsNullOrEmpty(saleReturn.Attribute("LastUpdatedBy").Value) ? null : saleReturn.Attribute("LastUpdatedBy").Value;

                var returnedItems = saleReturn.Element("returnedItems").Elements("returnedItem");
                foreach (var item in returnedItems)
                {
                    ReturnItemData retItem = new ReturnItemData();
                    retItem.SalesReturnID = retValue.ID;
                    retItem.ReturnedDetailsID = new Guid(item.Attribute("ReturnedDetailID").Value);
                    retItem.ItemID = item.Attribute("ItemID").Value;
                    retItem.Quantity = Convert.ToInt32(item.Attribute("Quantity").Value);
                    retItem.RefundedAmount = Convert.ToDouble(item.Attribute("RefundedAmount").Value);
                    retItem.CreatedBy = item.Attribute("CreatedBy").Value;
                    retItem.CreatedDate = Convert.ToDateTime(item.Attribute("CreatedDate").Value);
                    retItem.LastUpdatedBy = string.IsNullOrEmpty(item.Attribute("LastUpdatedBy").Value) ? null : item.Attribute("LastUpdatedBy").Value;
                    retItem.LastUpdatedDate = string.IsNullOrEmpty(item.Attribute("LastUpdatedDate").Value) ? (DateTime?)null : Convert.ToDateTime(item.Attribute("LastUpdatedDate").Value);
                    retItem.IsDeleted = string.IsNullOrEmpty(item.Attribute("IsDeleted").Value) ? (bool?)null : Convert.ToBoolean(item.Attribute("IsDeleted").Value);

                    saleReturnDetails.Add(retItem);
                }

                return retValue;
            }
            else
            {
                saleReturnDetails = null;
                return null;
            }
        }

        public static XElement GetIssuanceElement(string issuanceId)
        {
            if (string.IsNullOrEmpty(issuanceId))
                throw new ArgumentNullException("The receipt ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _issuance = (from i in db.Issuances.Include("IssuedItems")
                                 where i.id.Equals(issuanceId)
                                 select i).SingleOrDefault();

                return GetIssuanceElement(_issuance);
            }
        }

        public static XElement GetIssuanceElement(Issuance _issuance)
        {
            if (_issuance == null)
            {
                return null;
            }
            //XElement elem = new XElement("issuances",
            XElement elem = new XElement("issuance", new XAttribute("ID", _issuance.id),
                new XAttribute("IssuedTo", _issuance.issuedTo),
                new XAttribute("Date", _issuance.date),
                new XAttribute("IssuedBy", _issuance.issuedBy),
                new XAttribute("ApprovedBy", _issuance.approvedBy ?? string.Empty),
                new XAttribute("ApprovedDate", _issuance.approvedDate.HasValue ? _issuance.approvedDate.Value.ToString() : string.Empty),
                new XAttribute("StoreID", _issuance.storeID),
                new XAttribute("WarehouseID", _issuance.warehouseID),
                new XAttribute("CreatedBy", _issuance.createdBy),
                new XAttribute("CreatedDate", _issuance.createdDate),
                new XAttribute("LastUpdatedBy", _issuance.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", _issuance.lastUpdatedDate.HasValue ? _issuance.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", _issuance.isDeleted ?? false),
                new XElement("issuedItems", _issuance.IssuedItems.Select(x =>
                    new XElement("issuedItem", new XAttribute("IssueDetailID", x.issueDetailID),
                        new XAttribute("ItemID", x.itemID),
                        new XAttribute("Quantity", x.quantity),
                        new XAttribute("NoPack", x.noPack.HasValue ? x.noPack.ToString() : string.Empty),
                        new XAttribute("QtyPerPack", x.qtyPerPack.HasValue ? x.qtyPerPack.ToString() : string.Empty),
                        new XAttribute("CreatedBy", x.createdBy),
                        new XAttribute("CreatedDate", x.createdDate),
                        new XAttribute("LastUpdatedBy", x.lastUpdatedBy ?? string.Empty),
                        new XAttribute("LastUpdatedDate", x.lastUpdatedDate.HasValue ? x.lastUpdatedDate.ToString() : string.Empty),
                        new XAttribute("IsDeleted", x.isDeleted ?? false)))
                ));
            return elem;
        }

        public static IssueData GetIssuanceData(string syncValue, out List<IssuedItemData> issuanceDetails)
        {
            if (string.IsNullOrEmpty(syncValue))
            {
                issuanceDetails = null;
                return null;
            }
            var _elems = XElement.Parse(syncValue);
            //var _issuancesRoot = _elems.Element("issuances");
            var _issuance = _elems.Element("issuance");
            if (_issuance != null)
            {
                issuanceDetails = new List<IssuedItemData>();
                IssueData _retValue = new IssueData();
                _retValue.ID = _issuance.Attribute("ID").Value;
                _retValue.IssuedTo = _issuance.Attribute("IssuedTo").Value;
                _retValue.Date = Convert.ToDateTime(_issuance.Attribute("Date").Value);
                _retValue.IssuedBy = _issuance.Attribute("IssuedBy").Value;
                _retValue.ApprovedBy = _issuance.Attribute("ApprovedBy").Value;
                _retValue.ApprovedDate = string.IsNullOrEmpty(_issuance.Attribute("ApprovedDate").Value) ? (DateTime?)null : Convert.ToDateTime(_issuance.Attribute("ApprovedDate").Value);
                _retValue.StoreID = _issuance.Attribute("StoreID").Value;
                _retValue.WarehouseID = _issuance.Attribute("WarehouseID").Value;
                _retValue.CreatedBy = _issuance.Attribute("CreatedBy").Value;
                _retValue.CreatedDate = Convert.ToDateTime(_issuance.Attribute("CreatedDate").Value);
                _retValue.LastUpdatedBy = string.IsNullOrEmpty(_issuance.Attribute("LastUpdatedBy").Value) ? null : _issuance.Attribute("LastUpdatedBy").Value;
                _retValue.LastUpdatedDate = string.IsNullOrEmpty(_issuance.Attribute("LastUpdatedDate").Value) ? (DateTime?)null : Convert.ToDateTime(_issuance.Attribute("LastUpdatedDate").Value);
                _retValue.IsDeleted = string.IsNullOrEmpty(_issuance.Attribute("IsDeleted").Value) ? (bool?)null : Convert.ToBoolean(_issuance.Attribute("IsDeleted").Value);
                var _issuedItems = _issuance.Element("issuedItems").Elements("issuedItem");
                foreach (var _item in _issuedItems)
                {
                    IssuedItemData issuedItem = new IssuedItemData();
                    issuedItem.IssuanceID = _retValue.ID;
                    issuedItem.IssueDetailID = new Guid(_item.Attribute("IssueDetailID").Value);
                    issuedItem.ItemID = _item.Attribute("ItemID").Value;
                    issuedItem.Quantity = Convert.ToInt32(_issuance.Attribute("Quantity").Value);
                    issuedItem.NoPack = string.IsNullOrEmpty(_issuance.Attribute("NoPack").Value) ? (int?)null : Convert.ToInt32(_issuance.Attribute("NoPack").Value);
                    issuedItem.QtyPerPack = string.IsNullOrEmpty(_issuance.Attribute("QtyPerPack").Value) ? (int?)null : Convert.ToInt32(_issuance.Attribute("QtyPerPack").Value);
                    issuedItem.CreatedBy = _issuance.Attribute("CreatedBy").Value;
                    issuedItem.CreatedDate = Convert.ToDateTime(_issuance.Attribute("CreatedDate").Value);
                    issuedItem.LastUpdatedBy = string.IsNullOrEmpty(_issuance.Attribute("LastUpdatedBy").Value) ? null : _issuance.Attribute("LastUpdatedBy").Value;
                    issuedItem.LastUpdatedDate = string.IsNullOrEmpty(_issuance.Attribute("LastUpdatedDate").Value) ? (DateTime?)null : Convert.ToDateTime(_issuance.Attribute("LastUpdatedDate").Value);
                    issuedItem.IsDeleted = string.IsNullOrEmpty(_issuance.Attribute("IsDeleted").Value) ? (bool?)null : Convert.ToBoolean(_issuance.Attribute("IsDeleted").Value);

                    issuanceDetails.Add(issuedItem);
                }

                return _retValue;
            }
            else
            {
                issuanceDetails = null;
                return null;
            }
        }

        public static XElement GetManufacturerElement(string manId)
        {
            if (string.IsNullOrEmpty(manId))
                throw new ArgumentNullException("The manufacturer ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _man = (from m in db.Manufacturers
                            where m.lkManufacturerID.Equals(manId)
                            select m).SingleOrDefault();

                return GetManufacturerElement(_man);
            }
        }

        public static XElement GetManufacturerElement(Manufacturer _man)
        {
            if (_man == null)
                return null;
            XElement _elem = new XElement("manufacturer", new XAttribute("ManufacturerID", _man.lkManufacturerID),
                new XAttribute("Name", _man.name),
                new XAttribute("ContactPerson", _man.contactPerson ?? string.Empty),
                new XAttribute("Description", _man.description ?? string.Empty),
                new XAttribute("IsActive", _man.isActive),
                new XAttribute("CreatedBy", _man.createdBy),
                new XAttribute("CreatedDate", _man.createdDate),
                new XAttribute("LastUpdatedBy", _man.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", _man.lastUpdatedDate.HasValue ? _man.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", _man.isDeleted ?? false),
                new XAttribute("AddressID", _man.addressID));

            return _elem;
        }

        public static ManufacturerData GetManufacturerData(string syncValue)
        {
            if (string.IsNullOrEmpty(syncValue))
                return null;
            try
            {
                var _syncElem = XElement.Parse(syncValue);
                var _manElem = _syncElem.Element("manufacturer");
                if (_manElem != null)
                {
                    ManufacturerData _retValue = new ManufacturerData();
                    _retValue.ManufacturerID = _manElem.Attribute("ManufacturerID").Value;
                    _retValue.Name = _manElem.Attribute("Name").Value;
                    var _contactPerson = _manElem.Attribute("ContactPerson").Value;
                    _retValue.ContactPerson = string.IsNullOrEmpty(_contactPerson) ? null : _contactPerson;
                    var _desc = _manElem.Attribute("Description").Value;
                    _retValue.Description = string.IsNullOrEmpty(_desc) ? null : _desc;
                    _retValue.IsActive = Convert.ToBoolean(_manElem.Attribute("IsActive").Value);
                    _retValue.CreatedBy = _manElem.Attribute("CreatedBy").Value;
                    _retValue.CreatedDate = Convert.ToDateTime(_manElem.Attribute("CreatedDate").Value);
                    var _updatedBy = _manElem.Attribute("LastUpdatedBy").Value;
                    _retValue.LastUpdatedBy = string.IsNullOrEmpty(_updatedBy) ? null : _updatedBy;
                    var _updatedDate = _manElem.Attribute("LastUpdatedDate").Value;
                    _retValue.LastUpdatedDate = string.IsNullOrEmpty(_updatedDate) ? (DateTime?)null : Convert.ToDateTime(_updatedDate);
                    _retValue.IsDeleted = Convert.ToBoolean(_manElem.Attribute("IsDeleted").Value);
                    var _addressId = _manElem.Attribute("AddressID").Value;
                    _retValue.AddressID = string.IsNullOrEmpty(_addressId) ? (Guid?)null : new Guid(_addressId);

                    return _retValue;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XElement GetSupplierElement(string supId)
        {
            if (string.IsNullOrEmpty(supId))
                throw new ArgumentNullException("The supplier ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _sup = (from s in db.Suppliers
                            where s.lkSupplierID.Equals(supId)
                            select s).SingleOrDefault();

                return GetSupplierElement(_sup);
            }
        }

        public static XElement GetSupplierElement(Supplier sup)
        {
            if (sup == null)
                return null;
            XElement _elem = new XElement("supplier", new XAttribute("SupplierID", sup.lkSupplierID),
                new XAttribute("CompanyName", sup.companyName),
                new XAttribute("ContactPerson", sup.contactPerson ?? string.Empty),
                new XAttribute("Description", sup.description ?? string.Empty),
                new XAttribute("IsActive", sup.isActive),
                new XAttribute("CreatedBy", sup.createdBy),
                new XAttribute("CreatedDate", sup.createdDate),
                new XAttribute("LastUpdatedBy", sup.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", sup.lastUpdatedDate.HasValue ? sup.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", sup.isDeleted ?? false),
                new XAttribute("AddressID", sup.addressID));

            return _elem;
        }

        public static SupplierData GetSupplierData(string syncValue)
        {
            if (string.IsNullOrEmpty(syncValue))
                return null;
            try
            {
                var _syncElem = XElement.Parse(syncValue);
                var _suppElem = _syncElem.Element("supplier");
                if (_suppElem != null)
                {
                    SupplierData _retValue = new SupplierData();
                    _retValue.SupplierID = _suppElem.Attribute("SupplierID").Value;
                    _retValue.CompanyName = _suppElem.Attribute("CompanyName").Value;
                    var _contactPerson = _suppElem.Attribute("ContactPerson").Value;
                    _retValue.ContactPerson = string.IsNullOrEmpty(_contactPerson) ? null : _contactPerson;
                    var _desc = _suppElem.Attribute("Description").Value;
                    _retValue.Description = string.IsNullOrEmpty(_desc) ? null : _desc;
                    _retValue.IsActive = Convert.ToBoolean(_suppElem.Attribute("IsActive").Value);
                    _retValue.CreatedBy = _suppElem.Attribute("CreatedBy").Value;
                    _retValue.CreatedDate = Convert.ToDateTime(_suppElem.Attribute("CreatedDate").Value);
                    var _updatedBy = _suppElem.Attribute("LastUpdatedBy").Value;
                    _retValue.LastUpdatedBy = string.IsNullOrEmpty(_updatedBy) ? null : _updatedBy;
                    var _updatedDate = _suppElem.Attribute("LastUpdatedDate").Value;
                    _retValue.LastUpdatedDate = string.IsNullOrEmpty(_updatedDate) ? (DateTime?)null : Convert.ToDateTime(_updatedDate);
                    _retValue.IsDeleted = Convert.ToBoolean(_suppElem.Attribute("IsDeleted").Value);
                    var _addressId = _suppElem.Attribute("AddressID").Value;
                    _retValue.AddressID = string.IsNullOrEmpty(_addressId) ? (Guid?)null : new Guid(_addressId);

                    return _retValue;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XElement GetCustomerElement(string custId)
        {
            if (string.IsNullOrEmpty(custId))
                throw new ArgumentNullException("The customer ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _cust = (from c in db.Customers
                             where c.id.Equals(custId)
                             select c).SingleOrDefault();

                return GetCustomerElement(_cust);
            }
        }

        public static XElement GetCustomerElement(Customer cust)
        {
            if (cust == null)
                return null;
            XElement _elem = new XElement("customer", new XAttribute("ID", cust.id),
                new XAttribute("Name", cust.name),
                new XAttribute("FatherName", cust.fatherName),
                new XAttribute("GrandfatherName", cust.grandfatherName ?? string.Empty),
                new XAttribute("CompanyName", cust.companyName ?? string.Empty),
                new XAttribute("TINNo", cust.TINNo ?? string.Empty),
                new XAttribute("VATRegistrationNo", cust.VATRegistrationNo ?? string.Empty),
                new XAttribute("VATRegistrationDate", cust.VATRegistrationDate.HasValue ? cust.VATRegistrationDate.Value : (DateTime?)null),
                new XAttribute("IsActive", cust.isActive),
                new XAttribute("CreatedBy", cust.createdBy),
                new XAttribute("CreatedDate", cust.createdDate),
                new XAttribute("LastUpdatedBy", cust.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", cust.lastUpdatedDate.HasValue ? cust.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", cust.isDeleted ?? false),
                new XAttribute("AddressID", cust.addressID));

            return _elem;
        }

        public static CustomerData GetCustomerData(string syncValue)
        {
            if (string.IsNullOrEmpty(syncValue))
                return null;
            try
            {
                var _syncElem = XElement.Parse(syncValue);
                var _custElem = _syncElem.Element("customer");
                if (_custElem != null)
                {
                    CustomerData _retValue = new CustomerData();
                    _retValue.ID = _custElem.Attribute("ID").Value;
                    _retValue.Name = _custElem.Attribute("Name").Value;
                    _retValue.FatherName = _custElem.Attribute("FatherName").Value;
                    var _gfName = _custElem.Attribute("GrandfatherName").Value;
                    _retValue.GrandfatherName = string.IsNullOrEmpty(_gfName) ? null : _gfName;
                    var _compName = _custElem.Attribute("CompanyName").Value;
                    _retValue.CompanyName = string.IsNullOrEmpty(_compName) ? null : _compName;
                    var _tinNo = _custElem.Attribute("TINNo").Value;
                    _retValue.TINNo = string.IsNullOrEmpty(_tinNo) ? null : _tinNo;
                    var _vatNo = _custElem.Attribute("VATRegistrationNo").Value;
                    _retValue.VATRegistrationNo = string.IsNullOrEmpty(_vatNo) ? null : _vatNo;
                    var _vatDate = _custElem.Attribute("VATRegistrationDate").Value;
                    _retValue.VATRegistrationDate = string.IsNullOrEmpty(_vatDate) ? (DateTime?)null : Convert.ToDateTime(_vatDate);
                    _retValue.IsActive = Convert.ToBoolean(_custElem.Attribute("IsActive").Value);
                    _retValue.CreatedBy = _custElem.Attribute("CreatedBy").Value;
                    _retValue.CreatedDate = Convert.ToDateTime(_custElem.Attribute("CreatedDate").Value);
                    var _updatedBy = _custElem.Attribute("LastUpdatedBy").Value;
                    _retValue.LastUpdatedBy = string.IsNullOrEmpty(_updatedBy) ? null : _updatedBy;
                    var _updatedDate = _custElem.Attribute("LastUpdatedDate").Value;
                    _retValue.LastUpdatedDate = string.IsNullOrEmpty(_updatedDate) ? (DateTime?)null : Convert.ToDateTime(_updatedDate);
                    _retValue.IsDeleted = Convert.ToBoolean(_custElem.Attribute("IsDeleted").Value);
                    var _addressId = _custElem.Attribute("AddressID").Value;
                    _retValue.AddressID = string.IsNullOrEmpty(_addressId) ? (Guid?)null : new Guid(_addressId);

                    return _retValue;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XElement GetAddressElement(Guid addressId)
        {
            if (addressId == Guid.Empty)
                throw new ArgumentNullException("The address ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _address = (from c in db.Addresses
                                where c.id == addressId
                                select c).SingleOrDefault();

                return GetAddressElement(_address);
            }
        }

        public static XElement GetAddressElement(Address address)
        {
            if (address == null)
                return null;
            XElement _elem = new XElement("address", new XAttribute("ID", address.id),
                new XAttribute("Kebele", address.kebele ?? string.Empty),
                new XAttribute("Woreda", address.woreda ?? string.Empty),
                new XAttribute("Subcity", address.subcity ?? string.Empty),
                new XAttribute("City_Town", address.city_town),
                new XAttribute("Street", address.street ?? string.Empty),
                new XAttribute("HouseNo", address.houseNo ?? string.Empty),
                new XAttribute("PoBox", address.pobox ?? string.Empty),
                new XAttribute("PrimaryEmail", address.primaryEmail ?? string.Empty),
                new XAttribute("SecondaryEmail", address.secondaryEmail ?? string.Empty),
                new XAttribute("State_Region", address.state_region ?? string.Empty),
                new XAttribute("Country", address.country ?? string.Empty),
                new XAttribute("ZipCode_PostalCode", address.zipCode_postalCode ?? string.Empty),
                new XAttribute("AdditionalInfo", address.additionalInfo ?? string.Empty),
                new XAttribute("CreatedBy", address.createdBy),
                new XAttribute("CreatedDate", address.createdDate),
                new XAttribute("LastUpdatedBy", address.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", address.lastUpdatedDate.HasValue ? address.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", address.isDeleted ?? false));

            return _elem;
        }

        public static AddressData GetAddressData(string syncValue)
        {
            if (string.IsNullOrEmpty(syncValue))
                return null;
            try
            {
                var _syncElem = XElement.Parse(syncValue);
                var _addressElem = _syncElem.Element("customer");
                if (_addressElem != null)
                {
                    AddressData _retValue = new AddressData();
                    _retValue.ID = new Guid(_addressElem.Attribute("ID").Value);
                    var _kebele = _addressElem.Attribute("Kebele").Value;
                    _retValue.Kebele = string.IsNullOrEmpty(_kebele) ? null : _kebele;
                    var _woreda = _addressElem.Attribute("Woreda").Value;
                    _retValue.Woreda = string.IsNullOrEmpty(_woreda) ? null : _woreda;
                    var _subcity = _addressElem.Attribute("Subcity").Value;
                    _retValue.Subcity = string.IsNullOrEmpty(_subcity) ? null : _subcity;
                    var _city_town = _addressElem.Attribute("City_Town").Value;
                    _retValue.City_Town = _city_town;
                    var _street = _addressElem.Attribute("Street").Value;
                    _retValue.Street = string.IsNullOrEmpty(_street) ? null : _street;
                    var _houseNo = _addressElem.Attribute("HouseNo").Value;
                    _retValue.HouseNo = string.IsNullOrEmpty(_houseNo) ? null : _houseNo;
                    var _pobox = _addressElem.Attribute("PoBox").Value;
                    _retValue.PoBox = string.IsNullOrEmpty(_pobox) ? null : _pobox;
                    var _pEmail = _addressElem.Attribute("PrimaryEmail").Value;
                    _retValue.PrimaryEmail = string.IsNullOrEmpty(_pEmail) ? null : _pEmail;
                    var _sEmail = _addressElem.Attribute("SecondaryEmail").Value;
                    _retValue.SecondaryEmail = string.IsNullOrEmpty(_sEmail) ? null : _sEmail;
                    var _region = _addressElem.Attribute("State_Region").Value;
                    _retValue.State_Region = string.IsNullOrEmpty(_region) ? null : _region;
                    var _country = _addressElem.Attribute("Country").Value;
                    _retValue.Country = string.IsNullOrEmpty(_country) ? null : _country;
                    var _zip = _addressElem.Attribute("ZipCode_PostalCode").Value;
                    _retValue.ZipCode_PostalCode = string.IsNullOrEmpty(_zip) ? null : _zip;
                    var _info = _addressElem.Attribute("AdditionalInfo").Value;
                    _retValue.AdditionalInfo = string.IsNullOrEmpty(_info) ? null : _info;
                    _retValue.CreatedBy = _addressElem.Attribute("CreatedBy").Value;
                    _retValue.CreatedDate = Convert.ToDateTime(_addressElem.Attribute("CreatedDate").Value);
                    var _updatedBy = _addressElem.Attribute("LastUpdatedBy").Value;
                    _retValue.LastUpdatedBy = string.IsNullOrEmpty(_updatedBy) ? null : _updatedBy;
                    var _updatedDate = _addressElem.Attribute("LastUpdatedDate").Value;
                    _retValue.LastUpdatedDate = string.IsNullOrEmpty(_updatedDate) ? (DateTime?)null : Convert.ToDateTime(_updatedDate);
                    _retValue.IsDeleted = Convert.ToBoolean(_addressElem.Attribute("IsDeleted").Value);

                    return _retValue;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XElement GetTelephoneFaxElement(int id)
        {
            if (id == 0)
                throw new ArgumentNullException("The telephone/fax ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _telephone = (from tf in db.TelephoneFaxes
                                  where tf.id == id
                                  select tf).SingleOrDefault();

                return GetTelephoneFaxElement(_telephone);
            }
        }

        public static XElement GetTelephoneFaxElement(TelephoneFax _telephone)
        {
            if (_telephone == null)
                return null;
            XElement _elem = new XElement("telephoneFax", new XAttribute("ID", _telephone.id),
                new XAttribute("ParentID", _telephone.parentID),
                new XAttribute("Type", _telephone.type),
                new XAttribute("Number", _telephone.number),
                new XAttribute("IsActive", _telephone.isActive),
                new XAttribute("CreatedBy", _telephone.createdBy),
                new XAttribute("CreatedDate", _telephone.createdDate),
                new XAttribute("LastUpdatedBy", _telephone.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", _telephone.lastUpdatedDate.HasValue ? _telephone.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", _telephone.isDeleted ?? false));

            return _elem;
        }

        public static TelephoneFaxData GetTelphoneFaxData(string syncValue)
        {
            if (string.IsNullOrEmpty(syncValue))
                return null;
            try
            {
                var _syncElem = XElement.Parse(syncValue);
                var _teleElem = _syncElem.Element("telephoneFax");
                if (_teleElem != null)
                {
                    TelephoneFaxData _retValue = new TelephoneFaxData();
                    _retValue.ID = Convert.ToInt32(_teleElem.Attribute("ID").Value);
                    _retValue.Type = Convert.ToInt16(_teleElem.Attribute("Type").Value);
                    _retValue.Number = _teleElem.Attribute("Number").Value;
                    _retValue.IsActive = Convert.ToBoolean(_teleElem.Attribute("IsActive").Value);
                    _retValue.CreatedBy = _teleElem.Attribute("CreatedBy").Value;
                    _retValue.CreatedDate = Convert.ToDateTime(_teleElem.Attribute("CreatedDate").Value);
                    var _updatedBy = _teleElem.Attribute("LastUpdatedBy").Value;
                    _retValue.LastUpdatedBy = string.IsNullOrEmpty(_updatedBy) ? null : _updatedBy;
                    var _updatedDate = _teleElem.Attribute("LastUpdatedDate").Value;
                    _retValue.LastUpdatedDate = string.IsNullOrEmpty(_updatedDate) ? (DateTime?)null : Convert.ToDateTime(_updatedDate);
                    _retValue.IsDeleted = Convert.ToBoolean(_teleElem.Attribute("IsDeleted").Value);

                    return _retValue;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XElement GetUserElement(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("The user ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _user = (from u in db.Users
                             where u.userID.Equals(userId)
                             select u).SingleOrDefault();

                return GetUserElement(_user);
            }
        }

        public static XElement GetUserElement(User _user)
        {
            if (_user == null)
                return null;
            XElement _elem = new XElement("user", new XAttribute("UserID", _user.userID),
                new XAttribute("FullName", _user.fullName),
                new XAttribute("Password", _user.password),
                new XAttribute("IsActive", _user.isActive),
                new XAttribute("CreatedBy", _user.createdBy),
                new XAttribute("CreatedDate", _user.createdDate),
                new XAttribute("LastUpdatedBy", _user.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", _user.lastUpdatedDate.HasValue ? _user.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", _user.isDeleted ?? false),
                new XAttribute("AddressID", _user.addressID));

            return _elem;
        }

        public static UserData GetUserData(string syncValue)
        {
            if (string.IsNullOrEmpty(syncValue))
                return null;
            try
            {
                var _syncElem = XElement.Parse(syncValue);
                var _userElem = _syncElem.Element("user");
                if (_userElem != null)
                {
                    UserData _retValue = new UserData();
                    _retValue.UserID = _userElem.Attribute("ManufacturerID").Value;
                    _retValue.FullName = _userElem.Attribute("FullName").Value;
                    _retValue.Password = _userElem.Attribute("Password").Value;
                    _retValue.IsActive = Convert.ToBoolean(_userElem.Attribute("IsActive").Value);
                    _retValue.CreatedBy = _userElem.Attribute("CreatedBy").Value;
                    _retValue.CreatedDate = Convert.ToDateTime(_userElem.Attribute("CreatedDate").Value);
                    var _updatedBy = _userElem.Attribute("LastUpdatedBy").Value;
                    _retValue.LastUpdatedBy = string.IsNullOrEmpty(_updatedBy) ? null : _updatedBy;
                    var _updatedDate = _userElem.Attribute("LastUpdatedDate").Value;
                    _retValue.LastUpdatedDate = string.IsNullOrEmpty(_updatedDate) ? (DateTime?)null : Convert.ToDateTime(_updatedDate);
                    _retValue.IsDeleted = Convert.ToBoolean(_userElem.Attribute("IsDeleted").Value);
                    var _addressId = _userElem.Attribute("AddressID").Value;
                    _retValue.AddressID = string.IsNullOrEmpty(_addressId) ? (Guid?)null : new Guid(_addressId);

                    return _retValue;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XElement GetBranchElement(string branchId)
        {
            if (string.IsNullOrEmpty(branchId))
                throw new ArgumentNullException("The branch ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _branch = (from m in db.Branches
                               where m.id.Equals(branchId)
                               select m).SingleOrDefault();

                return GetBranchElement(_branch);
            }
        }

        public static XElement GetBranchElement(Branch _branch)
        {
            if (_branch == null)
                return null;
            XElement _elem = new XElement("branch", new XAttribute("ID", _branch.id),
                new XAttribute("Name", _branch.name),
                new XAttribute("Description", _branch.description ?? string.Empty),
                new XAttribute("IsActive", _branch.isActive),
                new XAttribute("CreatedBy", _branch.createdBy),
                new XAttribute("CreatedDate", _branch.createdDate),
                new XAttribute("LastUpdatedBy", _branch.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", _branch.lastUpdatedDate.HasValue ? _branch.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", _branch.isDeleted ?? false),
                new XAttribute("AddressID", _branch.addressID),
                new XAttribute("IsDefault", _branch.isDefault));

            return _elem;
        }

        public static BranchData GetBranchData(string syncValue)
        {
            if (string.IsNullOrEmpty(syncValue))
                return null;
            try
            {
                var _syncElem = XElement.Parse(syncValue);
                var _branchElem = _syncElem.Element("branch");
                if (_branchElem != null)
                {
                    BranchData _retValue = new BranchData();
                    _retValue.ID = _branchElem.Attribute("ID").Value;
                    _retValue.Name = _branchElem.Attribute("Name").Value;
                    var _desc = _branchElem.Attribute("Description").Value;
                    _retValue.Description = string.IsNullOrEmpty(_desc) ? null : _desc;
                    _retValue.IsActive = Convert.ToBoolean(_branchElem.Attribute("IsActive").Value);
                    _retValue.CreatedBy = _branchElem.Attribute("CreatedBy").Value;
                    _retValue.CreatedDate = Convert.ToDateTime(_branchElem.Attribute("CreatedDate").Value);
                    var _updatedBy = _branchElem.Attribute("LastUpdatedBy").Value;
                    _retValue.LastUpdatedBy = string.IsNullOrEmpty(_updatedBy) ? null : _updatedBy;
                    var _updatedDate = _branchElem.Attribute("LastUpdatedDate").Value;
                    _retValue.LastUpdatedDate = string.IsNullOrEmpty(_updatedDate) ? (DateTime?)null : Convert.ToDateTime(_updatedDate);
                    _retValue.IsDeleted = Convert.ToBoolean(_branchElem.Attribute("IsDeleted").Value);
                    var _addressId = _branchElem.Attribute("AddressID").Value;
                    _retValue.AddressID = string.IsNullOrEmpty(_addressId) ? (Guid?)null : new Guid(_addressId);
                    _retValue.IsDefault = false;//TODO: The IsDefault property need to be false when sync

                    return _retValue;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XElement GetWarehouseElement(string warehouseId)
        {
            if (string.IsNullOrEmpty(warehouseId))
                throw new ArgumentNullException("The warehouse ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _warehouse = (from m in db.Warehouses
                                  where m.lkWarehouseID.Equals(warehouseId)
                                  select m).SingleOrDefault();

                return GetWarehouseElement(_warehouse);
            }
        }

        public static XElement GetWarehouseElement(Warehouse _warehouse)
        {
            if (_warehouse == null)
                return null;
            XElement _elem = new XElement("warehouse", new XAttribute("WarehouseID", _warehouse.lkWarehouseID),
                new XAttribute("Name", _warehouse.name),
                new XAttribute("Description", _warehouse.description ?? string.Empty),
                new XAttribute("IsActive", _warehouse.isActive),
                new XAttribute("CreatedBy", _warehouse.createdBy),
                new XAttribute("CreatedDate", _warehouse.createdDate),
                new XAttribute("LastUpdatedBy", _warehouse.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", _warehouse.lastUpdatedDate.HasValue ? _warehouse.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", _warehouse.isDeleted ?? false),
                new XAttribute("AddressID", _warehouse.addressID),
                new XAttribute("IsDefault", _warehouse.isDefault));

            return _elem;
        }

        public static WarehouseData GetWarehouseData(string syncValue)
        {
            if (string.IsNullOrEmpty(syncValue))
                return null;
            try
            {
                var _syncElem = XElement.Parse(syncValue);
                var _warehouseElem = _syncElem.Element("warehouse");
                if (_warehouseElem != null)
                {
                    WarehouseData _retValue = new WarehouseData();
                    _retValue.WarehouseID = _warehouseElem.Attribute("WarehouseID").Value;
                    _retValue.Name = _warehouseElem.Attribute("Name").Value;
                    var _desc = _warehouseElem.Attribute("Description").Value;
                    _retValue.Description = string.IsNullOrEmpty(_desc) ? null : _desc;
                    _retValue.IsActive = Convert.ToBoolean(_warehouseElem.Attribute("IsActive").Value);
                    _retValue.CreatedBy = _warehouseElem.Attribute("CreatedBy").Value;
                    _retValue.CreatedDate = Convert.ToDateTime(_warehouseElem.Attribute("CreatedDate").Value);
                    var _updatedBy = _warehouseElem.Attribute("LastUpdatedBy").Value;
                    _retValue.LastUpdatedBy = string.IsNullOrEmpty(_updatedBy) ? null : _updatedBy;
                    var _updatedDate = _warehouseElem.Attribute("LastUpdatedDate").Value;
                    _retValue.LastUpdatedDate = string.IsNullOrEmpty(_updatedDate) ? (DateTime?)null : Convert.ToDateTime(_updatedDate);
                    _retValue.IsDeleted = Convert.ToBoolean(_warehouseElem.Attribute("IsDeleted").Value);
                    var _addressId = _warehouseElem.Attribute("AddressID").Value;
                    _retValue.AddressID = string.IsNullOrEmpty(_addressId) ? (Guid?)null : new Guid(_addressId);
                    _retValue.IsDefault = false;//TODO: The IsDefault property need to be false when sync

                    return _retValue;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XElement GetItemElement(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentNullException("The warehouse ID is empty.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _item = (from i in db.Items
                             where i.itemID.Equals(itemId)
                             select i).SingleOrDefault();

                return GetItemElement(_item);
            }
        }

        public static XElement GetItemElement(Item _item)
        {
            if (_item == null)
                return null;
            XElement _elem = new XElement("item", new XAttribute("ItemID", _item.itemID),
                new XAttribute("BrandID", _item.rlkBrandID),
                new XAttribute("CategoryID", _item.rlkCategoryID),
                new XAttribute("Origin", _item.origin),
                new XAttribute("UnitPrice", _item.unitPrice),
                new XAttribute("ReorderLevel", _item.reorderLevel.HasValue ? _item.reorderLevel.Value.ToString() : string.Empty),
                new XAttribute("PickFaceQty", _item.pickFaceQty.HasValue ? _item.pickFaceQty.Value.ToString() : string.Empty),
                new XAttribute("UnitID", _item.rlkUnitID),
                new XAttribute("OrderQuantity", _item.orderQuantity.HasValue ? _item.orderQuantity.Value.ToString() : string.Empty),
                new XAttribute("Description", _item.description ?? string.Empty),
                new XAttribute("IsActive", _item.isActive),
                new XAttribute("TaxTypeID", _item.rlkTaxTypeID.HasValue ? _item.rlkTaxTypeID.Value.ToString() : string.Empty),
                new XAttribute("IsTaxExempted", _item.isTaxExempted.HasValue ? _item.isTaxExempted.Value.ToString() : string.Empty),
                new XAttribute("CreatedBy", _item.createdBy),
                new XAttribute("CreatedDate", _item.createdDate),
                new XAttribute("LastUpdatedBy", _item.lastUpdatedBy ?? string.Empty),
                new XAttribute("LastUpdatedDate", _item.lastUpdatedDate.HasValue ? _item.lastUpdatedDate.Value.ToString() : string.Empty),
                new XAttribute("IsDeleted", _item.isDeleted ?? false));

            return _elem;
        }

        public static ItemData GetItemData(string syncValue)
        {
            if (string.IsNullOrEmpty(syncValue))
                return null;
            try
            {
                var _syncElem = XElement.Parse(syncValue);
                var _itemElem = _syncElem.Element("warehouse");
                if (_itemElem != null)
                {
                    ItemData _retValue = new ItemData();
                    _retValue.ItemID = _itemElem.Attribute("ItemID").Value;
                    _retValue.BrandID = Convert.ToInt32(_itemElem.Attribute("BrandID").Value);
                    _retValue.CategoryID = Convert.ToInt32(_itemElem.Attribute("CategoryID").Value);
                    _retValue.Origin = _itemElem.Attribute("Origin").Value;
                    _retValue.UnitPrice = Convert.ToSingle(_itemElem.Attribute("UnitPrice").Value);
                    var _reorder = _itemElem.Attribute("ReorderLevel").Value;
                    _retValue.ReorderLevel = string.IsNullOrEmpty(_reorder) ? (double?)null : Convert.ToDouble(_reorder);
                    var _pickFace = _itemElem.Attribute("PickFaceQty").Value;
                    _retValue.PickFaceQty = string.IsNullOrEmpty(_pickFace) ? (double?)null : Convert.ToDouble(_pickFace);
                    _retValue.UnitID = Convert.ToInt32(_itemElem.Attribute("UnitID").Value);
                    var _orderQty = _itemElem.Attribute("OrderQuantity").Value;
                    _retValue.OrderQuantity = string.IsNullOrEmpty(_orderQty) ? (double?)null : Convert.ToDouble(_orderQty);
                    var _desc = _itemElem.Attribute("Description").Value;
                    _retValue.Description = string.IsNullOrEmpty(_desc) ? null : _desc;
                    _retValue.IsActive = Convert.ToBoolean(_itemElem.Attribute("IsActive").Value);
                    var _taxType = _itemElem.Attribute("TaxTypeID").Value;
                    _retValue.TaxTypeID = string.IsNullOrEmpty(_taxType) ? (int?)null : Convert.ToInt32(_taxType);
                    var _isTaxEx = _itemElem.Attribute("IsTaxExempted").Value;
                    _retValue.IsTaxExempted = string.IsNullOrEmpty(_isTaxEx) ? (bool?)null : Convert.ToBoolean(_isTaxEx);
                    _retValue.CreatedBy = _itemElem.Attribute("CreatedBy").Value;
                    _retValue.CreatedDate = Convert.ToDateTime(_itemElem.Attribute("CreatedDate").Value);
                    var _updatedBy = _itemElem.Attribute("LastUpdatedBy").Value;
                    _retValue.LastUpdatedBy = string.IsNullOrEmpty(_updatedBy) ? null : _updatedBy;
                    var _updatedDate = _itemElem.Attribute("LastUpdatedDate").Value;
                    _retValue.LastUpdatedDate = string.IsNullOrEmpty(_updatedDate) ? (DateTime?)null : Convert.ToDateTime(_updatedDate);
                    _retValue.IsDeleted = Convert.ToBoolean(_itemElem.Attribute("IsDeleted").Value);

                    return _retValue;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an xml document object for the existing sync data list
        /// </summary>
        /// <returns>the xml document object</returns>
        public static XDocument GetDocument(string userId)
        {
            //throw new NotImplementedException();
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var syncList = (from s in db.SyncTransactions
                                where !(s.isDeleted ?? false)
                                select s).ToList();

                if (syncList != null && syncList.Count > 0)
                {
                    XNamespace _syncNS = "urn:lst-sync:sync";

                    var _elem = new XElement(_syncNS + "synchronizations",
                        new XAttribute("date", DateTime.Now),
                        new XAttribute("userId", userId) //"LogonUser"),//TODO: ADD LOGON USER HERE
                        );

                    foreach (var sync in syncList)
                    {
                        var _syncElem = new XElement("sync", new XAttribute("id", sync.id),
                            new XAttribute("table", sync.tableName), new XAttribute("action", sync.action),
                            new XAttribute("branch", sync.branchID), XElement.Parse(sync.value));

                        _elem.Add(_syncElem);
                    }

                    XDocument xDoc = new XDocument(new XDeclaration("1.0", "UTF-16", null),
                        _elem);

                    return xDoc;
                }
                return null;
            }
        }
        /// <summary>
        /// Saves the list of sync log records to database
        /// </summary>
        /// <param name="_syncLogList"></param>
        /// <returns></returns>
        public static bool SaveLog(List<SyncLogData> _syncLogList)
        {
            if (_syncLogList == null || _syncLogList.Count == 0)
            {
                return false;
            }
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //get the SyncLog object
                var _syncLog = (from l in _syncLogList
                                select new SyncLog()
                                {
                                    id = l.ID,
                                    date = l.Date,
                                    syncTransactionID = l.SyncTransactionID,
                                    userID = l.UserID,
                                    warehouseID = l.WarehouseID
                                }).ToList();

                db.SyncLogs.AddRange(_syncLog);
                int rows = db.SaveChanges();
                return rows > 0;
            }
        }
    }

    public class SyncLogData
    {
        public Guid ID { get; set; }
        public DateTime? Date { get; set; }
        public string WarehouseID { get; set; }
        public string UserID { get; set; }
        public Guid? SyncTransactionID { get; set; }
    }
}
