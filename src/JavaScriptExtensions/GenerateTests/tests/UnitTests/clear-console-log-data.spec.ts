// spec: test-plan-console-logs-commands.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';

test.describe('Clear Console Log Data', () => {
  test('Clear Console Log Data', async ({ page }) => {
    // Authenticate via token by navigating to login URL
    await page.goto('https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199');

    // Navigate to the console logs page for JavaScriptAppWithCommands after authentication
    await page.goto('https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands');

    // Locate the "Remove data" button in the toolbar (trash icon)
    const removeButton = page.getByRole('button', { name: 'Remove data' });

    // Click on "Remove data" button
    await removeButton.click();

    // Observe the console log area: all log entries are removed
    const logsAfter = await page.getByText(/Successfully executed command/).all();
    await expect(logsAfter.length).toBe(0);
  });
});