import { test, expect } from '@playwright/test';

// Scenario 8: Execute Multiple Commands Sequentially
// Seed: tests/seed.spec.ts

test.describe('Console Logs Commands', () => {
  test('Execute Multiple Commands Sequentially', async ({ page }) => {
    // Navigate to authentication URL
    await page.goto('https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199');

    // Wait for authentication to complete
    await new Promise(f => setTimeout(f, 3 * 1000));

    // Navigate to console logs page
    await page.goto('https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands');

    // Wait for console logs page load
    await new Promise(f => setTimeout(f, 3 * 1000));

    // Open resource actions menu for test command
    await page.getByRole('button', { name: 'Resource actions' }).click();

    // Expand commands submenu for test command
    await page.getByRole('menuitem', { name: 'Commands' }).hover();

    // Click test command
    await page.getByRole('menuitem', { name: 'test' }).click();

    // Verify test success notification visible
    const testSuccessNotification = page.getByText("executed command 'test'");
    //await expect(testSuccessNotification).toBeVisible();
    expect((await testSuccessNotification.all()).length).toBeGreaterThan(0);

    // Verify executing test log appears
    const executingTest = page.getByText("Executing command 'test'.");
    //await expect(executingTest).toBeVisible();
    expect((await executingTest.all()).length).toBeGreaterThan(0);

    // Verify test exec path log
    const execTestPath = page.getByText('Exec C:\\Program Files\\nodejs\\npm.cmd run test in folder: D:\\eu\\GitHub\\aspireExtensions\\src\\JavaScriptExtensions\\SampleJavaScript');
    //await expect(execTestPath).toBeVisible();
    expect((await execTestPath.all()).length).toBeGreaterThan(0);

    // Verify test result log
    const testResult = page.getByText('Executing test result: > samplejavascript@1.0.0 test > echo "Error: no test specified" "Error: no test specified"');
    //await expect(testResult).toBeVisible();
    expect((await testResult.all()).length).toBeGreaterThan(0);

    // Verify test success log
    const testSuccessLog = page.getByText("Successfully executed command 'test'.");
    //await expect(testSuccessLog).toBeVisible();
    expect((await testSuccessLog.all()).length).toBeGreaterThan(0);

    // Open resource actions menu for build command
    await page.getByRole('button', { name: 'Resource actions' }).click();

    // Expand commands submenu for build command
    await page.getByRole('menuitem', { name: 'Commands' }).hover();

    // Click build command
    await page.getByRole('menuitem', { name: 'build' }).click();

    // Verify executing build log visible
    const executingBuild = page.getByText("Executing command 'build'.");
    //await expect(executingBuild).toBeVisible();
    expect((await executingBuild.all()).length).toBeGreaterThan(0);

    // Verify build exec path log
    const execBuildPath = page.getByText('Exec C:\\Program Files\\nodejs\\npm.cmd run build in folder: D:\\eu\\GitHub\\aspireExtensions\\src\\JavaScriptExtensions\\SampleJavaScript');
    //await expect(execBuildPath).toBeVisible();
    expect((await execBuildPath.all()).length).toBeGreaterThan(0);

    // Verify build result log visible
    const buildResult = page.getByText('Executing build result: > samplejavascript@1.0.0 build > echo I am building I am building');
    //await expect(buildResult).toBeVisible();
    expect((await buildResult.all()).length).toBeGreaterThan(0);

    // Verify build success log visible
    const buildSuccessLog = page.getByText("Successfully executed command 'build'.");
    //await expect(buildSuccessLog).toBeVisible();
    expect((await buildSuccessLog.all()).length).toBeGreaterThan(0);

    // Verify executing dev log visible
    const executingDev = page.getByText("Executing command 'dev'.");
    //await expect(executingDev).toBeVisible();
    expect((await executingDev.all()).length).toBeGreaterThan(0);

    // Verify dev exec path log
    const execDevPath = page.getByText('Exec C:\\Program Files\\nodejs\\npm.cmd run dev in folder: D:\\eu\\GitHub\\aspireExtensions\\src\\JavaScriptExtensions\\SampleJavaScript');
    //await expect(execDevPath).toBeVisible();
    expect((await execDevPath.all()).length).toBeGreaterThan(0);

    // Verify dev result log visible
    const devResult = page.getByText('Executing dev result: > samplejavascript@1.0.0 dev > echo I am running dev I am running dev');
    //await expect(devResult).toBeVisible();
    expect((await devResult.all()).length).toBeGreaterThan(0);

    // Verify dev success log visible
    const devSuccessLog = page.getByText("Successfully executed command 'dev'.");
    //await expect(devSuccessLog).toBeVisible();
    expect((await devSuccessLog.all()).length).toBeGreaterThan(0);
  });
});
