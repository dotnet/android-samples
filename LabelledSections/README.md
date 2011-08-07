Labelled Sections 
=================

The `Labelled Sections` demo is a port from the
[Alphabetically ordered ListView with labelled sections](http://androidseverywhere.info/JAAB/?p=6)
sample, with some changes to follow .NET conventions.

The `ListItemInterface` interface is the [IHasLabel](IHasLabel.cs) interface.

The `ListItemObject` class is the [ListItemValue](ListItemValue.cs) class.

The `ListItemContainer` class is the
[ListItemCollection](ListItemCollection.cs) class.

The `ListWithHeaders` activity is the [Activity1](Activity1.cs) activity.

The `res/layout` XML files are unchanged, though `list_item.xml` has been
renamed to [ListItem.axml](Resources/layout/ListItem.axml) and
`list_header.xml` has been renamed to
[ListHeader.axml](Resources/layout/ListHeader.axml).

The `SeparatedListAdapter` type is the
[SeparatedListAdapter](SeparatedListAdapter.cs) type.
