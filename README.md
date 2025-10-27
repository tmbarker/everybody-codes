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
   S01
   ├── Q01
   │   ├── p1.txt
   │   ├── p2.txt
   │   └── p3.txt
   ├── Q02
   │   ├── p1.txt
   │   ├── p2.txt
   │   └── p3.txt
   Y2024
   ├── Q01
   │   ├── p1.txt
   │   ├── p2.txt
   │   └── p3.txt
   ├── Q02
   │   ├── p1.txt
   │   ├── p2.txt
   │   └── p3.txt
   ```
   In other words, the input cache directory should have a subdirectory for each story number (named `S<number>`, padded to two digits), and each event year (named `Y<year>`).    
   <br> 
   Each story and year directory should contain a subdirectory for each quest, named `Q<quest>`, padded to two digits. If you don't want to do this manually, there's a CLI command that will stub input files for you once you've completed step (1):
   ```bash
   cd EC
   dotnet run template <series> <quest>
   ```

3. Next, run the desired solution from your terminal:
   ```bash
   cd EC
   dotnet run solve <series> <quest>
   ```
   Note that `<series>` is the event year (e.g., `2024`) or story number (e.g. `1`), and `<quest>` is the quest number (e.g., `1`).

## Automation Commands
To see a full listing of available commands run:
```bash
cd EC
dotnet run --help
```