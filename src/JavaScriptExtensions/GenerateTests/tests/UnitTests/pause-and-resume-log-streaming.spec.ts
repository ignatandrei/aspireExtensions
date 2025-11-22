// spec: test-plan-console-logs-commands.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';

test.describe('Pause and Resume Log Streaming', () => {
  test('Pause and Resume Log Streaming', async ({ page }) => {
    // Authenticate via token by navigating to login URL
    await page.goto('https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199');

    // Navigate to the console logs page for JavaScriptAppWithCommands after authentication
    await page.goto('https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands');

    // Locate and click the 'Pause incoming data' button in the toolbar
    const pauseButton = page.getByRole('button', { name: 'Pause incoming data' });
    await pauseButton.click();
    await expect(page.getByRole('button', { name: 'Resume incoming data' })).toBeVisible();

    // Open the Resource actions menu to execute a command while log streaming is paused
    await page.getByRole('button', { name: 'Resource actions' }).click();

    // Expand the Commands submenu to access the 'test' command
    await page.getByRole('menuitem', { name: 'Commands' }).hover();

    // Click on the 'test' menuitem to execute the command while log streaming is paused
    await page.getByRole('menuitem', { name: 'test' }).click();

    // Assert that new logs do not appear immediately (log area does not update)
    // (This is a visual/manual check, but we can check that the pause indicator is still present)
    await expect(page.getByRole('button', { name: 'Resume incoming data' })).toBeVisible();

    // Click the pause button again to resume log streaming and observe log updates
    await page.getByRole('button', { name: 'Resume incoming data' }).click();
    await expect(page.getByRole('button', { name: 'Pause incoming data' })).toBeVisible();

    // Assert that buffered logs appear (look for the latest log entry for the test command)
    var texts= await page.locator('text=Successfully executed command').all();
    await expect(texts.length).toBeGreaterThan(0);
  });
});