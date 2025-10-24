// spec: specs/DepEmp-Resource-Actions-TestPlan.md
// seed: tests/seed.spec.ts

import {  expect } from '@playwright/test';
import { test,endTest, setupDepEmpTest, flashAndClick } from './common';

test.describe('DepEmp Resource Actions', () => {
 test.beforeEach(async ({ page }) => {
     await setupDepEmpTest(page);
   });
   test.afterEach( async ( {page}, testInfo) => {
    await endTest(page  , testInfo);
   });
   

  test('dropCreate Command Execution', async ({ page }) => {
    
    await page.goto('http://localhost:15102/consolelogs/resource/DepEmp');
      
    // 7. Click the Resource actions button (three dots icon)
    await flashAndClick(page.getByRole('button', { name: 'Resource actions' }));

    // 8. Click on the "dropCreate" menu item
    await flashAndClick(page.getByRole('menuitem', { name: 'dropCreate' }));
    
    // Verify: Success notification appears displaying: `DepEmp "dropCreate" succeeded`
    await expect(page.getByText('DepEmp "dropCreate" succeeded')).toBeVisible();
    
    // Verify: Console logs contain `Executing command 'dropCreate'.`
    await expect(page.getByText("Executing command 'dropCreate'.")).toBeVisible();
    
    // Verify: Console logs contain `Executing script 1`
    await expect(page.getByText('Executing script')).toBeVisible();
    
    // Verify: Console logs contain `"Executed batch (final), affected rows: -1"`
    await expect(page.getByText('Executed batch (final), affected rows: -1')).toBeVisible();
    
    // Verify: Console logs contain `Executed 1 scripts on database DepEmp`
    await expect(page.getByText('Executed 1 scripts on database DepEmp')).toBeVisible();
    
    // Verify: Console logs contain `Executed command dropCreate on database DepEmp`
    await expect(page.getByText('Executed command dropCreate on database DepEmp')).toBeVisible();
    
    // Verify: Console logs contain `Successfully executed command 'dropCreate'.`
    await expect(page.getByText("Successfully executed command 'dropCreate'.")).toBeVisible();
  });
});
