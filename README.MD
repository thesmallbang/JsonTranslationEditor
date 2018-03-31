
# Use at your own risk.

## I slapped this together after using the BabelEdit trial and wanted to keep some basic functionality. This is my first time learning WPF so be warned.

### For a more fully featured program you may want to look into [BabelEdit](https://www.codeandweb.com/babeledit)


### This program has no real error handling or friendly messages but should be work assuming your translation files are not too large until I add some optimizations.

### It can read two formats
```json
   {
       "your" : {
           "name" : {
               "space": "value",
               "another":"value"
           },
           "another" : "value"
       }
   }
```

or
```json
   {
       "your.name.space":"value",
       "your.name.another":"value",
       "your.another":"value"
   }
```

## Please note right now it will only save in the last format


![alt text](https://github.com/thesmallbang/JsonTranslationEditor/blob/master/JsonTranslationEditor/Assets/Images/screenshot1.png "Main Window")

## Hotkeys
```
 Open Folder (CTRL+O)
 Save (CTRL+S)
 Reload/Discard Changes (CTRL+R)
 Add Language (CTRL+L)
 Rename (F2)
 Delete (Del)
 Add Translation (CTRL+A)


