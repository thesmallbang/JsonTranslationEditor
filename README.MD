
# Use at your own risk.

## Slapped this together because I didn't want to pay for BabelEdit but wanted some functionality.

### This program has no real error handling or messages but it will work

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
