# Alley Cats

## First time setup
1. Clone the repository with `git clone https://github.com/HarryBouch96/Alley-Cats.git`
2. Open the project folder in VS Code.
3. Install the recommended **CSharpier** extension when VS Code prompts you.
4. Open a terminal in the repository root and run: `dotnet tool restore`

This will make sure we're all using the same code formatter and when you save a file it will automatically be formatted. If VS Code doesn't prompt you to install the **CSharpier** extension, just search for it in the sidebar and install it.
   
## Workflow
- Check the Notion Kanban board to avoid working on something that someone else is already working on.
- Don't work on `main` directly, create a new branch.
- Always pull the latest version of `main` before creating a branch and before merging a branch.
- Use these branch name prefixes:
  - `feature/` for new features
  - `fix/` for bug fixes
  - `level/` for new levels
  
### Example
Before starting a task, pull the latest version of `main` and create a new branch from it:
```
git switch main
git pull
git switch -c feature/your-new-feature
```
To save progress locally:
```
git add .
git commit -m "Describe your changes"
```
To upload an unfinished branch for someone else to work on:
```
git push -u origin feature/your-new-feature
```
Once the task is finished, merge it into the latest version of `main` and push the updated `main` branch:
```
git switch main
git pull
git merge feature/your-new-feature
git push origin main
```
After successfully merging, delete the local branch:
```
git branch -d feature/your-task-name
```
If the branch was previously uploaded, also delete it from GitHub:
```
git push origin --delete feature/your-task-name
```
