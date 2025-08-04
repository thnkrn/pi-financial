::: mermaid
sequenceDiagram
GE Lambda->>GE DB: Get latest sync time
opt
GE Lambda->>GE DB: Get Order Event by start-end time
end
GE Lambda->>Velexa: Get Order History by start-end time
loop
GE Lambda->>GE DB: Update Order Event if status is matched
GE Lambda->>Notofication: Send notification
end
GE Lambda->>Notofication: Update latest sync time
:::