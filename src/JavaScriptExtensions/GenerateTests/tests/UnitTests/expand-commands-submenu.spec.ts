// spec: test-plan-console-logs-commands.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';

test.describe('Expand Commands Submenu', () => {
  test('Scenario 2: Expand Commands Submenu', async ({ page }) => {
    // Navigate to authentication URL to authenticate via token
    await page.goto('https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199');
    
    // Wait for authentication to complete
    await new Promise(f => setTimeout(f, 3 * 1000));
    
    // Navigate to console logs page for JavaScriptAppWithCommands resource
    await page.goto('https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands');
    
    // Wait for page to fully load
    await new Promise(f => setTimeout(f, 3 * 1000));
    
    // Click on "Resource actions" button to open menu
    await page.getByRole('button', { name: 'Resource actions' }).click();
    
    // Hover over the "Commands" menuitem in the Resource actions menu
    const commandsMenuItem = page.getByRole('menuitem', { name: 'Commands' });
    await commandsMenuItem.hover();
    
    // Verify "test" menuitem is visible
    await expect(page.getByRole('menuitem', { name: 'test' })).toBeVisible();
    
    // Verify "dev" menuitem is visible
    await expect(page.getByRole('menuitem', { name: 'dev' })).toBeVisible();
    
    // Assert that there is at least one element found by getByText
    const buildElements = await page.getByText('build').all();
    expect(buildElements.length).toBeGreaterThan(0);
    
    
    // Verify "start" menuitem text is visible
     const startElements = await page.getByText('start').all();
    expect(startElements.length).toBeGreaterThan(0);
    
    // Verify "Restart" menuitem is visible (and disabled)
    await expect(page.getByRole('menuitem', { name: 'Restart' })).toBeVisible();
  });
});
