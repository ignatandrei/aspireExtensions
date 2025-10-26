// spec: specs/DepEmp-Resource-Actions-TestPlan.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';

test.describe('DepEmp Resource Actions', () => {
  test('selectEmployeeCount Command Execution', async ({ page }) => {
    // 1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
    await page.goto('http://localhost:15102/consolelogs/resource/DepEmp');
    
    // 2. Click the Resource actions button (three dots icon in the toolbar)
    await page.getByRole('button', { name: 'Resource actions' }).click();
    
    // 3. Select "Reset Everything" from the dropdown menu
    await page.getByRole('menuitem', { name: 'Reset Everything' }).click();
    
    // 4. Wait for the success notification "DepEmp 'Reset Everything' succeeded"
    await expect(page.getByText('DepEmp "Reset Everything" succeeded')).toBeVisible();
    
    // 5. Click the "Remove data" button in the toolbar
    await page.getByRole('button', { name: 'Remove data' }).click();
    
    // 6. Click "Remove all" from the dropdown menu
    await page.getByRole('menuitem', { name: 'Remove all' }).click();
    
    // 7. Click the Resource actions button (three dots icon)
    await page.getByRole('button', { name: 'Resource actions' }).click();
    
    // 8. Click on the "selectEmployeeCount" menu item
    await page.getByRole('menuitem', { name: 'selectEmployeeCount' }).click();
    
    // Verify: Success notification appears displaying: `DepEmp "selectEmployeeCount" succeeded`
    await expect(page.getByText('DepEmp "selectEmployeeCount" succeeded')).toBeVisible();
    
    // Verify: Console logs contain `Executing command 'selectEmployeeCount'.`
    await expect(page.getByText("Executing command 'selectEmployeeCount'.")).toBeVisible();
    
    // Verify: Console logs contain `Executing script 1`
    await expect(page.getByText('Executing script')).toBeVisible();
    
    // Verify: Console logs contain `"Executed batch (final), scalar result: 2"`
    await expect(page.getByText('Executed batch (final), scalar result: 2')).toBeVisible();
    
    // Verify: Console logs contain `Executed 1 scripts on database DepEmp`
    await expect(page.getByText('Executed 1 scripts on database DepEmp')).toBeVisible();
    
    // Verify: Console logs contain `Executed command selectEmployeeCount on database DepEmp`
    await expect(page.getByText('Executed command selectEmployeeCount on database DepEmp')).toBeVisible();
    
    // Verify: Console logs contain `Successfully executed command 'selectEmployeeCount'.`
    await expect(page.getByText("Successfully executed command 'selectEmployeeCount'.")).toBeVisible();
  });
});
