import { test, expect } from '@playwright/test';

test('has title', async ({ page }) => {
  await page.goto('https://localhost:17197/login?t=2dbbbbebe0c5aa573090d43b92a59566');

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle(/SqlServerExtensions resources/);
});

