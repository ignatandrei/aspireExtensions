# Aspire Dashboard (Local) — docuDBpubs Resource Test Plan

## Executive Summary

This test plan verifies that the `docuDBpubs` resource is discoverable and healthy in the local Aspire dashboard running at `https://localhost:17261`. It covers authentication, resource discovery, status/health verification, endpoint access, logs visibility, and error handling. The plan is written to be automation-friendly (Playwright-ready) and supports manual execution.

- Scope: Local developer environment; single-machine setup.
- Objective: Assert `docuDBpubs` is running and its key signals (status, endpoints, health, logs) indicate a healthy service.
- Out of scope: Deep functional testing of `docuDBpubs` business logic.

## Assumptions / Starting State

- Fresh local environment; services launched via Aspire with a dashboard available at `https://localhost:17261`.
- Self-signed certificate is expected; tests/automation ignore HTTPS errors when necessary.
- A login endpoint `https://localhost:17261/login?t=<token>` works and authenticates the current session.
- `docuDBpubs` appears as a resource/service in the dashboard UI once the solution has started.
- No prior session cookies; navigation begins unauthenticated.
- Network connectivity to localhost is stable.

## Environments and Config

- Base URL: `ASPIRE_BASE_URL=https://localhost:17261`
- Login URL: `ASPIRE_LOGIN_URL=https://localhost:17261/login?t=<token>`
- Browser: Chromium (or Playwright default browsers), ignoring HTTPS errors for local self-signed certs.

## Primary User Journeys

- Observer: View dashboard, locate `docuDBpubs`, confirm status is Running/Healthy, view basic info and logs, open the service URL.
- Operator: From the resource details, inspect health endpoints, view logs, and verify metrics (if available).

---

## Scenarios

### 1. Dashboard Access Requires Authentication

- Category: Access & Auth
- Purpose: Ensure unauthenticated access redirects or presents a login path.

Steps:
1. Navigate to `https://localhost:17261/` while not logged in.
2. Observe whether a redirect to login or a login affordance is shown.

Expected Results:
- Access is not granted directly to the dashboard without a valid session.
- A login page is presented or an HTTP redirect to `/login?...` occurs.

Success Criteria:
- Login page (or login redirect) is triggered for anonymous users.

Failure Conditions:
- Dashboard content is visible without authentication (if auth is required by design).
- Browser shows TLS error that cannot be bypassed (unexpected for local setup).

---

### 2. Login With Token Grants Dashboard Access

- Category: Access & Auth
- Purpose: Validate that the provided login token URL enables access to the dashboard.

Steps:
1. Navigate to `ASPIRE_LOGIN_URL` (e.g., `https://localhost:17261/login?t=<token>`).
2. Wait for network to be idle.
3. Navigate to `https://localhost:17261/` (if not auto-redirected).

Expected Results:
- Session becomes authenticated.
- Dashboard home renders with resource topology/list.

Success Criteria:
- Dashboard UI is visible; no login prompt is shown.

Failure Conditions:
- Login fails (invalid token/expired), or dashboard remains inaccessible.

---

### 3. Resource Discovery: `docuDBpubs` Is Present in the Dashboard

- Category: Resource Inventory
- Purpose: Ensure the resource appears in the list/topology with a name or ID `docuDBpubs`.

Steps:
1. From the dashboard main view, locate the resources list or topology.
2. Search/scan for `docuDBpubs` by name.

Expected Results:
- A tile/row/node for `docuDBpubs` is visible.
- The item includes a status indicator (color or label), and may show ports/URL.

Success Criteria:
- `docuDBpubs` is listed and selectable.

Failure Conditions:
- `docuDBpubs` is missing from the resource list.

Edge Cases:
- Name variations (case differences), or delayed appearance while service is starting.

---

### 4. Resource Status: `docuDBpubs` Shows Running/Healthy

- Category: Status & Health
- Purpose: Confirm the resource reports an operational state.

Steps:
1. Identify the status indicator for `docuDBpubs` on its tile/row (e.g., color dot or label text such as "Running", "Healthy").
2. Optionally, open the resource details page/panel.

Expected Results:
- Status shows Running/Healthy (or equivalent positive state).
- No error/warning badges are present on the resource.

Success Criteria:
- Positive/green status is shown consistently on list and details views.

Failure Conditions:
- Status shows Stopped/Failed/Degraded or missing status.

---

### 5. Open Service Endpoint for `docuDBpubs`

- Category: Endpoint Access
- Purpose: Validate the resource provides a reachable URL.

Steps:
1. On the `docuDBpubs` resource, click an "Open"/"Browse"/"URL" action (if provided by dashboard) or copy the exposed URL.
2. Open the service URL in a new tab.

Expected Results:
- HTTP 200 OK response with a service root page, landing JSON, or Swagger/health page.
- TLS warning can be bypassed if self-signed; content loads afterward.

Success Criteria:
- The endpoint is reachable and responds successfully.

Failure Conditions:
- Connection refused, timeouts, or 5xx errors.

Edge Cases:
- Endpoint binds to a dynamic port; first access may take a few seconds while warm-up completes.

---

### 6. Health Probes: `/health`, `/ready`, and `/live` (if implemented)

- Category: Status & Health
- Purpose: Confirm that health endpoints indicate a healthy service.

Steps:
1. From the open service base URL, navigate to `/health`.
2. Navigate to `/ready`.
3. Navigate to `/live`.

Expected Results:
- Each endpoint returns HTTP 200 and a body indicating Healthy/Ready/Live.
- Content type may be JSON or plain text depending on implementation.

Success Criteria:
- All implemented health endpoints return success (HTTP 200) and healthy state.

Failure Conditions:
- Any endpoint returns non-200, or payload indicates an unhealthy state.

Notes:
- If only a subset exists (e.g., only `/health`), treat missing endpoints as Not Applicable.

---

### 7. Resource Details: Metadata and Ports

- Category: Resource Details
- Purpose: Verify details view surfaces key metadata for `docuDBpubs`.

Steps:
1. Open the `docuDBpubs` details page/panel.
2. Inspect exposed ports/URLs, environment variables (if shown), and last updated time.

Expected Results:
- Ports/URLs are present and match the endpoint opened in Scenario 5.
- No error banners or missing fields for core metadata.

Success Criteria:
- Details view is coherent and consistent with the list view.

Failure Conditions:
- Details view shows errors or contradicts status in the list view.

---

### 8. Logs Visibility for `docuDBpubs`

- Category: Observability
- Purpose: Ensure the dashboard can show recent logs for the resource.

Steps:
1. Open the logs panel/view for `docuDBpubs`.
2. Verify that new or recent log entries are visible (timestamps within the last few minutes) or appear after a refresh.

Expected Results:
- Logs stream appears without error.
- No persistent errors like "failed to connect".

Success Criteria:
- At least one recent log line is visible or new logs appear after interaction/refresh.

Failure Conditions:
- Logs panel fails to load or shows repeated connection errors.

---

### 9. Error Handling: Resource Not Running (Negative)

- Category: Error Handling
- Purpose: Validate UI messaging when the resource is not healthy.

Precondition:
- Temporarily stop `docuDBpubs` service (if your environment allows) or simulate by disconnecting it.

Steps:
1. Refresh the dashboard while `docuDBpubs` is stopped/unhealthy.
2. Open the resource details.

Expected Results:
- Status changes to Stopped/Failed/Degraded with appropriate visual indication.
- Helpful error message or troubleshooting tips, and disabled "Open" action if not applicable.

Success Criteria:
- Clear, actionable error state in both list and details views.

Failure Conditions:
- UI continues to show Running despite the resource being down.

Notes:
- If stopping is not possible, mark this scenario as Not Executed (N/E) for local-only runs.

---

### 10. Security: Session Persistence and Re-authentication

- Category: Access & Auth
- Purpose: Verify that a valid session persists and that logout requires re-authentication.

Steps:
1. After a successful login, close and reopen the tab to `https://localhost:17261/`.
2. If a logout option exists, log out and attempt to access the dashboard again.

Expected Results:
- Session persists across tab reopen (within its lifetime).
- After logout, access requires login again.

Success Criteria:
- Session behavior is consistent and secure.

Failure Conditions:
- Session unexpectedly expires immediately, or logout doesn’t revoke access.

---

## Acceptance Criteria (Summary)

- `docuDBpubs` is visible in the dashboard and reports Running/Healthy.
- Its primary endpoint is reachable and returns HTTP 200.
- Health endpoints (available subset of `/health`, `/ready`, `/live`) return success and indicate health.
- Logs are visible without connection errors.
- Auth flow works: unauthenticated users must log in; authenticated users can see the dashboard.

## Automation Notes (Playwright)

- Use `ignoreHTTPSErrors: true` for local HTTPS with self-signed certs (already present in `seed.spec.ts`).
- Environment variables:
  - `ASPIRE_BASE_URL` and `ASPIRE_LOGIN_URL` can be supplied at runtime.
- Robust selectors:
  - Prefer locating by text for the resource name: e.g., `page.getByText('docuDBpubs', { exact: false })`.
  - Pair name with a nearby status badge: use locator chaining to find sibling elements containing `Running` or an aria-label indicating green/healthy.
- Health checks:
  - Once base service URL is known (from details), `await page.goto(`${serviceUrl}/health`)` and assert status code 200.

Example skeleton (pseudo-code for a spec):

```ts
import { test, expect } from '@playwright/test';

const BASE = process.env.ASPIRE_BASE_URL ?? 'https://localhost:17261';
const LOGIN = process.env.ASPIRE_LOGIN_URL ?? 'https://localhost:17261/login?t=<token>';

// Helper to open a known service URL if dashboard exposes it
async function openService(page, resourceName: string) {
  const card = page.getByText(resourceName, { exact: false }).first();
  await expect(card).toBeVisible();
  // Adjust selectors to match your dashboard UI controls
  const openBtn = card.getByRole('link', { name: /open|browse|url/i }).first();
  if (await openBtn.isVisible()) {
    const [newPage] = await Promise.all([
      page.context().waitForEvent('page'),
      openBtn.click()
    ]);
    await newPage.waitForLoadState('domcontentloaded');
    return newPage;
  }
  // Fallback: go to details then follow the exposed URL
  await card.click();
  const urlLink = page.getByRole('link', { name: /http/i }).first();
  const url = await urlLink.getAttribute('href');
  const [newPage] = await Promise.all([
    page.context().waitForEvent('page'),
    urlLink.click()
  ]);
  await newPage.waitForLoadState('domcontentloaded');
  return newPage;
}

test.describe('docuDBpubs running', () => {
  test.use({ ignoreHTTPSErrors: true });

  test.beforeAll(async ({ page }) => {
    await page.goto(LOGIN);
    await page.waitForLoadState('networkidle');
  });

  test('resource visible and healthy', async ({ page }) => {
    await page.goto(BASE);
    const resource = page.getByText('docuDBpubs', { exact: false }).first();
    await expect(resource).toBeVisible();

    // Assert status text or badge; adjust to actual DOM
    const status = resource.getByText(/running|healthy/i);
    await expect(status).toBeVisible();
  });

  test('endpoint reachable and health OK', async ({ page }) => {
    await page.goto(BASE);
    const servicePage = await openService(page, 'docuDBpubs');
    await expect(servicePage).toHaveURL(/https?:\/\//);

    // Check /health (if implemented)
    const url = servicePage.url().replace(/\/$/, '');
    await servicePage.goto(`${url}/health`, { waitUntil: 'domcontentloaded' });
    // If server returns non-200 with an HTML page, Playwright still loads but response status can be asserted via request context in a fuller impl.
    await expect(servicePage).toHaveURL(/\/health$/);
  });
});
```

## Execution Tips

- If the dashboard uses dynamic content, add small `locator.waitFor()` between navigation and assertions.
- For flaky initial loads, retry the resource discovery after a short delay (service warm-up).
- Capture console errors in the browser context to help diagnose unexpected UI failures.

## Reporting

- Record pass/fail per scenario with evidence: URLs visited, observed status text, and any error messages.
- For failures, include the dashboard status indicator text and a timestamp.

## Risks & Mitigations

- Self-signed certs can block navigation: configure Playwright to ignore HTTPS errors.
- Dynamic ports or delayed service start: implement waits/retries and discover URL from the dashboard details rather than hardcoding.
- Token expiration: refresh `ASPIRE_LOGIN_URL` token if authentication fails.

## Appendix: Quick-Start Commands (PowerShell)

```powershell
# Set environment for a session
$env:ASPIRE_BASE_URL = 'https://localhost:17261'
$env:ASPIRE_LOGIN_URL = 'https://localhost:17261/login?t=<token>'

# Install deps (if needed)
# npm ci

# Run all tests (example)
# npx playwright test

# Run a single file (after you create it)
# npx playwright test tests/docuDBpubs.spec.ts --headed
```
