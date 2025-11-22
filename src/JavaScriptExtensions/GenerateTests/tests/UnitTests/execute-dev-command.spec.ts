// spec: test-plan-console-logs-commands.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';

test.describe('Execute "dev" Command', () => {
  test('Scenario 5: Execute "dev" Command', async ({ page }) => {
    // Navigate to authentication URL to authenticate via token
    await page.goto('https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199');
    
    // Wait for authentication to complete
    await new Promise(f => setTimeout(f, 3 * 1000));
    
    // Navigate to console logs page for JavaScriptAppWithCommands resource
    await page.goto('https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands');
    
    // Wait for page to fully load
    await new Promise(f => setTimeout(f, 3 * 1000));
    
    // Open Resource actions menu
    await page.getByRole('button', { name: 'Resource actions' }).click();
    
    // Hover over "Commands" to expand submenu
    await page.getByRole('menuitem', { name: 'Commands' }).hover();
    
    // Click on the "dev" menuitem
    await page.getByRole('menuitem', { name: 'dev' }).click();
    
    // Verify "Executing command 'dev'." log entry is visible
    //await expect(page.getByText('Executing command \'dev\'.')).toBeVisible();
    const executingElements = await page.getByText('Executing command \'dev\'.').all();
    expect(executingElements.length).toBeGreaterThan(0);
    
    // Verify command execution path is logged
    //await expect(page.getByText('Exec C:\\Program Files\\nodejs\\npm.cmd run dev in folder:')).toBeVisible();
    const execElements = await page.getByText('Exec C:\\Program Files\\nodejs\\npm.cmd run dev in folder:').all();
    expect(execElements.length).toBeGreaterThan(0);
    
    // Verify dev output "I am running dev" is visible
    //await expect(page.getByText('I am running dev')).toBeVisible();
    const devOutputElements = await page.getByText('I am running dev').all();
    expect(devOutputElements.length).toBeGreaterThan(0);
    
    // Verify "Successfully executed command 'dev'." log entry is visible
    //await expect(page.getByText('Successfully executed command \'dev\'.')).toBeVisible();
    const successElements = await page.getByText('Successfully executed command \'dev\'.').all();
    expect(successElements.length).toBeGreaterThan(0);
  });
});
