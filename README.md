NancyOwinEmbeddedStaticFiles
============================

This repo serves to repro issues servinge static files to NancyFx when they are embedded as resources within a separate assembly.

Microsoft.Owin.StaticFiles is used to serve embedded resource static files for consumption by Nancy.

The repro tests demonstrate that these files are accessible over HTTP, and  Nancy can locate and serve Embedded Views 
AJAX requests initiating from these views can locate dependant, static resources (such as .js files).

Core.csproj
===========
where Microsoft.Owin.StaticFiles & Nancy.Owin are installed and execute from Startup.cs

UI.csrpoj
===========
contains embedded resource files as static content for Core.csproj

_Test.Core.Test.csproj
===========
automated tests ; All tests will pass 
