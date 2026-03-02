# Database Schema

## Agent

- AgentId (PK)
- AgentName (TEXT)

## Carrier

- CarrierId (PK)
- CarrierName (TEXT)

## AgentCarrier

- AgentCarrierId (PK)
- AgentId (FK)
- CarrierId (FK)
- WritingNumber (TEXT guid) - the insurance carrier's version of the agent's id

## AgentCarrierCommissionStatement

- AgentCarrierCommissionStatementId (PK)
- AgentCarrierId (FK)
- CommissionAmount (nullable INT)
- StatementDate (TEXT date)