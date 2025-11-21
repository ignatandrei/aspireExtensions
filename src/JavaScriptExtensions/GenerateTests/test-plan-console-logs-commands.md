# JavaScriptExtensions Console Logs - Command Execution Test Plan

## Application Overview

The JavaScriptExtensions Console Logs page is a resource management and monitoring interface that provides real-time console log viewing and command execution capabilities for JavaScript-based executable resources. The application is part of an Aspire-based development environment that manages multiple resources including JavaScript applications, projects, and services.

### Key Features:
- **Console Log Monitoring**: Real-time display of console output from executable resources
- **Command Execution**: Execute predefined npm scripts (test, dev, build, start) through a UI menu
- **Resource Management**: Start/stop resources, pause log streaming, and clear log data
- **Resource Details**: View comprehensive resource information including environment variables, references, and health checks
- **Navigation**: Quick access to different monitoring views (Console, Structured logs, Traces, Metrics)

### Technical Context:
- URL Pattern: `https://javascriptextensions.dev.localhost:17253/consolelogs/resource/{ResourceName}`
- Test Resource: JavaScriptAppWithCommands
- Available Commands: test, dev, build, start, Restart (disabled when resource is not running)
- Authentication: Token-based authentication via URL parameter

---

## Test Scenarios

### Seed Configuration
**Seed File:** `tests/seed.spec.ts`

**Initial Setup:**
1. Navigate to authentication URL: `https://javascriptextensions.dev.localhost:17253/login?t=99c3f542e0e4e1916be5148b2d2e1199`
2. Wait for authentication to complete
3. Navigate to console logs page: `https://javascriptextensions.dev.localhost:17253/consolelogs/resource/JavaScriptAppWithCommands`
4. Wait for page to fully load

---

## Scenario 1: Access Resource Actions Menu

**Objective:** Verify that users can successfully access the Resource Actions menu containing available commands

**Preconditions:**
- User is authenticated
- User is on the console logs page for JavaScriptAppWithCommands resource

**Steps:**
1. Locate the "Resource actions" button in the toolbar (appears as a three-dot menu icon)
2. Click on the "Resource actions" button
3. Observe the dropdown menu appears

**Expected Results:**
- Resource actions button is visible and clickable
- Menu opens and displays the following options:
  - "View details" menuitem
  - Separator line
  - "Start" menuitem
  - "Commands" menuitem with a submenu indicator (arrow icon)
- Menu remains visible and stable
- Active state is applied to the "Resource actions" button

**Success Criteria:**
- Menu appears within 1 second of clicking
- All menu items are properly labeled and visible
- Menu is positioned correctly below or near the button

---

## Scenario 2: Expand Commands Submenu

**Objective:** Verify that the Commands submenu can be expanded to show available commands

**Preconditions:**
- User is authenticated
- Resource actions menu is open

**Steps:**
1. Hover over the "Commands" menuitem in the Resource actions menu
2. Observe the submenu expansion

**Expected Results:**
- Commands submenu expands on hover
- Submenu displays the following commands:
  - "test" menuitem
  - "dev" menuitem
  - "build" menuitem
  - "start" menuitem
  - "Restart" menuitem (displayed as disabled)
- Expanded state indicator is shown on the Commands menuitem
- Submenu is properly aligned with the parent menu item

**Success Criteria:**
- Submenu appears immediately on hover
- All command menuitems are clearly visible
- Disabled "Restart" command is visually distinct from enabled commands
- Submenu remains stable when hovering over command items

---

## Scenario 3: Execute "test" Command

**Objective:** Verify successful execution of the "test" npm script command

**Preconditions:**
- User is authenticated
- Commands submenu is expanded
- JavaScriptAppWithCommands resource state is "Finished"

**Steps:**
1. Click on the "test" menuitem in the Commands submenu
2. Observe the menu closes
3. Monitor the console logs area for new log entries
4. Check for success notification

**Expected Results:**
- Menu closes immediately after clicking
- Success notification appears: "JavaScriptAppWithCommands 'test' succeeded"
- New console log entries are added showing:
  - "Executing command 'test'."
  - "Exec C:\\Program Files\\nodejs\\npm.cmd run test in folder: {path}"
  - Command execution output
  - "Successfully executed command 'test'."
- Notification includes a close/dismiss button
- Log entries are numbered sequentially
- Log entries include command execution details

**Success Criteria:**
- Command executes within 5 seconds
- Success notification appears
- All expected log entries are present
- No error messages appear
- Logs display in correct chronological order

---

## Scenario 4: Execute "build" Command

**Objective:** Verify successful execution of the "build" npm script command

**Preconditions:**
- User is authenticated
- Commands submenu is expanded
- JavaScriptAppWithCommands resource state is "Finished"

**Steps:**
1. Open Resource actions menu
2. Hover over "Commands" to expand submenu
3. Click on the "build" menuitem
4. Monitor console logs and notifications

**Expected Results:**
- Menu closes after clicking
- Success notification appears: "JavaScriptAppWithCommands 'build' succeeded"
- Console logs show:
  - "Executing command 'build'."
  - "Exec C:\\Program Files\\nodejs\\npm.cmd run build in folder: {path}"
  - Build output: "I am building"
  - "Successfully executed command 'build'."
- Notification is dismissible

**Success Criteria:**
- Build command executes successfully
- Success notification displays within 5 seconds
- Log output matches expected build script output
- No errors or warnings appear

---

## Scenario 5: Execute "dev" Command

**Objective:** Verify successful execution of the "dev" npm script command

**Preconditions:**
- User is authenticated
- Commands submenu is expanded
- JavaScriptAppWithCommands resource state is "Finished"

**Steps:**
1. Open Resource actions menu
2. Hover over "Commands" to expand submenu
3. Click on the "dev" menuitem
4. Monitor console logs and resource state

**Expected Results:**
- Menu closes after clicking
- Success notification appears if command completes
- Console logs show command execution details
- If "dev" is a long-running command, resource state may change to "Running"

**Success Criteria:**
- Command initiates successfully
- Appropriate logs are generated
- System handles long-running processes appropriately

---

## Scenario 6: Execute "start" Command

**Objective:** Verify successful execution of the "start" npm script command

**Preconditions:**
- User is authenticated
- Commands submenu is expanded
- JavaScriptAppWithCommands resource state is "Finished"

**Steps:**
1. Open Resource actions menu
2. Hover over "Commands" to expand submenu
3. Click on the "start" menuitem
4. Monitor console logs and resource state changes

**Expected Results:**
- Menu closes after clicking
- Command execution is logged in console
- Resource may transition to "Running" state if start command launches a persistent process
- Appropriate success or status notification appears

**Success Criteria:**
- Start command executes
- Logs reflect command execution
- Resource state updates appropriately
- No unexpected errors occur

---

## Scenario 7: Verify "Restart" Command Disabled State

**Objective:** Verify that the Restart command is properly disabled when resource is not running

**Preconditions:**
- User is authenticated
- Commands submenu is expanded
- JavaScriptAppWithCommands resource state is "Finished" (not Running)

**Steps:**
1. Open Resource actions menu
2. Hover over "Commands" to expand submenu
3. Locate the "Restart" menuitem
4. Attempt to click on "Restart" menuitem

**Expected Results:**
- "Restart" menuitem is visually indicated as disabled
- Menuitem displays with a distinct icon or styling
- Clicking on "Restart" has no effect (command does not execute)
- No notification or console logs are generated from clicking

**Success Criteria:**
- Disabled state is clearly visible
- Click action is blocked
- No errors or unexpected behavior occurs

---

## Scenario 8: Execute Multiple Commands Sequentially

**Objective:** Verify system handles sequential command execution properly

**Preconditions:**
- User is authenticated
- JavaScriptAppWithCommands resource state is "Finished"

**Steps:**
1. Execute "test" command and wait for completion
2. Execute "build" command and wait for completion
3. Execute "dev" command
4. Review all console logs

**Expected Results:**
- Each command executes in order
- Success notifications appear for each completed command
- Console logs show all command executions with proper sequencing
- Log line numbers increment continuously
- Each command's output is clearly distinguishable
- No commands interfere with each other

**Success Criteria:**
- All commands complete successfully
- Logs are properly ordered and numbered
- No race conditions or overlapping output
- Each command's success notification appears

---

## Scenario 9: View Resource Details from Actions Menu

**Objective:** Verify that View Details option navigates to detailed resource information

**Preconditions:**
- User is authenticated
- User is on console logs page

**Steps:**
1. Click on "Resource actions" button
2. Click on "View details" menuitem
3. Observe page navigation/content change

**Expected Results:**
- Page navigates to resources overview page with detail panel
- Detail panel opens showing JavaScriptAppWithCommands information
- Panel displays:
  - Resource properties (Display name, State, Start time, Stop time, Exit code, etc.)
  - URLs section (if applicable)
  - References section
  - Back references section
  - Health checks section
  - Environment variables section (14 variables)
- All sections are expandable/collapsible
- Data is properly formatted and readable

**Success Criteria:**
- Navigation occurs within 2 seconds
- All resource information displays correctly
- Detail panel is fully functional
- User can close detail panel
- User can navigate back to console logs

---

## Scenario 10: Start Resource from Console Logs Page

**Objective:** Verify that a finished resource can be restarted from the console logs page

**Preconditions:**
- User is authenticated
- JavaScriptAppWithCommands resource state is "Finished"

**Steps:**
1. Locate the "Start resource" button in the toolbar
2. Click on "Start resource" button
3. Monitor resource state change
4. Observe new console log entries

**Expected Results:**
- Resource state changes from "Finished" to "Running"
- New console logs appear showing resource startup
- "Start resource" button changes to "Stop resource" button
- Success notification may appear
- Logs show process initialization

**Success Criteria:**
- Resource starts successfully
- State updates in UI
- Console logs reflect startup process
- Button state updates appropriately

---

## Scenario 11: Pause and Resume Log Streaming

**Objective:** Verify that log streaming can be paused and resumed

**Preconditions:**
- User is authenticated
- Console logs page is displaying logs

**Steps:**
1. Locate the "Pause incoming data" button in the toolbar
2. Click on "Pause incoming data" button
3. Execute a command (e.g., "test")
4. Observe if new logs appear
5. Click the pause button again to resume
6. Observe log updates

**Expected Results:**
- Button toggles between paused and active states
- When paused, new log entries do not automatically appear
- When resumed, all buffered logs appear
- Button visual state indicates current status
- "Watching logs..." status indicator may update

**Success Criteria:**
- Pause functionality works correctly
- Resume functionality restores log streaming
- No log entries are lost
- Button state clearly indicates current mode

---

## Scenario 12: Clear Console Log Data

**Objective:** Verify that console log data can be cleared from the display

**Preconditions:**
- User is authenticated
- Console logs page contains multiple log entries

**Steps:**
1. Locate the "Remove data" button in the toolbar (trash icon)
2. Note the current number of log entries
3. Click on "Remove data" button
4. Observe the console log area

**Expected Results:**
- All log entries are removed from the display
- Console log area shows empty state or initial state
- Action is immediate
- Log numbering resets for new entries
- No confirmation dialog appears (or if it does, user can confirm)

**Success Criteria:**
- All visible logs are cleared
- Clear action completes within 1 second
- New logs start from line 1 or appropriate starting number
- No errors occur

---

## Scenario 13: Navigate to Console Logs via Resource Selector

**Objective:** Verify that users can switch between different resources using the resource selector

**Preconditions:**
- User is authenticated
- Multiple resources exist in the system

**Steps:**
1. Locate the "Resource" combobox showing "JavaScriptAppWithCommands (Finished)"
2. Click on the combobox to open resource list
3. Select a different resource from the dropdown
4. Observe page update

**Expected Results:**
- Combobox opens showing list of available resources
- Each resource shows name and current state
- Selecting a resource updates the console logs to show that resource's logs
- Page title updates to reflect selected resource
- Console logs refresh with new resource's data

**Success Criteria:**
- Resource list displays all available resources
- Selection successfully switches context
- Logs update to show correct resource
- No data from previous resource remains

---

## Scenario 14: Access Console Logs Settings

**Objective:** Verify that console logs settings can be accessed and configured

**Preconditions:**
- User is authenticated
- Console logs page is loaded

**Steps:**
1. Locate the "Settings" button in the toolbar (gear/cog icon)
2. Click on "Settings" button
3. Observe settings panel or modal

**Expected Results:**
- Settings interface opens
- Settings options are displayed
- User can modify log display preferences
- Changes can be saved or cancelled

**Success Criteria:**
- Settings interface is accessible
- Options are clearly labeled
- Changes apply correctly
- Settings persist across sessions (if applicable)

---

## Scenario 15: Verify Console Logs Display Format

**Objective:** Verify that console logs are displayed with proper formatting and information

**Preconditions:**
- User is authenticated
- Console logs page displays log entries

**Steps:**
1. Review existing log entries in the console area
2. Examine log entry structure
3. Verify log entry readability

**Expected Results:**
- Each log entry shows:
  - Line number (sequential)
  - Log message content
- Log entries are displayed in chronological order
- Long messages are properly wrapped or scrollable
- Different log types are distinguishable (if applicable)
- Text is readable with appropriate font size and contrast

**Success Criteria:**
- All log information is visible
- Format is consistent across entries
- Text is readable
- Scrolling works properly for long log lists

---

## Scenario 16: Execute Command When Resource is Running

**Objective:** Verify behavior when attempting to execute commands on a running resource

**Preconditions:**
- User is authenticated
- JavaScriptAppWithCommands resource is in "Running" state

**Steps:**
1. Start the resource (if not already running)
2. Open Resource actions menu
3. Hover over "Commands" to expand submenu
4. Observe "Restart" command state
5. Attempt to execute "Restart" command
6. Observe other command availability

**Expected Results:**
- "Restart" command becomes enabled when resource is running
- Clicking "Restart" stops and restarts the resource
- Other commands may be disabled or show different behavior
- Appropriate notifications appear

**Success Criteria:**
- Command availability reflects resource state
- Restart functionality works correctly
- System handles state transitions properly

---

## Scenario 17: Verify Real-time Log Streaming

**Objective:** Verify that logs stream in real-time when resource is active

**Preconditions:**
- User is authenticated
- A command or resource is running that generates continuous output

**Steps:**
1. Start a resource or command that produces ongoing log output
2. Monitor the console log area
3. Observe new log entries appearing
4. Verify timestamps and ordering

**Expected Results:**
- New log entries appear automatically without manual refresh
- Logs appear in real-time (within 1-2 seconds of generation)
- Auto-scrolling keeps newest logs visible (optional behavior)
- Log numbers increment correctly
- "Watching logs..." indicator shows active state

**Success Criteria:**
- Logs stream continuously
- No manual refresh required
- Performance remains smooth with many log entries
- No log entries are skipped or duplicated

---

## Scenario 18: Handle Command Execution Errors

**Objective:** Verify system properly handles and displays command execution failures

**Preconditions:**
- User is authenticated
- A command that can fail is available (test command shows error message)

**Steps:**
1. Execute a command that produces error output (e.g., "test" command shows "Error: no test specified")
2. Observe console logs
3. Check for error notifications
4. Verify error information is clear

**Expected Results:**
- Error output appears in console logs
- Console logs show error messages clearly
- Success notification still appears if command exits successfully (exit code 0)
- Error information is readable and helpful
- System remains stable after errors

**Success Criteria:**
- Errors are clearly displayed
- System doesn't crash or hang
- User can continue using the application
- Error information aids troubleshooting

---

## Scenario 19: Navigate Between Different Monitoring Views

**Objective:** Verify navigation between Console, Structured logs, Traces, and Metrics views

**Preconditions:**
- User is authenticated
- User is on console logs page

**Steps:**
1. Click on "Structured" navigation link
2. Observe page navigation
3. Return to "Console" view
4. Click on "Traces" navigation link
5. Return to "Console" view
6. Click on "Metrics" navigation link
7. Return to "Console" view

**Expected Results:**
- Each navigation link is clearly visible in the side navigation
- Clicking each link navigates to the corresponding view
- Active view is highlighted in navigation
- Resource context is maintained across views
- Navigation is smooth without errors

**Success Criteria:**
- All navigation links are functional
- Views load within 2 seconds
- Active state correctly indicates current view
- No data loss when switching views

---

## Scenario 20: Test Keyboard Navigation and Accessibility

**Objective:** Verify that the interface is keyboard accessible and follows accessibility standards

**Preconditions:**
- User is authenticated
- Console logs page is loaded

**Steps:**
1. Use Tab key to navigate through interactive elements
2. Use Enter/Space to activate buttons
3. Use arrow keys to navigate menus (if supported)
4. Use Escape to close menus
5. Verify focus indicators are visible

**Expected Results:**
- All interactive elements are keyboard accessible
- Focus order is logical and sequential
- Focus indicators are clearly visible
- Enter/Space keys activate buttons and menu items
- Escape key closes open menus
- Screen reader announcements are meaningful (if testing with screen reader)

**Success Criteria:**
- Complete keyboard navigation is possible
- Focus is always visible
- Keyboard shortcuts work as expected
- No keyboard traps exist
- ARIA attributes are properly implemented

---

## Edge Cases and Negative Testing

### Edge Case 1: Rapid Command Execution
**Steps:**
1. Execute "test" command
2. Immediately execute "build" command before "test" completes
3. Execute "dev" command

**Expected:** System queues or handles concurrent command requests gracefully

### Edge Case 2: Network Interruption During Command Execution
**Steps:**
1. Execute a command
2. Simulate network interruption (if possible)
3. Restore network connection

**Expected:** System recovers gracefully, shows appropriate error or retry mechanism

### Edge Case 3: Extremely Long Log Output
**Steps:**
1. Execute command that generates thousands of log lines
2. Monitor UI performance

**Expected:** UI remains responsive, implements pagination or virtualization if needed

### Edge Case 4: Special Characters in Log Output
**Steps:**
1. Execute command that outputs special characters, unicode, or escape sequences
2. Verify log display

**Expected:** Special characters render correctly or are escaped appropriately

### Edge Case 5: Browser Refresh During Command Execution
**Steps:**
1. Execute a long-running command
2. Refresh the browser page
3. Return to console logs page

**Expected:** Page loads correctly, shows current state of resource and recent logs

---

## Performance Criteria

- Page load time: < 3 seconds
- Command execution response: < 5 seconds for short commands
- Menu open/close: < 500ms
- Log streaming latency: < 2 seconds
- UI responsiveness: No freezing or blocking operations
- Memory usage: Stable even with 1000+ log entries

---

## Browser Compatibility

Test on:
- Chrome (latest)
- Edge (latest)
- Firefox (latest)
- Safari (latest, if applicable)

---

## Security Considerations

- Verify token authentication is required
- Ensure commands execute with appropriate permissions
- Validate that sensitive data in logs can be masked
- Confirm that environment variables are properly masked in UI (shown as ●●●●●●●●)

---

## Accessibility Requirements

- WCAG 2.1 Level AA compliance
- Keyboard navigation support
- Screen reader compatibility
- Sufficient color contrast
- Focus indicators visible
- Meaningful link and button labels

---

## Notes for Test Execution

1. **Test Environment Setup**: Ensure the JavaScriptExtensions application is running with the JavaScriptAppWithCommands resource configured
2. **Authentication Token**: Use the provided token in the login URL for authentication
3. **Resource State**: Some tests require the resource to be in "Finished" state, others in "Running" state
4. **Log Cleanup**: Consider clearing logs between test scenarios for clarity
5. **Command Availability**: The available commands depend on the package.json scripts in the JavaScript application
6. **Documentation**: Take screenshots of key interactions for test evidence
7. **Error Logging**: Document any unexpected behavior or errors encountered during testing

---

## Test Completion Checklist

- [ ] All 20 test scenarios executed
- [ ] Edge cases tested
- [ ] Performance criteria verified
- [ ] Accessibility validated
- [ ] Browser compatibility confirmed
- [ ] Security aspects reviewed
- [ ] Defects documented
- [ ] Test results compiled
- [ ] Test report created
