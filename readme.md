# Simple static analysis tool

Simple static analysis(SA) tool to analyze .Net assemblies at the IL-code level.

## The main features

- navigation between classes and methods within loaded assemblies with no source code needed
- predefined subset of patterns for analyzing security\performance or compatibility threats
- threats highlighting and reporting with different level of details
- method-by-method navigation and backtrace analysis
- interactive analysis via dynamically created subsets of dangerous members

Powered by exiting [HTML and CSS-based Desktop UI](https://docs.devexpress.com/WindowsForms/403397/common-features/html-css-based-desktop-ui) by [DevExpress WinForms](https://www.devexpress.com/products/net/controls/winforms/).

### UI - Classes and Patterns pages

Classes page - here you can load and inspect types and methods

![Classes](/IMGS/Classes.png)

Patterns page - here you can load and activate\deactivate available patterns

![Patterns](/IMGS/Patterns.png)

### Features

Backtrace - you can inspect the problematical method callers

![BackTrace](/IMGS/BackTrace.png)

### License

The ILSATools application is licensed under the [MIT](https://github.com/DmitryGaravsky/ILSATools/blob/master/LICENSE.TXT) license.
