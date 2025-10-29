# Console Logs Resource – Three-Dots (Kebab) Menu Test Plan

## Executive Summary

This plan covers functional and UX testing of the three-dots (kebab) menu on the Console Logs resource page at:
- Login seed: `https://localhost:17027/login?t=1073b270a9dc7c52b67380f1c228f355`
- Target page: `https://localhost:17027/consolelogs/resource/SampleTests`

The goal is to verify the kebab menu is discoverable, accessible, and exposes the expected actions without errors. The plan includes happy paths, edge cases, and validation of error handling and keyboard accessibility. Tests are designed to be independent and run in any order.

Assumptions:
- Fresh browser context for each scenario.
- The token in the login URL remains valid during test execution.
- The site uses HTTPS with a localhost/self-signed certificate (tests ignore cert errors).
- The resource page is reachable and renders the kebab menu associated with the Console Logs resource.

## Primary User Journeys

- Viewer user: Can open the kebab menu, preview non-destructive actions (e.g., view logs, copy link), and dismiss menu.
- Editor/Operator user: Can open the kebab menu and may see additional actions (e.g., tail logs, download, clear logs) if authorized.

Critical path:
1. Authenticate via token login URL.
2. Navigate to Console Logs resource page.
3. Locate and open the kebab menu.
4. Review the list of actions.
5. Dismiss the menu or perform safe actions.

## Test Scenarios

### 1. Kebab Menu: Visibility and Basic Operation (Happy Path)

Seed: `tests/consolelogs-menu.spec.ts`

Starting state:
- Fresh browser context, no prior cookies or storage.

Steps:
1. Navigate to `https://localhost:17027/login?t=1073b270a9dc7c52b67380f1c228f355`.
2. After login completes (implicit or redirect), navigate to `https://localhost:17027/consolelogs/resource/SampleTests`.
3. Locate the three-dots (kebab) menu button near the Console Logs resource header/toolbar.
4. Click the menu button.

Expected Results:
- The Console Logs resource page loads without errors.
- The kebab menu button is visible and enabled.
- A popover menu opens displaying at least one actionable item.

Success Criteria:
- Menu opens reliably and is not obstructed.
- No console/network errors related to opening the menu.

Failure Conditions:
- Menu button not found or disabled.
- Menu fails to open or shows an empty list.

---

### 2. Menu Items: Presence, Labels, and Readability (Happy Path)

Starting state:
- Fresh context; user logged in via seed.

Steps:
1. Repeat Steps 1–4 of Scenario 1 to open the menu.
2. Enumerate all visible menu items.
3. Verify text labels are non-empty and readable.

Expected Results:
- At least one menu item is present.
- Every item has a non-empty label or an accessible name.

Success Criteria:
- All visible items are legible and not clipped/overflowing.

Failure Conditions:
- No items present, or labels are blank/unreadable.

---

### 3. Accessibility: Keyboard Navigation

Starting state:
- Fresh context; user logged in via seed.

Steps:
1. Focus the menu button via keyboard (Tab navigation) or programmatically.
2. Press Enter or Space to open the menu.
3. Use ArrowDown/ArrowUp to cycle through items.
4. Press Escape to dismiss the menu.

Expected Results:
- Menu opens with Enter/Space.
- Focus moves between items with arrow keys.
- Escape dismisses the menu (or otherwise deactivates it).

Success Criteria:
- No keyboard traps; focus returns to a sensible element after dismiss.

Failure Conditions:
- Menu cannot be opened via keyboard.
- Arrow keys do not move focus; focus gets lost.

---

### 4. Robust Discovery: Locator Heuristics and Fallbacks

Starting state:
- Fresh context; user logged in via seed.

Steps:
1. Attempt to find the menu via accessible role and typical labels (e.g., "More", "More options", "Menu").
2. If not found, try fallbacks: aria-label includes "more"/"options", text "⋮", "…", or data-testid containing "kebab".
3. Open the menu using mouse click; if it fails, try Enter.

Expected Results:
- The button is discoverable by at least one strategy.
- Menu opens without errors.

Success Criteria:
- No unhandled exceptions while discovering the button.

Failure Conditions:
- All strategies fail to locate the menu.

---

### 5. Error Handling: Invalid/Expired Token

Starting state:
- Fresh context; use an intentionally invalid token.

Steps:
1. Navigate to `https://localhost:17027/login?t=invalid`.
2. Attempt to navigate to the resource page.

Expected Results:
- User is redirected to a login page or receives an appropriate error.
- Resource page is not accessible without valid auth.

Success Criteria:
- Clear error message or redirect flow; no blank screens.

Failure Conditions:
- Silent failure, infinite redirect loop, or cryptic error without guidance.

---

### 6. Authorization: Menu Items Based on Role (Edge Case)

Starting state:
- Fresh context; test with different roles if available.

Steps:
1. Login as a viewer-only role.
2. Open menu and record items.
3. Login as an editor/operator.
4. Open menu and compare items.

Expected Results:
- Editor sees additional actions; viewer does not see restricted items.

Success Criteria:
- Role-based differences are consistent and documented.

Failure Conditions:
- Restricted actions visible to unauthorized users; actions error on use.

---

### 7. Resilience: Menu Stability on Resize/Scroll (Edge Case)

Starting state:
- Fresh context; logged in.

Steps:
1. Open menu.
2. Resize viewport to narrow width.
3. Scroll the page, then reopen menu.

Expected Results:
- Menu remains usable; repositions correctly and stays within viewport.

Success Criteria:
- No clipped or off-screen menu.

Failure Conditions:
- Menu renders off-screen or becomes unreachable.

---

### 8. Non-Destructive Interaction: Safe Exploration

Starting state:
- Fresh context; logged in.

Steps:
1. Open menu and hover each item (do not click destructive actions).
2. If tooltips or descriptions appear, verify clarity.

Expected Results:
- Hovering does not trigger unintended actions.
- Tooltips/descriptions are accurate and helpful.

Success Criteria:
- No state changes from hover.

Failure Conditions:
- Hover triggers actions or state mutation.

---

### 9. Internationalization/Localization (If Applicable)

Starting state:
- Fresh context; logged in.

Steps:
1. Switch locale (if supported).
2. Open menu and verify labels adapt to the locale.

Expected Results:
- Menu labels are localized and do not overflow.

Success Criteria:
- No mixed-language artifacts.

Failure Conditions:
- Missing translations or layout breaks.

## How to Run (PowerShell)

```powershell
# From repo root of this test package
npm ci

# Run all tests
npx playwright test

# Run only the console logs menu spec
npx playwright test tests/consolelogs-menu.spec.ts

# View the HTML report
npx playwright show-report

# Override defaults with env vars
$env:ASPIRE_BASE_URL = 'https://localhost:17027'
$env:ASPIRE_LOGIN_URL = 'https://localhost:17027/login?t=YOUR_TOKEN_HERE'
$env:ASPIRE_RESOURCE_URL = 'https://localhost:17027/consolelogs/resource/SampleTests'
npx playwright test tests/consolelogs-menu.spec.ts
```

## Notes
- Tests ignore HTTPS errors on a per-file basis due to localhost/self-signed certs.
- Tests enumerate menu items without clicking them to avoid destructive actions. You can extend with explicit item selectors once labels are confirmed.
- Keep the login token valid during the test session; prefer environment variables for security and flexibility.
