// spec: specs/DepEmp-Resource-Actions-TestPlan.md
// seed: tests/seed.spec.ts

import {  expect } from '@playwright/test';
import { endTest, flashAndClick, setupDepEmpTest, sleepMessage, test,sleep } from '../common';

test.describe('DepEmp Resource Actions', () => {

  test.beforeEach(async ({ page,browser },testInfo) => {
      //var cnt = await browser.newContext();
      //var newPage = await cnt.newPage();
       await setupDepEmpTest(page,testInfo);
       //await cnt.close();
     });
     test.afterEach( async ( {page}, testInfo) => {
      await endTest(page  , testInfo);
     });
  test('deleteEmployee Command Execution', async ({ page }) => {
    // 1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
    
    await page.goto('http://localhost:15102/consolelogs/resource/DepEmp');
    
    // 7. Click the Resource actions button (three dots icon)
    await flashAndClick(page.getByRole('button', { name: 'Resource actions' }));
    await sleep(5);
    // 8. Click on the "deleteEmployee" menu item
    await flashAndClick(page.getByRole('menuitem', { name: 'deleteEmployee' }));
    await sleep(5);
    // Verify: Console logs contain `Executing command 'deleteEmployee'.`
    await expect(page.getByText("Executing command 'deleteEmployee'.", { exact: false })).toBeVisible();
    
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
