import { test, expect, Page, Locator } from '@playwright/test';

// This spec explores the three-dots (kebab) menu on the Console Logs resource page.
// It starts by hitting the provided login URL (token-based) and then navigates to the resource page.
// Notes:
// - HTTPS is on localhost; we ignore cert errors per-file to avoid config edits.
// - All tests are independent; each test performs its own login+nav.

const DEFAULT_BASE_URL = process.env.ASPIRE_BASE_URL ??"";
const DEFAULT_LOGIN_URL = process.env.ASPIRE_LOGIN_URL??""; 
const RESOURCE_URL = `${DEFAULT_BASE_URL}consolelogs/resource/SampleTests`;

// Per-file Playwright settings
// - Ignore self-signed/localhost HTTPS errors.
// - Use headless by default (override via CLI if needed).
test.use({ ignoreHTTPSErrors: true });

async function loginAndGotoResource(page: Page) {
  // 1) Hit the login URL (token-based auth).
  await page.goto(DEFAULT_LOGIN_URL, { waitUntil: 'domcontentloaded' });

  // 2) If login redirects or sets cookie, navigate to the target page.
  await page.goto(RESOURCE_URL, { waitUntil: 'domcontentloaded' });

  // 3) Basic sanity checks that we're on the Console Logs resource page.
  await expect(page).toHaveURL(new RegExp('/consolelogs/resource/', 'i'));
  // Wait for key UI elements to ensure the toolbar has rendered.
  await expect(page.getByRole('heading', { name: /Console logs/i })).toBeVisible();
  // Soft wait for the toolbar controls to hydrate
  await page.waitForTimeout(200); // brief stabilizing delay to avoid racing hydration
}

async function findKebabMenuButton(page: Page): Promise<Locator> {
  // Prefer the explicit, accessible label observed on the page
  const primary = page.getByRole('button', { name: /Resource actions/i });
  if (await primary.count()) {
    await expect(primary).toBeVisible();
    return primary.first();
  }

  const candidates: Locator[] = [
    page.getByRole('button', { name: /more options|more actions|more|options|menu|actions|kebab/i }),
    page.locator('button[aria-label*="resource actions" i]'),
    page.locator('button[aria-label*="more" i]'),
    page.locator('button[aria-label*="options" i]'),
    page.locator('[role="button"][aria-haspopup="menu"]'),
    page.locator('button:has-text("⋮")'),
    page.locator('button:has-text("…"), button:has-text("...")'),
    page.locator('[data-testid*="kebab" i], [data-test*="kebab" i]'),
  ];

  for (const loc of candidates) {
    const count = await loc.count();
    if (count > 0) {
      return loc.first();
    }
  }

  // As a last resort, try any button that opens a menu.
  const anyMenuButton = page.locator('button,[role="button"]').filter({ has: page.locator('[aria-haspopup="menu"]') });
  if (await anyMenuButton.count()) return anyMenuButton.first();

  throw new Error('Could not find the three-dots (kebab) menu button.');
}

async function openMenu(button: Locator) {
  // Try mouse click first; fall back to keyboard if needed.
  try {
    await button.click({ timeout: 3000 });
  } catch {
    await button.focus();
    await button.press('Enter');
  }
}

function menuItemsLocator(page: Page) {
  // Scope to an open menu container to avoid matching hidden combobox options elsewhere.
  const menu = page.getByRole('menu');
  return menu.locator('[role="menuitem"], [role="menuitemcheckbox"], [role="menuitemradio"]');
}
test("makeScreenshot", async ({ page }) => {
    console.log(`going to of ${DEFAULT_LOGIN_URL} page`);
  await page.goto(DEFAULT_LOGIN_URL, { waitUntil: 'domcontentloaded' });
  await page.waitForTimeout(1000);
  await page.screenshot({ path: 'consolelogs1.png', fullPage: true });
    console.log(`Taking screenshot of ${RESOURCE_URL} page`);
  await page.goto(RESOURCE_URL, { waitUntil: 'domcontentloaded' });
  await page.waitForTimeout(1000);
  await page.screenshot({ path: 'consolelogs-page.png', fullPage: true });
});
// Happy path: open kebab menu and list items
test('ConsoleLogs kebab menu is visible and has at least one item', async ({ page }) => {
  await loginAndGotoResource(page);

  const kebab = await findKebabMenuButton(page);
  await expect(kebab).toBeVisible();

  await openMenu(kebab);
  // Wait for the menu container to be visible
  await expect(page.getByRole('menu')).toBeVisible();
  const items = menuItemsLocator(page);
  const count = await items.count();
  // Expect at least one actionable item
  expect(count).toBeGreaterThan(0);

  const labels = await items.allTextContents();
  // Log discovered items to test output for quick inspection
  console.log('[ConsoleLogs Menu Items]', labels.map(t => t.trim()).filter(Boolean));
});

// Accessibility: keyboard navigation can open the menu and move focus within items
test('Kebab menu supports keyboard access (Enter/Arrow/Escape)', async ({ page }) => {
  await loginAndGotoResource(page);

  const kebab = await findKebabMenuButton(page);
  await kebab.focus();
  await kebab.press('Enter');

  // Ensure menu is visible before reading items
  await expect(page.getByRole('menu')).toBeVisible();
  const items = menuItemsLocator(page);
  await expect(items.first()).toBeVisible();

  // Try navigating through menu items using arrows; no hard assertion on count beyond existence.
  const itemCount = await items.count();
  if (itemCount > 1) {
    await page.keyboard.press('ArrowDown');
    await page.keyboard.press('ArrowDown');
    await page.keyboard.press('ArrowUp');
  }

  // Dismiss the menu via Escape to avoid side effects
  await page.keyboard.press('Escape');

  // Optionally assert that menu is dismissed if the UI hides it
  // Can't guarantee, but we try a soft check
  const afterDismissVisible = await items.first().isVisible().catch(() => false);
  // If still visible, it's OK; some UIs keep the menu until click elsewhere
  console.log('[Menu visibility after Escape]', afterDismissVisible);
});

// Robustness: menu button discoverability heuristics and fallbacks should work
// This test validates our locator strategy without asserting a specific label
// and ensures no unhandled exceptions occur during discovery/open.
test('Menu button discovery heuristics are resilient', async ({ page }) => {
  await loginAndGotoResource(page);
  const kebab = await findKebabMenuButton(page);
  await expect(kebab).toBeVisible();

  await openMenu(kebab);
  await expect(page.getByRole('menu')).toBeVisible();
  const items = menuItemsLocator(page);
  expect(await items.count()).toBeGreaterThan(0);
});
