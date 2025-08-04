namespace Pi.OnePort.TCP.Enums.DataTransfer;

// Request = INBOUND in document (request sent to oneport)
// Response = OUTBOUND in document (response sent by oneport)

public enum MessageType
{
    [SerializedValue("7a")]
    NewOrderRequest7A,

    [SerializedValue("7k")]
    OrderAcknowledgementResponse7K,

    [SerializedValue("7e")]
    ExecutionReportResponse7E,

    [SerializedValue("7c")]
    OrderCancelRequest7C,

    [SerializedValue("6a")]
    NewOrderResponse6A,

    [SerializedValue("7m")]
    OrderChangeRequest7M,

    [SerializedValue("7n")]
    OrderChangeResponse7N,

    [SerializedValue("6t")]
    OrderChangeByBrokerResponse6T,

    [SerializedValue("3D")]
    ConfirmCancelDealResponse3D,

    [SerializedValue("7g")]
    TradeReportRequest7G,

    [SerializedValue("7x")]
    ExecutionReportResponse7X,

    [SerializedValue("7u")]
    OrderChangePutThroughRequest7U,

    [SerializedValue("7v")]
    OrderChangeConfirmResponse7V,

    [SerializedValue("6w")]
    OrderChangePutThroughFromBrokerResponse6W,

    [SerializedValue("7q")]
    OrderCancelPutThroughRequest7Q,

    Unknown,
}
