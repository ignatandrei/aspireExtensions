// spec: tests/resource-details-test-plan.md
// scenario: 1.1 Happy Path: View and Assert URL Table Values
// seed: tests/seed.spec.ts


import { expect } from '@playwright/test';
import { endTest, flashAndClick, test } from './common';

const DEFAULT_BASE_URL = process.env.ASPIRE_BASE_URL ??"https://localhost:17146/";
const DEFAULT_LOGIN_URL = process.env.ASPIRE_LOGIN_URL??"https://localhost:17146/login?t=540eae37810193e8c06aa9a6a69d884d"; 
const RESOURCE_URL = `${DEFAULT_BASE_URL}`;

test.describe('Test group', () => {
  test.use({ ignoreHTTPSErrors: true });

  test.beforeEach(async ({ page }) => {
    await page.goto(DEFAULT_LOGIN_URL);
    await page.waitForLoadState('networkidle');
  
  });
  test.afterEach( async ( {page}, testInfo) => {
      await endTest(page  , testInfo);
     });
  test('seed', async ({ page }) => {
    await page.goto(RESOURCE_URL);
    await page.waitForLoadState('networkidle');
    await expect(page).toHaveTitle(/AspireResourceExtensions/);
  // generate code here.
  });


  test('Happy Path: View and Assert URL Table Values', async ({ page }) => {
    // 1. Locate the row in the resources table where the name is `AspireResource`.
    // 2. In the same row, click the three dots (Actions button).
    const actionsButton = page.getByRole('button', { name: 'Actions' }).nth(1);
    await flashAndClick(actionsButton);

    // 3. In the dropdown menu, select `View details`.
    await flashAndClick(page.getByRole('menuitem', { name: 'View details' }));

    // 4. In the details panel, locate the table under the "URLs" section.
    // 5. For each row in the URLs table, assert the following:
    //    - The "Address" column contains a valid URL (e.g., `https://localhost:17146/`, `https://localhost:17146/login?t=...`).
    //    - The "Text" column contains the expected label (e.g., `BaseUrl`, `LoginUrl`).
    //    - The "Endpoint name" column contains the expected environment variable (e.g., `ASPIRE_BASE_URL`, `ASPIRE_LOGIN_URL`).
    const urlTables = page.locator('table');
    await expect(urlTables).toHaveCount(7);

    const urlTable = urlTables.nth(2); // Assuming the URLs table is the second table on the page
    urlTable.scrollIntoViewIfNeeded();
    await flashAndClick(urlTable);
    const rows = urlTable.locator('tbody tr');
    await expect(rows).toHaveCount(2);

    // First row assertions
    const firstRow = rows.nth(0);
    await expect(firstRow.locator('a', { hasText: `${DEFAULT_BASE_URL}` })).toBeVisible();
    await expect(firstRow).toContainText('BaseUrl');
    await expect(firstRow).toContainText('ASPIRE_BASE_URL');
    await firstRow.highlight();
    // Second row assertions
    const secondRow = rows.nth(1);
    await expect(secondRow.locator('a', { hasText: `${DEFAULT_LOGIN_URL}` })).toBeVisible();
    await expect(secondRow).toContainText('LoginUrl');
    await expect(secondRow).toContainText('ASPIRE_LOGIN_URL');
    await secondRow.highlight();
  });
});
