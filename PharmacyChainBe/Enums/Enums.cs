namespace PharmacyChainBe.Enums
{
    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        BankTransfer
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }

    public enum InvoiceStatus
    {
        Draft,
        Finalized,
        Cancelled
    }

    public enum PurchaseRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        ConvertedToOrder,
        Received
    }

    public enum OrderStatus
    {
        PendingSupplierConfirmation,
        Accepted,
        Rejected,
        Completed,
        Cancelled
    }

    public enum DeliveryStatus
    {
        NotStarted,
        Preparing,
        Shipping,
        Delivered,
        Received
    }
}
