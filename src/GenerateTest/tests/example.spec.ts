import { test, expect } from '@playwright/test';

test('has title', async ({ page }) => {
  await page.goto('http://localhost:15102/');

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle(/SqlServerExtensions resources/);
});

test.afterEach(async ( {page}) => {
      // Take a screenshot after each test
      await page.screenshot({ path: `screenshot-${Date.now()}.png` });
      if(page.video() !== null  )
        await page.video()!.saveAs(`video-${Date.now()}.webm`);
  });
