# everybody-codes

## Running a Solution
1. First, configure your input cache directory in `appsettings.json`:       
   ```json
   {
       "InputCachePath": "<input cache directory path>"
   }
   ```
2. Populate your input cache directory to have the following structure:
   ```bash
   Y2024
   .
   ├── Q01
   │   ├── p1.txt
   │   ├── p2.txt
   │   └── p3.txt
   ├── Q02
   │   ├── p1.txt
   │   ├── p2.txt
   │   └── p3.txt
   ```
   I.e., The input cache directory should have a subdirectory for each event year, named `Y<year>`. In turn, each year directory should contain a subdirectory for each quest, named `Q<quest>`, padded to two digits.

3. Next, run the desired solution from your terminal:
   ```bash
   cd EC
   dotnet run solve <year> <quest>
   ```
