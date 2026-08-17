import { chromium , test, expect, Page, Locator } from '@playwright/test';


const DEFAULT_BASE_URL = process.env.ASPIRE_BASE_URL ;
const DEFAULT_LOGIN_URL = process.env.ASPIRE_LOGIN_URL; 
const RESOURCE_URL = "http://127.0.0.1:"+ (process.env.PORT_fileDisplay??36308);

test.use({ ignoreHTTPSErrors: true });


test("makeVideo", async ({  page},testInfo) => {

  // const browser = await chromium.launch();
  //  const context = await browser.newContext();
  //   const page = await context.newPage();
  await page.screencast.start({ path: 'video.webm' ,});

       await page.screencast.showActions({ position: 'top-right', duration: 1000, fontSize: 18 })

       await page.screencast.showOverlays();
//        const indicator = await page.screencast.showOverlay(
//   '<h1>● RECORDING</h1>'
// );

      await page.screencast.showChapter('Demo for Aspire FileDisplay', {
  description: 'It will show AppHost.cs and this test test.spec.ts',
  duration: 5000,
});
 //await sleep(2);
    console.log(`going to of ${DEFAULT_LOGIN_URL} page`);
  var resp = await page.goto(DEFAULT_LOGIN_URL, { waitUntil: 'domcontentloaded' });
  await page.waitForTimeout(3000);
  
await flash(page.getByText('FileDisplay' ,{ exact: true }))
await flashAndClick(page.getByText(RESOURCE_URL ,{ exact: true }))
await sleep(3);
    console.log(`Taking screenshot of ${RESOURCE_URL} page`);

    await page.goto(RESOURCE_URL+ `/files/AppHost.cs`, { waitUntil: 'domcontentloaded' });
  await sleep(3);
    await page.goto(RESOURCE_URL+ `/files/test.spec.ts`, { waitUntil: 'domcontentloaded' });
  await sleep(3);
  
  await page.screencast.stop();
  //  await context.close();
  // await browser.close();
});
async function flash(button:Locator ) {
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
  
}
async function flashAndClick(button:Locator ) {
  await flash(button);
  await sleep(2);
  await button.click();  
  
}
async function sleep(seconds: number) {
  return new Promise(resolve => setTimeout(resolve, seconds*1000));
}



