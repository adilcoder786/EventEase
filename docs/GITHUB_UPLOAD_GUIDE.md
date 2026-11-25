# GitHub Upload Guide (5 marks)

## One-time setup
1. Install Git: https://git-scm.com/downloads
2. Create a free account at https://github.com
3. Tell Git who you are:
   ```bash
   git config --global user.name "Your Name"
   git config --global user.email "you@example.com"
   ```

## Create the repository on GitHub
1. GitHub → **New repository**
2. Name: `EventEase`, visibility **Public** (so your reviewer can open it)
3. **Do not** tick "Add README" or ".gitignore" (the project already has them)
4. Click **Create repository** and copy the repository URL.

## Upload your project
Run these in the project folder (the folder containing `EventEase.csproj`):

```bash
git init
git add .
git status                 # check: no bin/, obj/ or .db files should be listed
git commit -m "Initial commit: EventEase event management system"
git branch -M main
git remote add origin https://github.com/<your-username>/EventEase.git
git push -u origin main
```

If asked to sign in, use the browser/Git Credential Manager prompt or a Personal Access Token.
**Never** paste a token or password into any project file.

## Good commit history (shows real step-by-step work)
After the first push, commit each improvement separately, for example:
```bash
git add . && git commit -m "Add screenshots and Copilot summary"
git push
```
Suggested messages: `Add EventCard component`, `Fix routing for /events/{id}`, `Add registration validation`, `Add attendance tracker`.

## Final check
- Open your repository URL in a private/incognito window: README should show.
- Confirm `.gitignore` worked: no `bin`, `obj`, `eventease.db` in the file list.
- Submit the repository URL.
