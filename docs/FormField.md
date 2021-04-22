# FilesCom.Models.FormField

## Example FormField Object

```
{
  "id": 1,
  "label": "Sample Label",
  "required": true,
  "help_text": "Help Text",
  "field_type": "text",
  "options_for_select": [
    "red",
    "green",
    "blue"
  ],
  "default_option": "red",
  "form_field_set_id": 1
}
```

* `id` / `Id`  (Nullable<Int64>): Form field id
* `label` / `Label`  (string): Label to be displayed
* `required` / `Required`  (bool): Is this a required field?
* `help_text` / `HelpText`  (string): Help text to be displayed
* `field_type` / `FieldType`  (string): Type of Field
* `options_for_select` / `OptionsForSelect`  (string): Options to display for radio and dropdown
* `default_option` / `DefaultOption`  (string): Default option for radio and dropdown
* `form_field_set_id` / `FormFieldSetId`  (Nullable<Int64>): Form field set id
