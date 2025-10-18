import { Page, TestInfo, expect } from '@playwright/test';

export async function sleep(ms: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function endTest(page: Page, testInfo: TestInfo): Promise<void> {
          // Take a screenshot after each test
          //await page.screenshot({ path: `screenshot-${Date.now()}.png` });
         //await page.context().tracing.stop({ path: `trace-${testInfo.title}.zip` });
          // if(page.video() !== null  )
          //   await page.video()!.saveAs(`video-${Date.now()}.webm`);      
}
export async function setupDepEmpTest(page: Page): Promise<void> {
    await sleep(5*1000);
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
}
