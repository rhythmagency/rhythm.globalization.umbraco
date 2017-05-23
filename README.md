# Introduction

A library of tools for globalization in Umbraco. The primary class is `TranslationHelper`.

Refer to the [generated documentation](docs/generated.md) for more details.

# Installation

Install with NuGet. Search for "Rhythm.Globalization.Umbraco".

# Overview

## TranslationHelper

* **GetTranslationNode** Gets the Umbraco content node containing the translations.
* **GetDictionaryTranslation** Returns an Umbraco dictionary translation for the specified dictionary term.
* **GetTranslatedCultures** Returns the cultures that have a translation for the specified page.

Note that these translation methods assume you have your translations stored in subnodes,
as would be the case if you were using [Polyglot](https://our.umbraco.org/projects/backoffice-extensions/polyglot/).
Here's an example of the expected node structure:

![Translations](assets/images/translations.png?raw=true "Translations")

# Maintainers

To create a new release to NuGet, see the [NuGet documentation](docs/nuget.md).