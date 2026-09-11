Review my current changes against the project conventions AND against the V1 reference implementation.

Steps:
1. Read `.claude/agents/envirotrax-angular-feature.md` — these are the Angular conventions
2. Read `.claude/agents/envirotrax-backend-feature.md` — these are the backend conventions
3. Identify which files are relevant: use `$ARGUMENTS` if provided, otherwise use the git diff (`git diff HEAD`) to find changed files
4. Read every changed file in full before judging it
5. Check each file against the conventions from steps 1 and 2
6. For every feature or behaviour in the changed files, find the equivalent in the V1 codebase at `C:\Envirotrax` and compare:
   - Does the V2 logic match V1 behaviour?
   - Are there V1 fields, rules, or edge cases missing in V2?
   - Are there V1 UI labels, help text, or validation messages that differ in V2?
   Use Glob and Grep to search `C:\Envirotrax` — look for matching table names, field names, page names, or keywords from the changed code.

Report format:
- **Conventions** section: for each file list PASS items briefly, then FAIL items with file path, line number, what is wrong, and the exact fix needed
- **V1 Comparison** section: for each changed feature list differences found between V2 and V1, with the V1 file reference and what needs to change in V2
- **Summary** of all failures ordered by severity: bug > missing V1 behaviour > convention violation > style
