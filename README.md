# ConanSharpmake - glue between Conan and Sharpmake.

This utility consumes "conanbuildinfo.json" (the product of Conan's JSON generator).

In the current state its usable, but lacks proper support for platform|environment|optimization differentiation (those values are hardcoded).
This could serve as a bootstrap for the generator in your own project.

