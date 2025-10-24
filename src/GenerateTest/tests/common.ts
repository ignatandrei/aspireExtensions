import { Page,test as baseTest, expect, Locator, TestInfo } from '@playwright/test';

import config from './config.json';
import path from 'path';
import fs from 'fs/promises';

 
export const test = baseTest.extend<{ page: any }>({
  page: async ({ browser }, use, testInfo) => {
    let context;
    if (shouldRecordVideo(testInfo)) {
      context = await browser.newContext({ recordVideo: { dir: 'videos/' } });
    } else {
      context = await browser.newContext();
    }
    const page = await context.newPage();
    await use(page);
    await context.close();    
    },      
}); 

export async function endTest(page: Page, testInfo: TestInfo): Promise<void> {
          
    if(page.video() !== null  ){
        console.log(`Video saved to: ${await page.video()!.path()}`);
        page.context().close();
        await sleep(2000);
    await fs.rename( await page.video()!.path(),
        path.join('videos', `video-${testInfo.title.replace(/[^a-zA-Z0-9-_]/g,'_')}-${Date.now()}.webm`));
        
    } 
}
export async function setupDepEmpTest(page: Page): Promise<void> {

  await sleep(1000);
    
  // 1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
  await page.goto('http://localhost:15102/consolelogs/resource/DepEmp');
  
  // 2. Click the Resource actions button (three dots icon in the toolbar)
  await flashAndClick(page.getByRole('button', { name: 'Resource actions' }));
  
  // 3. Select "Reset Everything" from the dropdown menu
  await flashAndClick(page.getByRole('menuitem', { name: 'Reset Everything' }) );
  
  // 4. Wait for the success notification "DepEmp 'Reset Everything' succeeded"
  await expect(page.getByText('DepEmp "Reset Everything" succeeded')).toBeVisible();
  
  // 5. Click the "Remove data" button in the toolbar
  await flashAndClick(page.getByRole('button', { name: 'Remove data' }));

  // 6. Click "Remove all" from the dropdown menu
  await flashAndClick(page.getByRole('menuitem', { name: 'Remove all' }));

}

function shouldRecordVideo(testInfo:TestInfo) {

    
  return (config.recordVideo);  
  //return testInfo.title.length>0;
}


function sleep(ms: number) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

export async function flashAndClick(button:Locator ) {
  await button.evaluate((el: HTMLElement) => {
    let i = 0;
    
    const colors = ['yellow', 'red', 'orange', 'white'];
    const interval = setInterval(() => {
      el.style.background = colors[i % colors.length];
      i++;
      if (i > 7) {
        clearInterval(interval);
        el.style.background = '';
      }
    }, 150);
  });
  await sleep(1000);
  await button.click();  
  
}
