::: mermaid
sequenceDiagram
    Client->> GE API: Place Order
    GE API->>Wallet API: Get balance
    GE API-->>GE API: Validate balance
    GE API->>GE DB: Save Order Event
    GE API->>Velexa: Place Order
    GE API->>GE DB: Update External-Id of Order Event
    GE API-->>Client: Response action status
:::