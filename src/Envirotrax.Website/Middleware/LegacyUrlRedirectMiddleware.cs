
using Envirotrax.Website.Configuration;
using Envirotrax.Website.Routing;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Envirotrax.Website.Middleware;

public class LegacyUrlRedirectMiddleware
{
    private readonly RequestDelegate _next;

    public LegacyUrlRedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IUmbracoContextFactory umbracoContextFactory,
        IDocumentUrlService documentUrlService,
        IPublishedUrlProvider publishedUrlProvider,
        IOptionsMonitor<LegacyUrlRedirectOptions> options,
        ILogger<LegacyUrlRedirectMiddleware> logger)
    {
        var rawPath = context.Request.Path.Value ?? string.Empty;

        if (!LegacyUrlResolver.IsLegacyRequest(rawPath))
        {
            await _next(context);
            return;
        }

        if (!documentUrlService.HasAny())
        {
            await _next(context);
            return;
        }

        var requestPath = LegacyUrlResolver.Normalize(rawPath);
        var targetPath = LegacyUrlResolver.ResolveCandidateTarget(requestPath, options.CurrentValue.Overrides);

        // UmbracoRequestMiddleware skips creating IUmbracoContext for any path with a file extension
        // (treated as a client-side/static-asset request), so ".aspx" paths never get one upstream.
        // EnsureUmbracoContext creates one here if needed, or reuses the ambient one if it already exists.
        using var umbracoContextReference = umbracoContextFactory.EnsureUmbracoContext();
        var umbracoContext = umbracoContextReference.UmbracoContext;

        var documentKey = documentUrlService.GetDocumentKeyByRoute(
            targetPath,
            culture: null,
            documentStartNodeId: null,
            isDraft: umbracoContext.InPreviewMode);

        var content = documentKey.HasValue
            ? umbracoContext.Content?.GetById(umbracoContext.InPreviewMode, documentKey.Value)
            : null;

        if (content == null)
        {
            logger.LogWarning(
                "Legacy URL {RequestPath} did not resolve to a published V2 page (checked target {TargetPath}). Add an entry to LegacyUrlRedirects:Overrides if this URL should redirect.",
                requestPath, targetPath);

            await _next(context);
            return;
        }

        var redirectUrl = content.Url(publishedUrlProvider, mode: UrlMode.Relative);

        if (context.Request.QueryString.HasValue)
        {
            redirectUrl += context.Request.QueryString.Value;
        }

        context.Response.Redirect(redirectUrl, permanent: true);
    }
}
