# DepEmp Resource Actions - Comprehensive Test Plan

## Application Overview

The DepEmp Resource Console Logs page is part of the SqlServerExtensions .NET Aspire Dashboard. It provides database management functionality for the DepEmp SQL Server resource. The application features:

- **Console Log Monitoring**: Real-time display of database command execution logs
- **Resource Actions Menu**: Dropdown menu (three dots) providing access to database operations
- **Log Management**: Ability to pause, remove logs for specific resources or all logs
- **Command Execution**: Execute various database commands through the resource actions menu
- **Success Notifications**: Visual feedback via toast notifications when commands complete
- **Live Updates**: Console logs update in real-time showing command execution details

## Available Resource Actions

The Resource Actions menu (accessed via the three dots button) provides the following commands:
1. **deleteEmployee** - Deletes employee records
2. **selectEmployeeCount** - Queries the count of employees
3. **dropCreate** - Drops and recreates the database
4. **Startup_ExecScripts** - Executes startup initialization scripts
5. **Reset Everything** - Performs a complete system reset (drops, recreates, and runs startup scripts)
6. **View details** - (Excluded from testing as per requirements)

## Test Scenarios

### Scenario 1: deleteEmployee Command Execution

**Seed:** `tests/seed.spec.ts`

**Objective:** Verify that the deleteEmployee command executes successfully and logs appropriate messages to the console.

**Preconditions:**
- Application is running at http://localhost:15102/consolelogs/resource/DepEmp
- Database is in a clean state

**Steps:**
1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
2. Click the Resource actions button (three dots icon in the toolbar)
3. Select "Reset Everything" from the dropdown menu
4. Wait for the success notification "DepEmp 'Reset Everything' succeeded"
5. Click the "Remove data" button in the toolbar
6. Click "Remove all" from the dropdown menu
7. Click the Resource actions button (three dots icon)
8. Click on the "deleteEmployee" menu item

**Expected Results:**
- Success notification appears displaying: `DepEmp "deleteEmployee" succeeded`
- Console logs contain the following text in sequence:
  - `Executing command 'deleteEmployee'.`
  - `Executing script 1`
  - `"Executed batch (final), affected rows: 2"`
  - `Executed 1 scripts on database DepEmp`
  - `Executed command deleteEmployee on database DepEmp`
  - `Successfully executed command 'deleteEmployee'.`
- Logs show that 2 rows were affected by the delete operation

---

### Scenario 2: selectEmployeeCount Command Execution

**Seed:** `tests/seed.spec.ts`

**Objective:** Verify that the selectEmployeeCount command executes successfully and returns the employee count.

**Preconditions:**
- Application is running at http://localhost:15102/consolelogs/resource/DepEmp
- Database is in a clean state

**Steps:**
1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
2. Click the Resource actions button (three dots icon in the toolbar)
3. Select "Reset Everything" from the dropdown menu
4. Wait for the success notification "DepEmp 'Reset Everything' succeeded"
5. Click the "Remove data" button in the toolbar
6. Click "Remove all" from the dropdown menu
7. Click the Resource actions button (three dots icon)
8. Click on the "selectEmployeeCount" menu item

**Expected Results:**
- Success notification appears displaying: `DepEmp "selectEmployeeCount" succeeded`
- Console logs contain the following text in sequence:
  - `Executing command 'selectEmployeeCount'.`
  - `Executing script 1`
  - `"Executed batch (final), scalar result: 2"`
  - `Executed 1 scripts on database DepEmp`
  - `Executed command selectEmployeeCount on database DepEmp`
  - `Successfully executed command 'selectEmployeeCount'.`
- Logs show a scalar result of 2 employees

---

### Scenario 3: dropCreate Command Execution

**Seed:** `tests/seed.spec.ts`

**Objective:** Verify that the dropCreate command successfully drops and recreates the database.

**Preconditions:**
- Application is running at http://localhost:15102/consolelogs/resource/DepEmp
- Database exists

**Steps:**
1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
2. Click the Resource actions button (three dots icon in the toolbar)
3. Select "Reset Everything" from the dropdown menu
4. Wait for the success notification "DepEmp 'Reset Everything' succeeded"
5. Click the "Remove data" button in the toolbar
6. Click "Remove all" from the dropdown menu
7. Click the Resource actions button (three dots icon)
8. Click on the "dropCreate" menu item

**Expected Results:**
- Success notification appears displaying: `DepEmp "dropCreate" succeeded`
- Console logs contain the following text in sequence:
  - `Executing command 'dropCreate'.`
  - `Executing script 1`
  - `"Executed batch (final), affected rows: -1"`
  - `Executed 1 scripts on database DepEmp`
  - `Executed command dropCreate on database DepEmp`
  - `Successfully executed command 'dropCreate'.`
- Affected rows value of -1 indicates DDL operation (database structure change)

---

### Scenario 4: Startup_ExecScripts Command Execution

**Seed:** `tests/seed.spec.ts`

**Objective:** Verify that the Startup_ExecScripts command executes initialization scripts successfully.

**Preconditions:**
- Application is running at http://localhost:15102/consolelogs/resource/DepEmp
- Database has been created

**Steps:**
1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
2. Click the Resource actions button (three dots icon in the toolbar)
3. Select "Reset Everything" from the dropdown menu
4. Wait for the success notification "DepEmp 'Reset Everything' succeeded"
5. Click the "Remove data" button in the toolbar
6. Click "Remove all" from the dropdown menu
7. Click the Resource actions button (three dots icon)
8. Click on the "Startup_ExecScripts" menu item

**Expected Results:**
- Success notification appears displaying: `DepEmp "Startup_ExecScripts" succeeded`
- Console logs contain the following text in sequence:
  - `Executing command 'Startup_ExecScripts'.`
  - `Executing script 1`
  - `"Executed batch (GO 1 from 1), affected rows: -1"` (appears multiple times for different batches)
  - `Executed 1 scripts on database DepEmp`
  - `Executed command Startup_ExecScripts on database DepEmp`
  - `Successfully executed command 'Startup_ExecScripts'.`
- Multiple batch executions are logged showing the script runs in several parts

---

### Scenario 5: Reset Everything Command Execution

**Seed:** `tests/seed.spec.ts`

**Objective:** Verify that the Reset Everything command performs a complete system reset including drop, create, and startup scripts.

**Preconditions:**
- Application is running at http://localhost:15102/consolelogs/resource/DepEmp
- Database may be in any state

**Steps:**
1. Navigate to http://localhost:15102/consolelogs/resource/DepEmp
2. Click the "Remove data" button in the toolbar
3. Click "Remove all" from the dropdown menu
4. Click the Resource actions button (three dots icon in the toolbar)
5. Click on the "Reset Everything" menu item

**Expected Results:**
- Success notification appears displaying: `DepEmp "Reset Everything" succeeded`
- Console logs contain the following text in sequence:
  - `Executing command 'reset-all'.`
  - `Starting database system reset...`
  - `Executing command 'dropCreate'.`
  - `Executing script 1`
  - `"Executed batch (final), affected rows: -1"`
  - `Executed 1 scripts on database DepEmp`
  - `Executed command dropCreate on database DepEmp`
  - `Successfully executed command 'dropCreate'.`
  - `Executing command 'Startup_ExecScripts'.`
  - `Executing script 1`
  - Multiple batch execution logs with `"Executed batch (GO 1 from 1), affected rows: -1"` and `"Executed batch (GO 1 from 1), affected rows: 2"`
  - `Executed 1 scripts on database DepEmp`
  - `Executed command Startup_ExecScripts on database DepEmp`
  - `Successfully executed command 'Startup_ExecScripts'.`
  - `System reset completed successfully`
  - `Successfully executed command 'reset-all'.`
- The command executes both dropCreate and Startup_ExecScripts in sequence
- Final employee count after reset is 2 (as shown by affected rows: 2)

---

## Test Execution Guidelines

### General Test Approach

1. **Test Independence**: Each test scenario should be executable independently
2. **State Management**: Always start with "Reset Everything" to ensure consistent starting state
3. **Log Verification**: Use text-based assertions on console log content, not CSS selectors or IDs
4. **Timing**: Allow adequate wait time for async operations to complete
5. **Success Indicators**: Verify both the toast notification and console log entries

### Assertion Strategy

For each test scenario, assertions should verify:
- **Success Notification**: Check that the toast message appears with correct command name
- **Log Sequence**: Verify that log entries appear in the expected order
- **Key Log Messages**: Assert presence of critical log messages (starting execution, completion, etc.)
- **Data Validation**: Check affected row counts or scalar results where applicable

### Test Data Assumptions

- Database contains employee data after Startup_ExecScripts runs
- Initial employee count is 2
- deleteEmployee command affects 2 rows
- selectEmployeeCount returns a scalar value of 2 after reset

### Known Behaviors

- **Affected Rows -1**: Indicates DDL operations (structure changes) rather than DML operations (data changes)
- **Batch Execution**: Some scripts execute in multiple batches (indicated by "GO 1 from 1" messages)
- **Command Composition**: Reset Everything internally calls dropCreate followed by Startup_ExecScripts

---

## Additional Test Considerations

### Edge Cases to Consider

1. **Rapid Command Execution**: Test executing commands in quick succession
2. **Log Overflow**: Verify behavior when console has many log entries
3. **Network Interruption**: Test resilience when connection to backend is lost
4. **Concurrent Operations**: Multiple users executing commands simultaneously
5. **Failed Commands**: Verify error handling when commands fail

### Accessibility Considerations

- Resource actions menu is keyboard accessible
- Success notifications should be announced to screen readers
- Console logs should be navigable via keyboard

### Performance Criteria

- Commands should complete within reasonable time (< 5 seconds for simple operations)
- UI should remain responsive during command execution
- Log updates should appear in real-time without significant delay

---

## Test Automation Notes

### Locator Strategy

Per requirements, tests should use **text-based locators** rather than CSS selectors or IDs:
- Use `getByRole('button', { name: 'Resource actions' })` for buttons
- Use `getByRole('menuitem', { name: 'deleteEmployee' })` for menu items
- Use `getByText()` for verifying log content

### Sample Test Structure

```typescript
test('deleteEmployee command execution', async ({ page }) => {
  // Navigate
  await page.goto('http://localhost:15102/consolelogs/resource/DepEmp');
  
  // Reset state
  await page.getByRole('button', { name: 'Resource actions' }).click();
  await page.getByRole('menuitem', { name: 'Reset Everything' }).click();
  await expect(page.getByText('DepEmp "Reset Everything" succeeded')).toBeVisible();
  
  // Clear logs
  await page.getByRole('button', { name: 'Remove data' }).click();
  await page.getByRole('menuitem', { name: 'Remove all' }).click();
  
  // Execute command
  await page.getByRole('button', { name: 'Resource actions' }).click();
  await page.getByRole('menuitem', { name: 'deleteEmployee' }).click();
  
  // Verify success notification
  await expect(page.getByText('DepEmp "deleteEmployee" succeeded')).toBeVisible();
  
  // Verify console logs
  await expect(page.getByText("Executing command 'deleteEmployee'.")).toBeVisible();
  await expect(page.getByText('Executing script 1')).toBeVisible();
  await expect(page.getByText('"Executed batch (final), affected rows: 2"')).toBeVisible();
  await expect(page.getByText('Executed command deleteEmployee on database DepEmp')).toBeVisible();
  await expect(page.getByText("Successfully executed command 'deleteEmployee'.")).toBeVisible();
});
```

---

## Version Information

- **Application**: SqlServerExtensions .NET Aspire Dashboard
- **Resource**: DepEmp (SQL Server Database)
- **URL**: http://localhost:15102/consolelogs/resource/DepEmp
- **Test Plan Created**: October 18, 2025
- **Test Plan Version**: 1.0
