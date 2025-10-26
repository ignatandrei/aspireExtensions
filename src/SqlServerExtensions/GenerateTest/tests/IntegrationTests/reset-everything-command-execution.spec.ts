// spec: specs/DepEmp-Resource-Actions-TestPlan.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';

test.describe('DepEmp Resource Actions', () => {
  test('Reset Everything Command Execution', async ({ page }) => {
    // 1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
    await page.goto('http://localhost:15102/consolelogs/resource/DepEmp');
    
    // 2. Click the "Remove data" button in the toolbar
    await page.getByRole('button', { name: 'Remove data' }).click();
    
    // 3. Click "Remove all" from the dropdown menu
    await page.getByRole('menuitem', { name: 'Remove all' }).click();
    
    // 4. Click the Resource actions button (three dots icon in the toolbar)
    await page.getByRole('button', { name: 'Resource actions' }).click();
    
    // 5. Click on the "Reset Everything" menu item
    await page.getByRole('menuitem', { name: 'Reset Everything' }).click();
    
    // Verify: Success notification appears displaying: `DepEmp "Reset Everything" succeeded`
    await expect(page.getByText('DepEmp "Reset Everything" succeeded')).toBeVisible();
    
    // Verify: Console logs contain `Executing command 'reset-all'.`
    await expect(page.getByText("Executing command 'reset-all'.")).toBeVisible();
    
    // Verify: Console logs contain `Starting database system reset...`
    await expect(page.getByText('Starting database system reset...')).toBeVisible();
    
    // Verify: Console logs contain `Executing command 'dropCreate'.`
    await expect(page.getByText("Executing command 'dropCreate'.")).toBeVisible();
    
    // Verify: Console logs contain `Executing script 1`
    var allMatchingScripts = await page.getByText(/Executing script \d+/).all();
    await expect(allMatchingScripts.length).toBe(2);

    // Verify: Console logs contain `"Executed batch (final), affected rows: -1"`
    await expect(page.getByText('Executed batch (final), affected rows: -1')).toBeVisible();
    
    // Verify: Console logs contain `Executed 1 scripts on database DepEmp`
    var allExecutedScripts = await page.getByText('Executed 1 scripts on database DepEmp').all();
    await expect(allExecutedScripts.length).toBe(2);
    
    // Verify: Console logs contain `Executed command dropCreate on database DepEmp`
    await expect(page.getByText('Executed command dropCreate on database DepEmp')).toBeVisible();
    
    // Verify: Console logs contain `Successfully executed command 'dropCreate'.`
    await expect(page.getByText("Successfully executed command 'dropCreate'.")).toBeVisible();
    
    // Verify: Console logs contain `Executing command 'Startup_ExecScripts'.`
    await expect(page.getByText("Executing command 'Startup_ExecScripts'.")).toBeVisible();
    
    // Verify: Console logs contain multiple batch execution logs
    var allBatchLogs1 = await page.getByText('Executed batch (GO 1 from 1), affected rows: -1').all();
    await expect(allBatchLogs1.length).toBe(2);
    var allBatchLogs2 = await page.getByText('Executed batch (GO 1 from 1), affected rows: 2').all();
    await expect(allBatchLogs2.length).toBe(2);

    // Additionally check at least one occurrence of each batch log
    await expect(allBatchLogs1[0]).toBeVisible();
    await expect(allBatchLogs2[0]).toBeVisible();
    
    // Verify: Console logs contain `Executed command Startup_ExecScripts on database DepEmp`
    await expect(page.getByText('Executed command Startup_ExecScripts on database DepEmp')).toBeVisible();
    
    // Verify: Console logs contain `Successfully executed command 'Startup_ExecScripts'.`
    await expect(page.getByText("Successfully executed command 'Startup_ExecScripts'.")).toBeVisible();
    
    // Verify: Console logs contain `System reset completed successfully`
    await expect(page.getByText('System reset completed successfully')).toBeVisible();
    
    // Verify: Console logs contain `Successfully executed command 'reset-all'.`
    await expect(page.getByText("Successfully executed command 'reset-all'.")).toBeVisible();
  });
});
