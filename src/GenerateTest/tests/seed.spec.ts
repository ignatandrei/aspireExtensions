import { test, expect, Page } from '@playwright/test';

test.describe('Test group', () => {
  test('seed', async ({ page }) => {
    // generate code here.
  });
   test.afterEach(async ( {page}) => {
      // Take a screenshot after each test
      await page.screenshot({ path: `screenshot-${Date.now()}.png` });
      if(page.video() !== null  )
        await page.video()!.saveAs(`video-${Date.now()}.webm`);
  });

});
