# Simple static analysis tool

Simple static analysis(SA) tool to analyze .Net assemblies at the IL-code level with visual client.

![BackTrace](/IMGS/BackTrace.png)

Powered by [ILReader](https://github.com/DmitryGaravsky/ILReader) and exiting [HTML and CSS-based Desktop UI](https://docs.devexpress.com/WindowsForms/403397/common-features/html-css-based-desktop-ui) by [DevExpress WinForms](https://www.devexpress.com/products/net/controls/winforms/).

## The main features

- navigation between classes and methods within loaded assemblies with no source code needed
- predefined subset of patterns for analyzing security\performance or compatibility threats
- threats highlighting and reporting with different level of details
- method-by-method navigation and backtrace analysis
- interactive analysis via dynamically created subsets of dangerous members

Read the [Why simple Static Analysys tools are important and how easy to use they are](/Articles/01-Simple-Static-Analysis.md) article to learn how it works and how to use this tool effectively.

## Options for ILSA.Tools users

Available for free:

- Downloading a nuget package of `ILSA.Core` library for your own needs (TODO),
- Downloading a Visual Client and analysing your libraries with predefined set of diagnostics,
- Creating your own diagnostics with `ILSA.Core` library (TODO).

Available for professional users:

- Using a minified version for embedding into your projects as a single-file-of-code solution for TDD,
- Using special runners for TDD approaches (assembly level, type level checking),
- Using a command-line mode tool for CI\CD purposes.

Additionally, for ultimate version users and sponsors you can:
- request for personalized version of Visual Client with your name and logo,
- request for adding some specific diagnostics.

Professional and ultimate version requests are available for any users via GaravskyDmitry@gmail.com.


### UI - Classes and Patterns pages

Classes page - here you can load and inspect types and methods

![Classes](/IMGS/Classes.png)

Patterns page - here you can load and activate\deactivate available patterns

![Patterns](/IMGS/Patterns.png)

### License

The ILSATools application is licensed under the [MIT](https://github.com/DmitryGaravsky/ILSATools/blob/master/LICENSE.TXT) license.
