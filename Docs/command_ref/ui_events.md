# UI event handlers # {#ui_events}

[TOC]
# Toggle Changed # {#ToggleChanged}
The block will execute when the state of the target UI toggle object changes. The state of the toggle is stored in the Toggle State boolean variable.

Defined in Fungus.ToggleChanged

Property | Type | Description
 --- | --- | ---
Target Toggle | UnityEngine.UI.Toggle | The block will execute when the state of the target UI toggle object changes.
Toggle State | Fungus.BooleanVariable | The new state of the UI toggle object is stored in this boolean variable.
Suppress Block Auto Select | System.Boolean | If true, the flowchart window will not auto select the Block when the Event Handler fires. Affects Editor only.

# End Edit # {#EndEdit}
The block will execute when the user finishes editing the text in the input field.

Defined in Fungus.EndEdit

Property | Type | Description
 --- | --- | ---
Target Input Field | UnityEngine.UI.InputField | The UI Input Field that the user can enter text into
Suppress Block Auto Select | System.Boolean | If true, the flowchart window will not auto select the Block when the Event Handler fires. Affects Editor only.

# Button Clicked # {#ButtonClicked}
The block will execute when the user clicks on the target UI button object.

Defined in Fungus.ButtonClicked

Property | Type | Description
 --- | --- | ---
Target Button | UnityEngine.UI.Button | The UI Button that the user can click on
Suppress Block Auto Select | System.Boolean | If true, the flowchart window will not auto select the Block when the Event Handler fires. Affects Editor only.

Auto-Generated by Fungus.ExportReferenceDocs