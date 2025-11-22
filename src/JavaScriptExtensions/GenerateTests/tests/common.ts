import { Page,test as baseTest, expect, Locator, TestInfo } from '@playwright/test';

import config from './config.json';
import path from 'path';
import fs from 'fs/promises';


export async function anyVisible(locators: import('@playwright/test').Locator[]) {
  for (const l of locators) {
    try {
      if (await l.first().isVisible()) return true;
    } catch {}
  }
  return false;
}

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
         
    } else {
      context = await browser.newContext();
    }
    context.addInitScript(
          {path:'tests/showMessage.js'   }         
        );

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

export const BASE = process.env.ASPIRE_BASE_URL ??" https://localhost:17261";
export const LOGIN = process.env.ASPIRE_LOGIN_URL??" https://localhost:17261/login?t=2dc508ebb9e7959d93fda0353ad5a987"; 
export const RESOURCE_URL = `${BASE}`;

export async function setupFirstPage(page: Page, test:TestInfo): Promise<void> {

  await sleep(1);
    console.log(`Logging in at ${LOGIN} =>`);
    await page.goto(LOGIN);
    await page.waitForLoadState('networkidle');
  //await sleepMessage(page,"Starting to clean data",3);
// await sleepMessage(page,`
//   <h1>Configuring the software for demo</h1> 
//   <b>${test.title}</b>
//   `,3);    
    
  
}
function shouldRecordVideo(testInfo:TestInfo) {

    
  return (config.recordVideo);  
  //return testInfo.title.length>0;
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
