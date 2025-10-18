// spec: specs/DepEmp-Resource-Actions-TestPlan.md
// seed: tests/seed.spec.ts

import { test, expect } from '@playwright/test';
async function sleep(ms: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, ms));
}
test.describe('DepEmp Resource Actions', () => {
  test('deleteEmployee Command Execution', async ({ page }) => {
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
    
    // 8. Click on the "deleteEmployee" menu item
    await page.getByRole('menuitem', { name: 'deleteEmployee' }).click();
    
    // Verify: Console logs contain `Executing command 'deleteEmployee'.`
    await expect(page.getByText("Executing command 'deleteEmployee'.")).toBeVisible();
    
    // Verify: Console logs contain `Executing script 1`
    await expect(page.getByText('Executing script')).toBeVisible();
    
    // Verify: Console logs contain `"Executed batch (final), affected rows: 2"`
    await expect(page.getByText('Executed batch (final), affected rows: 2')).toBeVisible();
    
    // Verify: Console logs contain `Executed 1 scripts on database DepEmp`
    await expect(page.getByText('Executed 1 scripts on database DepEmp')).toBeVisible();
    
    // Verify: Console logs contain `Executed command deleteEmployee on database DepEmp`
    await expect(page.getByText('Executed command deleteEmployee on database DepEmp')).toBeVisible();
    
    // Verify: Console logs contain `Successfully executed command 'deleteEmployee'.`
    await expect(page.getByText("Successfully executed command 'deleteEmployee'.")).toBeVisible();
  });
});
