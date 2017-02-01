namespace Rhythm.Globalization.Umbraco
{

    // Namespaces.
    using Caching.Core.Caches;
    using Caching.Umbraco.Invalidators;
    using Core;
    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    using System;
    using System.Linq;
    using System.Web;
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
        /// The ID's of the translation folders by the parent node.
        /// </summary>
        private static InstanceByKeyCache<int?, int> TranslationFolderNodeIds { get; set; }

        /// <summary>
        /// Invalidates the cache of translation folders.
        /// </summary>
        private static InvalidatorByParentPage<int?> TranslateFolderInvalidator { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static TranslationHelper()
        {
            TranslationNodeIds = new InstanceByKeyCache<int, NodeAndCulture>();
            TranslationFolderNodeIds = new InstanceByKeyCache<int?, int>();
            TranslateFolderInvalidator = new InvalidatorByParentPage<int?>(
                TranslationFolderNodeIds);
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
        /// <returns>
        /// The node containing the translations.
        /// </returns>
        public static IPublishedContent GetTranslationNode(IPublishedContent page,
            string culture)
        {

            // Validate input.
            if (page == null)
            {
                return null;
            }

            // Variables.
            var duration = TimeSpan.FromHours(1);
            var key = new NodeAndCulture(page.Id, culture);
            var keys = RhythmCacheHelper.PreviewCacheKeys;

            // Get the node ID from the cache.
            var translationNodeId = TranslationNodeIds.Get(key, nodeAndCulture =>
            {

                // Look for a translation folder.
                var folderNode = GetTranslationFolderForNode(page);

                // Look for a translation node under the folder.
                var translationNode = folderNode == null
                    ? null
                    : GetTranslationNodeFromFolder(folderNode, culture);
                return (translationNode ?? page).Id;

            }, duration, keys: keys);

            // Return translation node.
            var cache = UmbracoContext.Current.ContentCache;
            return cache.GetById(translationNodeId);

        }

        /// <summary>
        /// Gets the node containing the translations.
        /// </summary>
        /// <param name="page">
        /// The node to get the translation node for.
        /// </param>
        /// <returns>
        /// The node containing the translations.
        /// </returns>
        public static IPublishedContent GetTranslationNode(IPublishedContent page)
        {

            // Variables.
            var requestUrl = HttpContext.Current.Request.Url.ToString();
            var culture = GlobalizationHelper.GetCulture(requestUrl);

            // Return translation node.
            return GetTranslationNode(page, culture);

        }

        /// <summary>
        /// Returns a dictionary translation for the specified dictionary key.
        /// </summary>
        /// <param name="key">
        /// The Umbraco dictionary key.
        /// </param>
        /// <returns>
        /// The translation.
        /// </returns>
        public static string GetDictionaryTranslation(string key)
        {
            var requestUrl = HttpContext.Current.Request.Url.ToString();
            var culture = GlobalizationHelper.GetCulture(requestUrl);
            return GetDictionaryTranslation(key, culture);
        }

        /// <summary>
        /// Returns a dictionary translation for the specified dictionary key in the specified culture.
        /// </summary>
        /// <param name="key">
        /// The Umbraco dictionary key.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The translation.
        /// </returns>
        public static string GetDictionaryTranslation(string key, string culture)
        {
            var service = ApplicationContext.Current.Services.LocalizationService;
            var item = service.GetDictionaryItemByKey(key);
            return item?.Translations?.FirstOrDefault(x =>
                culture.InvariantEquals(x.Language.IsoCode))?.Value;
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
            IPublishedContent translationFolder, string culture)
        {

            // Find the translation node based on the language property.
            var translationNode = translationFolder.Children
                .Select(x => new
                {
                    Node = x,
                    Language = x.GetPropertyValue<string>("language")
                })
                .Where(x => x.Language != null)
                .Where(x => culture.InvariantEquals(x.Language))
                .Select(x => x.Node)
                .FirstOrDefault();

            // Return the translation node.
            return translationNode;

        }

        /// <summary>
        /// Gets the translation folder for the specified page.
        /// </summary>
        /// <param name="page">
        /// The page to get the translation folder for.
        /// </param>
        /// <returns>
        /// The translation folder, or null.
        /// </returns>
        private static IPublishedContent GetTranslationFolderForNode(IPublishedContent page)
        {

            // Variables.
            var duration = TimeSpan.FromHours(1);
            var keys = RhythmCacheHelper.PreviewCacheKeys;
            var cache = UmbracoContext.Current.ContentCache;

            // Get the translation folder ID from the cache.
            var folderNodeId = TranslationFolderNodeIds.Get(page.Id, pageId =>
            {

                // Variables.
                var folderDocType = page.DocumentTypeAlias + "TranslationFolder";

                // Look for a translation folder.
                var folderNode = page.Children
                    .Where(x => folderDocType.InvariantEquals(x.DocumentTypeAlias))
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