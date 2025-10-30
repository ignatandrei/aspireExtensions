# AspireResourceExtensions - Resource Details Test Plan

## Executive Summary

This test plan covers the verification of the "View Details" functionality for the `AspireResource` row in the resources table of the AspireResourceExtensions web application. The plan ensures that the user can locate the resource, open its details, and validate the URLs and their associated metadata.

---

## Test Scenarios

### 1. View Details for AspireResource

#### 1.1 Happy Path: View and Assert URL Table Values

**Assumptions:**  
- User is authenticated and on the main resources page (`https://localhost:17146/`).
- The `AspireResource` row is present in the resources table.

**Steps:**
1. Locate the row in the resources table where the name is `AspireResource`.
2. In the same row, click the three dots (Actions button).
3. In the dropdown menu, select `View details`.
4. In the details panel, locate the table under the "URLs" section.
5. For each row in the URLs table, assert the following:
    - The "Address" column contains a valid URL (e.g., `https://localhost:17146/`, `https://localhost:17146/login?t=...`).
    - The "Text" column contains the expected label (e.g., `BaseUrl`, `LoginUrl`).
    - The "Endpoint name" column contains the expected environment variable (e.g., `ASPIRE_BASE_URL`, `ASPIRE_LOGIN_URL`).

**Expected Results:**
- The details panel opens and displays the URLs table.
- Each row in the URLs table contains the correct address, text, and endpoint name values as configured for the resource.

**Success Criteria:**
- All values in the URLs table match the expected configuration for `AspireResource`.

**Failure Conditions:**
- The details panel does not open.
- The URLs table is missing or incomplete.
- Any value in the table does not match the expected value.

---

### 2. Edge Case: Resource Row Not Present

**Assumptions:**  
- The `AspireResource` row is not present in the table.

**Steps:**
1. Attempt to locate the `AspireResource` row in the resources table.

**Expected Results:**
- The row is not found, and no actions can be performed.

**Success Criteria:**
- The test gracefully reports the missing resource.

**Failure Conditions:**
- The test fails with an unhandled error.

---

### 3. Error Handling: Details Panel Fails to Load

**Assumptions:**  
- The details panel fails to load due to a network or application error.

**Steps:**
1. Click the Actions button for the `AspireResource` row.
2. Select `View details`.
3. Observe the application response.

**Expected Results:**
- An error message is displayed, or the application handles the failure gracefully.

**Success Criteria:**
- The application does not crash and provides user feedback.

**Failure Conditions:**
- The application becomes unresponsive or displays a blank panel.

---

### 4. Validation: URL Table Columns and Data Integrity

**Assumptions:**  
- The details panel and URLs table are visible.

**Steps:**
1. Verify that the URLs table contains the columns: "Address", "Text", "Endpoint name".
2. For each row, check that:
    - The "Address" is a valid URL.
    - The "Text" and "Endpoint name" are not empty.

**Expected Results:**
- All columns are present and populated with valid data.

**Success Criteria:**
- No missing or malformed data in the URLs table.

**Failure Conditions:**
- Any column is missing or contains invalid data.

---

## Notes

- Each scenario is independent and can be executed in any order.
- Negative and edge cases are included to ensure robust coverage.
- The plan assumes a fresh state for each test run.

---
