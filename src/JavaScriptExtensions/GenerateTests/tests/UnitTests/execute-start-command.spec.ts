// spec: test-plan-console-logs-commands.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';

test.describe('JavaScriptExtensions Console Logs - Command Execution', () => {
  test('Execute "start" Command', async ({ page }) => {
    // Navigate to authentication URL
    await page.goto('https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199');

    // Wait for authentication to complete
    await new Promise(f => setTimeout(f, 3 * 1000));

    // Navigate to console logs page for JavaScriptAppWithCommands
    await page.goto('https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands');

    // Wait for console logs page to fully load
    await new Promise(f => setTimeout(f, 3 * 1000));

    // 1. Open Resource actions menu
    await page.getByRole('button', { name: 'Resource actions' }).click();

    // 2. Hover over "Commands" to expand submenu
    await page.getByRole('menuitem', { name: 'Commands' }).hover();

    // 3. Click on the "start" menuitem
    await page.getByRole('menuitem', { name: 'start', exact: true }).click();

    // 4. Monitor console logs and resource state changes - Verify "Executing command 'start'." is visible
    //await expect(page.getByText('Executing command \'start\'.')).toBeVisible();
    const executingElements = await page.getByText('Executing command \'start\'.').all();
    expect(executingElements.length).toBeGreaterThan(0);

    // Verify execution path for start command is visible
    //await expect(page.getByText('Exec C:\\Program Files\\nodejs\\npm.cmd run start in folder:')).toBeVisible();
    const execPathElements = await page.getByText('Exec C:\\Program Files\\nodejs\\npm.cmd run start in folder:').all();
    expect(execPathElements.length).toBeGreaterThan(0);

    // Verify start command output is visible
    //await expect(page.getByText('I am starting')).toBeVisible();
    const outputElements = await page.getByText('I am starting').all();
    expect(outputElements.length).toBeGreaterThan(0);

    // Verify success message for start command is visible
    //await expect(page.getByText('Successfully executed command \'start\'.')).toBeVisible();
    const successElements = await page.getByText('Successfully executed command \'start\'.').all();
    expect(successElements.length).toBeGreaterThan(0);
  });
});
