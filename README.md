# unity3d-custom-popup

Custom popup control for Unity editor interfaces that defers popup menu construction until
shown. This is good for situations where the popup is relatively expensive to construct.

```sh
$ yarn add rotorz/unity3d-custom-popup
```

This package is compatible with the [unity3d-package-syncer][tool] tool. Refer to the
tools' [README][tool] for information on syncing packages into a Unity project.

[tool]: https://github.com/rotorz/unity3d-package-syncer


## Basic Usage Example

Here is a basic example of implementing a custom popup control:

```csharp
public static int SortingLayerPopup(Rect position, GUIContent label, int sortingLayerID)
{
    string sortingLayerName = SortingLayer.IDToName(sortingLayerID);
    using (var valueLabel = ControlContent.Basic(sortingLayerName)) {
        return CustomPopupGUI.Popup(position, label, sortingLayerID, valueLabel, context => {
            foreach (var layer in SortingLayer.layers) {
                context.Popup.AddOption(layer.name, context, layer.id);
            }
        });
    }
}

public static int SortingLayerPopup(GUIContent label, int sortingLayerID)
{
    var position = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.popup);
    return SortingLayerPopup(position, label, sortingLayerID);
}
```

Which can then be used as follows:

```csharp
this.sortingLayerID = SortingLayerPopup(new GUIContent("Sorting Layer", this.sortingLayerID));
```


## Verbose Usage Example

A more verbose approach can be used when more control is required when adding options:

```csharp
context.Popup.AddCommand(layer.name)
    .Checked(() => layer.id == context.CurrentValue)
    .Action(() => context.NotifySelection(layer.id));
```


## Contribution Agreement

This project is licensed under the MIT license (see LICENSE). To be in the best
position to enforce these licenses the copyright status of this project needs to
be as simple as possible. To achieve this the following terms and conditions
must be met:

- All contributed content (including but not limited to source code, text,
  image, videos, bug reports, suggestions, ideas, etc.) must be the
  contributors own work.

- The contributor disclaims all copyright and accepts that their contributed
  content will be released to the public domain.

- The act of submitting a contribution indicates that the contributor agrees
  with this agreement. This includes (but is not limited to) pull requests, issues,
  tickets, e-mails, newsgroups, blogs, forums, etc.
