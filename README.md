# Alley Cats Workflow
- Check the Notion Kanban board to avoid working on something that someone else is already working on.
- Don't work on `main` directly.
- Always pull the latest version of `main` before creating a branch and before merging a branch.
- Use these branch name prefixes:
  - `feature/` for new features
  - `fix/` for bug fixes
  - `level/` for new levels
## Example
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
`git push -u origin feature/your-new-feature`
Once the task is finished, merge it into the latest version of `main` and push the updated `main` branch:
```
git switch main
git pull
git merge feature/your-new-feature
git push origin main
```
After successfully merging, delete the local branch:
`git branch -d feature/your-task-name`
If the branch was previously uploaded, also delete it from GitHub:
`git push origin --delete feature/your-task-name`
