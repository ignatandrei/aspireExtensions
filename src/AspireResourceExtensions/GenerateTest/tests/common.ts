import { Page,test as baseTest, expect, Locator, TestInfo } from '@playwright/test';

import path from 'path';
import fs from 'fs/promises';

function formatTimestamp() {
    const now = new Date(Date.now());

    // Extract components of the date and time
    const year = String(now.getFullYear()).padStart(4, '0');
    const month = String(now.getMonth() + 1).padStart(2, '0'); // Months are zero-indexed
    const day = String(now.getDate()).padStart(2, '0');

    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');

    // Combine components into the desired format
    const formattedTimestamp = `${year}${month}${day}${hours}${minutes}${seconds}`;

    return formattedTimestamp;
}


export const test = baseTest.extend<{ page: any }>({
  page: async ({ browser }, use, testInfo) => {
    console.log(`Setting up page for test: ${testInfo.title}`);
    let context;
    if (shouldRecordVideo(testInfo)) {
      context = await browser.newContext({ 
          recordVideo: { dir: 'videos/' } ,
          // viewport:{
          //   width: 3480,
          //   height: 2400
          // },        
        });
         
        context.addInitScript(
          {path:'tests/showMessage.js'   }         
        );
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
        await sleep(10);
        var timeData = formatTimestamp();
        await fs.rename( 
          await page.video()!.path(),
          path.join('videos', `video-${testInfo.title.replace(/[^a-zA-Z0-9-_]/g,'_')}-${timeData}.webm`));
        
    } 
}
export async function setupDepEmpTest(page: Page, test:TestInfo): Promise<void> {

  await sleep(1);
  // 1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
  await page.goto('http://localhost:15102/consolelogs/resource/DepEmp');
  //await sleepMessage(page,"Starting to clean data",3);
await sleepMessage(page,`
  <h1>Configuring the software for demo</h1> 
  <b>${test.title}</b>
  `,3);    
    
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
function shouldRecordVideo(testInfo:TestInfo) {
  return true;  
}


export function sleep(seconds: number) {
  return new Promise(resolve => setTimeout(resolve, seconds*1000));
}

export async function sleepMessage(page:Page,msg: string, seconds: number){

   //const nonce = Buffer.from(Math.random().toString()).toString('base64');
  //page.context.setCSP
   await page.evaluate(({msg,seconds})=>
      
      (window as any).showMessageForVideo(msg,seconds)
   ,({msg,seconds}));
    return await new Promise(resolve => setTimeout(resolve, seconds*1000));

}

export async function flashAndClick(button:Locator ) {
  await button.evaluate((el: HTMLElement) => {
    let i = 0;
    // enlarge and animate
    el.style.transition = 'all 0.15s ease';
    el.style.transform = 'scale(1.25)';
    el.style.padding = '12px 18px';
    el.style.fontSize = '1.05em';
    el.style.borderWidth = '2px';
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
  await sleep(2);
  await button.click();  
  
}
