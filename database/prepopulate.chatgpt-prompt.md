# Goal

I will be creating an API for a demo project that uses a pre-populated SQLite database. The goal is to have you to write the commands to create the tables and then populate them. I'm going to give you the database schema, some requirements, and some necessary edge cases for this demo.

## Data

- All ids should be incremental
- Commission amounts should be stored in cents e.g. $45 = 4500

## Database Schema

### Agent

- AgentId (PK)
- AgentName (TEXT)

### Carrier

- CarrierId (PK)
- CarrierName (TEXT)

### AgentCarrier

- AgentCarrierId (PK)
- AgentId (FK)
- CarrierId (FK)
- WritingNumber (TEXT guid) - the insurance carrier's version of the agent's id

### AgentCarrierCommissionStatement

- AgentCarrierCommissionStatementId (PK)
- AgentCarrierId (FK)
- CommissionAmount (nullable INT)
- StatementDate (TEXT date)

## Data Requirements

## Agent Requirements

- 20 financial advisors (agents)

## Carrier Requirements

- 8 insurance carriers

## AgentCarrierCommissionStatement Requirements

- 12 months of monthly commission statements from each carrier for each Agent, 12 months past based on today
- I want CommissionAmount to vary. I want some Agents that perform really well and a few that perform lackluster, use a normal distribution.
- I do not want the lackluster performs to have absolutely no commission, I want everyone to have commission and statements.

## Necessary Data Edge Cases

### AgentCarrierCommissionStatement Edge Cases

- I want there to be a carrier that goes gradually from normal sales to eventually no sales (indicated by less and less statements)
- I want this same carrier to have no statements with any agents in the last two months. (It is February 2026 now, so none in February 2026 or January 2026)
- I need 157 additional statements (not limited to one agent) to not be in the table *e.g. {{CarrierId}} is missing a statement for {{AgentId}} in August 2025* (weigh more heavily towards the lackluster performers and the insurance carriers with less statements)
- I need 25 statements (not limited to one agent) that have 0 as the commission amount (weigh more heavily towards the lackluster performers and the insurance carriers with less statements)
- I need 7 statements (not limited to one agent) that have null as the commission amount (weigh more heavily towards the lackluster performers and the insurance carriers with less statements)

## Workflow

1. If you have clarifying questions, ask them. Do not write the commands yet. Repeat until you don't have anymore questions.
2. I want you to read the database schema and determine the best data type for the column, unless I already stated/implied one.
3. Write the commands

## Clarifying Questions Workflow

1. Come up with the most common scenarios or context implications in a multiple choice format *e.g. A) Yes B) No, A) 50 B) 75 C) 100*. Always include an *Other* options, unless it's a true or false question.
2. If the answer is in the multiple choice bracket, I will give you the letter to the answer. If it's not, I'll choose the *Other* option and type a response.

## Guardrails

- Do **not** change my schema or invent new columns
- Do **not** take any liberties, unless you format it as a clarifying question and I approve
- Choose reasonable values for the corresponding column and data type
- I will be running your commands in the **Execute SQL section of SQLite Browser**, so I do not need you to use ```BEGIN TRANSACTION;``` or ```COMMIT;```
- Prioritize using CTEs unless the problem is complex enough for you to justify using non-CTE solution
- No negative commission amounts
