// spec: test-plan-console-logs-commands.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';

test.describe('Access Resource Actions Menu', () => {
  test('Scenario 1: Access Resource Actions Menu', async ({ page }) => {
    // Navigate to authentication URL to authenticate via token
    await page.goto('https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199');
    
    // Wait for authentication to complete
    await new Promise(f => setTimeout(f, 3 * 1000));
    
    // Navigate to console logs page for JavaScriptAppWithCommands resource
    await page.goto('https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands');
    
    // Wait for page to fully load
    await new Promise(f => setTimeout(f, 3 * 1000));
    
    // Verify "Resource actions" button is visible
    await expect(page.getByRole('button', { name: 'Resource actions' })).toBeVisible();
    
    // Click on the "Resource actions" button
    await page.getByRole('button', { name: 'Resource actions' }).click();
    
    // Verify "View details" menuitem is visible
    await expect(page.getByRole('menuitem', { name: 'View details' })).toBeVisible();
    
    // Verify "Start" menuitem is visible
    await expect(page.getByRole('menuitem', { name: 'Start' })).toBeVisible();
    
    // Verify "Commands" menuitem is visible with submenu indicator
    await expect(page.getByRole('menuitem', { name: 'Commands' })).toBeVisible();
  });
});
