---
name: Xamarin.Android - Labelled Sections
description: "Port from the Alphabetically ordered ListView with labelled sections sample, with some changes to follow .NET conventions"
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: labelledsections
---
# Labelled Sections

The `Labelled Sections` demo is a port from the
`Alphabetically ordered ListView with labelled sections`
sample, with some changes to follow .NET conventions.

The `ListItemInterface` interface is the
[IHasLabel](https://github.com/xamarin/monodroid-samples/blob/master/LabelledSections/IHasLabel.cs) interface.

The `ListItemObject` class is the
[ListItemValue](https://github.com/xamarin/monodroid-samples/blob/master/LabelledSections/ListItemValue.cs) class.

The `ListItemContainer` class is the
[ListItemCollection](https://github.com/xamarin/monodroid-samples/blob/master/LabelledSections/ListItemCollection.cs)
class.

The `ListWithHeaders` activity is the
[Activity1](https://github.com/xamarin/monodroid-samples/blob/master/LabelledSections/Activity1.cs) activity.

The `res/layout` XML files are unchanged, though `list_item.xml` has been
renamed to [ListItem.axml](https://github.com/xamarin/monodroid-samples/blob/master/LabelledSections/Resources/layout/ListItem.axml)
and `list_header.xml` has been renamed to
[ListHeader.axml](https://github.com/xamarin/monodroid-samples/blob/master/LabelledSections/Resources/layout/ListHeader.axml).

The `SeparatedListAdapter` type is the
[SeparatedListAdapter](https://github.com/xamarin/monodroid-samples/blob/master/LabelledSections/SeparatedListAdapter.cs)
type.
