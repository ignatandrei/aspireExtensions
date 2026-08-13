import { test, expect, Page, Locator } from '@playwright/test';

// This spec explores the three-dots (kebab) menu on the Console Logs resource page.
// It starts by hitting the provided login URL (token-based) and then navigates to the resource page.
// Notes:
// - HTTPS is on localhost; we ignore cert errors per-file to avoid config edits.
// - All tests are independent; each test performs its own login+nav.

const DEFAULT_BASE_URL = process.env.ASPIRE_BASE_URL ??"https://portextension.dev.localhost:17144/";
const DEFAULT_LOGIN_URL = process.env.ASPIRE_LOGIN_URL??"https://portextension.dev.localhost:17144/login?t=390b4fb99f9d4a0efd97fd32a889f5c9"; 
const RESOURCE_URL = `${DEFAULT_BASE_URL}consolelogs/resource/PortResource`;
const Console_URL = `${DEFAULT_BASE_URL}consolelogs/resource/ShowPort`;

// Per-file Playwright settings
// - Ignore self-signed/localhost HTTPS errors.
// - Use headless by default (override via CLI if needed).
test.use({ ignoreHTTPSErrors: true });

async function loginAndGotoResource(page: Page,url: string) {
  await page.goto(DEFAULT_LOGIN_URL, { waitUntil: 'domcontentloaded' });

  await page.goto(RESOURCE_URL, { waitUntil: 'domcontentloaded' });

  await expect(page.getByRole('heading', { name: /Console logs/i })).toBeVisible();
  
  await page.waitForTimeout(200); // brief stabilizing delay to avoid racing hydration
}




test("makeVideo", async ({ page }) => {

      await page.screencast.start({ path: 'video.webm' ,});

      await page.screencast.showActions({ position: 'top-right', duration: 1000, fontSize: 18 })

      await page.screencast.showOverlays();

      await page.screencast.showChapter('A1ddin1g TODOs', {
  description: 'Type and prhess enter for each TODO',
  duration: 5000,
});

//var indic1 = await page.screencast.showOverlay('<div style="color: red">Recording</div>');
    console.log(`going to of ${DEFAULT_LOGIN_URL} page`);
  await page.goto(DEFAULT_LOGIN_URL, { waitUntil: 'domcontentloaded' });
  await page.waitForTimeout(3000);
//  await page.screenshot({ path: 'consolelogs1.png', fullPage: true });

//indic1.dispose();
  await page.screencast.showChapter('Adding3 TODOs', {
  description: 'Type and press enjter for each TODO',
  duration: 7000,
});

//var a1= await page.screencast.showOverlay('<div style="color: red">Recording</div>',{duration: 5000});

    //console.log(`Taking screenshot of ${RESOURCE_URL} page`);
  await page.goto(RESOURCE_URL, { waitUntil: 'domcontentloaded' });
  await page.waitForTimeout(3000);
  //await page.screenshot({ path: 'portresource-page.png', fullPage: true });

  await page.screencast.showChapter('Adding0 TODOs', {
  description: 'Type and press enter for eacfh TODO',
  duration: 8000,
});
//a1.dispose();
//var indic =await page.screencast.showOverlay('<div style="color: red">Recording</div>');
console.log(`Going to full ${Console_URL} page`);
await page.goto(Console_URL, { waitUntil: 'domcontentloaded' });
  //await page.waitForTimeout(3000);
  //await page.screenshot({ path: 'consolelogs-page.png', fullPage: true });

  await page.screencast.showChapter('Aasadding0 TODOs', {
  description: 'Type and press enter for eacfh TODO',
  duration: 8000,
});
//indic.dispose();
await page.screencast.stop();
});
// Happy path: open kebab menu and list items
