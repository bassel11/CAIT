# System Analysis: Credit Notes Module — ERP Integration

---

## SECTION 1: System Summary

### Purpose & Business Goals
Implement a unified **Credit Notes** system into an existing ERP, providing a single mechanism for issuing, tracking, and applying monetary credits to customer accounts across all credit-generating events: product returns, refunds, commercial offers, gifts, coupons, manual adjustments, transaction corrections, and more. The goal is financial consistency, audit traceability, and operational simplicity across all departments.

### Target Users & Roles
| Role | Responsibilities |
|---|---|
| Finance Manager | Approve credit notes, reconcile ledger, configure policies |
| Accounts Receivable Clerk | Issue, apply, and void credit notes manually |
| Sales Representative | Issue offer/gift/coupon credits, apply to orders |
| Customer Service Agent | Process return/refund credit requests |
| System Administrator | Configure types, workflows, approval thresholds |
| Customer (self-service portal) | View and apply credits on their account |
| Auditor | Read-only access to full credit note history |

### Core Modules / Domains
- Credit Note Lifecycle (create → approve → issue → apply/redeem → close/expire)
- Credit Note Types (return, refund, offer, gift, coupon, manual, transaction fix, etc.)
- Customer Credit Ledger (balance tracking per customer)
- Application Engine (apply credits to invoices, orders, or cash)
- Approval Workflow Engine
- Expiry & Policy Management
- Reporting & Reconciliation
- Audit Log

### Key Workflows
1. **Return/Refund Flow** — RMA triggers credit note, links to original invoice, approval by finance, applied to next invoice or refunded to wallet.
2. **Commercial Credit Flow** — Sales creates offer/gift credit, optional approval, customer redeems at checkout or on next order.
3. **Coupon/Promo Flow** — Marketing generates batch coupon-type credit notes with rules (min order, expiry, single-use).
4. **Manual Adjustment Flow** — Finance agent creates manual credit (billing error, goodwill), mandatory approval, applied to account.
5. **Transaction Fix Flow** — System or agent detects transaction anomaly, auto-generates fix credit note, routed for approval, auto-reconciled.

### External Integrations
- ERP General Ledger / Chart of Accounts
- ERP Invoice & Order modules
- ERP Customer master / CRM
- Payment gateway (for cash refund disbursement)
- Notification service (email/SMS to customer)
- Tax engine (credit note tax reversal)
- Reporting / BI layer

### Constraints & Assumptions
- Must not break existing invoice/payment flows
- All credit notes are immutable once issued (void-and-reissue pattern)
- Multi-currency support required
- Partial application of credits must be supported

---

## SECTION 2: Assumptions

1. The ERP already has **Customer**, **Invoice**, **Order**, and **Payment** entities that credit notes will reference.
2. The existing ERP uses a relational database (PostgreSQL or similar).
3. There is an existing **approval/workflow engine** in the ERP that can be extended, or a lightweight one will be built.
4. **Tax reversal** logic on credit notes will mirror the original invoice tax treatment unless overridden.
5. Credit notes are **customer-facing** (B2B and/or B2C) — not inter-company/vendor credits (those are debit notes, out of scope).
6. A credit note has a **monetary value** in the customer's account currency; FX conversion at time of issuance.
7. Credits can be applied to **invoices only**, **new orders only**, or **both** — this is configurable per credit type.
8. Credits have an **optional expiry date** configurable per type.
9. The system supports **partial redemption** (a $100 credit can be split across multiple invoices).
10. **Coupon-type** credits may have redemption rules (min order value, product category restrictions).
11. There is an existing **user authentication and RBAC** system in the ERP.
12. Notifications will use the existing ERP notification service.
13. No mobile app is required; web-only for internal users, and customer self-service portal (web).

---

## SECTION 3: Feature Breakdown

### 1. Credit Note Type Configuration
**1.1** Define and manage credit note types (return, refund, offer, gift, coupon, manual, transaction-fix, custom)
- 1.1.1 Admin creates/edits/deactivates credit note types
- 1.1.2 Per-type configuration: prefix/numbering, approval required (Y/N), approval threshold amount, default expiry days, applicable targets (invoice/order/both), auto-apply flag, tax treatment, GL account mapping
- 1.1.3 System enforces type-specific rules at creation time

### 2. Credit Note Creation
**2.1** Manual creation by authorized user
- 2.1.1 Select customer, credit type, amount, currency, reason, linked document (invoice/order/RMA — optional)
- 2.1.2 Attach supporting documents (PDF, image)
- 2.1.3 System auto-generates unique credit note number (per type prefix + sequence)
- 2.1.4 System calculates tax reversal based on linked invoice or type config

**2.2** Automatic creation triggers
- 2.2.1 RMA approval → auto-generate return/refund credit note
- 2.2.2 Transaction anomaly detection → auto-generate fix credit note
- 2.2.3 Batch coupon generation (marketing campaign) → bulk create coupon-type credit notes

**2.3** Coupon/Gift rule attachment
- 2.3.1 Define redemption rules: min order value, max uses, valid product categories, single-use flag
- 2.3.2 Generate unique redemption codes per coupon credit note

### 3. Approval Workflow
**3.1** Route credit note to approver based on type and amount threshold
- 3.1.1 Notify approver via in-app + email
- 3.1.2 Approver can approve with comments, reject with reason, or request more info
- 3.1.3 Rejection triggers notification to originator; credit note moves to "Rejected" state
- 3.1.4 Auto-approve if amount ≤ configured threshold and type allows it

**3.2** Escalation
- 3.2.1 If not actioned within configurable SLA hours, escalate to next-level approver
- 3.2.2 Log escalation events in audit trail

### 4. Credit Note Lifecycle States
**4.1** States: Draft → Pending Approval → Approved → Issued → Partially Applied → Fully Applied → Expired → Voided
- 4.1.1 State transitions enforced by business rules
- 4.1.2 Only Draft and Pending Approval states are editable
- 4.1.3 Void requires reason and generates a reversal journal entry
- 4.1.4 Expiry job runs daily, transitions Approved/Issued/Partially Applied to Expired if past expiry date

### 5. Customer Credit Ledger
**5.1** Per-customer running balance per currency
- 5.1.1 Ledger updated on issue, application, expiry, and void events
- 5.1.2 Ledger entry linked to originating credit note and application transaction
- 5.1.3 Balance visible to internal users on customer profile
- 5.1.4 Balance visible to customer in self-service portal

### 6. Credit Application Engine
**6.1** Apply credit to open invoice
- 6.1.1 Agent selects invoice(s) and credit note(s), system calculates application amounts
- 6.1.2 Partial application allowed; remaining balance stays on credit note
- 6.1.3 System posts journal entry to GL (debit credit note liability, credit AR)
- 6.1.4 Invoice marked as partially/fully paid

**6.2** Apply credit to new order at checkout
- 6.2.1 Customer or agent selects available credit notes during order creation
- 6.2.2 Coupon code entry field validates against coupon-type credit notes
- 6.2.3 Discount applied to order total; credit note balance reduced

**6.3** Auto-apply
- 6.3.1 If type config has auto-apply = true, system automatically matches credit to oldest open invoice on issuance
- 6.3.2 Auto-apply log entry created for traceability

### 7. Refund Disbursement
**7.1** Cash refund to original payment method
- 7.1.1 Finance agent triggers cash refund from refund-type credit note
- 7.1.2 System calls payment gateway refund API
- 7.1.3 On gateway success, credit note status → Fully Applied, ledger updated
- 7.1.4 On gateway failure, error surfaced with retry option

### 8. Expiry Management
**8.1** Configurable expiry per type and per individual credit note
- 8.1.1 Nightly job scans for expired credits and transitions state
- 8.1.2 Warning notification sent to customer N days before expiry (configurable)
- 8.1.3 Expired credits reversed in GL

### 9. Notifications
**9.1** Customer notifications
- 9.1.1 Credit note issued → email with details and balance
- 9.1.2 Credit applied → email/SMS confirmation
- 9.1.3 Expiry reminder (N days before)
- 9.1.4 Credit voided → email notification with reason

**9.2** Internal notifications
- 9.2.1 Approval request
- 9.2.2 Escalation alert
- 9.2.3 High-value credit note alert (above configurable threshold)

### 10. Customer Self-Service Portal
**10.1** View credit notes and balance
- 10.1.1 List with status, amount, expiry, type, origin document
- 10.1.2 Filter and search
- 10.1.3 Download credit note PDF

**10.2** Apply credit at checkout
- 10.2.1 Select from available credits or enter coupon code

### 11. Reporting & Reconciliation
**11.1** Credit notes by type, status, date range, customer
**11.2** Outstanding credit liability report (total unissued + issued balance)
**11.3** Credits issued vs applied vs expired vs voided reconciliation
**11.4** GL reconciliation report (credit note entries vs ledger)
**11.5** Export to CSV/XLSX

### 12. Audit & Compliance
**12.1** Immutable audit log: every state change, who, when, what
**12.2** Original and voided credit note pairs traceable
**12.3** Full tax reversal trace per credit note
**12.4** Compliance report export

---

## SECTION 4: Jira Task Breakdown

---

### 🗄️ DATABASE

**DB-01**
- **Type:** Task
- **Title:** Design and implement credit_note_types table
- **Description:** Create the `credit_note_types` table to store all configurable type definitions (return, refund, offer, gift, coupon, manual, fix, custom). Includes fields for approval rules, GL mapping, expiry defaults, tax treatment, and redemption rules.
- **Acceptance Criteria:**
  - GIVEN the schema migration runs, WHEN queried, THEN all type config fields are present and constrained correctly
  - GIVEN a duplicate type code insert, WHEN committed, THEN a unique constraint violation is raised
- **Technical Notes:** Fields: id, code, name, requires_approval, approval_threshold, default_expiry_days, applicable_target (enum: invoice/order/both), auto_apply, tax_treatment, gl_account_id, prefix, active, created_at
- **Dependencies:** None
- **Priority:** Critical
- **Story Points:** 2
- **Labels:** database

**DB-02**
- **Type:** Task
- **Title:** Design and implement credit_notes master table
- **Description:** Create the core `credit_notes` table with all lifecycle fields, links to customer, originating document, currency, amounts, state machine column, and audit timestamps.
- **Acceptance Criteria:**
  - GIVEN the migration runs, WHEN a credit note is inserted with all required fields, THEN it persists without error
  - GIVEN a credit note references a non-existent customer_id, WHEN inserted, THEN a FK constraint error is raised
- **Technical Notes:** Fields: id, number (unique), type_id FK, customer_id FK, currency, gross_amount, tax_amount, net_amount, status (enum), reason, notes, linked_invoice_id (nullable FK), linked_order_id (nullable FK), linked_rma_id (nullable FK), expiry_date, issued_at, created_by, approved_by, voided_by, void_reason, parent_credit_note_id (for void-reissue), created_at, updated_at
- **Dependencies:** DB-01
- **Priority:** Critical
- **Story Points:** 3
- **Labels:** database

**DB-03**
- **Type:** Task
- **Title:** Design and implement credit_note_ledger table
- **Description:** Create the `credit_note_ledger` table to track every debit/credit movement per credit note and per customer, enabling running balance calculation and audit traceability.
- **Acceptance Criteria:**
  - GIVEN an application event, WHEN a ledger row is inserted, THEN balance can be correctly summed by customer and currency
  - GIVEN a concurrent application, WHEN two transactions post simultaneously, THEN no double-spend occurs (serializable isolation or optimistic lock)
- **Technical Notes:** Fields: id, credit_note_id FK, customer_id FK, currency, movement_type (enum: issued, applied, voided, expired), amount, reference_type, reference_id, balance_after, created_by, created_at. Index on (customer_id, currency).
- **Dependencies:** DB-02
- **Priority:** Critical
- **Story Points:** 2
- **Labels:** database

**DB-04**
- **Type:** Task
- **Title:** Design and implement credit_note_applications table
- **Description:** Create the `credit_note_applications` junction table recording every application of a credit note against an invoice or order, supporting partial applications and multi-document redemptions.
- **Acceptance Criteria:**
  - GIVEN a partial application, WHEN recorded, THEN applied_amount ≤ remaining credit balance is enforced at DB level via a check or trigger
  - GIVEN an application record, WHEN queried by credit_note_id, THEN all linked invoices/orders are returned
- **Technical Notes:** Fields: id, credit_note_id FK, applied_to_type (enum: invoice/order), applied_to_id, applied_amount, applied_at, applied_by, gl_entry_id
- **Dependencies:** DB-02
- **Priority:** Critical
- **Story Points:** 2
- **Labels:** database

**DB-05**
- **Type:** Task
- **Title:** Design and implement credit_note_approvals table
- **Description:** Persist approval workflow history per credit note, including each approver action, comments, timestamps, and escalation records.
- **Acceptance Criteria:**
  - GIVEN an approval step, WHEN saved, THEN approver_id, action, and timestamp are all populated
  - GIVEN an escalation event, WHEN recorded, THEN escalated_from and escalated_to are both present
- **Technical Notes:** Fields: id, credit_note_id FK, step_order, approver_id FK, action (enum: pending/approved/rejected/escalated), comments, actioned_at, created_at
- **Dependencies:** DB-02
- **Priority:** High
- **Story Points:** 2
- **Labels:** database

**DB-06**
- **Type:** Task
- **Title:** Design and implement coupon_redemption_rules table
- **Description:** Store coupon/gift-type credit note rules: minimum order value, max uses, single-use flag, valid product category IDs, redemption code.
- **Acceptance Criteria:**
  - GIVEN a coupon credit note, WHEN rules are fetched, THEN all configured constraints are returned
  - GIVEN a duplicate redemption code insert, WHEN committed, THEN a unique constraint violation is raised
- **Technical Notes:** Fields: id, credit_note_id FK (unique), redemption_code (unique), min_order_value, max_uses, current_uses, single_use, valid_category_ids (jsonb), valid_from, valid_until
- **Dependencies:** DB-02
- **Priority:** High
- **Story Points:** 2
- **Labels:** database

**DB-07**
- **Type:** Task
- **Title:** Design and implement credit_note_audit_log table
- **Description:** Immutable append-only audit log capturing every state change, field edit, and system event for compliance and forensic tracing.
- **Acceptance Criteria:**
  - GIVEN any state change on a credit note, WHEN the log is queried, THEN the before/after state, actor, and timestamp are present
  - GIVEN a log row insert, WHEN an update or delete is attempted, THEN it is blocked by a DB-level trigger or row-level security
- **Technical Notes:** Fields: id, credit_note_id FK, event_type, actor_id, actor_role, before_state (jsonb), after_state (jsonb), ip_address, created_at. Append-only enforced via trigger blocking UPDATE/DELETE.
- **Dependencies:** DB-02
- **Priority:** Critical
- **Story Points:** 3
- **Labels:** database

---

### ⚙️ BACKEND

**BE-01**
- **Type:** Story
- **Title:** Implement Credit Note Type CRUD service
- **Description:** Build service layer and REST API endpoints for managing credit note types. Used by admins to configure all type parameters including approval thresholds, GL mappings, expiry, and tax treatment.
- **Acceptance Criteria:**
  - GIVEN valid type payload, WHEN POST /credit-note-types is called, THEN type is saved and returned with 201
  - GIVEN an inactive type code, WHEN used in a new credit note, THEN a 422 validation error is returned
- **Technical Notes:** Endpoints: GET/POST /credit-note-types, GET/PUT/DELETE /credit-note-types/:id. Validate GL account exists in chart of accounts. Cache type config in Redis (TTL 10 min).
- **Dependencies:** DB-01
- **Priority:** Critical
- **Story Points:** 3
- **Labels:** backend

**BE-02**
- **Type:** Story
- **Title:** Implement credit note creation service with validation
- **Description:** Core service to create credit notes of any type. Validates business rules per type config, auto-generates credit note number, computes tax reversal, sets initial state (Draft or Pending Approval), and persists to DB.
- **Acceptance Criteria:**
  - GIVEN valid creation payload, WHEN POST /credit-notes is called, THEN credit note is created with correct state and unique number
  - GIVEN a type requiring approval, WHEN created, THEN status = pending_approval and approval workflow is triggered
  - GIVEN a type with auto-approve and amount ≤ threshold, WHEN created, THEN status = issued and ledger entry is created
- **Technical Notes:** Number generation: type.prefix + zero-padded sequence (DB sequence per type). Tax reversal: call existing tax engine with original invoice reference. Emit domain event `CreditNoteCreated`.
- **Dependencies:** DB-02, BE-01
- **Priority:** Critical
- **Story Points:** 5
- **Labels:** backend

**BE-03**
- **Type:** Story
- **Title:** Implement credit note state machine and lifecycle transitions
- **Description:** Enforce all valid state transitions (Draft→PendingApproval→Approved→Issued→PartiallyApplied→FullyApplied/Expired/Voided) with guards, side-effects, and audit logging on every transition.
- **Acceptance Criteria:**
  - GIVEN a credit note in Issued state, WHEN void is requested, THEN it transitions to Voided, a reversal journal entry is posted, and audit log is written
  - GIVEN an invalid transition (e.g. FullyApplied → Draft), WHEN attempted, THEN a 409 Conflict error is returned
- **Technical Notes:** Use a state machine library (e.g. XState on Node, transitions enum on backend). Every transition emits a domain event. Void creates a reversal GL entry via GL integration service.
- **Dependencies:** DB-02, DB-07, BE-02
- **Priority:** Critical
- **Story Points:** 5
- **Labels:** backend

**BE-04**
- **Type:** Story
- **Title:** Implement approval workflow engine for credit notes
- **Description:** Build the approval routing engine: determine approver(s) based on type and amount, create approval records, handle approve/reject/escalation actions, and trigger state transitions.
- **Acceptance Criteria:**
  - GIVEN a credit note above approval threshold, WHEN submitted, THEN correct approver is notified and approval record created
  - GIVEN no action within SLA hours, WHEN the escalation job runs, THEN the credit note is escalated to the next-level approver
  - GIVEN a rejection, WHEN actioned, THEN originator is notified and credit note moves to Rejected state
- **Technical Notes:** Approver resolution uses existing ERP role/delegation config. SLA escalation driven by a scheduled job (BE-10). Emit events `CreditNoteApproved`, `CreditNoteRejected`, `CreditNoteEscalated`.
- **Dependencies:** DB-05, BE-03
- **Priority:** High
- **Story Points:** 5
- **Labels:** backend

**BE-05**
- **Type:** Story
- **Title:** Implement customer credit ledger service
- **Description:** Service to maintain the customer credit ledger: issue credits, record applications, handle expiry debits, and provide current balance queries. Must handle concurrent updates safely.
- **Acceptance Criteria:**
  - GIVEN a credit note issuance, WHEN ledger is updated, THEN balance_after equals prior balance + issued amount
  - GIVEN two concurrent applications of the same credit note, WHEN both execute, THEN total applied never exceeds available balance
  - GIVEN a void event, WHEN processed, THEN ledger debit entry reverses the original credit
- **Technical Notes:** Use optimistic locking or serializable transactions on ledger writes. Expose GET /customers/:id/credit-balance and GET /customers/:id/credit-ledger endpoints.
- **Dependencies:** DB-03, BE-03
- **Priority:** Critical
- **Story Points:** 5
- **Labels:** backend

**BE-06**
- **Type:** Story
- **Title:** Implement credit application engine (invoice & order)
- **Description:** Service to apply one or more credit notes against open invoices or orders, handling partial applications, balance updates, and GL journal posting.
- **Acceptance Criteria:**
  - GIVEN an invoice with $200 outstanding and a $150 credit note, WHEN applied, THEN invoice balance reduces by $150, credit note shows $0 remaining, and correct GL entries are posted
  - GIVEN a credit note with $0 remaining balance, WHEN application is attempted, THEN a 422 error is returned
  - GIVEN a coupon with min_order_value rule, WHEN applied to an order below that value, THEN a 422 error with rule violation message is returned
- **Technical Notes:** POST /credit-notes/:id/apply. Validate redemption rules for coupon types. Post GL entry: DR Credit Note Liability, CR Accounts Receivable / Order Revenue. Emit `CreditNoteApplied` event.
- **Dependencies:** DB-04, BE-05, BE-03
- **Priority:** Critical
- **Story Points:** 5
- **Labels:** backend

**BE-07**
- **Type:** Story
- **Title:** Implement cash refund disbursement via payment gateway
- **Description:** For refund-type credit notes, allow triggering a cash refund to the customer's original payment method via the payment gateway integration. Handle success, failure, and retry.
- **Acceptance Criteria:**
  - GIVEN a valid refund-type credit note in Issued state, WHEN POST /credit-notes/:id/disburse is called, THEN gateway refund API is called and on success credit note transitions to FullyApplied
  - GIVEN a gateway error, WHEN it occurs, THEN credit note remains Issued, error is logged, and a retry is possible
- **Technical Notes:** Use existing payment gateway adapter. Store gateway transaction reference on credit note. Idempotency key = credit_note_id to prevent double refunds. Emit `CreditNoteDisbursed` event.
- **Dependencies:** BE-03, BE-05, existing payment gateway adapter
- **Priority:** High
- **Story Points:** 5
- **Labels:** backend

**BE-08**
- **Type:** Story
- **Title:** Implement bulk coupon/gift credit note generation
- **Description:** Background service to generate batches of coupon-type credit notes from a campaign definition (count, amount, rules, expiry). Generate unique redemption codes and persist all records.
- **Acceptance Criteria:**
  - GIVEN a batch request for 500 coupons, WHEN processed, THEN 500 credit notes are created with unique codes and correct config
  - GIVEN a duplicate code collision (rare), WHEN detected, THEN it is retried with a new code automatically
- **Technical Notes:** Process as a background job (queue). Redemption codes: 8-char alphanumeric, cryptographically random. Return a job_id for status polling. Store batch reference on each generated credit note.
- **Dependencies:** DB-06, BE-02
- **Priority:** Medium
- **Story Points:** 3
- **Labels:** backend

**BE-09**
- **Type:** Story
- **Title:** Implement coupon redemption code validation endpoint
- **Description:** Endpoint to validate a coupon redemption code at order checkout: check existence, active status, not expired, not exceeded max uses, and applicable rules (min order value, categories).
- **Acceptance Criteria:**
  - GIVEN a valid unused coupon code and qualifying order, WHEN validated, THEN the credit note details and applicable discount are returned
  - GIVEN an expired or fully-used coupon code, WHEN validated, THEN a 422 with specific error code is returned
- **Technical Notes:** POST /coupons/validate. Returns: credit_note_id, applicable_amount, rules_summary, expiry_date. Rate-limit to prevent brute-force enumeration (100 req/min per IP).
- **Dependencies:** DB-06, BE-05
- **Priority:** High
- **Story Points:** 3
- **Labels:** backend

**BE-10**
- **Type:** Task
- **Title:** Implement nightly credit note expiry and escalation scheduled jobs
- **Description:** Two cron jobs: (1) scan for credit notes past expiry and transition them to Expired with GL reversal; (2) scan for approval-pending credit notes past SLA and trigger escalation.
- **Acceptance Criteria:**
  - GIVEN a credit note with expiry_date = yesterday, WHEN the expiry job runs at midnight, THEN status transitions to Expired and GL reversal entry is posted
  - GIVEN an approval pending for > SLA hours, WHEN escalation job runs, THEN escalated_to approver is notified and audit log updated
- **Technical Notes:** Use existing ERP job scheduler. Expiry job: batch process in chunks of 500 to avoid lock contention. Log job run results (processed count, errors) to job_runs table.
- **Dependencies:** BE-03, BE-04, BE-05
- **Priority:** High
- **Story Points:** 3
- **Labels:** backend

**BE-11**
- **Type:** Story
- **Title:** Implement credit note GL journal entry integration
- **Description:** Service layer to generate and post double-entry journal entries to the ERP General Ledger for all credit note events: issuance, application, void/reversal, and expiry.
- **Acceptance Criteria:**
  - GIVEN a credit note issuance, WHEN posted, THEN journal entry DR Revenue/Expense, CR Credit Note Liability is created
  - GIVEN a void, WHEN processed, THEN the reversal entry mirrors and offsets the original entry
  - GIVEN a GL posting failure, WHEN it occurs, THEN credit note state does not advance and error is surfaced
- **Technical Notes:** Use existing GL journal service. Journal lines must reference credit_note_id for traceability. Transactional: GL post and state update in the same DB transaction.
- **Dependencies:** BE-03, existing GL service
- **Priority:** Critical
- **Story Points:** 5
- **Labels:** backend

**BE-12**
- **Type:** Story
- **Title:** Implement credit notes reporting and reconciliation API
- **Description:** Build report endpoints: outstanding credit liability, credits issued/applied/expired/voided by period, GL reconciliation, and per-customer credit history. Support CSV/XLSX export.
- **Acceptance Criteria:**
  - GIVEN a date range filter, WHEN GET /reports/credit-notes/summary is called, THEN aggregated totals by type and status are returned
  - GIVEN a GL reconciliation request, WHEN called, THEN credit note ledger totals match GL account balances or discrepancies are flagged
- **Technical Notes:** Heavy queries should run against a read replica or materialized view. CSV export streamed to avoid memory issues on large datasets. Paginate list endpoints (max 200/page).
- **Dependencies:** DB-02, DB-03, DB-04
- **Priority:** Medium
- **Story Points:** 5
- **Labels:** backend

**BE-13**
- **Type:** Story
- **Title:** Implement credit note PDF generation
- **Description:** Generate a formatted, downloadable credit note PDF document for any issued credit note, matching company branding, showing all line items, tax breakdown, applied amounts, and remaining balance.
- **Acceptance Criteria:**
  - GIVEN an issued credit note ID, WHEN GET /credit-notes/:id/pdf is called, THEN a correctly formatted PDF is returned
  - GIVEN a voided credit note, WHEN PDF is requested, THEN a watermark "VOID" is overlaid on the document
- **Technical Notes:** Use existing ERP PDF generation service or introduce Puppeteer/WeasyPrint. Template stored as configurable HTML. Cache generated PDFs in object storage (S3/GCS) keyed by credit_note_id + version.
- **Dependencies:** BE-02
- **Priority:** Medium
- **Story Points:** 3
- **Labels:** backend

**BE-14**
- **Type:** Story
- **Title:** Implement notification dispatch service for credit note events
- **Description:** Consume domain events (CreditNoteIssued, CreditNoteApplied, CreditNoteExpiringSoon, CreditNoteVoided) and dispatch appropriate customer and internal notifications via the existing ERP notification service.
- **Acceptance Criteria:**
  - GIVEN a CreditNoteIssued event, WHEN processed, THEN customer receives an email with credit note details within 60 seconds
  - GIVEN a CreditNoteExpiringSoon event (N days before), WHEN processed, THEN customer receives expiry reminder notification
- **Technical Notes:** Subscribe to domain events via existing ERP event bus. Use existing notification service adapters. Notification templates stored in config. N (days before expiry) configurable per type.
- **Dependencies:** BE-02, BE-03, existing notification service
- **Priority:** Medium
- **Story Points:** 3
- **Labels:** backend

---

### 🖥️ FRONTEND (WEB — INTERNAL ERP UI)

**FE-01**
- **Type:** Story
- **Title:** Build Credit Note Type configuration admin screen
- **Description:** Admin page to create, edit, and deactivate credit note types. Form covers all type parameters: name, code, approval settings, expiry, GL account picker, tax treatment, applicable targets, prefix.
- **Acceptance Criteria:**
  - GIVEN an admin fills all required fields, WHEN they save, THEN the type is created and appears in the types list
  - GIVEN an admin tries to deactivate a type with active open credit notes, WHEN confirmed, THEN a warning is shown listing impacted records
- **Technical Notes:** Use existing ERP form components. GL account picker should be a searchable dropdown sourced from the chart of accounts endpoint. Inline form validation.
- **Dependencies:** BE-01
- **Priority:** High
- **Story Points:** 3
- **Labels:** frontend

**FE-02**
- **Type:** Story
- **Title:** Build Credit Note creation form (universal — all types)
- **Description:** Multi-step form for creating any credit note type. Step 1: select type and customer. Step 2: fill amount, reason, linked document (optional search). Step 3: coupon/gift rules (if applicable). Step 4: review and submit.
- **Acceptance Criteria:**
  - GIVEN user selects a coupon type, WHEN they reach step 3, THEN the coupon rules section is rendered
  - GIVEN the form is submitted, WHEN creation succeeds, THEN the user is redirected to the new credit note detail page with a success toast
  - GIVEN a validation error from the API, WHEN returned, THEN field-level error messages are displayed inline
- **Technical Notes:** Use existing ERP multi-step form or wizard component. Linked document search should use existing invoice/order/RMA search APIs. Amount field should enforce currency format per selected customer's currency.
- **Dependencies:** BE-02, FE-01
- **Priority:** Critical
- **Story Points:** 5
- **Labels:** frontend

**FE-03**
- **Type:** Story
- **Title:** Build Credit Note detail and lifecycle management page
- **Description:** Detail page for a single credit note showing all fields, status, timeline/history, ledger movements, applied invoices/orders, PDF download, and action buttons (approve, reject, apply, void, disburse) based on user role and current state.
- **Acceptance Criteria:**
  - GIVEN a Pending Approval credit note viewed by an approver, WHEN loaded, THEN Approve and Reject buttons are visible and functional
  - GIVEN a Voided credit note, WHEN loaded, THEN all action buttons are hidden and a void banner is shown
  - GIVEN user clicks Download PDF, WHEN the PDF is ready, THEN it opens in a new tab
- **Technical Notes:** Use existing ERP timeline/activity component. Action button visibility driven by state machine rules and RBAC. Void action requires a confirmation modal with mandatory reason field.
- **Dependencies:** BE-02, BE-03, BE-13
- **Priority:** Critical
- **Story Points:** 5
- **Labels:** frontend

**FE-04**
- **Type:** Story
- **Title:** Build Credit Notes list page with filtering and search
- **Description:** List/table view of all credit notes with columns for number, type, customer, amount, currency, status, expiry, created date. Support filter by type, status, date range, customer. Support sorting and pagination.
- **Acceptance Criteria:**
  - GIVEN a user filters by status=Issued and type=Return, WHEN applied, THEN only matching records are shown
  - GIVEN 0 results, WHEN displayed, THEN an appropriate empty state message is shown
- **Technical Notes:** Use existing ERP data table component. Preserve filter state in URL query params for shareability. Debounce search input (300ms).
- **Dependencies:** BE-02
- **Priority:** High
- **Story Points:** 3
- **Labels:** frontend

**FE-05**
- **Type:** Story
- **Title:** Build credit application UI (apply credit to invoice or order)
- **Description:** Modal or inline UI on both the invoice detail page and order detail page allowing authorized users to select available credit notes and apply them, with real-time remaining balance display.
- **Acceptance Criteria:**
  - GIVEN an open invoice and an available credit, WHEN the user selects the credit and confirms, THEN the invoice balance updates on screen and a success notification appears
  - GIVEN a partial application, WHEN applied, THEN the remaining credit balance is shown updated in real-time
- **Technical Notes:** Reuse the credit note search/select component across invoice and order contexts. Show FX warning if credit currency differs from invoice currency.
- **Dependencies:** BE-06, FE-03
- **Priority:** Critical
- **Story Points:** 3
- **Labels:** frontend

**FE-06**
- **Type:** Story
- **Title:** Build customer credit balance widget on customer profile page
- **Description:** Add a credit balance summary widget to the existing customer profile page: total available credit per currency, link to full credit note list filtered by customer.
- **Acceptance Criteria:**
  - GIVEN a customer with credits in 2 currencies, WHEN the profile page loads, THEN both balances are shown with correct currency codes
  - GIVEN a customer with zero credits, WHEN the widget renders, THEN a "No available credit" message is shown
- **Technical Notes:** Widget should be a lazy-loaded component to avoid slowing the customer profile page load. Hook into existing customer profile page extension point.
- **Dependencies:** BE-05
- **Priority:** Medium
- **Story Points:** 2
- **Labels:** frontend

**FE-07**
- **Type:** Story
- **Title:** Build credit notes reporting dashboard
- **Description:** Reporting screen with KPI cards (total outstanding, issued this period, applied this period, expired this period) and a breakdown table by type and status. Include date range picker and export to CSV/XLSX.
- **Acceptance Criteria:**
  - GIVEN the user selects a custom date range, WHEN applied, THEN all KPIs and table data update to match
  - GIVEN user clicks Export, WHEN the file downloads, THEN it contains the full result set (not just current page)
- **Technical Notes:** Use existing ERP chart/KPI components. Export via streaming download endpoint.
- **Dependencies:** BE-12
- **Priority:** Medium
- **Story Points:** 3
- **Labels:** frontend

**FE-08**
- **Type:** Story
- **Title:** Build bulk coupon generation UI
- **Description:** Form for marketing/admin users to generate a batch of coupon-type credit notes: specify campaign name, count, amount per coupon, rules (min order, categories, single-use), expiry date. Show job progress and download codes on completion.
- **Acceptance Criteria:**
  - GIVEN a valid batch form, WHEN submitted, THEN a background job starts and a progress indicator is shown
  - GIVEN job completion, WHEN status is polled, THEN a download link for the generated codes CSV appears
- **Technical Notes:** Poll job status endpoint every 3 seconds until complete or failed. Show error summary if some records fail.
- **Dependencies:** BE-08
- **Priority:** Medium
- **Story Points:** 3
- **Labels:** frontend

---

### 🌐 CUSTOMER SELF-SERVICE PORTAL (WEB)

**PORTAL-01**
- **Type:** Story
- **Title:** Build customer credit notes list page in self-service portal
- **Description:** Allow customers to view all their credit notes with status, amount, expiry, and type. Support filtering by status and date. Provide PDF download per credit note.
- **Acceptance Criteria:**
  - GIVEN a logged-in customer, WHEN they navigate to credits, THEN only their own credit notes are visible
  - GIVEN an expired credit note, WHEN displayed, THEN it is clearly labeled expired and not selectable for application
- **Technical Notes:** Data scoped strictly to authenticated customer via JWT claims. No cross-customer data leakage possible.
- **Dependencies:** BE-02, BE-13
- **Priority:** High
- **Story Points:** 3
- **Labels:** frontend

**PORTAL-02**
- **Type:** Story
- **Title:** Integrate coupon code entry and credit selection at checkout
- **Description:** Add a credit/coupon section to the self-service checkout: enter coupon code (validated in real-time) or select from available credits. Show discount applied and updated order total.
- **Acceptance Criteria:**
  - GIVEN a valid coupon code, WHEN entered, THEN the discount is shown on the order total in real-time
  - GIVEN an invalid/expired code, WHEN entered, THEN a specific user-friendly error message is shown
- **Technical Notes:** Debounce code validation call (500ms). Show remaining balance after application if partial.
- **Dependencies:** BE-09, BE-06, PORTAL-01
- **Priority:** High
- **Story Points:** 3
- **Labels:** frontend

---

### 🔌 API / INTEGRATIONS

**API-01**
- **Type:** Task
- **Title:** Integrate credit note GL events with ERP General Ledger service
- **Description:** Implement the adapter between the credit note GL journal entry service and the existing ERP GL posting API. Handle account mapping, journal line format, and idempotent posting.
- **Acceptance Criteria:**
  - GIVEN a valid journal payload, WHEN posted to the GL service, THEN a journal_id is returned and stored on the credit note
  - GIVEN a duplicate post attempt (same credit_note_id + event_type), WHEN received, THEN the GL service returns the existing journal_id (idempotent)
- **Technical Notes:** Map credit note GL accounts from type config. Use GL service's idempotency header (X-Idempotency-Key = credit_note_id:event_type).
- **Dependencies:** BE-11, existing GL service API
- **Priority:** Critical
- **Story Points:** 3
- **Labels:** api

**API-02**
- **Type:** Task
- **Title:** Integrate with payment gateway for cash refund disbursement
- **Description:** Implement refund call to existing payment gateway adapter, passing original transaction reference and refund amount. Handle partial refunds, gateway errors, and webhook confirmation.
- **Acceptance Criteria:**
  - GIVEN a valid refund request, WHEN gateway accepts it, THEN gateway transaction ID is stored and credit note moves to FullyApplied
  - GIVEN a gateway timeout, WHEN it occurs, THEN the system retries up to 3 times before marking as failed and alerting finance
- **Technical Notes:** Use existing payment gateway adapter interface. Store gateway_refund_id on credit note for reconciliation. Listen for gateway webhook to confirm async refunds.
- **Dependencies:** BE-07, existing payment gateway adapter
- **Priority:** High
- **Story Points:** 3
- **Labels:** api

**API-03**
- **Type:** Task
- **Title:** Integrate credit note creation trigger from RMA approval event
- **Description:** Subscribe to the RMA module's `RMAApproved` domain event and automatically create a return/refund credit note with correct amounts, linked invoice, and customer details.
- **Acceptance Criteria:**
  - GIVEN an RMA is approved, WHEN the event is consumed, THEN a credit note is created with linked_rma_id and status = Pending Approval or Issued per type config
  - GIVEN a duplicate RMA event (at-least-once delivery), WHEN processed again, THEN no duplicate credit note is created (idempotency check on rma_id)
- **Technical Notes:** Idempotency: check credit_notes.linked_rma_id before creating. Use existing ERP event bus subscription.
- **Dependencies:** BE-02, existing RMA module event bus
- **Priority:** High
- **Story Points:** 3
- **Labels:** api

---

### 🔒 SECURITY / COMPLIANCE

**SEC-01**
- **Type:** Task
- **Title:** Implement RBAC permission matrix for credit note operations
- **Description:** Define and enforce role-based access control for all credit note operations: create, view, approve, apply, void, disburse, configure types, view reports. Integrate with existing ERP RBAC system.
- **Acceptance Criteria:**
  - GIVEN a Sales Rep role, WHEN they attempt to access the approval endpoint, THEN a 403 Forbidden is returned
  - GIVEN a Finance Manager role, WHEN they access all endpoints, THEN correct permissions are granted per the defined matrix
- **Technical Notes:** Permission matrix: define in a config file / seed data. Use existing ERP RBAC middleware. Minimum permissions: credit_note:create, credit_note:approve, credit_note:apply, credit_note:void, credit_note:disburse, credit_note_type:manage, credit_note:report.
- **Dependencies:** BE-02, existing RBAC system
- **Priority:** Critical
- **Story Points:** 3
- **Labels:** security

**SEC-02**
- **Type:** Task
- **Title:** Implement coupon code brute-force protection
- **Description:** Rate-limit the coupon validation endpoint per IP and per customer session. Alert on anomalous enumeration patterns.
- **Acceptance Criteria:**
  - GIVEN >100 validation requests from one IP in 60 seconds, WHEN the 101st arrives, THEN a 429 Too Many Requests is returned
  - GIVEN an alert threshold breach, WHEN triggered, THEN a security alert is sent to the operations team
- **Technical Notes:** Use existing API rate-limiting middleware (Redis token bucket). Alert via existing monitoring/alerting system.
- **Dependencies:** BE-09
- **Priority:** High
- **Story Points:** 2
- **Labels:** security

**SEC-03**
- **Type:** Task
- **Title:** Ensure credit note data isolation and scoping in portal API
- **Description:** Audit all customer-facing API endpoints to confirm data is strictly scoped to the authenticated customer. No enumeration attacks possible on credit note numbers or IDs.
- **Acceptance Criteria:**
  - GIVEN customer A's JWT, WHEN they request customer B's credit note ID, THEN a 404 (not 403) is returned to prevent enumeration
  - GIVEN a penetration test on the portal endpoints, WHEN run, THEN no cross-customer data leakage is found
- **Technical Notes:** Use opaque UUIDs for credit note IDs exposed to customers. Always filter by customer_id from JWT claims server-side; never trust client-supplied customer_id.
- **Dependencies:** PORTAL-01, PORTAL-02
- **Priority:** Critical
- **Story Points:** 2
- **Labels:** security

---

### 🏗️ DEVOPS / INFRASTRUCTURE

**DEVOPS-01**
- **Type:** Task
- **Title:** Create and run database migration scripts for credit notes schema
- **Description:** Write and execute versioned migration scripts for all credit note tables (DB-01 through DB-07) using the existing ERP migration tool. Include rollback scripts.
- **Acceptance Criteria:**
  - GIVEN the migration is run on staging, WHEN completed, THEN all tables, indexes, constraints, and triggers exist as designed
  - GIVEN the rollback is run, WHEN completed, THEN all credit note tables are removed without impacting existing ERP tables
- **Technical Notes:** Use existing migration tool (Flyway/Liquibase). Test on a copy of production data before promotion.
- **Dependencies:** DB-01 through DB-07
- **Priority:** Critical
- **Story Points:** 3
- **Labels:** devops

**DEVOPS-02**
- **Type:** Task
- **Title:** Configure scheduled jobs for expiry and escalation in production
- **Description:** Register and configure the nightly credit note expiry and approval escalation cron jobs in the production scheduler. Set up alerting on job failures.
- **Acceptance Criteria:**
  - GIVEN job configuration is deployed, WHEN midnight passes, THEN expiry job runs and logs are visible in the monitoring system
  - GIVEN a job failure, WHEN it occurs, THEN a PagerDuty/Slack alert is triggered within 5 minutes
- **Technical Notes:** Register in existing ERP job scheduler. Set job timeout = 30 min. Log job run outcomes to job_runs table.
- **Dependencies:** BE-10, DEVOPS-01
- **Priority:** High
- **Story Points:** 2
- **Labels:** devops

---

### 🧪 QA / TESTING

**QA-01**
- **Type:** Task
- **Title:** Write integration tests for credit note creation and state machine
- **Description:** Automated integration tests covering all credit note creation paths (all types), state transitions, approval routing, and invalid transition guards.
- **Acceptance Criteria:**
  - GIVEN the test suite runs, WHEN all tests pass, THEN >90% line coverage on the state machine and creation service is achieved
  - GIVEN an invalid state transition is attempted in a test, WHEN the assertion runs, THEN the expected error code is returned
- **Technical Notes:** Use existing ERP test framework. Mock external dependencies (GL service, payment gateway, notification service). Separate test database seeded per test run.
- **Dependencies:** BE-02, BE-03, BE-04
- **Priority:** Critical
- **Story Points:** 5
- **Labels:** qa

**QA-02**
- **Type:** Task
- **Title:** Write integration tests for credit application and ledger balance
- **Description:** Test full credit application flows: full apply, partial apply, over-apply prevention, auto-apply, concurrency safety, and coupon rule enforcement.
- **Acceptance Criteria:**
  - GIVEN a partial application test, WHEN run, THEN remaining balance is correctly calculated
  - GIVEN a concurrent double-spend test, WHEN run, THEN only one application succeeds and the second is rejected
- **Technical Notes:** Concurrency tests must use actual DB transactions, not mocks. Run at least 50 concurrent requests in the double-spend test.
- **Dependencies:** BE-05, BE-06
- **Priority:** Critical
- **Story Points:** 5
- **Labels:** qa

**QA-03**
- **Type:** Task
- **Title:** Write end-to-end tests for key credit note user journeys
- **Description:** E2E tests covering: (1) Return → credit note → apply to invoice, (2) Coupon creation → customer redeems at checkout, (3) Manual credit → approval flow → GL reconciliation.
- **Acceptance Criteria:**
  - GIVEN the E2E suite runs against staging, WHEN all 3 journeys execute, THEN all steps pass including GL entry verification
  - GIVEN a test failure, WHEN investigated, THEN full request/response logs are available in the CI report
- **Technical Notes:** Use existing ERP E2E test framework (Cypress/Playwright). Seed test data programmatically. Verify GL entries via direct DB assertion at end of each journey.
- **Dependencies:** All BE and FE tasks
- **Priority:** High
- **Story Points:** 5
- **Labels:** qa

**QA-04**
- **Type:** Task
- **Title:** Performance test credit notes list and reporting endpoints
- **Description:** Load test the credit note list, customer ledger, and reporting endpoints with realistic data volumes (500K credit notes, 100K customers) to verify response times within SLA.
- **Acceptance Criteria:**
  - GIVEN 500K credit notes in DB, WHEN the list endpoint is called with default pagination (page 1, 50 items), THEN response time < 300ms at p99
  - GIVEN the outstanding credit liability report, WHEN called, THEN it returns within 5 seconds
- **Technical Notes:** Use k6 or existing load test tool. Seed realistic data volumes in performance environment. Index strategy should be validated as part of this test.
- **Dependencies:** BE-12, DEVOPS-01
- **Priority:** Medium
- **Story Points:** 3
- **Labels:** qa

---

## SECTION 5: Risks & Gaps

1. **GL Integration Complexity** — The exact GL account mapping structure in the existing ERP is unknown. Risk: credit note GL entries may need custom mapping per legal entity or cost center, requiring scope expansion.

2. **Concurrency / Double-Spend** — High-traffic environments may see race conditions on credit application. The ledger design uses optimistic locking, but this must be load-tested (QA-02) before go-live.

3. **Multi-Currency FX** — FX conversion rate at time of issuance vs. application may differ. Policy on FX differences (book to FX gain/loss account?) must be confirmed with finance before BE-05 is built.

4. **Tax Reversal Complexity** — If original invoices span multiple tax jurisdictions or tax periods, reversal logic may be more complex than a simple mirror. Tax engine capability needs validation.

5. **Existing RMA Module Integration** — The RMA module's event contract (`RMAApproved`) needs to be confirmed — if it doesn't emit domain events today, API-03 requires RMA module changes (out of scope risk).

6. **Payment Gateway Partial Refunds** — Not all gateways support partial refunds on the original transaction. Fallback strategy (new payment, manual wire) must be defined for each gateway in use.

7. **Customer Portal Scope** — It is assumed the self-service portal already exists and has an authenticated session mechanism. If not, portal authentication is an additional dependency.

8. **Approval Workflow Config** — The number of approval levels (single vs. multi-tier) and the exact approver resolution logic (by role? by amount band? by type?) needs sign-off from finance and operations before BE-04 is built.

9. **Void vs. Credit Note Cancellation** — Business distinction between voiding (post-issuance) and cancelling (pre-issuance Draft/Pending) should be confirmed to avoid confusing end users and auditors.

10. **Data Migration** — Any existing credits, manual refunds, or goodwill adjustments managed outside the ERP (e.g., in spreadsheets) may need a migration plan. Not scoped here.

