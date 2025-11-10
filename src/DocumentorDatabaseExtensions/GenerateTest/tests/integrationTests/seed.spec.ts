import { test, expect } from '@playwright/test';

const BASE = process.env.ASPIRE_BASE_URL ??" https://localhost:17261";
const LOGIN = process.env.ASPIRE_LOGIN_URL??" https://localhost:17261/login?t=2dc508ebb9e7959d93fda0353ad5a987"; 
const RESOURCE_URL = `${BASE}`;

test.describe('Test group', () => {
  test.use({ ignoreHTTPSErrors: true });
  test.beforeEach(async ({page}) => {
    console.log(`Logging in at ${LOGIN} =>`);
    await page.goto(LOGIN);
    await page.waitForLoadState('networkidle');
    // setup code here.
  });
  test('seed', async ({ page }) => {
  });
});
