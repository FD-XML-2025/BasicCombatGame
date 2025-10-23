# Usefull tools
- [Sprite Sheet Generator](https://codeshack.io/images-sprite-sheet-generator/)
- 

# 🚀 Contribution Workflow
To ensure the stability of the master branch and maintain a clear history, please strictly follow this process for all new features or modifications.

## 1. Branch Creation
Never work directly on the master branch.

All new development must begin by creating a new descriptive branch from master. The naming convention is feature-<feature-name>.

### 1. Make sure your local master branch is up-to-date
```
git checkout master
git pull origin master
```

### 2. Create your new branch
```
git checkout -b feature-my-awesome-feature
```

## 2. Development and Commits
Work and make your commits on your feature-... branch.

Be specific in your commit messages. A good commit message explains what you did and (if necessary) why.

Bad commit:
```
git commit -m "Update"
```
Good commit:
```
git commit -m "Feat: Add player collision detection"
```

## 3. Merging into Master
When your feature is complete, tested, and working:

Switch back to the master branch.

Merge your feature branch into master.

Push the updated master branch to the remote repository (GitHub).

Bash

### 1. Go to master
```
git checkout master
```

### 2. Merge your feature
```
git merge feature-my-awesome-feature
```

### 3. Push the changes to GitHub
```
git push origin master
```

### 4. Clean-up
Once your branch has been successfully merged into master, close it (delete it) to keep the repository clean.

Bash

# Delete the local branch
```
git branch -d feature-my-awesome-feature
```

# (Optional) Delete the remote branch if you had pushed it
```
git push origin --delete feature-my-awesome-feature
```

# MGCB Editor
```
dotnet tool install -g dotnet-mgcb
```
dotnet mgcb-editor
```
