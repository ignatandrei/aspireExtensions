// spec: test-plan-console-logs-commands.md
// seed: tests/seed.spec.ts

//import { expect } from '@playwright/test';
import { test,LOGIN, BASE, sleep, flashAndClick, sleepMessage, setupFirstPage, endTest } from '../common';

test.describe('DemoTests', () => {

  test.use({ ignoreHTTPSErrors: true });
  test.beforeEach(async ({ page,browser },testInfo) => {
      //var cnt = await browser.newContext();
      //var newPage = await cnt.newPage();
      await setupFirstPage(page,testInfo);
       //await cnt.close();
     });
    //  test.afterEach( async ( {page}, testInfo) => {
      
    //   await endTest(page  , testInfo);
    //  });
   
  test('Demo Execute And See Package Commands', async ({ page },test) => {
      await sleepMessage(page,`<h1>Starting demo commands from package.json </h1>
      I will show how to execute the scripts from package.json as Aspire Commands : 
      <h1>builder.AddJavaScriptApp(...).AddNpmCommandsFromPackage()</h1>    
      <pre>
        "scripts": {
    "test": "echo \"Error: no test specified\" ",
    "dev":"echo I am running dev",
    "build":"echo I am building",
  </pre>
    <h1>"start":"echo I am starting"</h1>
<pre>
  },
</pre>
 <h1>See how "start"  command will execute </h1>
       `,7);

        await page.goto(BASE+"consolelogs/resource/JavaScriptAppWithCommands");
        await sleep(2);
          // Locate the "Remove data" button in the toolbar (trash icon)
        const removeButton = page.getByRole('button', { name: 'Remove data' });

        // Click on "Remove data" button
        await removeButton.click();
        await sleep(2);
        const removeAll = page.getByRole('menuitem', { name: 'Remove all' });
        await removeAll.click();
        await sleep(2);
        await await flashAndClick(page.getByRole('button', { name: 'Resource actions' }));
        await sleep(2);
    
        // Hover over "Commands" to expand submenu
        await page.getByRole('menuitem', { name: 'Commands' }).hover();
        await sleep(2);
        
        // Click on the "build" menuitem
        await flashAndClick(page.getByRole('menuitem', { name: 'start',exact: true }));
        await sleep(10);
        await flashAndClick(page.getByText("I am starting").first());
        await sleep(2);

  });
});
