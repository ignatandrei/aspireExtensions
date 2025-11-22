// spec: test-plan-console-logs-commands.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';

test.describe('Execute "build" Command', () => {
  test('Scenario 4: Execute "build" Command', async ({ page }) => {
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
    
    // Click on the "build" menuitem
    await page.getByRole('menuitem', { name: 'build' }).click();
    
    // Verify "Executing command 'build'." log entry is visible
    const executingElements = await page.getByText('Executing command \'build\'.').all();
    expect(executingElements.length).toBeGreaterThan(0);
   
    // Verify command execution path is logged
    const execElements = await page.getByText('Exec C:\\Program Files\\nodejs\\npm.cmd run build in folder:').all();
    expect(execElements.length).toBeGreaterThan(0);
    
    //
    // Verify build output "I am building" is visible
    const buildingElements = await page.getByText('I am building').all();
    expect(buildingElements.length).toBeGreaterThan(0);
   
    // Verify "Successfully executed command 'build'." log entry is visible
    const successMessage = await page.getByText('Successfully executed command \'build\'.').all();
    expect(successMessage.length).toBeGreaterThan(0);
  });
});
