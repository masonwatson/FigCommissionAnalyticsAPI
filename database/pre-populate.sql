PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS AgentCarrierCommissionStatement;
DROP TABLE IF EXISTS AgentCarrier;
DROP TABLE IF EXISTS Carrier;
DROP TABLE IF EXISTS Agent;

-- =========================
-- TABLES
-- =========================
CREATE TABLE Agent (
  AgentId   INTEGER PRIMARY KEY AUTOINCREMENT,
  AgentName TEXT NOT NULL
);

CREATE TABLE Carrier (
  CarrierId   INTEGER PRIMARY KEY AUTOINCREMENT,
  CarrierName TEXT NOT NULL
);

CREATE TABLE AgentCarrier (
  AgentCarrierId INTEGER PRIMARY KEY AUTOINCREMENT,
  AgentId        INTEGER NOT NULL,
  CarrierId      INTEGER NOT NULL,
  WritingNumber  TEXT NOT NULL,
  FOREIGN KEY (AgentId)  REFERENCES Agent(AgentId),
  FOREIGN KEY (CarrierId) REFERENCES Carrier(CarrierId)
);

CREATE TABLE AgentCarrierCommissionStatement (
  AgentCarrierCommissionStatementId INTEGER PRIMARY KEY AUTOINCREMENT,
  AgentCarrierId INTEGER NOT NULL,
  CommissionAmount INTEGER NULL,   -- cents; may be 0 or NULL
  StatementDate TEXT NOT NULL,     -- YYYY-MM-DD (always mid-month per requirement)
  FOREIGN KEY (AgentCarrierId) REFERENCES AgentCarrier(AgentCarrierId)
);

-- =========================
-- SEED: AGENTS (20)
-- =========================
WITH RECURSIVE nums(n) AS (
  SELECT 1
  UNION ALL
  SELECT n + 1 FROM nums WHERE n < 20
)
INSERT INTO Agent (AgentName)
SELECT printf('Agent %02d', n)
FROM nums;

-- =========================
-- SEED: CARRIERS (8)
-- CarrierId 8 is the "failing carrier"
-- CarrierId 7 is treated as a weaker carrier for weighting (missing/0/null bias)
-- =========================
WITH RECURSIVE nums(n) AS (
  SELECT 1
  UNION ALL
  SELECT n + 1 FROM nums WHERE n < 8
)
INSERT INTO Carrier (CarrierName)
SELECT printf('Carrier %02d', n)
FROM nums;

-- =========================
-- SEED: AGENTCARRIER (20 x 8 = 160)
-- WritingNumber: deterministic GUID-like string derived from AgentCarrierId
-- =========================
WITH
ac_base AS (
  SELECT
    a.AgentId,
    c.CarrierId,
    ROW_NUMBER() OVER (ORDER BY a.AgentId, c.CarrierId) AS AgentCarrierId
  FROM Agent a
  CROSS JOIN Carrier c
),
ac_guid AS (
  SELECT
    AgentCarrierId,
    AgentId,
    CarrierId,
    -- Deterministic 32 hex chars from a simple LCG on AgentCarrierId, formatted as 8-4-4-4-12
    printf('%032x', ((AgentCarrierId * 1103515245) + 12345) & 2147483647) AS h32
  FROM ac_base
),
ac_final AS (
  SELECT
    AgentCarrierId,
    AgentId,
    CarrierId,
    lower(
      substr(h32,1,8) || '-' ||
      substr(h32,9,4) || '-' ||
      substr(h32,13,4) || '-' ||
      substr(h32,17,4) || '-' ||
      substr(h32,21,12)
    ) AS WritingNumber
  FROM ac_guid
)
INSERT INTO AgentCarrier (AgentCarrierId, AgentId, CarrierId, WritingNumber)
SELECT AgentCarrierId, AgentId, CarrierId, WritingNumber
FROM ac_final;

-- =========================
-- SEED: COMMISSION STATEMENTS
-- Rules implemented:
-- - 12 monthly statement dates ending 2026-02-15 (mid-month)
--   => 2025-03-15 through 2026-02-15
-- - CarrierId=8 gradually declines from 20 agents down to 2 agents (Mar-Dec 2025),
--   and has NO statements in Jan 2026 and Feb 2026.
-- - Additionally remove 157 more statements (weighted toward low performers and weak carriers (7,8))
-- - Set exactly 7 statements to NULL commission (weighted)
-- - Set exactly 25 statements to 0 commission (weighted; slightly biased, not extreme)
-- - All other commissions > 0, normal-ish distribution (12-uniform CLT approximation), no negatives
-- - Performance tiers:
--     Agents 1-2   = high (mean $6000, sd $1800)
--     Agents 3-17  = mid  (mean $2500, sd $1200)
--     Agents 18-20 = low  (mean $900,  sd $600)
-- =========================
WITH
-- Generate 12 months: 2025-03-15 .. 2026-02-15
months(i, StatementDate) AS (
  SELECT 0, date('2026-02-15', '-11 months')
  UNION ALL
  SELECT i + 1, date(StatementDate, '+1 month')
  FROM months
  WHERE i < 11
),
-- Tag agent performance tier + distribution params (in cents)
agent_perf AS (
  SELECT
    AgentId,
    CASE
      WHEN AgentId BETWEEN 1 AND 2 THEN 'high'
      WHEN AgentId BETWEEN 3 AND 17 THEN 'mid'
      ELSE 'low'
    END AS Tier,
    CASE
      WHEN AgentId BETWEEN 1 AND 2 THEN 600000  -- $6000
      WHEN AgentId BETWEEN 3 AND 17 THEN 250000 -- $2500
      ELSE  90000                               -- $900
    END AS MeanCents,
    CASE
      WHEN AgentId BETWEEN 1 AND 2 THEN 180000  -- $1800
      WHEN AgentId BETWEEN 3 AND 17 THEN 120000 -- $1200
      ELSE  60000                               -- $600
    END AS SdCents
  FROM Agent
),
-- Base full grid: 160 AgentCarrier rows x 12 months = 1920 candidates
all_candidates AS (
  SELECT
    ac.AgentCarrierId,
    ac.AgentId,
    ac.CarrierId,
    m.i AS MonthIndex,            -- 0..11
    m.StatementDate
  FROM AgentCarrier ac
  CROSS JOIN months m
),
-- Apply the "failing carrier" rule for CarrierId=8:
-- Keep month indices 0..9 only (Mar-Dec 2025), and keep fewer agents each month:
-- MonthIndex 0 => keep 20 agents
-- MonthIndex 1 => keep 18 agents
-- ...
-- MonthIndex 9 => keep 2 agents
candidates_after_failing_carrier AS (
  SELECT *
  FROM all_candidates
  WHERE
    CarrierId <> 8
    OR (
      CarrierId = 8
      AND MonthIndex <= 9
      AND AgentId <= (20 - (2 * MonthIndex))
    )
),
-- Weight function used for:
-- - selecting extra missing statements
-- - selecting NULL commissions
-- - selecting 0 commissions
weights AS (
  SELECT
    c.AgentCarrierId,
    c.AgentId,
    c.CarrierId,
    c.MonthIndex,
    c.StatementDate,
    ap.Tier,
    ap.MeanCents,
    ap.SdCents,
    -- "weak carriers" = 7 and 8; bias removals/0/null toward these
    (CASE WHEN c.CarrierId IN (7,8) THEN 2 ELSE 0 END) AS WeakCarrierBoost,
    (CASE WHEN ap.Tier = 'low' THEN 3 WHEN ap.Tier = 'mid' THEN 1 ELSE 0 END) AS LowPerfBoost
  FROM candidates_after_failing_carrier c
  JOIN agent_perf ap ON ap.AgentId = c.AgentId
),
weighted_for_ops AS (
  SELECT
    *,
    (1 + WeakCarrierBoost + LowPerfBoost) AS W
  FROM weights
),
-- Pick 157 additional missing statements (remove them)
remove_157 AS (
  SELECT AgentCarrierId, StatementDate
  FROM (
    SELECT
      AgentCarrierId,
      StatementDate,
      ROW_NUMBER() OVER (
        ORDER BY (abs(random()) * 1.0) / W
      ) AS rn
    FROM weighted_for_ops
  )
  WHERE rn <= 157
),
remaining_after_removals AS (
  SELECT w.*
  FROM weighted_for_ops w
  LEFT JOIN remove_157 r
    ON r.AgentCarrierId = w.AgentCarrierId
   AND r.StatementDate  = w.StatementDate
  WHERE r.AgentCarrierId IS NULL
),
-- Pick 7 NULL commission statements (disjoint from removals)
pick_null_7 AS (
  SELECT AgentCarrierId, StatementDate
  FROM (
    SELECT
      AgentCarrierId,
      StatementDate,
      ROW_NUMBER() OVER (
        ORDER BY (abs(random()) * 1.0) / W
      ) AS rn
    FROM remaining_after_removals
  )
  WHERE rn <= 7
),
remaining_after_nulls AS (
  SELECT w.*
  FROM remaining_after_removals w
  LEFT JOIN pick_null_7 n
    ON n.AgentCarrierId = w.AgentCarrierId
   AND n.StatementDate  = w.StatementDate
  WHERE n.AgentCarrierId IS NULL
),
-- Pick 25 zero commission statements (disjoint from removals + nulls)
pick_zero_25 AS (
  SELECT AgentCarrierId, StatementDate
  FROM (
    SELECT
      AgentCarrierId,
      StatementDate,
      ROW_NUMBER() OVER (
        ORDER BY (abs(random()) * 1.0) / W
      ) AS rn
    FROM remaining_after_nulls
  )
  WHERE rn <= 25
),
-- Final rows to insert (everything except the removed 157 and the failing-carrier exclusions)
final_rows AS (
  SELECT
    w.AgentCarrierId,
    w.AgentId,
    w.CarrierId,
    w.StatementDate,
    w.Tier,
    w.MeanCents,
    w.SdCents,
    CASE
      WHEN n.AgentCarrierId IS NOT NULL THEN 'NULL'
      WHEN z.AgentCarrierId IS NOT NULL THEN 'ZERO'
      ELSE 'NORMAL'
    END AS CommissionMode
  FROM remaining_after_removals w
  LEFT JOIN pick_null_7 n
    ON n.AgentCarrierId = w.AgentCarrierId
   AND n.StatementDate  = w.StatementDate
  LEFT JOIN pick_zero_25 z
    ON z.AgentCarrierId = w.AgentCarrierId
   AND z.StatementDate  = w.StatementDate
),
-- Generate a normal-ish random z using CLT (sum of 12 uniforms - 6) ~ N(0,1)
-- uniform u = abs(random()) / 9223372036854775808.0
commissions AS (
  SELECT
    AgentCarrierId,
    StatementDate,
    CommissionMode,
    MeanCents,
    SdCents,
    (
      (
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0) +
        (abs(random()) / 9223372036854775808.0)
      ) - 6.0
    ) AS z
  FROM final_rows
),
prepared_inserts AS (
  SELECT
    AgentCarrierId,
    StatementDate,
    CASE
      WHEN CommissionMode = 'NULL' THEN NULL
      WHEN CommissionMode = 'ZERO' THEN 0
      ELSE
        -- mean + sd*z, clamped to minimum 1 cent, no negatives
        CAST(
          CASE
            WHEN round(MeanCents + (SdCents * z)) < 1 THEN 1
            ELSE round(MeanCents + (SdCents * z))
          END
        AS INTEGER)
    END AS CommissionAmount
  FROM commissions
),
ordered_inserts AS (
  SELECT
    ROW_NUMBER() OVER (ORDER BY AgentCarrierId, StatementDate) AS AgentCarrierCommissionStatementId,
    AgentCarrierId,
    CommissionAmount,
    StatementDate
  FROM prepared_inserts
)
INSERT INTO AgentCarrierCommissionStatement (
  AgentCarrierCommissionStatementId,
  AgentCarrierId,
  CommissionAmount,
  StatementDate
)
SELECT
  AgentCarrierCommissionStatementId,
  AgentCarrierId,
  CommissionAmount,
  StatementDate
FROM ordered_inserts;

-- =========================
-- OPTIONAL: QUICK CHECKS (leave in if you want)
-- =========================
-- 1) Ensure failing carrier (CarrierId=8) has no Jan/Feb 2026 statements
-- SELECT COUNT(*) AS FailingCarrier_JanFeb2026
-- FROM AgentCarrierCommissionStatement s
-- JOIN AgentCarrier ac ON ac.AgentCarrierId = s.AgentCarrierId
-- WHERE ac.CarrierId = 8 AND s.StatementDate IN ('2026-01-15','2026-02-15');

-- 2) Confirm exact NULL and ZERO counts
-- SELECT
--   SUM(CASE WHEN CommissionAmount IS NULL THEN 1 ELSE 0 END) AS NullCount,
--   SUM(CASE WHEN CommissionAmount = 0 THEN 1 ELSE 0 END) AS ZeroCount
-- FROM AgentCarrierCommissionStatement;

-- 3) Confirm how many statements exist overall
-- SELECT COUNT(*) AS TotalStatements FROM AgentCarrierCommissionStatement;

UPDATE Agent
SET AgentName = CASE AgentId
  WHEN 1 THEN 'Ethan Caldwell'
  WHEN 2 THEN 'Olivia Martinez'
  WHEN 3 THEN 'Jackson Reed'
  WHEN 4 THEN 'Sophia Bennett'
  WHEN 5 THEN 'Liam Harper'
  WHEN 6 THEN 'Ava Sullivan'
  WHEN 7 THEN 'Noah Whitaker'
  WHEN 8 THEN 'Isabella Chen'
  WHEN 9 THEN 'Mason Delgado'
  WHEN 10 THEN 'Charlotte Hayes'
  WHEN 11 THEN 'Lucas Montgomery'
  WHEN 12 THEN 'Amelia Brooks'
  WHEN 13 THEN 'Henry Lawson'
  WHEN 14 THEN 'Mia Patel'
  WHEN 15 THEN 'Benjamin Clarke'
  WHEN 16 THEN 'Harper Nguyen'
  WHEN 17 THEN 'Elijah Foster'
  WHEN 18 THEN 'Abigail Turner'
  WHEN 19 THEN 'Daniel Kim'
  WHEN 20 THEN 'Grace Holloway'
END
WHERE AgentId BETWEEN 1 AND 20;

UPDATE Carrier
SET CarrierName = CASE CarrierId
  WHEN 1 THEN 'Pioneer Mutual Assurance'
  WHEN 2 THEN 'Summit Ridge Life'
  WHEN 3 THEN 'HarborStone Financial Group'
  WHEN 4 THEN 'Crescent Valley Insurance'
  WHEN 5 THEN 'Ironwood National Life'
  WHEN 6 THEN 'Prairie Shield Indemnity'
  WHEN 7 THEN 'NorthStar Benefit Solutions'
  WHEN 8 THEN 'SilverLine Risk Partners'
END
WHERE CarrierId BETWEEN 1 AND 8;

------------------------------------------------------------
-- Seeded data is resulting in negatives
-- Replace the negative numbers with 0s
------------------------------------------------------------
UPDATE AgentCarrierCommissionStatement
SET CommissionAmount = 0
WHERE CommissionAmount < 0;

UPDATE AgentCarrierCommissionStatement
SET CommissionAmount = CommissionAmount / 10;