# Rhythm.Globalization.Umbraco

<table>
<tbody>
<tr>
<td><a href="#translationhelper">TranslationHelper</a></td>
<td><a href="#nodeandculture">NodeAndCulture</a></td>
</tr>
</tbody>
</table>


## TranslationHelper

Helps with translations.

### #cctor

Static constructor.

### GetDictionaryTranslation(key, culture)

Returns a dictionary translation for the specified dictionary key in the specified culture.

| Name | Description |
| ---- | ----------- |
| key | *System.String*<br>The Umbraco dictionary key. |
| culture | *System.String*<br>The culture. |

#### Returns

The translation.

### GetDictionaryTranslation(key)

Returns a dictionary translation for the specified dictionary key.

| Name | Description |
| ---- | ----------- |
| key | *System.String*<br>The Umbraco dictionary key. |

#### Returns

The translation.

### GetTranslationFolderForNode(page)

Gets the translation folder for the specified page.

| Name | Description |
| ---- | ----------- |
| page | *Umbraco.Core.Models.IPublishedContent*<br>The page to get the translation folder for. |

#### Returns

The translation folder, or null.

### GetTranslationNode(page, culture)

Gets the node containing the translations.

| Name | Description |
| ---- | ----------- |
| page | *Umbraco.Core.Models.IPublishedContent*<br>The node to get the translation node for. |
| culture | *System.String*<br>The culture (e.g., "es-mx") to get the translation node for. |

#### Returns

The node containing the translations.

### GetTranslationNode(page)

Gets the node containing the translations.

| Name | Description |
| ---- | ----------- |
| page | *Umbraco.Core.Models.IPublishedContent*<br>The node to get the translation node for. |

#### Returns

The node containing the translations.

### GetTranslationNodeFromFolder(translationFolder, culture)

Gets the translation node under the specified translation folder.

| Name | Description |
| ---- | ----------- |
| translationFolder | *Umbraco.Core.Models.IPublishedContent*<br>The translation folder. |
| culture | *System.String*<br>The culture to find the translation node for. |

#### Returns

The translation node, or null.

### TranslateFolderInvalidator

Invalidates the cache of translation folders.

### TranslationFolderNodeIds

The ID's of the translation folders by the parent node.

### TranslationNodeIds

The ID's of the translations nodes by the parent node and culture.


## NodeAndCulture

Stores a node ID and a culture code.

#### Remarks

Equality is checked by comparing the node ID and culture code of two instances of this class.

### Constructor(nodeId, culture)

Primary constructor.

| Name | Description |
| ---- | ----------- |
| nodeId | *System.Int32*<br>The node ID. |
| culture | *System.String*<br>The culture code. |

### Culture

The culture code.

### Equals(other)

Compares this instance against another for equality.

| Name | Description |
| ---- | ----------- |
| other | *Rhythm.Globalization.Umbraco.Types.NodeAndCulture*<br>The other instance. |

#### Returns

True, if the instances are equal; otherwise, false.

### Equals(other)

Compares this instance against another for equality.

| Name | Description |
| ---- | ----------- |
| other | *System.Object*<br>The other instance. |

#### Returns

True, if the instances are equal; otherwise, false.

### GetHashCode

Returns a hash code for this instance.

#### Returns

The hash code.

### NodeId

The node ID.
