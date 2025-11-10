// spec: docs/test-plan-docuDBpubs.md
// seed: tests/seed.spec.ts
// Scenario 3: Resource Discovery: `docuDBpubs` Is Present in the Dashboard

import { test, expect } from '@playwright/test';

const BASE = (process.env.ASPIRE_BASE_URL || 'https://localhost:17261').trim();
const LOGIN = (process.env.ASPIRE_LOGIN_URL || 'https://localhost:17261/login?t=2dc508ebb9e7959d93fda0353ad5a987').trim();

// Small utility to probe for a visible element among candidate locators
async function anyVisible(locators: import('@playwright/test').Locator[]) {
  for (const l of locators) {
    try {
      if (await l.first().isVisible()) return true;
    } catch {}
  }
  return false;
}

test.describe('Resource Discovery', () => {
  test.use({ ignoreHTTPSErrors: true });

  test.beforeEach(async ({ page }) => {
    // Establish authenticated session via token URL
    await page.goto(LOGIN, { waitUntil: 'domcontentloaded' });
    // Ensure we're on the dashboard base after auth
    if (!page.url().startsWith(BASE)) {
      await page.goto(BASE, { waitUntil: 'domcontentloaded' });
    }
  });

  test('docuDBpubs Is Present in the Dashboard', async ({ page }) => {
    // 1. From the dashboard main view, locate the resources list or topology.
    // Heuristics for common resource containers — adjust if your UI exposes testids/roles.
    const topologyMarkers = [
      page.getByRole('heading', { name: /resources|services|topology/i }),
      page.locator('[data-testid="resource-list"]'),
      page.locator('[class*="resource-list" i]'),
      page.locator('[role="list" i]')
    ];
    // Non-fatal: proceed even if markers are not present; resource text search may still succeed
    await anyVisible(topologyMarkers);

    // 2. Search/scan for `docuDBpubs` by name by targeting the accessible row
    const resourceName = 'docuDBpubs';
    const row = page.getByRole('row', { name: new RegExp(resourceName, 'i') }).first();

    // Wait up to 10s for the row to appear (warm-up / delayed render)
    await row.waitFor({ state: 'visible', timeout: 10_000 });

    // Verification: The row for `docuDBpubs` is visible.
    await expect(row, 'Expected resource row to be visible in the dashboard').toBeVisible();

    
    // Optional: presence of URL/port text (non-fatal soft check)
    const urlLike = row.getByText(/https?:\/\//i).first();
    if (await urlLike.isVisible().catch(() => false)) {
      const text = await urlLike.innerText();
      expect(text).toMatch(/https?:\/\//i);
    }

    // Final: Ensure the resource is selectable (try clicking the name cell)
    const nameCell = row.getByRole('gridcell', { name: /docuDBpubs/i }).first();
    if (await nameCell.isVisible().catch(() => false)) {
      await nameCell.click({ trial: true });
      await nameCell.click();
    } else {
      // Fallback: click the row body if name cell is not individually clickable
      await row.click({ trial: true });
      await row.click();
    }
  });
});
