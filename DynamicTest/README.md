C# Dynamic Test
===============

This samples makes use of dynamic types in DLR. This sample imports
System.Json from ASP.NET MVC sources which includes JsonValue.AsDynamic()
which allows users to directly specify object keys as field names.

This sample retrieves Twitter public timeline JSON and parses it into
the ListAdapter. Hence it requires internet access (and working twitter).

Bonus:
You can also add reference to mono-reactive for Android from
https://github.com/atsushieno/mono-reactive , add REACTIVE symbol to
compiler options and get things done asynchronously.
