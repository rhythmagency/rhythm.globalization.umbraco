namespace Rhythm.Globalization.Umbraco
{
    // Namespaces.
    using Caching.Core.Caches;
    using Caching.Umbraco.Invalidators;
    using global::Umbraco.Cms.Core.Models.PublishedContent;
    using global::Umbraco.Cms.Core.Web;
    using global::Umbraco.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Types;
    using RhythmCacheHelper = Caching.Umbraco.CacheHelper;

    /// <summary>
    /// Helps with translations.
    /// </summary>
    public class TranslationHelper
    {

        #region Properties

        /// <summary>
        /// The ID's of the translations nodes by the parent node and culture.
        /// </summary>
        private static InstanceByKeyCache<int, NodeAndCulture> TranslationNodeIds { get; set; }

        /// <summary>
        /// The cultures that have been translated for the node indicated by the node ID of the key.
        /// </summary>
        private static InstanceByKeyCache<IEnumerable<string>, int> TranslationCulturesByNodeIds { get; set; }

        /// <summary>
        /// The ID's of the translation folders by the parent node.
        /// </summary>
        private static InstanceByKeyCache<int?, int> TranslationFolderNodeIds { get; set; }

        /// <summary>
        /// Invalidates the cache of translation folders.
        /// </summary>
        private static InvalidatorByParentPage<int?> TranslateFolderInvalidator { get; set; }

        /// <summary>
        /// The translations stored by term.
        /// </summary>
        private static InstanceByKeyCache<string, string> Translations { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static TranslationHelper()
        {
            TranslationNodeIds = new InstanceByKeyCache<int, NodeAndCulture>();
            TranslationCulturesByNodeIds = new InstanceByKeyCache<IEnumerable<string>, int>();
            TranslationFolderNodeIds = new InstanceByKeyCache<int?, int>();
            TranslateFolderInvalidator = new InvalidatorByParentPage<int?>(
                TranslationFolderNodeIds);
            Translations = new InstanceByKeyCache<string, string>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the node containing the translations.
        /// </summary>
        /// <param name="page">
        /// The node to get the translation node for.
        /// </param>
        /// <param name="culture">
        /// The culture (e.g., "es-mx") to get the translation node for.
        /// </param>
        /// <param name="umbracoContext"></param>
        /// <returns>
        /// The node containing the translations.
        /// </returns>
        public static IPublishedContent GetTranslationNode(
            IPublishedContent page,
            string culture, 
            IUmbracoContext umbracoContext)
        {

            // Validate input.
            if (page == null)
            {
                return null;
            }

            // Variables.
            var duration = TimeSpan.FromHours(1);
            var key = new NodeAndCulture(page.Id, culture);
            var keys = RhythmCacheHelper.PreviewCacheKeys(umbracoContext);

            // Get the node ID from the cache.
            var translationNodeId = TranslationNodeIds.Get(key, nodeAndCulture =>
            {

                // Look for a translation folder.
                var folderNode = GetTranslationFolderForNode(page, umbracoContext);

                // Look for a translation node under the folder.
                var translationNode = folderNode == null
                    ? null
                    : GetTranslationNodeFromFolder(folderNode, culture);
                return (translationNode ?? page).Id;

            }, duration, keys: keys);

            // Return translation node.
            var cache = umbracoContext.Content;
            return cache.GetById(translationNodeId);

        }

        /// <summary>
        /// Returns the cultures that have a translation for the specified page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="context"></param>
        /// <returns>
        /// The cultures.
        /// </returns>
        /// <remarks>
        /// Does not include the default culture. Only includes the cultures specified on
        /// the translation nodes.
        /// </remarks>
        public static IEnumerable<string> GetTranslatedCultures(
            IPublishedContent page, 
            IUmbracoContext context)
        {

            // Variables.
            var duration = TimeSpan.FromHours(1);
            var key = page.Id;
            var keys = RhythmCacheHelper.PreviewCacheKeys(context);

            // Get the cultures from the cache.
            var cultures = TranslationCulturesByNodeIds.Get(key, pageId =>
            {

                // Look for a translation folder.
                var folderNode = GetTranslationFolderForNode(page, context);

                // Look for the translation nodes under the folder.
                var translationNodes = folderNode == null
                    ? Enumerable.Empty<IPublishedContent>()
                    : GetAllTranslationNodesFromFolder(folderNode);

                // Return the cultures.
                return translationNodes
                    .Select(x => x.Value<string>("language"))
                    .ToArray();

            }, duration, keys: keys);

            // Return the cultures.
            return cultures;

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the translation node under the specified translation folder.
        /// </summary>
        /// <param name="translationFolder">
        /// The translation folder.
        /// </param>
        /// <param name="culture">
        /// The culture to find the translation node for.
        /// </param>
        /// <returns>
        /// The translation node, or null.
        /// </returns>
        private static IPublishedContent GetTranslationNodeFromFolder(
            IPublishedContent translationFolder, 
            string culture)
        {

            // Find the translation node based on the language property.
            var translationNode = translationFolder.Children
                .Select(x => new
                {
                    Node = x,
                    Language = x.Value<string>("language")
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Language))
                .Where(x => culture.InvariantEquals(x.Language))
                .Select(x => x.Node)
                .FirstOrDefault();

            // Return the translation node.
            return translationNode;

        }

        /// <summary>
        /// Returns all translation nodes under the specified translation folder.
        /// </summary>
        /// <param name="translationFolder">
        /// The translation folder.
        /// </param>
        /// <returns>
        /// The translation nodes.
        /// </returns>
        private static IEnumerable<IPublishedContent> GetAllTranslationNodesFromFolder(
            IPublishedContent translationFolder)
        {
            return translationFolder.Children
                .Select(x => new
                {
                    Node = x,
                    Language = x.Value<string>("language")
                })
                // Only include nodes that have a language.
                .Where(x => !string.IsNullOrWhiteSpace(x.Language))
                .Select(x => x.Node)
                .ToArray();
        }

        /// <summary>
        /// Gets the translation folder for the specified page.
        /// </summary>
        /// <param name="page">
        /// The page to get the translation folder for.
        /// </param>
        /// <param name="context"></param>
        /// <returns>
        /// The translation folder, or null.
        /// </returns>
        private static IPublishedContent GetTranslationFolderForNode(
            IPublishedContent page, 
            IUmbracoContext context)
        {

            // Variables.
            var duration = TimeSpan.FromHours(1);
            var keys = RhythmCacheHelper.PreviewCacheKeys(context);
            var cache = context.Content;

            // Get the translation folder ID from the cache.
            var folderNodeId = TranslationFolderNodeIds.Get(page.Id, pageId =>
            {

                // Variables.
                var folderDocType = page.ContentType.Alias + "TranslationFolder";

                // Look for a translation folder.
                var folderNode = page.Children
                    .Where(x => folderDocType.InvariantEquals(x.ContentType.Alias))
                    .FirstOrDefault();

                // Return the folder node ID.
                return folderNode?.Id;

            }, duration, keys: keys);

            // Return translation folder node.
            return folderNodeId.HasValue
                ? cache.GetById(folderNodeId.Value)
                : null;

        }

        #endregion

    }

}