# Coding Standards

## Braces

All `if` statements, `for`/`foreach`/`while` loops, and other control flow blocks **must** use curly braces — even for single-line bodies.

```csharp
// Wrong
if (condition)
    DoSomething();

// Correct
if (condition)
{
    DoSomething();
}
```

## Readability — Empty Lines as Paragraph Breaks

Code must be easy to read. Use empty lines to group related lines of logic together, similar to paragraphs in prose. A 10-line function should have a few blank lines that visually separate distinct steps.

```csharp
// Wrong — dense, no visual structure
public async Task<string> ProcessSite(int siteId)
{
    var site = await _siteRepository.GetByIdAsync(siteId);
    if (site == null)
    {
        return null;
    }
    var logs = await _logRepository.GetBySiteIdAsync(siteId);
    var summary = BuildSummary(site, logs);
    await _cacheService.SetAsync($"site:{siteId}", summary);
    return summary;
}

// Correct — steps are visually separated
public async Task<string> ProcessSite(int siteId)
{
    var site = await _siteRepository.GetByIdAsync(siteId);
    
    if (site == null)
    {
        return null;
    }

    var logs = await _logRepository.GetBySiteIdAsync(siteId);
    var summary = BuildSummary(site, logs);

    await _cacheService.SetAsync($"site:{siteId}", summary);

    return summary;
}
```

The goal is for a reader to scan the function and immediately see its phases — fetch, validate, transform, persist, return — without having to parse every line to understand the structure.
