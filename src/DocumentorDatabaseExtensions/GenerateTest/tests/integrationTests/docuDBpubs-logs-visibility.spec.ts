// spec: docs/test-plan-docuDBpubs.md
// Scenario 8: Logs Visibility for `docuDBpubs`

import { test, expect } from '@playwright/test';

const BASE = (process.env.ASPIRE_BASE_URL || 'https://localhost:17261/').trim();
const LOGIN = (process.env.ASPIRE_LOGIN_URL || 'https://localhost:17261/login?t=2dc508ebb9e7959d93fda0353ad5a987').trim();

// Utility: find the resource row by name in the Resources table
async function findResourceRow(page: import('@playwright/test').Page, name: string) {
  // Table rows are accessible via role=row; match by name text
  const row = page.getByRole('row', { name: new RegExp(name, 'i') }).first();
  await row.waitFor({ state: 'visible', timeout: 10_000 });
  return row;
}

// Poll for logs appearing (robust against delayed SignalR/stream startup)
async function waitForLogs(page: import('@playwright/test').Page, timeoutMs = 15000): Promise<void> {
  const start = Date.now();
  while (Date.now() - start < timeoutMs) {
    const count = await page.locator('log').count().catch(() => 0);
    if (count > 0) return;
    // Fallback text patterns if custom <log> tag not yet rendered
    const hasTextPattern = await page.getByText(/waiting for resource|application started|info:/i).first().isVisible().catch(() => false);
    if (hasTextPattern) return;
    await page.waitForTimeout(250);
  }
  throw new Error('Timed out waiting for logs to appear');
}

// Utility: assert that no persistent connection error is shown
async function assertNoConnectionErrors(page: import('@playwright/test').Page) {
  const err = page.getByText(/failed to connect|connection error|cannot connect|disconnected/i).first();
  await expect(err).toBeHidden({ timeout: 1000 });
}

test.describe('LogsVisibility', () => {
  test.use({ ignoreHTTPSErrors: true });

  test.beforeEach(async ({ page }) => {
    // Authenticate session
    await page.goto(LOGIN, { waitUntil: 'domcontentloaded' });
    if (!page.url().startsWith(BASE)) {
      await page.goto(BASE, { waitUntil: 'domcontentloaded' });
    }
  });

  test('docuDBpubs logs are visible without connection errors', async ({ page }) => {
    
    // Step 1: goto consolelogs/resource/docuDBpubs
    await  page.goto(BASE+'consolelogs/resource/docuDBpubs', { waitUntil: 'domcontentloaded' }).catch(() => null);

    
  // Step 2: Verify at least one log entry is visible (poll to handle delayed stream)
    await waitForLogs(page, 20000);
    // Fallback: match log entries by known text pattern (robust to custom elements)
    const logLike = page.getByText(/(Waiting for resource|Application started|info:|fail:|DONE documentation|Now listening on:|Health check ready|scaffold ef dbcontext|Found table|Creating table file|restore True|Task completed with result)/i);
    const count = await logLike.count();
    expect(count, 'Expected at least one log-like entry rendered').toBeGreaterThan(0);
    // Check that at least one log-like element has non-empty text
    let hasText = false;
    for (let i = 0; i < count; ++i) {
      const txt = (await logLike.nth(i).innerText()).trim();
      if (txt.length > 0) { hasText = true; break; }
    }
    expect(hasText, 'At least one log entry should contain text').toBeTruthy();    // Expected Results: No persistent connection errors
    await assertNoConnectionErrors(page);
  });
});
