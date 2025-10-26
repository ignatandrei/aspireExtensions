import { TestInfo } from '@playwright/test';
import { endTest, flashAndClick, setupDepEmpTest, sleepMessage, test,sleep } from '../common';

test.describe('Videos', () => {

  test.beforeEach(async ({ page,browser },testInfo) => {
      //var cnt = await browser.newContext();
      //var newPage = await cnt.newPage();
       await setupDepEmpTest(page,testInfo);
       //await cnt.close();
     });
     test.afterEach( async ( {page}, testInfo) => {
      await endTest(page  , testInfo);
     });

   test('Recreate Database With Scripts', async ({ page }, test:TestInfo) => {
    await sleepMessage(page,`<h1>Starting demo for ${test.title}</h1>
      I will show how to use this code: 
      <b>.ExecuteSqlServerScriptsAtStartup(DBFiles.FilesToCreate.ToArray())</b>
      This adds also a command to 

        <b>reset everything</b>, 
        i.e. <b>drop</b> and <b>create</b> the database and <b>execute scripts</b>

      Steps: 
          1. Delete all the employee from table - Execute selectEmployeeCount
          2. Show number of employees - 0
          3. Reset the database 
          4. Show number of employees - 2  
 
       `,7);

    await page.goto('http://localhost:15102/consolelogs/resource/DepEmp');    
    //  Click the Resource actions button (three dots icon)
    var resource  = page.getByRole('button', { name: 'Resource actions' });
    
    await flashAndClick(resource);        
    await flashAndClick(page.getByRole('menuitem', { name: 'deleteEmployee' }));  
    sleep(1);
    await flashAndClick(page.getByText('affected rows: 2'));
    await sleep(3);
    
    await flashAndClick(resource);
    await flashAndClick(page.getByRole('menuitem', { name: 'selectEmployeeCount' }));
    sleep(1);
    await flashAndClick(page.getByText('scalar result: 0'));    
    await sleep(3);
    
    await flashAndClick(resource);
    await flashAndClick(page.getByRole('menuitem', { name: 'Reset Everything' }));
    await sleep(1);
    await flashAndClick(page.getByText("executed command 'reset-all'."));    
    await sleep(3);

    await flashAndClick(resource);
    await flashAndClick(page.getByRole('menuitem', { name: 'selectEmployeeCount' }));        
    await flashAndClick(page.getByText('scalar result: 2'));
    await sleep(3);
    await sleepMessage(page,`<h1>End demo for ${test.title}</h1>
      Nuget :  ...
      `,5);
    
     });

  test('Execute SqlCommand', async ({ page }, test:TestInfo) => {
    
    await sleepMessage(page,`<h1>Starting demo for ${test.title}</h1>
      I will show how to use this code: 

      <b>.WithSqlCommand("selectEmployeeCount", "select count(*) from Employee", ExecCommandEnum.Scalar)</b>
      <b>.WithSqlCommand("deleteEmployee","delete from Employee", ExecCommandEnum.NonQuery)</b>
 
      I will click  <b>selectEmployeeCount</b> Result <i>2</i> ,        
      
      then click <b>deleteEmployee</b> ( <i>(2 rows affected) </i>), 
      
      then click again <b>selectEmployeeCount</b> Result <i>0</i> 
       `,7);
  
    await page.goto('http://localhost:15102/consolelogs/resource/DepEmp');    
    //  Click the Resource actions button (three dots icon)
    var resource  = page.getByRole('button', { name: 'Resource actions' });
    
    await flashAndClick(resource);
    
    await flashAndClick(page.getByRole('menuitem', { name: 'selectEmployeeCount' }));        
    await flashAndClick(page.getByText('scalar result: 2'));
    await sleep(3);
    
    await flashAndClick(resource);
    await flashAndClick(page.getByRole('menuitem', { name: 'deleteEmployee' }));  
    sleep(1);
    await flashAndClick(page.getByText('affected rows: 2'));
    await sleep(3);
    
    await flashAndClick(resource);
    await flashAndClick(page.getByRole('menuitem', { name: 'selectEmployeeCount' }));
    sleep(1);
    await flashAndClick(page.getByText('scalar result: 0'));    
    await sleep(3);
    
    await sleepMessage(page,`<h1>End demo for ${test.title}</h1>

      <b>.WithSqlCommand("selectEmployeeCount", "select count(*) from Employee", ExecCommandEnum.Scalar)</b>
      <b>.WithSqlCommand("deleteEmployee","delete from Employee", ExecCommandEnum.NonQuery)</b>
      Nuget :  ...
      `,5);
  });
});
