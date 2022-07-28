## 0.9.0 [19.07.2021]

### Added:
- FormattedNumberAttribute
- SceneAsset picker for the SceneNameAttribute
- Optional foldout for the ReorderableList and related attributes
- GuiColorAttribute

### Changed:

- Remove obsolete attributes
- Rename ToolboxCompositionAttribute class to ToolboxArchetypeAttribute

## 0.8.13 [04.07.2021]

### Added:
- Begin/EndHorizontalGroupAttribute

### Changed:
- Fix overall layouting issues
- Fix IgnoreParentAttribute issues

## 0.8.11 [02.07.2021]

### Added:
- IgnoreParentAttribute
- ShowDisabledIfAttribute
- HideDisabledIfAttribute
- Default constraint to the SerializedType drawer

### Changed:
- Fix assigning and clearing multiple drawers at once
- Fix drawing default numeric types (Vector2, Vector3, etc.)
- Fix drawing ReorderableList within horizontal groups

## 0.8.6 [13.06.2021]

### Added:
- Additional search field for the SerializedType drawer

## 0.8.5 [13.05.2021]

### Added:
- Composition attributes
- ReorderableListExposedAttribute
- OnValueChangedAttribute
- ShowIfWarningAttribute
- Equation test methods for comparison attributes

### Changed:
- Fix finding indexes of the serialized elements within reflection-based drawers
- Fix the 'Value' label appearance in the SerializedDictionary drawer when the Value property is an array

## 0.8.2 [03.04.2021]

### Added:
- Additional comparison methods for the built-in conditional attributes

## 0.8.1 [03.04.2021]

### Added:
- SerializedDictionary + associated drawer
- Layout-based ReorderableList implementation
- Support for ToolboxDrawers in the ReorderableList
- Possibility to hide labels in the ReorderableList

### Changed:
- Fix items focusing on nested lists
- Fix empty list drawing
- Fix scrolling behaviour outside SearchablePopup
- Unification of available attributes

## 0.7.5 [06.01.2021]

## 0.7.3 [28.12.2020]

## 0.7.1 [20.10.2020]

## 0.7.0 [29.09.2020]

## 0.6.5 [08.09.2020]

## 0.6.4 [05.09.2020]

## 0.6.3 [31.08.2020]

## 0.6.2 [28.08.2020]

## 0.6.1 [25.08.2020]

## 0.6.0 [21.08.2020]

## 0.5.9 [16.08.2020]

## 0.5.7 [05.08.2020]

### Added:
- Possibility to customize displayed data inside the Hierarchy window (check the ToolboxEditorHierarchy.cs)

## 0.5.6 [16.01.2020]

### Added:
- Possibility to customize folder icons positions (check the ToolboxEditorProject.cs)

### Changed:
- Fix footer position of the ReorderableList in Unity 2019.3+
- Fix handle position of the ReorderableList in Unity 2019.3+
- Fix custom folders scalling 
- Fix object assignment in the InLineEditorAttributeDrawer
- Fix settings reload in the Editor Settings window

## 0.5.5 [09.01.2020]

### Added:
- Tooltip for custom folders
- Possibility to add drawers to hierarchy overlay from external classes (check ToolboxEditorHierarchy)