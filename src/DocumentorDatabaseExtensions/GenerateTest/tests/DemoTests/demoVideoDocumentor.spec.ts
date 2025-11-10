import { TestInfo } from '@playwright/test';
import {BASE ,endTest, flashAndClick,  sleepMessage, test,sleep, setupFirstPage } from '../common';
test.describe('Videos', () => {

  test.beforeEach(async ({ page,browser },testInfo) => {
      //var cnt = await browser.newContext();
      //var newPage = await cnt.newPage();
      await setupFirstPage(page,testInfo);
       //await cnt.close();
     });
     test.afterEach( async ( {page}, testInfo) => {
      
      await endTest(page  , testInfo);
     });

   test('GenerateDocumentation', async ({ page }, test:TestInfo) => {
    
    await sleepMessage(page,`<h1>Starting demo for ${test.title}</h1>
      I will show how to generate SQL documentation from database : 
      <b>AddDocumentationOnFolder(@"D:\documentation");</b>
 
       `,7);

    await page.goto(`${BASE}`);    

    const resourceName = 'docuDBpubs';
    const row = page.getByRole('row', { name: new RegExp(resourceName, 'i') }).first();
    
    await flashAndClick(row);

    await sleep(3);
    await page.goto(`${BASE}consolelogs/resource/docuDBpubs`);
    var log=  page.getByText('DONE documentation').first();
    await flashAndClick(log);

    await sleepMessage(page,"Now opening the generated documentation diagram",3);
    await page.goto(`http://localhost:5161/docudb/docs/databases/PUBS/`);
    await sleep(5);
    await sleepMessage(page,`<h1>End demo for ${test.title}</h1>
      Nuget : https://www.nuget.org/packages/DocumentorDatabaseExtensionsAspire
      `,5);
    
     });

  
});
