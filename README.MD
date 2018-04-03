
# Binary for testing available in the releases section

## I slapped this together after using the [BabelEdit](https://www.codeandweb.com/babeledit) trial and wanted to keep some basic functionality. This is my first time learning WPF so it is fairly ugly both code and UI

### This program has no real error handling or friendly messages but should perform OK even on relatively large files.

### It can load/save two formats
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


![screenshot-mainwindow](https://github.com/thesmallbang/JsonTranslationEditor/blob/master/JsonTranslationEditor/Assets/Images/screenshot1.png "Main Window")

## Hotkeys
```
 Open Folder (CTRL+O)
 Save (CTRL+S)
 Reload/Discard Changes (CTRL+R)
 Add Language (CTRL+L)
 Rename (F2)
 Delete (Del)
 Add Translation (CTRL+A)
 Page Forward (ALT+Right)
 Page Back (Alt+Left)

