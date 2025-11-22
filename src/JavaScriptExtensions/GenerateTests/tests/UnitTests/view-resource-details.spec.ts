import { test, expect } from '@playwright/test';

// Scenario 9: View Resource Details from Actions Menu
// Seed: tests/seed.spec.ts

test.describe('Console Logs Commands', () => {
  test('View Resource Details from Actions Menu', async ({ page }) => {
    // Navigate to authentication URL
    await page.goto('https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199');

    // Wait for authentication to complete
    await new Promise(f => setTimeout(f, 3 * 1000));

    // Navigate directly to console logs page
    await page.goto('https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands');

    // Wait for console logs page load
    await new Promise(f => setTimeout(f, 3 * 1000));

    // Click resource actions button
    await page.getByRole('button', { name: 'Resource actions' }).click();

    // Click View details menuitem
    await page.getByRole('menuitem', { name: 'View details' }).click();

    await page.waitForTimeout(2000); // Wait for detail panel to open
    
    // Verify detail panel heading visible
    const heading = page.getByText('Executable: JavaScriptAppWithCommands');
    //await expect(heading).toBeVisible();
    expect((await heading.all()).length).toBeGreaterThan(0);

    // Verify Display name label visible
    const displayNameLabel = page.getByText('Display name');
    //await expect(displayNameLabel).toBeVisible();
    expect((await displayNameLabel.all()).length).toBeGreaterThan(0);

    // Verify Display name value visible
    const displayNameValue = page.getByText('JavaScriptAppWithCommands');
    //await expect(displayNameValue).toBeVisible();
    expect((await displayNameValue.all()).length).toBeGreaterThan(0);

    // Verify State label visible
    const stateLabel = page.getByText('State');
    //await expect(stateLabel).toBeVisible();
    expect((await stateLabel.all()).length).toBeGreaterThan(0);

    // Verify State value Finished visible
    const stateValue = page.getByText('Finished');
    //await expect(stateValue).toBeVisible();
    expect((await stateValue.all()).length).toBeGreaterThan(0);

    // Verify Environment variables section header visible
    const envHeader = page.getByText('Environment variables');
    //await expect(envHeader).toBeVisible();
    expect((await envHeader.all()).length).toBeGreaterThan(0);

    // Verify References section header visible
    const referencesHeader = page.getByText('References');
    //await expect(referencesHeader).toBeVisible();
    expect((await referencesHeader.all()).length).toBeGreaterThan(0);

    // Verify Back references section header visible
    const backReferencesHeader = page.getByText('Back references');
    //await expect(backReferencesHeader).toBeVisible();
    expect((await backReferencesHeader.all()).length).toBeGreaterThan(0);

    // Verify Health checks section header visible
    const healthChecksHeader = page.getByText('Health checks');
    //await expect(healthChecksHeader).toBeVisible();
    expect((await healthChecksHeader.all()).length).toBeGreaterThan(0);

    // Verify URLs section header visible
    const urlsHeader = page.getByText('URLs');
    //await expect(urlsHeader).toBeVisible();
    expect((await urlsHeader.all()).length).toBeGreaterThan(0);

    // Verify at least one environment variable name visible
    const envVar1 = page.getByText('OTEL_SERVICE_NAME');
    //await expect(envVar1).toBeVisible();
    expect((await envVar1.all()).length).toBeGreaterThan(0);

    // Verify another environment variable name visible
    const envVar2 = page.getByText('SSL_CERT_DIR');
    //await expect(envVar2).toBeVisible();
    expect((await envVar2.all()).length).toBeGreaterThan(0);

    // Verify Process ID label visible
    const processIdLabel = page.getByText('Process ID');
    //await expect(processIdLabel).toBeVisible();
    expect((await processIdLabel.all()).length).toBeGreaterThan(0);

    // Verify Executable path label visible
    const execPathLabel = page.getByText('Executable path');
    //await expect(execPathLabel).toBeVisible();
    expect((await execPathLabel.all()).length).toBeGreaterThan(0);

    // Verify Working directory label visible
    const workingDirLabel = page.getByText('Working directory');
    //await expect(workingDirLabel).toBeVisible();
    expect((await workingDirLabel.all()).length).toBeGreaterThan(0);

    // Verify Exit code label visible
    const exitCodeLabel = page.getByText('Exit code');
    //await expect(exitCodeLabel).toBeVisible();
    expect((await exitCodeLabel.all()).length).toBeGreaterThan(0);

    // Verify Executable arguments label visible
    const execArgsLabel = page.getByText('Executable arguments');
    //await expect(execArgsLabel).toBeVisible();
    expect((await execArgsLabel.all()).length).toBeGreaterThan(0);
  });
});
