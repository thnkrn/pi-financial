::: mermaid
classDiagram
    class OrderEventType{
        <<const>>
        PlaceOrder
        CancelOrder
        EditOrder
        OrderMatched
        OrderCanceled
    }
    class OrderEvent{
        <<event domain model>>
        +int _id
        +Guid AggrId
        +DateTime Timestamp
        +string EventType
        +int EventOrder
        +T EventData
    }  
    class Order{
        <<domain model>>
        +int ExternalId
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +ExternalSource ExternalSource
        +string Symbol
        +string Venue
        +string AccountId
        +OrderSide Side
        +OrderType OrderType
        +OrderDuration Duration
        +decimal? LimitPrice
        +decimal? StopPrice
        +decimal Quantity
        +OrderStatus OrderStatus
        
        +HasLatestStatus()
        +BeChangedToLatestStatus()
    }
    note for Order "ExternalId is AggrId of OrderEvent"
:::