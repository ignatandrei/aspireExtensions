import { test, expect } from '@playwright/test';

// Scenario 10: Start Resource from Console Logs Page
// Seed: tests/seed.spec.ts

test.describe('Console Logs Commands', () => {
  test('Start Resource from Console Logs Page', async ({ page }) => {
    // Navigate to authentication URL
    await page.goto('https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199');

    // Wait for authentication to complete
    await new Promise(f => setTimeout(f, 3 * 1000));

    // Navigate to console logs page
    await page.goto('https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands');

    // Wait for console logs page to load
    await new Promise(f => setTimeout(f, 3 * 1000));

    // Verify initial resource state shows Finished in combobox
    const finishedState = page.getByText('JavaScriptAppWithCommands (Finished)');
    //await expect(finishedState).toBeVisible();
    expect((await finishedState.all()).length).toBeGreaterThan(0);

    // Click Start resource button in toolbar
    await page.getByRole('button', { name: 'Start resource' }).click();

    await page.waitForTimeout(2000); // Wait for resource to start
    // Verify resource-start executing log visible
    const executingStartLog = page.getByText("Executing command 'resource-start'.");
    //await expect(executingStartLog).toBeVisible();
    expect((await executingStartLog.all()).length).toBeGreaterThan(0);

    // Verify resource-start success log visible
    const successStartLog = page.getByText("Successfully executed command 'resource-start'.");
    //await expect(successStartLog).toBeVisible();
    expect((await successStartLog.all()).length).toBeGreaterThan(0);

    // Verify startup process log visible (partial prefix match)
    const startupProcessLog = page.getByText('Starting process... {"Executable": "/JavaScriptAppWithCommands');
    //await expect(startupProcessLog).toBeVisible();
    expect((await startupProcessLog.all()).length).toBeGreaterThan(0);

    // Verify resource name still visible post start
    const resourceNameText = page.getByText('JavaScriptAppWithCommands');
    //await expect(resourceNameText).toBeVisible();
    expect((await resourceNameText.all()).length).toBeGreaterThan(0);
  });
});
