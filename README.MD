
# Use at your own risk.

## I slapped this together because I didn't want to pay for BabelEdit but wanted some functionality. I Have not used WPF before but it was much faster than win forms


### This program has no real error handling or messages but has worked for me.

### Have not added a select folder dialog yet so it has to be opened with an argument for your path

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
#### Save (CTRL+S), Reload/Discard Changes (CTRL+R), New Language (CTRL+T)
#### (Selected Item) Rename (F2), Delete (Del), New Translation (CTRL+A)

